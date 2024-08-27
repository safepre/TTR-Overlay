using System.Windows.Forms;
using System.Drawing;
using System;
using TTROverlay.API;
using TTROverlay.Forms;
using static TTROverlay.API.WindowsAPI;
using System.Security.AccessControl;

namespace TTROverlay
{
    public partial class Launcher : Form
    {
        private PictureBox exitButton;
        private PictureBox connectButton;
        private bool isDragging = false;
        private IntPtr toontownWindow = IntPtr.Zero;
        private IntPtr hookHandle = IntPtr.Zero;
        private WindowsAPI.WinEventDelegate winEventDelegate;
        private Point dragStart;
        private ToontownAPI toontown;
        private Overlay overlay;
        public Launcher()
        {
            InitializeComponent();
            SetupForm();
            SetupButtons();
            SetupEventHandlers();
            toontown = new ToontownAPI();
            toontown.DataUpdated += Toontown_DataUpdated;
        }

        private void SetupForm()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.LightYellow;
            this.TransparencyKey = Color.LightYellow;
            this.BackgroundImage = Properties.Resources.LauncherBackground;
            this.BackgroundImageLayout = ImageLayout.None;
            this.Size = this.BackgroundImage.Size;

        }
        private void Toontown_DataUpdated(object sender, APIData e)
        {
            if (overlay != null && !overlay.IsDisposed)
            {
                Invoke(new Action(() =>
                {
                    overlay.UpdateOverlayText($"District: {e.District} ",
                                              $"Neighborhood: {e.Neighborhood}",
                                              $"Location: {e.Zone}");
                }));
            }
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
        private void CreateOverlay()
        {
            if (overlay == null || overlay.IsDisposed)
            {
                overlay = new Overlay();
                SetParent(overlay.Handle, toontownWindow);

            }

            RepositionOverlay();
        }

        private void RepositionOverlay()
        {
            if (IsWindow(toontownWindow) && IsWindowVisible(toontownWindow))
            {
                RECT clientRect;
                GetClientRect(toontownWindow, out clientRect);
                int clientWidth = clientRect.Right - clientRect.Left;
                int clientHeight = clientRect.Bottom - clientRect.Top;
                int overlayWidth = clientWidth / 2;
                int overlayHeight = 25;
                int xPosition = (clientWidth - overlayWidth) / 2;
                int yPosition = 0;
                int initialStyle = GetWindowLong(overlay.Handle, -20);
                SetWindowLong(overlay.Handle, -20, initialStyle | 0x80000 | 0x20);
                SetWindowPos(overlay.Handle, IntPtr.Zero, xPosition, yPosition, overlayWidth, overlayHeight, SWP_NOZORDER | SWP_NOACTIVATE);
                overlay.Visible = true;
            }
            else
            {
                if (overlay != null && !overlay.IsDisposed)
                {
                    overlay.Visible = false;
                }

                FindToontownWindow();
            }
        }

        private void SetupEventHook()
        {
            winEventDelegate = new WindowsAPI.WinEventDelegate(WinEventProc);
            hookHandle = WindowsAPI.SetWinEventHook(
                WindowsAPI.EVENT_OBJECT_LOCATIONCHANGE,
                WindowsAPI.EVENT_OBJECT_LOCATIONCHANGE,
                IntPtr.Zero,
                winEventDelegate,
                0,
                0,
                WindowsAPI.WINEVENT_OUTOFCONTEXT | WindowsAPI.WINEVENT_SKIPOWNTHREAD
            );
        }
        private void WinEventProc(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            if (hwnd == toontownWindow)
            {
                RepositionOverlay();
            }
        }


        private async void ConnectButton_Click(object sender, EventArgs e)
        {
            try
            {
                FindToontownWindow();
                if (toontownWindow != IntPtr.Zero)
                { 
                    await toontown.StartConnection(connectButton);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Can't connect, please close the window and sign in again.", "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void FindToontownWindow()
        {
            toontownWindow = FindWindow(null, "Toontown Rewritten");
            if (toontownWindow != IntPtr.Zero)
            {
                CreateOverlay();
                SetupEventHook();
            }
            else
            {
                MessageBox.Show("Make sure the game is running.");
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
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (hookHandle != IntPtr.Zero)
            {
                WindowsAPI.UnhookWinEvent(hookHandle);
            }
            toontown.StopConnection();
            base.OnFormClosing(e);
        }
    }
}
