using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace TTROverlay.Forms
{
    public partial class Overlay : Form
    {
        
            private string overlayText = "";
            private string overlayText1 = "";
            private string overlayText2 = "";

            Font overlayFont = new Font("Consolas", 13f, FontStyle.Bold);

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
                this.BackColor = Color.Blue;
                this.TransparencyKey = Color.Blue;
            }

            public void UpdateOverlayText(string text1, string text2, string text3)
            {
                overlayText = text1;
                overlayText1 = text2;
                overlayText2 = text3;
                this.Invalidate();
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                e.Graphics.Clear(Color.Blue);
                e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
                e.Graphics.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;

                using (SolidBrush backgroundBrush = new SolidBrush(Color.FromArgb(128, 0, 0, 255)))
                {
                    e.Graphics.FillRectangle(backgroundBrush, this.ClientRectangle);
                }

                int sectionWidth = this.Width / 3;

                RectangleF leftRect = new RectangleF(0, 0, sectionWidth, this.Height);
                RectangleF centerRect = new RectangleF(sectionWidth, 0, sectionWidth, this.Height);
                RectangleF rightRect = new RectangleF(sectionWidth * 2, 0, sectionWidth, this.Height);

                DrawOutlinedText(e.Graphics, overlayText, overlayFont, leftRect, centerFormat, Color.White, Color.Black, 1.75f);
                DrawOutlinedText(e.Graphics, overlayText1, overlayFont, centerRect, centerFormat, Color.White, Color.Black, 1.75f);
                DrawOutlinedText(e.Graphics, overlayText2, overlayFont, rightRect, centerFormat, Color.White, Color.Black, 1.75f);
            }

            private void DrawOutlinedText(Graphics g, string text, Font font, RectangleF rect, StringFormat format, Color fillColor, Color outlineColor, float outlineWidth)
            {
                using (GraphicsPath path = new GraphicsPath())
                using (Pen outlinePen = new Pen(outlineColor, outlineWidth) { LineJoin = LineJoin.Round })
                using (SolidBrush fillBrush = new SolidBrush(fillColor))
                {
                    path.AddString(text, font.FontFamily, (int)font.Style, font.Size, rect, format);
                    g.DrawPath(outlinePen, path);
                    g.FillPath(fillBrush, path);
                }
            }
        }
}
