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
            ArgumentNullException.ThrowIfNull(message);

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

            // debug
            foreach (var symbol in symbolDict.Values)
            {
                Console.Write($"{symbol.Symbol} : {symbol.Count} : ");
                foreach (bool bit in symbol.Encoding)
                    Console.Write(bit ? '1' : '0');
                Console.WriteLine();
            }

            encoding = [];
            foreach (char ch in message)
                encoding.AddRange(symbolDict[ch].Encoding);

            return (encoding, symbolDict.Values);
        }

        private static Node BuildTree(IEnumerable<SymbolType> symbols)
        {
            var heap = new PriorityQueue<Node, int>();

            foreach (SymbolType symbol in symbols)
            {
                if (symbol.Count <= 0)
                {
                    Console.WriteLine("[!] symbolType.Count overflow");
                    Environment.Exit(1);
                }

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
                if(root is not SymbolNode)
                {
                    Console.WriteLine("[!] leaf isnt symbolNode");
                    Environment.Exit(1);
                }
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
            Node root = BuildTree(symbolTable);

            // treversat copacul cu bitii din encoded si append la string.
            string decoded = "";
            Node? currentNode = root;
            List<bool> currentEncoding = [];

            foreach (var bit in encoded)
            {
                if (currentNode is null)
                    throw new InvalidDataException("currentNode is null in Huffman.Decode");

                currentEncoding.Add(bit);
                currentNode = bit ? currentNode.Right : currentNode.Left;

                if (currentNode is null)
                    throw new InvalidDataException("currentNode is null in Huffman.Decode");

                if(currentNode.Left is null && currentNode.Right is null)
                {
                    if(currentNode is not SymbolNode)
                        throw new InvalidDataException("leaf is not SymbolNode in Huffman.Decode");

                    SymbolType symbol = ((SymbolNode)currentNode).Symbol;
                    if (!symbol.Encoding.SequenceEqual(currentEncoding))
                        throw new InvalidDataException("current encoding differs from the original in Huffman.Decode");

                    decoded += symbol.Symbol;
                    currentEncoding = [];
                    currentNode = root;
                }


            }
            return decoded;
        }


        private abstract class Node
        { 
            public abstract int Count { get; }
            public Node? Left { get; set; } = null;
            public Node? Right { get; set; } = null;

            public static bool operator <(Node a, Node b)
                => a.Count < b.Count;

            public static bool operator >(Node a, Node b)
                => a.Count > b.Count;
        }

        public class SymbolType
        {
            public char Symbol { get; set; }
            public int Count { get; set; } = 0;
            public List<bool> Encoding { get; set; } = [];
        }

        private class SymbolNode : Node
        {
            public required SymbolType Symbol { get; set; }
            public override int Count => Symbol.Count;
        }

        private class InternalNode : Node
        {
            public InternalNode(Node left, Node right)
            {
                Left = left;
                Right = right;
                _count = left.Count + right.Count;

                if(_count <= 0)
                {
                    Console.WriteLine("[!] overflow in InternalNode");
                    Environment.Exit(1);
                }
            }

            public override int Count => _count;

            private readonly int _count;
        }
    }
}
