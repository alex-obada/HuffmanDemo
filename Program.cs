using System;
using static HuffmanDemo.Huffman;

namespace HuffmanDemo
{
    internal class Program
    {
        static void Main(string[] args)
        {
       
           

            var message = "ana are mere";

            message = Console.ReadLine();

            var encodedMessage = Huffman.Encode(message);
            (List<bool> encoded, var symbols) = encodedMessage;




            int originalSize = 8 * message.Length;
            int compressedSize = encoded.Count;

            Console.WriteLine($"original: {originalSize}");
            Console.WriteLine($"compresat: {compressedSize}");
            Console.WriteLine($"ratio: {100.0 * compressedSize / originalSize}");

            Console.WriteLine();


            Console.WriteLine();
            Console.WriteLine(encodedMessage.ToString());

            Stream inputStream = new MemoryStream();
            BinaryWriter writer = new(inputStream);
            encodedMessage.Serialize(writer);

            //writer.Flush();

            // Write memory stream to file
            inputStream.Position = 0; // Rewind to beginning
            using (FileStream fileStream = File.Create("encoded_message.bin"))
            {
                inputStream.CopyTo(fileStream);
            }

            
            using (FileStream fileStream = File.OpenRead("encoded_message.bin"))
            using (BinaryReader reader = new(fileStream))
                encodedMessage.Deserialize(reader);

            var decoded = Huffman.Decode(encoded, symbols);

            Console.WriteLine(decoded);

            Console.WriteLine($"original nr of bytes: {message.Length}");


        }
    }





}
/*
 * ana are mere
011110011100100101101111100010
011110011100100101101111100010  
*/