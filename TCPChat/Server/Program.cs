using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server;

class Program
{
    static void Main(string[] args)
    {
        RunServer();
    }
    
    static void RunServer()
    {
        try
        {
            ServerWorker();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    static void ServerWorker()
    {
        IPAddress ip = IPAddress.Parse("127.0.0.1");
        IPEndPoint ipep = new IPEndPoint(ip, 8000);
        using Socket listener = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        listener.Bind(ipep);
        listener.Listen(10);

        while (true)
        {
            Console.WriteLine("Waiting for a connection...");
            using Socket handler = listener.Accept();
            Console.WriteLine("Client connected!");
            byte[] receiveBuffer = new byte[1024];
            int bytesCount = handler.Receive(receiveBuffer);
            string receiveString = Encoding.UTF8.GetString(receiveBuffer, 0, bytesCount);
            
            Console.WriteLine(receiveString);
            string response = "";
            if (receiveString.Contains("Hello"))
            {
                response = "Hi from server! ))";
            }
            else if (receiveString.Contains("Menu"))
            {
                response = "Commands list: \n1. Date - Current date.\n2. Time - Current time.\n3. Random - Random number from 1 to 10.\n4. IP - IP address of server." +
                           "\n5. Reverse (your text) - Reverses text.\n6. Palindrome (your text) - Check if text is a palendrome?";
            }
            else if (receiveString.Contains("Date"))
            {
                response = DateTime.Now.ToString("yyyy-MM-dd");
            }
            else if (receiveString.Contains("Time"))
            {
                response = DateTime.Now.ToString("HH:mm:ss");
            }
            else if (receiveString.Contains("Random"))
            {
                response = new Random().Next(1, 11).ToString();
            }
            else if (receiveString.Contains("IP"))
            {
                response = ip.ToString();
            }
            else if (receiveString.StartsWith("Reverse "))
            {
                string textReverse = receiveString.Substring(8);
                response = new string(textReverse.Reverse().ToArray());
            }
            else if (receiveString.StartsWith("Palindrome "))
            {
                string textCheck = receiveString.Substring(11).Replace(" ", "").ToLower();
                string reversedText = new string(textCheck.Reverse().ToArray());
                response = textCheck == reversedText ? "yes" : "no";
            }
            else
            {
                response = $"length of your message: {receiveString.Length}";
            }
            
            byte[] sendBuffer = Encoding.UTF8.GetBytes(response);
            handler.Send(sendBuffer);
            handler.Shutdown(SocketShutdown.Both);
            handler.Close();
            if (receiveString.Contains("Stop"))
                break;
        }
        Console.WriteLine("Server stopped");
    }
}