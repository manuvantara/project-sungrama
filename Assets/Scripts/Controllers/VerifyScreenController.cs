using System;
using GameWallet.Managers;
using GameWallet.Screens;
using UnityEngine;

namespace GameWallet.UI.Controllers
{
    public class VerifyScreenController: MonoBehaviour
    {
        public static event Action<int, string> WordToVerifySelected;
        
        private void OnEnable()
        {
            CreateScreen.ProceedToVerifyScreenClicked += OnProceedToVerifyScreenClicked;
        }
        
        private void OnDisable()
        {
            CreateScreen.ProceedToVerifyScreenClicked -= OnProceedToVerifyScreenClicked;
        }
        
        private void OnProceedToVerifyScreenClicked()
        {
           var randomNumber = UnityEngine.Random.Range(0, 12);
           var mnemonicWord = WalletManager.Instance.Mnemonic.Split(' ')[randomNumber];
           
           WordToVerifySelected?.Invoke(randomNumber, mnemonicWord);
        } 
    }
}