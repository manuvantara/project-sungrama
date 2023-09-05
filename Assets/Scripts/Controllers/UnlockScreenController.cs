using System;
using System.Collections.Generic;
using GameWallet.Managers;
using GameWallet.Screens;
using Thirdweb;
using UnityEngine;

namespace GameWallet.UI.Controllers
{
    public class UnlockScreenController : MonoBehaviour
    {
        public static event Action<List<Sprite>> IconsLoaded;
        public static event Action<bool> IsPasswordCorrect;
        public static event Action<bool> SuccessfullyConnected;

        [Header("Input Icons")] [Tooltip("Icon to show when input is visible")] [SerializeField]
        private Sprite m_InputShowIcon;

        [Tooltip("Icon to show when input is hidden")] [SerializeField]
        private Sprite m_InputHideIcon;

        private readonly List<Sprite> m_Icons = new List<Sprite>();

        private void Awake()
        {
            m_Icons.Add(m_InputShowIcon);
            m_Icons.Add(m_InputHideIcon);
        }

        private void Start()
        {
            IconsLoaded?.Invoke(m_Icons);
        }

        private void OnEnable()
        {
            UnlockScreen.UnlockButtonClicked += OnUnlockButtonClicked;
        }

        private void OnDisable()
        {
            UnlockScreen.UnlockButtonClicked -= OnUnlockButtonClicked;
        }

        private async void OnUnlockButtonClicked(string password)
        {
            var mnemonic = string.Empty;
            
            try
            {
                mnemonic = WalletManager.Instance.LoadWalletMnemonicFromJsonFile(password);
                WalletManager.Instance.Mnemonic = mnemonic;
                
                var connection = new WalletConnection(
                    WalletProvider.LocalHdWallet,
                    chainId: ThirdwebManager.Instance.SDK.session.ChainId
                );
                
                await ThirdwebManager.Instance.SDK.wallet.Connect(
                    connection
                );
                
                IsPasswordCorrect?.Invoke(true);
                SuccessfullyConnected?.Invoke(true);
            }
            catch (Exception e)
            {
                Debug.Log(e);
                IsPasswordCorrect?.Invoke(false);
                SuccessfullyConnected?.Invoke(false);
            }
        }
    }
}