using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuffmanDemo
{
    internal static class CliParser
    {
        public enum Mode
        {
            Listen,
            SendFile,
            SendMessage
        }

        public static ParseResult Parse(string[] args)
        {
            if (args.Length == 0)
                return PrintHelp();

            bool verbose = args.Contains("-v");
            args = args.Where(a => a != "-v").ToArray();

            return args switch
            {
                ["send", var target, "-m", var msg]     => HandleSend(target, msg, false, verbose),
                ["send", var portStr, "-f", var file]   => HandleSend(portStr, file, true, verbose),
                ["listen", "-p", var port]            => HandleListen(port, verbose),
                ["help"]                                => PrintHelp(),
                _                                       => PrintHelp("Invalid syntax")
            };
        }

        private static ParseResult HandleSend(string target, string data, bool isFile, bool verbose)
        {
            if (!TryParseEndpoint(target, out var host, out int port))
                return PrintHelp("Invalid target");

            if (isFile && (data = TryReadFile(data)) == null)
                return PrintHelp("Invalid file");
            else
                return new ParseResult
                {
                    Mode = isFile ? Mode.SendFile : Mode.SendMessage,
                    Target = host,
                    Port = port,
                    Verbose = verbose,
                    Content = data
                };
        }

        static string? TryReadFile(string path, long maxSizeBytes = 10_000_000) // 10 MB
        {
            try
            {
                if (string.IsNullOrWhiteSpace(path))
                    return null;

                var fullPath = Path.GetFullPath(path);

                if (!File.Exists(fullPath))
                    return null;

                var attrs = File.GetAttributes(fullPath);
                if ((attrs & FileAttributes.Directory) != 0)
                    return null;

                var info = new FileInfo(fullPath);
                if (info.Length > maxSizeBytes)
                    return null;

                return File.ReadAllText(fullPath, Encoding.UTF8);
            }
            catch (Exception)
            {
                return null;
            }
        }

        static bool TryParseEndpoint(string s, out string host, out int port)
        {
            host = "";
            port = -1;

            var i = s.LastIndexOf(':');
            if (i <= 0 || i == s.Length - 1)
                return false; // no ':' or nothing after it

            host = s[..i];
            if (string.IsNullOrEmpty(host))
                return false;

            if (i <= 0 || i == s.Length - 1) return 
                false;

            var portStr = s[(i + 1)..];

            if (!int.TryParse(portStr, out port) || port is < 1 or > 65535)
                return false;

            return true;
        }


        private static ParseResult HandleListen(string portStr, bool verbose)
        {
            if (!int.TryParse(portStr, out var port) || port is < 1 or > 65535)
                return PrintHelp("Invalid port");

            return new ParseResult
            {
                Mode = Mode.Listen,
                Port = port,
                Verbose = verbose
            };
        }

        private static ParseResult PrintHelp(string msg = "")
        {
            if (!string.IsNullOrEmpty(msg))
                Console.Error.WriteLine(msg);

            Console.WriteLine("""
            Usage: huffmandemo [command] [options]
            Commands:
              huffmandemo send <target:port> -f <file>
              huffmandemo send <target:port> -m <message>
              huffmandemo listen -p <port>
              huffmandemo help
            Flags:
              -v    verbose
            """);

            return new ParseResult();
        }



        public class ParseResult
        {
            public bool Verbose { get; set; } = false;

            public string Target { get; set; } = "";

            public int Port { get; set; } = -1;

            public Mode Mode { get; set; }

            public string Content { get; set; } = "";
        }
    }
}
