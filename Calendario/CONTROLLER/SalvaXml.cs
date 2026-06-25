using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.Linq;
using Calendario.MODEL;

namespace Calendario.CONTROLLER
{
    internal class SalvaXml
    {
        // --- PALETTE COLORI (esadecimale senza #) -----------------------------
        private const string COL_HEADER_BG      = "1E2A4A";
        private const string COL_HEADER_FG      = "FFFFFF";
        private const string COL_ROW_ODD        = "F4F6FB";
        private const string COL_ROW_EVEN       = "FFFFFF";
        private const string COL_SICURA         = "1A7A4A";
        private const string COL_INSICURA       = "C0392B";
        private const string COL_ACCENT         = "2E4099";
        private const string COL_BOX_ENTRATE    = "1A5276";
        private const string COL_BOX_SPESE      = "7D3C98";
        private const string COL_BOX_PROFITTO_P = "1A7A4A";
        private const string COL_BOX_PROFITTO_N = "C0392B";
        private const string COL_BORDER         = "BDC3D0";
        private const string COL_SUBTEXT        = "5D6D7E";
        private const string COL_TITLE_LINE     = "2E4099";

        private const string FONT_MAIN  = "Calibri";
        private const string FONT_TITLE = "Calibri Light";

        private static readonly int[] COL_WIDTHS = { 1500, 1500, 1300, 1300, 1200, 1400, 1400 };

        internal static void creaFile(string filePath)
        {
            using (WordprocessingDocument doc = WordprocessingDocument.Create(
                filePath, WordprocessingDocumentType.Document))
            {
                MainDocumentPart main = doc.AddMainDocumentPart();
                main.Document = new Document();
                Body body = main.Document.AppendChild(new Body());
                SectionProperties sectionProps = new SectionProperties();
                PageMargin pageMargin = new PageMargin()
                {
                    Top    = 720,
                    Bottom = 720,
                    Left   = 900,
                    Right  = 900,
                    Header = 360,
                    Footer = 360
                };
                sectionProps.Append(pageMargin);
                body.Append(sectionProps);
                doc.Save();
            }
        }

        internal static void aggiungiIntestazione(string filePath)
        {
            using (WordprocessingDocument doc = WordprocessingDocument.Open(filePath, true))
            {
                MainDocumentPart main = doc.MainDocumentPart;
                foreach (var hp in main.GetPartsOfType<HeaderPart>().ToList())
                    main.DeletePart(hp);

                HeaderPart headerPart = main.AddNewPart<HeaderPart>();
                Header header = new Header();

                Paragraph hPara = new Paragraph();
                ParagraphProperties hPP = new ParagraphProperties();
                ParagraphBorders hBorders = new ParagraphBorders(
                    new BottomBorder
                    {
                        Val   = new EnumValue<BorderValues>(BorderValues.Single),
                        Size  = 6,
                        Color = COL_ACCENT
                    });
                hPP.Append(hBorders);
                hPP.Append(new Justification { Val = JustificationValues.Right });
                hPP.Append(new SpacingBetweenLines { After = "120" });
                hPara.ParagraphProperties = hPP;

                Run hRun = new Run(new Text("Documento generato il: " + DateTime.Now.ToString("dddd dd MMMM yyyy", new System.Globalization.CultureInfo("it-IT")) + "   |   Sistema Gestione Prenotazioni"));
                RunProperties hRP = new RunProperties();
                hRP.Append(new RunFonts { Ascii = FONT_MAIN, HighAnsi = FONT_MAIN });
                hRP.Append(new FontSize { Val = "16" });
                hRP.Append(new Color { Val = COL_SUBTEXT });
                hRP.Append(new Italic());
                hRun.RunProperties = hRP;
                hPara.Append(hRun);
                header.Append(hPara);
                headerPart.Header = header;
                headerPart.Header.Save();

                string headerId = main.GetIdOfPart(headerPart);
                SectionProperties sectPr = main.Document.Body.Elements<SectionProperties>().FirstOrDefault();
                if (sectPr == null) { sectPr = new SectionProperties(); main.Document.Body.Append(sectPr); }
                foreach (var hr in sectPr.Elements<HeaderReference>().ToList()) sectPr.RemoveChild(hr);
                sectPr.Append(new HeaderReference { Id = headerId, Type = HeaderFooterValues.Default });

                foreach (var fp in main.GetPartsOfType<FooterPart>().ToList())
                    main.DeletePart(fp);

                FooterPart footerPart = main.AddNewPart<FooterPart>();
                Footer footer = new Footer();
                Paragraph fPara = new Paragraph();
                ParagraphProperties fPP = new ParagraphProperties();
                ParagraphBorders fBorders = new ParagraphBorders(
                    new TopBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4, Color = COL_BORDER });
                fPP.Append(fBorders);
                fPP.Append(new Justification { Val = JustificationValues.Center });
                fPara.ParagraphProperties = fPP;

                Run fRunLabel = new Run(new Text("Pagina "));
                ApplyRunStyle(fRunLabel, FONT_MAIN, "16", COL_SUBTEXT, false, false);
                fPara.Append(fRunLabel);
                fPara.Append(new Run(new FieldChar { FieldCharType = FieldCharValues.Begin }));
                fPara.Append(new Run(new FieldCode(" PAGE ") { Space = SpaceProcessingModeValues.Preserve }));
                fPara.Append(new Run(new FieldChar { FieldCharType = FieldCharValues.Separate }));
                fPara.Append(new Run(new FieldChar { FieldCharType = FieldCharValues.End }));

                footer.Append(fPara);
                footerPart.Footer = footer;
                footerPart.Footer.Save();

                string footerId = main.GetIdOfPart(footerPart);
                foreach (var fr in sectPr.Elements<FooterReference>().ToList()) sectPr.RemoveChild(fr);
                sectPr.Append(new FooterReference { Id = footerId, Type = HeaderFooterValues.Default });

                main.Document.Save();
            }
        }

        internal static void aggiungTitolo(string filePath, int year)
        {
            using (WordprocessingDocument doc = WordprocessingDocument.Open(filePath, true))
            {
                Body body = doc.MainDocumentPart.Document.Body;
                SectionProperties sectPr = body.Elements<SectionProperties>().FirstOrDefault();

                body.InsertBefore(MakeSpacerParagraph("200"), sectPr);

                Paragraph subPara = new Paragraph();
                ParagraphProperties subPP = new ParagraphProperties();
                subPP.Append(new Justification { Val = JustificationValues.Center });
                subPP.Append(new SpacingBetweenLines { Before = "0", After = "80" });
                subPara.ParagraphProperties = subPP;
                Run subRun = new Run(new Text("REPORT ANNUALE PRENOTAZIONI"));
                RunProperties subRP = new RunProperties();
                subRP.Append(new RunFonts { Ascii = FONT_TITLE, HighAnsi = FONT_TITLE });
                subRP.Append(new FontSize { Val = "20" });
                subRP.Append(new Color { Val = COL_SUBTEXT });
                subRP.Append(new Spacing { Val = 200 });
                subRP.Append(new Caps());
                subRun.RunProperties = subRP;
                subPara.Append(subRun);
                body.InsertBefore(subPara, sectPr);

                Paragraph titlePara = new Paragraph();
                ParagraphProperties titlePP = new ParagraphProperties();
                titlePP.Append(new Justification { Val = JustificationValues.Center });
                titlePP.Append(new SpacingBetweenLines { Before = "0", After = "160" });
                titlePara.ParagraphProperties = titlePP;
                Run titleRun = new Run(new Text(year.ToString()));
                RunProperties titleRP = new RunProperties();
                titleRP.Append(new RunFonts { Ascii = FONT_TITLE, HighAnsi = FONT_TITLE });
                titleRP.Append(new FontSize { Val = "96" });
                titleRP.Append(new Color { Val = COL_ACCENT });
                titleRP.Append(new Bold());
                titleRun.RunProperties = titleRP;
                titlePara.Append(titleRun);
                body.InsertBefore(titlePara, sectPr);

                Paragraph linePara = new Paragraph();
                ParagraphProperties linePP = new ParagraphProperties();
                linePP.Append(new Justification { Val = JustificationValues.Center });
                linePP.Append(new SpacingBetweenLines { Before = "0", After = "400" });
                ParagraphBorders pBorders = new ParagraphBorders(
                    new BottomBorder
                    {
                        Val   = new EnumValue<BorderValues>(BorderValues.Thick),
                        Size  = 12,
                        Color = COL_TITLE_LINE,
                        Space = 4
                    });
                linePP.Append(pBorders);
                linePara.ParagraphProperties = linePP;
                linePara.Append(new Run(new Text("")));
                body.InsertBefore(linePara, sectPr);

                doc.MainDocumentPart.Document.Save();
            }
        }

        internal static void creaTabella(string filePath, PrenotazioneController getstionePrenotazione, int year)
        {
            List<ClsPrenotazione> temp = getstionePrenotazione.getPrenotazione;

            using (WordprocessingDocument doc = WordprocessingDocument.Open(filePath, true))
            {
                MainDocumentPart mainPart = doc.MainDocumentPart;
                Body body = mainPart.Document.Body;
                SectionProperties sectPr = body.Elements<SectionProperties>().FirstOrDefault();

                var prenotazioni = temp
                    .Where(p => p.Id != "VDZ100" && p.DataInizio.Year == year && p.FamigliaDominici == false)
                    .OrderBy(p => p.DataInizio)
                    .ToList();

                double totaleEntrate = prenotazioni.Sum(p => p.Versamento);
                double totaleSpese   = prenotazioni.Sum(p => p.SpesePulizia);
                double profitto      = totaleEntrate - totaleSpese;

                Table table = new Table();
                table.AppendChild(BuildTableProperties());
                table.AppendChild(BuildHeaderRow());
                for (int i = 0; i < prenotazioni.Count; i++)
                    table.AppendChild(BuildDataRow(prenotazioni[i], i));

                body.InsertBefore(table, sectPr);
                body.InsertBefore(MakeSpacerParagraph("480"), sectPr);

                Paragraph riepilogoLabel = MakeSectionLabel("RIEPILOGO FINANZIARIO");
                body.InsertBefore(riepilogoLabel, sectPr);
                body.InsertBefore(MakeSpacerParagraph("160"), sectPr);

                Table summaryTable = BuildSummaryTable(totaleEntrate, totaleSpese, profitto, year);
                body.InsertBefore(summaryTable, sectPr);

                body.InsertBefore(MakeSpacerParagraph("200"), sectPr);
                Paragraph nota = new Paragraph();
                ParagraphProperties notaPP = new ParagraphProperties();
                notaPP.Append(new Justification { Val = JustificationValues.Right });
                nota.ParagraphProperties = notaPP;
                Run notaRun = new Run(new Text("Totale prenotazioni elaborate: " + prenotazioni.Count));
                ApplyRunStyle(notaRun, FONT_MAIN, "16", COL_SUBTEXT, false, true);
                nota.Append(notaRun);
                body.InsertBefore(nota, sectPr);

                mainPart.Document.Save();
            }
        }

        private static TableProperties BuildTableProperties()
        {
            return new TableProperties(
                new TableWidth { Type = TableWidthUnitValues.Dxa, Width = "9600" },
                new TableLayout { Type = TableLayoutValues.Fixed },
                new TableJustification { Val = TableRowAlignmentValues.Center },
                new TableLook { Val = "04A0" },
                new TableBorders(
                    new TopBorder    { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4, Color = COL_BORDER },
                    new BottomBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4, Color = COL_BORDER },
                    new LeftBorder   { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4, Color = COL_BORDER },
                    new RightBorder  { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4, Color = COL_BORDER },
                    new InsideHorizontalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4, Color = COL_BORDER },
                    new InsideVerticalBorder   { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4, Color = COL_BORDER }
                ),
                new TableCellMarginDefault(
                    new TopMargin    { Type = TableWidthUnitValues.Dxa, Width = "80" },
                    new BottomMargin { Type = TableWidthUnitValues.Dxa, Width = "80" },
                    new LeftMargin   { Type = TableWidthUnitValues.Dxa, Width = "120" },
                    new RightMargin  { Type = TableWidthUnitValues.Dxa, Width = "120" }
                )
            );
        }

        private static TableRow BuildHeaderRow()
        {
            string[] labels = { "Nome", "Cognome", "Data Inizio", "Data Fine", "Tipologia", "Versamento", "Spese Pulizia" };
            TableRow row = new TableRow();
            TableRowProperties trPr = new TableRowProperties();
            trPr.Append(new TableHeader());
            trPr.Append(new TableRowHeight { Val = 500, HeightType = HeightRuleValues.Exact });
            row.Append(trPr);
            for (int i = 0; i < labels.Length; i++)
            {
                JustificationValues align = (i >= 5) ? JustificationValues.Right : JustificationValues.Left;
                row.Append(BuildHeaderCell(labels[i], COL_WIDTHS[i], align));
            }
            return row;
        }

        private static TableCell BuildHeaderCell(string text, int width, JustificationValues align)
        {
            TableCell cell = new TableCell();
            TableCellProperties tcp = new TableCellProperties();
            tcp.Append(new TableCellWidth { Type = TableWidthUnitValues.Dxa, Width = width.ToString() });
            tcp.Append(new Shading { Val = ShadingPatternValues.Clear, Color = "auto", Fill = COL_HEADER_BG });
            tcp.Append(new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center });
            cell.TableCellProperties = tcp;

            Paragraph para = new Paragraph();
            ParagraphProperties pp = new ParagraphProperties();
            pp.Append(new Justification { Val = align });
            pp.Append(new SpacingBetweenLines { Before = "0", After = "0" });
            para.ParagraphProperties = pp;

            Run run = new Run(new Text(text));
            RunProperties rp = new RunProperties();
            rp.Append(new RunFonts { Ascii = FONT_MAIN, HighAnsi = FONT_MAIN });
            rp.Append(new FontSize { Val = "20" });
            rp.Append(new Bold());
            rp.Append(new Color { Val = COL_HEADER_FG });
            rp.Append(new Spacing { Val = 80 });
            rp.Append(new Caps());
            run.RunProperties = rp;
            para.Append(run);
            cell.Append(para);
            return cell;
        }

        private static TableRow BuildDataRow(ClsPrenotazione p, int rowIndex)
        {
            string rowColor  = (rowIndex % 2 == 0) ? COL_ROW_ODD : COL_ROW_EVEN;
            string tipoColor = p.TipoPrenotazione ? COL_SICURA : COL_INSICURA;
            string tipoText  = p.TipoPrenotazione ? "SICURA" : "INSICURA";

            TableRow row = new TableRow();
            TableRowProperties trPr = new TableRowProperties();
            trPr.Append(new TableRowHeight { Val = 420 });
            row.Append(trPr);

            row.Append(BuildDataCell(p.Nome,                                COL_WIDTHS[0], rowColor, JustificationValues.Left,   FONT_MAIN, "19", "2D3436", false, false));
            row.Append(BuildDataCell(p.Cognome,                             COL_WIDTHS[1], rowColor, JustificationValues.Left,   FONT_MAIN, "19", "2D3436", false, false));
            row.Append(BuildDataCell(p.DataInizio.ToString("dd/MM/yyyy"),   COL_WIDTHS[2], rowColor, JustificationValues.Center, FONT_MAIN, "19", "2D3436", false, false));
            row.Append(BuildDataCell(p.DataFine.ToString("dd/MM/yyyy"),     COL_WIDTHS[3], rowColor, JustificationValues.Center, FONT_MAIN, "19", "2D3436", false, false));
            row.Append(BuildDataCell(tipoText,                              COL_WIDTHS[4], rowColor, JustificationValues.Center, FONT_MAIN, "18", tipoColor, true, false));
            row.Append(BuildDataCell(p.Versamento.ToString("N2") + " EUR",  COL_WIDTHS[5], rowColor, JustificationValues.Right,  FONT_MAIN, "19", "2D3436", false, false));
            row.Append(BuildDataCell(p.SpesePulizia.ToString("N2") + " EUR",COL_WIDTHS[6], rowColor, JustificationValues.Right,  FONT_MAIN, "19", "2D3436", false, false));

            return row;
        }

        private static TableCell BuildDataCell(string text, int width, string bgColor, JustificationValues align,
            string font, string fontSize, string textColor, bool bold, bool italic)
        {
            TableCell cell = new TableCell();
            TableCellProperties tcp = new TableCellProperties();
            tcp.Append(new TableCellWidth { Type = TableWidthUnitValues.Dxa, Width = width.ToString() });
            tcp.Append(new Shading { Val = ShadingPatternValues.Clear, Color = "auto", Fill = bgColor });
            tcp.Append(new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center });
            cell.TableCellProperties = tcp;

            Paragraph para = new Paragraph();
            ParagraphProperties pp = new ParagraphProperties();
            pp.Append(new Justification { Val = align });
            pp.Append(new SpacingBetweenLines { Before = "0", After = "0" });
            para.ParagraphProperties = pp;

            Run run = new Run(new Text(text));
            ApplyRunStyle(run, font, fontSize, textColor, bold, italic);
            para.Append(run);
            cell.Append(para);
            return cell;
        }

        private static Table BuildSummaryTable(double entrate, double spese, double profitto, int year)
        {
            Table t = new Table();
            t.AppendChild(new TableProperties(
                new TableWidth { Type = TableWidthUnitValues.Dxa, Width = "9600" },
                new TableLayout { Type = TableLayoutValues.Fixed },
                new TableJustification { Val = TableRowAlignmentValues.Center },
                new TableBorders(
                    new TopBorder    { Val = new EnumValue<BorderValues>(BorderValues.None) },
                    new BottomBorder { Val = new EnumValue<BorderValues>(BorderValues.None) },
                    new LeftBorder   { Val = new EnumValue<BorderValues>(BorderValues.None) },
                    new RightBorder  { Val = new EnumValue<BorderValues>(BorderValues.None) },
                    new InsideHorizontalBorder { Val = new EnumValue<BorderValues>(BorderValues.None) },
                    new InsideVerticalBorder   { Val = new EnumValue<BorderValues>(BorderValues.None) }
                ),
                new TableCellMarginDefault(
                    new LeftMargin  { Type = TableWidthUnitValues.Dxa, Width = "160" },
                    new RightMargin { Type = TableWidthUnitValues.Dxa, Width = "160" }
                )
            ));

            string profittoColor = profitto >= 0 ? COL_BOX_PROFITTO_P : COL_BOX_PROFITTO_N;
            TableRow row = new TableRow();
            TableRowProperties trPr = new TableRowProperties();
            trPr.Append(new TableRowHeight { Val = 900, HeightType = HeightRuleValues.Exact });
            row.Append(trPr);

            row.Append(BuildSummaryBox("TOTALE ENTRATE",  entrate.ToString("N2") + " EUR",  COL_BOX_ENTRATE,    3100));
            row.Append(BuildSummaryBox("TOTALE SPESE",    spese.ToString("N2")   + " EUR",  COL_BOX_SPESE,      3100));
            row.Append(BuildSummaryBox("PROFITTO NETTO",  profitto.ToString("N2") + " EUR", profittoColor,      3400));

            t.Append(row);
            return t;
        }

        private static TableCell BuildSummaryBox(string label, string value, string bgColor, int width)
        {
            TableCell cell = new TableCell();
            TableCellProperties tcp = new TableCellProperties();
            tcp.Append(new TableCellWidth { Type = TableWidthUnitValues.Dxa, Width = width.ToString() });
            tcp.Append(new Shading { Val = ShadingPatternValues.Clear, Color = "auto", Fill = bgColor });
            tcp.Append(new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center });
            tcp.Append(new TableCellMargin(
                new TopMargin    { Type = TableWidthUnitValues.Dxa, Width = "160" },
                new BottomMargin { Type = TableWidthUnitValues.Dxa, Width = "160" },
                new LeftMargin   { Type = TableWidthUnitValues.Dxa, Width = "240" },
                new RightMargin  { Type = TableWidthUnitValues.Dxa, Width = "240" }
            ));
            cell.TableCellProperties = tcp;

            // Label
            Paragraph labelPara = new Paragraph();
            ParagraphProperties lPP = new ParagraphProperties();
            lPP.Append(new Justification { Val = JustificationValues.Center });
            lPP.Append(new SpacingBetweenLines { Before = "0", After = "60" });
            labelPara.ParagraphProperties = lPP;
            Run labelRun = new Run(new Text(label));
            RunProperties labelRP = new RunProperties();
            labelRP.Append(new RunFonts { Ascii = FONT_MAIN, HighAnsi = FONT_MAIN });
            labelRP.Append(new FontSize { Val = "17" });
            labelRP.Append(new Color { Val = "B2C6E8" });
            labelRP.Append(new Spacing { Val = 120 });
            labelRP.Append(new Caps());
            labelRun.RunProperties = labelRP;
            labelPara.Append(labelRun);
            cell.Append(labelPara);

            // Valore
            Paragraph valuePara = new Paragraph();
            ParagraphProperties vPP = new ParagraphProperties();
            vPP.Append(new Justification { Val = JustificationValues.Center });
            vPP.Append(new SpacingBetweenLines { Before = "0", After = "0" });
            valuePara.ParagraphProperties = vPP;
            Run valueRun = new Run(new Text(value));
            ApplyRunStyle(valueRun, FONT_TITLE, "40", "FFFFFF", true, false);
            valuePara.Append(valueRun);
            cell.Append(valuePara);

            return cell;
        }

        private static void ApplyRunStyle(Run run, string font, string fontSize, string color, bool bold, bool italic)
        {
            RunProperties rp = run.RunProperties ?? new RunProperties();
            rp.RemoveAllChildren();
            rp.Append(new RunFonts { Ascii = font, HighAnsi = font });
            rp.Append(new FontSize { Val = fontSize });
            rp.Append(new Color { Val = color });
            if (bold)   rp.Append(new Bold());
            if (italic) rp.Append(new Italic());
            run.RunProperties = rp;
        }

        private static Paragraph MakeSpacerParagraph(string spacingAfter)
        {
            Paragraph p = new Paragraph();
            ParagraphProperties pp = new ParagraphProperties();
            pp.Append(new SpacingBetweenLines { Before = "0", After = spacingAfter });
            p.ParagraphProperties = pp;
            p.Append(new Run(new Text("")));
            return p;
        }

        private static Paragraph MakeSectionLabel(string text)
        {
            Paragraph para = new Paragraph();
            ParagraphProperties pp = new ParagraphProperties();
            pp.Append(new Justification { Val = JustificationValues.Left });
            pp.Append(new SpacingBetweenLines { Before = "0", After = "80" });
            ParagraphBorders borders = new ParagraphBorders(
                new BottomBorder
                {
                    Val   = new EnumValue<BorderValues>(BorderValues.Single),
                    Size  = 4,
                    Color = COL_BORDER,
                    Space = 4
                });
            pp.Append(borders);
            para.ParagraphProperties = pp;

            Run run = new Run(new Text(text));
            RunProperties rp = new RunProperties();
            rp.Append(new RunFonts { Ascii = FONT_TITLE, HighAnsi = FONT_TITLE });
            rp.Append(new FontSize { Val = "22" });
            rp.Append(new Color { Val = COL_SUBTEXT });
            rp.Append(new Spacing { Val = 160 });
            rp.Append(new Caps());
            run.RunProperties = rp;
            para.Append(run);
            return para;
        }
    }
}

