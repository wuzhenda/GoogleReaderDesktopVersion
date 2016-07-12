namespace DesktopGoogleReader
{
    using System;
    using System.Windows;
    using System.Windows.Controls.Primitives;
    using System.Windows.Media;
    using System.Windows.Media.Animation;

    public class UnreadItemStarButton : RotatingAsyncToggleButton<GoogleReaderAPI.DataContracts.UnreadItem> 
    { 
    
    }

    public class RotatingAsyncToggleButton<AsyncActionTarget> : ToggleButton 
        where AsyncActionTarget : class
    {
        private TimeSpan _AnimationLength = TimeSpan.FromMilliseconds(1200);
        private DoubleAnimation anim;
        private bool asyncActionInProgress = false;
        private bool rotating = false;

        public static DependencyProperty AsyncCheckActionProperty =
            DependencyProperty.Register("AsyncCheckAction", typeof(Func<AsyncActionTarget, bool>), typeof(RotatingAsyncToggleButton<AsyncActionTarget>));

        public static DependencyProperty AsyncUnCheckActionProperty =
            DependencyProperty.Register("AsyncUnCheckAction", typeof(Func<AsyncActionTarget, bool>), typeof(RotatingAsyncToggleButton<AsyncActionTarget>));

        public static DependencyProperty ActionTargetProperty =
            DependencyProperty.Register("ActionTarget", typeof(AsyncActionTarget), typeof(RotatingAsyncToggleButton<AsyncActionTarget>));

        public AsyncActionTarget ActionTarget
        {
            get
            {
                return GetValue(ActionTargetProperty) as AsyncActionTarget;
            }
            set
            {
                SetValue(ActionTargetProperty, value);
            }
        }

        public Func<AsyncActionTarget, bool> AsyncCheckAction
        {
            get 
            {
                return GetValue(AsyncCheckActionProperty) as Func<AsyncActionTarget, bool>;
            }
            set 
            {
                SetValue(AsyncCheckActionProperty, value);
            }
        }

        public Func<AsyncActionTarget, bool> AsyncUnCheckAction
        {
            get
            {
                return GetValue(AsyncUnCheckActionProperty) as Func<AsyncActionTarget, bool>;
            }
            set
            {
                SetValue(AsyncUnCheckActionProperty, value);
            }
        }

        protected override void OnClick()
        {
            if (!rotating)
            {
                if (IsChecked.GetValueOrDefault(false))
                {
                    CheckAsync(AsyncUnCheckAction, false);
                }
                else
                {
                    CheckAsync(AsyncCheckAction, true);
                }
            }
        }

        public void CheckAsync(Func<AsyncActionTarget, bool> asyncAction, bool checkResult)
        {
            if (asyncAction != null)
            {
                asyncActionInProgress = true;
                rotating = true;
                Rotate();
                asyncAction.BeginInvoke(ActionTarget, (r) =>
                {
                    bool? result = null;
                    try
                    {
                        result = asyncAction.EndInvoke(r);
                    }
                    catch (Exception){}

                    if (result.HasValue && result.Value)
                    {
                        this.Dispatcher.InvokeMethod(() => 
                        {
                            IsChecked = checkResult;
                        });
                    }
                        
                    asyncActionInProgress = false;

                }, null);
            }
        }

        private void EndRotate()
        {
            if (anim != null)
            {
                anim.Completed -= new EventHandler(anim_Completed);
                this.RenderTransform.BeginAnimation(RotateTransform.AngleProperty, null);
                anim = null;
            }
            rotating = false;
        }

        void anim_Completed(object sender, EventArgs e)
        {
            if (!asyncActionInProgress)
            {
                EndRotate();
            }
            else
            {
                Rotate();
            }
        }

        private void Rotate()
        {
            RotateTransform trans = new RotateTransform();

            this.RenderTransform = trans;
            trans.CenterX = (this.ActualWidth / 2);
            trans.CenterY = (this.ActualHeight / 2) * 1.1;
            trans.Angle = 0;

            anim = new DoubleAnimation();
            anim.To = 360;
            anim.Duration = _AnimationLength;
            anim.Completed += new EventHandler(anim_Completed);

            trans.BeginAnimation(RotateTransform.AngleProperty, anim, HandoffBehavior.Compose);
        }
    }
}
