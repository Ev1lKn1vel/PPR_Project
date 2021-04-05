using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ParallelPasswordCracker
{
    class BruteForceParallel
    {
        private readonly char[] alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
        //private readonly char[] alphabet = "abcdefghijklmnopqrstuvwxyz".ToCharArray();

        private bool _found = false;
        private string _foundPassword = string.Empty;

        private string _hashToFind;

        private PasswordHasher _hasher;

        private int _originalLength = 0;

        public BruteForceParallel(string hashToFind, HashType hashType)
        {
            _hashToFind = hashToFind;
            _hasher = new PasswordHasher(hashType);
        }

        public string CrackFixedLength(int length)
        {
            _originalLength = length;

            //Parallel.For(0, alphabet.Length, from =>
            //{
            //    Generate("", from, from + 1, length);
            //});

            var _foundPassword = GenerateFor(4);

            if (_found)
            {
                return _foundPassword;
            }
            else
            {
                return null;
            }
        }

        //public string CrackVariableLength(int length)
        //{
        //    for (int i = 1; i <= length && !_found; i++)
        //    {
        //        Console.WriteLine($"Checking length={i}");
        //        Generate("", i);
        //    }

        //    if (_found)
        //    {
        //        return _foundPassword;
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}

        public string GenerateFor(int length)
        {
            foreach (char first in alphabet)
            {
                foreach (char second in alphabet)
                {
                    foreach (char third in alphabet)
                    {
                        foreach (char fourth in alphabet)
                        {
                            string possiblePW = "" + first + second + third + fourth;
                            if (_hasher.VerifyHash(possiblePW, _hashToFind))
                            {
                                _found = true;
                                return possiblePW;
                            }
                        }
                    }
                }
            }
            return "Password not found";
    }

        private void Generate(string s, int from, int to, int length)
        {
            if (_found || length == 0) // pw found or when length has been reached
            {
                return;
            }

            if (length == _originalLength) // first character
            {
                //Console.WriteLine("Test");
                for (int i = from; i < to; i++) // iterate through alphabet
                {
                    // Create new string with next character
                    // Call generate again until string has reached it's length
                    string appended = s + alphabet[i];

                    //Console.WriteLine(appended);

                    if (_hasher.VerifyHash(appended, _hashToFind))
                    {
                        _foundPassword = appended;
                        _found = true;
                        Console.WriteLine("FOUND");
                    }

                    Generate(appended, from, to, length - 1);
                }
            }
            else
            {
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

                    Generate(appended, from, to, length - 1);
                }
            }
            
        }
    }
}
