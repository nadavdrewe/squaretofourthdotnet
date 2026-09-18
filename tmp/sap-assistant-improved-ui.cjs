const { chromium } = require('C:/Users/nadav/.cache/codex-runtimes/codex-primary-runtime/dependencies/node/node_modules/playwright');

(async () => {
  const browser = await chromium.launch({ headless: true, executablePath: 'C:/Program Files (x86)/Microsoft/Edge/Application/msedge.exe' });
  const context = await browser.newContext({ viewport: { width: 1440, height: 1000 }, ignoreHTTPSErrors: true });
  const page = await context.newPage();
  await page.goto('http://localhost:5000/sap/requirements');
  await page.getByLabel('Company / brand').fill('Improved Assistant UI Test');
  await page.getByLabel('Your name').fill('UI tester');
  await page.getByLabel('Work email').fill('ui@example.invalid');
  await page.getByRole('button', { name: 'Create requirements workspace' }).click();
  await page.getByRole('link', { name: /Begin SAP landscape/ }).click();

  const fieldHelpCount = await page.locator('[data-assistant-field]').count();
  if (fieldHelpCount < 5) throw new Error(`Expected field-level help controls, found ${fieldHelpCount}.`);
  if (await page.locator('[data-assistant-fields]').count() !== 1) throw new Error('Missing AI-assisted audit field.');

  await page.locator('[data-assistant-field]').first().click();
  await page.getByText('Form guide', { exact: true }).waitFor({ timeout: 20000 });
  const guideText = await page.locator('[data-assistant-feed]').innerText();
  if (!guideText.includes('Live guidance is temporarily unavailable')) throw new Error('Deterministic fallback was not rendered.');
  if (!guideText.includes('Which SAP product')) throw new Error('Field-focused fallback did not explain the selected question.');

  const desktop = await page.evaluate(() => ({ viewport: innerWidth, page: document.documentElement.scrollWidth }));
  if (desktop.page > desktop.viewport) throw new Error(`Desktop overflow: ${JSON.stringify(desktop)}`);
  await page.screenshot({ path: 'C:/Code/updatedChucs/tmp/sap-assistant-improved-desktop.png', fullPage: true });

  await page.setViewportSize({ width: 390, height: 844 });
  const mobile = await page.evaluate(() => ({ viewport: innerWidth, page: document.documentElement.scrollWidth }));
  if (mobile.page > mobile.viewport) throw new Error(`Mobile overflow: ${JSON.stringify(mobile)}`);
  await page.screenshot({ path: 'C:/Code/updatedChucs/tmp/sap-assistant-improved-mobile.png', fullPage: true });
  console.log(JSON.stringify({ fieldHelpCount, desktop, mobile, fallback: true }));
  await browser.close();
})().catch(error => { console.error(error); process.exitCode = 1; });
