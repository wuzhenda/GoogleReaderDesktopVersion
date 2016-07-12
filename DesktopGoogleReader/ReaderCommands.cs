namespace DesktopGoogleReader
{
    using System;
    using System.Windows.Input;
    using GoogleReaderAPI.DataContracts;

    public static class ReaderCommands
    {
        private static RoutedUICommand setAsRead;
        private static RoutedUICommand setFeedAsRead;
        private static RoutedUICommand setAsShared;
        private static RoutedUICommand goToSource;

        private static RoutedUICommand goLeft;
        private static RoutedUICommand goRight;

        private static RoutedUICommand goFeedUp;
        private static RoutedUICommand goFeedDown;

        static ReaderCommands()
        {
            InputGestureCollection inputs = new InputGestureCollection();
            inputs.Add(new KeyGesture(Key.Delete));
            setAsRead = new RoutedUICommand("Set as read", "Set as read", typeof(ReaderCommands), inputs);

            inputs = new InputGestureCollection();
            inputs.Add(new KeyGesture(Key.Tab));
            goToSource = new RoutedUICommand("Go To Source", "Go To Source", typeof(ReaderCommands), inputs);

            inputs = new InputGestureCollection();
            inputs.Add(new KeyGesture(Key.Delete));
            setFeedAsRead = new RoutedUICommand("Mark feed read", "Mark feed read", typeof(ReaderCommands), inputs);

            inputs = new InputGestureCollection();
            inputs.Add(new KeyGesture(Key.Add));
            setAsShared = new RoutedUICommand("Share item", "Share item", typeof(ReaderCommands), inputs);

            inputs = new InputGestureCollection();
            inputs.Add(new KeyGesture(Key.Left));
            goLeft = new RoutedUICommand("Go left", "go left", typeof(ReaderCommands), inputs);

            inputs = new InputGestureCollection();
            inputs.Add(new KeyGesture(Key.Right));
            goRight = new RoutedUICommand("Go right", "go right", typeof(ReaderCommands), inputs);

            inputs = new InputGestureCollection();
            inputs.Add(new KeyGesture(Key.PageUp));
            goFeedUp = new RoutedUICommand("Go feed up", "Go feed up", typeof(ReaderCommands), inputs);

            inputs = new InputGestureCollection();
            inputs.Add(new KeyGesture(Key.PageDown));
            goFeedDown = new RoutedUICommand("Go feed down", "Go feed down", typeof(ReaderCommands), inputs);
        }

        public static RoutedUICommand SetAsRead
        {
            get 
            { 
                return setAsRead; 
            }
        }

        public static RoutedUICommand SetFeedAsRead
        {
            get
            {
                return setFeedAsRead;
            }
        }

        public static RoutedUICommand SetAsShared
        {
            get
            {
                return setAsShared;
            }
        }


        public static RoutedUICommand GoToSource
        {
            get 
            { 
                return goToSource; 
            }
        }

        public static RoutedUICommand GoLeft
        {
            get
            {
                return goLeft;
            }
        }

        public static RoutedUICommand GoRight
        {
            get
            {
                return goRight;
            }
        }

        public static RoutedUICommand GoFeedUp
        {
            get
            {
                return goFeedUp;
            }
        }

        public static RoutedUICommand GoFeedDown
        {
            get
            {
                return goFeedDown;
            }
        }

        public static void OpenPage(UnreadItem item)
        {
            try
            {
                if (item != null && !string.IsNullOrEmpty(item.ItemUrl))
                {
                    //System.Diagnostics.Process.Start("IExplore.exe", item.ItemUrl);
                    if (item.ItemUrl.StartsWith("http://") || item.ItemUrl.StartsWith("https://"))
                    {
                        System.Diagnostics.Process.Start(item.ItemUrl);
                    }
                }
            }
            catch (Exception) { }
        }
    }
}
