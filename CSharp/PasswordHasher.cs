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

    public class PasswordHasher : IDisposable
    {
        private HashAlgorithm _hashAlgorithm;

        public PasswordHasher(HashType hashType)
        {
            _hashAlgorithm = hashType == HashType.SHA1 ? SHA1.Create() : SHA256.Create();
        }

        public void Dispose()
        {
            _hashAlgorithm.Dispose();
        }

        public string Hash(string source)
        {
            StringBuilder sB = new StringBuilder();

            byte[] result = _hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(source));

            foreach (byte b in result)
                sB.Append(b.ToString("x2"));

            return sB.ToString();
        }

        public bool VerifyHash(string input, string hashToCompare)
        {
            var hashOfInput = Hash(input);

            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            return comparer.Compare(hashOfInput, hashToCompare) == 0;
        }
    }
}
