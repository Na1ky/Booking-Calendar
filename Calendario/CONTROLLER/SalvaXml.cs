using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Calendario.MODEL;


namespace Calendario.CONTROLLER
{
    internal class SalvaXml
    {
        const int COLLS = 8;
        internal static void creaFile(string filePath)
        {
            using (WordprocessingDocument wordDocument = WordprocessingDocument.Create(filePath, DocumentFormat.OpenXml.WordprocessingDocumentType.Document))
            {
                MainDocumentPart MainPart = wordDocument.AddMainDocumentPart();
                MainPart.Document = new Document();
                Body body = MainPart.Document.AppendChild(new Body());
                wordDocument.Save();
            }
        }

        internal static void aggiungiIntestazione(string filePath)
        {
            using (WordprocessingDocument wordDocument =
                    WordprocessingDocument.Open(filePath, true))
            {
                Body body = wordDocument.MainDocumentPart.Document.Body;
                HeaderPart headerPart =
                    wordDocument.MainDocumentPart.GetPartsOfType<HeaderPart>().FirstOrDefault();

                if (headerPart == null)
                {
                    headerPart = wordDocument.MainDocumentPart.AddNewPart<HeaderPart>();
                }
                Header header = new Header();
                Paragraph paragraph = new Paragraph(new Run(new Text("Creato il giorno: " + DateTime.Now.ToString("dd/MM/yyy"))));
                header.Append(paragraph);
                headerPart.Header = header;
                body.Append(
                    new HeaderReference()
                    { Id = wordDocument.MainDocumentPart.GetIdOfPart(headerPart) }
                );
                headerPart.Header.Save();
            }
        }

        internal static void aggiungTitolo(string filePath, int year)
        {
            using (WordprocessingDocument wordDocument = WordprocessingDocument.Open(filePath, true))
            {
                Body body = wordDocument.MainDocumentPart.Document.Body;
                Paragraph paragraph = new Paragraph();
                ParagraphProperties paragraphProperties = new ParagraphProperties();
                RunProperties titleProperties = new RunProperties();
                Bold bold = new Bold();
                Justification aligment = new Justification() { Val = JustificationValues.Center };
                RunFonts fontFamily = new RunFonts() { Ascii = "Arial" };
                FontSize fontSize = new FontSize() { Val = "48" };
                SpacingBetweenLines spacing = new SpacingBetweenLines() { After = "800" };
                paragraphProperties.Append(aligment, spacing);
                titleProperties.Append(bold, fontFamily, fontSize);
                Run titolo = new Run(new Text("TABELLA PRENOTAZIONI DEL " + year));
                titolo.RunProperties = titleProperties;
                paragraph.ParagraphProperties = paragraphProperties;
                paragraph.Append(titolo);
                body.Append(paragraph);
            }
        }

        internal static void creaTabella(string filePath, PrenotazioneController getstionePrenotazione, int year)
        {
            List<ClsPrenotazione> temp = getstionePrenotazione.getPrenotazione;
            using (WordprocessingDocument wordDocument = WordprocessingDocument.Open(filePath, true))
            {
                MainDocumentPart mainPart = wordDocument.MainDocumentPart;
                Body body = mainPart.Document.Body;
                Table table = new Table();
                TableProperties tableProps = new TableProperties(
                    new TableWidth { Type = TableWidthUnitValues.Pct, Width = "100%" },
                    new TableLayout { Type = TableLayoutValues.Fixed },
                    new TableBorders(
                        new TopBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 2 },
                        new BottomBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 2 },
                        new LeftBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 2 },
                        new RightBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 2 },
                        new InsideHorizontalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 2 },
                        new InsideVerticalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 2 }
                        ),
                    new TableCellMarginDefault(
                        new TopMargin { Width = "100" }
                        )
                    );
                table.AppendChild(tableProps);

                TableRow intestazione = new TableRow();

                string[] intestazioni = { "NOME", "COGNOME", "DATA INIZIO", "DATA FINE", "TIPOLOGIA", "VERSAMENTO", "SPESE" };
                foreach (var intesta in intestazioni)
                {
                    TableCell cell = new TableCell();
                    TableCellProperties cellProperties = new TableCellProperties();
                    Paragraph paragraph = new Paragraph();
                    Run textRun = new Run(new Text(intesta));

                    ParagraphProperties paragraphProperties = new ParagraphProperties();
                    Justification aligmnet = new Justification() { Val = JustificationValues.Center };
                    paragraphProperties.Append(aligmnet);
                    paragraph.ParagraphProperties = paragraphProperties;

                    Shading cellshading = new Shading() { Fill = "00FF00" };
                    cellProperties.Append(cellshading);

                    RunProperties Textproperties = new RunProperties();
                    FontSize fontSize = new FontSize() { Val = "22" };
                    Textproperties.Append(fontSize);

                    textRun.RunProperties = Textproperties;
                    paragraph.AppendChild(textRun);
                    cell.AppendChild(paragraph);
                    cell.TableCellProperties = cellProperties;
                    intestazione.AppendChild(cell);
                }
                table.AppendChild(intestazione);

                double totale = 0;
                double tSpese = 0;
                foreach (ClsPrenotazione p in temp)
                {
                    if (p.Id != "VDZ100" && p.DataInizio.Year == year && p.FamigliaDominici == false)
                    {
                        TableRow row = new TableRow();

                        TableCell nome = new TableCell(new Paragraph(new Run(new Text(p.Nome))));
                        TableCell cognome = new TableCell(new Paragraph(new Run(new Text(p.Cognome))));
                        TableCell dataInizio = new TableCell(new Paragraph(new Run(new Text(p.DataInizio.ToString("dd/MM/yyyy")))));
                        TableCell dataFine = new TableCell(new Paragraph(new Run(new Text(p.DataFine.ToString("dd/MM/yyyy")))));

                        // Versamento
                        TableCell versamento = new TableCell();
                        Paragraph paragrafoVersamento = new Paragraph();
                        ParagraphProperties paragraphPropertiesVersamento = new ParagraphProperties();
                        Justification alignmentVersamento = new Justification() { Val = JustificationValues.Right };
                        paragraphPropertiesVersamento.Append(alignmentVersamento);
                        paragrafoVersamento.ParagraphProperties = paragraphPropertiesVersamento;
                        Run versamentoRun = new Run(new Text(p.Versamento.ToString("N2") + " €"));
                        paragrafoVersamento.Append(versamentoRun);
                        versamento.Append(paragrafoVersamento);

                        // Spese di Pulizia
                        TableCell spese = new TableCell();
                        Paragraph paragrafoSpese = new Paragraph();
                        ParagraphProperties paragraphPropertiesSpese = new ParagraphProperties();
                        Justification alignmentSpese = new Justification() { Val = JustificationValues.Right };
                        paragraphPropertiesSpese.Append(alignmentSpese);
                        paragrafoSpese.ParagraphProperties = paragraphPropertiesSpese;
                        Run speseRun = new Run(new Text(p.SpesePulizia.ToString("N2") + " €"));
                        paragrafoSpese.Append(speseRun);
                        spese.Append(paragrafoSpese);

                        TableCell tipologia = new TableCell(new Paragraph(new Run(new Text(p.TipoPrenotazione ? "SICURA" : "INSICURA"))));

                        row.Append(nome, cognome, dataInizio, dataFine, tipologia, versamento, spese);
                        table.Append(row);

                        totale += p.Versamento;
                        tSpese += p.SpesePulizia;
                    }
                }

                body.Append(table);

                // TOTALE GUADAGNATO
                Paragraph totaleParagrafo = new Paragraph();
                Run totaleTesto = new Run();
                Text testoTotale = new Text("TOTALE GUADAGNATO NELL'ANNO " + year + ": " + totale.ToString("N2") + " €");
                RunProperties totaleRunProperties = new RunProperties();
                totaleRunProperties.Append(new FontSize() { Val = "28" });
                totaleTesto.RunProperties = totaleRunProperties;
                // Proprietà del paragrafo (allineamento e spaziatura)
                ParagraphProperties propietaParagrafoTotale = new ParagraphProperties();
                Justification justification = new Justification() { Val = JustificationValues.Right }; // Allineato a destra
                SpacingBetweenLines spacing = new SpacingBetweenLines() { Before = "1000" }; // Spaziatura uniforme
                propietaParagrafoTotale.Append(justification);
                propietaParagrafoTotale.Append(spacing);

                // Assembla il paragrafo
                totaleParagrafo.ParagraphProperties = propietaParagrafoTotale;
                totaleTesto.Append(testoTotale);
                totaleParagrafo.Append(totaleTesto);
                body.Append(totaleParagrafo);

                // TOTALE SPESE
                Paragraph speseParagrafo = new Paragraph();
                RunProperties speseRunProperties = new RunProperties();
                Run speseTesto = new Run();
                Text speseTotale = new Text("TOTALE SPESE NELL'ANNO " + year + ": " + tSpese.ToString("N2") + " €");
                speseRunProperties.Append(new FontSize() { Val = "28" }); // Dimensione del carattere
                speseTesto.RunProperties = speseRunProperties;

                // Proprietà del paragrafo
                ParagraphProperties propietaParagrafoSpese = new ParagraphProperties();
                Justification align = new Justification() { Val = JustificationValues.Right }; // Allineato a destra
                SpacingBetweenLines space = new SpacingBetweenLines() { Before = "500" }; // Spaziatura coerente
                propietaParagrafoSpese.Append(align);
                propietaParagrafoSpese.Append(space);

                // Assembla il paragrafo
                speseParagrafo.ParagraphProperties = propietaParagrafoSpese;
                speseTesto.Append(speseTotale);
                speseParagrafo.Append(speseTesto);
                body.Append(speseParagrafo);

                // PROFITTO
                Paragraph profittoParagrafo = new Paragraph();
                Run profittoTesto = new Run();
                RunProperties profittoRunProperties = new RunProperties();
                Text profittoTotale = new Text("PROFITTO NELL'ANNO " + year + ": " + (totale - tSpese).ToString("N2") + " €");
                if (totale - tSpese > 0)
                    profittoRunProperties.Append(new Color() { Val = "00FF00" }); // Colore verde
                else
                    profittoRunProperties.Append(new Color() { Val = "FF0000" }); // Colore rosso

                profittoRunProperties.Append(new FontSize() { Val = "28" }); // Dimensione del carattere
                profittoTesto.RunProperties = profittoRunProperties;
                // Proprietà del paragrafo
                ParagraphProperties propietaParagrafoProfitto = new ParagraphProperties();
                Justification alignprofitto = new Justification() { Val = JustificationValues.Right }; // Allineato a destra
                SpacingBetweenLines spaceprofitto = new SpacingBetweenLines() { Before = "500" }; // Spaziatura coerente
                propietaParagrafoProfitto.Append(alignprofitto);
                propietaParagrafoProfitto.Append(spaceprofitto);

                // Assembla il paragrafo
                profittoParagrafo.ParagraphProperties = propietaParagrafoProfitto;
                profittoTesto.Append(profittoTotale);
                profittoParagrafo.Append(profittoTesto);
                body.Append(profittoParagrafo);

                // Salva il documento
                wordDocument.MainDocumentPart.Document.Save();

            }
        }
    }
}
