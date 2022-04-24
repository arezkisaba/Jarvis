using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace MiLight.Lib
{
    public class WifiController
    {
        private static readonly ManualResetEvent ConnectDone = new ManualResetEvent(false);
        private static readonly ManualResetEvent SendDone = new ManualResetEvent(false);
        private readonly string _ip;
        private readonly int _port;

        public WifiController(string ip, int port = 8899)
        {
            _ip = ip;
            _port = port;
        }
        
        public void Send(byte[] command)
        {
            var sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            var serverAddr = IPAddress.Parse(_ip);

            var endPoint = new IPEndPoint(serverAddr, _port);

            sock.BeginConnect(endPoint, ConnectCallback, sock);
            ConnectDone.WaitOne();

            sock.BeginSend(command, 0, command.Length, 0, SendCallback, sock);

            SendDone.WaitOne();
            //sock.SendTo(command, endPoint);

            Thread.Sleep(100);
        }

        private static void ConnectCallback(IAsyncResult ar)
        {
            var client = (Socket) ar.AsyncState;
            client.EndConnect(ar);
            ConnectDone.Set();
        }

        private static void SendCallback(IAsyncResult ar)
        {
            var client = (Socket) ar.AsyncState;
            client.EndSend(ar);
            SendDone.Set();
        }
    }
}