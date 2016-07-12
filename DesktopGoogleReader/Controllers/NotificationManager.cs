namespace DesktopGoogleReader
{
    using System;
    using System.Windows.Threading;
    using WinForms = System.Windows.Forms;
    using System.Text.RegularExpressions;
    using Snarl;
    using System.Net;
    using System.Drawing;

    public class NotificationManager
    {
        private NewUnreadItemMonitor monitor;
        private Dispatcher dispatcher;
        private bool notificationActive;
        private bool notificationStopped;
        private string pathToIcon = "";
        private NativeWindowApplication.SnarlMsgWnd snarlMsgWindow;
        private bool initialRun = true;

        public NotificationManager(NewUnreadItemMonitor monitor, Dispatcher dispatcher)
        {
            this.monitor = monitor;
            this.dispatcher = dispatcher;
            this.pathToIcon = pathToIcon = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\Resources\\images\\feed.png";
            snarlMsgWindow = new NativeWindowApplication.SnarlMsgWnd();
            if(SnarlConnector.GetSnarlWindow() != IntPtr.Zero) {
                SnarlConnector.RegisterConfig(snarlMsgWindow.Handle, "Desktop Google Reader", Snarl.WindowsMessage.WM_USER + 55,pathToIcon);
                SnarlConnector.RegisterAlert("Desktop Google Reader", "New entry");
                SnarlConnector.RegisterAlert("Desktop Google Reader", "Number of new items");
            }
        }

        public void unreadItemMonitor_SetNewPollingInterval(int intervalInSeconds)
        {
            monitor.setNewPollingInterval(intervalInSeconds);
        }

        public void unreadItemMonitor_RefreshNow()
        {
            monitor.refreshNow();
        }

        public void shutdownNotificationManager() {
            if(SnarlConnector.GetSnarlWindow() != IntPtr.Zero) {
                SnarlConnector.RevokeConfig(snarlMsgWindow.Handle);
            }
        }

        public void StartNotification()
        {
            if (!Properties.Settings.Default.NotificationsEnabled)
            {
                return;
            }
            if (initialRun)
            {
                initialRun = false;
                monitor.FreshItems.Clear();
                return;
            }
                if (!notificationActive)
                {
                    if (monitor != null && monitor.FreshItems.Count > 0)
                    {
                        if (SnarlConnector.GetSnarlWindow() != IntPtr.Zero)
                        {
                            string oneOrMoreItems = "item";
                            if (monitor.FreshItems.Count > 2)
                            {
                                oneOrMoreItems = "items";
                            }
                            Int32 snarlMsgId = SnarlConnector.ShowMessageEx("Number of new items", monitor.FreshItems.Count.ToString() + " new Google Reader " + oneOrMoreItems, "You have " + monitor.FreshItems.Count.ToString() + " new news " + oneOrMoreItems + " in your Google Reader subscriptions", 15, pathToIcon, snarlMsgWindow.Handle, Snarl.WindowsMessage.WM_USER + 46, "");
                        }
                    }
                    ShowNextNotification();
                }   
        }

        public void ResumeNotification()
        {
            notificationStopped = false;
        }

        public void StopNotification()
        {
            notificationStopped = true;
            notificationActive = false;
        }

        private void ShowNextNotification()
        {
            if (notificationStopped || !Properties.Settings.Default.NotificationsEnabled)
            {
                return;
            }

            dispatcher.InvokeMethod(() =>
                {
                    if (monitor != null && monitor.FreshItems.Count > 0)
                    {
                        var item = monitor.FreshItems.Dequeue();

                        if (SnarlConnector.GetSnarlWindow() != IntPtr.Zero)
                        {
                            var handler = new AsyncHandler<bool>(
                            (() =>
                            {
                                bool deleteTempImage = false;
                                notificationActive = false;
                                string displayTitle = "";
                                string displayText = "";
                                string htmlFreeSummary = Regex.Replace(item.Summary, "<.*?>", string.Empty);
                                htmlFreeSummary = Regex.Replace(htmlFreeSummary, "\n{2,}", "\n");

                                switch (Properties.Settings.Default.notificationTitle)
                                {
                                    case "Feed item title":
                                        displayTitle = item.Title;
                                        break;

                                    default:
                                        displayTitle = item.ParentFeedTitle;
                                        break;
                                }

                                switch (Properties.Settings.Default.notificationText) {
                                    case "Feed item content":                       
                                        displayText = htmlFreeSummary;
                                        break;
                                
                                    case "Feed item title":
                                        displayText = item.Title;
                                        break;

                                    case "Feed name":
                                        displayText = item.ParentFeedTitle;
                                        break;
                                
                                    case "Nothing":
                                        displayText = "";
                                        break;

                                    default:
                                        // default behavoiur as fallback
                                        displayText = item.Title + "\n\n" + htmlFreeSummary;
                                        break;
                                }
                                
                                int maximumTextLength = 200;
                                maximumTextLength = AppController.Current.getCurrentMaximumNotificationLength();
                                if (displayText.Length > maximumTextLength)
                                {
                                    displayText = displayText.Substring(0, maximumTextLength-3) + "...";
                                }


                                string localPathToIcon = pathToIcon;
                                string tempPath = "";
                                if (item.Enclosure != string.Empty && item.EnclosureType.ToLower().StartsWith("image") && Properties.Settings.Default.notificationUseAttachmentIcon)
                                {
                                    tempPath = item.Enclosure;
                                }
                                if (tempPath == "" && Properties.Settings.Default.notificationUseContentIcon)
                                {
                                    tempPath = searchImageInHtml(item.Summary);
                                    if (tempPath != "")
                                    {
                                        deleteTempImage = true;
                                    }
                                }
                                if (tempPath != "") {
                                    localPathToIcon = tempPath;
                                }


                                Int32 snarlMsgId = SnarlConnector.ShowMessageEx("New entry", displayTitle, displayText, 15, localPathToIcon, snarlMsgWindow.Handle, Snarl.WindowsMessage.WM_USER + 45, "");
                                item.SnarlNotificationId = snarlMsgId;
                                snarlMsgWindow.memorizeNotificatedItem(item);

                                if(deleteTempImage) {
                                    try
                                    {
                                        System.IO.File.Delete(tempPath);
                                    }
                                    catch (Exception ex)
                                    {
                                        LogWriter.WriteTextToLogFile(ex);
                                    }
                                }
                                return true;
                            }),
                            ((result, exception) =>
                            {
                                 // something on true...  
                            }),
                            dispatcher);

                            handler.Invoke();

                            ShowNextNotification();
                        }
                        else
                        {
                            notificationActive = true;
                            var area = WinForms.Screen.PrimaryScreen.WorkingArea;
                            int x = area.Right - 400;
                            int y = area.Bottom - 150;

                            var nw = new NotificationWindow();
                            nw.DataContext = item;
                            nw.Closed += new EventHandler(nw_Closed);
                            nw.Show(x, y);
                        }
                    }
                    else {
                        notificationActive = false;
                    }
                });
            
        }

        private string searchImageInHtml(string htmlSourceCode)
        {


            htmlSourceCode = htmlSourceCode.Replace("\"", "");
            string pattern = @"\<img\s*src=(?<imageurl>http\S*)";
            Regex images = new Regex(pattern, RegexOptions.IgnoreCase);
            MatchCollection allImages = images.Matches(htmlSourceCode);
            if (allImages.Count > 0)
            {
                foreach (Match image in allImages)
                {
                    string currentImageUrl = image.Groups["imageurl"].Value;
                    string tempFile = System.IO.Path.GetTempFileName();
                    WebClient client = new WebClient();
                    try
                    {
                        client.DownloadFile(currentImageUrl, tempFile);
                        Image ImageOrig = Image.FromFile(tempFile);
                        // Let's test if the image is not a very small one (like blind GIFs) and
                        // is more or less square
                        if (Properties.Settings.Default.iconMinHeight <= ImageOrig.Height && 
                            Properties.Settings.Default.iconMinWidth <= ImageOrig.Width &&
                            ImageOrig.Height / ImageOrig.Width <= Properties.Settings.Default.iconMaxStretchFaktor &&
                            ImageOrig.Width / ImageOrig.Height <= Properties.Settings.Default.iconMaxStretchFaktor
                            )
                        {
                            return tempFile;
                        }
                        
                    }
                    catch (Exception ex)
                    {
                        LogWriter.WriteTextToLogFile(ex);
                    }
                }
                return allImages[0].Groups["imageurl"].Value;
            }
            return "";
        }



        void nw_Closed(object sender, EventArgs e)
        {
            NotificationWindow nw = sender as NotificationWindow;
            if (nw != null)
            {
                nw.Closed -= new EventHandler(nw_Closed);
            }
            ShowNextNotification();
        }
    }
}
