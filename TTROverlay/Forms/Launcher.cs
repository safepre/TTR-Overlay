using System.Windows.Forms;
using System.Drawing;
using System;

namespace TTROverlay
{
    public partial class Launcher : Form
    {
        private PictureBox exitButton;
        private PictureBox connectButton;
        private bool isDragging = false;
        private Point dragStart;

        public Launcher()
        {
            InitializeComponent();
            SetupForm();
            SetupButtons();
            SetupEventHandlers();
        }

        private void SetupForm()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;
            this.TransparencyKey = Color.White;
            this.BackgroundImage = Properties.Resources.LauncherBackground;
            this.BackgroundImageLayout = ImageLayout.None;
            this.Size = this.BackgroundImage.Size;

        }

        private void SetupButtons()
        {
            exitButton = new PictureBox
            {
                Image = Properties.Resources.ExitButtonNormal,
                SizeMode = PictureBoxSizeMode.AutoSize,
                Location = new Point(0, 0),
            };
            exitButton.MouseEnter += (s, e) => exitButton.Image = Properties.Resources.ExitButtonHover;
            exitButton.MouseLeave += (s, e) => exitButton.Image = Properties.Resources.ExitButtonNormal;
            exitButton.Click += (s, e) => this.Close();

            connectButton = new PictureBox
            {
                Image = Properties.Resources.ConnectButtonNormal,
                SizeMode = PictureBoxSizeMode.AutoSize,
                BackColor = Color.Transparent
            };
            connectButton.Click += ConnectButton_Click;

            connectButton.MouseEnter += (s, e) => connectButton.Image = Properties.Resources.ConnectButtonHover;
            connectButton.MouseLeave += (s, e) => connectButton.Image = Properties.Resources.ConnectButtonNormal;

            connectButton.Location = new Point(
                (this.ClientSize.Width - connectButton.Width) / 2,
                (this.ClientSize.Height - connectButton.Height) / 2
            );

            this.Controls.Add(exitButton);
            this.Controls.Add(connectButton);
        }

        private void SetupEventHandlers()
        {
            this.MouseDown += CustomLauncher_MouseDown;
            this.MouseMove += CustomLauncher_MouseMove;
            this.MouseUp += CustomLauncher_MouseUp;
        }

        private void ConnectButton_Click(object sender, EventArgs e)
        {
            if (connectButton.Image == Properties.Resources.ConnectButtonNormal)
            {
                connectButton.Image = Properties.Resources.ConnectedButtonNormal;
            }
            else
            {
                connectButton.Image = Properties.Resources.ConnectButtonNormal;
            }
        }

        private void CustomLauncher_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = true;
                dragStart = new Point(e.X, e.Y);
            }
        }

        private void CustomLauncher_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                Point p1 = new Point(e.X, e.Y);
                Point p2 = this.PointToScreen(p1);
                Point p3 = new Point(p2.X - dragStart.X, p2.Y - dragStart.Y);
                this.Location = p3;
            }
        }

        private void CustomLauncher_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = false;
            }
        }
    }
}
