using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Calendario.CONTROLLER;
using Calendario.MODEL;

namespace Calendario.VIEW
{
    public partial class FrmTabellaRecapito : Form
    {
        private Guna.UI2.WinForms.Guna2DataGridView dgvTabella;
        private Label lblTitolo;
        private PrenotazioneController _gestioneController;

        static readonly Color BG = Color.FromArgb(10, 13, 28);
        static readonly Color CARD_BG = Color.FromArgb(22, 28, 62);
        static readonly Color TEXT = Color.FromArgb(226, 232, 255);
        static readonly Color HEADER_BG = Color.FromArgb(14, 17, 35);
        static readonly Color GRID_LINE = Color.FromArgb(30, 38, 80);

        public FrmTabellaRecapito(PrenotazioneController gestionePrenotazioni)
        {
            InitializeComponent();
            _gestioneController = gestionePrenotazioni;
            this.TopMost = true;

            this.BackColor = BG;
            this.ForeColor = TEXT;
            this.Font = new Font("Segoe UI", 10F);
            this.MinimumSize = new Size(1000, 600);
            this.Size = new Size(1200, 700);
            this.Text = "Tabella Riepilogo Prenotazioni";
            this.StartPosition = FormStartPosition.CenterScreen;

            var mainLayout = new TableLayoutPanel {
                Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2,
                BackColor = Color.Transparent, Padding = new Padding(20)
            };
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 60)); // Title
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); // Grid
            this.Controls.Add(mainLayout);

            if (lblTitolo == null) lblTitolo = new Label();
            if (dgvTabella == null) dgvTabella = new Guna.UI2.WinForms.Guna2DataGridView();

            lblTitolo.Text = "Riepilogo Prenotazioni";
            lblTitolo.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
            lblTitolo.ForeColor = Color.White;
            lblTitolo.Dock = DockStyle.Fill;
            lblTitolo.TextAlign = ContentAlignment.MiddleCenter;
            mainLayout.Controls.Add(lblTitolo, 0, 0);

            var pnlGrid = new Guna.UI2.WinForms.Guna2Panel { Dock = DockStyle.Fill, Margin = new Padding(0, 10, 0, 0), Padding = new Padding(20), FillColor = CARD_BG, BorderRadius = 12 };
            mainLayout.Controls.Add(pnlGrid, 0, 1);

            ConfiguraUI();
            pnlGrid.Controls.Add(dgvTabella);
            dgvTabella.Dock = DockStyle.Fill;

            IntestaTabella();
            popolaTabella();

            this.Shown += (s, e) => {
                dgvTabella.ClearSelection();
                dgvTabella.CurrentCell = null;
                this.ActiveControl = lblTitolo;
            };
        }

        private void ConfiguraUI()
        {
            dgvTabella.AllowUserToAddRows = false;
            dgvTabella.AllowUserToDeleteRows = false;
            dgvTabella.AllowUserToResizeRows = false;
            dgvTabella.ReadOnly = true;
            dgvTabella.MultiSelect = false;
            dgvTabella.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            
            dgvTabella.BackgroundColor = CARD_BG;
            dgvTabella.BorderStyle = BorderStyle.None;
            dgvTabella.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvTabella.GridColor = GRID_LINE;
            dgvTabella.EnableHeadersVisualStyles = false;
            dgvTabella.RowHeadersVisible = false;
            dgvTabella.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dgvTabella.DefaultCellStyle.BackColor = CARD_BG;
            dgvTabella.DefaultCellStyle.ForeColor = TEXT;
            dgvTabella.DefaultCellStyle.SelectionBackColor = Color.FromArgb(124, 58, 237);
            dgvTabella.DefaultCellStyle.SelectionForeColor = Color.White;
            dgvTabella.DefaultCellStyle.Font = new Font("Segoe UI", 11F);
            dgvTabella.DefaultCellStyle.Padding = new Padding(10, 5, 10, 5);

            dgvTabella.ColumnHeadersDefaultCellStyle.BackColor = HEADER_BG;
            dgvTabella.ColumnHeadersDefaultCellStyle.ForeColor = Color.DarkGray;
            dgvTabella.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dgvTabella.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgvTabella.ColumnHeadersDefaultCellStyle.Padding = new Padding(10, 10, 10, 10);
            dgvTabella.ColumnHeadersHeight = 45;

            dgvTabella.RowTemplate.Height = 50;

            // Handle cell click natively
            dgvTabella.CellClick -= dgvTabella_CellClick; // prevent double firing if already bound in designer
            dgvTabella.CellClick += dgvTabella_CellClick;
        }

        private void IntestaTabella()
        {
            string[] intestazioni = { "NOME", "COGNOME", "DATA INIZIO", "DATA FINE", "ACCONTO", "VERSAMENTO", "SPESE", "TIPO", "COLORE" };
            dgvTabella.ColumnCount = intestazioni.Length;
            for (int i = 0; i < intestazioni.Length; i++)
            {
                dgvTabella.Columns[i].HeaderText = intestazioni[i];
                if (i >= 4 && i <= 6) dgvTabella.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                if (i == 8) dgvTabella.Columns[i].HeaderText = ""; // Empty header for color block
            }
        }

        private void popolaTabella()
        {
            List<ClsPrenotazione> lstPrenotazioni = _gestioneController.getPrenotazione;
            dgvTabella.Rows.Clear();

            foreach (ClsPrenotazione p in lstPrenotazioni)
            {
                if (p.Id != "VDZ100")
                {
                    int rowIndex = dgvTabella.Rows.Add(
                        p.Nome.ToUpper(), p.Cognome.ToUpper(),
                        p.DataInizio.ToShortDateString(), p.DataFine.ToShortDateString(),
                        p.Acconto + " €", p.Versamento + " €", p.SpesePulizia + " €",
                        p.TipoPrenotazione ? "SICURA" : "INSICURA", ""
                    );

                    DataGridViewCell tipoCell = dgvTabella.Rows[rowIndex].Cells[7];
                    tipoCell.Style.BackColor = p.TipoPrenotazione ? Color.FromArgb(46, 125, 50) : Color.FromArgb(198, 140, 0); // Modern Green/Yellow
                    tipoCell.Style.ForeColor = Color.White;
                    tipoCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

                    DataGridViewCell colorCell = dgvTabella.Rows[rowIndex].Cells[8];
                    colorCell.Style.BackColor = p.ColoreCella;
                }
            }
            ordinaPerMeseInizio();
        }

        private void ordinaPerMeseInizio()
        {
            var rows = dgvTabella.Rows.Cast<DataGridViewRow>()
                .Where(row => row.Cells[3].Value != null)
                .Select(row => new
                {
                    RowData = row.Cells.Cast<DataGridViewCell>().Select(c => c.Value).ToArray(),
                    TipoColor = row.Cells[7].Style.BackColor,
                    TipoFore = row.Cells[7].Style.ForeColor,
                    CellColor = row.Cells[8].Style.BackColor
                })
                .OrderBy(r => DateTime.Parse(r.RowData[3].ToString()).Month)
                .ThenBy(r => DateTime.Parse(r.RowData[3].ToString()).Day)
                .ToList();

            dgvTabella.Rows.Clear();
            foreach (var row in rows)
            {
                int rowIndex = dgvTabella.Rows.Add(row.RowData);
                dgvTabella.Rows[rowIndex].Cells[7].Style.BackColor = row.TipoColor;
                dgvTabella.Rows[rowIndex].Cells[7].Style.ForeColor = row.TipoFore;
                dgvTabella.Rows[rowIndex].Cells[7].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgvTabella.Rows[rowIndex].Cells[8].Style.BackColor = row.CellColor;
            }
        }

        public void AggiornaTabella()
        {
            popolaTabella();
        }

        private void dgvTabella_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                if (dgvTabella.Rows[e.RowIndex].Cells[3].Value != null)
                {
                    DateTime dataInizio = Convert.ToDateTime(dgvTabella.Rows[e.RowIndex].Cells[3].Value);
                    FrmMain mainForm = Application.OpenForms.OfType<FrmMain>().FirstOrDefault();
                    if (mainForm != null)
                    {
                        mainForm.vaiA(dataInizio);
                    }
                }
            }
        }
    }
}

