from __future__ import annotations

import argparse
import re
from collections import defaultdict
from dataclasses import dataclass
from datetime import date
from pathlib import Path

from reportlab.lib import colors
from reportlab.lib.pagesizes import A3, landscape
from reportlab.pdfbase import pdfmetrics
from reportlab.pdfbase.ttfonts import TTFont
from reportlab.pdfgen import canvas


PAGE_SIZE = landscape(A3)
PAGE_WIDTH, PAGE_HEIGHT = PAGE_SIZE
MARGIN = 34
FONT = "ArchitectureSans"
FONT_BOLD = "ArchitectureSansBold"

AREA_COLORS = {
    "Bootstrap": colors.HexColor("#D7E8FF"),
    "Composition": colors.HexColor("#E2D9FF"),
    "Game": colors.HexColor("#D8F3DC"),
    "Infrastructure": colors.HexColor("#E4E7EB"),
    "Presentation": colors.HexColor("#FFF0C7"),
    "UI": colors.HexColor("#FFD9E2"),
    "User": colors.HexColor("#CDEFF2"),
    "Validation": colors.HexColor("#F1E4C8"),
}


@dataclass(frozen=True)
class TypeInfo:
    namespace: str
    name: str
    kind: str
    bases: tuple[str, ...]
    file: Path

    @property
    def area(self) -> str:
        parts = self.namespace.split(".")
        return parts[1] if len(parts) > 1 else "Root"

    @property
    def uml_label(self) -> str:
        stereotype = {
            "interface": "«interface»",
            "enum": "«enumeration»",
            "struct": "«value type»",
            "record": "«record»",
        }.get(self.kind, "«class»")
        return f"{stereotype} {self.name}"


def register_fonts() -> None:
    regular_candidates = (
        Path("C:/Windows/Fonts/arial.ttf"),
        Path("C:/Windows/Fonts/segoeui.ttf"),
        Path("/usr/share/fonts/truetype/dejavu/DejaVuSans.ttf"),
    )
    bold_candidates = (
        Path("C:/Windows/Fonts/arialbd.ttf"),
        Path("C:/Windows/Fonts/segoeuib.ttf"),
        Path("/usr/share/fonts/truetype/dejavu/DejaVuSans-Bold.ttf"),
    )
    regular = next(path for path in regular_candidates if path.exists())
    bold = next(path for path in bold_candidates if path.exists())
    pdfmetrics.registerFont(TTFont(FONT, str(regular)))
    pdfmetrics.registerFont(TTFont(FONT_BOLD, str(bold)))


def parse_types(scripts_root: Path) -> tuple[list[TypeInfo], dict[tuple[str, str], int]]:
    type_pattern = re.compile(
        r"^\s*(?:(?:public|internal|private|protected)\s+)?"
        r"(?:(?:abstract|sealed|static|partial|readonly)\s+)*"
        r"(?P<kind>class|struct|interface|enum|record)\s+"
        r"(?P<name>[A-Za-z_][A-Za-z0-9_]*)"
        r"(?:\s*:\s*(?P<bases>[^{\r\n]+))?",
        re.MULTILINE,
    )
    namespace_pattern = re.compile(r"^namespace\s+(LL(?:\.[A-Za-z0-9_]+)+)", re.MULTILINE)
    using_pattern = re.compile(r"^using\s+LL\.(?P<area>[A-Za-z_][A-Za-z0-9_]*)", re.MULTILINE)
    types: list[TypeInfo] = []
    edges: dict[tuple[str, str], int] = defaultdict(int)

    for file in sorted(scripts_root.rglob("*.cs")):
        text = file.read_text(encoding="utf-8-sig")
        namespace_match = namespace_pattern.search(text)
        if not namespace_match:
            continue
        namespace = namespace_match.group(1)
        source_area = namespace.split(".")[1]
        for match in type_pattern.finditer(text):
            bases = tuple(
                part.strip().split("<", 1)[0]
                for part in (match.group("bases") or "").split(",")
                if part.strip()
            )
            types.append(
                TypeInfo(
                    namespace=namespace,
                    name=match.group("name"),
                    kind=match.group("kind"),
                    bases=bases,
                    file=file,
                )
            )
        for using_match in using_pattern.finditer(text):
            target_area = using_match.group("area")
            if target_area != source_area:
                edges[(source_area, target_area)] += 1

    return types, dict(edges)


def wrap_text(text: str, width: int) -> list[str]:
    words = text.split()
    lines: list[str] = []
    current = ""
    for word in words:
        candidate = word if not current else f"{current} {word}"
        if len(candidate) <= width:
            current = candidate
        else:
            if current:
                lines.append(current)
            current = word
    if current:
        lines.append(current)
    return lines


def draw_page_header(pdf: canvas.Canvas, title: str, subtitle: str = "") -> None:
    pdf.setFillColor(colors.HexColor("#17212B"))
    pdf.setFont(FONT_BOLD, 19)
    pdf.drawString(MARGIN, PAGE_HEIGHT - MARGIN, title)
    if subtitle:
        pdf.setFont(FONT, 8)
        pdf.setFillColor(colors.HexColor("#52616B"))
        pdf.drawRightString(PAGE_WIDTH - MARGIN, PAGE_HEIGHT - MARGIN + 2, subtitle)


def draw_page_footer(pdf: canvas.Canvas, page_number: int) -> None:
    pdf.setStrokeColor(colors.HexColor("#CBD2D9"))
    pdf.line(MARGIN, 25, PAGE_WIDTH - MARGIN, 25)
    pdf.setFillColor(colors.HexColor("#66788A"))
    pdf.setFont(FONT, 7)
    pdf.drawString(MARGIN, 13, "LL.Runtime — архитектурный UML-атлас")
    pdf.drawRightString(PAGE_WIDTH - MARGIN, 13, str(page_number))


def draw_box(
    pdf: canvas.Canvas,
    x: float,
    y: float,
    width: float,
    height: float,
    title: str,
    lines: list[str],
    fill: colors.Color,
    font_size: float = 7,
) -> None:
    pdf.setFillColor(fill)
    pdf.setStrokeColor(colors.HexColor("#5D6D7E"))
    pdf.roundRect(x, y, width, height, 7, fill=1, stroke=1)
    title_height = 23
    pdf.setFillColor(colors.Color(fill.red * 0.86, fill.green * 0.86, fill.blue * 0.86))
    pdf.roundRect(x, y + height - title_height, width, title_height, 7, fill=1, stroke=0)
    pdf.rect(x, y + height - title_height, width, title_height - 7, fill=1, stroke=0)
    pdf.setFillColor(colors.HexColor("#17212B"))
    pdf.setFont(FONT_BOLD, 9)
    pdf.drawString(x + 8, y + height - 15, title)
    pdf.setFont(FONT, font_size)
    cursor_y = y + height - title_height - 11
    for line in lines:
        if cursor_y < y + 7:
            pdf.drawString(x + 8, cursor_y, "…")
            break
        pdf.drawString(x + 8, cursor_y, line)
        cursor_y -= font_size + 2


def draw_arrow(
    pdf: canvas.Canvas,
    start: tuple[float, float],
    end: tuple[float, float],
    label: str = "",
    dashed: bool = False,
) -> None:
    x1, y1 = start
    x2, y2 = end
    pdf.saveState()
    pdf.setStrokeColor(colors.HexColor("#455A64"))
    pdf.setFillColor(colors.HexColor("#455A64"))
    pdf.setLineWidth(1)
    if dashed:
        pdf.setDash(4, 3)
    pdf.line(x1, y1, x2, y2)
    angle = __import__("math").atan2(y2 - y1, x2 - x1)
    size = 6
    for delta in (2.65, -2.65):
        pdf.line(
            x2,
            y2,
            x2 + size * __import__("math").cos(angle + delta),
            y2 + size * __import__("math").sin(angle + delta),
        )
    if label:
        pdf.setFont(FONT, 6.5)
        pdf.drawCentredString((x1 + x2) / 2, (y1 + y2) / 2 + 5, label)
    pdf.restoreState()


def render_overview(
    pdf: canvas.Canvas,
    types: list[TypeInfo],
    edges: dict[tuple[str, str], int],
    page_number: int,
) -> None:
    draw_page_header(
        pdf,
        "1. Обзор LL.Runtime",
        f"Фактическое состояние • {date.today().isoformat()}",
    )
    positions = {
        "Composition": (70, 650),
        "Bootstrap": (70, 485),
        "UI": (360, 620),
        "Presentation": (360, 440),
        "User": (650, 620),
        "Game": (650, 420),
        "Infrastructure": (940, 590),
        "Validation": (940, 390),
    }
    box_w, box_h = 205, 105
    counts = defaultdict(int)
    for item in types:
        counts[item.area] += 1
    descriptions = {
        "Composition": "Scopes, installers, factories",
        "Bootstrap": "Startup operations and flow",
        "UI": "Components, views, windows, flows",
        "Presentation": "Localization, icons, formatting",
        "User": "Defaults, snapshots, state, persistence",
        "Game": "Rules, catalogs, services, reference data",
        "Infrastructure": "Loading, saving, storage, reporting",
        "Validation": "Issues, contexts, rules, reports",
    }
    for source, target in sorted(edges):
        if source not in positions or target not in positions:
            continue
        x1, y1 = positions[source]
        x2, y2 = positions[target]
        if abs(x1 - x2) >= abs(y1 - y2):
            start = (x1 + (box_w if x2 > x1 else 0), y1 + box_h / 2)
            end = (x2 + (0 if x2 > x1 else box_w), y2 + box_h / 2)
        else:
            start = (x1 + box_w / 2, y1 + (box_h if y2 > y1 else 0))
            end = (x2 + box_w / 2, y2 + (0 if y2 > y1 else box_h))
        draw_arrow(pdf, start, end)
    for area, (x, y) in positions.items():
        draw_box(
            pdf,
            x,
            y,
            box_w,
            box_h,
            area,
            [descriptions[area], f"Types: {counts[area]}"],
            AREA_COLORS[area],
            8,
        )
    pdf.setFillColor(colors.HexColor("#263238"))
    pdf.setFont(FONT_BOLD, 10)
    pdf.drawString(70, 330, "Условные обозначения")
    pdf.setFont(FONT, 8)
    legend = [
        "Стрелка A → B: исходники области A импортируют namespace области B.",
        "Количества файловых import-связей приведены в матрице на последней странице.",
        "Логические области находятся в одной физической сборке LL.Runtime.",
        "Точная разрешённая матрица и исключения определены в ARCHITECTURE.md.",
    ]
    for index, line in enumerate(legend):
        pdf.drawString(70, 310 - index * 16, f"• {line}")
    draw_page_footer(pdf, page_number)
    pdf.showPage()


def render_composition(pdf: canvas.Canvas, page_number: int) -> None:
    draw_page_header(pdf, "2. Composition и lifetime boundaries")
    scope_y = 630
    scope_x = [70, 455, 840]
    scopes = [
        (
            "ProjectLifetimeScope",
            [
                "WindowInstaller",
                "PresentationInstaller",
                "ValidationReportingInstaller",
                "GameDataInstaller",
                "UserInstaller",
                "GameServicesInstaller",
            ],
            "process lifetime",
        ),
        (
            "BootstrapLifetimeScope",
            ["BootstrapInstaller"],
            "Bootstrap scene",
        ),
        (
            "MainLifetimeScope",
            ["MainSceneInstaller"],
            "Main scene",
        ),
    ]
    for x, (scope, installers, lifetime) in zip(scope_x, scopes):
        draw_box(
            pdf,
            x,
            scope_y,
            300,
            115,
            scope,
            [f"«lifetime» {lifetime}", "orchestrates installers only"],
            AREA_COLORS["Composition"],
            8,
        )
        installer_height = 28 + len(installers) * 14
        draw_box(
            pdf,
            x,
            390,
            300,
            installer_height,
            "Installers",
            [f"«installer» {name}" for name in installers],
            colors.HexColor("#EEE9FF"),
            7.5,
        )
        draw_arrow(
            pdf,
            (x + 150, scope_y),
            (x + 150, 390 + installer_height),
            "new + Install",
        )
    draw_box(
        pdf,
        70,
        110,
        360,
        175,
        "Factories",
        [
            "«factory» GameDataLoaderFactory",
            "  → ScriptableObjectGameDataLoader",
            "  → SerializedGameDataLoader",
            "«factory» UserSaveServiceFactory",
            "  → JsonSaveSerializer",
            "  → FileSaveStorage",
            "  → SaveService",
        ],
        colors.HexColor("#F4F0FF"),
        8,
    )
    draw_box(
        pdf,
        470,
        110,
        685,
        175,
        "Container-owned runtime graph",
        [
            "Project: WindowProvider, WindowNavigator, localization, catalogs, user state, game services",
            "Bootstrap: BootstrapFlow + parallel IBootstrapOperation implementations",
            "Main: WindowController, modal confirmation adapters, RankPromotionFlow, UpgradeFlow",
            "DI constructors / Construct methods: [Inject]",
            "Manual installer and factory construction: no [Inject]",
        ],
        colors.HexColor("#F7F8FA"),
        8,
    )
    draw_page_footer(pdf, page_number)
    pdf.showPage()


def render_flows(pdf: canvas.Canvas, page_number: int) -> None:
    draw_page_header(pdf, "3. Основные runtime-потоки")
    flows = [
        (
            "Game reference data",
            [
                ("Config assets", "IDataLoader<Catalog>"),
                ("IDataLoader<Catalog>", "ScriptableObjectGameDataLoader"),
                ("ScriptableObjectGameDataLoader", "GameDataSnapshot"),
                ("GameDataSnapshot", "Catalog registrations"),
            ],
            620,
            AREA_COLORS["Game"],
        ),
        (
            "Serialized game data",
            [
                ("ISaveStorage", "ISaveService"),
                ("ISaveService", "SerializedGameDataLoader"),
                ("SerializedGameDataLoader", "GameDataDocument"),
                ("GameDataDocument", "GameDataDocumentMapper"),
                ("GameDataDocumentMapper", "GameDataSnapshot"),
            ],
            420,
            AREA_COLORS["Infrastructure"],
        ),
        (
            "User load / live state / save",
            [
                ("Defaults or UserSaveData", "UserSnapshotLoader"),
                ("UserSnapshotLoader", "UserSnapshot"),
                ("UserSnapshot", "User State"),
                ("User State", "UserSaveController"),
                ("UserSaveController", "ISaveService"),
            ],
            220,
            AREA_COLORS["User"],
        ),
    ]
    for title, links, y, fill in flows:
        pdf.setFont(FONT_BOLD, 10)
        pdf.setFillColor(colors.HexColor("#263238"))
        pdf.drawString(70, y + 105, title)
        node_count = len(links) + 1
        node_w = 190 if node_count <= 5 else 165
        gap = (PAGE_WIDTH - 140 - node_count * node_w) / max(1, node_count - 1)
        names = [links[0][0]] + [target for _, target in links]
        for index, name in enumerate(names):
            x = 70 + index * (node_w + gap)
            draw_box(pdf, x, y, node_w, 72, name, [], fill, 7)
            if index < len(names) - 1:
                draw_arrow(
                    pdf,
                    (x + node_w, y + 36),
                    (x + node_w + gap, y + 36),
                )
    draw_page_footer(pdf, page_number)
    pdf.showPage()


def render_inventory(
    pdf: canvas.Canvas,
    types: list[TypeInfo],
    page_number: int,
) -> int:
    grouped: dict[str, list[TypeInfo]] = defaultdict(list)
    for item in types:
        grouped[item.namespace].append(item)
    ordered_groups = sorted(grouped.items())
    columns = 3
    gutter = 18
    column_width = (PAGE_WIDTH - 2 * MARGIN - gutter * (columns - 1)) / columns
    top = PAGE_HEIGHT - 70
    bottom = 40
    column = 0
    cursor_y = top
    current_page = page_number
    header_title = "4. Полный каталог типов по namespace"
    header_subtitle = f"{len(types)} declarations"
    draw_page_header(pdf, header_title, header_subtitle)

    for namespace, namespace_types in ordered_groups:
        lines = [
            item.uml_label
            + (f" : {', '.join(item.bases)}" if item.bases else "")
            for item in sorted(namespace_types, key=lambda value: value.name)
        ]
        height = 44 + len(lines) * 8.5
        if height > cursor_y - bottom:
            column += 1
            cursor_y = top
        if column >= columns:
            draw_page_header(pdf, header_title, header_subtitle)
            draw_page_footer(pdf, current_page)
            pdf.showPage()
            current_page += 1
            column = 0
            cursor_y = top
            header_title = "4. Каталог типов по namespace (продолжение)"
            draw_page_header(pdf, header_title, header_subtitle)
        x = MARGIN + column * (column_width + gutter)
        area = namespace.split(".")[1]
        draw_box(
            pdf,
            x,
            cursor_y - height,
            column_width,
            height,
            namespace,
            lines,
            AREA_COLORS.get(area, colors.HexColor("#F1F3F5")),
            6.5,
        )
        cursor_y -= height + 10

    draw_page_header(pdf, header_title, header_subtitle)
    draw_page_footer(pdf, current_page)
    pdf.showPage()
    return current_page + 1


def render_dependency_appendix(
    pdf: canvas.Canvas,
    edges: dict[tuple[str, str], int],
    page_number: int,
) -> None:
    draw_page_header(pdf, "5. Матрица фактических межмодульных импортов")
    areas = list(AREA_COLORS)
    table_x = 220
    table_y = 610
    cell_w = 105
    cell_h = 46
    pdf.setFont(FONT_BOLD, 7)
    pdf.setFillColor(colors.HexColor("#263238"))
    for index, area in enumerate(areas):
        pdf.saveState()
        pdf.translate(table_x + index * cell_w + cell_w / 2, table_y + 18)
        pdf.rotate(35)
        pdf.drawCentredString(0, 0, area)
        pdf.restoreState()
        pdf.drawRightString(table_x - 10, table_y - (index + 1) * cell_h + 17, area)
        for target_index, target in enumerate(areas):
            x = table_x + target_index * cell_w
            y = table_y - (index + 1) * cell_h
            value = edges.get((area, target), 0)
            fill = colors.HexColor("#F8FAFC")
            if value:
                fill = AREA_COLORS[target]
            pdf.setFillColor(fill)
            pdf.setStrokeColor(colors.HexColor("#CBD2D9"))
            pdf.rect(x, y, cell_w, cell_h, fill=1, stroke=1)
            pdf.setFillColor(colors.HexColor("#263238"))
            pdf.setFont(FONT_BOLD if value else FONT, 9)
            pdf.drawCentredString(x + cell_w / 2, y + 16, str(value) if value else "—")
    pdf.setFont(FONT, 8)
    notes = [
        "Строка — исходная область; столбец — импортируемая область.",
        "Диагональ опущена: внутримодульные связи не считаются.",
        "Нулевое значение подтверждает отсутствие прямого namespace-направления.",
        "Валидатор проверяет допустимость каждой ненулевой связи с учётом folder-specific исключений.",
    ]
    for index, note in enumerate(notes):
        pdf.drawString(220, 160 - index * 17, f"• {note}")
    draw_page_footer(pdf, page_number)
    pdf.showPage()


def generate(project_root: Path, output: Path) -> None:
    scripts_root = project_root / "Assets" / "_Project" / "Scripts"
    types, edges = parse_types(scripts_root)
    output.parent.mkdir(parents=True, exist_ok=True)
    register_fonts()
    pdf = canvas.Canvas(str(output), pagesize=PAGE_SIZE, pageCompression=1)
    pdf.setTitle("LL.Runtime — Architecture UML Atlas")
    pdf.setAuthor("LL project architecture tooling")
    pdf.setSubject("UML overview, lifetimes, data flows, and complete type inventory")
    page_number = 1
    render_overview(pdf, types, edges, page_number)
    page_number += 1
    render_composition(pdf, page_number)
    page_number += 1
    render_flows(pdf, page_number)
    page_number += 1
    page_number = render_inventory(pdf, types, page_number)
    render_dependency_appendix(pdf, edges, page_number)
    pdf.save()


def main() -> None:
    parser = argparse.ArgumentParser(description="Generate the LL.Runtime UML architecture PDF.")
    parser.add_argument(
        "--project-root",
        type=Path,
        default=Path(__file__).resolve().parent.parent,
    )
    parser.add_argument(
        "--output",
        type=Path,
        default=Path("Documentation/Architecture/LL-Runtime-UML.pdf"),
    )
    args = parser.parse_args()
    project_root = args.project_root.resolve()
    output = args.output
    if not output.is_absolute():
        output = project_root / output
    generate(project_root, output)
    print(output)


if __name__ == "__main__":
    main()
