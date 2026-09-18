const { chromium } = require('C:/Users/nadav/.cache/codex-runtimes/codex-primary-runtime/dependencies/node/node_modules/playwright');

(async () => {
  const browser = await chromium.launch({ headless: true, executablePath: 'C:/Program Files (x86)/Microsoft/Edge/Application/msedge.exe' });
  const context = await browser.newContext({ viewport: { width: 1280, height: 900 }, ignoreHTTPSErrors: true });
  const page = await context.newPage();
  await page.goto('http://localhost:5000/sap/requirements');
  await page.getByLabel('Company / brand').fill('Suggestion UI Test');
  await page.getByLabel('Your name').fill('UI tester');
  await page.getByLabel('Work email').fill('ui@example.invalid');
  await page.getByRole('button', { name: 'Create requirements workspace' }).click();
  await page.getByRole('link', { name: /Begin SAP landscape/ }).click();

  await page.route('**/assistant', route => route.fulfill({
    status: 200,
    contentType: 'application/json',
    body: JSON.stringify({
      answer: 'The selected product must match the customer SAP landscape.',
      findings: [{ kind: 'conflict', fieldId: 'sap.product', text: 'The supplied workshop evidence indicates S/4HANA, not ECC.' }],
      followUpQuestions: [],
      warnings: [],
      suggestions: [{ fieldId: 'sap.product', suggestedValue: 'S/4HANA', rationale: 'Use the product confirmed by the SAP platform owner.' }]
    })
  }));

  const product = page.locator('[id="sap.product"]');
  await product.selectOption('ECC');
  await page.locator('[data-assistant-field="sap.product"]').click();
  await page.getByText('The selected product must match the customer SAP landscape.').waitFor();
  const use = page.getByRole('button', { name: 'Use this draft' });
  await use.click();
  if (await product.inputValue() !== 'ECC') throw new Error('Existing value changed before replacement confirmation.');
  await page.getByRole('button', { name: 'Confirm replace' }).click();
  if (await product.inputValue() !== 'S/4HANA') throw new Error('Confirmed suggestion was not applied.');
  if (await page.locator('[data-assistant-fields]').inputValue() !== 'sap.product') throw new Error('AI-assisted field was not attributed.');
  await page.getByRole('button', { name: 'Undo' }).click();
  if (await product.inputValue() !== 'ECC') throw new Error('Undo did not restore the prior value.');
  if (await page.locator('[data-assistant-fields]').inputValue() !== '') throw new Error('Undo did not remove AI attribution.');

  await page.getByRole('button', { name: 'Replace with this draft' }).click();
  await page.getByRole('button', { name: 'Confirm replace' }).click();
  await page.getByRole('button', { name: 'Save draft' }).click();
  await page.getByRole('link', { name: 'Review & submit' }).click();
  if (!(await page.locator('body').innerText()).includes('AI-assisted drafts accepted: sap.product')) throw new Error('Saved revision did not retain AI field attribution.');

  console.log(JSON.stringify({ replacementConfirmed: true, undoRestored: true, auditAttribution: true, revisionAttribution: true }));
  await browser.close();
})().catch(error => { console.error(error); process.exitCode = 1; });
