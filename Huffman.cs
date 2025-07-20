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
        public static (List<bool> encoded, List<SymbolType> symbolTable) Encode(string message) 
        {
            ArgumentNullException.ThrowIfNull(message);

            // parcurs messajul odata si facut count
            List<SymbolType> symbols = [];
            foreach (char c in message)
            {
                int index = -1;
                for (int i = 0; i < symbols.Count; ++i)
                {
                    if (symbols[i].Symbol == c)
                    {
                        index = i; break;
                    }
                }

                if(index == -1)
                    symbols.Add(new SymbolType { Symbol = c, Count = 1 });
                else
                    symbols[index].Count++;
            }

            // constriuit arborele cu un min heap
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

            while(heap.Count > 1)
            {
                var left = heap.Dequeue();
                var right = heap.Dequeue();
                var newElem = new InternalNode(left, right); 

                heap.Enqueue(newElem, newElem.Count);
            }

            Node root = heap.Dequeue();
            List<bool> curentEncoding = [];

            EncodeLeafs(root, curentEncoding);

            foreach (var symbol in symbols)
            {
                Console.Write($"{symbol.Symbol} : {symbol.Count} : ");
                foreach (bool bit in symbol.Encoding)
                    Console.Write(bit ? '1' : '0');
                Console.WriteLine();
            }

            // parcurs mesajul si cautat simbolul in copac
            // apend in encoded
            List<bool> encoded = [];
            foreach (char ch in message)
            {
                foreach(var symbol in symbols)
                    if(symbol.Symbol == ch)
                    {
                        encoded.AddRange(symbol.Encoding);
                        break;
                    }
            }

            return (encoded, symbols);
        }

        private static void EncodeLeafs(Node? root, List<bool> curentEncoding)
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
                leaf.Symbol.Encoding = new List<bool>(curentEncoding);
                return;
            }

            curentEncoding.Add(false);
            EncodeLeafs(root.Left, curentEncoding);

            curentEncoding[^1] = true;
            EncodeLeafs(root.Right, curentEncoding);

            curentEncoding.RemoveAt(curentEncoding.Count - 1);
        }

        public static string Decode(List<bool> encoded, List<SymbolType> symbolTree) 
        {
            // treversat copacul cu bitii din encoded si append la string.
            return "";
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
