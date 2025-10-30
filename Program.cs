using System;
using static HuffmanDemo.CliParser;
using static HuffmanDemo.Huffman;
using static HuffmanDemo.NetworkHelper;

namespace HuffmanDemo
{
    internal class Program
    {
        static int Main(string[] args)
        {
            try
            {
                var result = Parse(args);
                Run(result);
                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine($"[Error] {e.Message}");
                return 1;
            }
        }

        static void Run(ParseResult result)
        {
            switch (result.Mode)
            {
                case Mode.SendFile:
                case Mode.SendMessage:

                    var encoded = Encode(result.Content);
                    if (result.Verbose)
                        ShowEncodingDetails(result.Content, encoded);

                    SendMessage(encoded, result.Target, result.Port);

                    break;

                case Mode.Listen:
                    var received = ReceiveMessage(result.Port)
                        ?? throw new Exception("No message received.");

                    var decoded = Decode(received);
                    if (result.Verbose)
                        ShowEncodingDetails(decoded, received);

                    Console.WriteLine($"""
                        -----BEGIN MESSAGE-----
                        {decoded}
                        -----END MESSAGE-----
                        """);
                    break;

                case Mode.Help:
                    break;
            }
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