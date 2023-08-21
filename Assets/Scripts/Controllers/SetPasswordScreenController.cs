using System;
using System.Collections.Generic;
using GameWallet.Managers;
using GameWallet.Screens;
using Thirdweb;
using UnityEngine;

namespace GameWallet.UI.Controllers
{
    public class SetPasswordScreenController : MonoBehaviour
    {
        public static event Action<List<Sprite>> IconsLoaded;
        public static event Action<bool> SuccessfullyConnected;

        [Header("Input Icons")] [SerializeField]
        private Sprite m_InputShowIcon;

        [SerializeField] private Sprite m_InputHideIcon;

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
            SetPasswordScreen.SetPasswordButtonClicked += OnSetPasswordButtonClicked;
        }

        private void OnDisable()
        {
            SetPasswordScreen.SetPasswordButtonClicked -= OnSetPasswordButtonClicked;
        }

        private async void OnSetPasswordButtonClicked(string password)
        {
            var connection = new WalletConnection(
                WalletProvider.LocalHdWallet,
                chainId: ThirdwebManager.Instance.SDK.session.ChainId,
                password: password
            );

            try
            {
                await ThirdwebManager.Instance.SDK.wallet.Connect(
                    connection
                );
                
                WalletManager.Instance.SaveWalletMnemonicToJsonFile(
                    WalletManager.Instance.Mnemonic,
                    password
                );

                SuccessfullyConnected?.Invoke(true);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error connecting to wallet: {e.Message}, {e.StackTrace}");
                SuccessfullyConnected?.Invoke(false);
            }
        }
    }
}