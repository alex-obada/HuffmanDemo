
using HuffmanDemo;

var message = "ana are mere";
(List<bool> encoded, var symbols) = Huffman.Encode(message);

foreach (var bit in encoded)
    Console.Write(bit ? '1' : '0');
Console.WriteLine();

var decoded = Huffman.Decode(encoded, symbols);

Console.WriteLine(decoded);

/*
011110011100100101101111100010
011110011100100101101111100010
*/