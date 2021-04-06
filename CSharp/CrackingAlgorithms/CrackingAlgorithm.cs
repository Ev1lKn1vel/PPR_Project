namespace ParallelPasswordCracker
{
    interface CrackingAlgorithm
    {
        public string Name { get; }

        public string Crack(byte[] hashToFind, int maxLength, HashType hashType = HashType.SHA1);
    }
}
