using System;
using System.Security.Cryptography;
using System.Text;

namespace ParallelPasswordCracker
{
    public enum HashType
    {
        SHA1,
        SHA256
    }

    public static class HashExtensions
    {
        public static string Hash(this string source, HashType hashType)
        {
            StringBuilder sB = new StringBuilder();

            using (HashAlgorithm hash = hashType == HashType.SHA1 ? SHA1.Create() : SHA256.Create())
            {
                byte[] result = hash.ComputeHash(Encoding.UTF8.GetBytes(source));

                foreach (byte b in result)
                    sB.Append(b.ToString("x2"));
            }

            return sB.ToString();
        }

        public static bool VerifyHash(this string input, string hash, HashType hashType)
        {
            var hashOfInput = input.Hash(hashType);

            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            return comparer.Compare(hashOfInput, hash) == 0;
        }
    }
}
