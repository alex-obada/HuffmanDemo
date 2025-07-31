using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using static HuffmanDemo.Huffman;

namespace HuffmanDemo
{
    internal static class NetworkHelper
    {
        public static void SendMessage(EncodedMessage encodedMessage, string hostname, int port = 9999)
        {
            Stream messageStream = new MemoryStream();
            BinaryWriter writer = new(messageStream);
            encodedMessage.Serialize(writer);

            using TcpClient client = new TcpClient(hostname, port);
            using NetworkStream networkStream = client.GetStream();

            messageStream.Position = 0;
            messageStream.CopyTo(networkStream);
            networkStream.Flush();
        }

        public static EncodedMessage ReceiveMessage(int port = 9999)
        {
            TcpListener listener = new TcpListener(IPAddress.Any, port);
            listener.Start();

            using TcpClient client = listener.AcceptTcpClient();
            using NetworkStream networkStream = client.GetStream();

            MemoryStream receivedData = new MemoryStream();
            networkStream.CopyTo(receivedData);
            receivedData.Position = 0;

            listener.Stop();

            EncodedMessage message = new();
            using (BinaryReader reader = new(receivedData))
                message.Deserialize(reader);
            return message;
        }
    }
}
