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
                                MessagesBox.Text += channel.receiveMessages(ref messageIndex);
                                //MessagesBox.Focus();
                                MessagesBox.ScrollToEnd();
                            });
                        }
                        catch (Exception) { };
                    }
                    Thread.Sleep(delay);
                    if (token.IsCancellationRequested)
                        break;
                }

                // cleanup, e.g. close connection
            }, token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            channel.sendMessage(YourMessage.Text,YourName.Text==""?null:YourName.Text);
        }

        private void ButtonConnect_Click(object sender, RoutedEventArgs e)
        {
            
            ChannelFactory<IChat> fabrik = new ChannelFactory<IChat>(
                new WSHttpBinding(SecurityMode.None),
                "http://"+ip.Text+":2310/Chat");
            channel = fabrik.CreateChannel();
            status.Content = "Connected";
            connected = true;
            
        }
    }

}
