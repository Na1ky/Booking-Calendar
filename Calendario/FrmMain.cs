using Calendario.CONTROLLER;
using Calendario.MODEL;
using Calendario.VIEW;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Threading.Tasks;


namespace Calendario
{
    public partial class FrmMain : Form
    {
        PrenotazioneController gestionePrenotazioni = new PrenotazioneController();
        DateTime currentDay = DateTime.Now;

        // ── Palette "Deep Ocean" ─────────────────────────────────────────────
        // Sfondo generale: blu notte profondo
        static readonly Color C_BG          = Color.FromArgb(10,  13,  28);
        // Sidebar: leggermente più chiaro
        static readonly Color C_SIDEBAR     = Color.FromArgb(14,  17,  35);
        // Card normale
        static readonly Color C_CARD        = Color.FromArgb(20,  24,  48);
        // Card hover
        static readonly Color C_CARD_HOV    = Color.FromArgb(30,  35,  68);
        // Cella vuota (fuori mese)
        static readonly Color C_EMPTY       = Color.FromArgb(12,  15,  30);
        // Accento primario: viola elettrico
        static readonly Color C_ACCENT      = Color.FromArgb(124,  58, 237);
        // Accento hover
        static readonly Color C_ACCENT_HOV  = Color.FromArgb( 91,  33, 182);
        // Accento luminoso (oggi)
        static readonly Color C_TODAY       = Color.FromArgb(139,  92, 246);
        // Weekend
        static readonly Color C_WEEKEND     = Color.FromArgb(251, 113, 133);
        // Testo principale
        static readonly Color C_TEXT        = Color.FromArgb(226, 232, 255);
        // Testo secondario
        static readonly Color C_MUTED       = Color.FromArgb( 99, 107, 150);
        // Bordi sottili
        static readonly Color C_BORDER      = Color.FromArgb( 30,  36,  70);
        // Gradiente sidebar top
        static readonly Color C_GRAD_TOP    = Color.FromArgb( 18,  20,  44);

        TableLayoutPanel rootLayout;
        Panel            pnlActionContainer;
        TableLayoutPanel calendarGrid;
        Label            lblMeseCorrente;
        Label            lblNavMeseText;
        Label            lblNavAnnoText;
        bool             isUpdatingNav = false;
        bool             isWeekView = false;
        DateTime         currentWeekViewDate = DateTime.Today;
        static readonly string[] MESI = { "Gennaio", "Febbraio", "Marzo", "Aprile", "Maggio", "Giugno", "Luglio", "Agosto", "Settembre", "Ottobre", "Novembre", "Dicembre" };

        public FrmMain()
        {
            InitializeComponent();
            ThemeHelper.ApplyTheme(this);
            
            // Ripristina stili per la finestra principale (ThemeHelper li fissa per i popup)
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MaximizeBox = true;
            this.MinimizeBox = true;
            this.StartPosition = FormStartPosition.CenterScreen;

            // Carica icona per la finestra
            string icoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "IMG", "Icona.ico");
            if (File.Exists(icoPath))
                this.Icon = new Icon(icoPath);

            this.Load += FrmMain_Load;
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            this.Controls.Clear();
            BuildUI();
            PopulateCalendar(currentDay.Year, currentDay.Month);

            // Controlla aggiornamenti in background
            _ = ControllaAggiornamenti();
        }

        private async Task ControllaAggiornamenti()
        {
            try
            {
                // INSERISCI QUI I TUOI DATI GITHUB
                string githubUser = "Na1ky"; 
                string githubRepo = "Booking-Calendar"; 
                Version versioneAttuale = VersioneApplicazione;

                string apiUrl = $"https://api.github.com/repos/{githubUser}/{githubRepo}/releases/latest";

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("User-Agent", "Calendario-App-Updater");

                    string jsonResponse = await client.GetStringAsync(apiUrl);
                    JObject release = JObject.Parse(jsonResponse);

                    string tagUltimaVersione = release["tag_name"]?.ToString();
                    Version ultimaVersione = EstraiVersione(tagUltimaVersione);

                    if (ultimaVersione != null && ultimaVersione.CompareTo(versioneAttuale) > 0)
                    {
                        DialogResult ris = MessageBox.Show(
                            $"È disponibile una nuova versione del Calendario ({FormattaVersione(ultimaVersione)})!\n" +
                            $"Attualmente stai usando la versione {FormattaVersione(versioneAttuale)}.\n\n" +
                            $"Vuoi aprire la pagina per scaricare l'aggiornamento?",
                            "Aggiornamento Disponibile",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Information);

                        if (ris == DialogResult.Yes)
                        {
                            Process.Start(new ProcessStartInfo
                            {
                                FileName = $"https://github.com/{githubUser}/{githubRepo}/releases/latest",
                                UseShellExecute = true
                            });
                        }
                    }
                }
            }
            catch
            {
                // Fallimento silenzioso se non c'è internet
            }
        }

        private static Version VersioneApplicazione
        {
            get
            {
                Version version = new Version(Application.ProductVersion);
                return NormalizzaVersione(version);
            }
        }

        private static Version EstraiVersione(string valore)
        {
            if (string.IsNullOrWhiteSpace(valore))
                return null;

            string versionePulita = valore.Trim().TrimStart('v', 'V');
            versionePulita = versionePulita.Split('-', '+')[0];

            if (Version.TryParse(versionePulita, out Version version))
                return NormalizzaVersione(version);

            return null;
        }

        private static Version NormalizzaVersione(Version version)
        {
            return new Version(
                version.Major,
                version.Minor,
                Math.Max(version.Build, 0),
                Math.Max(version.Revision, 0));
        }

        private static string FormattaVersione(Version version)
        {
            return $"v{version.Major}.{version.Minor}";
        }

        // ─── LAYOUT PRINCIPALE ───────────────────────────────────────────────
        private Control activeSidebarBtn = null;

        private void BuildUI()
        {
            this.MinimumSize = new Size(1000, 650);
            this.BackColor   = C_BG;

            rootLayout = new TableLayoutPanel
            {
                Dock        = DockStyle.Fill,
                ColumnCount = 2,
                RowCount    = 2,
                BackColor   = C_BG,
                Padding     = new Padding(0),
                Margin      = new Padding(0)
            };
            rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 268));
            rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 0)); // pannello azioni in basso, inizialmente nascosto

            var sidebar = BuildSidebar();
            rootLayout.Controls.Add(sidebar, 0, 0);
            rootLayout.SetRowSpan(sidebar, 2); // la sidebar copre entrambe le righe

            rootLayout.Controls.Add(BuildCalendarArea(), 1, 0);

            // Container per i form nel pannello in basso
            pnlActionContainer = new Panel
            {
                Dock       = DockStyle.Fill,
                BackColor  = C_BG,
                Padding    = new Padding(0),
                AutoScroll = true
            };
            var sep = new Panel { Height = 1, Dock = DockStyle.Top, BackColor = C_BORDER };
            pnlActionContainer.Controls.Add(sep);

            rootLayout.Controls.Add(pnlActionContainer, 1, 1);
            this.Controls.Add(rootLayout);
        }

        private void ShowActionPanel(Control sender, Form actionForm, string successMsg = null)
        {
            Control clickedBtn = sender;

            // Toggle: se clicco lo stesso bottone che era già attivo, chiudo il pannello
            if (activeSidebarBtn == clickedBtn && clickedBtn != null)
            {
                ChiudiActionPanel();
                return;
            }

            // De-evidenzio il bottone precedente
            if (activeSidebarBtn is Guna.UI2.WinForms.Guna2Button oldBtn)
                oldBtn.FillColor = C_CARD;

            // Evidenzio il nuovo bottone attivo
            activeSidebarBtn = clickedBtn;
            if (activeSidebarBtn is Guna.UI2.WinForms.Guna2Button newBtn)
                newBtn.FillColor = C_ACCENT;

            // Pulisce il pannello da eventuali form precedenti
            foreach (Control c in pnlActionContainer.Controls.OfType<Form>().ToList())
                c.Dispose();
            pnlActionContainer.Controls.Clear();
            pnlActionContainer.Controls.Add(new Panel { Height = 1, Dock = DockStyle.Top, BackColor = C_BORDER });

            // L'altezza del pannello inferiore si adatta al form (massimo 520px)
            int h = Math.Min(actionForm.Height, 520);
            rootLayout.RowStyles[1].Height = h;
            rootLayout.PerformLayout();
            
            isWeekView = true;
            currentWeekViewDate = DateTime.Today;
            PopulateCalendar(currentDay.Year, currentDay.Month);
            
            this.Refresh();

            actionForm.TopLevel         = false;
            actionForm.FormBorderStyle  = FormBorderStyle.None;
            actionForm.Dock             = DockStyle.Fill;

            actionForm.FormClosed += (s, ev) =>
            {
                ChiudiActionPanel();
                if (actionForm.DialogResult == DialogResult.OK)
                {
                    PopulateCalendar(currentDay.Year, currentDay.Month);
                    AggiornaTabellSeAperta();
                    if (!string.IsNullOrEmpty(successMsg))
                        MessageBox.Show(successMsg, "Successo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            };

            pnlActionContainer.Controls.Add(actionForm);
            actionForm.Show();
        }

        private void ChiudiActionPanel()
        {
            if (rootLayout.RowStyles[1].Height == 0) return;
            rootLayout.RowStyles[1].Height = 0;
            rootLayout.PerformLayout();
            
            isWeekView = false;
            PopulateCalendar(currentDay.Year, currentDay.Month);
            
            this.Refresh();
            if (activeSidebarBtn is Guna.UI2.WinForms.Guna2Button oldBtn && oldBtn.Tag?.ToString() != "accent")
                oldBtn.FillColor = C_CARD;
            else if (activeSidebarBtn != null)
                activeSidebarBtn.BackColor = C_CARD;
            activeSidebarBtn = null;
            foreach (Control c in pnlActionContainer.Controls.OfType<Form>().ToList())
                c.Dispose();
        }

        // ─── SIDEBAR ─────────────────────────────────────────────────────────
        private Panel BuildSidebar()
        {
            // Pannello con gradiente dipinto via OnPaint
            var sidebar = new GradientPanel(C_SIDEBAR, C_GRAD_TOP)
            {
                Dock    = DockStyle.Fill,
                Padding = new Padding(0)
            };

            // Linea separatrice
            sidebar.Controls.Add(new Panel
            {
                Width     = 1,
                Dock      = DockStyle.Right,
                BackColor = C_BORDER
            });

            var flow = new FlowLayoutPanel
            {
                Dock          = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents  = false,
                Padding       = new Padding(20, 22, 20, 20),
                BackColor     = Color.Transparent
            };
            sidebar.Controls.Add(flow);

            // ── Brand ────────────────────────────────────────────────────────
            var brand = new Panel
            {
                Width     = 228,
                Height    = 72,
                BackColor = Color.Transparent,
                Margin    = new Padding(0, 0, 0, 24)
            };

            var picIcon = new PictureBox
            {
                Size = new Size(46, 46),
                Location = new Point(0, 13),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent
            };
            string icoPathSide = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "IMG", "Icona.ico");
            if (File.Exists(icoPathSide))
                picIcon.Image = new Icon(icoPathSide, new Size(64, 64)).ToBitmap();
            
            brand.Controls.Add(picIcon);
            brand.Controls.Add(new Label
            {
                Text      = "Calendario",
                Font      = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = C_TEXT,
                BackColor = Color.Transparent,
                AutoSize  = false,
                Size      = new Size(170, 26),
                Location  = new Point(54, 11)
            });
            brand.Controls.Add(new Label
            {
                Text      = "Gestione prenotazioni",
                Font      = new Font("Segoe UI", 8),
                ForeColor = C_MUTED,
                BackColor = Color.Transparent,
                AutoSize  = false,
                Size      = new Size(170, 18),
                Location  = new Point(54, 37)
            });
            flow.Controls.Add(brand);

            // Divisore orizzontale
            flow.Controls.Add(HDivider());

            // ── Vai A (Salto rapido) ─────────────────────────────────────────
            flow.Controls.Add(SectionLabel("VAI A DATA"));
            var pnlVaiA = new Panel { Width = 228, Height = 40, BackColor = Color.Transparent, Margin = new Padding(0, 0, 0, 16) };
            
            // Finto ComboBox Mese
            var pnlMese = new Panel { Width = 125, Height = 36, Location = new Point(0, 0), BackColor = C_CARD, Cursor = Cursors.Hand };
            RoundRegion(pnlMese, 8);
            lblNavMeseText = new Label { Text = MESI[currentDay.Month - 1], ForeColor = C_TEXT, Font = new Font("Segoe UI", 9.5f), Location = new Point(10, 8), AutoSize = true, BackColor = Color.Transparent, Cursor = Cursors.Hand };
            var lblMeseArrow = new Label { Text = "▼", ForeColor = C_MUTED, Font = new Font("Segoe UI", 8f), Location = new Point(100, 11), AutoSize = true, BackColor = Color.Transparent, Cursor = Cursors.Hand };
            pnlMese.Controls.Add(lblNavMeseText);
            pnlMese.Controls.Add(lblMeseArrow);

            // Finto ComboBox Anno
            var pnlAnno = new Panel { Width = 95, Height = 36, Location = new Point(133, 0), BackColor = C_CARD, Cursor = Cursors.Hand };
            RoundRegion(pnlAnno, 8);
            lblNavAnnoText = new Label { Text = currentDay.Year.ToString(), ForeColor = C_TEXT, Font = new Font("Segoe UI", 9.5f), Location = new Point(10, 8), AutoSize = true, BackColor = Color.Transparent, Cursor = Cursors.Hand };
            var lblAnnoArrow = new Label { Text = "▼", ForeColor = C_MUTED, Font = new Font("Segoe UI", 8f), Location = new Point(70, 11), AutoSize = true, BackColor = Color.Transparent, Cursor = Cursors.Hand };
            pnlAnno.Controls.Add(lblNavAnnoText);
            pnlAnno.Controls.Add(lblAnnoArrow);

            // Menu a tendina custom (Mese)
            var menuMese = new ContextMenuStrip { Renderer = new DarkMenuRenderer(), ShowImageMargin = false };
            for(int i=0; i<MESI.Length; i++) {
                int mIndex = i;
                menuMese.Items.Add(new ToolStripMenuItem(MESI[i], null, (s, e) => {
                    if (isUpdatingNav) return;
                    lblNavMeseText.Text = MESI[mIndex];
                    vaiA(new DateTime(int.Parse(lblNavAnnoText.Text), mIndex + 1, 1));
                }) { ForeColor = C_TEXT, Font = new Font("Segoe UI", 9.5f) });
            }

            // Menu a tendina custom (Anno)
            var menuAnno = new ContextMenuStrip { Renderer = new DarkMenuRenderer(), ShowImageMargin = false };
            for(int i = DateTime.Now.Year - 10; i <= DateTime.Now.Year + 10; i++) {
                int y = i;
                menuAnno.Items.Add(new ToolStripMenuItem(y.ToString(), null, (s, e) => {
                    if (isUpdatingNav) return;
                    lblNavAnnoText.Text = y.ToString();
                    int m = Array.IndexOf(MESI, lblNavMeseText.Text) + 1;
                    vaiA(new DateTime(y, m, 1));
                }) { ForeColor = C_TEXT, Font = new Font("Segoe UI", 9.5f) });
            }

            // Eventi click
            EventHandler openMese = (s, e) => menuMese.Show(pnlMese, new Point(0, pnlMese.Height + 2));
            pnlMese.Click += openMese; lblNavMeseText.Click += openMese; lblMeseArrow.Click += openMese;

            EventHandler openAnno = (s, e) => menuAnno.Show(pnlAnno, new Point(0, pnlAnno.Height + 2));
            pnlAnno.Click += openAnno; lblNavAnnoText.Click += openAnno; lblAnnoArrow.Click += openAnno;

            // Hover effects
            Action<bool> hovM = on => pnlMese.BackColor = on ? C_CARD_HOV : C_CARD;
            pnlMese.MouseEnter += (s, e) => hovM(true); pnlMese.MouseLeave += (s, e) => hovM(false);
            lblNavMeseText.MouseEnter += (s, e) => hovM(true); lblNavMeseText.MouseLeave += (s, e) => hovM(false);
            lblMeseArrow.MouseEnter += (s, e) => hovM(true); lblMeseArrow.MouseLeave += (s, e) => hovM(false);

            Action<bool> hovA = on => pnlAnno.BackColor = on ? C_CARD_HOV : C_CARD;
            pnlAnno.MouseEnter += (s, e) => hovA(true); pnlAnno.MouseLeave += (s, e) => hovA(false);
            lblNavAnnoText.MouseEnter += (s, e) => hovA(true); lblNavAnnoText.MouseLeave += (s, e) => hovA(false);
            lblAnnoArrow.MouseEnter += (s, e) => hovA(true); lblAnnoArrow.MouseLeave += (s, e) => hovA(false);

            pnlVaiA.Controls.Add(pnlMese);
            pnlVaiA.Controls.Add(pnlAnno);
            flow.Controls.Add(pnlVaiA);

            // ── Bottoni ──────────────────────────────────────────────────────
            flow.Controls.Add(SectionLabel("AZIONI"));
            flow.Controls.Add(SideBtn("➕  Aggiungi Prenotazione", btnAddPrenotation_Click));
            flow.Controls.Add(SideBtn("✏️  Modifica Prenotazione",  btnModify_Click));
            flow.Controls.Add(SideBtn("🗑️  Cancella Prenotazione",  btnEliminaPrenotazione_Click));

            flow.Controls.Add(SectionLabel("REPORT"));
            flow.Controls.Add(SideBtn("📊  Tabella Recapito",       btnTabella_Click));
            flow.Controls.Add(SideBtn("💾  Salva File Word",         btnSalvaFile_Click));
            flow.Controls.Add(SideBtn("💰  Calcola Prezzo",          BtnPrezzo_Click));

            return sidebar;
        }

        private Control SideBtn(string text, EventHandler onClick, bool accent = false)
        {
            var btn = new Guna.UI2.WinForms.Guna2Button
            {
                Text = text,
                Width = 228,
                Height = 46,
                BorderRadius = 11,
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 0, 0, 7),
                Font = new Font("Segoe UI", 9.5f, accent ? FontStyle.Bold : FontStyle.Regular),
                TextAlign = HorizontalAlignment.Left,
                TextOffset = new Point(15, 0),
                Animated = true,
                Tag = accent ? "accent" : "normal"
            };

            if (accent)
            {
                btn.FillColor = C_ACCENT;
                btn.ForeColor = Color.White;
                btn.HoverState.FillColor = C_ACCENT_HOV;
            }
            else
            {
                btn.FillColor = C_CARD;
                btn.ForeColor = C_TEXT;
                btn.HoverState.FillColor = C_CARD_HOV;
                btn.CustomBorderThickness = new Padding(3, 0, 0, 0);
                btn.CustomBorderColor = Color.FromArgb(196, 181, 253);
            }

            btn.Click += onClick;
            return btn;
        }

        private Panel HDivider() => new Panel
        {
            Width     = 228,
            Height    = 1,
            BackColor = C_BORDER,
            Margin    = new Padding(0, 4, 0, 16)
        };

        private Label SectionLabel(string t) => new Label
        {
            Text      = t,
            Font      = new Font("Segoe UI", 7.5f, FontStyle.Bold),
            ForeColor = C_MUTED,
            BackColor = Color.Transparent,
            AutoSize  = false,
            Width     = 228,
            Height    = 22,
            Margin    = new Padding(0, 10, 0, 6),
            TextAlign = ContentAlignment.MiddleLeft
        };



        // ─── AREA CALENDARIO ─────────────────────────────────────────────────
        private Panel BuildCalendarArea()
        {
            var area = new Panel
            {
                Dock      = DockStyle.Fill,
                BackColor = C_BG,
                Padding   = new Padding(28, 18, 28, 18)
            };

            var topBar = new Panel { Dock = DockStyle.Top, Height = 36, BackColor = Color.Transparent };
            var pnlProfile = new FlowLayoutPanel {
                Dock = DockStyle.Right,
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                BackColor = Color.Transparent,
                Padding = new Padding(0),
                Margin = new Padding(0)
            };

            string uName = CONTROLLER.AuthController.CurrentUser?.Username ?? "Ospite";
            var lblIcon = new Label { Text = "👤", Font = new Font("Segoe UI", 14f), ForeColor = C_ACCENT, AutoSize = true, Margin = new Padding(0, 8, 4, 0) };
            var lblUser = new Label { Text = uName, ForeColor = C_TEXT, Font = new Font("Segoe UI", 10.5f, FontStyle.Bold), AutoSize = true, Margin = new Padding(0, 10, 8, 0) };
            var btnLogout = new Label { Text = "Esci", ForeColor = C_WEEKEND, Font = new Font("Segoe UI", 9.5f, FontStyle.Underline), AutoSize = true, Cursor = Cursors.Hand, Margin = new Padding(0, 11, 0, 0) };
            btnLogout.Click += (s, e) => {
                new CONTROLLER.AuthController().Logout();
                this.Hide();
                var login = new FrmLogin();
                login.FormClosed += (s2, args) => this.Close();
                login.Show();
            };

            pnlProfile.Controls.Add(lblIcon);
            pnlProfile.Controls.Add(lblUser);
            pnlProfile.Controls.Add(btnLogout);
            topBar.Controls.Add(pnlProfile);

            // Header mese
            var monthHdr = new Panel { Dock = DockStyle.Top, Height = 62, BackColor = Color.Transparent };

            var btnPrev = NavBtn("‹");
            btnPrev.Dock   = DockStyle.Left;
            btnPrev.Click += BtnPrev_Click;

            var btnNext = NavBtn("›");
            btnNext.Dock   = DockStyle.Right;
            btnNext.Click += BtnNext_Click;

            lblMeseCorrente = new Label
            {
                Dock      = DockStyle.Fill,
                Text      = "",
                Font      = new Font("Segoe UI", 21, FontStyle.Bold),
                ForeColor = C_TEXT,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleCenter
            };

            monthHdr.Controls.Add(lblMeseCorrente);
            monthHdr.Controls.Add(btnPrev);
            monthHdr.Controls.Add(btnNext);

            // Barra nomi giorni
            var dayBar = new TableLayoutPanel
            {
                Dock        = DockStyle.Top,
                Height      = 38,
                ColumnCount = 7,
                RowCount    = 1,
                BackColor   = Color.Transparent,
                Margin      = new Padding(0, 6, 0, 2),
                Padding     = new Padding(0)
            };
            for (int i = 0; i < 7; i++)
                dayBar.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 14.28f));
            dayBar.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            string[] days = { "LUN", "MAR", "MER", "GIO", "VEN", "SAB", "DOM" };
            for (int i = 0; i < 7; i++)
            {
                dayBar.Controls.Add(new Label
                {
                    Text      = days[i],
                    Font      = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                    ForeColor = i >= 5 ? C_WEEKEND : C_MUTED,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Dock      = DockStyle.Fill,
                    BackColor = Color.Transparent
                }, i, 0);
            }

            // Griglia 6×7
            calendarGrid = new TableLayoutPanel
            {
                Dock            = DockStyle.Fill,
                ColumnCount     = 7,
                RowCount        = 6,
                BackColor       = Color.Transparent,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                Margin          = new Padding(0)
            };
            for (int i = 0; i < 7; i++)
                calendarGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 14.28f));
            for (int i = 0; i < 6; i++)
                calendarGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 16.66f));

            // ORDINE IMPORTA: Fill prima, poi i Top
            area.Controls.Add(calendarGrid);
            area.Controls.Add(dayBar);
            area.Controls.Add(monthHdr);
            area.Controls.Add(topBar);

            return area;
        }

        private Control NavBtn(string sym)
        {
            var btn = new Guna.UI2.WinForms.Guna2Button
            {
                Text = sym,
                Width = 46,
                BorderRadius = 11,
                Cursor = Cursors.Hand,
                FillColor = C_CARD,
                ForeColor = C_TEXT,
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                Animated = true
            };
            btn.HoverState.FillColor = C_ACCENT;
            btn.HoverState.ForeColor = Color.White;
            return btn;
        }

        // ─── POPULATE ────────────────────────────────────────────────────────
        private void PopulateCalendar(int year, int month)
        {
            foreach (Control c in calendarGrid.Controls.Cast<Control>().ToList())
            { calendarGrid.Controls.Remove(c); c.Dispose(); }

            // Sincronizza i menu a tendina laterali
            isUpdatingNav = true;
            if (lblNavMeseText != null) lblNavMeseText.Text = MESI[month - 1];
            if (lblNavAnnoText != null) lblNavAnnoText.Text = year.ToString();
            isUpdatingNav = false;

            var datesToRender = new List<DateTime?>();
            if (isWeekView) {
                int diff = (7 + (currentWeekViewDate.DayOfWeek - DayOfWeek.Monday)) % 7;
                DateTime weekStart = currentWeekViewDate.AddDays(-1 * diff).Date;
                lblMeseCorrente.Text = $"{weekStart.ToString("dd MMM")} - {weekStart.AddDays(6).ToString("dd MMM yyyy")}".ToUpper();
                for (int i = 0; i < 7; i++) datesToRender.Add(weekStart.AddDays(i));
            } else {
                var firstDay = new DateTime(year, month, 1);
                lblMeseCorrente.Text = firstDay.ToString("MMMM yyyy", new CultureInfo("it-IT")).ToUpper();
                int daysInMonth = DateTime.DaysInMonth(year, month);
                int startCol = (int)firstDay.DayOfWeek;
                startCol = startCol == 0 ? 6 : startCol - 1;
                
                int cDay = 1;
                for (int i = 0; i < 42; i++) {
                    if (i < startCol || cDay > daysInMonth) {
                        datesToRender.Add(null);
                    } else {
                        datesToRender.Add(new DateTime(year, month, cDay++));
                    }
                }
            }

            int rows = isWeekView ? 1 : 6;
            calendarGrid.RowStyles.Clear();
            calendarGrid.RowCount = rows;
            float rowHeight = isWeekView ? 100f : 16.66f;
            for (int i = 0; i < rows; i++)
                calendarGrid.RowStyles.Add(new RowStyle(SizeType.Percent, rowHeight));

            var pren = gestionePrenotazioni.getPrenotazione;
            var today = DateTime.Today;

            int index = 0;
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < 7; col++)
                {
                    DateTime? dateOrNull = datesToRender[index++];
                    bool isEmpty = !dateOrNull.HasValue;
                    bool isWeekend = col >= 5;

                    var cell = new Guna.UI2.WinForms.Guna2Panel
                    {
                        Dock = DockStyle.Fill,
                        FillColor = isEmpty ? C_EMPTY : C_CARD,
                        Margin = new Padding(3),
                        BorderRadius = 9,
                        Cursor = Cursors.Hand
                    };

                    if (!isEmpty)
                    {
                        DateTime currentDate = dateOrNull.Value;
                        bool isToday = currentDate.Date == today.Date;
                        int day = currentDate.Day;

                        var bookings = pren.Where(p =>
                            p.DataInizio.Date <= currentDate &&
                            p.DataFine.Date   >= currentDate).OrderBy(p => p.DataInizio).ToList();

                        // ── Numero giorno ─────────────────────────────────────
                        var lblNum = new Label
                        {
                            Text      = day.ToString(),
                            Font      = new Font("Segoe UI", isToday ? 10f : 9.5f,
                                                 isToday ? FontStyle.Bold : FontStyle.Regular),
                            ForeColor = isToday   ? Color.White
                                      : isWeekend ? C_WEEKEND
                                      : C_TEXT,
                            BackColor = isToday ? C_TODAY : Color.Transparent,
                            AutoSize  = false,
                            Size      = new Size(30, 30),
                            Location  = new Point(9, 7),
                            TextAlign = ContentAlignment.MiddleCenter,
                            Cursor    = Cursors.Hand
                        };
                        if (isToday) RoundRegion_Label(lblNum, 8);
                        cell.Controls.Add(lblNum);

                        // ── Badge prenotazione ────────────────────────────────
                        if (bookings.Count > 0)
                        {
                            cell.Tag = string.Join(",", bookings.Select(b => b.Id));

                            if (bookings.Count == 1)
                            {
                                var booking = bookings[0];
                                cell.FillColor = Color.FromArgb(70, booking.ColoreCella.R, booking.ColoreCella.G, booking.ColoreCella.B);
                                var sideBar = new Panel { Width = 6, Dock = DockStyle.Left, BackColor = booking.ColoreCella, Cursor = Cursors.Hand };
                                cell.Controls.Add(sideBar);

                                var lblPrenotato = new Label { Text = "PRENOTATO", Font = new Font("Segoe UI", 8.5f, FontStyle.Bold), ForeColor = booking.ColoreCella, Location = new Point(14, 38), AutoSize = true, BackColor = Color.Transparent, Cursor = Cursors.Hand };
                                cell.Controls.Add(lblPrenotato);

                                string displayName = (booking.Nome + " " + booking.Cognome).Trim();
                                if (string.IsNullOrEmpty(displayName)) displayName = "Sconosciuto";
                                var lblNome = new Label { Text = displayName.ToUpper(), Font = new Font("Segoe UI", 9.5f, FontStyle.Regular), ForeColor = C_TEXT, Location = new Point(14, 56), AutoSize = false, Size = new Size(150, 40), Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right, BackColor = Color.Transparent, Cursor = Cursors.Hand };
                                cell.Controls.Add(lblNome);

                                Color baseColor = cell.FillColor;
                                Color hoverColor = Color.FromArgb(100, booking.ColoreCella.R, booking.ColoreCella.G, booking.ColoreCella.B);
                                Action<bool> hovBooking = on => cell.FillColor = on ? hoverColor : baseColor;
                                cell.MouseEnter += (s, ev) => hovBooking(true); cell.MouseLeave += (s, ev) => hovBooking(false);
                                sideBar.MouseEnter += (s, ev) => hovBooking(true); sideBar.MouseLeave += (s, ev) => hovBooking(false);
                                lblPrenotato.MouseEnter += (s, ev) => hovBooking(true); lblPrenotato.MouseLeave += (s, ev) => hovBooking(false);
                                lblNome.MouseEnter += (s, ev) => hovBooking(true); lblNome.MouseLeave += (s, ev) => hovBooking(false);

                                sideBar.Click += (s, ev) => CellClick(cell);
                                lblPrenotato.Click += (s, ev) => CellClick(cell);
                                lblNome.Click += (s, ev) => CellClick(cell);
                                
                                if (isWeekView) {
                                    var lblDetails = new Label {
                                        Text = $"Inizio: {booking.DataInizio:dd/MM/yyyy HH:mm}\nFine: {booking.DataFine:dd/MM/yyyy HH:mm}\nAcconto: {booking.Acconto}€\nVersamento: {booking.Versamento}€\nSpese: {booking.SpesePulizia}€",
                                        Font = new Font("Segoe UI", 9f),
                                        ForeColor = Color.FromArgb(160, 170, 210),
                                        Location = new Point(14, 100),
                                        AutoSize = true,
                                        BackColor = Color.Transparent,
                                        Cursor = Cursors.Hand
                                    };
                                    cell.Controls.Add(lblDetails);
                                    lblDetails.MouseEnter += (s, ev) => hovBooking(true); 
                                    lblDetails.MouseLeave += (s, ev) => hovBooking(false);
                                    lblDetails.Click += (s, ev) => CellClick(cell);
                                }
                            }
                            else
                            {
                                // Turnover: Due prenotazioni
                                var b1 = bookings[0]; // Quella che finisce oggi
                                var b2 = bookings[1]; // Quella che inizia oggi
                                
                                cell.FillColor = Color.FromArgb(50, 100, 100, 100); // Sfondo grigio neutro per il turnover

                                var sideBar1 = new Panel { Width = 6, Dock = DockStyle.Left, BackColor = b1.ColoreCella, Cursor = Cursors.Hand };
                                var sideBar2 = new Panel { Width = 6, Dock = DockStyle.Right, BackColor = b2.ColoreCella, Cursor = Cursors.Hand };
                                cell.Controls.Add(sideBar1);
                                cell.Controls.Add(sideBar2);

                                string n1 = (b1.Nome + " " + b1.Cognome).Trim();
                                string n2 = (b2.Nome + " " + b2.Cognome).Trim();

                                var lblOut = new Label { Text = "OUT: " + n1, Font = new Font("Segoe UI", 7.5f, FontStyle.Bold), ForeColor = b1.ColoreCella, Location = new Point(14, 38), AutoSize = false, Width = 110, Height = 15, BackColor = Color.Transparent, Cursor = Cursors.Hand };
                                var lblIn = new Label { Text = "IN: " + n2, Font = new Font("Segoe UI", 7.5f, FontStyle.Bold), ForeColor = b2.ColoreCella, Location = new Point(14, 56), AutoSize = false, Width = 110, Height = 15, BackColor = Color.Transparent, Cursor = Cursors.Hand };
                                cell.Controls.Add(lblOut);
                                cell.Controls.Add(lblIn);

                                Color baseColor = cell.FillColor;
                                Color hoverColor = Color.FromArgb(70, 120, 120, 120);
                                Action<bool> hovBooking = on => cell.FillColor = on ? hoverColor : baseColor;
                                cell.MouseEnter += (s, ev) => hovBooking(true); cell.MouseLeave += (s, ev) => hovBooking(false);
                                sideBar1.MouseEnter += (s, ev) => hovBooking(true); sideBar1.MouseLeave += (s, ev) => hovBooking(false);
                                sideBar2.MouseEnter += (s, ev) => hovBooking(true); sideBar2.MouseLeave += (s, ev) => hovBooking(false);
                                lblOut.MouseEnter += (s, ev) => hovBooking(true); lblOut.MouseLeave += (s, ev) => hovBooking(false);
                                lblIn.MouseEnter += (s, ev) => hovBooking(true); lblIn.MouseLeave += (s, ev) => hovBooking(false);

                                sideBar1.Click += (s, ev) => CellClick(cell);
                                sideBar2.Click += (s, ev) => CellClick(cell);
                                lblOut.Click += (s, ev) => CellClick(cell);
                                lblIn.Click += (s, ev) => CellClick(cell);
                                
                                if (isWeekView) {
                                    lblOut.Location = new Point(14, 38);
                                    lblIn.Location = new Point(14, 115);
                                    
                                    var lblDetOut = new Label {
                                        Text = $"Fine: {b1.DataFine:dd/MM/yyyy HH:mm}\nAcconto: {b1.Acconto}€ | Vers: {b1.Versamento}€",
                                        Font = new Font("Segoe UI", 8.5f), ForeColor = Color.FromArgb(160, 170, 210),
                                        Location = new Point(14, 55), AutoSize = true, BackColor = Color.Transparent, Cursor = Cursors.Hand
                                    };
                                    var lblDetIn = new Label {
                                        Text = $"Inizio: {b2.DataInizio:dd/MM/yyyy HH:mm}\nAcconto: {b2.Acconto}€ | Vers: {b2.Versamento}€",
                                        Font = new Font("Segoe UI", 8.5f), ForeColor = Color.FromArgb(160, 170, 210),
                                        Location = new Point(14, 132), AutoSize = true, BackColor = Color.Transparent, Cursor = Cursors.Hand
                                    };
                                    cell.Controls.Add(lblDetOut);
                                    cell.Controls.Add(lblDetIn);
                                    
                                    lblDetOut.MouseEnter += (s, ev) => hovBooking(true); lblDetOut.MouseLeave += (s, ev) => hovBooking(false);
                                    lblDetIn.MouseEnter += (s, ev) => hovBooking(true); lblDetIn.MouseLeave += (s, ev) => hovBooking(false);
                                    lblDetOut.Click += (s, ev) => CellClick(cell);
                                    lblDetIn.Click += (s, ev) => CellClick(cell);
                                }
                            }
                        }
                        else
                        {
                            // ── Hover standard per celle vuote ─────────────────────
                            Color normalBg = C_CARD;
                            Action<bool> hov = on => cell.FillColor = on ? C_CARD_HOV : normalBg;
                            cell.MouseEnter  += (s, ev) => hov(true);
                            cell.MouseLeave  += (s, ev) => hov(false);
                            lblNum.MouseEnter += (s, ev) => hov(true);
                            lblNum.MouseLeave += (s, ev) => hov(false);
                        }

                        cell.Click       += (s, ev) => CellClick(cell);
                        lblNum.Click     += (s, ev) => CellClick(cell);
                    }

                    calendarGrid.Controls.Add(cell, col, row);
                }
            }
        }

        private void CellClick(Panel cell)
        {
            if (cell.Tag is string id && !string.IsNullOrEmpty(id))
            {
                gestionePrenotazioni.stampaInformazioniPrenotazione(id);
            }
        }

        // ─── NAVIGAZIONE ─────────────────────────────────────────────────────
        private void BtnNext_Click(object s, EventArgs e)
        { 
            if (isWeekView) {
                currentWeekViewDate = currentWeekViewDate.AddDays(7);
            } else {
                currentDay = currentDay.AddMonths(1);
            }
            PopulateCalendar(currentDay.Year, currentDay.Month); 
        }
        private void BtnPrev_Click(object s, EventArgs e)
        { 
            if (isWeekView) {
                currentWeekViewDate = currentWeekViewDate.AddDays(-7);
            } else {
                currentDay = currentDay.AddMonths(-1);
            }
            PopulateCalendar(currentDay.Year, currentDay.Month); 
        }

        public void vaiA(DateTime d)
        { currentDay = d; PopulateCalendar(d.Year, d.Month); Activate(); BringToFront(); }

        // ─── AZIONI ──────────────────────────────────────────────────────────
        private void btnAddPrenotation_Click(object s, EventArgs e)
        {
            try
            {
                ShowActionPanel(s as Control, new FrmAggiungiPrenotazione(gestionePrenotazioni), "Prenotazione aggiunta!");
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void btnModify_Click(object s, EventArgs e)
        {
            if (!CheckHasBookings()) return;
            try
            {
                ShowActionPanel(s as Control, new FrmModificaPrenotazione(gestionePrenotazioni), "Prenotazione modificata!");
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void btnEliminaPrenotazione_Click(object s, EventArgs e)
        {
            if (!CheckHasBookings()) return;
            try
            {
                ShowActionPanel(s as Control, new FrmCancellaPrenotazione(gestionePrenotazioni), "Prenotazione cancellata!");
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void btnTabella_Click(object s, EventArgs e)
        {
            if (!CheckHasBookings()) return;
            ShowActionPanel(s as Control, new FrmTabellaRecapito(gestionePrenotazioni));
        }

        private void btnSalvaFile_Click(object s, EventArgs e)
        {
            if (!CheckHasBookings()) return;
            ShowActionPanel(s as Control, new FrmSalvaIlFile(gestionePrenotazioni));
        }

        private void BtnPrezzo_Click(object s, EventArgs e)
        {
            ShowActionPanel(s as Control, new FrmGestionePrezzo(gestionePrenotazioni));
        }

        private bool CheckHasBookings()
        {
            if (gestionePrenotazioni.getPrenotazione.Count < 1)
            { MessageBox.Show("Nessuna prenotazione presente.", "Attenzione", MessageBoxButtons.OK, MessageBoxIcon.Warning); return false; }
            return true;
        }

        private void AggiornaTabellSeAperta()
        {
            foreach (Form f in Application.OpenForms)
                if (f is FrmTabellaRecapito t) t.AggiornaTabella();
        }

        // ─── HELPERS GDI+ ────────────────────────────────────────────────────
        private static void RoundRegion(Panel pnl, int r)
        {
            pnl.Paint += (s, e) =>
            {
                using (var path = RoundedPath(new Rectangle(0, 0, ((Panel)s).Width - 1, ((Panel)s).Height - 1), r))
                    ((Panel)s).Region = new Region(path);
            };
        }

        private static void RoundRegion_Label(Label lbl, int r)
        {
            lbl.Paint += (s, e) =>
            {
                using (var path = RoundedPath(new Rectangle(0, 0, ((Label)s).Width - 1, ((Label)s).Height - 1), r))
                    ((Label)s).Region = new Region(path);
            };
        }

        private static GraphicsPath RoundedPath(Rectangle b, int r)
        {
            int d = r * 2;
            var p = new GraphicsPath();
            p.AddArc(b.X,         b.Y,          d, d, 180, 90);
            p.AddArc(b.Right - d, b.Y,          d, d, 270, 90);
            p.AddArc(b.Right - d, b.Bottom - d, d, d,   0, 90);
            p.AddArc(b.X,         b.Bottom - d, d, d,  90, 90);
            p.CloseFigure();
            return p;
        }
    }

    // ── Panel con sfondo gradiente verticale ────────────────────────────────
    internal class GradientPanel : Panel
    {
        readonly Color _top, _bot;
        public GradientPanel(Color bottom, Color top) { _top = top; _bot = bottom; DoubleBuffered = true; }
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            using (var br = new LinearGradientBrush(ClientRectangle, _top, _bot, 90f))
                e.Graphics.FillRectangle(br, ClientRectangle);
        }
    }

    // ── Renderer Custom per Menu a Tendina (Dark Theme) ─────────────────────
    internal class DarkMenuRenderer : ToolStripProfessionalRenderer
    {
        public DarkMenuRenderer() : base(new DarkColorTable()) { }
        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            e.TextColor = Color.FromArgb(226, 232, 255); // C_TEXT
            base.OnRenderItemText(e);
        }
    }

    internal class DarkColorTable : ProfessionalColorTable
    {
        public override Color ToolStripDropDownBackground => Color.FromArgb(20, 24, 48); // C_CARD
        public override Color ImageMarginGradientBegin    => Color.FromArgb(20, 24, 48);
        public override Color ImageMarginGradientMiddle   => Color.FromArgb(20, 24, 48);
        public override Color ImageMarginGradientEnd      => Color.FromArgb(20, 24, 48);
        public override Color MenuBorder                  => Color.FromArgb(30, 36, 70); // C_BORDER
        public override Color MenuItemBorder              => Color.Transparent;
        public override Color MenuItemSelected            => Color.FromArgb(124, 58, 237); // C_ACCENT
        public override Color MenuItemSelectedGradientBegin => Color.FromArgb(124, 58, 237);
        public override Color MenuItemSelectedGradientEnd   => Color.FromArgb(124, 58, 237);
    }
}

