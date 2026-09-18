from pathlib import Path

from reportlab.graphics.barcode.qr import QrCodeWidget
from reportlab.graphics.shapes import Drawing
from reportlab.lib import colors
from reportlab.lib.enums import TA_CENTER, TA_LEFT, TA_RIGHT
from reportlab.lib.pagesizes import A4
from reportlab.lib.styles import ParagraphStyle, getSampleStyleSheet
from reportlab.lib.units import mm
from reportlab.pdfbase import pdfmetrics
from reportlab.pdfbase.ttfonts import TTFont
from reportlab.platypus import (
    HRFlowable,
    Image,
    PageBreak,
    Paragraph,
    SimpleDocTemplate,
    Spacer,
    Table,
    TableStyle,
)


ROOT = Path(r"C:\Code\updatedChucs")
ASSETS = ROOT / "tmp" / "pdfs" / "assets"
OUTPUT = ROOT / "output" / "pdf" / "square-to-fourth-workforce-onboarding-brochure.pdf"
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

PAGE_W, PAGE_H = A4
INK = colors.HexColor("#101B18")
FOREST = colors.HexColor("#087A52")
GREEN = colors.HexColor("#12A66A")
MINT = colors.HexColor("#E8F7F0")
CREAM = colors.HexColor("#F7F3EA")
ORANGE = colors.HexColor("#F27A3D")
FOURTH_BRAND = colors.HexColor("#00538B")
MUTED = colors.HexColor("#5D6D67")
LINE = colors.HexColor("#D8E1DD")
WHITE = colors.white

pdfmetrics.registerFont(TTFont("Segoe", r"C:\Windows\Fonts\segoeui.ttf"))
pdfmetrics.registerFont(TTFont("SegoeBold", r"C:\Windows\Fonts\segoeuib.ttf"))
pdfmetrics.registerFont(TTFont("SegoeSemi", r"C:\Windows\Fonts\seguisb.ttf"))

base = getSampleStyleSheet()


def style(name, font="Segoe", size=9.5, leading=13, color=INK, **kwargs):
    return ParagraphStyle(name=name, fontName=font, fontSize=size, leading=leading, textColor=color, **kwargs)


S = {
    "cover_eyebrow": style("cover_eyebrow", "SegoeSemi", 9, 11, colors.HexColor("#73E4B3"), tracking=1.4, spaceAfter=6),
    "cover_title": style("cover_title", "SegoeBold", 34, 37, WHITE, spaceAfter=10),
    "cover_sub": style("cover_sub", "Segoe", 13, 18, colors.HexColor("#DDE9E5"), spaceAfter=14),
    "cover_metric": style("cover_metric", "SegoeBold", 25, 27, WHITE, spaceAfter=2),
    "cover_label": style("cover_label", "SegoeSemi", 8.5, 11, colors.HexColor("#B8CCC5")),
    "eyebrow": style("eyebrow", "SegoeSemi", 8.5, 10, FOREST, tracking=1.2, spaceAfter=5),
    "title": style("title", "SegoeBold", 27, 30, INK, spaceAfter=8),
    "subtitle": style("subtitle", "Segoe", 11, 16, MUTED, spaceAfter=12),
    "h2": style("h2", "SegoeBold", 15, 18, INK, spaceBefore=3, spaceAfter=7),
    "h3": style("h3", "SegoeSemi", 10, 13, INK, spaceAfter=3),
    "body": style("body", "Segoe", 9.2, 13.2, INK, spaceAfter=5),
    "small": style("small", "Segoe", 7.7, 10.5, MUTED),
    "tiny": style("tiny", "Segoe", 6.8, 8.8, MUTED),
    "number": style("number", "SegoeBold", 8.2, 10, FOREST, alignment=TA_RIGHT),
    "table_head": style("table_head", "SegoeSemi", 8, 10, WHITE),
    "table_body": style("table_body", "Segoe", 8.2, 11, INK),
    "cta": style("cta", "SegoeSemi", 11, 14, WHITE),
}


def p(text, name="body"):
    return Paragraph(text, S[name])


def logo(path, width, height):
    return Image(str(path), width=width, height=height, kind="proportional", mask="auto")


def cover_background(canvas, doc):
    canvas.saveState()
    canvas.setFillColor(INK)
    canvas.rect(0, 0, PAGE_W, PAGE_H, stroke=0, fill=1)
    canvas.setFillColor(FOREST)
    canvas.circle(PAGE_W - 20 * mm, PAGE_H - 18 * mm, 58 * mm, stroke=0, fill=1)
    canvas.setFillColor(GREEN)
    canvas.circle(PAGE_W + 12 * mm, PAGE_H - 10 * mm, 46 * mm, stroke=0, fill=1)
    canvas.setFillColor(ORANGE)
    canvas.rect(0, 0, 8 * mm, 67 * mm, stroke=0, fill=1)
    canvas.setFillColor(colors.HexColor("#163029"))
    canvas.circle(PAGE_W - 8 * mm, 7 * mm, 65 * mm, stroke=0, fill=1)
    canvas.restoreState()


def content_background(canvas, doc):
    canvas.saveState()
    canvas.resetTransforms()
    canvas.setFillColor(CREAM)
    canvas.rect(0, 0, PAGE_W, PAGE_H, stroke=0, fill=1)
    canvas.drawImage(
        str(SQUARE_BLACK),
        20 * mm,
        PAGE_H - 18 * mm,
        width=31 * mm,
        height=8 * mm,
        preserveAspectRatio=True,
        anchor="sw",
        mask="auto",
    )
    canvas.setFont("SegoeSemi", 9)
    canvas.setFillColor(INK)
    canvas.drawString(55 * mm, PAGE_H - 15 * mm, "+")
    canvas.drawImage(
        str(FOURTH_BLUE),
        65 * mm,
        PAGE_H - 18.5 * mm,
        width=31 * mm,
        height=9 * mm,
        preserveAspectRatio=True,
        anchor="sw",
        mask="auto",
    )
    canvas.setFont("Segoe", 7.5)
    canvas.setFillColor(MUTED)
    canvas.drawRightString(PAGE_W - 20 * mm, PAGE_H - 15 * mm, "LIVE INTEGRATION PLATFORM")
    canvas.setStrokeColor(LINE)
    canvas.setLineWidth(0.5)
    canvas.line(20 * mm, PAGE_H - 24 * mm, PAGE_W - 20 * mm, PAGE_H - 24 * mm)
    canvas.line(20 * mm, 13 * mm, PAGE_W - 20 * mm, 13 * mm)
    canvas.setFont("Segoe", 7)
    canvas.setFillColor(MUTED)
    canvas.drawString(20 * mm, 8.5 * mm, "SQUARE TO FOURTH  |  WORKFORCE ONBOARDING")
    canvas.drawRightString(PAGE_W - 20 * mm, 8.5 * mm, f"PAGE {doc.page}")
    canvas.restoreState()


def brand_header():
    row = Table(
        [[
            logo(SQUARE_BLACK, 31 * mm, 8 * mm),
            p("+", "h3"),
            logo(FOURTH_BLUE, 31 * mm, 9 * mm),
            p("LIVE INTEGRATION PLATFORM", "small"),
        ]],
        colWidths=[34 * mm, 7 * mm, 36 * mm, 85 * mm],
    )
    row.setStyle(
        TableStyle(
            [
                ("VALIGN", (0, 0), (-1, -1), "MIDDLE"),
                ("ALIGN", (3, 0), (3, 0), "RIGHT"),
                ("LEFTPADDING", (0, 0), (-1, -1), 0),
                ("RIGHTPADDING", (0, 0), (-1, -1), 0),
                ("TOPPADDING", (0, 0), (-1, -1), 0),
                ("BOTTOMPADDING", (0, 0), (-1, -1), 0),
            ]
        )
    )
    return [row, Spacer(1, 4 * mm), HRFlowable(width="100%", thickness=0.6, color=LINE), Spacer(1, 7 * mm)]


def metric(value, label, detail, dark=False):
    bg = colors.HexColor("#17332B") if dark else WHITE
    border = colors.HexColor("#315047") if dark else LINE
    value_style = S["cover_metric"] if dark else style(f"metric_{value}", "SegoeBold", 22, 24, FOREST, spaceAfter=2)
    label_style = S["cover_label"] if dark else style(f"label_{label}", "SegoeSemi", 8.5, 11, INK)
    detail_style = style(f"detail_{label}", "Segoe", 7.6, 10.2, colors.HexColor("#B8CCC5") if dark else MUTED)
    card = Table(
        [[[Paragraph(value, value_style), Paragraph(label, label_style), Spacer(1, 2 * mm), Paragraph(detail, detail_style)]]],
        colWidths=[49 * mm],
    )
    card.setStyle(
        TableStyle(
            [
                ("BACKGROUND", (0, 0), (-1, -1), bg),
                ("BOX", (0, 0), (-1, -1), 0.7, border),
                ("LEFTPADDING", (0, 0), (-1, -1), 10),
                ("RIGHTPADDING", (0, 0), (-1, -1), 10),
                ("TOPPADDING", (0, 0), (-1, -1), 10),
                ("BOTTOMPADDING", (0, 0), (-1, -1), 10),
                ("VALIGN", (0, 0), (-1, -1), "TOP"),
            ]
        )
    )
    return card


def card(title, body, width, bg=WHITE, accent=GREEN):
    block = Table(
        [["", [p(title, "h3"), p(body, "body")]]],
        colWidths=[3 * mm, width - 3 * mm],
    )
    block.setStyle(
        TableStyle(
            [
                ("BACKGROUND", (0, 0), (0, -1), accent),
                ("BACKGROUND", (1, 0), (1, -1), bg),
                ("BOX", (0, 0), (-1, -1), 0.6, LINE),
                ("LEFTPADDING", (0, 0), (0, -1), 0),
                ("RIGHTPADDING", (0, 0), (0, -1), 0),
                ("LEFTPADDING", (1, 0), (1, -1), 10),
                ("RIGHTPADDING", (1, 0), (1, -1), 10),
                ("TOPPADDING", (0, 0), (-1, -1), 9),
                ("BOTTOMPADDING", (0, 0), (-1, -1), 7),
                ("VALIGN", (0, 0), (-1, -1), "TOP"),
            ]
        )
    )
    return block


def numbered_list(items, width=162 * mm):
    rows = []
    for i, item in enumerate(items, 1):
        rows.append([p(f"{i:02d}", "number"), p(item, "table_body")])
    table = Table(rows, colWidths=[12 * mm, width - 12 * mm])
    table.setStyle(
        TableStyle(
            [
                ("VALIGN", (0, 0), (-1, -1), "TOP"),
                ("LINEBELOW", (0, 0), (-1, -2), 0.4, LINE),
                ("LEFTPADDING", (0, 0), (0, -1), 0),
                ("RIGHTPADDING", (0, 0), (0, -1), 7),
                ("LEFTPADDING", (1, 0), (1, -1), 5),
                ("RIGHTPADDING", (1, 0), (1, -1), 0),
                ("TOPPADDING", (0, 0), (-1, -1), 6),
                ("BOTTOMPADDING", (0, 0), (-1, -1), 6),
            ]
        )
    )
    return table


def screenshot(path, width, height):
    image = Image(str(path), width=width, height=height, kind="proportional")
    frame = Table([[image]], colWidths=[width + 6 * mm])
    frame.setStyle(
        TableStyle(
            [
                ("BACKGROUND", (0, 0), (-1, -1), WHITE),
                ("BOX", (0, 0), (-1, -1), 0.8, LINE),
                ("LEFTPADDING", (0, 0), (-1, -1), 3 * mm),
                ("RIGHTPADDING", (0, 0), (-1, -1), 3 * mm),
                ("TOPPADDING", (0, 0), (-1, -1), 3 * mm),
                ("BOTTOMPADDING", (0, 0), (-1, -1), 3 * mm),
            ]
        )
    )
    return frame


def qr_code(value, size=27 * mm):
    qr = QrCodeWidget(value)
    bounds = qr.getBounds()
    scale = min(size / (bounds[2] - bounds[0]), size / (bounds[3] - bounds[1]))
    drawing = Drawing(size, size, transform=[scale, 0, 0, scale, 0, 0])
    drawing.add(qr)
    return drawing


story = []

# Cover
cover_logos = Table(
    [[logo(SQUARE_WHITE, 43 * mm, 11 * mm), p("+", "cover_sub"), logo(FOURTH_WHITE, 45 * mm, 14 * mm)]],
    colWidths=[48 * mm, 11 * mm, 50 * mm],
)
cover_logos.setStyle(TableStyle([("VALIGN", (0, 0), (-1, -1), "MIDDLE"), ("LEFTPADDING", (0, 0), (-1, -1), 0), ("RIGHTPADDING", (0, 0), (-1, -1), 0)]))
story.extend(
    [
        Spacer(1, 2 * mm),
        cover_logos,
        Spacer(1, 36 * mm),
        p("LIVE WORKFORCE ONBOARDING", "cover_eyebrow"),
        p("Square to Fourth<br/>Workforce Management", "cover_title"),
        p("The temporary integration is built, live and ready to configure for an eight-location quick service restaurant group.", "cover_sub"),
    ]
)

cover_url = Table(
    [[p("<link href='https://squaretofourth.store/' color='#FFFFFF'>squaretofourth.store</link>", "cta"), qr_code("https://squaretofourth.store/", 23 * mm)]],
    colWidths=[118 * mm, 30 * mm],
)
cover_url.setStyle(
    TableStyle(
        [
            ("BACKGROUND", (0, 0), (-1, -1), FOREST),
            ("BOX", (0, 0), (-1, -1), 0.7, colors.HexColor("#44B88C")),
            ("VALIGN", (0, 0), (-1, -1), "MIDDLE"),
            ("LEFTPADDING", (0, 0), (-1, -1), 12),
            ("RIGHTPADDING", (0, 0), (-1, -1), 12),
            ("TOPPADDING", (0, 0), (-1, -1), 8),
            ("BOTTOMPADDING", (0, 0), (-1, -1), 8),
            ("ALIGN", (1, 0), (1, 0), "RIGHT"),
        ]
    )
)
story.extend([cover_url, Spacer(1, 13 * mm)])

cover_metrics = Table(
    [[
        metric("2 days", "PILOT ONBOARDING", "Connect and prove one location.", dark=True),
        metric("3-4 days", "ALL LOCATIONS", "Map, activate and verify all eight.", dark=True),
        metric("Read only", "SQUARE ACCESS", "OAuth tokens, never the seller password.", dark=True),
    ]],
    colWidths=[52 * mm, 52 * mm, 52 * mm],
)
cover_metrics.setStyle(TableStyle([("VALIGN", (0, 0), (-1, -1), "TOP"), ("LEFTPADDING", (0, 0), (-1, -1), 0), ("RIGHTPADDING", (0, 0), (-1, -1), 3)]))
story.extend([cover_metrics, Spacer(1, 17 * mm), p("EXISTING PLATFORM  /  CUSTOMER ONBOARDING  /  CONTROLLED ROLLOUT", "cover_label"), PageBreak()])

# Page 2
story.extend(
    [
        p("THE PLATFORM", "eyebrow"),
        p("Already built. Ready to onboard.", "title"),
        p("The service connects an authorised Square seller account to Fourth, maps locations and employees, and runs the daily worked-time transfer with an auditable result.", "subtitle"),
        screenshot(PORTAL_SCREEN, 156 * mm, 88 * mm),
        Spacer(1, 7 * mm),
        p("One controlled path from Square to Fourth", "h2"),
    ]
)

flow = Table(
    [[
        card("1. AUTHORISE", "The seller signs into Square on Square's own screen and approves read access.", 52 * mm, MINT),
        card("2. MAP", "Square locations and team members are matched to Fourth site and employee identifiers.", 52 * mm, WHITE, ORANGE),
        card("3. TRANSFER", "The scheduled service reads timecards, creates the Fourth payload and records the result.", 52 * mm, MINT, FOURTH_BRAND),
    ]],
    colWidths=[54 * mm, 54 * mm, 54 * mm],
)
flow.setStyle(TableStyle([("VALIGN", (0, 0), (-1, -1), "TOP"), ("LEFTPADDING", (0, 0), (-1, -1), 0), ("RIGHTPADDING", (0, 0), (-1, -1), 2)]))
story.extend(
    [
        flow,
        Spacer(1, 6 * mm),
        card(
            "NO SQUARE PASSWORD IS SHARED",
            "The customer uses its normal Square login during OAuth. The platform receives renewable access tokens for the approved permissions; it does not receive or store the customer's Square password.",
            162 * mm,
            colors.HexColor("#FFF1E9"),
            ORANGE,
        ),
        PageBreak(),
    ]
)

# Page 3
story.extend(
    [
        Spacer(1, 28 * mm),
        p("ONBOARDING PACK", "eyebrow"),
        p("What we need from the customer", "title"),
        p("The two-day pilot begins when the following information and contacts are available.", "subtitle"),
    ]
)

requirements = numbered_list(
    [
        "<b>Square authoriser.</b> A seller administrator opens squaretofourth.store and approves read-only <b>TIMECARDS_READ</b> and <b>EMPLOYEES_READ</b> OAuth access.",
        "<b>Fourth connection.</b> Workforce API/import base URL, endpoint and authentication credentials.",
        "<b>Organisation and sites.</b> Fourth organisation ID and the Fourth site/location code for each of the eight restaurants.",
        "<b>Employee mapping.</b> An export containing Fourth EmployeeNumber and a reliable Square match field, ideally email or employee reference.",
        "<b>Test and sign-off.</b> One completed trading day plus a Fourth user who can verify imported records and approve rollout.",
    ],
    83 * mm,
)
onboard_visual = screenshot(ONBOARD_SCREEN, 70 * mm, 44 * mm)
needs_layout = Table(
    [[requirements, [onboard_visual, Spacer(1, 4 * mm), p("The public onboarding screen accepts an administrator-issued invitation phrase before customer details are entered.", "small")]]],
    colWidths=[86 * mm, 76 * mm],
)
needs_layout.setStyle(TableStyle([("VALIGN", (0, 0), (-1, -1), "TOP"), ("LEFTPADDING", (0, 0), (-1, -1), 0), ("RIGHTPADDING", (0, 0), (-1, -1), 3)]))
story.extend([needs_layout, Spacer(1, 8 * mm), p("Two-day pilot plan", "h2")])

days = Table(
    [[
        card("DAY 1 - CONNECT AND MAP", "Authorise Square; confirm locations; configure the pilot site; map its team members; verify Fourth connectivity and the accepted import contract.", 79 * mm, WHITE),
        card("DAY 2 - RUN AND RECONCILE", "Retrieve the agreed day's timecards; transform and submit them; compare timecard, employee and worked-hour totals; record exceptions and obtain sign-off.", 79 * mm, WHITE, FOURTH_BRAND),
    ]],
    colWidths=[82 * mm, 82 * mm],
)
days.setStyle(TableStyle([("VALIGN", (0, 0), (-1, -1), "TOP"), ("LEFTPADDING", (0, 0), (-1, -1), 0), ("RIGHTPADDING", (0, 0), (-1, -1), 4)]))
story.extend(
    [
        days,
        Spacer(1, 7 * mm),
        card(
            "START CONDITION",
            "The estimate measures engineering time after access, mappings and the Fourth interface details are supplied. Waiting for customer access, vendor responses or source-data correction does not consume the two-day delivery allowance.",
            162 * mm,
            MINT,
        ),
        PageBreak(),
    ]
)

# Page 4
story.extend(
    [
        Spacer(1, 28 * mm),
        p("DELIVERY", "eyebrow"),
        p("Fast pilot. Controlled rollout.", "title"),
        p("No new core integration build is expected. We prove the customer's specific Fourth connection at one location, then activate the remaining sites from the same platform.", "subtitle"),
    ]
)

delivery_metrics = Table(
    [[
        metric("2 days", "PILOT", "One location connected, transferred and reconciled."),
        metric("1-2 days", "ROLLOUT", "Seven further locations mapped and verified."),
        metric("3-4 days", "TOTAL", "Expected developer effort for all eight sites."),
    ]],
    colWidths=[54 * mm, 54 * mm, 54 * mm],
)
delivery_metrics.setStyle(TableStyle([("VALIGN", (0, 0), (-1, -1), "TOP"), ("LEFTPADDING", (0, 0), (-1, -1), 0), ("RIGHTPADDING", (0, 0), (-1, -1), 3)]))
story.extend([delivery_metrics, Spacer(1, 5 * mm), p("Acceptance criteria", "h2")])
story.append(
    numbered_list(
        [
            "Eligible completed Square timecard count matches the transformed Fourth record count for the agreed day.",
            "Mapped employees and site codes are accepted by Fourth with no unexplained rejected records.",
            "Clock-in, clock-out and net worked time reconcile, including the agreed treatment of unpaid breaks.",
            "A repeated test run is controlled and does not create an unexplained duplicate payroll result.",
            "Upload status, response, payload and exceptions are available in the platform audit record.",
        ]
    )
)
story.extend([Spacer(1, 7 * mm), p("Pricing options", "h2")])

estimate = Table(
    [
        [p("PACKAGE", "table_head"), p("ONBOARDING", "table_head"), p("MONTHLY SERVICE", "table_head")],
        [p("Single location", "table_body"), p("£495 fixed", "table_body"), p("£149 per month", "table_body")],
        [p("Up to 8 locations", "table_body"), p("£1,250 fixed", "table_body"), p("£295 per month", "table_body")],
        [p("Customer-specific changes", "table_body"), p("£600 per day", "table_body"), p("Only when agreed outside the standard integration.", "table_body")],
    ],
    colWidths=[55 * mm, 43 * mm, 64 * mm],
)
estimate.setStyle(
    TableStyle(
        [
            ("BACKGROUND", (0, 0), (-1, 0), INK),
            ("BACKGROUND", (0, 1), (-1, -2), WHITE),
            ("BACKGROUND", (0, -1), (-1, -1), MINT),
            ("GRID", (0, 0), (-1, -1), 0.5, LINE),
            ("VALIGN", (0, 0), (-1, -1), "TOP"),
            ("LEFTPADDING", (0, 0), (-1, -1), 9),
            ("RIGHTPADDING", (0, 0), (-1, -1), 9),
            ("TOPPADDING", (0, 0), (-1, -1), 7),
            ("BOTTOMPADDING", (0, 0), (-1, -1), 7),
        ]
    )
)
story.extend([estimate, Spacer(1, 3 * mm)])

scope = Table(
    [[
        p("INCLUDED", "h3"),
        p("Daily actual worked-time export, OAuth connection, employee/location mapping, scheduling, upload and audit reporting.", "small"),
        p("EXCLUDED", "h3"),
        p("Employee creation, rota synchronisation, holidays, payroll calculation and two-way updates unless separately agreed.", "small"),
    ]],
    colWidths=[25 * mm, 54 * mm, 25 * mm, 58 * mm],
)
scope.setStyle(TableStyle([("BACKGROUND", (0, 0), (-1, -1), WHITE), ("BOX", (0, 0), (-1, -1), 0.6, LINE), ("VALIGN", (0, 0), (-1, -1), "TOP"), ("LEFTPADDING", (0, 0), (-1, -1), 8), ("RIGHTPADDING", (0, 0), (-1, -1), 8), ("TOPPADDING", (0, 0), (-1, -1), 8), ("BOTTOMPADDING", (0, 0), (-1, -1), 7)]))

cta = Table(
    [[p("NEXT STEP", "table_head"), p("Nominate the Square and Fourth contacts, then send the location and employee mapping pack.", "cta")]],
    colWidths=[28 * mm, 134 * mm],
)
cta.setStyle(TableStyle([("BACKGROUND", (0, 0), (0, 0), FOREST), ("BACKGROUND", (1, 0), (1, 0), INK), ("VALIGN", (0, 0), (-1, -1), "MIDDLE"), ("LEFTPADDING", (0, 0), (-1, -1), 10), ("RIGHTPADDING", (0, 0), (-1, -1), 10), ("TOPPADDING", (0, 0), (-1, -1), 9), ("BOTTOMPADDING", (0, 0), (-1, -1), 9)]))

story.extend([
    p("<b>Included:</b> daily worked-time export, OAuth, mappings, scheduling, upload and audit reporting. "
      "<b>Excluded:</b> employee creation, rotas, holidays, payroll calculation and two-way updates.", "tiny"),
    Spacer(1, 3 * mm),
    cta,
    Spacer(1, 2 * mm),
    p("Commercial terms: three-month minimum; prices exclude VAT and third-party Square/Fourth charges. A materially different Fourth interface is handled through change control.", "tiny"),
])

doc = SimpleDocTemplate(
    str(OUTPUT),
    pagesize=A4,
    leftMargin=24 * mm,
    rightMargin=24 * mm,
    topMargin=31 * mm,
    bottomMargin=18 * mm,
    title="Square to Fourth Workforce Management - Customer Onboarding Brochure",
    author="Square to Fourth Integration",
    subject="Existing integration onboarding for an eight-location quick service restaurant group",
)
doc.build(story, onFirstPage=cover_background, onLaterPages=content_background)
print(OUTPUT)
