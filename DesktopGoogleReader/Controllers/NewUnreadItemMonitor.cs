namespace DesktopGoogleReader
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Timers;
    using System.Windows.Threading;
    using GoogleReaderAPI;
    using GoogleReaderAPI.DataContracts;

    public class NewUnreadItemMonitor
    {
        private IReader reader;
        private Dictionary<string, string> currentData = new Dictionary<string, string>();
        private Timer timer;
        private Dispatcher dispather;
        

        private int pollInterval = 180000;

        public NewUnreadItemMonitor(IReader reader, IEnumerable<UnreadFeed> initialData, Dispatcher dispather) 
        {
            this.reader = reader;
            foreach (var f in initialData) 
            {
                foreach (var i in f.Items) 
                {
                    currentData.Add(i.Id, i.ParentFeedUrl);
                }
            }
            this.dispather = dispather;
            FreshItems = new Queue<UnreadItem>();
            timer = new Timer();
            timer.Interval = pollInterval;
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            StoreFreshItems = true;
        }

        public void setNewPollingInterval(int intervalInSeconds)
        {
            StopMonitoring();
            this.pollInterval = intervalInSeconds * 1000;
            timer.Interval = this.pollInterval;
            StartMonitoring();
        }

        public void refreshNow()
        {
            ElapsedEventArgs dummy;
            dummy = null;
            StopMonitoring();
            this.timer_Elapsed(new object(),dummy);
            StartMonitoring();
        }


        public void StartMonitoring() 
        {
            timer.Enabled = true;
        }

        public void StopMonitoring() 
        {
            timer.Enabled = false;
        }

        private bool timerProcessing = false;


        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                if (timerProcessing)
                    return;
                timerProcessing = true;

                AppController.Current.updateCurrentActionInformation(true,"Updating feeds...");
                var oldData = new Dictionary<string, string>(currentData);
                var freshItems = reader.GetNewUnreadItems(oldData).ToList();

                dispather.InvokeMethod(() =>
                {
                    foreach (var i in freshItems)
                    {
                        currentData.Add(i.Id, i.ParentFeedUrl);
                        if (StoreFreshItems)
                        {
                            FreshItems.Enqueue(i);
                        }
                    }

                    foreach (var id in oldData)
                    {
                        currentData.Remove(id.Key);
                    }
                });

                if (freshItems.Count > 0 || oldData.Count > 0)
                {
                    OnDataChanged(freshItems, oldData);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                AppController.Current.updateCurrentActionInformation(false,"");
                timerProcessing = false;
            }
        }

        public Queue<UnreadItem> FreshItems { get; set; }
        
        public bool StoreFreshItems { get; set; }

        protected void OnDataChanged(List<UnreadItem> freshItems, Dictionary<string, string> removedItemIds)
        {
            if (DataChanged != null) 
            {
                DataChanged(this, new UnreadItemCollectionChangedEventArgs() 
                { 
                    FreshItems = freshItems,
                    RemovedItemIds = removedItemIds 
                });
            }
        }

        public event EventHandler<UnreadItemCollectionChangedEventArgs> DataChanged;

        public class UnreadItemCollectionChangedEventArgs : EventArgs 
        {
            public Dictionary<string, string> RemovedItemIds { get; set; }
            public List<UnreadItem> FreshItems { get; set; }
        }
    }
}
