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

        static readonly Color BG       = Color.FromArgb(10, 13, 28);
        static readonly Color CARD_BG  = Color.FromArgb(22, 28, 62);
        static readonly Color TEXT     = Color.FromArgb(226, 232, 255);
        static readonly Color ACCENT   = Color.FromArgb(124, 58, 237);
        static readonly Color INPUT_BG = Color.FromArgb(30, 38, 80);

        public FrmGestionePrezzo(Calendario.CONTROLLER.PrenotazioneController ctrl = null)
        {
            InitializeComponent();
            this.BackColor = BG;
            this.ForeColor = TEXT;
            this.Font = new Font("Segoe UI", 10F);
            this.MinimumSize = new Size(820, 500);
            this.Size = new Size(980, 560);
            this.Text = "Gestione Prezzi e Calcolatore";
            this.StartPosition = FormStartPosition.CenterParent;

            // ─── TITOLO ────────────────────────────────────────────────────────────
            var titleLbl = new Label
            {
                Text = "Prezzi e Calcolo Soggiorno",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.White,
                Dock = DockStyle.Top,
                Height = 44,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(15, 0, 0, 0)
            };
            this.Controls.Add(titleLbl);

            // ─── LAYOUT PRINCIPALE (2 colonne) ─────────────────────────────────────
            var mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = Color.Transparent,
                Padding = new Padding(10, 4, 10, 10)
            };
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 48));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 52));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            this.Controls.Add(mainLayout);
            titleLbl.SendToBack();

            // ─── SINISTRA: TABELLA PREZZI ──────────────────────────────────────────
            var pnlLeft = new ModernPanel
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 0, 5, 0),
                Padding = new Padding(15)
            };
            mainLayout.Controls.Add(pnlLeft, 0, 0);

            var lblPrezziTitle = new Label
            {
                Text = "Prezzi Mensili (Euro / Notte)",
                ForeColor = ACCENT,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                Dock = DockStyle.Top,
                Height = 30
            };
            pnlLeft.Controls.Add(lblPrezziTitle);

            dgvPrezzi = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeColumns = false,
                AllowUserToResizeRows = false,
                MultiSelect = false,
                SelectionMode = DataGridViewSelectionMode.CellSelect,
                EditMode = DataGridViewEditMode.EditOnEnter,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = CARD_BG,
                GridColor = INPUT_BG,
                BorderStyle = BorderStyle.None,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                EnableHeadersVisualStyles = false
            };
            dgvPrezzi.DefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = CARD_BG, ForeColor = TEXT,
                SelectionBackColor = ACCENT, SelectionForeColor = Color.White,
                Font = new Font("Segoe UI", 10F), Padding = new Padding(5)
            };
            dgvPrezzi.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(14, 17, 35), ForeColor = Color.DarkGray,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                Alignment = DataGridViewContentAlignment.MiddleLeft,
                Padding = new Padding(5)
            };
            dgvPrezzi.ColumnHeadersHeight = 35;
            dgvPrezzi.RowTemplate.Height = 28;

            pnlLeft.Controls.Add(dgvPrezzi);
            lblPrezziTitle.SendToBack();

            dgvPrezzi.Columns.Add("Mese", "Mese");
            dgvPrezzi.Columns.Add("Prezzo", "Prezzo");
            dgvPrezzi.Columns[0].ReadOnly = true;
            dgvPrezzi.Columns[1].ReadOnly = false;
            dgvPrezzi.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            string[] mesi = { "Gennaio","Febbraio","Marzo","Aprile","Maggio","Giugno",
                               "Luglio","Agosto","Settembre","Ottobre","Novembre","Dicembre" };
            foreach (string mese in mesi) dgvPrezzi.Rows.Add(mese, 0);
            CaricaPrezzi();

            // ─── DESTRA: LAYOUT 2 RIGHE (calcolatore | pulsanti) ─────────────────
            var rightLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Margin = new Padding(5, 0, 0, 0),
                BackColor = Color.Transparent
            };
            rightLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            rightLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 62));
            mainLayout.Controls.Add(rightLayout, 1, 0);

            // ── CARD CALCOLATORE ──────────────────────────────────────────────────
            var pnlCalcolo = new ModernPanel
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 0, 0, 5),
                Padding = new Padding(15, 12, 15, 12)
            };
            rightLayout.Controls.Add(pnlCalcolo, 0, 0);

            var lblCalcoloTitle = new Label
            {
                Text = "Calcolatore Soggiorno",
                ForeColor = Color.FromArgb(79, 172, 254),
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                Dock = DockStyle.Top,
                Height = 28
            };
            pnlCalcolo.Controls.Add(lblCalcoloTitle);

            // ── Layout verticale: label | calendari | bottone | totale ────────────
            var calcStack = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 4,
                BackColor = Color.Transparent,
                Padding = new Padding(0, 4, 0, 0)
            };
            calcStack.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            calcStack.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            calcStack.RowStyles.Add(new RowStyle(SizeType.Absolute, 26));   // label Check-in / Check-out
            calcStack.RowStyles.Add(new RowStyle(SizeType.Percent, 100));   // MonthCalendar (si espande)
            calcStack.RowStyles.Add(new RowStyle(SizeType.Absolute, 46));   // bottone CALCOLA
            calcStack.RowStyles.Add(new RowStyle(SizeType.Absolute, 52));   // label totale
            pnlCalcolo.Controls.Add(calcStack);
            lblCalcoloTitle.SendToBack();

            // ── Helper colori calendario ─────────────────────────────────────────
            Color calBg    = Color.FromArgb(18, 24, 54);
            Color calFg    = Color.FromArgb(226, 232, 255);
            Color calTitle = Color.FromArgb(124, 58, 237);
            Color calTrail = Color.FromArgb(60, 70, 110);

            // Label Check-in
            var lblIn = new Label
            {
                Text = "Check-in",
                ForeColor = Color.FromArgb(160, 170, 210),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };
            calcStack.Controls.Add(lblIn, 0, 0);

            // Label Check-out
            var lblOut = new Label
            {
                Text = "Check-out",
                ForeColor = Color.FromArgb(160, 170, 210),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };
            calcStack.Controls.Add(lblOut, 1, 0);

            // ── MonthCalendar Check-in ───────────────────────────────────────────
            var wrapIn = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent };
            calCheckIn = new MonthCalendar
            {
                MaxSelectionCount = 1,
                BackColor  = calBg,
                ForeColor  = calFg,
                TitleBackColor = calTitle,
                TitleForeColor = calFg,
                TrailingForeColor = calTrail,
                ShowToday  = true,
                ShowTodayCircle = true
            };
            wrapIn.Controls.Add(calCheckIn);
            // Centra orizzontalmente il calendario nel pannello
            wrapIn.Resize += (s, e) => {
                calCheckIn.Left = Math.Max(0, (wrapIn.Width - calCheckIn.Width) / 2);
                calCheckIn.Top  = Math.Max(0, (wrapIn.Height - calCheckIn.Height) / 2);
            };
            calcStack.Controls.Add(wrapIn, 0, 1);

            // ── MonthCalendar Check-out ──────────────────────────────────────────
            var wrapOut = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent };
            calCheckOut = new MonthCalendar
            {
                MaxSelectionCount = 1,
                BackColor  = calBg,
                ForeColor  = calFg,
                TitleBackColor = calTitle,
                TitleForeColor = calFg,
                TrailingForeColor = calTrail,
                ShowToday  = true,
                ShowTodayCircle = true
            };
            calCheckOut.SetDate(DateTime.Today.AddDays(1));
            wrapOut.Controls.Add(calCheckOut);
            wrapOut.Resize += (s, e) => {
                calCheckOut.Left = Math.Max(0, (wrapOut.Width - calCheckOut.Width) / 2);
                calCheckOut.Top  = Math.Max(0, (wrapOut.Height - calCheckOut.Height) / 2);
            };
            calcStack.Controls.Add(wrapOut, 1, 1);

            // Bottone CALCOLA (span 2 colonne)
            btnCalcola = new ModernButton
            {
                Text = "CALCOLA PREZZO",
                Dock = DockStyle.Fill,
                Margin = new Padding(2, 8, 2, 4),
                BackColor = Color.FromArgb(79, 172, 254),
                ForeColor = Color.Black
            };
            calcStack.Controls.Add(btnCalcola, 0, 2);
            calcStack.SetColumnSpan(btnCalcola, 2);

            // Label totale (span 2 colonne)
            lblTotale = new Label
            {
                Text = "Totale: 0.00",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.LightGreen,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Margin = new Padding(2)
            };
            calcStack.Controls.Add(lblTotale, 0, 3);
            calcStack.SetColumnSpan(lblTotale, 2);

            // ── CARD PULSANTI SALVA / CHIUDI ──────────────────────────────────────
            var pnlOps = new ModernPanel
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 5, 0, 0),
                Padding = new Padding(8)
            };
            rightLayout.Controls.Add(pnlOps, 0, 1);

            var opsTlp = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = Color.Transparent
            };
            opsTlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            opsTlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            pnlOps.Controls.Add(opsTlp);

            btnSalva = new ModernButton
            {
                Text = "SALVA",
                Dock = DockStyle.Fill,
                Margin = new Padding(4),
                BackColor = Color.FromArgb(46, 125, 50)
            };
            btnChiudi = new ModernButton
            {
                Text = "CHIUDI",
                Dock = DockStyle.Fill,
                Margin = new Padding(4),
                BackColor = Color.FromArgb(40, 48, 90)
            };
            opsTlp.Controls.Add(btnSalva, 0, 0);
            opsTlp.Controls.Add(btnChiudi, 1, 0);

            // ─── EVENTI ────────────────────────────────────────────────────────────
            btnCalcola.Click += BtnCalcola_Click;
            btnSalva.Click   += BtnSalva_Click;
            btnChiudi.Click  += (s, e) => this.Close();
        }

        private void BtnCalcola_Click(object sender, EventArgs e)
        {
            DateTime dIn  = calCheckIn.SelectionStart.Date;
            DateTime dOut = calCheckOut.SelectionStart.Date;

            if (dOut <= dIn)
            {
                MessageBox.Show("Check-out deve essere successivo al Check-in", "Errore",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal totale = 0;
            DateTime giorno = dIn;

            try
            {
                while (giorno < dOut)
                {
                    totale += Convert.ToDecimal(dgvPrezzi.Rows[giorno.Month - 1].Cells[1].Value);
                    giorno = giorno.AddDays(1);
                }
                lblTotale.Text = $"Totale: {totale.ToString("#,##0.00")} Euro";
            }
            catch
            {
                MessageBox.Show("Assicurati che i prezzi inseriti siano numerici.", "Errore",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSalva_Click(object sender, EventArgs e)
        {
            try
            {
                var prezzi = new Dictionary<int, decimal>();
                for (int i = 0; i < 12; i++)
                    prezzi.Add(i + 1, Convert.ToDecimal(dgvPrezzi.Rows[i].Cells[1].Value));

                string jsonData = JsonConvert.SerializeObject(prezzi, Formatting.Indented);
                File.WriteAllText(filePrezzi, jsonData);
                MessageBox.Show("Prezzi salvati correttamente!", "Salvataggio",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch
            {
                MessageBox.Show("Errore nel salvataggio. Controlla che i valori siano numerici.", "Errore",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CaricaPrezzi()
        {
            if (!File.Exists(filePrezzi)) return;
            try
            {
                var json = File.ReadAllText(filePrezzi);
                var prezzi = JsonConvert.DeserializeObject<Dictionary<int, decimal>>(json);
                foreach (var p in prezzi)
                    dgvPrezzi.Rows[p.Key - 1].Cells[1].Value = p.Value;
            }
            catch { } // Ignora errori in fase di caricamento
        }
    }
}
