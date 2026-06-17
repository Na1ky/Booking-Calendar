using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Calendario.VIEW
{
    public class ModernButton : Button
    {
        private int borderSize = 0;
        private int borderRadius = 8;
        private Color borderColor = Color.PaleVioletRed;
        private bool isHovered = false;
        private bool isPressed = false;
        
        public int BorderRadius { get { return borderRadius; } set { borderRadius = value; this.Invalidate(); } }

        public ModernButton()
        {
            this.FlatStyle = FlatStyle.Flat;
            this.FlatAppearance.BorderSize = 0;
            this.Size = new Size(150, 40);
            this.BackColor = Color.FromArgb(124, 58, 237);
            this.ForeColor = Color.White;
            this.Cursor = Cursors.Hand;
            this.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            isHovered = true;
            this.Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            isHovered = false;
            this.Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs mevent)
        {
            base.OnMouseDown(mevent);
            isPressed = true;
            this.Invalidate();
        }

        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            base.OnMouseUp(mevent);
            isPressed = false;
            this.Invalidate();
        }

        private GraphicsPath GetFigurePath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            float curveSize = radius * 2F;
            path.StartFigure();
            path.AddArc(rect.X, rect.Y, curveSize, curveSize, 180, 90);
            path.AddArc(rect.Right - curveSize, rect.Y, curveSize, curveSize, 270, 90);
            path.AddArc(rect.Right - curveSize, rect.Bottom - curveSize, curveSize, curveSize, 0, 90);
            path.AddArc(rect.X, rect.Bottom - curveSize, curveSize, curveSize, 90, 90);
            path.CloseFigure();
            return path;
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            base.OnPaint(pevent);
            pevent.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle rectSurface = this.ClientRectangle;
            Rectangle rectBorder = Rectangle.Inflate(rectSurface, -borderSize, -borderSize);
            int smoothSize = 2;
            if (borderSize > 0) smoothSize = borderSize;

            if (borderRadius > 2)
            {
                using (GraphicsPath pathSurface = GetFigurePath(rectSurface, borderRadius))
                using (GraphicsPath pathBorder = GetFigurePath(rectBorder, borderRadius - borderSize))
                using (Pen penSurface = new Pen(this.Parent.BackColor, smoothSize))
                using (Pen penBorder = new Pen(borderColor, borderSize))
                {
                    this.Region = new Region(pathSurface);
                    pevent.Graphics.DrawPath(penSurface, pathSurface);
                    if (borderSize >= 1) pevent.Graphics.DrawPath(penBorder, pathBorder);
                }
            }
            else
            {
                this.Region = new Region(rectSurface);
                if (borderSize >= 1)
                {
                    using (Pen penBorder = new Pen(borderColor, borderSize))
                    {
                        penBorder.Alignment = PenAlignment.Inset;
                        pevent.Graphics.DrawRectangle(penBorder, 0, 0, this.Width - 1, this.Height - 1);
                    }
                }
            }

            // Fill background based on state
            Color bg = this.BackColor;
            if (!this.Enabled) bg = Color.FromArgb(40, 48, 90);
            else if (isPressed) bg = Color.FromArgb(Math.Max(0, bg.R - 30), Math.Max(0, bg.G - 30), Math.Max(0, bg.B - 30));
            else if (isHovered) bg = Color.FromArgb(Math.Min(255, bg.R + 30), Math.Min(255, bg.G + 30), Math.Min(255, bg.B + 30));
            
            using (SolidBrush brush = new SolidBrush(bg))
            {
                pevent.Graphics.FillPath(brush, GetFigurePath(rectSurface, borderRadius));
            }

            // Draw text
            TextRenderer.DrawText(pevent.Graphics, this.Text, this.Font, rectSurface, this.ForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }
    }

    public class ModernPanel : Panel
    {
        private int borderRadius = 10;
        public int BorderRadius { get { return borderRadius; } set { borderRadius = value; this.Invalidate(); } }

        public ModernPanel()
        {
            this.BackColor = Color.FromArgb(22, 28, 62);
            this.ForeColor = Color.White;
        }

        private GraphicsPath GetFigurePath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            float curveSize = radius * 2F;
            path.StartFigure();
            path.AddArc(rect.X, rect.Y, curveSize, curveSize, 180, 90);
            path.AddArc(rect.Right - curveSize, rect.Y, curveSize, curveSize, 270, 90);
            path.AddArc(rect.Right - curveSize, rect.Bottom - curveSize, curveSize, curveSize, 0, 90);
            path.AddArc(rect.X, rect.Bottom - curveSize, curveSize, curveSize, 90, 90);
            path.CloseFigure();
            return path;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle rectSurface = this.ClientRectangle;
            if (borderRadius > 2)
            {
                using (GraphicsPath pathSurface = GetFigurePath(rectSurface, borderRadius))
                using (Pen penSurface = new Pen(this.Parent.BackColor, 2))
                {
                    this.Region = new Region(pathSurface);
                    e.Graphics.DrawPath(penSurface, pathSurface);
                }
            }
        }
    }

    public class ModernInputContainer : Panel
    {
        private int borderRadius = 8;
        private bool isFocused = false;

        // Colori
        private static readonly Color BG_NORMAL  = Color.FromArgb(18, 24, 54);
        private static readonly Color BG_FOCUS   = Color.FromArgb(26, 34, 72);
        private static readonly Color BORDER_NORMAL = Color.FromArgb(55, 68, 130);
        private static readonly Color BORDER_FOCUS  = Color.FromArgb(124, 58, 237);
        private static readonly Color BORDER_GLOW   = Color.FromArgb(50, 124, 58, 237);

        public int BorderRadius { get { return borderRadius; } set { borderRadius = value; this.Invalidate(); } }

        public ModernInputContainer()
        {
            this.BackColor = BG_NORMAL;
            this.Padding = new Padding(10, 7, 10, 7);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
        }

        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);
            // Monitora focus dei figli per l'effetto glow
            AttachFocusEvents(e.Control);
        }

        private void AttachFocusEvents(Control c)
        {
            c.Enter += (s, ev) => { isFocused = true;  this.BackColor = BG_FOCUS; this.Invalidate(); };
            c.Leave += (s, ev) => { isFocused = false; this.BackColor = BG_NORMAL; this.Invalidate(); };
            foreach (Control child in c.Controls) AttachFocusEvents(child);
        }

        private GraphicsPath GetRoundedPath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            float d = radius * 2F;
            path.StartFigure();
            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            Rectangle rect = Rectangle.Inflate(this.ClientRectangle, -2, -2);

            using (GraphicsPath path = GetRoundedPath(rect, borderRadius))
            {
                // Sfondo
                using (SolidBrush bg = new SolidBrush(this.BackColor))
                    e.Graphics.FillPath(bg, path);

                // Bordo esterno glow (solo quando focused)
                if (isFocused)
                {
                    Rectangle gRect = Rectangle.Inflate(rect, 1, 1);
                    using (GraphicsPath gPath = GetRoundedPath(gRect, borderRadius + 1))
                    using (Pen glow = new Pen(BORDER_GLOW, 3f))
                        e.Graphics.DrawPath(glow, gPath);
                }

                // Bordo principale
                Color borderC = isFocused ? BORDER_FOCUS : BORDER_NORMAL;
                using (Pen pen = new Pen(borderC, 1.5f))
                    e.Graphics.DrawPath(pen, path);

                // Clip region per nascondere angoli
                this.Region = new Region(GetRoundedPath(this.ClientRectangle, borderRadius + 1));
            }
        }
    }
}
