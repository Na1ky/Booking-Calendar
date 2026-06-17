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

        private ComboBox cmbPrenotazioni, cmbTip;
        private TextBox txtN, txtC;
        private NumericUpDown nudAcc, nudSp, nudVers;
        private DateTimePicker dtpInizio, dtpFine; // kept for save logic compatibility
        private MonthCalendar calInizio, calFine;
        private ModernButton btnSalva;
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

            cmbPrenotazioni = new ComboBox {
                Dock = DockStyle.Fill,
                DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = INPUT_BG, ForeColor = TEXT, FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F)
            };
            var selContainer = new ModernInputContainer { Dock = DockStyle.Top, Height = 35 };
            selContainer.Controls.Add(cmbPrenotazioni);
            pnlTop.Controls.Add(selContainer);
            lblSel.SendToBack();

            var pnlLeft = new ModernPanel { Dock = DockStyle.Fill, Padding = new Padding(15) };
            pnlLeftLayout.Controls.Add(pnlLeft, 0, 1);

            var lblDatiTitle = new Label { Text = "Dettagli Ospite", ForeColor = ACCENT, Font = new Font("Segoe UI", 11F, FontStyle.Bold), Dock = DockStyle.Top, Height = 30 };
            pnlLeft.Controls.Add(lblDatiTitle);

            var tlpDatiLeft = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 2, Padding = new Padding(0, 5, 0, 0) };
            tlpDatiLeft.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            tlpDatiLeft.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            tlpDatiLeft.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            tlpDatiLeft.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            pnlLeft.Controls.Add(tlpDatiLeft);
            lblDatiTitle.SendToBack();

            // ─── RIGHT: DATE & BOTTONI ──────────────────────────────────────────────
            var pnlRightLayout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2, Margin = new Padding(5, 0, 0, 0) };
            pnlRightLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            pnlRightLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 70));
            mainLayout.Controls.Add(pnlRightLayout, 1, 0);

            var pnlRight = new ModernPanel { Dock = DockStyle.Fill, Margin = new Padding(0, 0, 0, 5), Padding = new Padding(15) };
            pnlRightLayout.Controls.Add(pnlRight, 0, 0);

            var lblRightTitle = new Label { Text = "Periodo e Costi", ForeColor = Color.FromArgb(79, 172, 254), Font = new Font("Segoe UI", 11F, FontStyle.Bold), Dock = DockStyle.Top, Height = 30 };
            pnlRight.Controls.Add(lblRightTitle);

            var tlpDatiRight = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 2, Padding = new Padding(0, 5, 0, 0) };
            tlpDatiRight.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            tlpDatiRight.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            tlpDatiRight.RowStyles.Add(new RowStyle(SizeType.Absolute, 65)); // acconto/spese
            tlpDatiRight.RowStyles.Add(new RowStyle(SizeType.Absolute, 65)); // versamento
            pnlRight.Controls.Add(tlpDatiRight);

            // Calendari affiancati — centrati nel pannello
            var calLayout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1 };
            calLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            calLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

            calInizio = new MonthCalendar { Anchor = AnchorStyles.None, MaxSelectionCount = 1, BackColor = Color.FromArgb(18, 24, 54), ForeColor = TEXT, TitleBackColor = Color.FromArgb(40, 28, 90), TitleForeColor = Color.White, TrailingForeColor = Color.FromArgb(80, 90, 140) };
            calFine   = new MonthCalendar { Anchor = AnchorStyles.None, MaxSelectionCount = 1, BackColor = Color.FromArgb(18, 24, 54), ForeColor = TEXT, TitleBackColor = Color.FromArgb(40, 28, 90), TitleForeColor = Color.White, TrailingForeColor = Color.FromArgb(80, 90, 140) };

            void WrapCal(string lbl, MonthCalendar cal, int col) {
                var wrapper = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent };
                var lblCal = new Label { Text = lbl, ForeColor = Color.FromArgb(160, 170, 210), Font = new Font("Segoe UI", 8.5F), TextAlign = ContentAlignment.MiddleCenter, AutoSize = false, Height = 20, Dock = DockStyle.Top };
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
                wrapper.Controls.Add(cal);
                wrapper.Controls.Add(lblCal);
                wrapper.Controls.Add(lblData);
                wrapper.Resize += (s, ev) => {
                    int x = Math.Max(0, (wrapper.Width - cal.Width) / 2);
                    int y = lblCal.Height + Math.Max(0, (wrapper.Height - lblCal.Height - lblData.Height - cal.Height) / 2);
                    cal.Location = new Point(x, y);
                    lblCal.Width = cal.Width;
                    lblData.Width = cal.Width;
                    lblData.Left  = x;
                    lblData.Top   = cal.Bottom + 2;
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
                        if (child is ModernInputContainer mic) {
                            mic.BackColor = enabled ? NORMAL_BG : DISABLED_BG;
                            foreach (Control inner in mic.Controls) {
                                inner.ForeColor = enabled ? TEXT : DISABLED_FG;
                                if (inner is TextBox tb2) tb2.BackColor = enabled ? NORMAL_BG : DISABLED_BG;
                                if (inner is ComboBox cb2) cb2.BackColor = enabled ? NORMAL_BG : DISABLED_BG;
                                if (inner is NumericUpDown n2) n2.BackColor = enabled ? NORMAL_BG : DISABLED_BG;
                            }
                        }
                        if (child is Label l2) l2.ForeColor = enabled ? Color.FromArgb(160, 170, 210) : Color.FromArgb(45, 55, 90);
                    }
                }
                tlp.Enabled = enabled;
            }

            void AddInput(string label, Control c, TableLayoutPanel tlp, int col, int row) {
                var p = new Panel { Dock = DockStyle.Fill, Margin = new Padding(5) };
                var lbl = new Label { Text = label, ForeColor = Color.FromArgb(45, 55, 90), Dock = DockStyle.Top, Height = 20, Font = new Font("Segoe UI", 9F), TextAlign = ContentAlignment.MiddleLeft };
                var container = new ModernInputContainer { Dock = DockStyle.Top, Height = 35, BackColor = DISABLED_BG };
                c.Dock = DockStyle.Fill;
                c.Font = new Font("Segoe UI", 10F);
                if (c is TextBox tb) { tb.BackColor = DISABLED_BG; tb.ForeColor = DISABLED_FG; tb.BorderStyle = BorderStyle.None; tb.Margin = new Padding(0, 5, 0, 0); }
                else if (c is ComboBox cb) { cb.BackColor = DISABLED_BG; cb.ForeColor = DISABLED_FG; cb.FlatStyle = FlatStyle.Flat; cb.DropDownStyle = ComboBoxStyle.DropDownList; }
                else if (c is NumericUpDown nud) { nud.BackColor = DISABLED_BG; nud.ForeColor = DISABLED_FG; nud.BorderStyle = BorderStyle.None; nud.DecimalPlaces = 2; nud.Maximum = 99999; nud.TextAlign = HorizontalAlignment.Center; }
                container.Controls.Add(c);
                p.Controls.Add(container);
                p.Controls.Add(lbl);
                tlp.Controls.Add(p, col, row);
            }

            txtN = new TextBox(); txtC = new TextBox();
            cmbTip = new ComboBox(); cmbTip.Items.AddRange(new object[] { "SICURA", "INSICURA" });
            nudAcc = new NumericUpDown(); nudSp = new NumericUpDown(); nudVers = new NumericUpDown();
            dtpInizio = new DateTimePicker(); dtpFine = new DateTimePicker(); // mantenuti per logica salvataggio

            AddInput("Nome", txtN, tlpDatiLeft, 0, 0);
            AddInput("Cognome", txtC, tlpDatiLeft, 1, 0);
            AddInput("Tipologia", cmbTip, tlpDatiLeft, 0, 1);
            
            AddInput("Acconto (€)", nudAcc, tlpDatiRight, 0, 0);
            AddInput("Spese (€)", nudSp, tlpDatiRight, 1, 0);
            AddInput("Versamento (€)", nudVers, tlpDatiRight, 0, 1);
            pnlRight.Controls.Add(tlpDatiRight);

            // Variabili per abilitare dopo la selezione
            tlpDati = tlpDatiLeft; // per la compatibilità col blocco sotto
            var tlpDatiRightRef = tlpDatiRight;
            
            // Applica lo stato disabilitato con overlay visivo
            SetPanelEnabled(tlpDatiLeft, false);
            SetPanelEnabled(tlpDatiRight, false);
            calInizio.Enabled = false;
            calFine.Enabled = false;

            // ─── BUTTONS ───────────────────────────────────────────────────────────
            var pnlOps = new ModernPanel { Dock = DockStyle.Fill, Padding = new Padding(10) };
            pnlRightLayout.Controls.Add(pnlOps, 0, 1);

            var opsTlp = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1 };
            opsTlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            opsTlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            pnlOps.Controls.Add(opsTlp);

            btnSalva = new ModernButton { Text = "SALVA MODIFICHE", Dock = DockStyle.Fill, Margin = new Padding(4), BackColor = Color.FromArgb(46, 125, 50), Enabled = false };
            var btnAnn = new ModernButton { Text = "ANNULLA", Dock = DockStyle.Fill, Margin = new Padding(4), BackColor = Color.FromArgb(40, 48, 90) };

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
                // sync dtpInizio/dtpFine per la logica di salvataggio
                dtpInizio.Value = sel.DataInizio;
                dtpFine.Value   = sel.DataFine;

                // Rimuove lo stato disabilitato
                SetPanelEnabled(tlpDatiLeft, true);
                SetPanelEnabled(tlpDatiRightRef, true);
                calInizio.Enabled = true;
                calFine.Enabled = true;
                btnSalva.Enabled = true;
            };

            btnAnn.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };
            btnSalva.Click += (s, e) => {
                DateTime dataIn  = calInizio.SelectionStart.Date;
                DateTime dataOut = calFine.SelectionStart.Date;
                if (dataIn >= dataOut) { MessageBox.Show("La data di inizio deve precedere la data di fine.", "Attenzione", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

                var lst = _gestionePrenotazioni.getPrenotazione;
                bool sovrap = lst.Any(p => p.Id != idPrenotazione &&
                    ((dataIn >= p.DataInizio && dataIn <= p.DataFine) ||
                     (dataOut >= p.DataInizio && dataOut <= p.DataFine) ||
                     (dataIn <= p.DataInizio && dataOut >= p.DataFine)));
                
                if (sovrap) { MessageBox.Show("Sovrapposizione date con un'altra prenotazione!", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
                
                _gestionePrenotazioni.ModificaPrenotazioni(idPrenotazione, txtN.Text, txtC.Text,
                    Convert.ToDouble(nudVers.Value), cmbTip.SelectedIndex != 1,
                    dataIn, dataOut, Convert.ToDouble(nudSp.Value), Convert.ToDouble(nudAcc.Value));
                
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

