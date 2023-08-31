using System;
using Game.Managers;
using GameWallet.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Controllers
{
    public class SendBNBController : MonoBehaviour
    {
        [SerializeField] private TMP_InputField m_recipientAddressInputField;
        [SerializeField] private TMP_InputField m_amountInputField;
        [SerializeField] private Toggle m_agreeToggle;
        [SerializeField] private Button m_proceedButton;
        [SerializeField] private TMP_Text m_amountToTransferText;
        [SerializeField] private TMP_Text m_addressToTransferText;
        [SerializeField] private TMP_InputField m_passwordInputField;
        [SerializeField] private Button m_sendButton;
        
        private void OnEnable()
        {
            m_recipientAddressInputField.onValueChanged.AddListener(OnRecipientAddressInputFieldChanged);
            m_amountInputField.onValueChanged.AddListener(OnAmountInputFieldChanged);
            m_agreeToggle.onValueChanged.AddListener(OnAgreeToggleChanged);
            m_proceedButton.onClick.AddListener(OnProceedButtonClicked);
            m_passwordInputField.onEndEdit.AddListener(OnPasswordInputFieldEndEdit);
            m_sendButton.onClick.AddListener(OnSendButtonClicked);
            m_sendButton.interactable = false;
        }

        private void OnDisable()
        {
            m_recipientAddressInputField.onValueChanged.RemoveListener(OnRecipientAddressInputFieldChanged);
            m_amountInputField.onValueChanged.RemoveListener(OnAmountInputFieldChanged);
            m_agreeToggle.onValueChanged.RemoveListener(OnAgreeToggleChanged);
            m_proceedButton.onClick.RemoveListener(OnProceedButtonClicked);
            m_passwordInputField.onEndEdit.RemoveListener(OnPasswordInputFieldEndEdit);
            m_sendButton.onClick.RemoveListener(OnSendButtonClicked);
        }

        private void OnRecipientAddressInputFieldChanged(string value)
        {
            m_addressToTransferText.text = value;
        }

        private void OnAmountInputFieldChanged(string value)
        {
            m_amountToTransferText.text = value;
        }

        private void OnAgreeToggleChanged(bool value)
        {
            m_proceedButton.interactable = value;
        }

        private void OnProceedButtonClicked()
        {
            // If we need to do something when the proceed button is clicked
        }

        private void OnPasswordInputFieldEndEdit(string value)
        {
            try
            {
                // We don't save the password itself, but we save the mnemonic encrypted with the password
                // So if the user can decrypt the mnemonic with the password, then the password is correct
                WalletManager.Instance.LoadWalletMnemonicFromJsonFile(value);
                m_sendButton.interactable = true;
            }
            catch (Exception e)
            {
                m_sendButton.interactable = false;
            }
        }

        private async void OnSendButtonClicked()
        {
            EventManager.Instance.TransactionSent();
            
            var receipt = await ThirdwebManager.Instance.SDK.wallet.Transfer(
                m_recipientAddressInputField.text,
                m_amountInputField.text
            );
            
            EventManager.Instance.TransactionConfirmed();
            
            Debug.Log(receipt);
        }
    }
}