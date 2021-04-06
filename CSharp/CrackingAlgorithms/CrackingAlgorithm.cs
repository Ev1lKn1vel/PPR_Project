namespace ParallelPasswordCracker
{
    interface CrackingAlgorithm
    {
        public string Name { get; }

        public string Crack(string hashToFind, int maxLength, HashType hashType = HashType.SHA1);
    }
}
