using System.Numerics;
using System.Threading.Tasks;
using GameWallet.Managers;
using NBitcoin;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;

namespace Thirdweb.Wallets
{
    public class LocalHdWallet: IThirdwebWallet
    {
        private Account _account;
        private Web3 _web3;
        private readonly WalletProvider _provider;
        private readonly WalletProvider _signerProvider;
        
        public LocalHdWallet()
        {
            _account = null;
            _web3 = null;
            _provider = WalletProvider.LocalHdWallet;
            _signerProvider = WalletProvider.LocalHdWallet;
        }
        
        public Task<string> Connect(WalletConnection walletConnection, string rpc)
        {
            var mnemonic = WalletManager.Instance.Mnemonic;
            _account = new Nethereum.HdWallet.Wallet(mnemonic, null).GetAccount(0);
            _web3 = new Web3(_account, rpc);
            return Task.FromResult(_account.Address);
        }

        public Task Disconnect()
        {
            _account = null;
            _web3 = null;
            return Task.CompletedTask;
        }

        public Account GetLocalAccount()
        {
            return _account;
        }

        public Task<string> GetAddress()
        {
            var addy = _account?.Address;
            if (addy != null)
                addy = addy.ToChecksumAddress();
            return Task.FromResult(addy);
        }

        public async Task<string> GetSignerAddress()
        {
            return await GetAddress();
        }

        public WalletProvider GetProvider()
        {
            return _provider;
        }

        public WalletProvider GetSignerProvider()
        {
            return _signerProvider;
        }

        public Task<Web3> GetWeb3()
        {
            return Task.FromResult(_web3);
        }

        public Task<Web3> GetSignerWeb3()
        {
            return Task.FromResult(_web3);
        }

        public Task<bool> IsConnected()
        {
            return Task.FromResult(_web3 != null);
        }

        public Task<NetworkSwitchAction> PrepareForNetworkSwitch(BigInteger newChainId, string newRpc)
        {
            _account = new Account(_account.PrivateKey, newChainId);
            _web3 = new Web3(_account, newRpc);
            return Task.FromResult(NetworkSwitchAction.Handled);
        }
    }
}