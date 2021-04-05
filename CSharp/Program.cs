using System;
using System.Diagnostics;

namespace ParallelPasswordCracker
{
    class Program
    {
        static void Main(string[] args)
        {
            var secret = "A";
            var hashType = HashType.SHA1;
            var passwordLengthToCrack = secret.Length;

            PasswordHasher hasher = new PasswordHasher(HashType.SHA1);
            var hashToFind = hasher.Hash(secret);

            BruteForceSerial bruteForceSequential = new BruteForceSerial(hashToFind, hashType);
            //BruteForceParallel bruteForceParallel = new BruteForceParallel(hashToFind, hashType);

            Stopwatch sW = new Stopwatch();
            sW.Start();
            var result = bruteForceSequential.CrackFixedLength(passwordLengthToCrack);
            //var result = bruteForceParallel.CrackFixedLength(passwordLengthToCrack);
            sW.Stop();

            if (result != null)
            {
                Console.WriteLine("Password is " + result);
            }
            else
            {
                Console.WriteLine("Not found");
            }

            Console.WriteLine($"Duration: {sW.ElapsedMilliseconds} ms");

            Console.ReadLine();
        }
    }
}
