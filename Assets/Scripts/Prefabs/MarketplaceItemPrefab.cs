using System.Numerics;
using Game.ContractInteractions;
using Game.Managers;
using Game.Types;
using Thirdweb;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Prefabs
{
    public class MarketplaceItemPrefab : MonoBehaviour
    {
        [Header("UI Elements")] public Toggle m_ItemToggle;
        public TMP_Text m_ItemName;
        public Image m_ItemImage;
        public Button m_ItemBuyButton;

        public void Init(Token token, ToggleGroup toggleGroup)
        {
            m_ItemName.text = token.tokenName;
            m_ItemImage.sprite = Resources.Load<Sprite>(token.image);
            m_ItemBuyButton.onClick.RemoveAllListeners();
            m_ItemBuyButton.onClick.AddListener(() => BuyItem(token));

            m_ItemToggle.group = toggleGroup;
            m_ItemToggle.onValueChanged.AddListener(isOn =>
            {
                m_ItemName.color = isOn ? new Color(0.9882353f, 0.9333333f, 0.03529412f) : new Color(0.6f, 0.6f, 0.6f);
                m_ItemImage.color = isOn ? new Color(0.9882353f, 0.9333333f, 0.03529412f) : Color.white;
            });
        }

        private async void BuyItem(Token token)
        {
            UIController.ShowPending();
            try
            {
                var receipt = await ContractInteraction.MintBatch(new BigInteger[] { BigInteger.Parse(token.tokenId) },
                    new BigInteger[] { 1 });
                UIController.ShowSuccess();
                EventManager.MarketplaceItemBought();
                Debug.Log(receipt);
            }
            catch
            {
                UIController.ShowFail();
            }
        }
    }
}