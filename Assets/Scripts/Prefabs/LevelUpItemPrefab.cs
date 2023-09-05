using Game.Controllers;
using Game.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Prefabs
{
    public class LevelUpItemPrefab : MonoBehaviour
    {
        [Header("UI Elements")] [SerializeField]
        public Image m_Image;

        public TMP_Text m_NameText;
        public TMP_Text m_AccountAmountText;
        public TMP_Text m_DeliminatorText;
        public TMP_Text m_RequiredAmountText;

        public void Init(GameDataManager.LevelUpItem levelUpItem)
        {
            m_Image.sprite = Resources.Load<Sprite>(levelUpItem.image);
            m_NameText.text = levelUpItem.name;
            m_AccountAmountText.text = levelUpItem.accountAmount.ToString();
            m_RequiredAmountText.text = levelUpItem.requiredAmount.ToString();

            ValidateResourceAmount(levelUpItem);
        }

        private void ValidateResourceAmount(GameDataManager.LevelUpItem levelUpItem)
        {
            if (levelUpItem.accountAmount < levelUpItem.requiredAmount)
            {
                m_AccountAmountText.color = Color.red;
                m_DeliminatorText.color = Color.red;
                m_RequiredAmountText.color = Color.red;

                // ArenaController.CanLevelUp(false);
            }
            else
            {
                m_AccountAmountText.color = Color.white;
                m_DeliminatorText.color = Color.white;
                m_RequiredAmountText.color = Color.white;

                // ArenaController.CanLevelUp(true);
            }
        }
    }
}