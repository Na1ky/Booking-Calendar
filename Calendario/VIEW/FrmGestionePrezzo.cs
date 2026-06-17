using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;


namespace Calendario.VIEW
{
    public partial class FrmGestionePrezzo : Form
    {
        private DataGridView dgvPrezzi;
        private MonthCalendar calCheckIn;
        private MonthCalendar calCheckOut;
        private Label lblTotale;
        private ModernButton btnCalcola;
        private ModernButton btnSalva;
        private ModernButton btnChiudi;

        private readonly string filePrezzi = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "prezzi_mensili.json");
        
        static readonly Color BG = Color.FromArgb(10, 13, 28);
        static readonly Color CARD_BG = Color.FromArgb(22, 28, 62);
        static readonly Color TEXT = Color.FromArgb(226, 232, 255);
        static readonly Color ACCENT = Color.FromArgb(124, 58, 237);
        static readonly Color INPUT_BG = Color.FromArgb(30, 38, 80);

        public FrmGestionePrezzo(Calendario.CONTROLLER.PrenotazioneController ctrl = null)
        {
            InitializeComponent();
            this.BackColor = BG;
            this.ForeColor = TEXT;
            this.Font = new Font("Segoe UI", 10F);
            this.MinimumSize = new Size(820, 420);
            this.Size = new Size(900, 470);
            this.Text = "Gestione Prezzi e Calcolatore";
            this.StartPosition = FormStartPosition.CenterParent;

            var titleLbl = new Label { Text = "Prezzi e Calcolo Soggiorno", Font = new Font("Segoe UI", 16F, FontStyle.Bold), ForeColor = Color.White, Dock = DockStyle.Top, Height = 40, TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(15, 0, 0, 0) };
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

            // ─── LEFT: TABELLA PREZZI ──────────────────────────────────────────────
            var pnlLeft = new ModernPanel { Dock = DockStyle.Fill, Margin = new Padding(0, 0, 5, 0), Padding = new Padding(15) };
            mainLayout.Controls.Add(pnlLeft, 0, 0);

            var lblPrezziTitle = new Label { Text = "Prezzi Mensili (Euro / Notte)", ForeColor = ACCENT, Font = new Font("Segoe UI", 11F, FontStyle.Bold), Dock = DockStyle.Top, Height = 30 };
            pnlLeft.Controls.Add(lblPrezziTitle);

            dgvPrezzi = new DataGridView {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false, AllowUserToDeleteRows = false, AllowUserToResizeColumns = false, AllowUserToResizeRows = false,
                MultiSelect = false, SelectionMode = DataGridViewSelectionMode.CellSelect, EditMode = DataGridViewEditMode.EditOnEnter,
                RowHeadersVisible = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = CARD_BG, GridColor = INPUT_BG, BorderStyle = BorderStyle.None, CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                EnableHeadersVisualStyles = false
            };
            dgvPrezzi.DefaultCellStyle = new DataGridViewCellStyle { BackColor = CARD_BG, ForeColor = TEXT, SelectionBackColor = ACCENT, SelectionForeColor = Color.White, Font = new Font("Segoe UI", 10F), Padding = new Padding(5) };
            dgvPrezzi.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle { BackColor = Color.FromArgb(14, 17, 35), ForeColor = Color.DarkGray, Font = new Font("Segoe UI", 9.5F, FontStyle.Bold), Alignment = DataGridViewContentAlignment.MiddleLeft, Padding = new Padding(5) };
            dgvPrezzi.ColumnHeadersHeight = 35;
            dgvPrezzi.RowTemplate.Height = 30;
            
            pnlLeft.Controls.Add(dgvPrezzi);
            lblPrezziTitle.SendToBack();

            dgvPrezzi.Columns.Add("Mese", "Mese");
            dgvPrezzi.Columns.Add("Prezzo", "Prezzo");
            dgvPrezzi.Columns[0].ReadOnly = true;
            dgvPrezzi.Columns[1].ReadOnly = false;
            dgvPrezzi.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            string[] mesi = { "Gennaio","Febbraio","Marzo","Aprile","Maggio","Giugno","Luglio","Agosto","Settembre","Ottobre","Novembre","Dicembre" };
            foreach (string mese in mesi) dgvPrezzi.Rows.Add(mese, 0);
            CaricaPrezzi();

            // ─── RIGHT: CALCOLATORE E OPERAZIONI ───────────────────────────────────
            var rightLayout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2, Margin = new Padding(5, 0, 0, 0) };
            rightLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            rightLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 70));
            mainLayout.Controls.Add(rightLayout, 1, 0);

            // CALCOLO CARD
            var pnlCalcolo = new ModernPanel { Dock = DockStyle.Fill, Margin = new Padding(0, 0, 0, 5), Padding = new Padding(15) };
            rightLayout.Controls.Add(pnlCalcolo, 0, 0);

            var lblCalcoloTitle = new Label { Text = "Calcolatore Soggiorno", ForeColor = Color.FromArgb(79, 172, 254), Font = new Font("Segoe UI", 11F, FontStyle.Bold), Dock = DockStyle.Top, Height = 30 };
            pnlCalcolo.Controls.Add(lblCalcoloTitle);

            var calcTlp = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 3, Padding = new Padding(0, 5, 0, 0) };
            calcTlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            calcTlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            calcTlp.RowStyles.Add(new RowStyle(SizeType.Absolute, 70));
            calcTlp.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
            calcTlp.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            pnlCalcolo.Controls.Add(calcTlp);
            lblCalcoloTitle.SendToBack();

            // Due calendari affiancati
            var calLayout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1 };
            calLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            calLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            calcTlp.Controls.Add(calLayout, 0, 0);
            calcTlp.SetColumnSpan(calLayout, 2);

            calCheckIn = new MonthCalendar { Anchor = AnchorStyles.None, MaxSelectionCount = 1, BackColor = Color.FromArgb(18, 24, 54), ForeColor = TEXT, TitleBackColor = Color.FromArgb(40, 28, 90), TitleForeColor = Color.White, TrailingForeColor = Color.FromArgb(80, 90, 140) };
            calCheckOut = new MonthCalendar { Anchor = AnchorStyles.None, MaxSelectionCount = 1, BackColor = Color.FromArgb(18, 24, 54), ForeColor = TEXT, TitleBackColor = Color.FromArgb(40, 28, 90), TitleForeColor = Color.White, TrailingForeColor = Color.FromArgb(80, 90, 140) };
            calCheckOut.SelectionStart = DateTime.Today.AddDays(1);
            calCheckOut.SelectionEnd = DateTime.Today.AddDays(1);

            void WrapCal(string lbl, MonthCalendar cal, int col) {
                var wrapper = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent };
                var lblCal = new Label { Text = lbl, ForeColor = Color.FromArgb(160, 170, 210), Font = new Font("Segoe UI", 8.5F), TextAlign = ContentAlignment.MiddleCenter, AutoSize = false, Height = 20, Dock = DockStyle.Top };
                wrapper.Controls.Add(cal);
                wrapper.Controls.Add(lblCal);
                wrapper.Resize += (s, ev) => {
                    int x = Math.Max(0, (wrapper.Width - cal.Width) / 2);
                    int y = lblCal.Height + Math.Max(0, (wrapper.Height - lblCal.Height - cal.Height) / 2);
                    cal.Location = new Point(x, y);
                    lblCal.Width = cal.Width;
                };
                calLayout.Controls.Add(wrapper, col, 0);
            }

            WrapCal("Check-in", calCheckIn, 0);
            WrapCal("Check-out", calCheckOut, 1);

            btnCalcola = new ModernButton { Text = "CALCOLA PREZZO", Dock = DockStyle.Fill, Margin = new Padding(5, 10, 5, 5), BackColor = Color.FromArgb(79, 172, 254), ForeColor = Color.Black };
            calcTlp.Controls.Add(btnCalcola, 0, 1);
            calcTlp.SetColumnSpan(btnCalcola, 2);

            lblTotale = new Label { Text = "Totale: 0.00", Font = new Font("Segoe UI", 16F, FontStyle.Bold), ForeColor = Color.LightGreen, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter, Margin = new Padding(5) };
            calcTlp.Controls.Add(lblTotale, 0, 2);
            calcTlp.SetColumnSpan(lblTotale, 2);

            // OPERAZIONI
            var pnlOps = new ModernPanel { Dock = DockStyle.Fill, Margin = new Padding(0, 5, 0, 0), Padding = new Padding(10) };
            rightLayout.Controls.Add(pnlOps, 0, 1);

            var opsTlp = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1 };
            opsTlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            opsTlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            pnlOps.Controls.Add(opsTlp);

            btnSalva = new ModernButton { Text = "SALVA", Dock = DockStyle.Fill, Margin = new Padding(4), BackColor = Color.FromArgb(46, 125, 50) };
            btnChiudi = new ModernButton { Text = "CHIUDI", Dock = DockStyle.Fill, Margin = new Padding(4), BackColor = Color.FromArgb(40, 48, 90) };
            opsTlp.Controls.Add(btnSalva, 0, 0);
            opsTlp.Controls.Add(btnChiudi, 1, 0);

            // ─── EVENTI ────────────────────────────────────────────────────────────
            btnCalcola.Click += BtnCalcola_Click;
            btnSalva.Click += BtnSalva_Click;
            btnChiudi.Click += (s, e) => this.Close();
        }

        private void BtnCalcola_Click(object sender, EventArgs e)
        {
            DateTime dIn = calCheckIn.SelectionStart.Date;
            DateTime dOut = calCheckOut.SelectionStart.Date;

            if (dOut <= dIn) {
                MessageBox.Show("Check-out deve essere successivo al Check-in", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal totale = 0;
            DateTime giorno = dIn;

            try {
                while (giorno < dOut) {
                    totale += Convert.ToDecimal(dgvPrezzi.Rows[giorno.Month - 1].Cells[1].Value);
                    giorno = giorno.AddDays(1);
                }
                lblTotale.Text = $"Totale: {totale.ToString("#,##0.00")} Euro";
            } catch {
                MessageBox.Show("Assicurati che i prezzi inseriti siano numerici.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSalva_Click(object sender, EventArgs e)
        {
            try {
                var prezzi = new Dictionary<int, decimal>();
                for (int i = 0; i < 12; i++)
                    prezzi.Add(i + 1, Convert.ToDecimal(dgvPrezzi.Rows[i].Cells[1].Value));

                string jsonData = JsonConvert.SerializeObject(prezzi, Formatting.Indented);
                File.WriteAllText(filePrezzi, jsonData);
                MessageBox.Show("Prezzi salvati correttamente!", "Salvataggio", MessageBoxButtons.OK, MessageBoxIcon.Information);
            } catch {
                MessageBox.Show("Errore nel salvataggio. Controlla che i valori siano numerici.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CaricaPrezzi()
        {
            if (!File.Exists(filePrezzi)) return;
            try {
                var json = File.ReadAllText(filePrezzi);
                var prezzi = JsonConvert.DeserializeObject<Dictionary<int, decimal>>(json);
                foreach (var p in prezzi)
                    dgvPrezzi.Rows[p.Key - 1].Cells[1].Value = p.Value;
            } catch { } // Ignora errori in fase di caricamento
        }
    }
}
