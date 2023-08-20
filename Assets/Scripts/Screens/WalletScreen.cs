using GameWallet.Base;
using Nethereum.Web3.Accounts;
using UnityEngine.UIElements;

namespace GameWallet.Screens
{
    public class WalletScreen : BaseScreen
    {
        private Account m_Account;
        private Label m_AddressLabel;

        private void OnEnable()
        {
            SetPasswordScreen.ProceedToWalletScreenClicked += OnProceedToWalletScreenClicked;
        }
        
        private void OnDisable()
        {
            SetPasswordScreen.ProceedToWalletScreenClicked -= OnProceedToWalletScreenClicked;
        }

        protected override void SetVisualElements()
        {
            base.SetVisualElements();

            m_AddressLabel = m_Root.Q<Label>("address");
        }
        
        private async void OnProceedToWalletScreenClicked()
        {
            var address = await ThirdwebManager.Instance.SDK.wallet.GetAddress();
            m_AddressLabel.text = address;
        }
    }
}