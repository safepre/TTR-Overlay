using System.Windows.Forms;
using System.Drawing;
using System;
using TTROverlay.API;
using TTROverlay.Forms;
using static TTROverlay.API.WindowsAPI;

namespace TTROverlay
{
    public partial class Launcher : Form
    {
        private IntPtr toontownWindow = IntPtr.Zero;
        private IntPtr hookHandle = IntPtr.Zero;
        private WindowsAPI.WinEventDelegate winEventDelegate;
        private ToontownAPI toontown;
        private Overlay districtOverlay;
        private Overlay locationOverlay;
        public Launcher()
        {
            InitializeComponent();
            toontown = new ToontownAPI();
            this.Text = "TTR Overlay";
            this.Size = new Size(250, 150);
            this.FormBorderStyle = FormBorderStyle.FixedDialog; 
            this.MaximizeBox = false;
            this.MinimizeBox = false;  
            Button connectButton = new Button();
            connectButton.Text = "Display";
            connectButton.Size = new Size(100, 25);
            connectButton.Location = new Point(75, 50);
            connectButton.Click += ConnectButton_Click;
            this.Controls.Add(connectButton);

            toontown.DataUpdated += Toontown_DataUpdated;
        }


        private void Toontown_DataUpdated(object sender, APIData e)
        {
            if ((districtOverlay != null && !districtOverlay.IsDisposed))
            {
                Invoke(new Action(() =>
                {
                    districtOverlay.UpdateOverlayText($"District: { e.District}");
                }));    
            }

            if ((locationOverlay != null && !locationOverlay.IsDisposed))
            {
                Invoke(new Action(() =>
                {
                    locationOverlay.UpdateOverlayText($"Playground: {e.Neighborhood}");
                }));
            }
        }
        

 
        private void CreateOverlay()
        {
            if ((districtOverlay == null || districtOverlay.IsDisposed) || (locationOverlay == null || locationOverlay.IsDisposed))
            {
                districtOverlay = new Overlay();
                locationOverlay = new Overlay();
                SetParent(districtOverlay.Handle, toontownWindow);
                SetParent(locationOverlay.Handle, toontownWindow);
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

                int districtStyle = GetWindowLong(districtOverlay.Handle, -20);
                SetWindowLong(districtOverlay.Handle, -20, districtStyle);
                SetWindowPos(districtOverlay.Handle, IntPtr.Zero, xPosition, 0, overlayWidth, overlayHeight, SWP_NOZORDER | SWP_NOACTIVATE);
                districtOverlay.Visible = true;

                int locationStyle = GetWindowLong(locationOverlay.Handle, -20);
                SetWindowLong(locationOverlay.Handle, -20, locationStyle);
                SetWindowPos(locationOverlay.Handle, IntPtr.Zero, xPosition, 0, overlayWidth, overlayHeight, SWP_NOZORDER | SWP_NOACTIVATE);
                locationOverlay.Visible = true;
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
                   await toontown.StartConnection();
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
    }
}
