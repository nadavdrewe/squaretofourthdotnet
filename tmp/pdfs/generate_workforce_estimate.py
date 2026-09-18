from pathlib import Path

from reportlab.lib import colors
from reportlab.lib.enums import TA_LEFT, TA_RIGHT
from reportlab.lib.pagesizes import A4
from reportlab.lib.styles import ParagraphStyle, getSampleStyleSheet
from reportlab.lib.units import mm
from reportlab.pdfbase import pdfmetrics
from reportlab.pdfbase.ttfonts import TTFont
from reportlab.platypus import (
    KeepTogether,
    PageBreak,
    Paragraph,
    SimpleDocTemplate,
    Spacer,
    Table,
    TableStyle,
)


ROOT = Path(r"C:\Code\updatedChucs")
OUTPUT = ROOT / "output" / "pdf" / "square-to-fourth-workforce-pilot-spec-estimate.pdf"
OUTPUT.parent.mkdir(parents=True, exist_ok=True)

PAGE_W, PAGE_H = A4
INK = colors.HexColor("#15201D")
MUTED = colors.HexColor("#596963")
GREEN = colors.HexColor("#16A36A")
GREEN_DARK = colors.HexColor("#08744A")
MINT = colors.HexColor("#EAF8F1")
CREAM = colors.HexColor("#F7F4EC")
LINE = colors.HexColor("#DCE4E0")
WHITE = colors.white


def register_fonts():
    pdfmetrics.registerFont(TTFont("Segoe", r"C:\Windows\Fonts\segoeui.ttf"))
    pdfmetrics.registerFont(TTFont("SegoeBold", r"C:\Windows\Fonts\segoeuib.ttf"))
    pdfmetrics.registerFont(TTFont("SegoeSemi", r"C:\Windows\Fonts\seguisb.ttf"))


register_fonts()

styles = getSampleStyleSheet()
styles.add(
    ParagraphStyle(
        name="Eyebrow",
        fontName="SegoeSemi",
        fontSize=8,
        leading=10,
        textColor=GREEN_DARK,
        spaceAfter=5,
        tracking=1.2,
    )
)
styles.add(
    ParagraphStyle(
        name="TitleSF",
        fontName="SegoeBold",
        fontSize=26,
        leading=29,
        textColor=INK,
        spaceAfter=7,
    )
)
styles.add(
    ParagraphStyle(
        name="SubtitleSF",
        fontName="Segoe",
        fontSize=11,
        leading=16,
        textColor=MUTED,
        spaceAfter=15,
    )
)
styles.add(
    ParagraphStyle(
        name="LinkSF",
        fontName="SegoeSemi",
        fontSize=10,
        leading=13,
        textColor=GREEN_DARK,
        spaceAfter=10,
    )
)
styles.add(
    ParagraphStyle(
        name="H2SF",
        fontName="SegoeBold",
        fontSize=15,
        leading=18,
        textColor=INK,
        spaceBefore=5,
        spaceAfter=8,
    )
)
styles.add(
    ParagraphStyle(
        name="H3SF",
        fontName="SegoeSemi",
        fontSize=10,
        leading=13,
        textColor=INK,
        spaceAfter=3,
    )
)
styles.add(
    ParagraphStyle(
        name="BodySF",
        fontName="Segoe",
        fontSize=9.2,
        leading=13.5,
        textColor=INK,
        spaceAfter=6,
    )
)
styles.add(
    ParagraphStyle(
        name="SmallSF",
        fontName="Segoe",
        fontSize=7.7,
        leading=10.5,
        textColor=MUTED,
    )
)
styles.add(
    ParagraphStyle(
        name="WhiteSmall",
        fontName="SegoeSemi",
        fontSize=8,
        leading=10,
        textColor=WHITE,
    )
)
styles.add(
    ParagraphStyle(
        name="Metric",
        fontName="SegoeBold",
        fontSize=22,
        leading=24,
        textColor=GREEN_DARK,
        spaceAfter=2,
    )
)
styles.add(
    ParagraphStyle(
        name="MetricLabel",
        fontName="SegoeSemi",
        fontSize=8.5,
        leading=11,
        textColor=INK,
    )
)
styles.add(
    ParagraphStyle(
        name="TableHead",
        fontName="SegoeSemi",
        fontSize=8,
        leading=10,
        textColor=WHITE,
    )
)
styles.add(
    ParagraphStyle(
        name="TableBody",
        fontName="Segoe",
        fontSize=8.2,
        leading=11,
        textColor=INK,
    )
)


def p(text, style="BodySF"):
    return Paragraph(text, styles[style])


def page_frame(canvas, doc):
    canvas.saveState()
    canvas.setFillColor(CREAM)
    canvas.rect(0, 0, PAGE_W, PAGE_H, stroke=0, fill=1)

    canvas.setFillColor(INK)
    canvas.rect(0, PAGE_H - 15 * mm, PAGE_W, 15 * mm, stroke=0, fill=1)
    canvas.setFillColor(GREEN)
    canvas.rect(0, PAGE_H - 15 * mm, 29 * mm, 15 * mm, stroke=0, fill=1)
    canvas.setFillColor(GREEN_DARK)
    canvas.circle(29 * mm, PAGE_H - 7.5 * mm, 7.5 * mm, stroke=0, fill=1)

    canvas.setFont("SegoeSemi", 8)
    canvas.setFillColor(WHITE)
    canvas.drawString(18 * mm, PAGE_H - 9.5 * mm, "SQUARE TO FOURTH")
    canvas.setFont("Segoe", 7.5)
    canvas.drawRightString(PAGE_W - 18 * mm, PAGE_H - 9.5 * mm, "WORKFORCE INTEGRATION / ESTIMATE")

    canvas.setStrokeColor(LINE)
    canvas.setLineWidth(0.6)
    canvas.line(18 * mm, 13 * mm, PAGE_W - 18 * mm, 13 * mm)
    canvas.setFont("Segoe", 7)
    canvas.setFillColor(MUTED)
    canvas.drawString(18 * mm, 8.5 * mm, "Prepared 9 September 2026  |  Draft for discussion")
    canvas.drawRightString(PAGE_W - 18 * mm, 8.5 * mm, f"PAGE {doc.page}")
    canvas.restoreState()


def metric_card(value, label, note):
    content = [p(value, "Metric"), p(label, "MetricLabel"), Spacer(1, 2 * mm), p(note, "SmallSF")]
    box = Table([[content]], colWidths=[51 * mm])
    box.setStyle(
        TableStyle(
            [
                ("BACKGROUND", (0, 0), (-1, -1), WHITE),
                ("BOX", (0, 0), (-1, -1), 0.7, LINE),
                ("LEFTPADDING", (0, 0), (-1, -1), 11),
                ("RIGHTPADDING", (0, 0), (-1, -1), 11),
                ("TOPPADDING", (0, 0), (-1, -1), 11),
                ("BOTTOMPADDING", (0, 0), (-1, -1), 11),
                ("VALIGN", (0, 0), (-1, -1), "TOP"),
            ]
        )
    )
    return box


def info_card(title, body, width=80 * mm, background=WHITE):
    card = Table([[[p(title, "H3SF"), p(body, "BodySF")]]], colWidths=[width])
    card.setStyle(
        TableStyle(
            [
                ("BACKGROUND", (0, 0), (-1, -1), background),
                ("BOX", (0, 0), (-1, -1), 0.7, LINE),
                ("LEFTPADDING", (0, 0), (-1, -1), 11),
                ("RIGHTPADDING", (0, 0), (-1, -1), 11),
                ("TOPPADDING", (0, 0), (-1, -1), 10),
                ("BOTTOMPADDING", (0, 0), (-1, -1), 8),
                ("VALIGN", (0, 0), (-1, -1), "TOP"),
            ]
        )
    )
    return card


def checklist(items):
    rows = []
    for idx, item in enumerate(items, start=1):
        marker = Paragraph(f"{idx:02d}", ParagraphStyle(
            name=f"Marker{idx}", fontName="SegoeBold", fontSize=8, leading=10,
            textColor=GREEN_DARK, alignment=TA_RIGHT
        ))
        rows.append([marker, p(item, "TableBody")])
    table = Table(rows, colWidths=[11 * mm, 149 * mm], hAlign="LEFT")
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


story = []
story.extend(
    [
        Spacer(1, 5 * mm),
        p("TEMPORARY INTEGRATION PROPOSAL", "Eyebrow"),
        p("Square to Fourth<br/>Workforce Management", "TitleSF"),
        p(
            "The integration platform is already built. This proposal covers onboarding an eight-location quick service "
            "restaurant group, validating its Fourth connection and activating the daily worked-time transfer.",
            "SubtitleSF",
        ),
        p("Existing platform: <link href='https://squaretofourth.store/'>https://squaretofourth.store/</link>", "LinkSF"),
    ]
)

metrics = Table(
    [[
        metric_card("2 days", "BASIC PILOT", "One location, one agreed trading day, end-to-end."),
        metric_card("3-4 days", "FULL ONBOARDING", "Pilot validation and all eight locations."),
        metric_card("8 sites", "ROLLOUT", "Shared integration with per-location configuration."),
    ]],
    colWidths=[54 * mm, 54 * mm, 54 * mm],
    hAlign="LEFT",
)
metrics.setStyle(TableStyle([("VALIGN", (0, 0), (-1, -1), "TOP"), ("LEFTPADDING", (0, 0), (-1, -1), 0), ("RIGHTPADDING", (0, 0), (-1, -1), 3)]))
story.extend([metrics, Spacer(1, 8 * mm)])

recommendation = Table(
    [[
        p("RECOMMENDATION", "WhiteSmall"),
        p(
            "No new core integration build is expected. Start with a two-working-day onboarding pilot using the existing "
            "OAuth, Square Timecards retrieval, employee/location mapping, Fourth XML generation, scheduling and audit logging. "
            "Activate the remaining locations after one representative day reconciles in Fourth.",
            "BodySF",
        ),
    ]],
    colWidths=[35 * mm, 127 * mm],
)
recommendation.setStyle(
    TableStyle(
        [
            ("BACKGROUND", (0, 0), (0, -1), GREEN_DARK),
            ("BACKGROUND", (1, 0), (1, -1), MINT),
            ("BOX", (0, 0), (-1, -1), 0.7, GREEN_DARK),
            ("VALIGN", (0, 0), (-1, -1), "TOP"),
            ("LEFTPADDING", (0, 0), (-1, -1), 11),
            ("RIGHTPADDING", (0, 0), (-1, -1), 11),
            ("TOPPADDING", (0, 0), (-1, -1), 11),
            ("BOTTOMPADDING", (0, 0), (-1, -1), 8),
        ]
    )
)
story.extend([recommendation, Spacer(1, 8 * mm), p("Pilot delivery plan", "H2SF")])

day_cards = Table(
    [[
        info_card(
            "DAY 1 - CONNECT AND MAP",
            "Connect the customer's Square account using OAuth; confirm the eight Square locations; configure one pilot "
            "location; import the team roster and map Square team member IDs to Fourth employee numbers; validate the Fourth "
            "endpoint and authentication.",
            79 * mm,
        ),
        info_card(
            "DAY 2 - RUN AND RECONCILE",
            "Retrieve the agreed day's completed Square timecards; transform them into the accepted Fourth payload; submit the "
            "file; compare employee, timecard and worked-hour totals; record the result and document any exceptions before "
            "approval to roll out.",
            79 * mm,
        ),
    ]],
    colWidths=[82 * mm, 82 * mm],
)
day_cards.setStyle(TableStyle([("VALIGN", (0, 0), (-1, -1), "TOP"), ("LEFTPADDING", (0, 0), (-1, -1), 0), ("RIGHTPADDING", (0, 0), (-1, -1), 4)]))
story.extend([day_cards, Spacer(1, 7 * mm), p("What the basic pilot delivers", "H2SF")])
story.append(
    checklist(
        [
            "Read-only Square connection using merchant, employee and timecard permissions.",
            "A configured Square location mapped to its Fourth site/location code.",
            "Completed timecards converted into the agreed Fourth Workforce import format.",
            "Employee-number mapping, upload result, generated payload and operational audit record.",
            "A written reconciliation for one representative trading day.",
        ]
    )
)

story.append(PageBreak())
story.extend(
    [
        Spacer(1, 5 * mm),
        p("DELIVERY BASIS", "Eyebrow"),
        p("Scope, controls and dependencies", "TitleSF"),
        p(
            "The platform is already available at squaretofourth.store. The two-day target starts when the inputs below are "
            "complete; waiting for access, endpoint confirmation or source-data corrections is outside engineering elapsed time.",
            "SubtitleSF",
        ),
        p("Required before Day 1", "H2SF"),
    ]
)

requirements = Table(
    [
        [p("SQUARE", "TableHead"), p("FOURTH", "TableHead"), p("MAPPING DATA", "TableHead")],
        [
            p("An authorised seller administrator opens squaretofourth.store and completes OAuth for the live account. No Square password is shared with us.", "TableBody"),
            p("Workforce API/import base URL, endpoint, authentication credentials, organisation ID, eight site codes and an accepted sample payload or interface specification.", "TableBody"),
            p("An employee export containing each Fourth EmployeeNumber and a reliable Square match field, ideally email or employee reference.", "TableBody"),
        ],
    ],
    colWidths=[54 * mm, 54 * mm, 54 * mm],
    repeatRows=1,
)
requirements.setStyle(
    TableStyle(
        [
            ("BACKGROUND", (0, 0), (-1, 0), INK),
            ("BACKGROUND", (0, 1), (-1, -1), WHITE),
            ("GRID", (0, 0), (-1, -1), 0.6, LINE),
            ("VALIGN", (0, 0), (-1, -1), "TOP"),
            ("LEFTPADDING", (0, 0), (-1, -1), 9),
            ("RIGHTPADDING", (0, 0), (-1, -1), 9),
            ("TOPPADDING", (0, 0), (-1, -1), 8),
            ("BOTTOMPADDING", (0, 0), (-1, -1), 8),
        ]
    )
)
story.extend([
    requirements,
    Spacer(1, 4 * mm),
    info_card(
        "TEST DAY AND SIGN-OFF",
        "The customer nominates one completed trading day and a Fourth user who can verify the imported records. "
        "Approval is required before the remaining seven locations are activated.",
        162 * mm,
        MINT,
    ),
    Spacer(1, 5 * mm),
    p("Acceptance criteria", "H2SF"),
])
story.append(
    checklist(
        [
            "The count of eligible completed Square timecards matches the transformed Fourth records for the agreed day.",
            "Mapped employees and site codes are accepted by Fourth with no rejected records.",
            "Clock-in, clock-out and net worked time reconcile, including the agreed treatment of unpaid breaks.",
            "A repeated test run is controlled and does not create an unexplained duplicate payroll result.",
            "Upload status, response, payload and exceptions are visible in the integration audit log.",
        ]
    )
)

story.extend([Spacer(1, 7 * mm), p("Estimate and rollout", "H2SF")])
estimate = Table(
    [
        [p("PHASE", "TableHead"), p("EFFORT", "TableHead"), p("OUTPUT", "TableHead")],
        [p("Customer onboarding pilot", "TableBody"), p("2 working days", "TableBody"), p("Customer connected and one location imported and reconciled end-to-end.", "TableBody")],
        [p("Seven remaining locations", "TableBody"), p("1-2 working days", "TableBody"), p("Location and employee mapping, controlled activation and cross-site verification.", "TableBody")],
        [p("Expected total", "TableBody"), p("3-4 developer days", "TableBody"), p("No new core build expected, assuming the customer's Fourth interface matches the existing import contract.", "TableBody")],
    ],
    colWidths=[42 * mm, 35 * mm, 85 * mm],
    repeatRows=1,
)
estimate.setStyle(
    TableStyle(
        [
            ("BACKGROUND", (0, 0), (-1, 0), INK),
            ("BACKGROUND", (0, 1), (-1, -2), WHITE),
            ("BACKGROUND", (0, -1), (-1, -1), MINT),
            ("GRID", (0, 0), (-1, -1), 0.6, LINE),
            ("VALIGN", (0, 0), (-1, -1), "TOP"),
            ("LEFTPADDING", (0, 0), (-1, -1), 9),
            ("RIGHTPADDING", (0, 0), (-1, -1), 9),
            ("TOPPADDING", (0, 0), (-1, -1), 7),
            ("BOTTOMPADDING", (0, 0), (-1, -1), 7),
        ]
    )
)
story.extend([estimate, Spacer(1, 3 * mm)])

scope_notes = Table(
    [[[
        p("SCOPE AND COMMERCIAL BASIS", "H3SF"),
        p("<b>Included:</b> daily worked-time export, OAuth, mappings, scheduling and audit reporting. "
          "<b>Excluded:</b> employee creation, rotas, holidays, payroll calculations and two-way updates. "
          "Estimate: three to four developer days at the agreed day rate; third-party charges excluded. A materially different Fourth interface requires change control.", "SmallSF"),
    ]]],
    colWidths=[162 * mm],
)
scope_notes.setStyle(
    TableStyle(
        [
            ("BACKGROUND", (0, 0), (-1, -1), WHITE),
            ("BOX", (0, 0), (-1, -1), 0.7, LINE),
            ("VALIGN", (0, 0), (-1, -1), "TOP"),
            ("LEFTPADDING", (0, 0), (-1, -1), 11),
            ("RIGHTPADDING", (0, 0), (-1, -1), 11),
            ("TOPPADDING", (0, 0), (-1, -1), 10),
            ("BOTTOMPADDING", (0, 0), (-1, -1), 9),
        ]
    )
)
story.append(scope_notes)

doc = SimpleDocTemplate(
    str(OUTPUT),
    pagesize=A4,
    leftMargin=24 * mm,
    rightMargin=24 * mm,
    topMargin=22 * mm,
    bottomMargin=18 * mm,
    title="Square to Fourth Workforce Management - Pilot Specification and Estimate",
    author="Square to Fourth Integration",
    subject="Temporary integration for an eight-location quick service restaurant group",
)
doc.build(story, onFirstPage=page_frame, onLaterPages=page_frame)
print(OUTPUT)
