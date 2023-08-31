using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Newtonsoft.Json;
using UnityEngine;

namespace Game.Managers
{
    // TODO: Move to Types namespace?
    // TODO: Create Types namespace?
    [System.Serializable]
    public struct Token
    {
        public string tokenName;
        public string tokenId;
        public string image;
        public string amount;
    }

    public class GameDataManager : MonoBehaviour
    {
        [SerializeField] private const string k_GameDataPath = "Game Data/data";

        private TextAsset m_GameDataJson;
        private GameData m_GameData;
        private List<Resource> m_Resources = new List<Resource>();
        private Dictionary<CardType, List<Card>> m_Cards = new Dictionary<CardType, List<Card>>();

        private List<BigInteger> m_AllTokenIds = new List<BigInteger>();

        private List<Token> m_AccountTokens = new List<Token>();
        public List<Token> AccountTokens => m_AccountTokens;

        public static GameDataManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                Debug.LogWarning("Two GameDataManager instances were found, removing this one.");
                Destroy(this.gameObject);
                return;
            }
        }

        private void Start()
        {
            m_GameDataJson = Resources.Load<TextAsset>(k_GameDataPath);

            if (m_GameDataJson == null)
            {
                Debug.LogError("Failed to load the JSON file. Make sure it's in the Resources folder.");
                return;
            }

            LoadGameData();

            ParseGameResources();
            ParseGameCards();

            CountAvailableTokenIds();

            GetAccountTokens();
        }

        private void LoadGameData()
        {
            m_GameData = JsonConvert.DeserializeObject<GameData>(
                m_GameDataJson.text,
                new JsonSerializerSettings { NullValueHandling = NullValueHandling.Include }
            );
        }

        private void ParseGameResources()
        {
            foreach (var resource in m_GameData.resources)
            {
                m_Resources.Add(resource);
            }
        }

        private void ParseGameCards()
        {
            var chainsawCards = m_GameData.cards.chainsaw.ToList();
            m_Cards.Add(CardType.Chainsaw, chainsawCards);

            var roosterCards = m_GameData.cards.rooster.ToList();
            m_Cards.Add(CardType.Rooster, roosterCards);
        }

        private void CountAvailableTokenIds()
        {
            var allResourceIds = m_Resources.Select(resource => BigInteger.Parse(resource.id)).ToList();
            var allChainsawIds = m_Cards[CardType.Chainsaw].Select(card => BigInteger.Parse(card.id)).ToList();
            var allRoosterIds = m_Cards[CardType.Rooster].Select(card => BigInteger.Parse(card.id)).ToList();

            m_AllTokenIds.AddRange(allResourceIds);
            m_AllTokenIds.AddRange(allChainsawIds);
            m_AllTokenIds.AddRange(allRoosterIds);
        }

        private async void GetAccountTokens()
        {
            var data = await ContractInteraction.BalanceOfBatch(m_AllTokenIds.ToArray());

            var tokensAmounts = m_AllTokenIds.Zip(data, (k, v) => new { k, v })
                .ToDictionary(x => x.k, x => BigInteger.Parse(x.v));
            

            foreach (var token in tokensAmounts.Where(token => token.Value > new BigInteger(0)))
            {
                var resourceTokenData = m_GameData.resources.Find(resource => resource.id == token.Key.ToString());
                var cardTokenData = m_GameData.cards.chainsaw.Concat(m_GameData.cards.rooster).ToList().Find(card => card.id == token.Key.ToString());

                if (resourceTokenData != null)
                {
                    m_AccountTokens.Add(
                        new Token
                        {
                            tokenName = resourceTokenData.name,
                            tokenId = token.Key.ToString(),
                            image = resourceTokenData.image,
                            amount = token.Value.ToString()
                        }
                    );
                }
                else if (cardTokenData != null)
                {
                    m_AccountTokens.Add(
                        new Token
                        {
                            tokenName = cardTokenData.name,
                            tokenId = token.Key.ToString(),
                            image = cardTokenData.image,
                            amount = token.Value.ToString()
                        }
                    );
                }
            }
        }

        public class Resource
        {
            public string name { get; set; }
            public string id { get; set; }
            public string image { get; set; }
        }

        public class Card
        {
            public string name { get; set; }
            public int level { get; set; }
            public string id { get; set; }
            public string image { get; set; }
        }

        public class Cards
        {
            public List<Card> chainsaw { get; set; }
            public List<Card> rooster { get; set; }
        }

        public class GameData
        {
            public List<Resource> resources { get; set; }
            public Cards cards { get; set; }
        }

        public enum CardType
        {
            Chainsaw,
            Rooster
        }
    }
}