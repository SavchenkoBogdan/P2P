using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading;

namespace ChatBackend
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class ChatBackend : IChatBackend
    {

        #region Everything we need to receive messages

        DisplayMessageDelegate _displayMessageDelegate = null;
        UpdateCanvasDelegate _updateCanvasDelegate = null;

        public ChatBackend(DisplayMessageDelegate dmd, UpdateCanvasDelegate ucd)
        {
            _displayMessageDelegate = dmd;
            _updateCanvasDelegate = ucd;
            StartService();
        }

        /// <summary>
        /// This method gets called by our friends when they want to display a message on our screen.
        /// We're really only returning a string for demonstration purposes … it might be cleaner
        /// to return void and also make this a one-way communication channel.
        /// </summary>
        /// <param name="composite"></param>
        public void DisplayMessage(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            if (_displayMessageDelegate != null)
            {
                _displayMessageDelegate(composite);
            }
        }

        public void UpdateCanvas(ImageType data)
        {
            if (_updateCanvasDelegate != null && data.Username != _myUserName)
                _updateCanvasDelegate.Invoke(data);
        }

        #endregion // Everything we need to receive messages

        #region Everything we need for bi-directional communication

        private string _myUserName = "Anonymous";
        private ServiceHost host = null;
        private ChannelFactory<IChatBackend> channelFactory = null;
        private IChatBackend _channel;

        /// <summary>
        /// The front-end calls the SendMessage method in order to broadcast a message to our friends
        /// </summary>
        /// <param name="text"></param>
        public void SendMessage(string text)
        {
            if (text.StartsWith("setname:", StringComparison.OrdinalIgnoreCase))
            {
                _myUserName = text.Substring("setname:".Length).Trim();
                _displayMessageDelegate(new CompositeType("Event", "Setting your name to " + _myUserName));
            }
            else
            {
                _channel.DisplayMessage(new CompositeType(_myUserName, text));
            }
        }

        public void SendMessage(Command cmd)
        {
            _channel.UpdateCanvas(new ImageType(_myUserName, cmd));
        }

        private void StartService()
        {
            host = new ServiceHost(this);
            host.Open();

            Thread.Sleep(1000);
            _myUserName += new Random().Next(0, 100000000);
            var a = 5f;
            channelFactory = new ChannelFactory<IChatBackend>("ChatEndpoint");
            _channel = channelFactory.CreateChannel();
            _channel.DisplayMessage(new CompositeType("Event", _myUserName + " has entered the conversation."));
            _displayMessageDelegate(new CompositeType("Info", "To change your name, type setname: NEW_NAME"));
        }

        public void StopService()
        {
            if (host != null)
            {
                _channel.DisplayMessage(new CompositeType("Event", _myUserName + " is leaving the conversation."));
                if (host.State != CommunicationState.Closed)
                {
                    channelFactory.Close();
                    host.Close();
                }
            }
        }


        #endregion // Everything we need for bi-directional communication

    }
}
