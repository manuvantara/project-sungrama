using System;
using Game.ContractInteractions;
using Game.Types;
using GameWallet.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Controllers
{
    public class TokenTransferController : MonoBehaviour
    {
        [SerializeField] private PageSwitch m_PageSwitchScript;

        [Header("UI Elements")] [SerializeField]
        private TMP_InputField m_ReceiverAddressInputField;

        [SerializeField] private Toggle m_ConfirmationToggle;
        [SerializeField] private TMP_Text m_TokenNameText;
        [SerializeField] private TMP_Text m_ReceivingAddressText;
        [SerializeField] private TMP_InputField m_passwordInputField;
        [SerializeField] private Button m_SendButton;

        private Token m_SelectedToken;

        public static TokenTransferController Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Debug.LogWarning("Two TokenTransferController instances were found, removing this one.");
                Destroy(this.gameObject);
                return;
            }
        }

        private void OnEnable()
        {
            m_TokenNameText.text = m_SelectedToken.tokenName;

            m_ReceiverAddressInputField.onValueChanged.AddListener(OnReceiverAddressInputFieldChanged);
            m_ConfirmationToggle.onValueChanged.AddListener(OnConfirmationToggleChanged);
            m_passwordInputField.onEndEdit.AddListener(OnPasswordInputFieldEndEdit);
            m_SendButton.onClick.AddListener(OnSendButtonClicked);
            m_SendButton.interactable = false;
        }

        private void OnDisable()
        {
            m_ReceiverAddressInputField.onValueChanged.RemoveListener(OnReceiverAddressInputFieldChanged);
            m_ConfirmationToggle.onValueChanged.RemoveListener(OnConfirmationToggleChanged);
            m_passwordInputField.onEndEdit.RemoveListener(OnPasswordInputFieldEndEdit);
            m_SendButton.onClick.RemoveListener(OnSendButtonClicked);
        }

        public void SelectToken(Token token)
        {
            m_SelectedToken = token;
        }

        private void OnReceiverAddressInputFieldChanged(string value)
        {
            m_ReceivingAddressText.text = value;
            m_SendButton.interactable = !string.IsNullOrEmpty(value) && m_ConfirmationToggle.isOn;
        }

        private void OnConfirmationToggleChanged(bool value)
        {
            m_SendButton.interactable = !string.IsNullOrEmpty(m_ReceiverAddressInputField.text) && value;
        }

        private void OnPasswordInputFieldEndEdit(string value)
        {
            try
            {
                // We don't save the password itself, but we save the mnemonic encrypted with the password
                // So if the user can decrypt the mnemonic with the password, then the password is correct
                WalletManager.Instance.LoadWalletMnemonicFromJsonFile(value);
                m_SendButton.interactable = true;
            }
            catch (Exception e)
            {
                m_SendButton.interactable = false;
            }
        }

        private async void OnSendButtonClicked()
        {
            UIController.ShowPending();

            try
            {
                var receipt = await ContractInteraction.Transfer(
                    m_ReceiverAddressInputField.text,
                    m_SelectedToken.tokenId,
                    1
                );

                Debug.Log(receipt);

                m_ReceiverAddressInputField.text = "";
                m_ConfirmationToggle.isOn = false;
                m_passwordInputField.text = "";
                m_SendButton.interactable = false;

                UIController.ShowSuccess();
                m_PageSwitchScript.SwitchPage(0);
            }
            catch (Exception e)
            {
                UIController.ShowFail();
                Debug.Log(e);
            }
        }
    }
}