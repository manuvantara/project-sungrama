using System.Collections.Generic;
using Game.Managers;
using Game.Prefabs;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private Transform m_contentParent;
    [SerializeField] private TokenPrefab m_tokenPrefab;
    // [SerializeField] private GameObject m_loadingPanel;
    private List<Token> m_AccountTokens;

    private void Awake()
    {
        m_AccountTokens = GameDataManager.Instance.AccountTokens;
    }

    private void Start()
    {
        foreach (Transform child in m_contentParent)
            Destroy(child.gameObject);
        
        if (!Application.isPlaying)
            return;
        
        foreach (var token in m_AccountTokens)
        {
            var tokenPrefab = Instantiate(m_tokenPrefab, m_contentParent);
            tokenPrefab.Init(token);
        }
    }
}
