using System;
using System.Collections.Generic;
using GameWallet.Base;
using GameWallet.UI.Controllers;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace GameWallet.Screens
{
    public class SetPasswordScreen : BaseScreen
    {
        #region Events

        public static event Action<string> SetPasswordButtonClicked;
        public static event Action SetPasswordScreenShown;
        public static event Action ProceedToWalletScreenClicked;

        #endregion

        #region Constants

        private const string k_SetPasswordInput = "set-password__input";
        private const string k_SetPasswordInputIcon = "set-password__input-icon";
        private const string k_SetPasswordSecurityToggle = "set-password__security-toggle";
        private const string k_SetPasswordButtonTemplate = "set-password__button-template";

        #endregion

        #region Fields

        private List<TextField> m_SetPasswordInputs;
        private List<Button> m_SetPasswordInputIcons;
        private Toggle m_SetPasswordSecurityToggle;
        private Button m_SetPasswordButton;
        private Sprite m_SetPasswordShowIcon;
        private Sprite m_SetPasswordHideIcon;
        private string m_Password;
        private bool m_IsPasswordsEqual;
        private bool m_IsSecurityChecked;

        #endregion

        #region Unity Lifecycle

        private void OnEnable()
        {
            SetPasswordScreenController.IconsLoaded += OnIconsLoaded;
            SetPasswordScreenController.SuccessfullyConnected += OnSuccessfullyConnected;
            DisableButton();
        }

        private void OnDisable()
        {
            SetPasswordScreenController.IconsLoaded -= OnIconsLoaded;
            SetPasswordScreenController.SuccessfullyConnected -= OnSuccessfullyConnected;
        }

        protected override void SetVisualElements()
        {
            base.SetVisualElements();

            GetVisualElements();
        }

        protected override void RegisterButtonCallbacks()
        {
            base.RegisterButtonCallbacks();

            RegisterInputCallbacks();
            RegisterIconCallbacks();
            RegisterToggleCallback();
            RegisterButtonCallback();
        }

        #endregion

        #region Initialization

        private void DisableButton()
        {
            m_SetPasswordButton.SetEnabled(false);
        }

        private void GetVisualElements()
        {
            m_SetPasswordInputs = m_Root.Query<TextField>(k_SetPasswordInput).ToList();
            m_SetPasswordInputIcons = m_Root.Query<Button>(k_SetPasswordInputIcon).ToList();
            m_SetPasswordSecurityToggle = m_Root.Q<Toggle>(k_SetPasswordSecurityToggle);
            var buttonTemplate = m_Root.Q<TemplateContainer>(k_SetPasswordButtonTemplate);
            m_SetPasswordButton = buttonTemplate.Q<Button>();
        }

        private void RegisterInputCallbacks()
        {
            foreach (var input in m_SetPasswordInputs)
            {
                input.RegisterCallback<ChangeEvent<string>>(CheckPasswordStrength);
            }
        }

        private void RegisterIconCallbacks()
        {
            foreach (var icon in m_SetPasswordInputIcons)
            {
                icon.RegisterCallback<ClickEvent>(TogglePasswordVisibility);
            }
        }

        private void RegisterToggleCallback()
        {
            m_SetPasswordSecurityToggle.RegisterCallback<ChangeEvent<bool>>(ToggleSecurityCheck);
        }

        private void RegisterButtonCallback()
        {
            m_SetPasswordButton.RegisterCallback<ClickEvent>(SetPassword);
        }

        #endregion

        #region Event Handlers

        private void OnIconsLoaded(List<Sprite> icons)
        {
            m_SetPasswordShowIcon = icons[0];
            m_SetPasswordHideIcon = icons[1];
        }

        private void OnSuccessfullyConnected(bool isConnected)
        {
            if (!isConnected)
            {
                Debug.Log("Failed to connect");
                return;
            }

            Debug.Log("Successfully connected");
            ProceedToWalletScreenClicked?.Invoke();
            SceneManager.LoadScene("Menu");
        }

        #endregion

        #region Callbacks

        private void CheckPasswordStrength(ChangeEvent<string> evt)
        {
            m_Password = evt.newValue;

            CheckPasswordMatch(m_Password);

            if (m_Password.Length < 8)
            {
                Debug.Log("Password is too short");
            }
        }

        private void TogglePasswordVisibility(ClickEvent evt)
        {
            var icon = evt.target as Button;
            var inputIndex = m_SetPasswordInputIcons.IndexOf(icon);
            var input = m_SetPasswordInputs[inputIndex];

            UpdateIconAndInput(input, icon);
        }

        private void ToggleSecurityCheck(ChangeEvent<bool> evt)
        {
            m_IsSecurityChecked = evt.newValue;

            UpdateButtonState(evt.newValue && m_IsPasswordsEqual);
        }

        private void SetPassword(ClickEvent evt)
        {
            SetPasswordButtonClicked?.Invoke(m_Password);
        }

        #endregion

        #region Navigation

        public override void ShowScreen()
        {
            base.ShowScreen();

            SetPasswordScreenShown?.Invoke();
        }

        #endregion

        #region Password Logic

        private void CheckPasswordMatch(string password)
        {
            var isPasswordMatch = true;

            foreach (var input in m_SetPasswordInputs)
            {
                if (input.text != password)
                {
                    isPasswordMatch = false;
                    Debug.Log("Passwords don't match");
                    break;
                }
            }

            m_IsPasswordsEqual = isPasswordMatch;

            UpdateButtonState(isPasswordMatch && m_IsSecurityChecked);
        }

        private void UpdateIconAndInput(TextField input, Button icon)
        {
            icon.style.backgroundImage = input.isPasswordField
                ? new StyleBackground(m_SetPasswordHideIcon)
                : new StyleBackground(m_SetPasswordShowIcon);

            input.isPasswordField = !input.isPasswordField;
        }

        private void UpdateButtonState(bool shouldEnable)
        {
            m_SetPasswordButton.SetEnabled(shouldEnable);
        }

        #endregion
    }
}