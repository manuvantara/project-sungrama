using System;
using GameWallet.Base;
using GameWallet.Helpers;
using GameWallet.UI.Controllers;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameWallet.Screens
{
    public class ImportScreen : BaseScreen
    {
        #region Events

        public static event Action ImportScreenShown; 
        public static event Action<string> ImportButtonClicked;
        public static event Action ProceedToSetPasswordScreenClicked;
        public static event Action ProceedToCreateWalletScreenClicked;

        #endregion
        
        #region Constants

        private const string k_ImportInput = "import-form-control__input";
        private const string k_ImportInputInvalid = "import-form-control__input--invalid";
        private const string k_ImportTermsToggle = "import-terms__toggle";
        private const string k_ImportButtonTemplate = "import-button__template";
        private const string k_ImportLinkButton = "import-footer__link-button";

        #endregion

        #region Fields

        private TextField m_ImportInput;
        private Toggle m_ImportTermsToggle;
        private Button m_ImportButton;
        private Button m_ImportLinkButton;
        private string m_ImportedMnemonic;
        private bool m_IsTermsChecked;
        private bool m_IsMnemonicValid;

        #endregion

        #region Unity Lifecycle

        private void OnEnable()
        {
            ImportScreenController.ImportedMnemonicValid += OnImportedMnemonicValid;
            ImportScreenController.ImportedMnemonicInvalid += OnImportedMnemonicInvalid;
        }

        private void Start()
        {
            // Disabling the button on start
            m_ImportButton.SetEnabled(false);
        }
        
        private void OnDisable()
        {
            ImportScreenController.ImportedMnemonicValid -= OnImportedMnemonicValid;
            ImportScreenController.ImportedMnemonicInvalid -= OnImportedMnemonicInvalid;
        }

        protected override void SetVisualElements()
        {
            base.SetVisualElements();

            GetVisualElements();
        }

        protected override void RegisterButtonCallbacks()
        {
            base.RegisterButtonCallbacks();

            RegisterInputCallback();
            RegisterToggleCallback();
            RegisterButtonCallback();
            RegisterLinkButtonCallback();
        }

        #endregion

        #region Initialization

        private void GetVisualElements()
        {
            m_ImportInput = m_Root.Q<TextField>(k_ImportInput);
            m_ImportTermsToggle = m_Root.Q<Toggle>(k_ImportTermsToggle);
            var importButtonTemplate = m_Root.Q<VisualElement>(k_ImportButtonTemplate);
            m_ImportButton = importButtonTemplate.Q<Button>();
            m_ImportLinkButton = m_Root.Q<Button>(k_ImportLinkButton);
        }
        
        private void RegisterInputCallback()
        {
            m_ImportInput?.RegisterCallback<ChangeEvent<string>>(UpdateImportedMnemonic);
        }

        private void RegisterToggleCallback()
        {
            m_ImportTermsToggle?.RegisterCallback<ChangeEvent<bool>>(ToggleSecurityCheck);
        }

        private void RegisterButtonCallback()
        {
            m_ImportButton?.RegisterCallback<ClickEvent>(ProceedToSetPasswordScreen);
        }

        private void RegisterLinkButtonCallback()
        {
            m_ImportLinkButton?.RegisterCallback<ClickEvent>(ProceedToCreateScreen);
        }

        #endregion

        #region Callbacks

        private void UpdateImportedMnemonic(ChangeEvent<string> evt)
        {
            m_ImportedMnemonic = evt.newValue;
            
            UIHelpers.UpdateButtonState(m_ImportButton, m_ImportedMnemonic != null && m_IsTermsChecked);
        }

        private void ToggleSecurityCheck(ChangeEvent<bool> evt)
        {
            m_IsTermsChecked = evt.newValue;
            
            UIHelpers.UpdateButtonState(m_ImportButton, m_ImportedMnemonic != null && m_IsTermsChecked);
        }

        #endregion
        
        #region Navigation

        public override void ShowScreen()
        {
            base.ShowScreen();
            
            ImportScreenShown?.Invoke();
        }
        
        private void ProceedToSetPasswordScreen(ClickEvent evt)
        {
            ImportButtonClicked?.Invoke(m_ImportedMnemonic);

            if (!m_IsMnemonicValid) return;
            
            ProceedToSetPasswordScreenClicked?.Invoke();
            m_MainUIManager.ShowSetPasswordScreen();
        }
        
        private void ProceedToCreateScreen(ClickEvent evt)
        {
            m_MainUIManager.ShowCreateScreen();
            
            ProceedToCreateWalletScreenClicked?.Invoke();
        }

        #endregion
        
        #region Event Handlers
        
        private void OnImportedMnemonicValid()
        {
            m_IsMnemonicValid = true;
            Debug.Log("Imported mnemonic is valid");
            
            m_ImportInput.RemoveFromClassList(k_ImportInputInvalid);
        }
        
        private void OnImportedMnemonicInvalid()
        {
            m_IsMnemonicValid = false;
            Debug.LogWarning("Imported mnemonic is invalid");
            
            m_ImportInput.AddToClassList(k_ImportInputInvalid);
            
            m_ImportInput.value = string.Empty;
            m_ImportInput.Focus();
        }
        
        #endregion
    }
}