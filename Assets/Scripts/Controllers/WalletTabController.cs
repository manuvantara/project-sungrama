using System;
using Game.Managers;
using GameWallet.Managers;
using Thirdweb;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Controllers
{
    public class WalletTabController : MonoBehaviour
    {
        [SerializeField] private PageSwitch m_PageSwitchScript;
        [SerializeField] private TMP_Text m_AccountBalanceText;
        [SerializeField] private TMP_InputField m_RecipientAddressInputField;
        [SerializeField] private TMP_InputField m_AmountInputField;
        [SerializeField] private Toggle m_AgreeToggle;
        [SerializeField] private Button m_ProceedButton;
        [SerializeField] private TMP_Text m_AmountToTransferText;
        [SerializeField] private TMP_Text m_AddressToTransferText;
        [SerializeField] private TMP_InputField m_PasswordInputField;
        [SerializeField] private Button m_SendButton;

        private void OnEnable()
        {
            EventManager.OnTransactionConfirmed += OnTransactionConfirmed;

            m_RecipientAddressInputField.onValueChanged.AddListener(OnRecipientAddressInputFieldChanged);
            m_AmountInputField.onValueChanged.AddListener(OnAmountInputFieldChanged);
            m_AgreeToggle.onValueChanged.AddListener(OnAgreeToggleChanged);
            m_ProceedButton.onClick.AddListener(OnProceedButtonClicked);
            m_PasswordInputField.onEndEdit.AddListener(OnPasswordInputFieldEndEdit);
            m_SendButton.onClick.AddListener(OnSendButtonClicked);
            m_SendButton.interactable = false;
        }

        private async void Start()
        {
            var balance = await ThirdwebManager.Instance.SDK.wallet.GetBalance();
            m_AccountBalanceText.text = $"{balance.displayValue} {balance.symbol}";
        }

        private void OnDisable()
        {
            m_RecipientAddressInputField.onValueChanged.RemoveListener(OnRecipientAddressInputFieldChanged);
            m_AmountInputField.onValueChanged.RemoveListener(OnAmountInputFieldChanged);
            m_AgreeToggle.onValueChanged.RemoveListener(OnAgreeToggleChanged);
            m_ProceedButton.onClick.RemoveListener(OnProceedButtonClicked);
            m_PasswordInputField.onEndEdit.RemoveListener(OnPasswordInputFieldEndEdit);
            m_SendButton.onClick.RemoveListener(OnSendButtonClicked);
        }

        private async void OnTransactionConfirmed()
        {
            m_RecipientAddressInputField.text = "";
            m_AmountInputField.text = "";
            m_PasswordInputField.text = "";
            m_SendButton.interactable = false;

            var balance = await ThirdwebManager.Instance.SDK.wallet.GetBalance();
            m_AccountBalanceText.text = $"{balance.displayValue} {balance.symbol}";
        }

        private void OnRecipientAddressInputFieldChanged(string value)
        {
            m_AddressToTransferText.text = value;
        }

        private void OnAmountInputFieldChanged(string value)
        {
            m_AmountToTransferText.text = value;
        }

        private void OnAgreeToggleChanged(bool value)
        {
            m_ProceedButton.interactable = value;
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
                m_SendButton.interactable = true;
            }
            catch (Exception e)
            {
                m_SendButton.interactable = false;
            }
        }

        private async void OnSendButtonClicked()
        {
            TransactionResult receipt;

            UIController.ShowPending();
            EventManager.TransactionSent();

            try
            {
                receipt = await ThirdwebManager.Instance.SDK.wallet.Transfer(
                    m_RecipientAddressInputField.text,
                    m_AmountInputField.text
                );

                UIController.ShowSuccess();
                m_PageSwitchScript.SwitchPage(0);
                EventManager.TransactionConfirmed();
            }
            catch (Exception e)
            {
                receipt = new TransactionResult();

                UIController.ShowFail();
                m_PageSwitchScript.SwitchPage(0);
                // EventManager.Instance.TransactionFailed();
            }

            Debug.Log(receipt);
        }

        public async void CopyAddressToClipboard()
        {
            GUIUtility.systemCopyBuffer = await ThirdwebManager.Instance.SDK.wallet.GetAddress();
        }
    }
}