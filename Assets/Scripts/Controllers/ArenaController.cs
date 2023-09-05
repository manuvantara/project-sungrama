using System;
using System.Collections.Generic;
using Game.Managers;
using Game.Prefabs;
using Thirdweb;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Controllers
{
    public class ArenaController : MonoBehaviour
    {
        [SerializeField] private Transform m_EmptyAccountPanel;

        [Header("Drone List UI Elements")] [SerializeField]
        private Transform m_ContentParent;

        [SerializeField] private ToggleGroup m_ToggleGroup;
        [SerializeField] private ArenaItemPrefab m_ArenaItemPrefab;

        [Header("Stats UI Elements")] [SerializeField]
        private TMP_Text m_ArmorStatText;

        [SerializeField] private Slider m_ArmorStatSlider;

        [SerializeField] private TMP_Text m_DamageStatText;
        [SerializeField] private Slider m_DamageStatSlider;
        [SerializeField] private TMP_Text m_EnergyStatText;
        [SerializeField] private Slider m_EnergyStatSlider;

        [Header("Level Up UI Elements")] [SerializeField]
        private Transform m_LevelUpContentParent;

        [SerializeField] private LevelUpItemPrefab m_LevelUpItemPrefab;
        [SerializeField] private Button m_LevelUpButton;

        [Header("Drone Info Elements")] [SerializeField]
        private ToggleGroup m_DroneInfoToggleGroup;

        [SerializeField] private PageSwitch m_DroneInfoPageSwitch;

        private List<GameDataManager.DroneWithBlueprint> m_AccountDrones;
        private GameDataManager.DroneWithBlueprint m_SelectedDrone;

        public static ArenaController Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Debug.LogWarning("Two ArenaController instances were found, removing this one.");
                Destroy(this.gameObject);
                return;
            }
            
            GameDataManager.OnGetAccountTokensFinished += OnGetAccountTokensFinished;
            GameDataManager.OnLevelUpDrone += OnLevelUpDrone;
        }

        private void OnGetAccountTokensFinished()
        {
            m_AccountDrones = GameDataManager.Instance.GetDronesWithHighestLevel();
            
            if (m_AccountDrones == null || m_AccountDrones.Count == 0)
            {
                m_EmptyAccountPanel.gameObject.SetActive(true);
                // m_ContentParent.gameObject.SetActive(false);
            }
            else
            {
                m_EmptyAccountPanel.gameObject.SetActive(false);
                // m_ContentParent.gameObject.SetActive(true);
                InitializeDroneList();
            }
        }

        private void InitializeDroneList()
        {
            foreach (Transform child in m_ContentParent)
                Destroy(child.gameObject);

            if (!Application.isPlaying)
                return;

            foreach (var droneWithBlueprint in m_AccountDrones)
            {
                var itemPrefab = Instantiate(m_ArenaItemPrefab, m_ContentParent);
                itemPrefab.Init(droneWithBlueprint, m_ToggleGroup);
            }
        }

        private void InitializeLevelUpList(GameDataManager.DroneWithBlueprint drone)
        {
            foreach (Transform child in m_LevelUpContentParent)
                Destroy(child.gameObject);

            if (!Application.isPlaying)
                return;

            var levelUpItems = GameDataManager.Instance.GetLevelUpItems(drone);
            var canLevelUp = true;

            foreach (var levelUpItem in levelUpItems)
            {
                var itemPrefab = Instantiate(m_LevelUpItemPrefab, m_LevelUpContentParent);
                itemPrefab.Init(levelUpItem);
                if (levelUpItem.accountAmount < levelUpItem.requiredAmount)
                {
                    canLevelUp = false;
                }
            }

            UpdateButtonState(canLevelUp);
        }

        public void SelectItem(GameDataManager.DroneWithBlueprint droneWithBlueprint)
        {
            var drone = droneWithBlueprint.drone;
            m_SelectedDrone = droneWithBlueprint;

            m_ArmorStatText.text = drone.hitpoints.ToString("R");
            m_ArmorStatSlider.value = drone.hitpoints;
            m_DamageStatText.text = drone.damage.ToString("R");
            m_DamageStatSlider.value = drone.damage;

            m_EnergyStatText.text = drone.name switch
            {
                "Maus" => Constants.k_MausSpawnEnergy.ToString(),
                "Carlson" => Constants.k_CarlsonSpawnEnergy.ToString(),
                "Rooster" => Constants.k_RoosterSpawnEnergy.ToString(),
                "Chainsaw" => Constants.k_ChainsawSpawnEnergy.ToString(),
                _ => m_EnergyStatText.text
            };
            m_EnergyStatSlider.value = drone.name switch
            {
                "Maus" => Constants.k_MausSpawnEnergy,
                "Carlson" => Constants.k_CarlsonSpawnEnergy,
                "Rooster" => Constants.k_RoosterSpawnEnergy,
                "Chainsaw" => Constants.k_ChainsawSpawnEnergy,
                _ => m_EnergyStatSlider.value
            };

            InitializeLevelUpList(droneWithBlueprint);
        }

        private void OnLevelUpDrone(Receipt receipt)
        {
            GameDataManager.Instance.GetAccountTokens();

            m_ToggleGroup.GetFirstActiveToggle().onValueChanged.Invoke(true);
        }

        public async void LevelUp()
        {
            UIController.ShowPending();
            try
            {
                var receipt = await GameDataManager.Instance.LevelUpDrone(m_SelectedDrone);
                UIController.ShowSuccess();
                m_DroneInfoPageSwitch.SwitchPage(0);
                // Activate first toggle
                m_DroneInfoToggleGroup.SetAllTogglesOff();
                m_DroneInfoToggleGroup.transform.GetChild(0).GetComponent<Toggle>().onValueChanged.Invoke(true);

                Debug.Log(receipt);
            }
            catch (Exception e)
            {
                UIController.ShowFail();
                Debug.Log(e);
            }
        }

        private void UpdateButtonState(bool canLevelUp)
        {
            m_LevelUpButton.interactable = canLevelUp;
        }
    }
}