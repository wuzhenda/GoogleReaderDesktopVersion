namespace DesktopGoogleReader
{
    using System.Windows;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App() 
        {
            ShutdownMode = ShutdownMode.OnExplicitShutdown;

            
        }

        protected override void OnExit(ExitEventArgs e)
        {
            FeedItemSummarySourceConverter.DeleteTempFiles();
            base.OnExit(e);
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            LogWriter.CreateLogfile();
            LogWriter.WriteTextToLogFile("Initializing application");
            AppController.Start();
        }
    }
}
