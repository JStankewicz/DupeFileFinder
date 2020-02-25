using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

namespace DupeFileFinder
{
    class Program
    {
        static readonly Dictionary<string, List<string>> hashStringToFilePaths = new Dictionary<string, List<string>>();

        static int Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Expects 1 argument: dirpath.");
            }

            string dirPath = args[0];

            try
            {
                var files = Directory.GetFiles(dirPath, "*", SearchOption.AllDirectories);

                Console.WriteLine("Examining {0} files...", files.Length);
                int count = 0;

                foreach (var filePath in files)
                {
                    Console.WriteLine("Checking file {0}, {1} of {2}. {3}", filePath, ++count, files.Length, ((double)count / files.Length).ToString("P2"));

                    string hashString = GetHashString(filePath);
                    if (hashStringToFilePaths.TryGetValue(hashString, out List<string> filenames))
                    {
                        filenames.Add(filePath);
                    }
                    else
                    {
                        hashStringToFilePaths.Add(hashString, new List<string> { filePath });
                    }
                }

                foreach (var hashString in hashStringToFilePaths.Keys)
                {
                    var filePaths = hashStringToFilePaths.GetValueOrDefault(hashString);
                    if(filePaths.Count < 2)
                    {
                        continue;
                    }

                    Console.WriteLine("{0}:", hashString);
                    foreach (var filePath in filePaths)
                    {
                        Console.WriteLine("\t{0} ", filePath);
                    }
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Invalid filepath.");
                return -1;
            }

            return 0;
        }

        static string GetHashString(string filepath)
        {
            var md5 = MD5.Create();
            var stream = File.OpenRead(filepath);
            var hash = md5.ComputeHash(stream);
            return BitConverter.ToString(hash).Replace("-", "").ToUpperInvariant();
        }
    }
}
