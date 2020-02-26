using System;

namespace DupeFileFinder
{
    class Program
    {

        static int Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Expects 1 argument: dirpath.");
            }

            string dirPath = args[0];

            try
            {
                DupeFinderParallel.FindDupes(dirPath);
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
