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
        public static void SendMessage(EncodedMessage message, string hostname = "localhost", int port = 9999)
        {
            byte[] payload = SerializeMessage(message);

            try
            {
                using TcpClient client = new(hostname, port);
                using NetworkStream stream = client.GetStream();
                using BinaryWriter writer = new(stream);

                Console.WriteLine($"[Client] Connected to {hostname}");

                // Write message length first (4 bytes)
                writer.Write(payload.Length);
                // Then write the actual message
                writer.Write(payload);
                writer.Flush();
            }
            catch (SocketException ex)
            {
                Console.Error.WriteLine($"[SocketException] Connection error: {ex.Message}");
            }
            catch (IOException ex)
            {
                Console.Error.WriteLine($"[IOException] Error on writing to network: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[Exception] Unexpected error on receiving: {ex.Message}");
            }
        }

        private static byte[] SerializeMessage(EncodedMessage message)
        {
            using MemoryStream ms = new();
            using BinaryWriter writer = new(ms);

            message.Serialize(writer);
            return ms.ToArray();
        }


        public static EncodedMessage? ReceiveMessage(int port = 9999)
        {
            try
            {
                TcpListener listener = new(IPAddress.Any, port);
                listener.Start();
                Console.WriteLine($"[Server] Listening on port {port}...");

                using TcpClient client = listener.AcceptTcpClient();
                using NetworkStream stream = client.GetStream();
                Console.WriteLine($"[Server] Client connected");
                using BinaryReader reader = new(stream);

                // Read message length
                int length = reader.ReadInt32();

                if (length <= 0 || length > 10_000_000)  // 10 MB
                    throw new InvalidDataException($"(Network.ReceiveMessage) Invalid length: {length}");

                // Then the actual message
                byte[] buffer = reader.ReadBytes(length);

                if (buffer.Length != length)
                    throw new EndOfStreamException($"(Network.ReceiveMessage) Different length: {buffer.Length} (should be {length})");

                return DeserializeMessage(buffer);
            }
            catch (IOException ex)
            {
                Console.Error.WriteLine($"[IOException] Error on reading from network: {ex.Message}");
            }
            catch (InvalidDataException ex)
            {
                Console.Error.WriteLine($"[InvalidData] {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[Exception] Unexpected error on sending: {ex.Message}");
            }

            return null; 
        }

        private static EncodedMessage DeserializeMessage(byte[] data)
        {
            using MemoryStream ms = new(data);
            using BinaryReader reader = new(ms);
            EncodedMessage message = new();

            message.Deserialize(reader);
            return message;
        }

    }
}
