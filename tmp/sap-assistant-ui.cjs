const { chromium } = require('C:/Users/nadav/.cache/codex-runtimes/codex-primary-runtime/dependencies/node/node_modules/playwright');

(async () => {
  const browser = await chromium.launch({ headless: true, executablePath: 'C:/Program Files (x86)/Microsoft/Edge/Application/msedge.exe' });
  const context = await browser.newContext({ viewport: { width: 1440, height: 1000 } });
  const page = await context.newPage();
  await page.goto('http://localhost:5182/sap');
  await page.screenshot({ path: 'C:/Code/updatedChucs/tmp/sap-landing-polished.png', fullPage: true });
  await page.goto('http://localhost:5182/sap/requirements');
  await page.getByLabel('Company / brand').fill('Assistant UI Test');
  await page.getByLabel('Your name').fill('UI tester');
  await page.getByLabel('Work email').fill('ui@example.invalid');
  await page.getByRole('button', { name: 'Create requirements workspace' }).click();
  await page.getByRole('link', { name: /Begin SAP landscape/ }).click();
  if (!await page.getByText('Requirements guide', { exact: true }).isVisible()) throw new Error('Assistant panel is not visible.');
  await page.screenshot({ path: 'C:/Code/updatedChucs/tmp/sap-assistant-desktop.png', fullPage: true });
  await page.setViewportSize({ width: 390, height: 844 });
  const metrics = await page.evaluate(() => ({ viewport: innerWidth, page: document.documentElement.scrollWidth }));
  if (metrics.page > metrics.viewport) throw new Error(`Mobile overflow: ${JSON.stringify(metrics)}`);
  await page.screenshot({ path: 'C:/Code/updatedChucs/tmp/sap-assistant-mobile.png', fullPage: true });
  const revision = await page.locator('.pill').innerText();
  await page.setViewportSize({ width: 1440, height: 1000 });
  await page.goto('http://localhost:5182/Access/Login?returnUrl=%2Fsap%2Fadmin');
  const loginText = await page.locator('body').innerText();
  if (loginText.includes('Square to Fourth')) throw new Error('SAP login still contains Fourth branding.');
  await page.screenshot({ path: 'C:/Code/updatedChucs/tmp/sap-login-polished.png', fullPage: true });
  console.log(JSON.stringify({ revision, metrics, loginTitle: await page.title() }));
  await browser.close();
})().catch(error => { console.error(error); process.exitCode = 1; });
