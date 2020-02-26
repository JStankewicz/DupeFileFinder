using System;
using System.IO;
using System.Security.Cryptography;

namespace DupeFileFinder
{
    class Utils
    {
        internal static string GetHashString(string filePath)
        {
            var md5 = MD5.Create();
            var stream = File.OpenRead(filePath);
            var hash = md5.ComputeHash(stream);
            return BitConverter.ToString(hash).Replace("-", "").ToUpperInvariant();
        }
    }
}
