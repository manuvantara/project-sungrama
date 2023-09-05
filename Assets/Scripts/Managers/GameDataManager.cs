using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Game.ContractInteractions;
using Game.Types;
using Newtonsoft.Json;
using Thirdweb;
using UnityEngine;

namespace Game.Managers
{
    public class GameDataManager : MonoBehaviour
    {
        public static event Action OnAccountTokensUpdated;
        public static event Action OnGetAccountTokensFinished;
        public static event Action<Receipt> OnLevelUpDrone;
        
        // [SerializeField] public static k_Maximum
        [SerializeField] private string m_GameDataPath = "Game Data/data";
        [SerializeField] private string m_BlueprintsPath = "Game Data/blueprints";

        private GameData m_GameData;
        private List<Blueprint> m_Blueprints = new List<Blueprint>();
        private List<Resource> m_Resources = new List<Resource>();
        private Dictionary<DroneType, List<Drone>> m_Drones = new Dictionary<DroneType, List<Drone>>();

        private List<BigInteger> m_AllTokenIds = new List<BigInteger>();

        private List<Token> m_MarketplaceTokens = new List<Token>();
        public List<Token> MarketplaceTokens => m_MarketplaceTokens;

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
            
            EventManager.OnMarketplaceItemBought += GetAccountTokens;
        }
        
        private void Start()
        {
            var gameDataJson = Resources.Load<TextAsset>(m_GameDataPath);
            var blueprintsJson = Resources.Load<TextAsset>(m_BlueprintsPath);

            if (gameDataJson == null || blueprintsJson == null)
            {
                Debug.LogError("Failed to load the JSON file. Make sure it's in the Resources folder.");
                return;
            }

            LoadGameData(gameDataJson, blueprintsJson);

            ParseGameResources();
            ParseGameCards();

            CountAvailableTokenIds();
            GetMarketplaceTokens();

            GetAccountTokens();
        }

        private void LoadGameData(TextAsset gameDataJson, TextAsset blueprintsJson)
        {
            m_GameData = JsonConvert.DeserializeObject<GameData>(
                gameDataJson.text,
                new JsonSerializerSettings { NullValueHandling = NullValueHandling.Include }
            );

            m_Blueprints = JsonConvert.DeserializeObject<List<Blueprint>>(
                blueprintsJson.text,
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
            var chainsawCards = m_GameData.drones.chainsaw.ToList();
            m_Drones.Add(DroneType.Chainsaw, chainsawCards);

            var roosterCards = m_GameData.drones.rooster.ToList();
            m_Drones.Add(DroneType.Rooster, roosterCards);

            var mausCards = m_GameData.drones.maus.ToList();
            m_Drones.Add(DroneType.Maus, mausCards);

            var carlsonCards = m_GameData.drones.carlson.ToList();
            m_Drones.Add(DroneType.Carlson, carlsonCards);
        }

        private void CountAvailableTokenIds()
        {
            var allResourceIds = m_Resources.Select(resource => BigInteger.Parse(resource.id)).ToList();
            var allChainsawIds = m_Drones[DroneType.Chainsaw].Select(card => BigInteger.Parse(card.id)).ToList();
            var allRoosterIds = m_Drones[DroneType.Rooster].Select(card => BigInteger.Parse(card.id)).ToList();
            var allMausIds = m_Drones[DroneType.Maus].Select(card => BigInteger.Parse(card.id)).ToList();
            var allCarlsonIds = m_Drones[DroneType.Carlson].Select(card => BigInteger.Parse(card.id)).ToList();

            m_AllTokenIds.AddRange(allResourceIds);
            m_AllTokenIds.AddRange(allChainsawIds);
            m_AllTokenIds.AddRange(allRoosterIds);
            m_AllTokenIds.AddRange(allMausIds);
            m_AllTokenIds.AddRange(allCarlsonIds);
        }

        private void GetMarketplaceTokens()
        {
            // Only first level cards are available on the marketplace.
            var allTokensList = m_GameData.drones.chainsaw.Concat(m_GameData.drones.maus)
                .Concat(m_GameData.drones.rooster).Concat(
                    m_GameData.drones.carlson).ToList().FindAll(card => card.level == 1);

            // Our marketplace tokens are all available tokens from the game data.
            foreach (var token in allTokensList)
            {
                m_MarketplaceTokens.Add(
                    new Token
                    {
                        tokenName = token.name,
                        tokenId = token.id,
                        image = token.image,
                        amount = "1"
                    }
                );
            }

            foreach (var resource in m_GameData.resources)
            {
                m_MarketplaceTokens.Add(
                    new Token
                    {
                        tokenName = resource.name,
                        tokenId = resource.id,
                        image = resource.image,
                        amount = "1"
                    }
                );
            }
        }

        public async void GetAccountTokens()
        {
            m_AccountTokens.Clear();

            var data = await ContractInteraction.BalanceOfBatch(m_AllTokenIds.ToArray());

            var tokensAmounts = m_AllTokenIds.Zip(data, (k, v) => new { k, v })
                .ToDictionary(x => x.k, x => BigInteger.Parse(x.v));

            foreach (var token in tokensAmounts.Where(token => token.Value > new BigInteger(0)))
            {
                var resourceTokenData = m_GameData.resources.Find(resource => resource.id == token.Key.ToString());
                var cardTokenData = m_GameData.drones.chainsaw.Concat(m_GameData.drones.rooster)
                    .Concat(m_GameData.drones.maus).Concat(
                        m_GameData.drones.carlson).ToList().Find(card => card.id == token.Key.ToString());

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
                            amount = token.Value.ToString(),
                            metadata = new TokenMetadata
                            {
                                level = cardTokenData.level,
                                hitpoints = cardTokenData.hitpoints,
                                damage = cardTokenData.damage,
                                reload = cardTokenData.reload,
                                distance = cardTokenData.distance,
                                speed = cardTokenData.speed
                            }
                        }
                    );
                }
            }
            
            OnAccountTokensUpdated?.Invoke();
            OnGetAccountTokensFinished?.Invoke();
        }

        public List<DroneWithBlueprint> GetDronesWithHighestLevel()
        {
            if (m_AccountTokens.Count == 0)
            {
                Debug.LogError("No tokens found in the account.");
                return null;
            }

            var drones = new List<DroneWithBlueprint>();

            foreach (var drone in from token in m_AccountTokens
                     let droneType = GetDroneTypeById(token.tokenId)
                     where droneType != DroneType.Empty
                     select m_Drones[droneType].Find(drone => drone.id == token.tokenId))
            {
                // Blueprint input ids always start with the drone id. 
                var blueprint = m_Blueprints.Find(b => b.inputIds[0] == drone.id);

                if (drones.Find(d => d.drone.id == drone.id) == null)
                {
                    // Get the same type drone from our local list and compare the levels.
                    // If the level is higher, add it to the list and remove the previous one.
                    var sameTypeDrone = drones.Find(d => d.drone.name == drone.name);
                    if (sameTypeDrone != null)
                    {
                        if (drone.level <= sameTypeDrone.drone.level) continue;
                        drones.Remove(sameTypeDrone);
                        drones.Add(new DroneWithBlueprint
                        {
                            drone = drone,
                            blueprint = blueprint
                        });
                    }
                    else
                    {
                        drones.Add(new DroneWithBlueprint
                        {
                            drone = drone,
                            blueprint = blueprint
                        });
                    }
                }
                else
                {
                    Debug.Log($"Drone with id {drone.id} already exists in the list.");
                }
            }

            return drones;
        }

        public List<LevelUpItem> GetLevelUpItems(DroneWithBlueprint drone)
        {
            var levelUpItems = new List<LevelUpItem>();
            
            if (drone.drone.level == Constants.k_DroneMaxLevel)
            {
                Debug.Log("Drone is already at max level.");
                return levelUpItems;
            }
            
            var blueprint = drone.blueprint;
            
            for (var i = 0; i < blueprint.inputIds.Length; i++)
            {
                var inputId = blueprint.inputIds[i];
                var inputAmount = blueprint.inputAmounts[i];
                var resource = m_Resources.Find(r => r.id == inputId);

                if (resource == null) continue;
                
                var resourceAmount = m_AccountTokens.Find(t => t.tokenId == inputId).amount ?? "0";

                levelUpItems.Add(new LevelUpItem
                {
                    name = resource.name,
                    id = resource.id,
                    image = resource.image,
                    accountAmount = int.Parse(resourceAmount),
                    requiredAmount = int.Parse(inputAmount)
                });
            }
            
            return levelUpItems;
        }

        public async Task<Receipt> LevelUpDrone(DroneWithBlueprint drone)
        {
            var blueprint = drone.blueprint;

            var receipt = await ContractInteraction.Craft(blueprint.blueprintId);
            OnLevelUpDrone?.Invoke(receipt);
            GetAccountTokens();
            return receipt;
        }

        private DroneType GetDroneTypeById(string id)
        {
            var allDrones = m_GameData.drones.chainsaw.Concat(m_GameData.drones.maus)
                .Concat(m_GameData.drones.rooster).Concat(
                    m_GameData.drones.carlson).ToList();

            var drone = allDrones.Find(d => d.id == id);

            if (drone != null) return (DroneType)Enum.Parse(typeof(DroneType), drone.name);
            return DroneType.Empty;
        }

        public class Resource
        {
            public string name { get; set; }
            public string id { get; set; }
            public string image { get; set; }
        }
        
        public class LevelUpItem
        {
            public string name { get; set; }
            public string id { get; set; }
            public string image { get; set; }
            public int accountAmount { get; set; }
            public int requiredAmount { get; set; }
        }

        public class Drone
        {
            public string name { get; set; }
            public int level { get; set; }
            public string id { get; set; }
            public string image { get; set; }
            public float hitpoints { get; set; }
            public float damage { get; set; }
            public string reload { get; set; }
            public float distance { get; set; }
            public float speed { get; set; }
            public int energy { get; set; }
        }

        public class DroneWithBlueprint
        {
            public Drone drone { get; set; }
            public Blueprint blueprint { get; set; }
        }

        public class Blueprint
        {
            public string[] inputIds { get; set; }
            public string[] inputAmounts { get; set; }
            public string[] outputIds { get; set; }
            public string[] outputAmounts { get; set; }
            public string blueprintId { get; set; }
        }

        public class Cards
        {
            public List<Drone> chainsaw { get; set; }
            public List<Drone> rooster { get; set; }
            public List<Drone> maus { get; set; }
            public List<Drone> carlson { get; set; }
        }

        public class GameData
        {
            public List<Resource> resources { get; set; }
            public Cards drones { get; set; }
        }

        public enum DroneType
        {
            Empty,
            Chainsaw,
            Rooster,
            Maus,
            Carlson
        }
    }
}