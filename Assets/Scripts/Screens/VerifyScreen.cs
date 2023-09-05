using System;
using GameWallet.Helpers;
using GameWallet.Base;
using GameWallet.UI.Controllers;
using UnityEngine.UIElements;

namespace GameWallet.Screens
{
    public class VerifyScreen: BaseScreen
    {
        #region Events
        
        public static event Action<string> WordVerified;
        public static event Action VerifyScreenShown;
        public static event Action ProceedToSetPasswordScreenClicked;
        
        #endregion
        
        #region Constants
        
        private const string k_VerifyWordLabel = "verify-word__label";
        private const string k_VerifyWordInput = "verify-word__input";
        private const string k_VerifySecurityToggle = "verify-security__toggle";
        private const string k_VerifyButtonTemplate = "verify-button__template";
        private const string k_VerifyButton = "button";
        
        #endregion

        #region Fields

        private string m_MnemonicWord;
        private Label m_VerifyWordLabel;
        private TextField m_VerifyWordInput;
        private Toggle m_VerifySecurityToggle;
        private Button m_VerifyButton;
        private bool m_IsMnemonicWordVerified;
        private bool m_IsSecurityChecked;

        #endregion
        
        #region Unity Lifecycle

        private void OnEnable()
        {
            VerifyScreenController.WordToVerifySelected += OnWordToVerifySelected;
        }

        private void Start()
        {
            // Disabling the button on start
            m_VerifyButton.SetEnabled(false);
        }

        private void OnDisable()
        {
            VerifyScreenController.WordToVerifySelected -= OnWordToVerifySelected;
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
        }
        
        #endregion

        #region Initialization

        private void GetVisualElements()
        {
            m_VerifyWordLabel = m_Root.Q<Label>(k_VerifyWordLabel);
            m_VerifyWordInput = m_Root.Q<TextField>(k_VerifyWordInput);
            m_VerifySecurityToggle = m_Root.Q<Toggle>(k_VerifySecurityToggle);
            
            var buttonTemplate = m_Root.Q<TemplateContainer>(k_VerifyButtonTemplate);
            m_VerifyButton = buttonTemplate.Q<Button>(k_VerifyButton);
        }
        
        private void RegisterInputCallback()
        {
            m_VerifyWordInput.RegisterCallback<ChangeEvent<string>>(VerifyMnemonicWord);
        }
        
        private void RegisterToggleCallback()
        {
            m_VerifySecurityToggle.RegisterCallback<ChangeEvent<bool>>(ToggleSecurityCheck);
        }
        
        private void RegisterButtonCallback()
        {
            m_VerifyButton.RegisterCallback<ClickEvent>(ProceedToSetPasswordScreen);
        }
        
        #endregion

        #region Callbacks
        
        private void VerifyMnemonicWord(ChangeEvent<string> evt)
        {
            var mnemonicWord = evt.newValue;

            UIHelpers.UpdateButtonState(m_VerifyButton, mnemonicWord == m_MnemonicWord && m_IsSecurityChecked);

            if (mnemonicWord != m_MnemonicWord) return;
            m_IsMnemonicWordVerified = true;
            WordVerified?.Invoke(mnemonicWord);
        }
        
        private void ToggleSecurityCheck(ChangeEvent<bool> evt)
        {
            m_IsSecurityChecked = evt.newValue;

            UIHelpers.UpdateButtonState(m_VerifyButton, m_IsSecurityChecked && m_IsMnemonicWordVerified);
        }

        private void ProceedToSetPasswordScreen(ClickEvent evt)
        {
            m_MainUIManager.ShowSetPasswordScreen();
            ProceedToSetPasswordScreenClicked?.Invoke();
        }
        
        #endregion

        #region Navigation

        public override void ShowScreen()
        {
            base.ShowScreen();
            
            VerifyScreenShown?.Invoke();
        }

        #endregion

        #region Event Handlers 

        private void OnWordToVerifySelected(int wordNumber, string mnemonicWord)
        {
            m_VerifyWordLabel.text = $"Word #{wordNumber + 1}";
            m_MnemonicWord = mnemonicWord;
        }

        #endregion
    }
}