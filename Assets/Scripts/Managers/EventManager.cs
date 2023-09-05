using System;
using UnityEngine;

namespace Game.Managers
{
    public class EventManager : MonoBehaviour
    {
        public static event Action OnTransactionSent;
        public static event Action OnTransactionConfirmed;
        public static event Action OnMarketplaceItemBought;
        public static event Action<string> OnRevealMnemonicClicked;

        public static EventManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                Debug.LogWarning("Two EventManager instances were found, removing this one.");
                Destroy(this.gameObject);
                return;
            }
        }

        public static void TransactionSent()
        {
            OnTransactionSent?.Invoke();
        }

        public static void TransactionConfirmed()
        {
            OnTransactionConfirmed?.Invoke();
        }

        public static void MarketplaceItemBought()
        {
            OnMarketplaceItemBought?.Invoke();
        }
        
        public static void RevealMnemonicClicked(string password)
        {
            OnRevealMnemonicClicked?.Invoke(password);
        }
    }
}