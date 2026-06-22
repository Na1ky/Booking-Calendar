using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Calendario.CONTROLLER;
using Calendario.MODEL;


namespace Calendario.VIEW
{
    public partial class FrmCancellaPrenotazione : Form
    {
        private PrenotazioneController _gestionePrenotazioni;
        
        static readonly Color BG       = Color.FromArgb(10, 13, 28);
        static readonly Color TEXT     = Color.FromArgb(226, 232, 255);
        static readonly Color INPUT_BG = Color.FromArgb(30, 38, 80);

        public FrmCancellaPrenotazione(PrenotazioneController gestionePrenotazioni)
        {
            InitializeComponent();
            _gestionePrenotazioni = gestionePrenotazioni;

            this.BackColor = BG;
            this.ForeColor = TEXT;
            this.Font = new Font("Segoe UI", 10F);
            this.MinimumSize = new Size(620, 360);
            this.Size = new Size(700, 400);
            this.Text = "Cancella Prenotazione";
            this.StartPosition = FormStartPosition.CenterParent;

            this.AutoScroll = true;
            var mainLayout = new TableLayoutPanel {
                Dock = DockStyle.Top, Height = 400, ColumnCount = 1, RowCount = 3,
                BackColor = Color.Transparent, Padding = new Padding(30)
            };
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));  // Title
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); // Card
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 70));  // Buttons
            this.Controls.Add(mainLayout);

            // ─── TITOLO ──────────────────────────────────────────────────────────
            var titleLbl = new Label { Text = "Cancella Prenotazione", Font = new Font("Segoe UI", 16F, FontStyle.Bold), ForeColor = Color.White, Dock = DockStyle.Top, Height = 40, TextAlign = ContentAlignment.MiddleCenter };
            mainLayout.Controls.Add(titleLbl, 0, 0);

            var card = new ModernPanel { Dock = DockStyle.Fill, Margin = new Padding(0, 10, 0, 10), Padding = new Padding(30) };
            mainLayout.Controls.Add(card, 0, 1);

            var lblSel = new Label { Text = "Seleziona la prenotazione da eliminare:", Font = new Font("Segoe UI", 11F), ForeColor = Color.LightGray, Dock = DockStyle.Top, Height = 30 };
            card.Controls.Add(lblSel);

            var cmb = new Guna.UI2.WinForms.Guna2ComboBox {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent, FillColor = INPUT_BG, ForeColor = TEXT,
                BorderRadius = 6, BorderColor = Color.FromArgb(30, 36, 70),
                Font = new Font("Segoe UI", 11F)
            };
            var selContainer = new Panel { Dock = DockStyle.Top, Height = 40 };
            selContainer.Controls.Add(cmb);
            card.Controls.Add(selContainer);
            lblSel.SendToBack();

            // Popola la combobox
            var lst = _gestionePrenotazioni.getPrenotazione;
            lst.Sort((x, y) => (x.Cognome + " " + x.Nome).CompareTo(y.Cognome + " " + y.Nome));
            foreach (var p in lst)
                if (p.Id != "VDZ100")
                    cmb.Items.Add(new KeyValuePair<string, string>(p.Id, p.Cognome + " " + p.Nome));
            cmb.DisplayMember = "Value";
            cmb.ValueMember = "Key";

            // ─── BOTTONI ─────────────────────────────────────────────────────────
            var pnlOps = new ModernPanel { Dock = DockStyle.Fill, Padding = new Padding(10) };
            mainLayout.Controls.Add(pnlOps, 0, 2);

            var opsTlp = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 3, RowCount = 1 };
            opsTlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));
            opsTlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));
            opsTlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 34));
            opsTlp.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            pnlOps.Controls.Add(opsTlp);

            var btnDelAll = new Guna.UI2.WinForms.Guna2Button { Text = "ELIMINA TUTTE", Dock = DockStyle.Fill, Margin = new Padding(4), FillColor = Color.Maroon, BorderRadius = 6, Font = new Font("Segoe UI", 10F, FontStyle.Bold) };
            var btnDel = new Guna.UI2.WinForms.Guna2Button { Text = "ELIMINA", Dock = DockStyle.Fill, Margin = new Padding(4), FillColor = Color.FromArgb(220, 38, 38), BorderRadius = 6, Font = new Font("Segoe UI", 10F, FontStyle.Bold) };
            var btnAnn = new Guna.UI2.WinForms.Guna2Button { Text = "ANNULLA", Dock = DockStyle.Fill, Margin = new Padding(4), FillColor = Color.FromArgb(40, 48, 90), BorderRadius = 6, Font = new Font("Segoe UI", 10F, FontStyle.Bold) };

            opsTlp.Controls.Add(btnDelAll, 0, 0);
            opsTlp.Controls.Add(btnDel, 1, 0);
            opsTlp.Controls.Add(btnAnn, 2, 0);

            // ─── EVENTI ──────────────────────────────────────────────────────────
            btnAnn.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };
            
            btnDel.Click += (s, e) => {
                if (cmb.SelectedItem == null) { MessageBox.Show("Seleziona una prenotazione!", "Attenzione", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
                var sel = (KeyValuePair<string, string>)cmb.SelectedItem;
                if (MessageBox.Show($"Sei sicuro di voler eliminare la prenotazione di {sel.Value}?", "Conferma Eliminazione", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes) {
                    _gestionePrenotazioni.eliminaPrenotazione(sel.Key);
                    DialogResult = DialogResult.OK;
                    Close();
                }
            };

            btnDelAll.Click += (s, e) => {
                if (MessageBox.Show("SEI VERAMENTE SICURO DI VOLER ELIMINARE TUTTE LE PRENOTAZIONI? QUESTA AZIONE E' IRREVERSIBILE.", "ATTENZIONE!", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes) {
                    _gestionePrenotazioni.eliminaTutteLePrenotazioni();
                    DialogResult = DialogResult.OK;
                    Close();
                }
            };
        }

        // Stub handlers per il designer
        private void btnAnnulla_Click(object sender, EventArgs e) => DialogResult = DialogResult.Cancel;
        private void btnCancella_Click(object sender, EventArgs e) { }
        private void BtnEliminaTutte_Click(object sender, EventArgs e) { }
    }
}

