using Game.Managers;
using Game.Prefabs;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    [SerializeField] private Transform m_ContentParent;

    [SerializeField] private TokenPrefab m_TokenPrefab;

    // [SerializeField] private GameObject m_loadingPanel;
    [SerializeField] private PageSwitch m_PageSwitch;
    [SerializeField] private ToggleGroup m_ToggleGroup;

    private bool m_IsInitialized;
    private bool m_ShouldRefresh;

    private void Awake()
    {
        EventManager.OnMarketplaceItemBought += OnMarketplaceItemBought;
    }

    private void Start()
    {
        InitializeInventory();
    }

    private void OnEnable()
    {
        if (m_IsInitialized && m_ShouldRefresh)
        {
            InitializeInventory();
            m_ShouldRefresh = false;
        }
    }

    private void InitializeInventory()
    {
        foreach (RectTransform child in m_ContentParent)
        {
            Destroy(child.gameObject);
        }

        if (!Application.isPlaying)
            return;

        foreach (var token in GameDataManager.Instance.AccountTokens)
        {
            var tokenPrefab = Instantiate(m_TokenPrefab, m_ContentParent);
            tokenPrefab.Init(token, m_PageSwitch, m_ToggleGroup);
        }

        m_IsInitialized = true;
    }

    private void OnMarketplaceItemBought()
    {
        m_ShouldRefresh = true;
    }
}