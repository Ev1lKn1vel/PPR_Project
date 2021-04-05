using System;
using System.Diagnostics;

namespace ParallelPasswordCracker
{
    class Program
    {
        static void Main(string[] args)
        {
            var secret = "zzzzz";
            var hashType = HashType.SHA1;
            var passwordLengthToCrack = secret.Length;

            var hashToFind = secret.Hash(hashType);

            BruteForceSequential bruteForceSequential = new BruteForceSequential(hashToFind, hashType);

            Stopwatch sW = new Stopwatch();
            sW.Start();
            var result = bruteForceSequential.CrackFixedLength(passwordLengthToCrack);
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
