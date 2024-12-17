using GameObjects;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
using UDPLib;
using System.Diagnostics;

namespace SeaBattleGame
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        Action<string> action;
        private MainViewModel vm;
        private Dictionary<string, ShipType> dc;
        private String[] ships = { "Alaska", "Arabella", "Emma", "Ghost", "Pharaoh" };
        private Client client;
        int idShip;

        public MainWindow()
        {
            InitializeComponent();
            action = OpenGame;
            dc = new Dictionary<string, ShipType>();
            dc.Add("Ship1", 0);
            dc.Add("Ship2", 0);

            Ship1ComboBox.ItemsSource = ships;
        }


        private void newGameButton_Click(object sender, RoutedEventArgs e)
        {
            string ip = textBoxIpAddress.Text;
            int localPort = int.Parse(textBoxLocalPort.Text);
            int remotePort = int.Parse(textBoxRemotePort.Text);
            dc["Ship1"]= (ShipType)Ship1ComboBox.SelectedIndex;
            client = new Client(ip, remotePort, localPort, (int)dc["Ship1"]);
            client.Notify += OpenGameDispatcher;
            labelReceive.Content = "Waiting for connection...";
        }

        public void OpenGameDispatcher(string message)
        {
            Dispatcher.Invoke(action, message);
        }


        public void OpenGame(string message)
        {
            labelReceive.Content = message;
            if (message == "Connect")
            {
                client.ClearNotify();
                if (App.Current.Windows.OfType<ShipType>().Count() == 0 && dc != null)
                {
                    labelShips.Content = ships[(int)dc["Ship1"]] + " и " + ships[(int)dc["Ship2"]];
                    try
                    {
                        vm = new MainViewModel
                        {
                            Content = new Renderer(dc, idShip, client)
                        };
                        DataContext = vm;
                    }
                    catch
                    {

                    }
                }
            }
            else
            {
                idShip = int.Parse(message.Split('|')[1]);
                int typeShip = int.Parse(message.Split('|')[0]);
                dc["Ship2"] = (ShipType)typeShip;
            }

        }

        private void buttonLocalPortUp_Click(object sender, RoutedEventArgs e)
        {
            int port = int.Parse(textBoxLocalPort.Text);

            if (port < 10000 && port > 0)
                textBoxLocalPort.Text = (port + 1).ToString();
        }

        private void buttonLocalPortDown_Click(object sender, RoutedEventArgs e)
        {
            int port = int.Parse(textBoxLocalPort.Text);

            if (port < 10000 && port > 0)
                textBoxLocalPort.Text = (port - 1).ToString();
        }

        private void buttonRemotePortUp_Click(object sender, RoutedEventArgs e)
        {
            int port = int.Parse(textBoxRemotePort.Text);

            if (port < 10000 && port > 0)
                textBoxRemotePort.Text = (port + 1).ToString();
        }

        private void buttonRemotePortDown_Click(object sender, RoutedEventArgs e)
        {
            int port = int.Parse(textBoxRemotePort.Text);

            if (port < 10000 && port > 0)
                textBoxRemotePort.Text = (port - 1).ToString();
        }

        private void mainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (client != null)
            {
                client.CloseClient();
            }
        }

        private void buttonServer_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("GameServer.exe");
        }
    }
}
