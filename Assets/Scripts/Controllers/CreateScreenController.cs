using System;
using GameWallet.Managers;
using GameWallet.Screens;
using UnityEngine;

namespace GameWallet.UI.Controllers
{
    public class CreateScreenController: MonoBehaviour
    {
        public static event Action<string> MnemonicRegenerated;

        private void OnEnable()
        {
            CreateScreen.RegenerateMnemonicClicked += OnRegenerateMnemonicClicked;
            CreateScreen.CopyMnemonicClicked += OnCopyMnemonicClicked;
        }
        
        private void OnDisable()
        {
            CreateScreen.RegenerateMnemonicClicked -= OnRegenerateMnemonicClicked;
        }
        
        private void OnRegenerateMnemonicClicked()
        {
            WalletManager.Instance.RegenerateWalletMnemonic();   
            MnemonicRegenerated?.Invoke(WalletManager.Instance.Mnemonic);
        }
        
        private void OnCopyMnemonicClicked()
        {
            GUIUtility.systemCopyBuffer = WalletManager.Instance.Mnemonic;
        }
    }
}