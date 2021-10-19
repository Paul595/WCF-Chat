using System;
using System.ServiceModel;
using System.ServiceModel.Discovery;
using System.ServiceModel.Description;
using System.Collections.Generic;

namespace WCFService
{
    [ServiceContract]
    public interface IChat
    {
        [OperationContract]
        string whichServiceAmI();
        [OperationContract]
        void sendMessage(string message, string sender="");
        [OperationContract]
        string receiveMessages(ref int id,bool myService =false);
    }
    public class Chat : IChat 
    {
        static List<Message> messages = new List<Message>();
   
        public string whichServiceAmI()
        {
            return "KatzensteinerMiedl";
        }

        public void sendMessage(string message, string sender="")
        {
            messages.Add(new Message(sender, message));
        }

        public string receiveMessages(ref int id,bool mySerivce = false)
        {

            string reString = "";
            
            for(int i = id + 1; i <messages.Count; i++)
            {
                if (mySerivce) reString += messages[i].sender + ": " + messages[i].data + "\n";
                else reString += messages[i].data + "\n";
            }
            id = messages.Count-1;
            return reString;
        }

    }
    class Message
    {
        public string sender { get; set; }
        public string data { get; set; }
        public Message(string Sender, string Data)
        {
            sender = Sender;
            data = Data;
        }

    }
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starte WebService ...");

            string ip="";
            int i = 0;

            do
            {
                 ip = System.Net.Dns.GetHostAddresses(
                     Environment.MachineName)[i++].ToString();
                
            } while (ip.Contains(":"));
            Console.WriteLine("IP: " + ip);
            ServiceHost meinHost = new ServiceHost(typeof(Chat));
            
            meinHost.AddServiceEndpoint(typeof(IChat),
                new WSHttpBinding(SecurityMode.None),
                "http://"+ip+":2310/Chat");

            //meinHost.Description.Behaviors.Add(
            //    new ServiceDiscoveryBehavior());
            //meinHost.Description.Endpoints.Add(
            //    new UdpDiscoveryEndpoint());

            meinHost.Open();

            //Console.WriteLine("läuft!");
            Console.Write("Press Enter to quit:");

            Console.ReadLine();

            meinHost.Close();
        }
    }
}
