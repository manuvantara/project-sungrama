using System.Collections.Generic;
using Game.Managers;
using GameWallet.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Game.Controllers
{
    public class SettingsController : MonoBehaviour
    {
        [SerializeField] private GameObject m_SettingsPanel;
        [SerializeField] private TMP_InputField m_PasswordInputField;
        [SerializeField] private Button m_RevealMnemonicButton;
        [SerializeField] private PageSwitch m_PageSwitchScript;

        private readonly List<TMP_InputField> m_LeftPanelFields = new List<TMP_InputField>();
        private readonly List<TMP_InputField> m_RightPanelFields = new List<TMP_InputField>();

        private void Awake()
        {
            var leftPanel = m_SettingsPanel.transform.GetChild(0).gameObject;
            var rightPanel = m_SettingsPanel.transform.GetChild(1).gameObject;

            GetSettingsPanelInputs(leftPanel, rightPanel);
        }

        private void OnEnable()
        {
            EventManager.OnRevealMnemonicClicked += OnRevealMnemonic;

            m_RevealMnemonicButton.onClick.AddListener(() =>
            {
                EventManager.RevealMnemonicClicked(m_PasswordInputField.text);
                m_PageSwitchScript.SwitchPage(0);
            });
        }

        private void OnDisable()
        {
            EventManager.OnRevealMnemonicClicked -= OnRevealMnemonic;
        }

        private void GetSettingsPanelInputs(GameObject leftPanel, GameObject rightPanel)
        {
            foreach (RectTransform child in leftPanel.transform)
            {
                var inputField = child.GetComponent<TMP_InputField>();
                if (inputField != null)
                {
                    m_LeftPanelFields.Add(inputField);
                }
            }

            foreach (RectTransform child in rightPanel.transform)
            {
                var inputField = child.GetComponent<TMP_InputField>();
                if (inputField != null)
                {
                    m_RightPanelFields.Add(inputField);
                }
            }
        }

        private void OnRevealMnemonic(string password)
        {
            if (!WalletManager.Instance)
            {
                Debug.LogError("WalletManager instance not found. Did you run the wallet scene?");
                return;
            }

            // Here, we're going to set mnemonic words as text in the input fields
            var mnemonic = WalletManager.Instance.LoadWalletMnemonicFromJsonFile(password).Split(' ');

            for (var i = 0; i < m_LeftPanelFields.Count; i++)
            {
                m_LeftPanelFields[i].text = mnemonic[i];
            }

            for (var i = 0; i < m_RightPanelFields.Count; i++)
            {
                m_RightPanelFields[i].text = mnemonic[i + 6];
            }
        }
        
        public void RemoveWallet()
        {
            if (!WalletManager.Instance)
            {
                Debug.LogError("WalletManager instance not found. Did you run the wallet scene?");
                return;
            }
            
            WalletManager.Instance.DeleteWalletMnemonicJsonFile();
            SceneManager.LoadScene("Wallet");
        }
    }
}