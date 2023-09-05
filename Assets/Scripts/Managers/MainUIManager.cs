using System.Collections.Generic;
using GameWallet.Base;
using GameWallet.Screens;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameWallet.Managers
{
    
    // high-level manager for the various parts of the Main UI. Here we use one master UXML and one UIDocument.
    // We allow the individual parts of the user interface to have separate UIDocuments if needed (but not shown in this example).
    
    [RequireComponent(typeof(UIDocument))]
    public class MainUIManager : MonoBehaviour
    {
        [Header("Modal Menu Screens")]
        [Tooltip("Only one modal interface can appear on-screen at a time.")]
        [SerializeField] CreateScreen m_CreateModalScreen;
        [SerializeField] ImportScreen m_ImportModalScreen;
        [SerializeField] VerifyScreen m_VerifyModalScreen;
        [SerializeField] SetPasswordScreen m_SetPasswordModalScreen;
        [SerializeField] WalletScreen m_WalletModalScreen;
        [SerializeField] UnlockScreen m_UnlockModalScreen;
        
        List<BaseScreen> m_AllModalScreens = new List<BaseScreen>();
        
        UIDocument m_MainMenuDocument;
        public UIDocument MainMenuDocument => m_MainMenuDocument;
        
        void OnEnable()
        {
            m_MainMenuDocument = GetComponent<UIDocument>();
            SetupModalScreens();
        }

        void Start()
        {
            Time.timeScale = 1f;
            
            if (WalletManager.Instance.WalletMnemonicJsonFileExists())
            {
                ShowUnlockScreen();
            } else {
                ShowCreateScreen();
            }
        }

        void SetupModalScreens()
        {
            if (m_CreateModalScreen != null)
                m_AllModalScreens.Add(m_CreateModalScreen);

            if (m_ImportModalScreen != null)
                m_AllModalScreens.Add(m_ImportModalScreen);

            if (m_VerifyModalScreen != null)
                m_AllModalScreens.Add(m_VerifyModalScreen);

            if (m_SetPasswordModalScreen != null)
                m_AllModalScreens.Add(m_SetPasswordModalScreen);
            
            if (m_WalletModalScreen != null)
                m_AllModalScreens.Add(m_WalletModalScreen);
            
            if (m_UnlockModalScreen != null)
                m_AllModalScreens.Add(m_UnlockModalScreen);
        }

        // shows one screen at a time
        void ShowModalScreen(BaseScreen modalScreen)
        {
            foreach (BaseScreen m in m_AllModalScreens)
            {
                if (m == modalScreen)
                {
                    m?.ShowScreen();
                }
                else
                {
                    m?.HideScreen();
                }
            }
        }

        // methods to toggle screens on/off

        // modal screen methods 
        public void ShowCreateScreen()
        {
            ShowModalScreen(m_CreateModalScreen);
        }

        // note: screens with tabbed menus default to showing the first tab
        public void ShowImportScreen()
        {
            ShowModalScreen(m_ImportModalScreen);
        }

        public void ShowVerifyScreen()
        {
            ShowModalScreen(m_VerifyModalScreen);
        }

        public void ShowSetPasswordScreen()
        {
            ShowModalScreen(m_SetPasswordModalScreen);
        }
        
        public void ShowWalletScreen()
        {
            ShowModalScreen(m_WalletModalScreen);
        }

        public void ShowUnlockScreen()
        {
            ShowModalScreen(m_UnlockModalScreen);
        }

        // opens the Shop Screen directly to a specific tab (e.g. to gold or gem shop) from the Options Bar
        // public void ShowShopScreen(string tabName)
        // {
        //     m_MenuToolbar?.ShowShopScreen();
        //     m_SetPasswordModalScreen?.SelectTab(tabName);
        // }

        
    }
}

