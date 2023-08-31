using System;
using UnityEngine;

namespace Game.Managers
{
    public class EventManager: MonoBehaviour
    {
        public event Action OnTransactionSent;
        public event Action OnTransactionConfirmed;
        
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
        
        public void TransactionSent()
        {
            OnTransactionSent?.Invoke();
        }
        
        public void TransactionConfirmed()
        {
            OnTransactionConfirmed?.Invoke();
        }
    }
}