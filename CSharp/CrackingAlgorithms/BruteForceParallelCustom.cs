using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ParallelPasswordCracker
{
    class BruteForceParallelCustom : CrackingAlgorithm
    {
        private readonly char[] alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

        private string _foundPassword; // No lock needed since only one thread can have correct password
        private volatile bool _found; // Race condition doesn't matter. Worst case a few more iterations are done

        private const int MaxTasks = 8;

        public BruteForceParallelCustom() { }

        public string Name => "Custom Parallelization";

        public string Crack(byte[] hashToFind, int maxLength, HashType hashType = HashType.SHA1)
        {
            _foundPassword = null;
            _found = false;

            var semaphore = new SemaphoreSlim(0, MaxTasks);

            Task[] tasks = new Task[alphabet.Length];

            // Take starting letter and let one thread/task do every combination for that letter.
            // Example:
            //    - Thread #1: aaaa ... aZZZ
            //    - Thread #2: baaa ... bZZZ 
            //    - ...
            //    - Thread #n: Zaaa ... ZZZZ
            for (int i = 0; i < alphabet.Length; i++)
            {
                var letter = alphabet[i];

                tasks[i] = Task.Run(() =>
                {
                    semaphore.Wait();
                    
                    try
                    {
                        using (var hasher = new PasswordHasher(hashType))
                        {
                            //Console.WriteLine(letter);
                            Generate(letter + "", hashToFind, hasher, maxLength - 1);
                        }
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                });
            }

            semaphore.Release(MaxTasks);

            Task.WaitAll(tasks);

            return _foundPassword;
        }

        private void Generate(string s, byte[] hashToFind, PasswordHasher hasher, int length)
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
                    _found = false;
                }

                Generate(appended, hashToFind, hasher, length - 1);
            }
        }
    }
}
