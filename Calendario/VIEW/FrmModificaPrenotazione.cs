using Calendario.CONTROLLER;
using Calendario.MODEL;

using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Calendario.VIEW
{
    public partial class FrmModificaPrenotazione : Form
    {
        private PrenotazioneController _gestionePrenotazioni;
        private string idPrenotazione;
        
        static readonly Color BG       = Color.FromArgb(10, 13, 28);
        static readonly Color ACCENT   = Color.FromArgb(124, 58, 237);
        static readonly Color TEXT     = Color.FromArgb(226, 232, 255);
        static readonly Color INPUT_BG = Color.FromArgb(18, 24, 54);

        private Guna.UI2.WinForms.Guna2ComboBox cmbPrenotazioni, cmbTip;
        private Guna.UI2.WinForms.Guna2TextBox txtN, txtC;
        private Guna.UI2.WinForms.Guna2NumericUpDown nudAcc, nudSp, nudVers;
        private DateTimePicker dtpInizio, dtpFine; // kept for save logic compatibility
        private MonthCalendar calInizio, calFine;
        private Guna.UI2.WinForms.Guna2Button btnSalva;
        private TableLayoutPanel tlpDati;

        public FrmModificaPrenotazione(PrenotazioneController gestionePrenotazioni)
        {
            InitializeComponent();
            _gestionePrenotazioni = gestionePrenotazioni;

            this.BackColor = BG;
            this.ForeColor = TEXT;
            this.Font = new Font("Segoe UI", 10F);
            this.MinimumSize = new Size(900, 460);
            this.Size = new Size(1000, 480);
            this.Text = "Modifica Prenotazione";
            this.StartPosition = FormStartPosition.CenterParent;

            var titleLbl = new Label { Text = "Modifica Prenotazione", Font = new Font("Segoe UI", 16F, FontStyle.Bold), ForeColor = Color.White, Dock = DockStyle.Top, Height = 40, TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(15, 0, 0, 0) };
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

            // ─── LEFT: SELECTOR & DATI ──────────────────────────────────────────────
            var pnlLeftLayout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2, Margin = new Padding(0, 0, 5, 0) };
            pnlLeftLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 85));
            pnlLeftLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            mainLayout.Controls.Add(pnlLeftLayout, 0, 0);

            var pnlTop = new ModernPanel { Dock = DockStyle.Fill, Margin = new Padding(0, 0, 0, 5), Padding = new Padding(15) };
            pnlLeftLayout.Controls.Add(pnlTop, 0, 0);

            var lblSel = new Label { Text = "Seleziona Prenotazione:", ForeColor = Color.FromArgb(79, 172, 254), Font = new Font("Segoe UI", 11F, FontStyle.Bold), Dock = DockStyle.Top, Height = 25 };
            pnlTop.Controls.Add(lblSel);

            cmbPrenotazioni = new Guna.UI2.WinForms.Guna2ComboBox {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent, FillColor = INPUT_BG, ForeColor = TEXT,
                BorderRadius = 6, BorderColor = Color.FromArgb(30, 36, 70),
                Font = new Font("Segoe UI", 10F)
            };
            var selContainer = new Panel { Dock = DockStyle.Top, Height = 36 };
            selContainer.Controls.Add(cmbPrenotazioni);
            pnlTop.Controls.Add(selContainer);
            lblSel.SendToBack();

            var pnlLeft = new ModernPanel { Dock = DockStyle.Fill, Padding = new Padding(15) };
            pnlLeftLayout.Controls.Add(pnlLeft, 0, 1);

            var lblDatiTitle = new Label { Text = "Dettagli Ospite", ForeColor = ACCENT, Font = new Font("Segoe UI", 11F, FontStyle.Bold), Dock = DockStyle.Top, Height = 30 };
            pnlLeft.Controls.Add(lblDatiTitle);

            var tlpDatiLeft = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 3, Padding = new Padding(0, 5, 0, 0) };
            tlpDatiLeft.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            tlpDatiLeft.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            tlpDatiLeft.RowStyles.Add(new RowStyle(SizeType.Absolute, 65));
            tlpDatiLeft.RowStyles.Add(new RowStyle(SizeType.Absolute, 90)); // Spazio extra per Famiglia Dominici
            tlpDatiLeft.RowStyles.Add(new RowStyle(SizeType.Absolute, 65));
            pnlLeft.Controls.Add(tlpDatiLeft);
            lblDatiTitle.SendToBack();

            // ─── RIGHT: DATE & BOTTONI ──────────────────────────────────────────────
            var pnlRightLayout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2, Margin = new Padding(5, 0, 0, 0) };
            pnlRightLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            pnlRightLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 70));
            mainLayout.Controls.Add(pnlRightLayout, 1, 0);

            var pnlRight = new ModernPanel { Dock = DockStyle.Fill, Margin = new Padding(0, 0, 0, 5), Padding = new Padding(15) };
            pnlRightLayout.Controls.Add(pnlRight, 0, 0);

            var lblRightTitle = new Label { Text = "Periodo Soggiorno", ForeColor = Color.FromArgb(79, 172, 254), Font = new Font("Segoe UI", 11F, FontStyle.Bold), Dock = DockStyle.Top, Height = 30 };
            pnlRight.Controls.Add(lblRightTitle);

            // Calendari affiancati — centrati nel pannello
            var calLayout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1 };
            calLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            calLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

            calInizio = new MonthCalendar { Anchor = AnchorStyles.None, MaxSelectionCount = 1, BackColor = Color.FromArgb(18, 24, 54), ForeColor = TEXT, TitleBackColor = Color.FromArgb(40, 28, 90), TitleForeColor = Color.White, TrailingForeColor = Color.FromArgb(80, 90, 140) };
            calFine   = new MonthCalendar { Anchor = AnchorStyles.None, MaxSelectionCount = 1, BackColor = Color.FromArgb(18, 24, 54), ForeColor = TEXT, TitleBackColor = Color.FromArgb(40, 28, 90), TitleForeColor = Color.White, TrailingForeColor = Color.FromArgb(80, 90, 140) };

            void WrapCal(string lbl, MonthCalendar cal, int col) {
                var wrapper = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent };
                var lblCal = new Label { Text = lbl, ForeColor = Color.FromArgb(160, 170, 210), Font = new Font("Segoe UI", 8.5F), TextAlign = ContentAlignment.MiddleCenter, AutoSize = false, Height = 20 };
                var lblData = new Label {
                    Text = "Data selezionata: " + cal.SelectionStart.ToString("dd/MM/yyyy"),
                    ForeColor = Color.FromArgb(124, 58, 237),
                    Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                    TextAlign = ContentAlignment.MiddleCenter,
                    AutoSize = false,
                    Height = 18
                };
                cal.DateChanged += (s, ev) => {
                    lblData.Text = "Data selezionata: " + cal.SelectionStart.ToString("dd/MM/yyyy");
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
                wrapper.Controls.Add(lblData);
                wrapper.Resize += (s, ev) => {
                    int x = Math.Max(0, (wrapper.Width - cal.Width) / 2);
                    int totalH = lblCal.Height + cal.Height + lblData.Height + pnlTime.Height + 5;
                    int y = Math.Max(0, (wrapper.Height - totalH) / 2);
                    lblCal.Location = new Point(x, y);
                    lblCal.Width = cal.Width;
                    cal.Location = new Point(x, lblCal.Bottom);
                    lblData.Location = new Point(x, cal.Bottom + 5);
                    lblData.Width = cal.Width;
                    pnlTime.Location = new Point(x, lblData.Bottom);
                };
                calLayout.Controls.Add(wrapper, col, 0);
            }
            WrapCal("Check-in",  calInizio, 0);
            WrapCal("Check-out", calFine,   1);

            // Aggiungiamo il calLayout prima di tlpDatiRight
            pnlRight.Controls.Add(calLayout);
            lblRightTitle.SendToBack();

            tlpDati = new TableLayoutPanel(); // dummy per non rompere il codice sotto

            // ─── COLORI DISABLED (overlay visivo) ──────────────────────────────────
            Color DISABLED_FG = Color.FromArgb(60, 70, 110);
            Color DISABLED_BG = Color.FromArgb(12, 15, 32);
            Color NORMAL_BG   = Color.FromArgb(18, 24, 54);

            void SetPanelEnabled(TableLayoutPanel tlp, bool enabled) {
                foreach (Control ctrl in tlp.Controls) {
                    foreach (Control child in ctrl.Controls) {
                        if (child is Guna.UI2.WinForms.Guna2TextBox tb) { tb.FillColor = enabled ? NORMAL_BG : DISABLED_BG; tb.ForeColor = enabled ? TEXT : DISABLED_FG; tb.Enabled = enabled; }
                        if (child is Guna.UI2.WinForms.Guna2ComboBox cb) { cb.FillColor = enabled ? NORMAL_BG : DISABLED_BG; cb.ForeColor = enabled ? TEXT : DISABLED_FG; cb.Enabled = enabled; }
                        if (child is Guna.UI2.WinForms.Guna2NumericUpDown nud) { nud.FillColor = enabled ? NORMAL_BG : DISABLED_BG; nud.ForeColor = enabled ? TEXT : DISABLED_FG; nud.UpDownButtonFillColor = enabled ? ACCENT : DISABLED_FG; nud.Enabled = enabled; }
                        if (child is Label l2) l2.ForeColor = enabled ? Color.FromArgb(160, 170, 210) : Color.FromArgb(45, 55, 90);
                    }
                }
                tlp.Enabled = enabled;
            }

            Panel AddInput(string label, Control c, TableLayoutPanel tlp, int col, int row) {
                var p = new Panel { Dock = DockStyle.Fill, Margin = new Padding(5) };
                var lbl = new Label { Text = label, ForeColor = Color.FromArgb(45, 55, 90), Dock = DockStyle.Top, Height = 20, Font = new Font("Segoe UI", 9F), TextAlign = ContentAlignment.MiddleLeft };
                c.Dock = DockStyle.Top;
                c.Height = 36;
                c.Font = new Font("Segoe UI", 10F);
                if (c is Guna.UI2.WinForms.Guna2TextBox tb) { tb.FillColor = DISABLED_BG; tb.ForeColor = DISABLED_FG; tb.BorderRadius = 6; tb.BorderColor = Color.FromArgb(30, 36, 70); }
                else if (c is Guna.UI2.WinForms.Guna2ComboBox cb) { cb.FillColor = DISABLED_BG; cb.ForeColor = DISABLED_FG; cb.BorderRadius = 6; cb.BorderColor = Color.FromArgb(30, 36, 70); }
                else if (c is Guna.UI2.WinForms.Guna2NumericUpDown nud) { nud.FillColor = DISABLED_BG; nud.ForeColor = DISABLED_FG; nud.BorderRadius = 6; nud.BorderColor = Color.FromArgb(30, 36, 70); nud.UpDownButtonFillColor = DISABLED_FG; nud.UpDownButtonForeColor = DISABLED_BG; nud.DecimalPlaces = 2; nud.Maximum = 99999; }
                p.Controls.Add(c);
                p.Controls.Add(lbl);
                tlp.Controls.Add(p, col, row);
                return p;
            }

            txtN = new Guna.UI2.WinForms.Guna2TextBox(); txtC = new Guna.UI2.WinForms.Guna2TextBox();
            cmbTip = new Guna.UI2.WinForms.Guna2ComboBox(); cmbTip.Items.AddRange(new object[] { "SICURA", "INSICURA" });
            nudAcc = new Guna.UI2.WinForms.Guna2NumericUpDown(); nudSp = new Guna.UI2.WinForms.Guna2NumericUpDown(); nudVers = new Guna.UI2.WinForms.Guna2NumericUpDown();
            dtpInizio = new DateTimePicker(); dtpFine = new DateTimePicker(); // mantenuti per logica salvataggio

            AddInput("Nome", txtN, tlpDatiLeft, 0, 0);
            AddInput("Cognome", txtC, tlpDatiLeft, 1, 0);
            var pnlTipo = AddInput("Tipologia", cmbTip, tlpDatiLeft, 0, 1);
            AddInput("Acconto (€)", nudAcc, tlpDatiLeft, 1, 1);
            AddInput("Spese (€)", nudSp, tlpDatiLeft, 0, 2);
            AddInput("Versamento (€)", nudVers, tlpDatiLeft, 1, 2);
            
            // ── Famiglia Dominici checkbox accodata sotto Tipologia ─────────────────
            var chkFamDominici = new Guna.UI2.WinForms.Guna2CheckBox
            {
                Text = "Famiglia Dominici",
                ForeColor = Color.FromArgb(45, 55, 90), // starts disabled
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

            // ── Variabile per mantenere il colore esistente ───────────────────────
            Color coloreScelto = Color.Gray;

            // Variabili per abilitare dopo la selezione
            tlpDati = tlpDatiLeft; // per la compatibilità col blocco sotto
            
            // Applica lo stato disabilitato con overlay visivo
            SetPanelEnabled(tlpDatiLeft, false);
            chkFamDominici.Enabled = false;
            calInizio.Enabled = false;
            calFine.Enabled = false;

            // ─── BUTTONS ───────────────────────────────────────────────────────────
            var pnlOps = new ModernPanel { Dock = DockStyle.Fill, Padding = new Padding(10) };
            pnlRightLayout.Controls.Add(pnlOps, 0, 1);

            var opsTlp = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1 };
            opsTlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            opsTlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            opsTlp.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            pnlOps.Controls.Add(opsTlp);

            btnSalva = new Guna.UI2.WinForms.Guna2Button { Text = "SALVA MODIFICHE", Dock = DockStyle.Fill, Margin = new Padding(4), FillColor = Color.FromArgb(46, 125, 50), Enabled = false, BorderRadius = 6, Font = new Font("Segoe UI", 10F, FontStyle.Bold) };
            var btnAnn = new Guna.UI2.WinForms.Guna2Button { Text = "ANNULLA", Dock = DockStyle.Fill, Margin = new Padding(4), FillColor = Color.FromArgb(40, 48, 90), BorderRadius = 6, Font = new Font("Segoe UI", 10F, FontStyle.Bold) };

            opsTlp.Controls.Add(btnSalva, 0, 0);
            opsTlp.Controls.Add(btnAnn, 1, 0);

            // ─── LOGIC & EVENTS ────────────────────────────────────────────────────
            caricaCmb();

            cmbPrenotazioni.SelectedIndexChanged += (s, e) => {
                if (cmbPrenotazioni.SelectedItem == null) return;
                string selText = cmbPrenotazioni.SelectedItem.ToString();
                var sel = _gestionePrenotazioni.getPrenotazione.FirstOrDefault(p => (p.Cognome + " " + p.Nome) == selText);
                if (sel == null) return;
                
                idPrenotazione = sel.Id;
                txtN.Text = sel.Nome;
                txtC.Text = sel.Cognome;
                nudVers.Value = Convert.ToDecimal(sel.Versamento);
                nudSp.Value = Convert.ToDecimal(sel.SpesePulizia);
                nudAcc.Value = Convert.ToDecimal(sel.Acconto);
                cmbTip.SelectedIndex = sel.TipoPrenotazione ? 0 : 1;
                calInizio.SelectionStart = sel.DataInizio;
                calInizio.SelectionEnd   = sel.DataInizio;
                calFine.SelectionStart   = sel.DataFine;
                calFine.SelectionEnd     = sel.DataFine;
                
                var inTimeData = (dynamic)calInizio.Tag;
                var outTimeData = (dynamic)calFine.Tag;

                if (sel.DataInizio.TimeOfDay.TotalSeconds > 0) {
                    inTimeData.Chk.Checked = true;
                    inTimeData.Dtp.Value = sel.DataInizio;
                } else {
                    inTimeData.Chk.Checked = false;
                }

                if (sel.DataFine.TimeOfDay.TotalSeconds > 0 && sel.DataFine.TimeOfDay < new TimeSpan(23, 59, 59)) {
                    outTimeData.Chk.Checked = true;
                    outTimeData.Dtp.Value = sel.DataFine;
                } else {
                    outTimeData.Chk.Checked = false;
                }
                chkFamDominici.Checked = sel.FamigliaDominici;
                coloreScelto = sel.ColoreCella; // Aggiorna il colore in memoria
                
                // Rimuove lo stato disabilitato
                SetPanelEnabled(tlpDatiLeft, true);
                chkFamDominici.Enabled = true;
                chkFamDominici.ForeColor = ACCENT;
                calInizio.Enabled = true;
                calFine.Enabled = true;
                btnSalva.Enabled = true;
            };

            btnAnn.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };
            btnSalva.Click += (s, e) => {
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

                if (dataIn >= dataOut) { MessageBox.Show("La data/ora di inizio deve precedere la data/ora di fine.", "Attenzione", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

                var lst = _gestionePrenotazioni.getPrenotazione;
                bool sovrap = lst.Any(p => p.Id != idPrenotazione &&
                    (dataIn < p.DataFine && dataOut > p.DataInizio));
                
                if (sovrap) { MessageBox.Show("Sovrapposizione date con un'altra prenotazione!", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
                
                // Usa coloreScelto (mantiene il colore esistente della prenotazione)

                _gestionePrenotazioni.ModificaPrenotazioni(idPrenotazione, txtN.Text, txtC.Text,
                    Convert.ToDouble(nudVers.Value), cmbTip.SelectedIndex != 1,
                    dataIn, dataOut, Convert.ToDouble(nudSp.Value), Convert.ToDouble(nudAcc.Value), chkFamDominici.Checked, coloreScelto);
                
                DialogResult = DialogResult.OK;
                Close();
            };
        }

        private void caricaCmb() {
            var temp = _gestionePrenotazioni.getPrenotazione;
            temp.Sort((x, y) => (x.Cognome + " " + x.Nome).CompareTo(y.Cognome + " " + y.Nome));
            cmbPrenotazioni.Items.Clear();
            foreach (var p in temp) {
                if (p.Id != "VDZ100") cmbPrenotazioni.Items.Add(p.Cognome + " " + p.Nome);
            }
        }

        // Dummy handlers to satisfy the designer file references if any
        private void btnCancel_Click(object sender, EventArgs e) {}
        private void cmbPrenotazioni_SelectedIndexChanged(object sender, EventArgs e) {}
        private void btnSalva_Click(object sender, EventArgs e) {}
        private void DataInizioCalendar_DateChanged(object sender, DateRangeEventArgs e) {}
        private void DataFineCalendar_DateChanged(object sender, DateRangeEventArgs e) {}
    }
}

