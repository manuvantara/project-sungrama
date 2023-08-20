using System;
using NBitcoin;
using UnityEngine;

namespace GameWallet.Managers
{
    public class WalletManager: MonoBehaviour
    {
        public static WalletManager Instance { get; private set; }

        private string m_Mnemonic;
        
        public string Mnemonic => m_Mnemonic;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            
            m_Mnemonic = new Mnemonic(Wordlist.English, WordCount.Twelve).ToString();
        }

        public string ExportWalletMnemonic()
        {
            return m_Mnemonic;
        }
        
        public void RegenerateWalletMnemonic()
        {
            m_Mnemonic = new Mnemonic(Wordlist.English, WordCount.Twelve).ToString();
        }
        
        public void ImportWalletMnemonic(string mnemonic)
        {
            m_Mnemonic = mnemonic;
        }
    }
}