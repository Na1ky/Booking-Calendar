using Calendario.CONTROLLER;
using Calendario.MODEL;

using System;
using System.Drawing;
using System.Windows.Forms;

namespace Calendario.VIEW
{
    public partial class FrmAggiungiPrenotazione : Form
    {
        private PrenotazioneController _ctrl;
        
        static readonly Color BG       = Color.FromArgb(10, 13, 28);
        static readonly Color ACCENT   = Color.FromArgb(124, 58, 237);
        static readonly Color TEXT     = Color.FromArgb(226, 232, 255);
        static readonly Color INPUT_BG = Color.FromArgb(18, 24, 54);

        private TextBox txtNome, txtCognome;
        private ComboBox cmbTipo;
        private NumericUpDown nudAcc, nudSp, nudVers;
        private MonthCalendar calInizio, calFine;
        private CheckBox chkFamDominici;

        public FrmAggiungiPrenotazione(PrenotazioneController gestionePrenotazioni)
        {
            InitializeComponent();
            _ctrl = gestionePrenotazioni;
            this.BackColor = BG;
            this.ForeColor = TEXT;
            this.Font = new Font("Segoe UI", 10F);
            this.MinimumSize = new Size(900, 460);
            this.Size = new Size(1000, 480);
            this.Text = "Aggiungi Prenotazione";
            this.StartPosition = FormStartPosition.CenterParent;

            var titleLbl = new Label { Text = "Nuova Prenotazione", Font = new Font("Segoe UI", 16F, FontStyle.Bold), ForeColor = Color.White, Dock = DockStyle.Top, Height = 40, TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(15, 0, 0, 0) };
            this.Controls.Add(titleLbl);

            var mainLayout = new TableLayoutPanel {
                Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1,
                BackColor = Color.Transparent, Padding = new Padding(10, 0, 10, 10)
            };
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            this.Controls.Add(mainLayout);
            titleLbl.SendToBack();

            // ─── SINISTRA: DATI OSPITE ──────────────────────────────────────────────
            var pnlLeft = new ModernPanel { Dock = DockStyle.Fill, Margin = new Padding(0, 0, 5, 0), Padding = new Padding(15) };
            mainLayout.Controls.Add(pnlLeft, 0, 0);

            var lblDatiTitle = new Label { Text = "Dettagli Ospite", ForeColor = ACCENT, Font = new Font("Segoe UI", 11F, FontStyle.Bold), Dock = DockStyle.Top, Height = 28 };
            pnlLeft.Controls.Add(lblDatiTitle);

            var tlpDati = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 3, Padding = new Padding(0, 5, 0, 0) };
            tlpDati.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            tlpDati.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            tlpDati.RowStyles.Add(new RowStyle(SizeType.Absolute, 65));
            tlpDati.RowStyles.Add(new RowStyle(SizeType.Absolute, 90)); // Extra space for Famiglia Dominici
            tlpDati.RowStyles.Add(new RowStyle(SizeType.Absolute, 65));
            pnlLeft.Controls.Add(tlpDati);
            lblDatiTitle.SendToBack();

            Panel AddInput(string label, Control c, int col, int row) {
                var p = new Panel { Dock = DockStyle.Fill, Margin = new Padding(4, 2, 4, 2) };
                var lbl = new Label { Text = label, ForeColor = Color.FromArgb(160, 170, 210), Dock = DockStyle.Top, Height = 18, Font = new Font("Segoe UI", 8.5F), TextAlign = ContentAlignment.MiddleLeft };
                var container = new ModernInputContainer { Dock = DockStyle.Top, Height = 36 };
                c.Dock = DockStyle.Fill;
                c.Font = new Font("Segoe UI", 10F);
                if (c is TextBox tb) { tb.BackColor = INPUT_BG; tb.ForeColor = TEXT; tb.BorderStyle = BorderStyle.None; }
                else if (c is ComboBox cb) { cb.BackColor = INPUT_BG; cb.ForeColor = TEXT; cb.FlatStyle = FlatStyle.Flat; cb.DropDownStyle = ComboBoxStyle.DropDownList; }
                else if (c is NumericUpDown nud) { nud.BackColor = INPUT_BG; nud.ForeColor = TEXT; nud.BorderStyle = BorderStyle.None; nud.DecimalPlaces = 2; nud.Maximum = 99999; nud.TextAlign = HorizontalAlignment.Center; }
                container.Controls.Add(c);
                p.Controls.Add(container);
                p.Controls.Add(lbl);
                tlpDati.Controls.Add(p, col, row);
                return p;
            }

            txtNome = new TextBox();
            txtCognome = new TextBox();
            cmbTipo = new ComboBox(); cmbTipo.Items.AddRange(new object[] { "SICURA", "INSICURA" }); cmbTipo.SelectedIndex = 0;
            nudAcc = new NumericUpDown();
            nudSp = new NumericUpDown();
            nudVers = new NumericUpDown();

            AddInput("Nome", txtNome, 0, 0);
            AddInput("Cognome", txtCognome, 1, 0);
            var pnlTipo = AddInput("Tipologia", cmbTipo, 0, 1);
            AddInput("Acconto (€)", nudAcc, 1, 1);
            AddInput("Spese (€)", nudSp, 0, 2);
            AddInput("Versamento (€)", nudVers, 1, 2);

            // ── Famiglia Dominici checkbox accodata sotto Tipologia ─────────────────
            chkFamDominici = new CheckBox
            {
                Text = "Famiglia Dominici",
                ForeColor = ACCENT,
                Font = new Font("Segoe UI", 9.0F),
                BackColor = Color.Transparent,
                AutoSize = true,
                Margin = new Padding(0),
                Cursor = Cursors.Hand
            };
            chkFamDominici.FlatStyle = FlatStyle.Flat;
            chkFamDominici.FlatAppearance.BorderColor = ACCENT;
            
            var pnlChk = new Panel { Dock = DockStyle.Bottom, Height = 26, BackColor = Color.Transparent };
            chkFamDominici.Location = new Point(0, 4);
            pnlChk.Controls.Add(chkFamDominici);
            pnlTipo.Controls.Add(pnlChk);

            // ─── DESTRA: CALENDARIO ──────────────────────────────────────────────
            var pnlRightLayout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2, Margin = new Padding(5, 0, 0, 0) };
            pnlRightLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            pnlRightLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 72));
            mainLayout.Controls.Add(pnlRightLayout, 1, 0);

            var pnlDate = new ModernPanel { Dock = DockStyle.Fill, Margin = new Padding(0, 0, 0, 5), Padding = new Padding(12) };
            pnlRightLayout.Controls.Add(pnlDate, 0, 0);

            var lblDateTitle = new Label { Text = "Periodo Soggiorno", ForeColor = Color.FromArgb(79, 172, 254), Font = new Font("Segoe UI", 11F, FontStyle.Bold), Dock = DockStyle.Top, Height = 28 };
            pnlDate.Controls.Add(lblDateTitle);

            // Due calendari affiancati — centrati nel loro spazio
            var calLayout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1 };
            calLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            calLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            pnlDate.Controls.Add(calLayout);
            lblDateTitle.SendToBack();

            void AddCal(string label, MonthCalendar cal, int col) {
                // wrapper: centra il calendario e mostra la data selezionata sotto
                var wrapper = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent };
                var lblCal = new Label {
                    Text = label,
                    ForeColor = Color.FromArgb(160, 170, 210),
                    Font = new Font("Segoe UI", 8.5F),
                    TextAlign = ContentAlignment.MiddleCenter,
                    AutoSize = false,
                    Height = 20,
                    Dock = DockStyle.Top
                };
                // Label data selezionata — appare sotto il calendario
                var lblSel = new Label {
                    Text = "Data selezionata: " + cal.SelectionStart.ToString("dd/MM/yyyy"),
                    ForeColor = Color.FromArgb(124, 58, 237),
                    Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                    TextAlign = ContentAlignment.MiddleCenter,
                    AutoSize = false,
                    Height = 18
                };
                cal.MaxSelectionCount = 1;
                cal.BackColor = Color.FromArgb(18, 24, 54);
                cal.ForeColor = TEXT;
                cal.TitleBackColor = Color.FromArgb(40, 28, 90);
                cal.TitleForeColor = Color.White;
                cal.TrailingForeColor = Color.FromArgb(80, 90, 140);
                cal.ShowToday = true;
                cal.ShowTodayCircle = true;
                cal.Anchor = AnchorStyles.None;
                // Aggiorna la label quando cambia la selezione
                cal.DateChanged += (s, ev) => {
                    lblSel.Text = "Data selezionata: " + cal.SelectionStart.ToString("dd/MM/yyyy");
                };

                wrapper.Controls.Add(cal);
                wrapper.Controls.Add(lblCal);
                wrapper.Controls.Add(lblSel);
                wrapper.Resize += (s, ev) => {
                    int x = Math.Max(0, (wrapper.Width - cal.Width) / 2);
                    int y = lblCal.Height + Math.Max(0, (wrapper.Height - lblCal.Height - lblSel.Height - cal.Height) / 2);
                    cal.Location = new Point(x, y);
                    lblCal.Width = cal.Width;
                    lblSel.Width = cal.Width;
                    lblSel.Left  = x;
                    lblSel.Top   = cal.Bottom + 2;
                };
                calLayout.Controls.Add(wrapper, col, 0);
            }

            calInizio = new MonthCalendar();
            calFine   = new MonthCalendar { SelectionStart = DateTime.Today.AddDays(1), SelectionEnd = DateTime.Today.AddDays(1) };
            AddCal("Check-in", calInizio, 0);
            AddCal("Check-out", calFine, 1);

            // Bottoni
            var pnlOps = new ModernPanel { Dock = DockStyle.Fill, Margin = new Padding(0, 5, 0, 0), Padding = new Padding(8) };
            pnlRightLayout.Controls.Add(pnlOps, 0, 1);

            var opsTlp = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1 };
            opsTlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            opsTlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            pnlOps.Controls.Add(opsTlp);

            var btnSalva = new ModernButton { Text = "SALVA", Dock = DockStyle.Fill, Margin = new Padding(4), BackColor = Color.FromArgb(46, 125, 50) };
            var btnAnn = new ModernButton { Text = "ANNULLA", Dock = DockStyle.Fill, Margin = new Padding(4), BackColor = Color.FromArgb(40, 48, 90) };
            
            opsTlp.Controls.Add(btnSalva, 0, 0);
            opsTlp.Controls.Add(btnAnn, 1, 0);

            // ─── EVENTI ────────────────────────────────────────────────────────────
            btnAnn.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };
            btnSalva.Click += (s, e) => {
                if (string.IsNullOrWhiteSpace(txtNome.Text) || string.IsNullOrWhiteSpace(txtCognome.Text)) {
                    MessageBox.Show("Inserire Nome e Cognome.", "Attenzione", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                DateTime dataIn  = calInizio.SelectionStart.Date;
                DateTime dataOut = calFine.SelectionStart.Date;
                if (dataIn >= dataOut) {
                    MessageBox.Show("La data di Check-in deve precedere quella di Check-out.", "Attenzione", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                try {
                    Color colore = (cmbTipo.SelectedItem.ToString() == "SICURA") ? Color.LightGreen : Color.Yellow;
                    var p = new ClsPrenotazione(
                        Guid.NewGuid().ToString().Substring(0, 8).ToUpper(),
                        txtNome.Text.Trim(), txtCognome.Text.Trim(),
                        (double)nudVers.Value,
                        dataIn, dataOut,
                        cmbTipo.SelectedItem.ToString() == "SICURA",
                        colore,
                        (double)nudSp.Value,
                        (double)nudAcc.Value,
                        false
                    );
                    p.FamigliaDominici = chkFamDominici.Checked;
                    _ctrl.AddPrenotazione(p);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                catch (ArgumentException ex) {
                    // Mostra il messaggio di validazione del modello in modo elegante
                    MessageBox.Show(ex.Message, "Dati non validi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            };
        }
    }
}
