using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Client;

class Program
{
    static void Main(string[] args)
    {
        RunClient();
    }

    static void RunClient()
    {
        try
        {
            while (ClientWorker()) {}
            Console.WriteLine("Client Stopped!");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    static bool ClientWorker()
    {
        IPAddress ip = IPAddress.Loopback;
        IPEndPoint ipep = new IPEndPoint(ip, 8000);
        using Socket sender = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        sender.Connect(ipep);
        Console.WriteLine($"You connected to {sender.RemoteEndPoint}!");

        Console.WriteLine("Enter you message:");
        string message = Console.ReadLine();

        byte[] sandingBytes = Encoding.UTF8.GetBytes(message);
        sender.Send(sandingBytes);
        byte[] recvBytes = new byte[1024];
        int bytesReceived = sender.Receive(recvBytes);
        string response = Encoding.UTF8.GetString(recvBytes, 0, bytesReceived);
        Console.WriteLine(response);

        sender.Shutdown(SocketShutdown.Both);
        sender.Close();

        return !message.Contains("Stop");
    }
}

