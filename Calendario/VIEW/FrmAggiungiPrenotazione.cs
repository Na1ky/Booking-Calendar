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

        private Guna.UI2.WinForms.Guna2TextBox txtNome, txtCognome;
        private Guna.UI2.WinForms.Guna2ComboBox cmbTipo;
        private Guna.UI2.WinForms.Guna2NumericUpDown nudAcc, nudSp, nudVers;
        private MonthCalendar calInizio, calFine;
        private Guna.UI2.WinForms.Guna2CheckBox chkFamDominici;

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

            this.AutoScroll = false;
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
                c.Dock = DockStyle.Top;
                c.Height = 36;
                c.Font = new Font("Segoe UI", 10F);
                if (c is Guna.UI2.WinForms.Guna2TextBox tb) { tb.FillColor = INPUT_BG; tb.ForeColor = TEXT; tb.BorderRadius = 6; tb.BorderColor = Color.FromArgb(30, 36, 70); }
                else if (c is Guna.UI2.WinForms.Guna2ComboBox cb) { cb.FillColor = INPUT_BG; cb.ForeColor = TEXT; cb.BorderRadius = 6; cb.BorderColor = Color.FromArgb(30, 36, 70); }
                else if (c is Guna.UI2.WinForms.Guna2NumericUpDown nud) { nud.FillColor = INPUT_BG; nud.ForeColor = TEXT; nud.BorderRadius = 6; nud.BorderColor = Color.FromArgb(30, 36, 70); nud.UpDownButtonFillColor = ACCENT; nud.UpDownButtonForeColor = Color.White; nud.DecimalPlaces = 2; nud.Maximum = 99999; }
                p.Controls.Add(c);
                p.Controls.Add(lbl);
                tlpDati.Controls.Add(p, col, row);
                return p;
            }

            txtNome = new Guna.UI2.WinForms.Guna2TextBox();
            txtCognome = new Guna.UI2.WinForms.Guna2TextBox();
            cmbTipo = new Guna.UI2.WinForms.Guna2ComboBox(); cmbTipo.Items.AddRange(new object[] { "SICURA", "INSICURA" }); cmbTipo.SelectedIndex = 0;
            nudAcc = new Guna.UI2.WinForms.Guna2NumericUpDown();
            nudSp = new Guna.UI2.WinForms.Guna2NumericUpDown();
            nudVers = new Guna.UI2.WinForms.Guna2NumericUpDown();

            AddInput("Nome", txtNome, 0, 0);
            AddInput("Cognome", txtCognome, 1, 0);
            var pnlTipo = AddInput("Tipologia", cmbTipo, 0, 1);
            AddInput("Acconto (€)", nudAcc, 1, 1);
            AddInput("Spese (€)", nudSp, 0, 2);
            AddInput("Versamento (€)", nudVers, 1, 2);

            // ── Famiglia Dominici checkbox accodata sotto Tipologia ─────────────────
            chkFamDominici = new Guna.UI2.WinForms.Guna2CheckBox
            {
                Text = "Famiglia Dominici",
                ForeColor = ACCENT,
                Font = new Font("Segoe UI", 9.0F),
                BackColor = Color.Transparent,
                AutoSize = true,
                Margin = new Padding(0),
                Cursor = Cursors.Hand
            };
            chkFamDominici.CheckedState.BorderColor = ACCENT;
            chkFamDominici.CheckedState.FillColor = ACCENT;
            chkFamDominici.CheckedState.BorderRadius = 2;
            chkFamDominici.UncheckedState.BorderColor = ACCENT;
            chkFamDominici.UncheckedState.FillColor = Color.Transparent;
            chkFamDominici.UncheckedState.BorderRadius = 2;
            
            var pnlChk = new Panel { Dock = DockStyle.Bottom, Height = 26, BackColor = Color.Transparent };
            chkFamDominici.Location = new Point(0, 4);
            pnlChk.Controls.Add(chkFamDominici);
            pnlTipo.Controls.Add(pnlChk);

            // ── Selettore Colore (nascosto) ───────────────────────────────────────
            Color selectedColor = Color.FromArgb(new Random().Next(150, 255), new Random().Next(150, 255), new Random().Next(150, 255));

            // ─── DESTRA: CALENDARIO ──────────────────────────────────────────────
            var pnlRightLayout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2, Margin = new Padding(5, 0, 0, 0) };
            pnlRightLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            pnlRightLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 70));
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
                    Height = 20
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

                var pnlTime = new Panel { Dock = DockStyle.None, Height = 45, BackColor = Color.Transparent, Width = 220 };
                var chkTime = new Guna.UI2.WinForms.Guna2CheckBox { Text = "Specifica orario", ForeColor = Color.FromArgb(160, 170, 210), Font = new Font("Segoe UI", 8.5F), AutoSize = true, Location = new Point(10, 10) };
                chkTime.CheckedState.BorderColor = ACCENT; chkTime.CheckedState.FillColor = ACCENT; chkTime.CheckedState.BorderRadius = 2;
                chkTime.UncheckedState.BorderColor = ACCENT; chkTime.UncheckedState.FillColor = Color.Transparent; chkTime.UncheckedState.BorderRadius = 2;
                var dtpTime = new Guna.UI2.WinForms.Guna2DateTimePicker { Format = DateTimePickerFormat.Time, ShowUpDown = true, Width = 80, Location = new Point(130, 8), Visible = false, Value = DateTime.Today.AddHours(col == 0 ? 14 : 10), FillColor = INPUT_BG, ForeColor = TEXT, BorderRadius = 6 };
                chkTime.CheckedChanged += (s, ev) => dtpTime.Visible = chkTime.Checked;
                pnlTime.Controls.Add(chkTime);
                pnlTime.Controls.Add(dtpTime);
                cal.Tag = new { Chk = chkTime, Dtp = dtpTime };

                wrapper.Controls.Add(pnlTime);
                wrapper.Controls.Add(cal);
                wrapper.Controls.Add(lblCal);
                wrapper.Controls.Add(lblSel);
                wrapper.Resize += (s, ev) => {
                    int x = Math.Max(0, (wrapper.Width - cal.Width) / 2);
                    // y centers the block [lblCal + cal + lblSel + pnlTime]
                    int totalH = lblCal.Height + cal.Height + lblSel.Height + pnlTime.Height + 5;
                    int y = Math.Max(0, (wrapper.Height - totalH) / 2);
                    lblCal.Location = new Point(x, y);
                    lblCal.Width = cal.Width;
                    cal.Location = new Point(x, lblCal.Bottom);
                    lblSel.Location = new Point(x, cal.Bottom + 5);
                    lblSel.Width = cal.Width;
                    pnlTime.Location = new Point(x, lblSel.Bottom);
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
            opsTlp.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            pnlOps.Controls.Add(opsTlp);

            var btnSalva = new Guna.UI2.WinForms.Guna2Button { Text = "SALVA", Dock = DockStyle.Fill, Margin = new Padding(4), FillColor = Color.FromArgb(46, 125, 50), BorderRadius = 6, Font = new Font("Segoe UI", 10F, FontStyle.Bold) };
            var btnAnn = new Guna.UI2.WinForms.Guna2Button { Text = "ANNULLA", Dock = DockStyle.Fill, Margin = new Padding(4), FillColor = Color.FromArgb(40, 48, 90), BorderRadius = 6, Font = new Font("Segoe UI", 10F, FontStyle.Bold) };
            
            opsTlp.Controls.Add(btnSalva, 0, 0);
            opsTlp.Controls.Add(btnAnn, 1, 0);

            // ─── EVENTI ────────────────────────────────────────────────────────────
            btnAnn.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };
            btnSalva.Click += (s, e) => {
                if (string.IsNullOrWhiteSpace(txtNome.Text) || string.IsNullOrWhiteSpace(txtCognome.Text)) {
                    MessageBox.Show("Inserire Nome e Cognome.", "Attenzione", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                
                var inTimeData = (dynamic)calInizio.Tag;
                var outTimeData = (dynamic)calFine.Tag;
                
                DateTime dataIn = calInizio.SelectionStart.Date;
                if (inTimeData.Chk.Checked)
                    dataIn = dataIn.Add(inTimeData.Dtp.Value.TimeOfDay);
                
                DateTime dataOut = calFine.SelectionStart.Date;
                if (outTimeData.Chk.Checked)
                    dataOut = dataOut.Add(outTimeData.Dtp.Value.TimeOfDay);
                else
                    dataOut = dataOut.Add(new TimeSpan(23, 59, 59));

                if (dataIn >= dataOut) {
                    MessageBox.Show("La data/ora di Check-in deve precedere quella di Check-out.", "Attenzione", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                try {
                    Color colore = selectedColor;
                    var p = new ClsPrenotazione(
                        MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
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
