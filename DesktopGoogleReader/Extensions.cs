namespace DesktopGoogleReader
{
    using System;
    using System.Windows.Threading;

    public static class Extensions
    {
        public static DispatcherOperation BeginInvokeMethod(this Dispatcher dispatcher, Action action)
        {
            return dispatcher.BeginInvoke(action);
        }

        public static object InvokeMethod(this Dispatcher dispatcher, Action action)
        {
            return dispatcher.Invoke(action);
        }
    }
}
