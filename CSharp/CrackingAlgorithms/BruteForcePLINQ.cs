using System;
using System.Linq;

namespace ParallelPasswordCracker
{
    class BruteForcePLINQ : CrackingAlgorithm
    {
        private readonly char[] alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

        private string _foundPassword; // No lock needed since only one thread can have correct password
        private volatile bool _found; // Race condition doesn't matter. Worst case a few more iterations are done

        public BruteForcePLINQ() { }

        public string Name => "PLINQ";

        public string Crack(string hashToFind, int maxLength, HashType hashType = HashType.SHA1)
        {
            _foundPassword = null;
            _found = false;

            // Take starting letter and let one thread/task do every combination for that letter.
            // Example:
            //    - Thread #1: aaaa ... aZZZ
            //    - Thread #2: baaa ... bZZZ 
            //    - ...
            //    - Thread #n: Zaaa ... ZZZZ
            alphabet.AsParallel().ForAll(letter =>
            {
                using (var hasher = new PasswordHasher(hashType))
                {
                    Console.WriteLine(letter);
                    Generate(letter + "", hashToFind, hasher, maxLength - 1);
                }
            });

            return _foundPassword;
        }

        private void Generate(string s, string hashToFind, PasswordHasher hasher, int length)
        {
            if (_found || length == 0) // password found or when length has been reached
            {
                return;
            }

            for (int i = 0; i < alphabet.Length; i++)
            {
                string appended = s + alphabet[i];

                //Console.WriteLine(appended);

                if (hasher.VerifyHash(appended, hashToFind))
                {
                    _foundPassword = appended; // No lock needed since only one thread can have correct password
                    _found = true;
                }

                Generate(appended, hashToFind, hasher, length - 1);
            }
        }
    }
}
