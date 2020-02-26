using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DupeFileFinder
{
    class DupeFinderParallel
    {
        private const int MEGABYTE_SIZE = 1048576;
        private static readonly Dictionary<string, List<string>> hashStringToFilePaths = new Dictionary<string, List<string>>();

        /// <summary>
        /// Find duplicate files above an optional minimum size in a given directory.
        /// </summary>
        /// <param name="dirPath">Directory to check for duplicates.</param>
        /// <param name="minSize">Minimum file size in megabytes.</param>
        public static void FindDupes(string dirPath, int minSize)
        {
            if(minSize < 0)
            {
                Console.WriteLine("Invalid min size {0}. Setting minimum size to check to 0.", minSize);
                minSize = Math.Max(minSize, 0);
            }

            // Get the files from the directory specified and its subdirectories.
            var files = Directory.EnumerateFiles(@dirPath, "*", new EnumerationOptions() { RecurseSubdirectories = true });

            // For each file, find its length and map it to the file name.
            var fileLengths = from f in files select new { name = f, len = new FileInfo(f).Length };

            // Get counts for each file length. Filter out unique file lengths. Order by size, descending.
            var fileLengthCounts = fileLengths.GroupBy(l => l.len).Select(group => new { key = group.Key, val = group.Count() }).Where(x => x.val >= 2).OrderByDescending(x => x.val);

            // For debugging...
            //foreach (var x in fileLengthCounts)
            //{
            //    Console.WriteLine("{0},{1}", x.key, x.val);
            //}

            // Join file lengths with file length counts to get all file names for files which are the same size as another file.
            // Then filter out files below the specified minimum size.
            var filteredFiles = fileLengths.Join(fileLengthCounts, x => x.len, y => y.key, (x, y) => new { x.name, x.len }).Where(x => x.len >= minSize * MEGABYTE_SIZE);

            Console.WriteLine("Examining {0} files...", filteredFiles.Select(x => x.name).ToList().Count);

            // For debugging, to pause and read the file count.
            //Console.ReadLine();

            // Set MaxDegreeOfParallelism to 32 because larger numbers caused desktop responsiveness problems.
            Parallel.ForEach(filteredFiles, new ParallelOptions() { MaxDegreeOfParallelism = 32 }, x => Process(x.name));

            // Print out hashes with more than one file name followed by the filenames.
            foreach (var hashString in hashStringToFilePaths.Keys)
            {
                var filePaths = hashStringToFilePaths.GetValueOrDefault(hashString);
                if (filePaths.Count < 2)
                {
                    continue;
                }

                filePaths.Sort();

                Console.WriteLine("{0}:", hashString);
                foreach (var filePath in filePaths)
                {
                    Console.WriteLine("\t{0} ", filePath);
                }
            }
        }

        private static void Process(string filePath)
        {
            // Possibly want to filter on type later...
            //if (string.Equals(".jpg", Path.GetExtension(filePath), StringComparison.InvariantCultureIgnoreCase))
            //{
            //    return;
            //}

            Console.WriteLine("Checking file {0}", filePath);
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            string hashString = Utils.GetHashString(filePath);

            stopwatch.Stop();
            Console.WriteLine("Checked file {0} in {1} ms", filePath, stopwatch.ElapsedMilliseconds);


            lock (hashStringToFilePaths)
            {
                if (hashStringToFilePaths.TryGetValue(hashString, out List<string> filenames))
                {
                    filenames.Add(filePath);
                }
                else
                {
                    hashStringToFilePaths.Add(hashString, new List<string> { filePath });
                }
            }
        }
    }
}
