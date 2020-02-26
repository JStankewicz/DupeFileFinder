using System;

namespace DupeFileFinder
{
    class Program
    {

        static int Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Expects at least 1 argument: dirpath.");
            }

            string dirPath = args[0];
            int minSize = -1;
            if (args.Length >= 2)
            {
                minSize = int.Parse(args[1]);
            }

            try
            {
                DupeFinderParallel.FindDupes(dirPath, minSize);
            }
            catch (Exception)
            {
                Console.WriteLine("Invalid filepath.");
                return -1;
            }

            return 0;
        }
    }
}
