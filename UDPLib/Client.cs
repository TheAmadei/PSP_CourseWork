using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace UDPLib
{
    public class Client
    {
        public int isEnd = 0;
        EndPoint remoteEndPoint;
        private int localPort; // local port
        bool appQuit;
        Socket socket;
        int idShip;
        public ShipData MyShip, EnemyShip;

        public delegate void ReceiveHandler(string message);
        public event ReceiveHandler Notify;
        public Client(string ip, int remotePort, int localPort, int idShip)
        {
            try
            {
                this.localPort = localPort;
                remoteEndPoint = new IPEndPoint(IPAddress.Parse(ip), remotePort);
                appQuit = false;
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                IPEndPoint localIP = new IPEndPoint(IPAddress.Any, localPort);
                socket.Bind(localIP);
                this.idShip = idShip;
                RunConnect();
                MyShip = new ShipData();
                EnemyShip = new ShipData();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void RunConnect()
        {
            Thread receiveThread = new Thread(new ThreadStart(Connect));
            receiveThread.Start();
        }

        public void Connect()
        {
            Thread.Sleep(100);
            try
            {
                byte[] data = Encoding.Unicode.GetBytes(idShip.ToString());
                socket.SendTo(data, remoteEndPoint);
                ReceiveData();
                ReceiveData();
                RunGameLogic();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void ReceiveData()
        {
            try
            {
                byte[] data = new byte[1024 * 8]; // получаем данные
                int bytes = socket.Receive(data);
                string message = Encoding.Unicode.GetString(data, 0, bytes);

                Notify?.Invoke(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void RunGameLogic()
        {
            Thread receiveThread = new Thread(new ThreadStart(ShipDataRecieve));
            receiveThread.Start();
            Thread sendThread = new Thread(new ThreadStart(ShipDataSend));
            sendThread.Start();
        }

        public void ShipDataRecieve()
        {
            IPEndPoint remoteIp = null; // адрес входящего подключения

            while (!appQuit)
            {
                byte[] data = new byte[1024 * 8]; // получаем данные
                int bytes = socket.Receive(data); // получаем данные

                EnemyShip.x = BitConverter.ToInt32(data, 0);
                EnemyShip.y = BitConverter.ToInt32(data, 4);
                EnemyShip.dircetion = BitConverter.ToInt32(data, 8);
                EnemyShip.bullet = BitConverter.ToInt32(data, 12);
                EnemyShip.mode = BitConverter.ToInt32(data, 16);
                isEnd = BitConverter.ToInt32(data, 20);
            }

        }

        public void ShipDataSend()
        {
            try
            {
                while (!appQuit)
                {
                    byte[] data = BitConverter.GetBytes(MyShip.x)
                                  .Concat(BitConverter.GetBytes(MyShip.y))
                                  .Concat(BitConverter.GetBytes(MyShip.dircetion))
                                  .Concat(BitConverter.GetBytes(MyShip.bullet))
                                  .Concat(BitConverter.GetBytes(MyShip.mode))
                                  .Concat(BitConverter.GetBytes(isEnd))
                                  .ToArray();
                    if (MyShip.bullet == 1)
                    {
                        MyShip.bullet = 0;
                    }
                    if (MyShip.mode == 1)
                    {
                        MyShip.mode = 0;
                    }
                    socket.SendTo(data, 0, data.Length - 1, SocketFlags.None, remoteEndPoint);
                    Thread.Sleep(30);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void CloseClient()
        {
            if (socket != null)
                socket.Close();
            appQuit = true;
        }

        public void ClearNotify()
        {
            Notify = null;
        }
    }
}
