using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ChatBackend
{
    [ServiceContract]
    public interface IChatBackend
    {
        [OperationContract(IsOneWay = true)]
        void DisplayMessage(CompositeType composite);
        [OperationContract(IsOneWay = true)]
        void UpdateCanvas(ImageType data);

        void SendMessage(string text);
    }

    [DataContract]
    public class ImageType
    {
        private Command _command;
        private string _username = "Anonymous";

        public ImageType() { }
        public ImageType(string user, Command cmd)
        {
            _command = cmd;
            _username = user;
        }

        [DataMember]
        public Command Comand
        {
            get { return _command;}
            set { _command = value; }
        }
        [DataMember]
        public string Username
        {
            get { return _username; }
            set { _username = value; }
        }
    }

    [DataContract]
    public class CompositeType
    {
        private string _username = "Anonymous";
        private string _message = "";

        public CompositeType() { }
        public CompositeType(string u, string m)
        {
            _username = u;
            _message = m;
        }

        [DataMember]
        public string Username
        {
            get { return _username; }
            set { _username = value; }
        }

        [DataMember]
        public string Message
        {
            get { return _message; }
            set { _message = value; }
        }
    }

    public delegate void DisplayMessageDelegate(CompositeType data);
    public delegate void UpdateCanvasDelegate(ImageType data);
}
