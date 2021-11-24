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
        void sendMessage(string message);
        [OperationContract]
        string receiveMessages(ref int id);
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static IChat channel;
        public static int message_id = 0;

        public MainWindow()
        {
            InitializeComponent();


            //---Code teil von Stackoverlow
            int delay = 10; //in ms
            var cancellationTokenSource = new CancellationTokenSource();
            var token = cancellationTokenSource.Token;
            var listener = Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    if (channel == null) continue;
                    try
                    {
                        this.Dispatcher.Invoke(() =>
                        {
                            tb_AllMessages.Text += channel.receiveMessages(ref message_id);
                            //MessagesBox.Focus();
                            tb_AllMessages.ScrollToEnd();
                        });
                    }
                    catch (Exception e) {
                        Console.WriteLine(e);

                    };
                    
                    Thread.Sleep(delay);
                    if (token.IsCancellationRequested)
                        break;
                }

                // cleanup, e.g. close connection
            }, token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            //---ende Stackoverflow
            return;
        }

        private void bt_send_Click(object sender, RoutedEventArgs e)
        {
            channel.sendMessage(tb_Message.Text);
        }

        private void bt_connect_click(object sender, RoutedEventArgs e)
        {

            ChannelFactory<IChat> fabrik = new ChannelFactory<IChat>(
                new WSHttpBinding(SecurityMode.None),
                "http://"+tb_ip.Text+":2310/Chat");

            channel = fabrik.CreateChannel();
            lb_status.Content = "Connected";

            channel.sendMessage("User connected");

        }
    }

}
