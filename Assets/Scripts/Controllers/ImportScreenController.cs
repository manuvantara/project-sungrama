using System;
using GameWallet.Managers;
using GameWallet.Screens;
using UnityEngine;

namespace GameWallet.UI.Controllers
{
    public class ImportScreenController: MonoBehaviour
    {
        public static event Action ImportedMnemonicValid;
        public static event Action ImportedMnemonicInvalid;
        
        private void OnEnable()
        {
            ImportScreen.ImportScreenShown += OnImportScreenShown;
            ImportScreen.ImportButtonClicked += OnImportButtonClicked;
            ImportScreen.ProceedToSetPasswordScreenClicked += OnProceedToSetPasswordScreenClicked;
            ImportScreen.ProceedToCreateWalletScreenClicked += OnProceedToCreateWalletScreenClicked;
        }
        
        private void OnDisable()
        {
            ImportScreen.ImportScreenShown -= OnImportScreenShown;
            ImportScreen.ImportButtonClicked -= OnImportButtonClicked;
            ImportScreen.ProceedToSetPasswordScreenClicked -= OnProceedToSetPasswordScreenClicked;
            ImportScreen.ProceedToCreateWalletScreenClicked -= OnProceedToCreateWalletScreenClicked;
        }

        private void OnImportScreenShown()
        {
            // If we need to do something when the screen is shown
        }
        
        private void OnImportButtonClicked(string mnemonic)
        {
            if (new NBitcoin.Mnemonic(mnemonic).IsValidChecksum == false)
            {
                ImportedMnemonicInvalid?.Invoke();
                return;
            }
            
            WalletManager.Instance.Mnemonic = mnemonic;
            ImportedMnemonicValid?.Invoke();
        }
        
        private void OnProceedToSetPasswordScreenClicked()
        {
            // If we need to do something when the proceed to set password button is clicked
        }
        
        private void OnProceedToCreateWalletScreenClicked()
        {
            // If we need to do something when the proceed to create wallet button is clicked
        }
    }
}