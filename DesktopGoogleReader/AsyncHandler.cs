namespace DesktopGoogleReader
{
    using System;
    using System.Windows.Threading;

    public class AsyncHandler<TResult>
    {
        private Func<TResult> asyncAction;
        private Action<TResult, Exception> resultAction;
        Dispatcher dispatcher;

        public AsyncHandler(Func<TResult> asyncAction, Action<TResult, Exception> resultAction, Dispatcher dispatcher)
        {
            this.asyncAction = asyncAction;
            this.resultAction = resultAction;
            this.dispatcher = dispatcher;
        }

        public void Invoke() 
        {
            asyncAction.BeginInvoke((r) => 
            {
                TResult result = default(TResult);
                Exception e = null;

                try
                {
                    result = asyncAction.EndInvoke(r);
                }
                catch (Exception ex) {
                    e = ex; 
                }

                dispatcher.InvokeMethod(() => 
                {
                    resultAction(result, e);  
                });

            }, null);
        }
    }
}
