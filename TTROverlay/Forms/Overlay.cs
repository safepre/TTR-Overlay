using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using System;

namespace TTROverlay.Forms
{
    public partial class Overlay : Form
    {
            private PrivateFontCollection privateFonts = new PrivateFontCollection();
            private Font customFont;
            private Point mouseLocation;
            private string overlayText = "";      
            private StringFormat centerFormat = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };

        public Overlay()
            {
                InitializeComponent();
                this.FormBorderStyle = FormBorderStyle.None;
                this.ShowInTaskbar = false;
                this.BackColor = Color.Black;
                this.TransparencyKey = Color.Black;
                LoadCustomFont();
                customFont = new Font(privateFonts.Families[0], 18f, FontStyle.Bold);
                this.MouseDown += new MouseEventHandler(mouse_Down);
                this.MouseMove += new MouseEventHandler(mouse_Move);
         
        }

        private void LoadCustomFont()
        {
            var fontBytes = Properties.Resources.Minnie;
            IntPtr fontData = Marshal.AllocCoTaskMem(fontBytes.Length);
            Marshal.Copy(fontBytes, 0, fontData, fontBytes.Length);

            privateFonts.AddMemoryFont(fontData, fontBytes.Length);

            Marshal.FreeCoTaskMem(fontData);
        }
        public void UpdateOverlayText(string text)
            {
                overlayText = text;
                this.Invalidate();
            }


        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.Clear(Color.Black);
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            int sectionWidth = this.Width /3;

            RectangleF centerRect = new RectangleF(sectionWidth, 0, sectionWidth + 350, this.Height);

            using (Brush textBrush = new SolidBrush(Color.White))
            {
                e.Graphics.DrawString(overlayText, customFont, textBrush, centerRect, centerFormat);
            }
        }
        private void mouse_Down(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mouseLocation = e.Location;
            }
        }

        private void mouse_Move(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point newLocation = this.PointToClient(Control.MousePosition);
                this.Location = new Point(
                    this.Location.X + (newLocation.X - mouseLocation.X),
                    /*for some odd reason it shifts down when moving the instance so -20 helps with that*/
                    (this.Location.Y + (newLocation.Y - mouseLocation.Y)-20)); 

            }
        }
    }
}
