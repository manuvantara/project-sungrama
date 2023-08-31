using Game.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Prefabs
{
    public class TokenPrefab: MonoBehaviour
    {
        [Header("UI Elements")]
        public TMP_Text tokenName;
        public TMP_Text tokenAmount;
        public Image tokenImage;
        public TMP_Text selectedTokenName;
        public Button tokenButton;
        
        public async void Init(Token token)
        {
            tokenName.text = token.tokenName;
            tokenAmount.text = token.amount;
            // tokenImage.sprite = await ThirdwebManager.Instance.SDK.storage.DownloadImage(token.image);
            selectedTokenName.text = token.tokenName;
            // tokenButton.onClick.RemoveAllListeners();
            // tokenButton.onClick.AddListener(() => SelectToken(token));
        }
    }
}