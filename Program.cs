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

            //message = Console.ReadLine();

            var encodedMessage = Huffman.Encode(message);
            (List<bool> encoded, var symbols) = encodedMessage;

            int originalSize = 8 * message.Length;
            int compressedSize = encoded.Count;

            Console.WriteLine($"original: {originalSize}");
            Console.WriteLine($"compresat: {compressedSize}");
            Console.WriteLine($"ratio: {100.0 * compressedSize / originalSize}");

            NetworkHelper.SendMessage(encodedMessage, "localhost");
            
            Console.WriteLine();

            Console.WriteLine(encodedMessage.ToString());

            

            var decoded = Huffman.Decode(encoded, symbols);

            Console.WriteLine(decoded);

            Console.WriteLine($"\noriginal nr of bytes: {message.Length}");

        }
    }





}
/*
 * ana are mere
011110011100100101101111100010
011110011100100101101111100010  
*/