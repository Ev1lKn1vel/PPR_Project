namespace ParallelPasswordCracker
{
    class BruteForceSequentialIterative : CrackingAlgorithm
    {
        private readonly char[] alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

        public BruteForceSequentialIterative() { }

        public string Name => "Sequential (Iterative)";

        public string Crack(byte[] hashToFind, int maxLength, HashType hashType = HashType.SHA1)
        {
            using (PasswordHasher hasher = new PasswordHasher(hashType))
            {
                return GuessPasswordSerial(hashToFind, maxLength, hasher);
            }
        }

        public string GuessPasswordSerial(byte[] hashToFind, int maxLength, PasswordHasher hasher)
        {
            string possiblePW;
            for (int first = 0; first < alphabet.Length && maxLength > 0; first++)
            {
                possiblePW = "" + alphabet[first];
                if (hasher.VerifyHash(possiblePW, hashToFind))
                {
                    return possiblePW;
                }
                for (int second = 0; second < alphabet.Length && maxLength > 1; second++)
                {
                    possiblePW = "" + alphabet[first] + alphabet[second];
                    if (hasher.VerifyHash(possiblePW, hashToFind))
                    {
                        return possiblePW;
                    }
                    for (int third = 0; third < alphabet.Length && maxLength > 2; third++)
                    {
                        possiblePW = "" + alphabet[first] + alphabet[second] + alphabet[third];
                        if (hasher.VerifyHash(possiblePW, hashToFind))
                        {
                            return possiblePW;
                        }
                        for (int fourth = 0; fourth < alphabet.Length && maxLength > 3; fourth++)
                        {
                            possiblePW = "" + alphabet[first] + alphabet[second] + alphabet[third] + alphabet[fourth];
                            if (hasher.VerifyHash(possiblePW, hashToFind))
                            {
                                return possiblePW;
                            }
                            for (int fifth = 0; fifth < alphabet.Length && maxLength > 4; fifth++)
                            {
                                possiblePW = "" + alphabet[first] + alphabet[second] + alphabet[third] + alphabet[fourth] + alphabet[fifth];
                                if (hasher.VerifyHash(possiblePW, hashToFind))
                                {
                                    return possiblePW;
                                }
                            }
                        }
                    }
                }
            }

            return null;
        }
    }
}
