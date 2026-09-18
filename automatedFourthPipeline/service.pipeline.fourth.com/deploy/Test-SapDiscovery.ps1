param([string]$BaseUrl = 'https://squaresap.store')
$ErrorActionPreference = 'Stop'
$session = New-Object Microsoft.PowerShell.Commands.WebRequestSession
function Fetch([string]$path) { Invoke-WebRequest -Uri ($BaseUrl + $path) -WebSession $session -TimeoutSec 30 }
function Token($response) {
    $match = [regex]::Match($response.Content, 'name="__RequestVerificationToken"[^>]*value="([^"]+)"')
    if (-not $match.Success) { throw 'Missing antiforgery token.' }
    [Net.WebUtility]::HtmlDecode($match.Groups[1].Value)
}
function Assert($condition, [string]$message) { if (-not $condition) { throw $message }; Write-Output "PASS $message" }
$start = Fetch '/sap/requirements'
$created = Invoke-WebRequest -Uri ($BaseUrl + '/sap/requirements') -Method Post -WebSession $session -Body @{
    __RequestVerificationToken = (Token $start); company = 'DEPLOYMENT TEST SAP 20260917'; contact = 'Integration test'; email = 'integration-test@example.invalid'
}
$codes = [regex]::Matches($created.Content, '<code>([A-Fa-f0-9]+)</code>')
Assert ($codes.Count -eq 2) 'Workspace created with private resume details'
$id = $codes[0].Groups[1].Value
$secret = $codes[1].Groups[1].Value
Write-Output "TEST_DOCUMENT=$id"
$prefix = '/sap/requirements/' + $id
$anonymous = Invoke-WebRequest -Uri ($BaseUrl + $prefix + '/export/json') -SkipHttpErrorCheck -MaximumRedirection 5
Assert (-not $anonymous.Content.Contains('integration-test@example.invalid')) 'Anonymous requests cannot export another project'
$first = Fetch ($prefix + '/0')
Assert ($first.Content.Contains('v0</span>') -and -not $first.Content.Contains('v@d.Revision')) 'Revision badge renders its saved number'
$badCsrf = Invoke-WebRequest -Uri ($BaseUrl + $prefix + '/0') -Method Post -WebSession $session -Body @{actor='test';revision=0;action='save'} -SkipHttpErrorCheck
Assert ($badCsrf.StatusCode -eq 400) 'Save requires antiforgery token'
$review = Fetch ($prefix + '/6')
$incomplete = Invoke-WebRequest -Uri ($BaseUrl + $prefix + '/6') -Method Post -WebSession $session -Body @{
    __RequestVerificationToken=(Token $review); revision=0; actor='Integration test'; action='submit'; confirm='yes'
}
Assert ($incomplete.Content.Contains('is required.')) 'Incomplete submission blocked'
for ($step=0; $step -lt 6; $step++) {
    $page = Fetch ($prefix + '/' + $step)
    $export = (Fetch ($prefix + '/export/json')).Content | ConvertFrom-Json
    $data = @{__RequestVerificationToken=(Token $page); revision=$export.document.Revision; actor='Deployment test / integration'; action='next'}
    # Include conditionally hidden controls: choosing a parent option reveals them in the browser.
    foreach ($match in [regex]::Matches($page.Content, 'name="answers\[([^\]]+)\]"')) {
        $fieldId = $match.Groups[1].Value
        $escaped = [regex]::Escape($fieldId)
        $control = [regex]::Match($page.Content, '<(input|select|textarea)[^>]*name="answers\[' + $escaped + '\]"[^>]*>')
        $value = 'Test requirement - no live posting'
        if ($control.Groups[1].Value -eq 'select') {
            $select = [regex]::Match($page.Content.Substring($control.Index), '^<select.*?</select>', 'Singleline').Value
            $value = [Net.WebUtility]::HtmlDecode([regex]::Match($select, '<option value="([^"]+)"').Groups[1].Value)
        }
        if ($control.Value.Contains('type="number"')) { $value = '10' }
        if ($control.Value.Contains('type="date"')) { $value = '2026-12-31' }
        if ($fieldId.EndsWith('.actions')) { $value = 'None' }
        $data['answers[' + $fieldId + ']'] = $value
    }
    $saved = Invoke-WebRequest -Uri ($BaseUrl + $prefix + '/' + $step) -Method Post -WebSession $session -Body $data
    Assert ($saved.Content.Contains('Status: Draft.')) "Section $step saved"
    if ($step -eq 2) {
        $session = New-Object Microsoft.PowerShell.Commands.WebRequestSession
        $resumeDraft = Fetch '/sap/requirements'
        $draft = Invoke-WebRequest -Uri ($BaseUrl + '/sap/resume') -Method Post -WebSession $session -Body @{
            __RequestVerificationToken=(Token $resumeDraft); reference=$id; code=$secret
        }
        Assert ($draft.Content.Contains('Section 4 / 6')) 'Partially completed draft resumes at the next saved section'
    }
    if ($step -eq 0) {
        $conflict = Invoke-WebRequest -Uri ($BaseUrl + $prefix + '/0') -Method Post -WebSession $session -Body $data -SkipHttpErrorCheck
        Assert ($conflict.StatusCode -eq 409) 'Stale revisions cannot overwrite newer answers'
    }
}
$review = Fetch ($prefix + '/6')
$export = (Fetch ($prefix + '/export/json')).Content | ConvertFrom-Json
Assert ($export.validation.Count -eq 0) 'All required answers validated'
$submitted = Invoke-WebRequest -Uri ($BaseUrl + $prefix + '/6') -Method Post -WebSession $session -Body @{
    __RequestVerificationToken=(Token $review); revision=$export.document.Revision; actor='Deployment test'; action='submit'; confirm='yes'
}
Assert ($submitted.Content.Contains('Status: Submitted.')) 'Completed requirements submitted for review'
$export = (Fetch ($prefix + '/export/json')).Content | ConvertFrom-Json
Assert ($export.history.Count -eq 7) 'Every save and submission has an audit version'
$csv = Fetch ($prefix + '/export/csv')
Assert ($csv.Content.Contains('mapping.products')) 'Mapping and decision CSV export available'
$pdf = Fetch ($prefix + '/export/pdf')
Assert ($pdf.Headers['Content-Type'] -match 'application/pdf') 'PDF download has correct content type'
Assert ([Text.Encoding]::ASCII.GetString([byte[]]$pdf.Content, 0, 5) -eq '%PDF-') 'PDF download contains a real PDF document'
$anonymousPdf = Invoke-WebRequest -Uri ($BaseUrl + $prefix + '/export/pdf') -SkipHttpErrorCheck
Assert ($anonymousPdf.Headers['Content-Type'] -notmatch 'application/pdf') 'Private PDF cannot be downloaded anonymously'
$forgedApproval = Invoke-WebRequest -Uri ($BaseUrl + $prefix + '/6') -Method Post -WebSession $session -Body @{
    __RequestVerificationToken=(Token (Fetch ($prefix + '/6'))); revision=$export.document.Revision; actor='Test'; action='approve'
} -SkipHttpErrorCheck
Assert (-not $forgedApproval.Content.Contains('Status: Approved.')) 'Customer cannot grant administrator approval'
$session = New-Object Microsoft.PowerShell.Commands.WebRequestSession
$resume = Fetch '/sap/requirements'
$wrongCode = Invoke-WebRequest -Uri ($BaseUrl + '/sap/resume') -Method Post -WebSession $session -Body @{
    __RequestVerificationToken=(Token $resume); reference=$id; code='INVALID'
}
Assert ($wrongCode.Content.Contains('reference or access code is incorrect')) 'Incorrect resume code is rejected'
$resumed = Invoke-WebRequest -Uri ($BaseUrl + '/sap/resume') -Method Post -WebSession $session -Body @{
    __RequestVerificationToken=(Token $resume); reference=$id.ToUpperInvariant(); code=$secret.ToLowerInvariant()
}
Assert ($resumed.Content.Contains('DEPLOYMENT TEST SAP 20260917') -and $resumed.Content.Contains('Submitted')) 'Fresh session resumes persisted answers with code'
Assert ($resumed.Content.Contains('Review the whole picture.')) 'Resume returns to last saved review section'
$restored = (Fetch ($prefix + '/export/json')).Content | ConvertFrom-Json
Assert ($restored.document.Revision -eq $export.document.Revision) 'Resume does not change the saved revision'
Assert (($restored.document.Answers | ConvertTo-Json -Compress) -eq ($export.document.Answers | ConvertTo-Json -Compress)) 'All saved answers survive a fresh-session resume'
Write-Output "TEST_DOCUMENT=$id"
