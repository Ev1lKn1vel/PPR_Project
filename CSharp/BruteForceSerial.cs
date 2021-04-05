using System;
using System.Collections.Generic;
using System.Text;

namespace ParallelPasswordCracker
{
    class BruteForceSerial
    {
        private readonly char[] alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
        //private readonly char[] alphabet = "abcdefghijklmnopqrstuvwxyz".ToCharArray();

        private bool _found = false;
        private string _foundPassword = string.Empty;

        private string _hashToFind;

        private PasswordHasher _hasher;

        public BruteForceSerial(string hashToFind, HashType hashType)
        {
            _hashToFind = hashToFind;
            _hasher = new PasswordHasher(hashType);
        }

        public string CrackFixedLength(int length)
        {
            Generate("", length);

            if (_found)
            {
                return _foundPassword;
            }
            else
            {
                return null;
            }
        }

        public string CrackVariableLength(int length)
        {
            for (int i = 1; i <= length && !_found; i++)
            {
                Console.WriteLine($"Checking length={i}");
                Generate("", i);
            }

            if (_found)
            {
                return _foundPassword;
            }
            else
            {
                return null;
            }
        }

        private void Generate(string s, int length)
        {
            if (_found || length == 0) // pw found or when length has been reached
            {
                return;
            }

            for (int i = 0; i < alphabet.Length; i++) // iterate through alphabet
            {
                // Create new string with next character
                // Call generate again until string has reached it's length
                string appended = s + alphabet[i];

                //Console.WriteLine(appended);

                if (_hasher.VerifyHash(appended, _hashToFind))
                {
                    _foundPassword = appended;
                    _found = true;
                }

                Generate(appended, length - 1);
            }
        }
    }
}
