using System;
using static HuffmanDemo.Huffman;

namespace HuffmanDemo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            char sr = Console.ReadLine()[0];

            if (sr == 's')
                SendData();
            else
                ReceiveData();
        }

        private static void ReceiveData()
        {
            EncodedMessage encoded = NetworkHelper.ReceiveMessage();
            string messaje = Huffman.Decode(encoded);
            Console.WriteLine(messaje);
        }

        private static void SendData()
        {
            var message = "ana are mere";

            message = Console.ReadLine();

            var em = Huffman.Encode(message);
            //ShowEncodingDetails(message, em);

            NetworkHelper.SendMessage(em, "localhost");

            Console.WriteLine($"[Client] Message sent");

        }

        private static void ShowEncodingDetails(string message, EncodedMessage em)
        {
            int originalSize = message.Length;

            int compressedSize = 4 + (em.Message.Count + 7) / 8 + // encoded 
                                 4 + em.SymbolTable.Count() * (1 + 4 + 1); // symbols (~= 1 byte per encoding)

            Console.WriteLine($"\noriginal: {originalSize} bytes");
            Console.WriteLine($"compressed: {compressedSize} bytes");
            Console.WriteLine($"ratio: {100.0 * compressedSize / originalSize}\n");

            Console.WriteLine(em.ToString());
        }
    }





}
/*
 * ana are mere
011110011100100101101111100010
011110011100100101101111100010  
*/