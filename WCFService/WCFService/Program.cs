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
        void sendMessage(string message);
        [OperationContract]
        string receiveMessages(ref int id);
    }
    public class Chat : IChat 
    {
        static List<string> Nachrichten = new List<string>();

        public string whichServiceAmI()
        {
            return "BosichStein";
        }

        public void sendMessage(string message)
        {
            Nachrichten.Add(message);

        }

        public string receiveMessages(ref int id)
        {

            string value = "";
            
            for(int i = id + 1; i <Nachrichten.Count; i++)
            {
                value += Nachrichten[i] + "\n";
            }
            id = Nachrichten.Count-1;
            return value;
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

            meinHost.Description.Behaviors.Add(
                new ServiceDiscoveryBehavior());
            meinHost.Description.Endpoints.Add(
                new UdpDiscoveryEndpoint());

            meinHost.Open();

            Console.WriteLine("läuft!");
            Console.Write("Press Enter to quit:");

            Console.ReadLine();

            meinHost.Close();
        }
    }
}
