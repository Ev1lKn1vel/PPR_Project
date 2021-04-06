using System;
using System.Collections.Generic;
using System.Diagnostics;

/* Author
Name: Tim Paulus
Username: se20m031
Matrikelnummer: 01527558
*/
namespace ParallelPasswordCracker
{
    class Program
    {
        private const string Secret = "ZZZZ";
        private static readonly int PasswordLengthToCrack = Secret.Length;
        private static readonly HashType HashType = HashType.SHA1;

        private static byte[] hashToFind;

        // Benchmark constants
        private const int IterationsPerAlgorithm = 10;
        private const string DiscardSecret = "ZZZZ";
        private static byte[] discardHashToFind;

        static void Main(string[] args)
        {
            using var hasher = new PasswordHasher(HashType);
            hashToFind = hasher.Hash(Secret);

            //ManualTesting();

            Benchmark();

            Console.ReadLine();
        }

        public static void ManualTesting()
        {
            Console.WriteLine("Starting...");
            Stopwatch sW = new Stopwatch();
            sW.Start();

            // CHANGE HERE
            var result = new BruteForceSequentialIterative().Crack(hashToFind, PasswordLengthToCrack); 

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
        }

        private static void Benchmark()
        {
            Console.WriteLine("\n----- STARTING BENCHMARK -----");

            using var hasher = new PasswordHasher(HashType);
            discardHashToFind = hasher.Hash(Secret);

            var crackingAlgorithms = new List<CrackingAlgorithm>()
            {
                new BruteForceSequentialRecursive(),
                new BruteForceSequentialIterative(),
                new BruteForceParallelCustom(),
                new BruteForceParallelFor(),
                new BruteForcePLINQ()
            };

            // Benchmark every algorithm
            var results = new Dictionary<string, float>();
            foreach (var crackingAlgorithm in crackingAlgorithms)
            {
                var averageRuntime = BenchmarkSingle(crackingAlgorithm);
                results.Add(crackingAlgorithm.Name, averageRuntime);
            }

            // Print end results
            Console.WriteLine($"----- Results (over {IterationsPerAlgorithm} iterations) -----");
            foreach (var result in results)
            {
                Console.WriteLine($"{result.Key}: {result.Value} ms");
            }
        }

        private static float BenchmarkSingle(CrackingAlgorithm crackingAlgorithm)
        {
            Console.WriteLine($"Starting benchmark for {crackingAlgorithm.Name}");

            // Discard first batch (warmup)
            Console.Write("Initializing");
            for (int i = 0; i < 5; i++)
            {
                crackingAlgorithm.Crack(discardHashToFind, DiscardSecret.Length);
                Console.Write(".");
                GC.Collect();
            }
            Console.WriteLine("\n");

            Stopwatch sw = new Stopwatch();

            long milliseconds = 0;
            for (int i = 0; i < IterationsPerAlgorithm; i++)
            {
                Console.Write($"({i + 1}/{IterationsPerAlgorithm})");

                sw.Restart();
                var result = crackingAlgorithm.Crack(hashToFind, PasswordLengthToCrack, HashType);
                sw.Stop();

                milliseconds += sw.ElapsedMilliseconds;

                var resultString = result != null ? $"SUCCESS ({result})" : "FAILED";
                Console.WriteLine($" {resultString} in {sw.ElapsedMilliseconds} ms");
            }
            float averageRuntime = milliseconds / IterationsPerAlgorithm;

            Console.WriteLine($"{crackingAlgorithm.Name}: {averageRuntime} ms (average)\n");

            return averageRuntime;
        }
    }
}
