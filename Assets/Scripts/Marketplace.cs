using Game.ContractInteractions;
using Game.Managers;
using Game.Prefabs;
using UnityEngine;
using UnityEngine.UI;

public class Marketplace : MonoBehaviour
{
    [SerializeField] private Transform m_ContentParent;
    [SerializeField] private ToggleGroup m_ToggleGroup;
    [SerializeField] private MarketplaceItemPrefab m_ItemPrefab;
    // [SerializeField] private GameObject m_loadingPanel;

    private async void Start()
    {
        foreach (Transform child in m_ContentParent)
            Destroy(child.gameObject);

        if (!Application.isPlaying)
            return;

        foreach (var token in GameDataManager.Instance.MarketplaceTokens)
        {
            var itemPrefab = Instantiate(m_ItemPrefab, m_ContentParent);
            itemPrefab.Init(token, m_ToggleGroup);
        }

        await ContractInteraction.Approve();
    }
}