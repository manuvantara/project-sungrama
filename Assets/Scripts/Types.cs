namespace Game.Types
{
    [System.Serializable]
    public struct TokenMetadata
    {
        public int level;
        public float hitpoints;
        public float damage;
        public string reload;
        public float distance;
        public float speed;
    }
    
    [System.Serializable]
    public struct Token
    {
        public string tokenName;
        public string tokenId;
        public string image;
        public string amount;
        public TokenMetadata metadata;
    }
}