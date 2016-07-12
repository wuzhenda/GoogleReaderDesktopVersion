namespace DesktopGoogleReader
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Windows;
    using GoogleReaderAPI;
    using GoogleReaderAPI.DataContracts;

    public class AppController
    {
        #region Fields & Properties

        private MainWindow mw;
        private NewUnreadItemMonitor unreadItemMonitor;
        private NotificationManager notificationManager;
        private LoginManager loginManager;

        public static AppController Current { get; private set; }
        public static IReader CurrentReader { get; private set; }

        private System.Net.IWebProxy originalProxy = System.Net.HttpWebRequest.DefaultWebProxy;

        #endregion

        private AppController(bool withLoginManager) 
        {
            Current = this;
            
            LogWriter.WriteTextToLogFile("Initializing AppController");
            if (withLoginManager)
            {
                setProxy();
                LogWriter.WriteTextToLogFile("Creating new LoginManager");
                LogWriter.WriteTextToLogFile("Creating LoginManager");
                loginManager = new LoginManager();
                loginManager.LoginReady += new EventHandler<LoginManager.LoginReadyEventArgs>(loginManager_LoginReady);
                loginManager.Login(ConfigurationManager.AppSettings["source"]);
            }

        }
        
        public static void Start() 
        {
            LogWriter.WriteTextToLogFile("Checking if we have some older settings to update");
            try
            {
                string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string alreadyMigratedSettingsTriggerFile = appDataPath + "\\Desktop Google Reader\\PreferencesMigrated-" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString() + ".dgr";
                if (!System.IO.File.Exists(alreadyMigratedSettingsTriggerFile))                
                {
                    DesktopGoogleReader.Properties.Settings.Default.Upgrade();
                    if(!System.IO.Directory.Exists(appDataPath  + "\\Desktop Google Reader"))
                    {
                        System.IO.Directory.CreateDirectory(appDataPath + "\\Desktop Google Reader");
                    }
                    System.IO.File.Create(alreadyMigratedSettingsTriggerFile);
                }
                DesktopGoogleReader.Properties.Settings.Default.LatestStartedVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                DesktopGoogleReader.Properties.Settings.Default.Save();
            }
            catch (Exception e)
            {
                LogWriter.WriteTextToLogFile("ERROR - not fatal");
                LogWriter.WriteTextToLogFile(e);
            }

            LogWriter.WriteTextToLogFile("Starting check if update is available");
            try
            {
                if (Properties.Settings.Default.useAutomaticUpdate)
                {
                    Winkle.VersionCheck myUpdateChecker = new Winkle.VersionCheck("Desktop Google Reader", "http://tlhan-ghun.de/files/desktopGoogleReaderWinkle.xml");
                    Winkle.UpdateInfo myUpdateResponse = myUpdateChecker.checkForUpdate(System.Reflection.Assembly.GetExecutingAssembly(), Properties.Settings.Default.useAutomaticUpdateBeta);
                    Console.WriteLine("Update check done");
                }
            }
            catch (Exception e)
            {
                LogWriter.WriteTextToLogFile("ERROR - but we can run without that");
                LogWriter.WriteTextToLogFile(e);
            }


            Current = new AppController(true);
        }

        private void Start(IEnumerable<UnreadFeed> initialData, IReader reader)
        {
            LogWriter.WriteTextToLogFile("AppController start");
            CurrentReader = reader;
            
            LogWriter.WriteTextToLogFile("Trying to open MainWindow");

                try
                {
                    
                    
                    mw = new MainWindow()
                    {
                        WindowStartupLocation = WindowStartupLocation.CenterScreen,
                        ShowInTaskbar = true,
                        Topmost = false
                    };
                    if (initialData == null)
                    {
                        initialData = new List<UnreadFeed>();
                    }
                    mw.SetData(initialData);
                }
                catch (Exception e2)
                {
                    LogWriter.WriteTextToLogFile("FATAL: Couldn't also open mainwindow");
                    LogWriter.WriteTextToLogFile(e2);
                    throw e2;
                }
   

                mw.Width = Properties.Settings.Default.WindowWidth;
                mw.Height = Properties.Settings.Default.WindowHeight;
                mw.Top = Properties.Settings.Default.WindowXPos;
                mw.Left = Properties.Settings.Default.WindowYPos;

                mw.Show();

                
            

            LogWriter.WriteTextToLogFile("Initializing unreadItemMonitor");
            try
            {
                unreadItemMonitor = new NewUnreadItemMonitor(reader, initialData, mw.Dispatcher);
            }
            catch (Exception e)
            {
                LogWriter.WriteTextToLogFile("Failed to initialize");
                LogWriter.WriteTextToLogFile(e);
                throw e;
            }

            LogWriter.WriteTextToLogFile("Initializing notificationManager");
            try
            {
                notificationManager = new NotificationManager(unreadItemMonitor, mw.Dispatcher);
            }
            catch (Exception e)
            {
                LogWriter.WriteTextToLogFile("Initializing failed");
                LogWriter.WriteTextToLogFile(e);
                throw e;
            }

            unreadItemMonitor.DataChanged += new EventHandler<NewUnreadItemMonitor.UnreadItemCollectionChangedEventArgs>(unreadItemMonitor_DataChanged);
            this.setCurrentProgressStateOnWindow(true);
            this.setCurrentProgressDescription("Getting initial feed list");
            unreadItemMonitor.StartMonitoring();

            unreadItemMonitor.setNewPollingInterval((int)Properties.Settings.Default.RefreshInterval * 60);

            AppController.Current.updateCurrentActionInformation(false, "Welcome to Desktop Google Reader - have fun");

            LogWriter.WriteTextToLogFile("Checking if first start and Snarl not running");
            try
            {
                if (Properties.Settings.Default.showSnarlDownloadHint)
                {
                    if (Snarl.SnarlConnector.GetSnarlWindow() == IntPtr.Zero)
                    {
                        UIelements.useSnarlPopups mySnarlPusher = new UIelements.useSnarlPopups();
                        mySnarlPusher.Show();
                    }
                    Properties.Settings.Default.showSnarlDownloadHint = false;
                }
            }
            catch (Exception e)
            {
                LogWriter.WriteTextToLogFile("ERROR - but we can run without that");
                LogWriter.WriteTextToLogFile(e);
            }

            
            mw.OnSelectedItemChanged(null, null);

            unreadItemMonitor_RefreshNow();
        }

        public static void QuickStart() 
        {
            Current = new AppController(false);

            //var reader = Reader.CreateReader(ConfigurationManager.AppSettings["email"], 
            //    ConfigurationManager.AppSettings["password"], ConfigurationManager.AppSettings["source"]);
            var reader = Reader.CreateTestReader();

            var data = reader.GetUnreadFeedsWithItems();
            Current.Start(data, reader);
        }

        void loginManager_LoginReady(object sender, LoginManager.LoginReadyEventArgs e)
        {
            if (e.LoginCancelled)
            {
                ShutDown();
            }
            else
            {
                this.Start(e.InitialData, e.Reader);
            }
        }

        void unreadItemMonitor_DataChanged(object sender, NewUnreadItemMonitor.UnreadItemCollectionChangedEventArgs e)
        {
            mw.RefreshData(e.FreshItems, e.RemovedItemIds);
            if (notificationManager != null)
            {
                notificationManager.StartNotification();
            }
        }

        public void unreadItemMonitor_RefreshNow()
        {
            if (notificationManager != null)
            {
                notificationManager.unreadItemMonitor_RefreshNow();
            }
        }

        public void unreadItemMonitor_SetNewPollingInterval(int intervalInSeconds)
        {
            if (notificationManager != null)
            {
                notificationManager.unreadItemMonitor_SetNewPollingInterval(intervalInSeconds);
            }
        }
        
        public void ResumeNotifications()
        {
            if (notificationManager != null && unreadItemMonitor != null)
            {
                unreadItemMonitor.StoreFreshItems = true;
                notificationManager.ResumeNotification();
            }
        }

        public void StopNotifications()
        {
            if (notificationManager != null && unreadItemMonitor != null)
            {
                unreadItemMonitor.StoreFreshItems = false;
                unreadItemMonitor.FreshItems.Clear();
                notificationManager.StopNotification();
            }
        }

        private void setCurrentProgressStateOnWindow(bool currentProgressState)
        {
            mw.setCurrentActionState(currentProgressState);
            
        }

        private void setCurrentProgressDescription(string descriptiveText)
        {
            mw.labelCurrentAction.Content = descriptiveText;
        }

        public void updateCurrentActionInformation(bool activateCurrentProgressIcon, string descriptiveText)
        {
            try
            {
                setCurrentProgressDescription(descriptiveText);
                setCurrentProgressStateOnWindow(activateCurrentProgressIcon);
                
            }
            catch (Exception e)
            {
                Console.WriteLine("Thread already in use so no update now: " + e.Message);
            }
        }


        public void ShutDown() {
            if (notificationManager != null) {
                notificationManager.shutdownNotificationManager();
            }


            if (mw != null)
            {
                Properties.Settings.Default.WindowWidth = mw.Width;
                Properties.Settings.Default.WindowHeight = mw.Height;
                Properties.Settings.Default.WindowXPos = mw.Top;
                Properties.Settings.Default.WindowYPos = mw.Left;

            }
            Properties.Settings.Default.Save();
            Application.Current.Shutdown();
        }

        public void refreshGui() {
            mw.SetUnreadTitle();
            mw.SetTitle();
        }

        public int getCurrentMaximumNotificationLength()
        {
            return (int)Properties.Settings.Default.MaximumNotificationLength;
        }

        public void bringMainWindowToFront()
        {
            mw.Activate();
            mw.Topmost = true;
        }

        public void setProxy()
        {
            if (DesktopGoogleReader.Properties.Settings.Default.proxyEnabled && DesktopGoogleReader.Properties.Settings.Default.proxyServer != "" && DesktopGoogleReader.Properties.Settings.Default.proxyPort != 0)
            {
                System.Net.WebProxy myProxy = new System.Net.WebProxy(DesktopGoogleReader.Properties.Settings.Default.proxyServer, DesktopGoogleReader.Properties.Settings.Default.proxyPort);
                if (DesktopGoogleReader.Properties.Settings.Default.proxyUser != "")
                {
                    System.Net.NetworkCredential myCredentials = new System.Net.NetworkCredential(DesktopGoogleReader.Properties.Settings.Default.proxyUser, DesktopGoogleReader.Properties.Settings.Default.proxyPassword);
                    myProxy.Credentials = myCredentials;
                }
                System.Net.WebRequest.DefaultWebProxy = myProxy;
                System.Net.HttpWebRequest.DefaultWebProxy = myProxy;
                System.Net.WebRequest.DefaultWebProxy = myProxy;
            }
            else
            {
                System.Net.WebRequest.DefaultWebProxy = originalProxy;
                System.Net.HttpWebRequest.DefaultWebProxy = originalProxy;
                System.Net.WebRequest.DefaultWebProxy = originalProxy;
            }

        }

        public bool setFeedAsRead(UnreadFeed feed)
        {
            System.Collections.ObjectModel.Collection<UnreadItem> tempList = new System.Collections.ObjectModel.Collection<UnreadItem>();
            
            try
            {
                foreach (UnreadItem item in feed.Items)
                {
                    tempList.Add(item);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            foreach (UnreadItem item in tempList)
            {
                mw.RefreshData(null, new Dictionary<string, string>() 
                    { 
                        { item.Id, item.ParentFeedUrl }
                    });
            }

            foreach (UnreadItem item in tempList)
            {
                updateCurrentActionInformation(false, "Marking item " + item.Title + " as read");
                CurrentReader.SetAsRead(item.Id,item.ParentFeedUrl);
                
            }
            return true;
        }

        public void RefreshDataInGui(List<UnreadItem> addedItems, Dictionary<string, string> removedItems)
        {
            mw.RefreshData(addedItems, removedItems);
        }

        public bool setAllAsRead()
        {
            System.Collections.ObjectModel.Collection<UnreadItem> tempList = new System.Collections.ObjectModel.Collection<UnreadItem>();
            foreach (UnreadItem item in CurrentReader.GetUnreadItems())
            {
                tempList.Add(item);
                mw.RefreshData(null, new Dictionary<string, string>() 
                { 
                    { item.Id, item.ParentFeedUrl }
                });
            }

            foreach (UnreadItem item in tempList)
            {
                updateCurrentActionInformation(false, "Marking item " + item.Title + " as read");
                CurrentReader.SetAsRead(item.Id, item.ParentFeedUrl);
      
            }
            return true;
        }
    }
}
