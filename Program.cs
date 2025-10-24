using System;
using static HuffmanDemo.CliParser;
using static HuffmanDemo.Huffman;
using static HuffmanDemo.NetworkHelper;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HuffmanDemo
{
    internal class Program
    {
        static int Main(string[] args)
        {
            var result = Parse(args);

            switch (result.Mode)
            {
                case Mode.SendFile:
                case Mode.SendMessage:

                    var encoded = Encode(result.Content);
                    if(result.Verbose == true)
                        ShowEncodingDetails(result.Content, encoded);

                    SendMessage(encoded, result.Target, result.Port);

                    break;

                case Mode.Listen:
                    var received = ReceiveMessage(result.Port);

                    var decoded = Decode(received); 
                    if (result.Verbose == true)
                        ShowEncodingDetails(decoded, received);

                    Console.WriteLine("-----BEGIN MESSAGE----");
                    Console.WriteLine(decoded);
                    Console.WriteLine("-----END MESSAGE-----");

                    break;

                default:
                    return 1;
            }
            return 0;
        }

        private static void ShowEncodingDetails(string message, EncodedMessage em)
        {
            Console.WriteLine(em.ToString());

            int originalSize = message.Length;

            int compressedSize = 4 + (em.Message.Count + 7) / 8 + // encoded 
                                 4 + em.SymbolTable.Count() * (1 + 4 + 1); // symbols (~= 1 byte per encoding)

            Console.WriteLine($"\noriginal: {originalSize} bytes");
            Console.WriteLine($"compressed: {compressedSize} bytes");
            Console.WriteLine($"ratio: {100.0 * compressedSize / originalSize}\n");
        }
    }
}