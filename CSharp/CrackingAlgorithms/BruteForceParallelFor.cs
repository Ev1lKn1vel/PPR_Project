using System.Threading.Tasks;

namespace ParallelPasswordCracker
{
    class BruteForceParallelFor : CrackingAlgorithm
    {
        private readonly char[] alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

        private string _foundPassword; // No lock needed since only one thread can have correct password

        public BruteForceParallelFor() { }

        public string Name => "Parallel.For";

        public string Crack(byte[] hashToFind, int maxLength, HashType hashType = HashType.SHA1)
        {
            _foundPassword = null;

            // Take starting letter and let one thread/task do every combination for that letter.
            // Example:
            //    - Thread #1: aaaa ... aZZZ
            //    - Thread #2: baaa ... bZZZ 
            //    - ...
            //    - Thread #n: Zaaa ... ZZZZ
            Parallel.ForEach(alphabet, (letter, state) => {
                using (var hasher = new PasswordHasher(hashType))
                {
                    Generate(letter + "", hashToFind, hasher, state, maxLength - 1);
                }
            });

            return _foundPassword;
        }

        private void Generate(string s, byte[] hashToFind, PasswordHasher hasher, ParallelLoopState state, int length)
        {
            if (state.IsStopped || length == 0) // password found or when length has been reached
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
                    state.Stop();
                }

                Generate(appended, hashToFind, hasher, state, length - 1);
            }
        }
    }
}
