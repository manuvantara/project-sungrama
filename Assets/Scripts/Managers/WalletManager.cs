using System;
using System.IO;
using NBitcoin;
using Newtonsoft.Json;
using Rijndael256;
using UnityEngine;

namespace GameWallet.Managers
{
    public class WalletManager : MonoBehaviour
    {
        public static WalletManager Instance { get; private set; }

        public string Mnemonic { get; set; }

        [SerializeField] private string m_WalletJsonPath;

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

            Mnemonic = new Mnemonic(Wordlist.English, WordCount.Twelve).ToString();
        }

        public string ExportWalletMnemonic()
        {
            return Mnemonic;
        }

        public void RegenerateWalletMnemonic()
        {
            Mnemonic = new Mnemonic(Wordlist.English, WordCount.Twelve).ToString();
        }

        public void SaveWalletMnemonicToJsonFile(string mnemonic, string password)
        {
            var encryptedMnemonic = Rijndael.Encrypt(mnemonic, password, KeySize.Aes256);
            var date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            var walletJsonData = new { encryptedWords = encryptedMnemonic, date = date };
            var walletJson = JsonConvert.SerializeObject(walletJsonData);
            Debug.Log(walletJson);

            File.WriteAllText(Application.persistentDataPath + "/" + m_WalletJsonPath, walletJson);
        }

        public string LoadWalletMnemonicFromJsonFile(string password)
        {
            var mnemonic = string.Empty;

            var walletJson = File.ReadAllText(Application.persistentDataPath + "/" + m_WalletJsonPath);
            var walletJsonData = JsonConvert.DeserializeObject<dynamic>(walletJson);
            string encryptedMnemonic = walletJsonData.encryptedWords;

            mnemonic = Rijndael.Decrypt(encryptedMnemonic, password, KeySize.Aes256);

            return mnemonic;
        }

        public bool WalletMnemonicJsonFileExists()
        {
            return File.Exists(Application.persistentDataPath + "/" + m_WalletJsonPath);
        }
        
        public void DeleteWalletMnemonicJsonFile()
        {
            File.Delete(Application.persistentDataPath + "/" + m_WalletJsonPath);
        }
    }
}