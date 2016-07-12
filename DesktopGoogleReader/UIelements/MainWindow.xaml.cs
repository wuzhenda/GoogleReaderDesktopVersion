using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GoogleReaderAPI.DataContracts;
using System.Windows.Data;
using System.ComponentModel;
using System.IO;
using System.Windows.Media.Imaging;

namespace DesktopGoogleReader
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Data

        private WindowState m_storedWindowState = WindowState.Normal;
        private System.Windows.Forms.NotifyIcon m_notifyIcon;
        private System.Windows.Forms.ContextMenu m_notifyMenu;
        private static List<string> tempFilesList = new List<string>();

        private System.Windows.Forms.Integration.WindowsFormsHost _MyHost;
        public WebKit.WebKitBrowser browser;

        public ObservableCollection<UnreadFeed> DataSource
        {
            get;
            set;
        }

        public Func<UnreadItem, bool> StarAction
        {
            get
            {
                return (i) =>
                {
                    return AppController.CurrentReader.SetAsStarred(i.Id, i.ParentFeedUrl, true);
                };
            }
        }

        public Func<UnreadItem, bool> UnStarAction
        {
            get
            {
                return (i) =>
                {
                    return AppController.CurrentReader.SetAsStarred(i.Id, i.ParentFeedUrl, false);
                };
            }
        }

        #endregion

        #region Init

        public MainWindow()
        {
            InitializeComponent();

            _MyHost = new System.Windows.Forms.Integration.WindowsFormsHost();
            browser = new WebKit.WebKitBrowser();
            _MyHost.Child = browser;
            WebkitBrowserGrid.Children.Add(_MyHost);

            string[] commandLine = Environment.GetCommandLineArgs();

            if (commandLine.Length == 2)
            {
                DesktopGoogleReader.UIelements.SubscribeToNewFeed mySubWindows = new DesktopGoogleReader.UIelements.SubscribeToNewFeed();
                mySubWindows.textBoxFeedUrl.Text = commandLine[1];
                mySubWindows.Show();
            }

            SetCommandBindings();

            // tray icon stuff
            m_notifyIcon = new System.Windows.Forms.NotifyIcon();
            m_notifyIcon.Text = "Desktop Google Reader";
            m_notifyIcon.Icon = new System.Drawing.Icon("Resources\\Images\\tray.ico");
            m_notifyIcon.DoubleClick += new EventHandler(m_notifyIcon_Click);

            m_notifyMenu = new System.Windows.Forms.ContextMenu();
            m_notifyMenu.MenuItems.Add("Desktop Google Reader");
            m_notifyMenu.MenuItems.Add("-");
            m_notifyMenu.MenuItems.Add(new System.Windows.Forms.MenuItem("Show main window", new System.EventHandler(trayContextShow)));
            m_notifyMenu.MenuItems.Add(new System.Windows.Forms.MenuItem("Refresh now", new System.EventHandler(trayContextRefresh)));
            m_notifyMenu.MenuItems.Add("-");
            m_notifyMenu.MenuItems.Add(new System.Windows.Forms.MenuItem("Quit", new System.EventHandler(trayContextQuit)));

            m_notifyIcon.ContextMenu = m_notifyMenu;

            
            browser.Navigating += delegate { 
                AppController.Current.updateCurrentActionInformation(true, "Loading page..."); 
                    
            };
            browser.DocumentCompleted += delegate { AppController.Current.updateCurrentActionInformation(false, "");   };
            browser.DocumentTitleChanged += delegate {
                if (!browser.Url.ToString().StartsWith("file"))
                {
                    labelCurrentItemUrl.Text = browser.Url.AbsoluteUri;
                }
            };
            browser.UserAgent = "Desktop Google Reader (http://desktopgooglereader.codeplex.com/)";
            
            
        }

        ~MainWindow()
        {
            try
            {
                if (m_notifyIcon != null)
                {
                    m_notifyIcon.Visible = false;
                    m_notifyIcon.Dispose();
                    m_notifyIcon = null;
                }
            }
            catch { }
        }

        #endregion

        #region Tray icon 

        protected void trayContextShow(Object sender, System.EventArgs e)
        {
            Show();
        }

        protected void trayContextRefresh(Object sender, System.EventArgs e)
        {
            AppController.Current.unreadItemMonitor_RefreshNow();
        }

        protected void trayContextQuit(Object sender, System.EventArgs e)
        {
            buttonCloseMe_Click(null, null);
        }

        #endregion

        #region Command bindings

        private void SetCommandBindings()
        {
            this.CommandBindings.Add(new CommandBinding(ReaderCommands.SetAsRead, OnSetAsRead, OnCanExecute));
            this.CommandBindings.Add(new CommandBinding(ReaderCommands.SetFeedAsRead, OnSetFeedAsRead, OnCanExecute));
            
            this.CommandBindings.Add(new CommandBinding(ReaderCommands.GoToSource, OnGoToSource, OnCanExecute));
            this.CommandBindings.Add(new CommandBinding(ReaderCommands.GoLeft, OnGoLeft, OnCanExecute));
            this.CommandBindings.Add(new CommandBinding(ReaderCommands.GoRight, OnGoRight, OnCanExecute));
            this.CommandBindings.Add(new CommandBinding(ReaderCommands.GoFeedUp, OnFeedUp, OnCanExecute));
            this.CommandBindings.Add(new CommandBinding(ReaderCommands.GoFeedDown, OnFeedDown, OnCanExecute));
        }

        #endregion

        #region Window handling

        private void Header_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
                this.DragMove();
        }

        private void OnGoLeft(object sender, ExecutedRoutedEventArgs e)
        {
            var view = CollectionViewSource.GetDefaultView(DataSource);
            int currentPosition = 0;
            if (view != null)
            {
                currentPosition = view.CurrentPosition;
            }

            lstFeeds.Focus();

            if (view != null)
            {
                view.MoveCurrentToPosition(currentPosition);
                var item = lstFeeds.ItemContainerGenerator.ContainerFromIndex(currentPosition) as ListBoxItem;
                if (item != null)
                {
                    item.Focus();
                }
            }
        }

        private void OnFeedUp(object sender, ExecutedRoutedEventArgs e)
        {
            var view = CollectionViewSource.GetDefaultView(DataSource);
            int currentPosition = 0;
            if (view != null)
            {
                currentPosition = view.CurrentPosition;
            }

            lstFeeds.Focus();

            if (view != null)
            {
                view.MoveCurrentToPosition(currentPosition);
                view.MoveCurrentToPrevious();
                currentPosition = view.CurrentPosition;
                var item = lstFeeds.ItemContainerGenerator.ContainerFromIndex(currentPosition) as ListBoxItem;
                if (item != null)
                {
                    item.Focus();
                }
            }
        }

        private void OnFeedDown(object sender, ExecutedRoutedEventArgs e)
        {
            var view = CollectionViewSource.GetDefaultView(DataSource);
            int currentPosition = 0;
            if (view != null)
            {
                currentPosition = view.CurrentPosition;
            }

            lstFeeds.Focus();

            if (view != null)
            {
                view.MoveCurrentToPosition(currentPosition);
                view.MoveCurrentToNext();
                currentPosition = view.CurrentPosition;
                var item = lstFeeds.ItemContainerGenerator.ContainerFromIndex(currentPosition) as ListBoxItem;
                if (item != null)
                {
                    item.Focus();
                }
            }
        }


        private void OnGoRight(object sender, ExecutedRoutedEventArgs e)
        {
            if (!lstItems.IsFocused)
            {
                lstItems.Focus();
                lstItems.Items.MoveCurrentToFirst();
                var item = lstItems.ItemContainerGenerator.ContainerFromIndex(0) as ListBoxItem;
                if (item != null)
                {
                    item.Focus();
                }
            }
        }



        #endregion

        #region Items handling

        private void OnSetAsRead(object sender, ExecutedRoutedEventArgs e)
        {
            var selectedItems = lstItems.SelectedItems;

            if (selectedItems != null && selectedItems.Count > 0)
            {
                foreach (UnreadItem i in selectedItems)
                {
                    SetAsRead(i, FindButtonFromItemTemplate(i));
                }
            }
            else
            {
                var data = e.Parameter as UnreadItem;
                Button button = e.OriginalSource as Button;
                SetAsRead(data, button);
            }
        }

        private void OnSetAsShared(object sender, RoutedEventArgs e)
        {
            var selectedItem = lstItems.SelectedItem as UnreadItem;
            if (selectedItem != null)
            {
                AppController.CurrentReader.SetAsShared(selectedItem.Shared, selectedItem.Id, selectedItem.ParentFeedUrl, "");
                selectedItem.Shared = !selectedItem.Shared;
                setSharedIcon();
             /*   UIelements.ShareItem myShareWindow = new UIelements.ShareItem(selectedItem);
                myShareWindow.Show(); */
            }
        }

        private void OnEditTags(object sender, RoutedEventArgs e)
        {
            var selectedItem = lstItems.SelectedItem as UnreadItem;
            if (selectedItem != null)
            {
                UIelements.EditTags myTagsWindow = new UIelements.EditTags(selectedItem);
                myTagsWindow.Show();
            }
        }

        private void OnSetLiked(object sender, RoutedEventArgs e)
        {
            var selectedItem = lstItems.SelectedItem as UnreadItem;
            if (selectedItem != null)
            {
                if (AppController.CurrentReader.SetAsLiked(selectedItem.Id, selectedItem.ParentFeedUrl, selectedItem.Liked))
                {
                    selectedItem.Liked = !selectedItem.Liked;
                    if (selectedItem.Liked)
                    {
                        selectedItem.NumberOfLikingUsers++;
                    }
                    else
                    {
                        selectedItem.NumberOfLikingUsers--;
                    }
                    setLikedIcon();
                }
            }
        }

        private void OnReadItLater(object sender, RoutedEventArgs e)
        {
            var selectedItem = lstItems.SelectedItem as UnreadItem;
            if (selectedItem != null)
            {
                if (Properties.Settings.Default.RILusername != "" && Properties.Settings.Default.RILpassword != "")
                {
                    ExternalServices.ReadItLater.addToList(selectedItem);
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("Please enter your Read It Later username and password data in the preferences", "Missing login data for Read It Later", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Asterisk);
                }
            }
        }


        private void OnGoToSource(object sender, ExecutedRoutedEventArgs e)
        {
            var currentItem = lstItems.SelectedItem as UnreadItem;

            if (currentItem != null)
            {
                ReaderCommands.OpenPage(currentItem);
            }

        }

        private void buttonOnGoToSource_Clicked(object sender, RoutedEventArgs e)
        {
            var currentItem = lstItems.SelectedItem as UnreadItem;

            if (currentItem != null)
            {
                ReaderCommands.OpenPage(currentItem);
            }

        }




        #endregion

        #region Feeds handling

        private void OnSetFeedAsRead(object sender, ExecutedRoutedEventArgs e)
        {
            Button button;

            try
            {
                var data = e.Parameter as UnreadFeed;
                button = e.OriginalSource as Button;
                if (button != null)
                {
                    SetFeedAsRead(data, button);
                    return;
                }
            }
            catch (Exception exp)
            {
                LogWriter.WriteTextToLogFile(exp);
            }

            var selectedItems = lstFeeds.SelectedItems;
            if (selectedItems != null && selectedItems.Count > 0)
            {
                foreach (UnreadFeed i in selectedItems)
                {
                    SetFeedAsRead(i, FindButtonFromFeedTemplate(i));
                }
                            
            }

        }

        #endregion





        private void OnCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private Button FindButtonFromItemTemplate(UnreadItem data)
        {
            if (data != null)
            {
                ListBoxItem lbi = (ListBoxItem)lstItems.ItemContainerGenerator.ContainerFromItem(data);
                return TreeHelper.FindChildByName(lbi, "SetAsReadButton") as Button;
            }
            return null;
        }

        private Button FindButtonFromFeedTemplate(UnreadFeed data)
        {
            if (data != null)
            {
                ListBoxItem lbi = (ListBoxItem)lstFeeds.ItemContainerGenerator.ContainerFromItem(data);
                return TreeHelper.FindChildByName(lbi, "SetFeedAsReadButton") as Button;
            }
            return null;
        }

        private void SetAsSeen(UnreadItem item)
        {
            //
        }

 


        private void SetAsRead(UnreadItem item, Button button)
        {
            object currentButtonContent = null;
            if (button != null)
            {
                button.IsEnabled = false;
                currentButtonContent = button.Content;
                button.IsEnabled = false;
                button.ToolTip = "Deleting...";
               
            }

            if (item != null)
            {
                AppController.Current.updateCurrentActionInformation(true, "Marking " + item.Title + " as read...");
                var handler = new AsyncHandler<bool>(
                    (() =>
                    {
                        return AppController.CurrentReader.SetAsRead(item.Id, item.ParentFeedUrl);
                    }),
                    ((result, exception) =>
                    {
                        if (result && exception == null)
                        {

                            RefreshData(null, new Dictionary<string, string>() 
                                { 
                                    { item.Id, item.ParentFeedUrl }
                                }
                            );
                
                        }
                        else
                        {
                            if (button != null && currentButtonContent != null)
                            {
                                button.Content = currentButtonContent;
                            }
                        }
                        AppController.Current.updateCurrentActionInformation(false, "");
                    }),
                    Dispatcher);

                handler.Invoke();
            }
        }

        private void SetFeedAsRead(UnreadFeed item, Button button)
        {
            object currentButtonContent = null;
            if (button != null)
            {
                button.IsEnabled = false;
                currentButtonContent = button.Content;
                button.IsEnabled = false;
                button.ToolTip = "Deleting...";
            }

            if (item != null)
            {
                AppController.Current.updateCurrentActionInformation(true, "Marking feed as read...");
                var handler = new AsyncHandler<bool>(
                    (() =>
                    {
                        return AppController.Current.setFeedAsRead(item);
                    }),
                    ((result, exception) =>
                    {
                        AppController.Current.updateCurrentActionInformation(false, "");
                    }),
                    Dispatcher);

                handler.Invoke();
            }
        }



        public void SetData(IEnumerable<UnreadFeed> data)
        {
            if (DataSource != null)
            {
                var viewPrevious = CollectionViewSource.GetDefaultView(DataSource);
                if (viewPrevious != null)
                {
                    viewPrevious.CurrentChanged -= new EventHandler(FeedsList_CurrentChanged);
                }
            }

            
            DataSource = new ObservableCollection<UnreadFeed>(data);
            
            /*
            UnreadFeed allItems = new UnreadFeed();
            allItems.Title = "All unread";
            allItems.UnreadCount = AppController.CurrentReader.GetUnreadCount();

            foreach (UnreadItem item in AppController.CurrentReader.GetUnreadItems())
            {
                allItems.Items.Add(item);
            }
            
            
            DataSource.Insert(0,allItems);
             */
             
            lstFeeds.ItemsSource = DataSource;

            var viewNext = CollectionViewSource.GetDefaultView(DataSource);
            if (viewNext != null)
            {
                viewNext.CurrentChanged += new EventHandler(FeedsList_CurrentChanged);
            }
            
            SetTitle();
            SetUnreadTitle();
        }

        void FeedsList_CurrentChanged(object sender, EventArgs e)
        {
            ICollectionView view = sender as ICollectionView;
            if (view != null)
            {
                UnreadFeed feed = view.CurrentItem as UnreadFeed;
                if (feed != null && feed.Items!=null) 
                {
                    ICollectionView itemsView = CollectionViewSource.GetDefaultView(feed.Items);
                    if (itemsView != null)
                    {
                        itemsView.MoveCurrentToFirst();
                    }
                }
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            AppController.Current.ShutDown();
        }

        public void RefreshData(List<UnreadItem> freshItems, Dictionary<string, string> removedItems)
        {
            AppController.Current.updateCurrentActionInformation(true, "Updating items list...");
            Dispatcher.BeginInvokeMethod(() =>
            {
                if (freshItems != null)
                {
                    AppController.Current.updateCurrentActionInformation(true, "Adding new items...");
                    foreach (var item in freshItems)
                    {
                        var parentFeed = DataSource.FirstOrDefault(f => f.Url == item.ParentFeedUrl);
                        if (parentFeed == null)
                        {
                            parentFeed = new UnreadFeed()
                            {
                                Title = item.ParentFeedTitle,
                                Url = item.ParentFeedUrl
                            };
                            DataSource.Add(parentFeed);
                        }
                        parentFeed.Items.Add(item);
                        /*
                        var allItemsFeed = DataSource.FirstOrDefault(f => f.Url == "");
                        allItemsFeed.Items.Add(item); */


                    }

                    AppController.Current.updateCurrentActionInformation(false, "");
                }

                AppController.Current.updateCurrentActionInformation(false, "");

                if (removedItems != null)
                {
                    AppController.Current.updateCurrentActionInformation(true, "Removing read items...");
                    foreach (var id in removedItems)
                    {
                        var parentFeed = DataSource.FirstOrDefault(f => f.Url == id.Value);
                        if (parentFeed != null)
                        {
                            var item = parentFeed.Items.FirstOrDefault(i => i.Id == id.Key);
                            if (item != null)
                            {
                                parentFeed.Items.Remove(item);
                                if (parentFeed.Items.Count == 0)
                                {
                                    DataSource.Remove(parentFeed);
                                }
                                
                            }
                        }

                        
                        
                    }
                }
                AppController.Current.updateCurrentActionInformation(false, "");
                SetUnreadTitle();
                SetTitle();
            });
        }

        public void SetTitle()
        { 
            int count = DataSource.Sum(f => f.UnreadCount);
            string title = "Desktop Google Reader";
            if (count == 0)
            {
                Title = title;
                m_notifyIcon.Text = title;
                m_notifyIcon.Icon = new System.Drawing.Icon("Resources\\Images\\tray.ico");
                
            }
            else
            {
                this.Title = string.Format("{0} {1} - {2}", count,"unread items", title);
                m_notifyIcon.Text = string.Format("{0} {1} - {2}", count, "unread items", title);
                m_notifyIcon.Icon = new System.Drawing.Icon("Resources\\Images\\trayUnreadItems.ico");
            }
        }

        public void SetUnreadTitle()
        {
            int count = DataSource.Sum(f => f.UnreadCount);
            string title = "Unread items";
            if (count == 0)
            {
                this.nameOfCurrentView.Text = title;

            }
            else
            {
                this.nameOfCurrentView.Text = string.Format("{0} ({1})", "Unread items", count);

            }
        }



        private void buttomSubscribeNewFeed_Clicked(object sender, RoutedEventArgs e)
        {
            DesktopGoogleReader.UIelements.SubscribeToNewFeed mySubWindows = new DesktopGoogleReader.UIelements.SubscribeToNewFeed();
            mySubWindows.Show();
        }

        private void buttomUnsubscribeFeed_Clicked(object sender, RoutedEventArgs e)
        {
            if (lstFeeds.SelectedItem != null)
            {
                UnreadFeed selectedFeed = (UnreadFeed)lstFeeds.SelectedItem;
                DesktopGoogleReader.UIelements.Unsubscribe mySubWindows = new DesktopGoogleReader.UIelements.Unsubscribe(selectedFeed);
                mySubWindows.Show();
            }
        }

        private void buttonRefeshFeeds_Clicked(object sender, RoutedEventArgs e)
        {
            AppController.Current.unreadItemMonitor_RefreshNow();
        }


        private void buttonMarkItemRead_Clicked(object sender, RoutedEventArgs e)
        {
            if (lstItems.SelectedItems.Count > 0)
            {
                foreach (UnreadItem currentItem in lstItems.SelectedItems)
                {
                    SetAsRead(currentItem, null);
                }
            }
        }


        private void buttonMarkAllFeedsRead_Clicked(object sender, RoutedEventArgs e)
        {

                AppController.Current.updateCurrentActionInformation(true, "Marking all items as read...");
                var handler = new AsyncHandler<bool>(
                    (() =>
                    {
                        return AppController.Current.setAllAsRead();
                    }),
                    ((result, exception) =>
                    {
                        AppController.Current.updateCurrentActionInformation(false, "");
                    }),
                    Dispatcher);

                handler.Invoke();

        }

        public void OnSelectedItemChanged(object sender, RoutedEventArgs e)
        {
            if (lstItems.SelectedItem != null)
            {
                var currentItem = lstItems.SelectedItem as UnreadItem;
                labelCurrentItemUrl.Text = currentItem.ItemUrl;
                this._MyHost.Visibility = Visibility.Visible;
                if (Properties.Settings.Default.showFullAlways || (((currentItem.Summary == "" || currentItem.Summary == string.Empty) && Properties.Settings.Default.showFullIfEmpty) || (currentItem.Title.ToLower().Trim() == currentItem.Summary.ToLower().Trim() && Properties.Settings.Default.showFullIfSameAsTitle)))
                {
                    browser.Navigate(currentItem.ItemUrl);
                    
                }
                else
                {
                    string htmlContent = "<html><head><meta http-equiv=\"Content-Type\" content=\"text/html; charset=UTF-8\"></head><body>"
                        + currentItem.Summary
                        + "<p /><p><a style=\"border:1px solid black;padding:0.5em;background-color:#eee;float:right\" href=\""
                        + currentItem.ItemUrl
                        + "\">Read full article</a></p></body></html>";                    

                   /* string temp_file = Path.GetTempFileName();
                    FileInfo f = new FileInfo(temp_file);

                    temp_file = string.Format("{0}.html", temp_file.Substring(0, temp_file.LastIndexOf('.')));

                    if (!File.Exists(temp_file))
                        f.MoveTo(temp_file);

                    tempFilesList.Add(temp_file);

                    File.WriteAllText(temp_file, "<html><head><meta http-equiv=\"Content-Type\" content=\"text/html; charset=UTF-8\"></head><body>"
                        + currentItem.Summary
                        + "<p /><p><a style=\"border:1px solid black;padding:0.5em;background-color:#eee;float:right\" href=\""
                        + currentItem.ItemUrl
                        + "\">Read full article</a></p></body></html>");

                    browser.Navigate(new Uri(temp_file).AbsoluteUri); */
                    browser.DocumentText = htmlContent;
                    labelCurrentItemUrl.Text = "";

                    setLikedIcon();
                    setSharedIcon();
                }
                

                  
            }
            else
            {
                labelCurrentItemUrl.Text = "";
                this._MyHost.Visibility = Visibility.Hidden;
                // xxx show loading page
            }

            
        }


        private void setLikedIcon()
        {
            var selectedItem = lstItems.SelectedItem as UnreadItem;

            if (selectedItem != null)
            {                
                if (selectedItem.Liked)
                {
                    LikedState.Source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(Properties.Resources.heart.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                    if (selectedItem.NumberOfLikingUsers == 1)
                    {
                        LikedState.ToolTip = "You like this item (click to remove)";
                    }
                    else if (selectedItem.NumberOfLikingUsers > 1)
                    {
                        LikedState.ToolTip = selectedItem.NumberOfLikingUsers.ToString() + " are liking this item (including you)";
                    }

                }
                else
                {
                    LikedState.Source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(Properties.Resources.heartGrey.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                    if (selectedItem.NumberOfLikingUsers == 0)
                    {
                        LikedState.ToolTip = "Mark this item as liked";
                    }
                    else
                    {
                        LikedState.ToolTip = selectedItem.NumberOfLikingUsers.ToString() + " user are liking this item - click to also mark it as liked";
                    }

                }
            }
        }

        private void setSharedIcon()
        {
            var selectedItem = lstItems.SelectedItem as UnreadItem;

            if (selectedItem != null)
            {
                if (selectedItem.Shared)
                {
                    ShareIcon.Source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(Properties.Resources.share.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                    ShareIcon.ToolTip = "You share this item - click to stop sharing";
                }
                else
                {
                    ShareIcon.Source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(Properties.Resources.notShared.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                    ShareIcon.ToolTip = "Click to share this item";

                }
            }
        }


  
        public void setCurrentActionState(bool isWorkInProgress)
        {
            if (isWorkInProgress)
            {
                currentProgress.Visibility = Visibility.Visible;
                
            }
            else
            {
                currentProgress.Visibility = Visibility.Hidden;
            }
        }


        private void buttonCloseMe_Click(object sender, RoutedEventArgs e)
        {
            m_notifyIcon.Dispose();
            m_notifyIcon = null;
            foreach (var f in tempFilesList)
            {
                try
                {
                    File.Delete(f);
                }
                catch (Exception) { }
            }
            AppController.Current.ShutDown();
        }

        private void buttonMinimizeMe_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState != WindowState.Minimized)
            {
                this.WindowState = WindowState.Minimized;
                if (Properties.Settings.Default.minimizeToTray)
                {
                    Hide();
                }
            }
        }

        private void buttonMaximizeMe_Click(object sender, RoutedEventArgs e)
        {
            if(this.WindowState == WindowState.Maximized) {
                this.WindowState = WindowState.Normal;
            }
            else if (this.WindowState == WindowState.Normal)
            {
                this.WindowState = WindowState.Maximized;
            }
        }

        private void HeaderMouseClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                buttonMaximizeMe_Click(null, null);
            }
        }

        private void ChromiumDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount >= 2)
            {
                return;
            }
        }

        private void KeyPressedInUrlBox(object sender, KeyEventArgs e)
        {
            if (e.IsDown && e.Key == Key.Return)
            {
                if(!labelCurrentItemUrl.Text.StartsWith("http://") && !labelCurrentItemUrl.Text.StartsWith("https://"))
                {
                    labelCurrentItemUrl.Text = "http://" + labelCurrentItemUrl.Text;
                }
                browser.Navigate(labelCurrentItemUrl.Text);
            }
        }

        private void buttonPreferences_Clicked(object sender, RoutedEventArgs e)
        {
            UIelements.Preferences myPreferences = new DesktopGoogleReader.UIelements.Preferences();
            myPreferences.Show();
        }

        private void buttonHelp_Clicked(object sender, RoutedEventArgs e)
        {
            UIelements.About myAboutBox = new UIelements.About();
            myAboutBox.Show();
        }

        #region Browser Controls

        private void buttonBrowserBack_Clicked(object sender, RoutedEventArgs e)
        {
            browser.GoBack();
        }

        private void buttonBrowserForward_Clicked(object sender, RoutedEventArgs e)
        {
            browser.GoForward();
        }

        #endregion

        void OnStateChanged(object sender, EventArgs args)
        {
            if (WindowState != WindowState.Minimized)
            {
                m_storedWindowState = WindowState;
            }

            
        }
        void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            CheckTrayIcon();
        }

        void m_notifyIcon_Click(object sender, EventArgs e)
        {
            Show();
            WindowState = m_storedWindowState;
        }
        void CheckTrayIcon()
        {
            ShowTrayIcon(!IsVisible);
        }

        void ShowTrayIcon(bool show)
        {
            if (m_notifyIcon != null)
                m_notifyIcon.Visible = show;
        }
        #region WebKit


        #endregion

        private void TweetItButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = lstItems.SelectedItem as UnreadItem;

            if (selectedItem != null)
            {
                ExternalServices.TweetIt myTweetWindow = new ExternalServices.TweetIt(selectedItem);
                myTweetWindow.Show();
            }
        }

        private void InstapaperButton_Click(object sender, RoutedEventArgs e)
        {
            
                var selectedItem = lstItems.SelectedItem as UnreadItem;
                if (selectedItem != null)
                {
                    if (Properties.Settings.Default.Instapaper_username != "")
                    {
                        ExternalServices.Instapaper.addToList(selectedItem);
                    }
                    else
                    {
                        System.Windows.Forms.MessageBox.Show("Please enter your Instapaper username in the preferences", "Missing login data for Instapaper", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Asterisk);
                    }
                }
            

        }

        private void DeliciousButton_Click(object sender, RoutedEventArgs e)
        {
                var selectedItem = lstItems.SelectedItem as UnreadItem;
                if (selectedItem != null)
                {
                    if (Properties.Settings.Default.deliciousPassword != "" && Properties.Settings.Default.deliciousUser != "")
                    {
                        ExternalServices.Delicious.add(selectedItem);
                    }
                    else
                    {
                        System.Windows.Forms.MessageBox.Show("Please enter your del.icio.us credentials in the preferences", "Missing login data for Instapaper", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Asterisk);
                    }
                }
        }

        private void OnMailSend(object sender, RoutedEventArgs e)
        {
            var selectedItem = lstItems.SelectedItem as UnreadItem;
            if (selectedItem != null)
            {
                UIelements.MailItem myMailWindow = new DesktopGoogleReader.UIelements.MailItem(selectedItem);
                myMailWindow.Show();
            }
        }

        private void FacebookButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = lstItems.SelectedItem as UnreadItem;
            if (selectedItem != null)
            {
                ExternalServices.FacebookSend myFacebook = new DesktopGoogleReader.ExternalServices.FacebookSend();
                myFacebook.Send(selectedItem.Title + ": " + selectedItem.ItemUrl);
            }
        }

        private void PosterousButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = lstItems.SelectedItem as UnreadItem;
            if (selectedItem != null)
            {
                if (Properties.Settings.Default.posterousPassword != "" && Properties.Settings.Default.posterousUser != "")
                {
                    ExternalServices.Posterous.add(selectedItem);
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("Please enter your Posterous credentials in the preferences", "Missing login data for Posterous", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Asterisk);
                }
            }
        }

        private void DiigoButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = lstItems.SelectedItem as UnreadItem;
            if (selectedItem != null)
            {
                if (Properties.Settings.Default.diigoPassword != "" && Properties.Settings.Default.deliciousUser != "")
                {
                    ExternalServices.Diigo.add(selectedItem);
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("Please enter your Diigo credentials in the preferences", "Missing login data for Diigo", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Asterisk);
                }
            }
        }

        private void ShowFullPageButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = lstItems.SelectedItem as UnreadItem;
            if (selectedItem != null)
            {
                browser.Navigate(selectedItem.ItemUrl);
            }
        }

 
    }
}
