using System;
using System.Collections.Generic;
using GameWallet.Base;
using GameWallet.Helpers;
using GameWallet.UI.Controllers;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace GameWallet.Screens
{
    public class UnlockScreen : BaseScreen
    {
        #region Events

        public static event Action<string> UnlockButtonClicked;

        #endregion

        #region Constants

        private const string k_UnlockInputField = "unlock-form__input";
        private const string k_UnlockInputIcon = "unlock-form__input-icon";
        private const string k_UnlockToggle = "unlock-form__toggle";
        private const string k_UnlockButtonTemplate = "unlock-button-template";
        private const string k_UnlockLink = "unlock-footer__link";

        #endregion

        #region Fields

        private TextField m_UnlockInputField;
        private Button m_UnlockInputIcon;
        private Toggle m_UnlockToggle;
        private Button m_UnlockButton;
        private string m_UnlockInputValue = string.Empty;
        private Sprite m_UnlockShowIcon;
        private Sprite m_UnlockHideIcon;
        private bool m_IsSecurityChecked;
        private Button m_UnlockLink;

        #endregion

        #region Unity Lifecycle

        private void OnEnable()
        {
            UnlockScreenController.IconsLoaded += OnIconsLoaded;
            UnlockScreenController.IsPasswordCorrect += OnIsPasswordCorrect;
            UnlockScreenController.SuccessfullyConnected += OnSuccessfullyConnected;
        }

        private void Start()
        {
            UIHelpers.UpdateButtonState(m_UnlockButton, false);
        }
        
        private void OnDisable()
        {
            UnlockScreenController.IconsLoaded -= OnIconsLoaded;
            UnlockScreenController.IsPasswordCorrect -= OnIsPasswordCorrect;
            UnlockScreenController.SuccessfullyConnected -= OnSuccessfullyConnected;
        }

        protected override void SetVisualElements()
        {
            base.SetVisualElements();

            GetVisualElements();
        }

        protected override void RegisterButtonCallbacks()
        {
            base.RegisterButtonCallbacks();

            RegisterButtonsCallbacks();
            RegisterInputCallback();
            RegisterToggleCallback();
        }

        #endregion

        #region Initialization

        private void GetVisualElements()
        {
            m_UnlockInputField = m_Root.Q<TextField>(k_UnlockInputField);
            m_UnlockInputIcon = m_Root.Q<Button>(k_UnlockInputIcon);
            m_UnlockToggle = m_Root.Q<Toggle>(k_UnlockToggle);
            var buttonTemplate = m_Root.Q<VisualElement>(k_UnlockButtonTemplate);
            m_UnlockButton = buttonTemplate.Q<Button>();
            m_UnlockLink = m_Root.Q<Button>(k_UnlockLink);
        }

        private void RegisterButtonsCallbacks()
        {
            m_UnlockButton.RegisterCallback<ClickEvent>(HandleUnlockButtonClick);
            m_UnlockInputIcon.RegisterCallback<ClickEvent>(HandleUnlockInputIconClick);
            m_UnlockLink.RegisterCallback<ClickEvent>(HandleUnlockLinkClick);
        }

        private void RegisterInputCallback()
        {
            m_UnlockInputField.RegisterCallback<ChangeEvent<string>>(HandleUnlockInputChanged);
        }

        private void RegisterToggleCallback()
        {
            m_UnlockToggle.RegisterCallback<ChangeEvent<bool>>(HandleUnlockToggleChanged);
        }

        #endregion

        #region Callbacks

        private void HandleUnlockButtonClick(ClickEvent evt)
        {
            UnlockButtonClicked?.Invoke(m_UnlockInputValue);
        }

        private void HandleUnlockInputIconClick(ClickEvent evt)
        {
            m_UnlockInputField.isPasswordField = !m_UnlockInputField.isPasswordField;
            m_UnlockInputIcon.style.backgroundImage = m_UnlockInputField.isPasswordField
                ? new StyleBackground(m_UnlockShowIcon)
                : new StyleBackground(m_UnlockHideIcon);
        }

        private void HandleUnlockLinkClick(ClickEvent evt)
        {
            m_MainUIManager?.ShowImportScreen();
        }

        private void HandleUnlockInputChanged(ChangeEvent<string> evt)
        {
            m_UnlockInputValue = evt.newValue;

            UIHelpers.UpdateButtonState(m_UnlockButton,
                !string.IsNullOrEmpty(m_UnlockInputValue) && m_IsSecurityChecked);
        }

        private void HandleUnlockToggleChanged(ChangeEvent<bool> evt)
        {
            m_IsSecurityChecked = evt.newValue;

            UIHelpers.UpdateButtonState(m_UnlockButton,
                !string.IsNullOrEmpty(m_UnlockInputValue) && m_IsSecurityChecked);
        }

        #endregion

        #region Event Handlers

        private void OnIconsLoaded(List<Sprite> icons)
        {
            m_UnlockShowIcon = icons[0];
            m_UnlockHideIcon = icons[1];
        }

        private void OnIsPasswordCorrect(bool isPasswordCorrect)
        {
            if (isPasswordCorrect) return;
            
            m_UnlockInputField.value = string.Empty;
            m_UnlockInputValue = string.Empty;
            m_IsSecurityChecked = false;
            m_UnlockToggle.value = false;
            UIHelpers.UpdateButtonState(m_UnlockButton, false);
        }
        
        private void OnSuccessfullyConnected(bool isConnected)
        {
            if (!isConnected)
            {
                Debug.Log("Failed to connect");
                return;
            }

            Debug.Log("Successfully connected");
            SceneManager.LoadScene("Menu");
        }

        #endregion
    }
}