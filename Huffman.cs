using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace HuffmanDemo
{
    internal static class Huffman
    {
        public static (List<bool> encoded, IEnumerable<SymbolType> symbolTable) Encode(string message)
        {
            if (string.IsNullOrEmpty(message) || message.Length == 1)
                throw new ArgumentException("message is null, empty or invalid in Huffman.Encode()");

            Dictionary<char, SymbolType> symbolDict = [];

            foreach (char ch in message)
            {
                if (symbolDict.TryGetValue(ch, out var symbol))
                    symbol.Count++;
                else
                    symbolDict[ch] = new SymbolType { Count = 1, Symbol = ch };
            }

            Node root = BuildTree(symbolDict.Values);

            List<bool> encoding = [];
            EncodeLeafs(root, encoding);

            //encoding.Clear(); // redundant
            foreach (char ch in message)
                encoding.AddRange(symbolDict[ch].Encoding);

            return (encoding, symbolDict.Values);
        }

        private static Node BuildTree(IEnumerable<SymbolType> symbols)
        {
            var heap = new PriorityQueue<Node, long>();

            foreach (SymbolType symbol in symbols)
            {
                if (symbol.Count < 1)
                    throw new OverflowException("symbolType.Count overflow in Huffman.BuildTree()");

                heap.Enqueue(new SymbolNode() { Symbol = symbol }, symbol.Count);
            }

            while (heap.Count > 1)
            {
                var left = heap.Dequeue();
                var right = heap.Dequeue();
                var newElem = new InternalNode(left, right);

                heap.Enqueue(newElem, newElem.Count);
            }

            return heap.Dequeue();
        }

        private static void EncodeLeafs(Node? root, List<bool> currentEncoding)
        {
            if (root is null)
                return;

            if (root.Left is null && root.Right is null)
            {
                if (root is not SymbolNode)
                    throw new InvalidDataException("leaf is not SymbolNode in Huffman.EncodeLeafs()");

                var leaf = (SymbolNode)root;
                leaf.Symbol.Encoding = new List<bool>(currentEncoding);

                return;
            }

            currentEncoding.Add(false);
            EncodeLeafs(root.Left, currentEncoding);

            currentEncoding[^1] = true;
            EncodeLeafs(root.Right, currentEncoding);

            currentEncoding.RemoveAt(currentEncoding.Count - 1);
        }

        public static string Decode(List<bool> encoded, IEnumerable<SymbolType> symbolTable)
        {

            if (encoded is null || encoded.Count < 2)
                throw new ArgumentException("bit list is null or invalid in Huffman.Decode()");
            if (symbolTable is null || symbolTable.Any(symbol => symbol.Count < 1) || symbolTable.Skip(1).Any() == false)
                throw new ArgumentException("invalid symbolTable in Huffman.Decode()");

            Node root = BuildTree(symbolTable);

            StringBuilder decoded = new();
            Node? currentNode = root;
            List<bool> currentEncoding = [];

            foreach (var bit in encoded)
            {
                if (currentNode is null)
                    throw new InvalidDataException("currentNode is null in Huffman.Decode()");

                currentEncoding.Add(bit);
                currentNode = bit ? currentNode.Right : currentNode.Left;

                if (currentNode is null)
                    throw new InvalidDataException("currentNode is null in Huffman.Decode()");

                if (currentNode is { Left: null, Right: null })
                {
                    if (currentNode is not SymbolNode leaf)
                        throw new InvalidDataException("leaf is not SymbolNode in Huffman.Decode()");

                    SymbolType symbol = leaf.Symbol;
                    if (!symbol.Encoding.SequenceEqual(currentEncoding))
                        throw new InvalidDataException("current encoding differs from the original in Huffman.Decode()");

                    decoded.Append(symbol.Symbol);
                    currentEncoding.Clear();
                    currentNode = root;
                }
            }
            return decoded.ToString();
        }


        private abstract class Node
        {
            public abstract long Count { get; }
            public Node? Left { get; set; } = null;
            public Node? Right { get; set; } = null;
        }

        public class SymbolType
        {
            public required char Symbol { get; set; }
            public long Count { get; set; } = 0;
            public List<bool> Encoding { get; set; } = [];
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
