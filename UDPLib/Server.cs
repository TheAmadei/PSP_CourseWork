using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace UDPLib
{
    public class Server
    {
        Queue<byte[]> queue = new Queue<byte[]>();
        UdpClient server;
        private int localPort; // local port
        private bool appClose;
        List<IPEndPoint> clients;
        List<int> typeOfShips;

        public delegate void ReceiveHandler(string message);
        public event ReceiveHandler Notify;

        public Server(int localPort)
        {
            try
            {
                this.localPort = localPort;
                appClose = false;
                server = new UdpClient(localPort);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void SendData(IPEndPoint client, string message)
        {
            try
            {
                byte[] data = Encoding.Unicode.GetBytes(message);
                server.Send(data, data.Length, client);
            }
            catch (Exception ex)
            {
                Notify(ex.Message);
            }
        }

        public void RunServer()
        {
            RunRecieve();
        }

        private void RunRecieve()
        {
            Thread receiveThread = new Thread(new ThreadStart(ReceiveDataConnect));
            receiveThread.Start();
        }

        public void ReceiveDataConnect()
        {
            clients = new List<IPEndPoint>();
            typeOfShips = new List<int>();
            IPEndPoint remoteIp = null; // адрес входящего подключения
            try
            {
                while (!appClose && clients.Count < 2)
                {
                    byte[] data = server.Receive(ref remoteIp); // получаем данные
                    string message = Encoding.Unicode.GetString(data);
                    clients.Add(remoteIp);
                    typeOfShips.Add(int.Parse(message));

                    Notify?.Invoke($"{remoteIp.Address}:{remoteIp.Port} - Ready to connect!");
                }
                Notify?.Invoke($"Play!");

                SendData(clients[0], typeOfShips[1].ToString() + "|1");
                SendData(clients[1], typeOfShips[0].ToString() + "|2");
                SendData(clients[0], "Connect");
                SendData(clients[1], "Connect");
                RunPlayLogic();
            }
            catch (Exception ex)
            {
                Notify?.Invoke(ex.Message);
            }
        }

        void RunPlayLogic()
        {
            Thread receiveThread = new Thread(new ThreadStart(PlayLogicReceive));
            receiveThread.Start();
            Thread sendThread = new Thread(new ThreadStart(PlayLogicSend));
            sendThread.Start();
        }

        void PlayLogicReceive()
        {
            IPEndPoint remoteIp = null; // адрес входящего подключения
            try
            {
                while (!appClose)
                {
                    byte[] data = server.Receive(ref remoteIp); // получаем данные
                    Array.Resize(ref data, data.Length + 1);
                    if (clients[0].Address.Equals(remoteIp.Address))
                        data[data.Length - 1] = 1;
                    else
                        data[data.Length - 1] = 0;
                    queue.Enqueue(data);
                }
            }
            catch (Exception ex)
            {
                Notify?.Invoke(ex.Message);
            }
        }

        public string GetIpAddress()
        {
            string Host = Dns.GetHostName();
            return Dns.GetHostByName(Host).AddressList[0].ToString();
        }

        void PlayLogicSend()
        {
            IPEndPoint remoteIp = null; // адрес входящего подключения
            try
            {
                byte[] data;
                while (!appClose)
                {
                    if (queue.Count != 0)
                    {
                        data = queue.Dequeue();
                        server.Send(data, data.Length - 1, clients[data[data.Length - 1]]);
                    }
                }
            }
            catch (Exception ex)
            {
                Notify?.Invoke(ex.Message);
            }
        }

        public void Close()
        {
            if (server != null)
                server.Close();
            appClose = true;
        }

        public void ClearNotify()
        {
            Notify = null;
        }
    }
}
