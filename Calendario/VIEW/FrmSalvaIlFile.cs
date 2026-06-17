using Calendario.CONTROLLER;
using Calendario.MODEL;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Calendario.VIEW
{
    public partial class FrmSalvaIlFile : Form
    {
        private int annoSelezionato = DateTime.Now.Year;
        private readonly PrenotazioneController _gestionePrenotazione;
        private string filePath = "";
        private string selectedPath = "";
        private bool creato = false;

        static readonly Color BG = Color.FromArgb(10, 13, 28);
        static readonly Color CARD_BG = Color.FromArgb(22, 28, 62);
        static readonly Color TEXT = Color.FromArgb(226, 232, 255);
        static readonly Color ACCENT = Color.FromArgb(124, 58, 237);
        static readonly Color INPUT_BG = Color.FromArgb(30, 38, 80);

        private ComboBox cmbAnni;
        private TextBox txtPath;
        private ModernButton btnOpenSrc, btnSalva, btnApriFile, btnCancel;

        public FrmSalvaIlFile(PrenotazioneController gestionePrenotazioni)
        {
            InitializeComponent();
            _gestionePrenotazione = gestionePrenotazioni;

            this.BackColor = BG;
            this.ForeColor = TEXT;
            this.Font = new Font("Segoe UI", 10F);
            this.MinimumSize = new Size(620, 360);
            this.Size = new Size(700, 400);
            this.Text = "Esporta in Word";
            this.StartPosition = FormStartPosition.CenterParent;

            var mainLayout = new TableLayoutPanel {
                Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2,
                BackColor = Color.Transparent, Padding = new Padding(30)
            };
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50)); // Title
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); // Card
            this.Controls.Add(mainLayout);

            var titleLbl = new Label { Text = "Esporta in Word", Font = new Font("Segoe UI", 16F, FontStyle.Bold), ForeColor = Color.White, Dock = DockStyle.Top, Height = 40, TextAlign = ContentAlignment.MiddleCenter };
            mainLayout.Controls.Add(titleLbl, 0, 0);

            // ─── CARD CENTRALE ───────────────────────────────────────────────────
            var card = new ModernPanel { Dock = DockStyle.Fill, Margin = new Padding(0, 10, 0, 10), Padding = new Padding(20) };
            mainLayout.Controls.Add(card, 0, 1);

            var cardLayout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 4 };
            cardLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 60)); // Year
            cardLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 65)); // Path
            cardLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); // Spacer
            cardLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50)); // Buttons
            card.Controls.Add(cardLayout);

            // Year Selection
            var pnlYear = new Panel { Dock = DockStyle.Fill, Margin = new Padding(0) };
            cardLayout.Controls.Add(pnlYear, 0, 0);
            pnlYear.Controls.Add(new Label { Text = "Seleziona Anno:", ForeColor = Color.LightGray, Dock = DockStyle.Top, Height = 25 });
            cmbAnni = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList, BackColor = INPUT_BG, ForeColor = TEXT, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 11F) };
            var yearContainer = new ModernInputContainer { Dock = DockStyle.Top, Height = 35 };
            yearContainer.Controls.Add(cmbAnni);
            pnlYear.Controls.Add(yearContainer);

            // Path Selection
            var pnlPath = new Panel { Dock = DockStyle.Fill, Margin = new Padding(0) };
            cardLayout.Controls.Add(pnlPath, 0, 1);
            pnlPath.Controls.Add(new Label { Text = "Percorso di Salvataggio:", ForeColor = Color.LightGray, Dock = DockStyle.Top, Height = 25 });
            
            var pathTlp = new TableLayoutPanel { Dock = DockStyle.Top, Height = 40, ColumnCount = 2, Margin = new Padding(0) };
            pathTlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            pathTlp.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
            pnlPath.Controls.Add(pathTlp);

            txtPath = new TextBox { Dock = DockStyle.Fill, ReadOnly = true, BackColor = INPUT_BG, ForeColor = TEXT, BorderStyle = BorderStyle.None, Font = new Font("Segoe UI", 11F) };
            var pathContainer = new ModernInputContainer { Dock = DockStyle.Fill, Height = 35 };
            pathContainer.Controls.Add(txtPath);
            btnOpenSrc = new ModernButton { Text = "SFOGLIA", Dock = DockStyle.Fill, Margin = new Padding(10, 0, 0, 0), BackColor = Color.FromArgb(79, 172, 254), ForeColor = Color.Black, Font = new Font("Segoe UI", 10F, FontStyle.Bold) };
            pathTlp.Controls.Add(pathContainer, 0, 0);
            pathTlp.Controls.Add(btnOpenSrc, 1, 0);

            // Buttons
            var pnlButtons = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 3, RowCount = 1, Margin = new Padding(0) };
            pnlButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33f));
            pnlButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33f));
            pnlButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33f));
            cardLayout.Controls.Add(pnlButtons, 0, 3);

            btnCancel = new ModernButton { Text = "CHIUDI", Dock = DockStyle.Fill, Margin = new Padding(4, 0, 4, 0), BackColor = Color.FromArgb(40, 48, 90) };
            btnApriFile = new ModernButton { Text = "APRI FILE", Dock = DockStyle.Fill, Margin = new Padding(4, 0, 4, 0), BackColor = Color.FromArgb(20, 150, 100), Enabled = false };
            btnSalva = new ModernButton { Text = "CREA WORD", Dock = DockStyle.Fill, Margin = new Padding(4, 0, 4, 0), BackColor = ACCENT, Enabled = false };

            pnlButtons.Controls.Add(btnCancel, 0, 0);
            pnlButtons.Controls.Add(btnApriFile, 1, 0);
            pnlButtons.Controls.Add(btnSalva, 2, 0);

            // ─── LOGICA ──────────────────────────────────────────────────────────
            caricaCmb();

            btnOpenSrc.Click += (s, e) => {
                using (FolderBrowserDialog fbD = new FolderBrowserDialog()) {
                    if (fbD.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(fbD.SelectedPath)) {
                        selectedPath = fbD.SelectedPath;
                        aggiornaPercorsoFile();
                        btnSalva.Enabled = true;
                        btnSalva.Text = File.Exists(filePath) ? "AGGIORNA" : "CREA WORD";
                        btnApriFile.Enabled = File.Exists(filePath);
                        creato = File.Exists(filePath);
                    }
                }
            };

            cmbAnni.SelectedIndexChanged += (s, e) => {
                annoSelezionato = (int)cmbAnni.SelectedItem;
                aggiornaPercorsoFile();
                if (!string.IsNullOrWhiteSpace(selectedPath)) {
                    btnSalva.Text = File.Exists(filePath) ? "AGGIORNA" : "CREA WORD";
                    btnApriFile.Enabled = File.Exists(filePath);
                    creato = File.Exists(filePath);
                }
            };

            btnApriFile.Click += (s, e) => {
                if (File.Exists(filePath)) {
                    try { Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true }); }
                    catch (Exception ex) { MessageBox.Show($"Errore: {ex.Message}", "ERRORE", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                } else {
                    MessageBox.Show("Il file non esiste.", "ERRORE", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            };

            btnCancel.Click += (s, e) => this.Close();

            btnSalva.Click += (s, e) => {
                var prenotazioniAnno = GetPrenotazioniPerAnno();
                if (prenotazioniAnno.Count > 0) {
                    if (string.IsNullOrWhiteSpace(selectedPath)) { MessageBox.Show("Seleziona un percorso.", "ERRORE", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
                    try {
                        SalvaXml.creaFile(filePath);
                        SalvaXml.aggiungiIntestazione(filePath);
                        SalvaXml.aggiungTitolo(filePath, annoSelezionato);
                        SalvaXml.creaTabella(filePath, _gestionePrenotazione, annoSelezionato);
                        creato = true;
                        btnApriFile.Enabled = true;
                        btnSalva.Text = "AGGIORNA";
                        if (MessageBox.Show("File generato con successo! Vuoi aprirlo?", "SUCCESSO", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes) {
                            btnApriFile.PerformClick();
                        }
                    } catch (Exception ex) {
                        MessageBox.Show($"Errore nella creazione: {ex.Message}", "ERRORE", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                } else {
                    MessageBox.Show("Non ci sono prenotazioni per l'anno selezionato.", "ERRORE", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            };
        }

        private void caricaCmb() {
            int annoCorrente = DateTime.Now.Year;
            cmbAnni.Items.Clear();
            for (int i = -5; i <= 5; i++) cmbAnni.Items.Add(annoCorrente + i);
            cmbAnni.SelectedIndex = 5;
        }

        private void aggiornaPercorsoFile() {
            if (!string.IsNullOrWhiteSpace(selectedPath)) {
                filePath = Path.Combine(selectedPath, $"Tp{annoSelezionato}.docx");
                txtPath.Text = filePath;
            } else { txtPath.Text = ""; }
        }

        private List<ClsPrenotazione> GetPrenotazioniPerAnno() {
            try {
                var p = _gestionePrenotazione.GetPrenotazioniPerAnno(annoSelezionato);
                if (p == null || p.Count == 0) return new List<ClsPrenotazione>();
                return p;
            } catch { return new List<ClsPrenotazione>(); }
        }

        // Stub methods per i binding nel file designer
        private void btnApriFile_Click(object sender, EventArgs e) {}
        private void btnOpenSrc_Click(object sender, EventArgs e) {}
        private void btnCancel_Click(object sender, EventArgs e) {}
        private void cmbAnni_SelectedIndexChanged(object sender, EventArgs e) {}
        private void btnSalva_Click(object sender, EventArgs e) {}
    }
}

