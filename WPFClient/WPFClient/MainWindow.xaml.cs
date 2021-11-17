using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ServiceModel;
using System.ServiceModel.Discovery;
using System.ServiceModel.Description;
using System.Threading;
using System.ComponentModel;

namespace WPFClient
{
    [ServiceContract]
    public interface IChat
    {
        [OperationContract]
        string whichServiceAmI();
        [OperationContract]
        void sendMessage(string message, string sender = "");
        [OperationContract]
        string receiveMessages(ref int id, bool myService = false);
        [OperationContract]
        int getUserCount();
        [OperationContract]
        void join();
        [OperationContract]
        void leave();
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static IChat channel;
        public static int messageIndex = 0;
        public static bool connected = false;
        public MainWindow()
        {
            InitializeComponent();

            int delay = 50; //in ms
            var cancellationTokenSource = new CancellationTokenSource();
            var token = cancellationTokenSource.Token;
            var listener = Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    if (connected)
                    {
                        try
                        {
                            this.Dispatcher.Invoke(() =>
                            {
                                MessagesBox.Text += channel.receiveMessages(ref messageIndex,true);
                                userCount.Content = "PremiumUserCount: "+channel.getUserCount();
                                //MessagesBox.Focus();
                                MessagesBox.ScrollToEnd();
                            });
                        }
                        catch (Exception e) {

                            disconnectAll();
                            MessagesBox.Text = "Ein Fehler ist aufgetreten:" + e;
                        };
                    }
                    Thread.Sleep(delay);
                    if (token.IsCancellationRequested)
                        break;
                }

                // cleanup, e.g. close connection
            }, token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            return;
        }

        private void Button_Click(object sender = null, RoutedEventArgs e = null)
        {
            if (YourMessage.Text == "") return;
            string sys_name = System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString().Split('\\')[1];


            string n = YourName.Text == "" ?  sys_name: YourName.Text;
            try { 
            channel.sendMessage(YourMessage.Text,n);
            }
            catch (Exception e1)
            {

                disconnectAll();
                MessagesBox.Text = "Ein Fehler ist aufgetreten:" + e1;
            };
            YourMessage.Text = "";
            return;
        }
        //private void Window_Closing(object sender, CancelEventArgs e)
        //{
            
        //}

        private void disconnectAll()
        {
            var converter2 = new System.Windows.Media.BrushConverter();
            var brush2 = (Brush)converter2.ConvertFromString("#FFFF0000");
            status.Foreground = brush2;
            btConnect.Content = "Connect";
            try { 
            channel.leave();
            channel = null;
            }
            catch (Exception e)
            {

                disconnectAll();
                MessagesBox.Text = "Ein Fehler ist aufgetreten:" + e;
            };
            connected = false;
            messageIndex = 0;
            MessagesBox.Text = "";
            userCount.Content = "PremiumUserCount: 0";
            return;
        }

        private void ButtonConnect_Click(object sender, RoutedEventArgs e)
        {
            if (connected)
            {
                disconnectAll();   
                return;
            }
                

            string sys_name = System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString().Split('\\')[1];
            string n = YourName.Text == "" ? sys_name : YourName.Text;


            try { 
            ChannelFactory<IChat> fabrik = new ChannelFactory<IChat>(
                new WSHttpBinding(SecurityMode.None),
                "http://"+ip.Text+":2310/Chat");
            channel = fabrik.CreateChannel();
            }
            catch (Exception e1)
            {

                disconnectAll();
                MessagesBox.Text = "Ein Fehler ist aufgetreten:" + e1;
            };
            status.Content = "Connected";
            var converter = new System.Windows.Media.BrushConverter();
            var brush = (Brush)converter.ConvertFromString("#FF08B800");
            status.Foreground = brush;
            btConnect.Content = "Disconnect";
            connected = true;
            userCount.Content = "PremiumUserCount: 0";

            try { 
            channel.join();
            }
            catch (Exception e1)
            {

                disconnectAll();
                MessagesBox.Text = "Ein Fehler ist aufgetreten:" + e1;
            };

            return;
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            // ... Test for F5 key.
            if (e.Key == Key.Enter)
            {
                Button_Click();
            }
        }
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            try { 
            if(connected)channel.leave();
            }
            catch (Exception e1)
            {

                disconnectAll();
                MessagesBox.Text = "Ein Fehler ist aufgetreten:" + e1;
            };
        }   

    }

}
