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

        //public string Hash(string source)
        //{
        //    StringBuilder sB = new StringBuilder();

        //    byte[] result = _hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(source));

        //    foreach (byte b in result)
        //        sB.Append(b.ToString("x2"));

        //    return sB.ToString();
        //}

        public byte[] Hash(string source)
        {
            return _hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(source));
        }

        public bool VerifyHash(string input, ReadOnlySpan<byte> hashToCompare)
        {
            var hashOfInput = Hash(input);

            return ByteArrayCompare(hashOfInput, hashToCompare);
        }

        static bool ByteArrayCompare(ReadOnlySpan<byte> a1, ReadOnlySpan<byte> a2)
        {
            return a1.SequenceEqual(a2);
        }
    }
}
