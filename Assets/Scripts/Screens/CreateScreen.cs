using System;
using System.Collections.Generic;
using GameWallet.Base;
using GameWallet.Managers;
using GameWallet.UI.Controllers;
using UnityEngine.UIElements;

namespace GameWallet.Screens
{
    public class CreateScreen : BaseScreen
    {
        #region Events
        
        public static event Action RegenerateMnemonicClicked;
        public static event Action CopyMnemonicClicked;
        public static event Action CreateScreenShown;
        public static event Action ProceedToVerifyScreenClicked;
        public static event Action ProceedToImportScreenClicked;
        
        #endregion

        #region Constants

        private const string k_Input = "input";
        private const string k_CreateInputClassName = "create-body__input";
        private const string k_MnemonicContainerLeft = "create-body-mnemonic-container__left";
        private const string k_MnemonicContainerRight = "create-body-mnemonic-container__right";
        private const string k_CreateButtonTemplate = "create-button-template";
        private const string k_CreateButton = "button";
        private const string k_CopyButton = "create-body__copy";
        private const string k_RegenerateButton = "create-body__regenerate";
        private const string k_CreateLinkButton = "create-footer__link-button";

        #endregion
        
        #region Fields

        private VisualElement m_MnemonicContainerLeft;
        private VisualElement m_MnemonicContainerRight;
        private List<TextField> m_MnemonicInputs;
        private Button m_CreateButton;
        private Button m_CopyButton;
        private Button m_RegenerateButton;
        private Button m_CreateLinkButton;
        private string m_Mnemonic;
        
        #endregion

        #region Unity Lifecycle

        private void OnEnable()
        {
            CreateScreenController.MnemonicRegenerated += OnMnemonicRegenerated;
        }
        
        private void Start()
        {
            m_Mnemonic = WalletManager.Instance.Mnemonic;
            m_MnemonicInputs = new List<TextField>();
            
            CreateMnemonicFields();
            PopulateMnemonicFields();
            PopulateContainers();
        }
        
        private void OnDisable()
        {
            CreateScreenController.MnemonicRegenerated -= OnMnemonicRegenerated;
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
        }
        
        #endregion

        #region Initialization

        private void GetVisualElements()
        {
            m_MnemonicContainerLeft = m_Root.Q<VisualElement>(k_MnemonicContainerLeft);
            m_MnemonicContainerRight = m_Root.Q<VisualElement>(k_MnemonicContainerRight);

            // Button template container
            var buttonTemplate = m_Root.Q<TemplateContainer>(k_CreateButtonTemplate);
            m_CreateButton = buttonTemplate.Q<Button>(k_CreateButton);
            
            m_CreateButton = m_Root.Q<Button>(k_CreateButton);
            m_CopyButton = m_Root.Q<Button>(k_CopyButton);
            m_RegenerateButton = m_Root.Q<Button>(k_RegenerateButton);
            m_CreateLinkButton = m_Root.Q<Button>(k_CreateLinkButton);
        }
        
        private void RegisterButtonsCallbacks()
        {
            m_CreateButton.RegisterCallback<ClickEvent>(ProceedToVerifyScreen);
            m_CopyButton.RegisterCallback<ClickEvent>(CopyMnemonic);
            m_RegenerateButton.RegisterCallback<ClickEvent>(Regenerate);
            m_CreateLinkButton.RegisterCallback<ClickEvent>(ProceedToImportScreen);
        }
        
        private void CreateMnemonicFields()
        {
            for (var i = 0; i < 12; i++)
            {
                var wordInput = new TextField
                {
                    value = $"{i + 1:00} ",
                    isReadOnly = true
                };
                
                wordInput.AddToClassList(k_Input);
                wordInput.AddToClassList(k_CreateInputClassName);

                if (i is 5 or 11)
                {
                    RemoveBottomMargin(wordInput);
                }
                
                m_MnemonicInputs.Add(wordInput);
            }
        }
        
        private void PopulateMnemonicFields()
        {
            var mnemonic = m_Mnemonic.Split(' ');
            
            for (var i = 0; i < 12; i++)
            {
                m_MnemonicInputs[i].value += mnemonic[i];
            }
        }
        
        private void PopulateContainers()
        {
            for (var i = 0; i < 12; i++)
            {
                if (i < 6)
                {
                    m_MnemonicContainerLeft.Add(m_MnemonicInputs[i]);
                }
                else
                {
                    m_MnemonicContainerRight.Add(m_MnemonicInputs[i]);
                }
            }
        }
        
        #endregion

        #region Event Handlers

        private void OnMnemonicRegenerated(string mnemonic)
        {
            m_Mnemonic = mnemonic;
        }
        
        #endregion
        
        #region Navigation

        public override void ShowScreen()
        {
            base.ShowScreen();
            
            CreateScreenShown?.Invoke();
        }
        
        private void ProceedToVerifyScreen(ClickEvent evt)
        {
            m_MainUIManager?.ShowVerifyScreen();
            
            ProceedToVerifyScreenClicked?.Invoke();
        }
        
        private void ProceedToImportScreen(ClickEvent evt)
        {
            m_MainUIManager?.ShowImportScreen();
            
            ProceedToImportScreenClicked?.Invoke();
        }
        
        #endregion
        
        #region Button Callbacks
        
        private void CopyMnemonic(ClickEvent evt)
        {
            CopyMnemonicClicked?.Invoke();
        }

        private void Regenerate(ClickEvent evt)
        {
            RegenerateMnemonicClicked?.Invoke();
            var mnemonicArray = m_Mnemonic.Split(' ');
            
            // Change mnemonic container inputs values
            var inputsLeft = m_MnemonicContainerLeft.Query<TextField>().ToList();
            var inputsRight = m_MnemonicContainerRight.Query<TextField>().ToList();
            
            for (int i = 0; i < 6; i++)
            {
                inputsLeft[i].value = $"{i + 1:00} {mnemonicArray[i]}";
                inputsRight[i].value = $"{i + 7:00} {mnemonicArray[i + 6]}";
            }
        }
        
        #endregion
        
        #region Utilities

        private void RemoveBottomMargin(TextField input)
        {
            input.style.marginBottom = 0;
        }

        #endregion
    }
}