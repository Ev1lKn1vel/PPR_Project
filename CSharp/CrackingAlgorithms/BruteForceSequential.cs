namespace ParallelPasswordCracker
{
    class BruteForceSequential : CrackingAlgorithm
    {
        private readonly char[] alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

        private bool _found;
        private string _foundPassword;

        public BruteForceSequential() { }

        public string Name => "Sequential";

        public string Crack(string hashToFind, int maxLength, HashType hashType = HashType.SHA1)
        {
            _found = false;
            _foundPassword = null;

            using (PasswordHasher hasher = new PasswordHasher(hashType))
            {
                for (int i = 1; i <= maxLength && !_found; i++)
                {
                    Generate("", hashToFind, hasher, i);
                }
            }

            return _foundPassword;
        }

        private void Generate(string s, string hashToFind, PasswordHasher hasher, int length)
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

                if (hasher.VerifyHash(appended, hashToFind))
                {
                    _foundPassword = appended;
                    _found = true;
                }

                Generate(appended, hashToFind, hasher, length - 1);
            }
        }
    }
}
