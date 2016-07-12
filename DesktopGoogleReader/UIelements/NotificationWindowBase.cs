namespace DesktopGoogleReader
{
    using System;
    using System.Timers;
    using System.Windows.Controls;
    using System.Windows.Interop;
    using System.Windows.Media.Animation;

    public class NotificationWindowBase : UserControl
    {
        #region Fields

        private const int WS_EX_TOPMOST = 0x00000008;
        private const int WS_EX_NOACTIVATE = 0x08000000;
        private HwndSource hwnd;
        private bool isMouseOver;
        private Timer closeTimer;
        string activeCloseAnimationName;

        private int showHideDuration = 3000;
        private int afterMouseLeaveShowDuration = 2000;

        private int animationNameCouner;

        #endregion

        public NotificationWindowBase()
        {
            this.Opacity = 0;
            this.Loaded += new System.Windows.RoutedEventHandler(NotificationWindow_Loaded);
            this.MouseEnter += new System.Windows.Input.MouseEventHandler(NotificationWindowBase_MouseEnter);
            this.MouseLeave += new System.Windows.Input.MouseEventHandler(NotificationWindowBase_MouseLeave);
            closeTimer = new Timer()
            {
                Interval = afterMouseLeaveShowDuration,
                AutoReset = false
            };
            closeTimer.Elapsed += new ElapsedEventHandler(closeTimer_Elapsed);
        }

        void NotificationWindow_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            // gradually show up the windows:
            DoubleAnimation a = new DoubleAnimation();
            a.To = 1;
            a.Duration = TimeSpan.FromMilliseconds(showHideDuration);
            a.Completed += ((s, ar) =>
            {
                // if mouse is not over window when we finished showin-up, begin closing it:
                if (!isMouseOver)
                    this.closeTimer.Enabled = true;
            });
            this.BeginAnimation(UserControl.OpacityProperty, a);
        }

        void NotificationWindowBase_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            isMouseOver = false;
            BeginCloseAnimation();
        }

        void NotificationWindowBase_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            isMouseOver = true;
            this.BeginAnimation(UserControl.OpacityProperty, null);
            this.Opacity = 1;
            activeCloseAnimationName = null;
        }

        void closeTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            BeginCloseAnimation();
        }

        private void BeginCloseAnimation() 
        {
            this.Dispatcher.BeginInvokeMethod(() =>
            {
                DoubleAnimation a = new DoubleAnimation();
                a.From = 1;
                a.To = 0;
                a.Duration = TimeSpan.FromMilliseconds(showHideDuration);
                a.Name = "name" + (++animationNameCouner).ToString();
                a.Completed += ((s, ar) =>
                {
                    // cancelled closing animations still appear here, so we act (close window) 
                    // only if this is current animation:
                    var animationClock = s as AnimationClock;
                    if (animationClock != null)
                    {
                        var senderAnimation = animationClock.Timeline as DoubleAnimation;
                        if (!string.IsNullOrEmpty(activeCloseAnimationName) &&
                            (senderAnimation != null && activeCloseAnimationName == senderAnimation.Name))
                        {
                            this.Close();
                        }
                    }
                });
                activeCloseAnimationName = a.Name;
                this.BeginAnimation(UserControl.OpacityProperty, a);
            });
        }

        public void Show(int x, int y)
        {
            // show window, so that it does not force current window lose focus:

            if (hwnd != null) {
                hwnd.Dispose();
            }

            HwndSourceParameters param = new HwndSourceParameters("Notification", Convert.ToInt32(this.Width), Convert.ToInt32(this.Height));
            param.UsesPerPixelOpacity = true;
            param.ExtendedWindowStyle = WS_EX_TOPMOST | WS_EX_NOACTIVATE;
            param.PositionX = x;
            param.PositionY = y;

            hwnd = new HwndSource(param);
            hwnd.RootVisual = this;
        }

        public void Close() 
        {
            if (hwnd != null)
            {
                hwnd.Dispose();
            }
            OnClosed();
        }

        protected virtual void OnClosed()
        {
            if (Closed != null)
            {
                Closed(this, EventArgs.Empty);
            }
        }

        public event EventHandler Closed;
    }
}
