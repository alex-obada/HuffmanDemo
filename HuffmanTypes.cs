using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace HuffmanDemo
{
    internal static partial class Huffman
    {
        public class EncodedMessage : IBinarySerializable
        {
            public required List<bool> Message { get; set; }
            public required IEnumerable<SymbolType> SymbolTable { get; set; }

            public void Deconstruct(out List<bool> encoded, out IEnumerable<SymbolType> symbolTable)
            {
                encoded = Message;
                symbolTable = SymbolTable;
            }

            public void Serialize(BinaryWriter writer)
            {
                BitHelper.WriteBoolList(writer, Message);

                var symbols = SymbolTable.ToList();
                writer.Write(symbols.Count);
                foreach (var symbol in symbols)
                    symbol.Serialize(writer);
            }

            public void Deserialize(BinaryReader reader)
            {
                Message = BitHelper.ReadBoolList(reader);

                int symbolCount = reader.ReadInt32();
                var symbols = new List<SymbolType>(symbolCount);
                for (int i = 0; i < symbolCount; i++)
                {
                    var symbol = new SymbolType();
                    symbol.Deserialize(reader);
                    symbols.Add(symbol);
                }
                SymbolTable = symbols;
            }

            public override string ToString()
            {
                StringBuilder stringBuilder = new();

                stringBuilder.AppendLine($"Encoded Message ({Message.Count} bits):");
                foreach (var bit in Message)
                    stringBuilder.Append(bit ? "1" : "0");
                stringBuilder.AppendLine();

                stringBuilder.AppendLine($"Symbol Table ({SymbolTable.Count()}):");
                foreach (var symbol in SymbolTable)
                    stringBuilder.AppendLine($"    {symbol.ToString()}");

                return stringBuilder.ToString();
            }
        }

        private abstract class Node
        {
            public abstract long Count { get; }
            public Node? Left { get; set; } = null;
            public Node? Right { get; set; } = null;
        }

        public class SymbolType : IBinarySerializable
        {
            public char Symbol { get; set; }
            public long Count { get; set; } = 0;
            public List<bool> Encoding { get; set; } = [];

            public void Deserialize(BinaryReader reader)
            {
                Symbol = reader.ReadChar();
                Count = reader.ReadInt64();

                Encoding = BitHelper.ReadBoolList(reader);
            }

            public void Serialize(BinaryWriter writer)
            {
                writer.Write(Symbol);
                writer.Write(Count);

                BitHelper.WriteBoolList(writer, Encoding);
            }

            public override string ToString()
            {
                StringBuilder stringBuilder = new();
                // ' ' | 0x | [  ]
                if (!char.IsControl(Symbol) && !char.IsWhiteSpace(Symbol))
                    stringBuilder.Append(Symbol);
                else
                    stringBuilder.Append(' ');

                stringBuilder.Append(" | ");
                stringBuilder.Append($"0x{ ((int)Symbol).ToString("X2") }");
                stringBuilder.Append(" | ");

                foreach (bool bit in Encoding)
                    stringBuilder.Append(bit ? '1' : '0');

                return stringBuilder.ToString();
            }
        }

        private class SymbolNode : Node
        {
            public required SymbolType Symbol { get; set; }

            public override long Count => Symbol.Count;
        }

        private class InternalNode : Node
        {
            public InternalNode(Node left, Node right)
            {
                Left = left;
                Right = right;
                _count = left.Count + right.Count;

                if (_count < 1)
                    throw new OverflowException("_count overflow in Huffman.InternalNode.InternalNode()");
            }

            public override long Count => _count;

            private readonly long _count;
        }
    }
}
