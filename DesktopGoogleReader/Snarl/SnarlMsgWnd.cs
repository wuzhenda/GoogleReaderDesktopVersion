using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using GoogleReaderAPI.DataContracts;
using System.Linq;
using Snarl;

namespace NativeWindowApplication
{

    // Summary description for SnarlMsgWnd.
    [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
    
    
    public class SnarlMsgWnd : NativeWindow
    {
        CreateParams cp = new CreateParams();

        public string pathToIcon = "";
        
        private List<GoogleReaderAPI.DataContracts.UnreadItem> notifiedItems = new List<GoogleReaderAPI.DataContracts.UnreadItem>();

        int SNARL_GLOBAL_MESSAGE;

        public SnarlMsgWnd()
        {
            // Create the actual window
            this.CreateHandle(cp);
            this.SNARL_GLOBAL_MESSAGE = Snarl.SnarlConnector.GetGlobalMsg();
            pathToIcon = System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "\\Resources\\images\\feed.png";
        }

        public void memorizeNotificatedItem(UnreadItem item)
        {
            notifiedItems.Add(item);
        }

        public IEnumerable<UnreadItem> FindItemsIoSnarlId(Int32 snarlId)
        {
            return from item in notifiedItems
                   where item.SnarlNotificationId == snarlId
                   select item;
        }

        public void markAsRead(UnreadItem item)
        {
            DesktopGoogleReader.AppController.CurrentReader.SetAsRead(item.Id, item.ParentFeedUrl);

        }


        protected override void WndProc(ref Message m)
        {

        if (m.Msg == this.SNARL_GLOBAL_MESSAGE)
        {
            if ((int)m.WParam == Snarl.SnarlConnector.SNARL_LAUNCHED)
            {
                // Snarl has been (re)started 
                SnarlConnector.GetSnarlWindow(true);
                SnarlConnector.RegisterConfig(this.Handle, "Desktop Google Reader", Snarl.WindowsMessage.WM_USER + 55,pathToIcon);
                SnarlConnector.RegisterAlert("Desktop Google Reader", "New entry");
                SnarlConnector.RegisterAlert("Desktop Google Reader", "Number of new items");
            }
        }
        else if (m.Msg == (int)Snarl.WindowsMessage.WM_USER + 45)
        {
            // single news item
            if ((int)m.WParam == Snarl.SnarlConnector.SNARL_NOTIFICATION_ACK)
            {
                IEnumerable<UnreadItem> clickedItems = FindItemsIoSnarlId((Int32)m.LParam);
                if (clickedItems != null)
                {
                    foreach (UnreadItem clickedItem in clickedItems)
                    {
                        DesktopGoogleReader.ReaderCommands.OpenPage(clickedItem);
                        SnarlConnector.HideMessage(clickedItem.SnarlNotificationId);
                        notifiedItems.Remove(clickedItem);
                        DesktopGoogleReader.AppController.CurrentReader.SetAsRead(clickedItem.Id, clickedItem.ParentFeedUrl);
                     
                        break;
                    }
                }
            }
            else if ((int)m.WParam == Snarl.SnarlConnector.SNARL_NOTIFICATION_CLICKED)
            {
                IEnumerable<UnreadItem> clickedItems = FindItemsIoSnarlId((Int32)m.LParam);
                if (clickedItems != null)
                {
                    foreach (UnreadItem clickedItem in clickedItems)
                    {
                        SnarlConnector.HideMessage(clickedItem.SnarlNotificationId);
                        notifiedItems.Remove(clickedItem);
                        DesktopGoogleReader.AppController.CurrentReader.SetAsRead(clickedItem.Id, clickedItem.ParentFeedUrl);
                        DesktopGoogleReader.AppController.Current.RefreshDataInGui(null, new Dictionary<string, string>()
                            { 
                                { clickedItem.Id, clickedItem.ParentFeedUrl }
                            }
                         );
                   
                        break;
                    }
                }
            }
            else if (m.Msg == (int)Snarl.WindowsMessage.WM_USER + 46)
            {
                // number of new items
                if ((int)m.WParam == Snarl.SnarlConnector.SNARL_NOTIFICATION_ACK || (int)m.WParam == Snarl.SnarlConnector.SNARL_NOTIFICATION_CLICKED)
                {
                    
                }
            }
            else if (
    (int)m.WParam == Snarl.SnarlConnector.SNARL_NOTIFICATION_TIMED_OUT ||
    (int)m.WParam == Snarl.SnarlConnector.SNARL_NOTIFICATION_CANCELLED ||
    (int)m.WParam == Snarl.SnarlConnector.SNARL_NOTIFICATION_CLOSED ||
    (int)m.WParam == Snarl.SnarlConnector.SNARL_NOTIFICATION_MIDDLE_MOUSE
    )
            {
                IEnumerable<UnreadItem> clickedItems = FindItemsIoSnarlId((Int32)m.LParam);
                if (clickedItems != null)
                {
                    foreach (UnreadItem clickedItem in clickedItems)
                    {
                        notifiedItems.Remove(clickedItem);
                        break;
                    }
                }
            }
        }
            base.WndProc(ref m);

        }


    }

}
