from pathlib import Path

from reportlab.lib import colors
from reportlab.lib.pagesizes import A4
from reportlab.lib.styles import ParagraphStyle
from reportlab.lib.utils import ImageReader
from reportlab.lib.units import mm
from reportlab.pdfbase import pdfmetrics
from reportlab.pdfbase.ttfonts import TTFont
from reportlab.pdfgen import canvas
from reportlab.platypus import Paragraph


ROOT = Path(r"C:\Code\updatedChucs")
ASSETS = ROOT / "tmp" / "pdfs" / "assets"
OUTPUT = ROOT / "output" / "pdf" / "square-to-fourth-workforce-onboarding-brochure-final.pdf"
OUTPUT.parent.mkdir(parents=True, exist_ok=True)

SQUARE_WHITE = ASSETS / "square-logo" / "White" / "Square_Logo_2025_White.png"
SQUARE_BLACK = ASSETS / "square-logo" / "Black" / "Square_Logo_2025_Black.png"
FOURTH_WHITE = ASSETS / "fourth-primary-white.png"
FOURTH_BLUE = ASSETS / "fourth-primary-blue.png"
PORTAL_SCREEN = ASSETS / "squaretofourth-portal.png"
ONBOARD_SCREEN = ASSETS / "squaretofourth-onboarding.png"

for asset in [SQUARE_WHITE, SQUARE_BLACK, FOURTH_WHITE, FOURTH_BLUE, PORTAL_SCREEN, ONBOARD_SCREEN]:
    if not asset.exists():
        raise FileNotFoundError(asset)

W, H = A4
INK = colors.HexColor("#0B1714")
DEEP = colors.HexColor("#102720")
GREEN = colors.HexColor("#00A86B")
FOREST = colors.HexColor("#087A52")
LIME = colors.HexColor("#9DE7BF")
MINT = colors.HexColor("#E5F5ED")
CREAM = colors.HexColor("#F6F2E9")
PAPER = colors.HexColor("#FEFCF8")
FOURTH = colors.HexColor("#00538B")
BLUE = colors.HexColor("#1D74A7")
ORANGE = colors.HexColor("#F07842")
MUTED = colors.HexColor("#60706A")
LINE = colors.HexColor("#D5DFDA")
WHITE = colors.white

pdfmetrics.registerFont(TTFont("Display", r"C:\Windows\Fonts\bahnschrift.ttf"))
pdfmetrics.registerFont(TTFont("Body", r"C:\Windows\Fonts\segoeui.ttf"))
pdfmetrics.registerFont(TTFont("BodySemi", r"C:\Windows\Fonts\seguisb.ttf"))
pdfmetrics.registerFont(TTFont("BodyBold", r"C:\Windows\Fonts\segoeuib.ttf"))


def ps(name, font="Body", size=10, leading=14, color=INK, **kwargs):
    return ParagraphStyle(name, fontName=font, fontSize=size, leading=leading, textColor=color, **kwargs)


STYLES = {
    "cover_title": ps("cover_title", "Display", 41, 43, WHITE),
    "cover_sub": ps("cover_sub", "Body", 13, 18, colors.HexColor("#D5E2DD")),
    "title": ps("title", "Display", 28, 31, INK),
    "title_white": ps("title_white", "Display", 28, 31, WHITE),
    "subtitle": ps("subtitle", "Body", 11, 16, MUTED),
    "subtitle_white": ps("subtitle_white", "Body", 10.5, 15, colors.HexColor("#C9D8D2")),
    "body": ps("body", "Body", 9, 13, INK),
    "body_muted": ps("body_muted", "Body", 8.5, 12, MUTED),
    "body_white": ps("body_white", "Body", 8.5, 12, colors.HexColor("#DCE7E3")),
    "small": ps("small", "Body", 7.2, 9.5, MUTED),
    "small_white": ps("small_white", "Body", 7.2, 9.5, colors.HexColor("#BFD0C9")),
    "label": ps("label", "BodySemi", 7.2, 9, GREEN),
    "label_white": ps("label_white", "BodySemi", 7.2, 9, LIME),
    "card_title": ps("card_title", "BodyBold", 10, 12, INK),
    "card_title_white": ps("card_title_white", "BodyBold", 10, 12, WHITE),
    "price": ps("price", "Display", 26, 28, INK),
    "price_white": ps("price_white", "Display", 26, 28, WHITE),
}


def para(c, text, x, top, width, style="body", max_height=500):
    p = Paragraph(text, STYLES[style])
    _, height = p.wrap(width, max_height)
    p.drawOn(c, x, top - height)
    return height


def image_fit(c, path, x, y, width, height, contain=True):
    reader = ImageReader(str(path))
    iw, ih = reader.getSize()
    scale = min(width / iw, height / ih) if contain else max(width / iw, height / ih)
    dw, dh = iw * scale, ih * scale
    dx, dy = x + (width - dw) / 2, y + (height - dh) / 2
    c.drawImage(reader, dx, dy, dw, dh, preserveAspectRatio=True, mask="auto")


def rounded(c, x, y, width, height, fill, radius=10, stroke=None, line_width=0.7):
    c.setFillColor(fill)
    c.setStrokeColor(stroke or fill)
    c.setLineWidth(line_width)
    c.roundRect(x, y, width, height, radius, fill=1, stroke=1 if stroke else 0)


def pill(c, text, x, y, width, fill, text_color=INK):
    rounded(c, x, y, width, 20, fill, 10)
    c.setFont("BodySemi", 7.2)
    c.setFillColor(text_color)
    c.drawCentredString(x + width / 2, y + 6.6, text)


def draw_logos(c, x, y, dark=False, scale=1.0):
    square = SQUARE_WHITE if dark else SQUARE_BLACK
    fourth = FOURTH_WHITE if dark else FOURTH_BLUE
    c.drawImage(str(square), x, y, 80 * scale, 21 * scale, preserveAspectRatio=True, mask="auto")
    c.setFont("BodySemi", 8 * scale)
    c.setFillColor(WHITE if dark else INK)
    c.drawString(x + 91 * scale, y + 7 * scale, "+")
    c.drawImage(str(fourth), x + 111 * scale, y - 1 * scale, 82 * scale, 23 * scale, preserveAspectRatio=True, mask="auto")


def draw_header(c, page, dark=False):
    draw_logos(c, 40, H - 55, dark=dark, scale=0.72)
    c.setFont("BodySemi", 6.6)
    c.setFillColor(colors.HexColor("#BFD0C9") if dark else MUTED)
    c.drawRightString(W - 40, H - 45, "WORKFORCE INTEGRATION  /  CLIENT ONBOARDING")
    c.setStrokeColor(colors.HexColor("#335047") if dark else LINE)
    c.setLineWidth(0.6)
    c.line(40, H - 67, W - 40, H - 67)
    c.line(40, 29, W - 40, 29)
    c.setFont("Body", 6.4)
    c.drawString(40, 18, "SQUARE TO FOURTH")
    c.drawRightString(W - 40, 18, f"{page:02d} / 04")


def browser_frame(c, path, x, y, width, height, dark=False):
    c.saveState()
    c.setFillAlpha(0.12)
    rounded(c, x + 6, y - 7, width, height, colors.black, 12)
    c.restoreState()
    rounded(c, x, y, width, height, WHITE, 12, stroke=colors.HexColor("#CBD7D2"))
    c.setFillColor(colors.HexColor("#F1F3F2"))
    c.roundRect(x, y + height - 24, width, 24, 12, fill=1, stroke=0)
    c.rect(x, y + height - 24, width, 12, fill=1, stroke=0)
    for index, color in enumerate([ORANGE, colors.HexColor("#F1BE4B"), GREEN]):
        c.setFillColor(color)
        c.circle(x + 15 + index * 12, y + height - 12, 3.2, fill=1, stroke=0)
    c.setFillColor(colors.HexColor("#D9DFDC"))
    c.roundRect(x + 58, y + height - 17, width - 75, 10, 5, fill=1, stroke=0)
    image_fit(c, path, x + 10, y + 10, width - 20, height - 43, contain=True)


def dashboard_frame(c, x, y, width, height):
    c.saveState()
    c.setFillAlpha(0.09)
    rounded(c, x + 5, y - 5, width, height, colors.black, 12)
    c.restoreState()
    rounded(c, x, y, width, height, WHITE, 12, stroke=colors.HexColor("#CBD7D2"))
    c.setFillColor(colors.HexColor("#F1F3F2"))
    c.roundRect(x, y + height - 24, width, 24, 12, fill=1, stroke=0)
    c.rect(x, y + height - 24, width, 12, fill=1, stroke=0)
    for index, color in enumerate([ORANGE, colors.HexColor("#F1BE4B"), GREEN]):
        c.setFillColor(color)
        c.circle(x + 15 + index * 12, y + height - 12, 3.2, fill=1, stroke=0)
    c.setFillColor(colors.HexColor("#D9DFDC"))
    c.roundRect(x + 58, y + height - 17, width - 75, 10, 5, fill=1, stroke=0)

    c.setFillColor(CREAM)
    c.rect(x + 10, y + 10, width - 20, height - 43, fill=1, stroke=0)
    draw_logos(c, x + 26, y + height - 61, dark=False, scale=0.48)
    c.setFont("BodySemi", 6.5)
    c.setFillColor(MUTED)
    c.drawRightString(x + width - 25, y + height - 50, "INTEGRATION CONTROL")
    c.setStrokeColor(LINE)
    c.line(x + 25, y + height - 72, x + width - 25, y + height - 72)

    para(c, "Workforce transfer", x + 26, y + height - 88, 210, "card_title")
    para(c, "Square timecards to Fourth, mapped and monitored.", x + 26, y + height - 106, 235, "body_muted")
    pill(c, "CONNECTED", x + width - 112, y + height - 108, 82, MINT, FOREST)

    # Keep the visualisation clear of the dashboard heading at every render scale.
    chart_x, chart_y, chart_w, chart_h = x + 25, y + 18, 274, 82
    rounded(c, chart_x, chart_y, chart_w, chart_h, WHITE, 8, stroke=LINE)
    c.setFont("BodySemi", 6.6)
    c.setFillColor(MUTED)
    c.drawString(chart_x + 13, chart_y + chart_h - 17, "RECORDS RECEIVED / EXPORTED")
    c.setFillColor(GREEN)
    c.circle(chart_x + 184, chart_y + chart_h - 14, 2.8, fill=1, stroke=0)
    c.setFillColor(MUTED)
    c.drawString(chart_x + 190, chart_y + chart_h - 17, "Square")
    c.setFillColor(FOURTH)
    c.circle(chart_x + 229, chart_y + chart_h - 14, 2.8, fill=1, stroke=0)
    c.setFillColor(MUTED)
    c.drawString(chart_x + 235, chart_y + chart_h - 17, "Fourth")
    c.setStrokeColor(colors.HexColor("#E4EAE7"))
    for line_index in range(3):
        line_y = chart_y + 20 + line_index * 17
        c.line(chart_x + 14, line_y, chart_x + chart_w - 14, line_y)
    received = [20, 31, 26, 39, 34, 46, 43]
    exported = [19, 30, 26, 38, 34, 45, 43]
    for index, (incoming, outgoing) in enumerate(zip(received, exported)):
        bx = chart_x + 23 + index * 34
        c.setFillColor(GREEN)
        c.roundRect(bx, chart_y + 12, 7, incoming, 2, fill=1, stroke=0)
        c.setFillColor(FOURTH)
        c.roundRect(bx + 9, chart_y + 12, 7, outgoing, 2, fill=1, stroke=0)

    gauge_x, gauge_y, gauge_w, gauge_h = x + 311, y + 58, 169, 42
    rounded(c, gauge_x, gauge_y, gauge_w, gauge_h, WHITE, 8, stroke=LINE)
    c.setLineWidth(4.4)
    c.setStrokeColor(colors.HexColor("#DDE8E3"))
    c.circle(gauge_x + 25, gauge_y + 21, 12, fill=0, stroke=1)
    c.setStrokeColor(GREEN)
    c.arc(gauge_x + 13, gauge_y + 9, gauge_x + 37, gauge_y + 33, 40, 300)
    c.setFont("BodySemi", 6.6)
    c.setFillColor(MUTED)
    c.drawString(gauge_x + 48, gauge_y + 27, "RECONCILIATION")
    c.setFont("Display", 10.5)
    c.setFillColor(INK)
    c.drawString(gauge_x + 48, gauge_y + 11, "Counts matched")

    trend_x, trend_y, trend_w, trend_h = x + 311, y + 18, 169, 32
    rounded(c, trend_x, trend_y, trend_w, trend_h, WHITE, 8, stroke=LINE)
    c.setFont("BodySemi", 6.4)
    c.setFillColor(MUTED)
    c.drawString(trend_x + 12, trend_y + 20, "RUN HEALTH")
    points = [(trend_x + 76, trend_y + 9), (trend_x + 92, trend_y + 13), (trend_x + 108, trend_y + 11), (trend_x + 124, trend_y + 17), (trend_x + 140, trend_y + 15), (trend_x + 156, trend_y + 21)]
    path = c.beginPath()
    path.moveTo(*points[0])
    for px, py in points[1:]:
        path.lineTo(px, py)
    c.setStrokeColor(GREEN)
    c.setLineWidth(1.8)
    c.drawPath(path, fill=0, stroke=1)
    for px, py in points:
        c.setFillColor(GREEN)
        c.circle(px, py, 2.2, fill=1, stroke=0)
    c.setFont("BodySemi", 6.4)
    c.setFillColor(FOREST)
    c.drawString(trend_x + 12, trend_y + 7, "Healthy")


def numbered_card(c, number, title, body, x, y, width, height, dark=False, accent=GREEN):
    bg = colors.HexColor("#17332B") if dark else WHITE
    border = colors.HexColor("#2E4B42") if dark else LINE
    rounded(c, x, y, width, height, bg, 9, stroke=border)
    c.setFillColor(accent)
    c.roundRect(x + 12, y + height - 31, 30, 20, 10, fill=1, stroke=0)
    c.setFont("BodyBold", 8)
    c.setFillColor(INK if accent == LIME else WHITE)
    c.drawCentredString(x + 27, y + height - 24, number)
    para(c, title, x + 53, y + height - 12, width - 65, "card_title_white" if dark else "card_title")
    para(c, body, x + 14, y + height - 43, width - 28, "body_white" if dark else "body_muted")


def cover(c):
    c.setFillColor(INK)
    c.rect(0, 0, W, H, fill=1, stroke=0)

    c.setFillColor(DEEP)
    c.circle(W + 15, H - 130, 205, fill=1, stroke=0)
    c.setFillColor(FOURTH)
    path = c.beginPath()
    path.moveTo(W - 145, H)
    path.lineTo(W, H)
    path.lineTo(W, H - 315)
    path.close()
    c.drawPath(path, fill=1, stroke=0)
    c.setFillColor(GREEN)
    path = c.beginPath()
    path.moveTo(W - 92, H)
    path.lineTo(W, H)
    path.lineTo(W, H - 193)
    path.close()
    c.drawPath(path, fill=1, stroke=0)

    draw_logos(c, 42, H - 67, dark=True, scale=0.9)
    c.setFont("BodySemi", 7.2)
    c.setFillColor(colors.HexColor("#D6E3DE"))
    c.drawRightString(W - 42, H - 52, "CLIENT ONBOARDING / 2026")

    pill(c, "ALREADY BUILT  /  LIVE  /  PROVEN", 42, H - 145, 210, LIME)
    para(c, "Square to Fourth<br/>Workforce Integration", 42, H - 182, 330, "cover_title")
    para(c, "The integration already exists and is proven in live operation at <b>squaretofourth.store</b>. Client onboarding covers Square OAuth, Fourth configuration, mapping and scheduled delivery.", 42, H - 334, 315, "cover_sub")

    rounded(c, 385, H - 356, 162, 186, colors.HexColor("#142C25"), 16, stroke=colors.HexColor("#315147"))
    c.setStrokeColor(colors.HexColor("#5E8C7C"))
    c.setLineWidth(1.4)
    c.line(466, H - 222, 466, H - 304)
    c.setFillColor(LIME)
    c.circle(466, H - 222, 7, fill=1, stroke=0)
    c.circle(466, H - 263, 7, fill=1, stroke=0)
    c.circle(466, H - 304, 7, fill=1, stroke=0)
    c.setFont("BodySemi", 7.4)
    c.setFillColor(WHITE)
    c.drawString(486, H - 225, "AUTHORISE")
    c.drawString(486, H - 266, "MAP")
    c.drawString(486, H - 307, "TRANSFER")
    c.setFont("Body", 6.6)
    c.setFillColor(colors.HexColor("#AFC4BC"))
    c.drawString(486, H - 236, "Square OAuth")
    c.drawString(486, H - 277, "People + sites")
    c.drawString(486, H - 318, "Fourth + audit")
    c.setFillColor(GREEN)
    c.rect(385, H - 356, 5, 186, fill=1, stroke=0)

    c.setStrokeColor(colors.HexColor("#29443B"))
    c.line(42, 360, W - 42, 360)
    c.setFont("BodySemi", 7.2)
    c.setFillColor(LIME)
    c.drawString(42, 341, "ONBOARDING, END TO END")

    numbered_card(c, "01", "Connect", "Create the client workspace and complete Square OAuth authorisation.", 42, 174, 158, 142, dark=True, accent=LIME)
    numbered_card(c, "02", "Configure", "Set the Fourth connection, locations and employee mappings.", 212, 174, 158, 142, dark=True, accent=FOURTH)
    numbered_card(c, "03", "Launch", "Reconcile the first transfer, record sign-off and enable scheduling.", 382, 174, 158, 142, dark=True, accent=ORANGE)
    c.setStrokeColor(colors.HexColor("#5E8C7C"))
    c.setLineWidth(1.2)
    c.line(201, 295, 211, 295)
    c.line(371, 295, 381, 295)

    c.setFont("BodySemi", 7.4)
    c.setFillColor(WHITE)
    c.drawString(42, 67, "LIVE PLATFORM / SQUARETOFOURTH.STORE")
    c.setFont("Body", 6.8)
    c.setFillColor(colors.HexColor("#AFC4BC"))
    c.drawString(42, 51, "The established integration is ready for client onboarding.")
    c.setFont("BodySemi", 7.4)
    c.setFillColor(WHITE)
    c.drawRightString(W - 42, 67, "CLIENT ACCESS / SECURE PORTAL")
    c.setFont("Body", 6.8)
    c.setFillColor(colors.HexColor("#AFC4BC"))
    c.drawRightString(W - 42, 51, "OAuth handled by Square. No password sharing.")
    c.showPage()


def platform_page(c):
    c.setFillColor(CREAM)
    c.rect(0, 0, W, H, fill=1, stroke=0)
    c.setFillColor(colors.HexColor("#EAF4EF"))
    c.circle(W + 28, H - 205, 118, fill=1, stroke=0)
    c.setFillColor(colors.HexColor("#EDF4F7"))
    path = c.beginPath()
    path.moveTo(W - 40, H)
    path.lineTo(W, H)
    path.lineTo(W, H - 165)
    path.close()
    c.drawPath(path, fill=1, stroke=0)
    draw_header(c, 2)
    pill(c, "HOW IT WORKS", 40, H - 112, 84, MINT, FOREST)
    para(c, "Controlled daily transfer.", 40, H - 139, 470, "title")
    para(c, "At squaretofourth.store, the service connects Square, confirms the mappings and exposes the operational signals needed to monitor every transfer.", 40, H - 180, 500, "subtitle")

    dashboard_frame(c, 40, 360, 515, 228)
    c.setFont("BodySemi", 7.2)
    c.setFillColor(MUTED)
    c.drawRightString(555, 346, "LIVE OPERATIONS  /  SQUARETOFOURTH.STORE")

    card_y = 190
    numbered_card(c, "01", "Authorise", "A Square seller administrator signs in on Square's own OAuth screen and approves read-only access.", 40, card_y, 162, 122)
    numbered_card(c, "02", "Map", "Square locations and team members are matched to Fourth site codes and EmployeeNumber values.", 216, card_y, 162, 122, accent=FOURTH)
    numbered_card(c, "03", "Transfer", "Completed timecards are transformed, uploaded and retained with a clear audit result.", 392, card_y, 163, 122, accent=ORANGE)

    rounded(c, 40, 76, 515, 94, INK, 12)
    pill(c, "READ-ONLY OAUTH", 55, 135, 102, LIME, INK)
    para(c, "No Square password is shared", 55, 127, 240, "card_title_white")
    para(c, "The portal stores renewable OAuth tokens for <b>TIMECARDS_READ</b> and <b>EMPLOYEES_READ</b>. The seller's login stays with Square.", 55, 108, 310, "body_white")
    c.setStrokeColor(colors.HexColor("#3D5A50"))
    c.line(390, 92, 390, 151)
    c.setFont("Display", 20)
    c.setFillColor(LIME)
    c.drawString(410, 119, "SECURE")
    c.setFont("BodySemi", 7)
    c.setFillColor(colors.HexColor("#BFD0C9"))
    c.drawString(410, 103, "AUTHORISED ACCESS")
    c.showPage()


def onboarding_page(c):
    c.setFillColor(INK)
    c.rect(0, 0, W, H, fill=1, stroke=0)
    c.setFillColor(DEEP)
    c.circle(W + 48, H - 210, 235, fill=1, stroke=0)
    draw_header(c, 3, dark=True)
    pill(c, "ONBOARDING PACK", 40, H - 112, 102, colors.HexColor("#24483C"), LIME)
    para(c, "What onboarding includes.", 40, H - 139, 370, "title_white")
    para(c, "We configure the existing live integration from initial access through to a reconciled first transfer and operational handover.", 40, H - 180, 485, "subtitle_white")

    requirements = [
        ("01", "Client workspace", "Create the client brand, invitation and operator access."),
        ("02", "Square authorisation", "Complete OAuth and store renewable read-only tokens."),
        ("03", "Fourth configuration", "Set the endpoint, credentials, organisation and sites."),
        ("04", "Data mapping", "Match locations and team members to Fourth identifiers."),
        ("05", "Validation + launch", "Reconcile the first transfer, sign off and schedule."),
    ]
    y = 558
    for number, title, body in requirements:
        numbered_card(c, number, title, body, 40, y, 250, 66, dark=True, accent=LIME)
        y -= 76

    browser_frame(c, ONBOARD_SCREEN, 315, 430, 240, 194, dark=True)
    pill(c, "CUSTOMER PROVIDES", 330, 404, 108, colors.HexColor("#24483C"), LIME)
    rounded(c, 315, 218, 240, 171, colors.HexColor("#17332B"), 10, stroke=colors.HexColor("#315047"))
    customer_items = [
        "Square seller administrator",
        "Fourth endpoint and credentials",
        "Organisation and site identifiers",
        "EmployeeNumber mapping export",
        "Completed day and validation contact",
    ]
    item_y = 365
    for item in customer_items:
        c.setFillColor(LIME)
        c.circle(334, item_y + 3, 3.2, fill=1, stroke=0)
        para(c, item, 347, item_y + 9, 188, "body_white")
        item_y -= 29

    c.setFont("BodySemi", 7.2)
    c.setFillColor(LIME)
    c.drawString(40, 190, "ONBOARDING IS COMPLETE WHEN")
    numbered_card(c, "01", "Connected", "Square and Fourth access is verified for the client.", 40, 75, 165, 98, dark=True, accent=GREEN)
    numbered_card(c, "02", "Reconciled", "The first day's people, timecards and worked hours match.", 215, 75, 165, 98, dark=True, accent=FOURTH)
    numbered_card(c, "03", "Running", "Fourth accepts the upload and recurring delivery is enabled.", 390, 75, 165, 98, dark=True, accent=ORANGE)
    c.showPage()


def pricing_page(c):
    c.setFillColor(PAPER)
    c.rect(0, 0, W, H, fill=1, stroke=0)
    c.setFillColor(MINT)
    c.circle(W + 18, H - 80, 150, fill=1, stroke=0)
    draw_header(c, 4)
    pill(c, "COMMERCIALS", 40, H - 112, 88, MINT, FOREST)
    para(c, "One service. Two ways to pay.", 40, H - 139, 410, "title")
    para(c, "Choose a discounted all-in bundle or monthly billing for the same 12-month integration service.", 40, H - 180, 470, "subtitle")

    rounded(c, 40, 497, 250, 148, WHITE, 13, stroke=LINE)
    c.setStrokeColor(GREEN)
    c.setLineWidth(2.8)
    c.line(58, 641, 272, 641)
    pill(c, "SINGLE LOCATION", 56, 610, 100, MINT, FOREST)
    para(c, "£1,795", 56, 590, 130, "price")
    c.setFont("BodySemi", 7.5)
    c.setFillColor(FOREST)
    c.drawString(162, 567, "ONBOARDING + 12 MONTHS")
    c.setStrokeColor(LINE)
    c.line(56, 548, 274, 548)
    c.setFont("BodySemi", 9)
    c.setFillColor(INK)
    c.drawString(56, 526, "£149.58 per location / month equivalent")
    c.setFont("Body", 7.6)
    c.setFillColor(MUTED)
    c.drawString(56, 507, "Monthly option: £495 + 12 x £149 (£2,283 total)")

    rounded(c, 305, 497, 250, 148, INK, 13)
    c.setStrokeColor(LIME)
    c.setLineWidth(2.8)
    c.line(323, 641, 537, 641)
    pill(c, "8 LOCATIONS", 321, 610, 108, colors.HexColor("#24483C"), LIME)
    c.setFont("BodySemi", 6.8)
    c.setFillColor(LIME)
    c.drawRightString(539, 618, "BEST VALUE")
    style_price_white = ps("price_white", "Display", 26, 28, WHITE)
    temp = Paragraph("£2,999", style_price_white)
    _, ph = temp.wrap(135, 50)
    temp.drawOn(c, 321, 590 - ph)
    c.setFont("BodySemi", 7.5)
    c.setFillColor(LIME)
    c.drawString(438, 567, "ONBOARDING + 12 MONTHS")
    c.setStrokeColor(colors.HexColor("#38544B"))
    c.line(321, 548, 539, 548)
    c.setFont("BodySemi", 9)
    c.setFillColor(WHITE)
    c.drawString(321, 526, "£31.24 per location / month equivalent")
    c.setFont("Body", 7.6)
    c.setFillColor(colors.HexColor("#C1D1CB"))
    c.drawString(321, 507, "Monthly option: £1,250 + 12 x £295 (£4,790 total)")

    c.setFont("BodySemi", 7.2)
    c.setFillColor(MUTED)
    c.drawCentredString(W / 2, 477, "UPFRONT BY INVOICE / BANK TRANSFER  /  MONTHLY BY STRIPE OR BANK TRANSFER")

    c.setFont("BodySemi", 7.2)
    c.setFillColor(FOREST)
    c.drawString(40, 449, "EVERY 12-MONTH PLAN INCLUDES")
    service_items = [
        ("Secure hosting", "Managed integration runtime"),
        ("Token lifecycle", "Secure storage and refresh"),
        ("Scheduled transfer", "Automated daily delivery"),
        ("Run monitoring", "Counts, health and exceptions"),
        ("Audit history", "Payloads and responses"),
        ("Standard support", "Operational assistance"),
    ]
    for index, (title, detail) in enumerate(service_items):
        row, column = divmod(index, 3)
        sx = 40 + column * 175
        sy = 362 - row * 74
        service_tints = [colors.HexColor("#F1FAF6"), colors.HexColor("#F0F6FA"), colors.HexColor("#FFF5F0")]
        rounded(c, sx, sy, 165, 62, service_tints[column], 9, stroke=LINE)
        c.setFillColor([GREEN, FOURTH, ORANGE][column])
        c.circle(sx + 20, sy + 31, 8, fill=1, stroke=0)
        c.setFont("BodyBold", 7)
        c.setFillColor(WHITE)
        c.drawCentredString(sx + 20, sy + 28.5, f"{index + 1:02d}")
        para(c, title, sx + 36, sy + 49, 116, "card_title")
        para(c, detail, sx + 36, sy + 29, 116, "small")

    rounded(c, 40, 145, 515, 118, INK, 11)
    payment_columns = [
        ("PAY MONTHLY", "Recurring Stripe subscription or an agreed bank-transfer schedule."),
        ("PAY UPFRONT", "One annual invoice by Stripe or bank transfer, with the stated discount."),
        ("BESPOKE CHANGES", "£600/day, only when work falls outside the established integration."),
    ]
    for index, (title, detail) in enumerate(payment_columns):
        px = 55 + index * 168
        if index:
            c.setStrokeColor(colors.HexColor("#365149"))
            c.line(px - 15, 162, px - 15, 246)
        c.setFont("BodySemi", 7.1)
        c.setFillColor(LIME if index < 2 else colors.HexColor("#F6A17A"))
        c.drawString(px, 230, title)
        para(c, detail, px, 214, 140, "body_white")

    rounded(c, 40, 79, 515, 44, MINT, 8)
    para(c, "Commercial scope", 55, 110, 95, "card_title")
    para(c, "12-month agreement; prices exclude VAT and vendor fees. The eight-location plan covers the eight sites agreed at onboarding; additions are quoted first.", 155, 111, 380, "body_muted")

    rounded(c, 40, 33, 515, 35, INK, 8)
    c.setFont("BodySemi", 7.2)
    c.setFillColor(LIME)
    c.drawString(55, 47, "NEXT STEP")
    c.setFont("BodySemi", 9)
    c.setFillColor(WHITE)
    c.drawString(127, 45.5, "Nominate the Square and Fourth contacts, then send the mapping pack.")
    c.showPage()


pdf = canvas.Canvas(str(OUTPUT), pagesize=A4, pageCompression=1)
pdf.setTitle("Square to Fourth Workforce Integration - Customer Onboarding Brochure")
pdf.setAuthor("Square to Fourth Integration")
pdf.setSubject("Live Square to Fourth Workforce integration onboarding")
cover(pdf)
platform_page(pdf)
onboarding_page(pdf)
pricing_page(pdf)
pdf.save()
print(OUTPUT)
