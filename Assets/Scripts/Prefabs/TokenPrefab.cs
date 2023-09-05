using Game.Controllers;
using Game.Types;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Prefabs
{
    public class TokenPrefab : MonoBehaviour
    {
        [Header("UI Elements")] public Toggle m_TokenToggle;
        public TMP_Text m_TokenName;
        public TMP_Text m_TokenAmount;
        public TMP_Text m_TokenLevel;
        public Image m_TokenImage;
        public Button m_TokenButton;

        private PageSwitch m_PageSwitchScript;

        public void Init(Token token, PageSwitch pageSwitchScript, ToggleGroup toggleGroup)
        {
            m_PageSwitchScript = pageSwitchScript;

            m_TokenName.text = token.tokenName;
            m_TokenAmount.text = $"X{token.amount}";
            m_TokenLevel.text = token.metadata.level >= 1 ? $"Level {token.metadata.level.ToString()}" : "";
            m_TokenImage.sprite = Resources.Load<Sprite>(token.image);
            m_TokenButton.onClick.RemoveAllListeners();
            m_TokenButton.onClick.AddListener(() => SelectToken(token));

            m_TokenToggle.group = toggleGroup;
            m_TokenToggle.onValueChanged.AddListener(isOn =>
            {
                m_TokenName.color =
                    isOn ? new Color(0.9882353f, 0.9333333f, 0.03529412f) : new Color(0.6f, 0.6f, 0.6f);
            });
        }

        private void SelectToken(Token token)
        {
            m_PageSwitchScript.SwitchPage(1);
            TokenTransferController.Instance.SelectToken(token);
        }
    }
}