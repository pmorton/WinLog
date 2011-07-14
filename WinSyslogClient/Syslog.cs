// syslog.cs
//
// This code illustrates how to generate a syslog message from C#. 
//
// To test it, you need a syslog server.  There is a freeware syslog server
// for Win32 available from Kiwi, http://www.kiwisyslog.com/ . 
// Or, you can use syslogd on any Unix. 
// 
// (c) Ionic Shade
// Tue, 02 Nov 2004  09:29
//


using System;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace WinSyslogClient
{
    public enum Level
    {
        Emergency = 0,
        Alert = 1,
        Critical = 2,
        Error = 3,
        Warning = 4,
        Notice = 5,
        Information = 6,
        Debug = 7,
    }


    public enum Facility
    {
        Kernel = 0,
        User = 1,
        Mail = 2,
        Daemon = 3,
        Auth = 4,
        Syslog = 5,
        Lpr = 6,
        News = 7,
        UUCP = 8,
        Cron = 9,
        Local0 = 10,
        Local1 = 11,
        Local2 = 12,
        Local3 = 13,
        Local4 = 14,
        Local5 = 15,
        Local6 = 16,
        Local7 = 17,
    }


    public class Message
    {
        private int _facility;
        public int Facility
        {
            get { return _facility; }
            set { _facility = value; }
        }
        private int _level;
        public int Level
        {
            get { return _level; }
            set { _level = value; }
        }
        private string _text;
        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }
        private string _hostname;
        public string hostName
        {
            get { return _hostname; }
            set { _hostname = value; }
        }
        private string _source;
        public string source
        {
            get { return _source; }
            set { _source = value; }
        }
        private string _id;
        public string id
        {
            get { return _id; }
            set { _id = value; }
        }
        private DateTime _timestamp;
        public DateTime timestamp
        {
            get { return _timestamp; }
            set { _timestamp = value; }
        }
        public Message() { }
        public Message(int facility, int level, string text)
        {
            _facility = facility;
            _level = level;
            _text = text;
        }
    }


    /// need this helper class to expose the Active propery of UdpClient
    /// (why is it protected, anyway?) 
    public class Helper : System.Net.Sockets.UdpClient
    {
        public Helper() : base() { }
        public Helper(IPEndPoint ipe) : base(ipe) { }
        ~Helper()
        {
            if (this.Active) this.Close();
        }

        public bool IsActive
        {
            get { return this.Active; }
        }
    }


    public class Client
    {

        private IPHostEntry ipHostInfo;
        private IPAddress ipAddress;
        private IPEndPoint ipLocalEndPoint;
        private WinSyslogClient.Helper helper;
        public Client()
        {
            ipHostInfo = Dns.Resolve(Dns.GetHostName());
            ipAddress = ipHostInfo.AddressList[0];
            ipLocalEndPoint = new IPEndPoint(ipAddress, 0);
            helper = new WinSyslogClient.Helper(ipLocalEndPoint);
        }

        public bool IsActive
        {
            get { return helper.IsActive; }
        }

        public void Close()
        {
            if (helper.IsActive) helper.Close();
        }

        private int _port = 514;
        public int Port
        {
            set { _port = value; }
            get { return _port; }
        }

        private string _hostIp = null;
        public string HostIp
        {
            get { return _hostIp; }
            set
            {
                if ((_hostIp == null) && (!IsActive))
                {
                    _hostIp = value;
                    //helper.Connect(_hostIp, _port);
                }
            }
        }

        public void Send(WinSyslogClient.Message message)
        {
            if (!helper.IsActive)
                helper.Connect(_hostIp, _port);
            if (helper.IsActive)
            {
                //<Facility/Priority>TimeStamp HostName [Source] Message
                string timestamp = message.timestamp.ToUniversalTime().ToString("MMM dd HH:mm:ss");
                string msg = System.String.Format("<{0}>{1} {2} {3}[{4}]: {5}",
                                 message.Facility * 8 + message.Level, timestamp,message.hostName,message.source.Replace(" ","") , message.id, message.Text);

                byte[] bytes = System.Text.Encoding.ASCII.GetBytes(msg);
                helper.Send(bytes, bytes.Length);

                StreamWriter sw = File.AppendText("c:\\Events2.log");
		        sw.WriteLine(msg);
		        sw.Close();

            }
            else throw new Exception("Syslog client Socket is not connected. Please set the host IP");
        }
    }


}