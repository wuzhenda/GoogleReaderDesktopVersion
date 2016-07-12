using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Facebook;
using Facebook.BindingHelper;
using Facebook.Session;
using Facebook.Schema;
using Facebook.Rest;

namespace DesktopGoogleReader.ExternalServices
{
    class FacebookSend
    {
        private string appId = "117655798268396";
        DesktopSession session;
        public static BindingManager FacebookService { get; private set; }

        public FacebookSend() {
            session = new DesktopSession(appId, null, null, true, new List<Enums.ExtendedPermissions>() {Enums.ExtendedPermissions.publish_stream });
            session.Login();
        }

        public bool Send(string Message)
        {
            var service = BindingManager.CreateInstance(session);
            service.UpdateStatus(Message);
            return true;
        }
        
    }
}
