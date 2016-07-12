namespace DesktopGoogleReader
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows;
    using GoogleReaderAPI;
    using GoogleReaderAPI.DataContracts;
    using System.Collections;

    public class LoginManager
    {
        private BackgroundWorker loginBgWorker;
        private LoginWindow lw;
        private string loginSource;
        private IReader reader;

        public LoginManager()
        {
            loginBgWorker = new BackgroundWorker();
            loginBgWorker.DoWork += new DoWorkEventHandler(loginBgWorker_DoWork);
            loginBgWorker.ProgressChanged += new ProgressChangedEventHandler(loginBgWorker_ProgressChanged);
            loginBgWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(loginBgWorker_RunWorkerCompleted);
            loginBgWorker.WorkerReportsProgress = true;
            LogWriter.WriteTextToLogFile("LoginManager has been initialized");
        }

        void lw_Closed(object sender, EventArgs e)
        {
            Clean(false);
            OnLoginReady(true, null, null);
        }

        public void Login(string source)
        {
            loginSource = source;
            LogWriter.WriteTextToLogFile("Opening login window");
            lw = new LoginWindow()
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                ShowInTaskbar = true
            };
            lw.Loggining += new EventHandler<LoginWindow.LoginEventArgs>(lw_Loggining);
            lw.Closed += new EventHandler(lw_Closed);
            lw.Show();
        }

        void lw_Loggining(object sender, LoginWindow.LoginEventArgs e)
        {
            loginBgWorker.RunWorkerAsync(e);
        }

        void loginBgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            LoginWindow.LoginEventArgs loginData = e.Argument as LoginWindow.LoginEventArgs;
            if (loginData != null)
            {
                LogWriter.WriteTextToLogFile("Trying to login username " + loginData.Username);
                reader = Reader.CreateReader(loginData.Username, loginData.Password, loginSource);
                reader.enableLogOutput(Properties.Settings.Default.writeLogfiles);
            }

            loginBgWorker.ReportProgress(50);
            e.Result = reader.GetUnreadFeedsWithItems() as IList;
        }

        void loginBgWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            lw.ReportLoginProgress(LoginWindow.LoginProgress.LoginOK);
        }

        void loginBgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            LogWriter.WriteTextToLogFile("Logon try finished");
            if (e.Error != null)
            {
                LogWriter.WriteTextToLogFile("Login error");
                LogWriter.WriteTextToLogFile(e.Error);
                if (e.Error is System.Net.WebException)
                {
                    lw.ReportLoginProgress(LoginWindow.LoginProgress.LoginConnectionError);
                }
                else
                {
                    // todo: change
                    lw.ReportLoginProgress(LoginWindow.LoginProgress.LoginFailed);
                    //Clean(true);
                    //OnLoginReady(false, null, reader);
                }
            }
            else
            {

                Clean(true);
                var data = e.Result as IEnumerable<UnreadFeed>;
                OnLoginReady(false, data, reader);
                
            }
        }

        public void Clean(bool doClose)
        {
            lw.Closed -= new EventHandler(lw_Closed);
            lw.Loggining -= new EventHandler<LoginWindow.LoginEventArgs>(lw_Loggining);
            if (doClose)
            {
                lw.Close();
            }
            lw = null;
        }

        protected void OnLoginReady(bool loginCancelled, IEnumerable<UnreadFeed> initialData, IReader reader)
        {
            LogWriter.WriteTextToLogFile("Starting OnLoginReady");
            OnLoginReady(new LoginReadyEventArgs() { 
                LoginCancelled = loginCancelled,
                InitialData = initialData,
                Reader = reader
            });
        }

        protected void OnLoginReady(LoginReadyEventArgs args)
        {
            LogWriter.WriteTextToLogFile("Starting OnLoginReady with args");
            if (LoginReady != null)
            {
                LoginReady(this, args);
            }
        }

        public event EventHandler<LoginReadyEventArgs> LoginReady;

        public class LoginReadyEventArgs : EventArgs
        {
            public bool LoginCancelled { get; set; }
            public IEnumerable<UnreadFeed> InitialData {get; set;}
            public IReader Reader { get; set; }
        }
    }
}
