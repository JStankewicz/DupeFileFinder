using System;
using System.Collections.Generic;
using System.IO;

namespace DupeFileFinder
{
    class DupeFinderSingle
    {
        private static readonly Dictionary<string, List<string>> hashStringToFilePaths = new Dictionary<string, List<string>>();

        public static void FindDupes(string dirPath)
        {
            var files = Directory.GetFiles(dirPath, "*", SearchOption.AllDirectories);

            Console.WriteLine("Examining {0} files...", files.Length);
            int count = 0;

            foreach (var filePath in files)
            {
                Console.WriteLine("Checking file {0}, {1} of {2}. {3}", filePath, ++count, files.Length, ((double)count / files.Length).ToString("P2"));

                string hashString = Utils.GetHashString(filePath);
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
                if (filePaths.Count < 2)
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
    }
}
