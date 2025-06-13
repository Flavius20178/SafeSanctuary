using UnityEngine.Assertions;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.Inputs;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation;
using UnityEngine.XR.Interaction.Toolkit.Utilities;
#if XR_HANDS_1_1_OR_NEWER
using UnityEngine.XR.Hands;
#endif

namespace UnityEngine.XR.Interaction.Toolkit.Samples.DeviceSimulator
{
    internal class XRInteractionSimulatorPlayModeMenu : MonoBehaviour
    {
        private static readonly Color k_DisabledColor = new(0x70 / 255f, 0x70 / 255f, 0x70 / 255f);

        [Header("Menus")] [SerializeField] private GameObject m_InputSelectionMenu;

        [SerializeField] private GameObject m_ClosedInputSelectionMenu;

        [SerializeField] private GameObject m_ControllerActionsMenu;

        [SerializeField] private GameObject m_ClosedControllerActionsMenu;

        [SerializeField] private GameObject m_HandActionsMenu;

        [SerializeField] private GameObject m_ClosedHandActionsMenu;

        [Header("Input Readers")] [SerializeField]
        private XRInputButtonReader m_ToggleActionMenu;

        [SerializeField] private XRInputButtonReader m_ToggleInputSelectionMenu;

        [Header("Device Highlight Panels")] [SerializeField]
        private GameObject m_HighlightFullBodyPanel;

        [SerializeField] private GameObject m_HighlightLeftControllerPanel;

        [SerializeField] private GameObject m_HighlightRightControllerPanel;

        [SerializeField] private GameObject m_HighlightLeftHandPanel;

        [SerializeField] private GameObject m_HighlightRightHandPanel;

        [SerializeField] private GameObject m_HighlightHeadPanel;

        [Header("Controller Action Panels")] [SerializeField]
        private GameObject m_ControllerActionHighlightPanel;

        [SerializeField] private Text m_FirstControllerActionText;

        [SerializeField] private Text m_SecondControllerActionText;

        [SerializeField] private Text m_ThirdControllerActionText;

        [SerializeField] private Text m_FourthControllerActionText;

        [SerializeField] private Text m_FirstControllerBindingText;

        [SerializeField] private Text m_SecondControllerBindingText;

        [SerializeField] private Text m_ThirdControllerBindingText;

        [SerializeField] private Text m_FourthControllerBindingText;

        [SerializeField] private GameObject m_FirstControllerBindingGO;

        [SerializeField] private GameObject m_SecondControllerBindingGO;

        [SerializeField] private GameObject m_ThirdControllerBindingGO;

        [SerializeField] private GameObject m_FourthControllerBindingGO;

        [Header("Hand Action Panels")] [SerializeField]
        private GameObject m_HandActionHighlightPanel;

        [SerializeField] private Text m_FirstHandActionText;

        [SerializeField] private Text m_SecondHandActionText;

        [SerializeField] private Text m_ThirdHandActionText;

        [SerializeField] private Text m_FourthHandActionText;

        [SerializeField] private Text m_FirstHandBindingText;

        [SerializeField] private Text m_SecondHandBindingText;

        [SerializeField] private Text m_ThirdHandBindingText;

        [SerializeField] private Text m_FourthHandBindingText;

        [SerializeField] private GameObject m_FirstHandBindingGO;

        [SerializeField] private GameObject m_SecondHandBindingGO;

        [SerializeField] private GameObject m_ThirdHandBindingGO;

        [SerializeField] private GameObject m_FourthHandBindingGO;

        [Header("Hand UI")] [SerializeField] private Image m_LeftHandIcon;

        [SerializeField] private Image m_RightHandIcon;

        [SerializeField] private GameObject m_HandPackageWarningPanel;

        [SerializeField] private GameObject m_InputModalityManagerWarningPanel;

        [SerializeField] private GameObject m_InputMenuHandVisualizerWarningPanel;

        [SerializeField] private GameObject m_HandMenuHandVisualizerWarningPanel;

        private int m_ControllerActionIndex = -1;
        private SimulatedDeviceLifecycleManager m_DeviceLifecycleManager;
        private int m_HandActionIndex = -1;
        private readonly int[] m_HandExpressionIndices = { -1, -1, -1, -1 };
        private SimulatedHandExpressionManager m_HandExpressionManager;
        private ControllerInputMode m_PreviousControllerInputMode = ControllerInputMode.None;
        private bool m_PreviousControllerMenuState;

        private SimulatedDeviceLifecycleManager.DeviceMode m_PreviousDeviceMode =
            SimulatedDeviceLifecycleManager.DeviceMode.None;

        private SimulatedHandExpression m_PreviousHandExpression = new();
        private bool m_PreviousHandMenuState;
        private TargetedDevices m_PreviousTargetedDeviceInput = TargetedDevices.None;
        private int m_QuickActionHandExpressionLength;

        private XRInteractionSimulator m_Simulator;

        /// <summary>
        ///     See <see cref="MonoBehaviour" />.
        /// </summary>
        protected void Start()
        {
#if HAS_FIND_FIRST_OBJECT_BY_TYPE
            var simulator = FindFirstObjectByType<XRInteractionSimulator>();
#else
            var simulator = Object.FindObjectOfType<XRInteractionSimulator>();
#endif
            if (simulator != null)
            {
                m_Simulator = simulator;
            }
            else
            {
                Debug.LogError("Could not find the XRInteractionSimulator component, disabling simulator UI.", this);
                gameObject.SetActive(false);
                return;
            }

            if (!m_Simulator.gameObject.TryGetComponent(out m_DeviceLifecycleManager))
            {
                Debug.LogError(
                    $"Could not find SimulatedDeviceLifecycleManager component on {m_Simulator.name}, disabling simulator UI.",
                    this);
                gameObject.SetActive(false);
                return;
            }

            if (!m_Simulator.gameObject.TryGetComponent(out m_HandExpressionManager))
            {
                Debug.LogError(
                    $"Could not find SimulatedHandExpressionManager component on {m_Simulator.name}, disabling simulator UI.",
                    this);
                gameObject.SetActive(false);
                return;
            }

            InitializeQuickActionPanels();

#if XR_HANDS_1_1_OR_NEWER
            CheckInputModalityManager();
#else
            m_HandPackageWarningPanel.SetActive(true);
            m_LeftHandIcon.color = k_DisabledColor;
            m_RightHandIcon.color = k_DisabledColor;
#endif

#if XR_HANDS_1_2_OR_NEWER
            if (!m_HandPackageWarningPanel.activeSelf && !m_InputModalityManagerWarningPanel.activeSelf)
                CheckHandVisualizer();
#endif
        }

        /// <summary>
        ///     See <see cref="MonoBehaviour" />.
        /// </summary>
        protected void Update()
        {
            HandleHighlightedDevicePanels();
            HandleHighlightedControllerActionPanels();
            HandleHighlightedHandActionPanels();
            HandleActiveMenus();
        }

        private void CheckInputModalityManager()
        {
            if (ComponentLocatorUtility<XRInputModalityManager>.TryFindComponent(out var inputModalityManager) &&
                inputModalityManager.leftHand == null && inputModalityManager.rightHand == null)
            {
                m_InputModalityManagerWarningPanel.SetActive(true);
                m_LeftHandIcon.color = k_DisabledColor;
                m_RightHandIcon.color = k_DisabledColor;
            }
        }

#if XR_HANDS_1_2_OR_NEWER
        private void CheckHandVisualizer()
        {
            if (ComponentLocatorUtility<XRInputModalityManager>.TryFindComponent(out var inputModalityManager))
            {
                if (inputModalityManager.leftHand == null && inputModalityManager.rightHand == null)
                    return;

                if ((inputModalityManager.leftHand != null &&
                     inputModalityManager.leftHand.GetComponentInChildren<XRHandMeshController>() != null) ||
                    (inputModalityManager.rightHand != null &&
                     inputModalityManager.rightHand.GetComponentInChildren<XRHandMeshController>() != null))
                    return;

                m_InputMenuHandVisualizerWarningPanel.SetActive(true);
                m_HandMenuHandVisualizerWarningPanel.SetActive(true);
            }
        }
#endif

        private void InitializeQuickActionPanels()
        {
            InitializeControllerQuickActionPanels();
            InitializeHandQuickActionPanels();
        }

        private void InitializeControllerQuickActionPanels()
        {
            var inputModesLength = m_Simulator.quickActionControllerInputModes.Count;
            if (inputModesLength > 0)
            {
                GetControllerQuickActionNames(m_Simulator.quickActionControllerInputModes[0],
                    m_FirstControllerActionText, m_FirstControllerBindingText);
            }
            else
            {
                m_FirstControllerActionText.gameObject.SetActive(false);
                m_FirstControllerBindingGO.SetActive(false);
            }

            if (inputModesLength > 1)
            {
                GetControllerQuickActionNames(m_Simulator.quickActionControllerInputModes[1],
                    m_SecondControllerActionText, m_SecondControllerBindingText);
            }
            else
            {
                m_SecondControllerActionText.gameObject.SetActive(false);
                m_SecondControllerBindingGO.SetActive(false);
            }

            if (inputModesLength > 2)
            {
                GetControllerQuickActionNames(m_Simulator.quickActionControllerInputModes[2],
                    m_ThirdControllerActionText, m_ThirdControllerBindingText);
            }
            else
            {
                m_ThirdControllerActionText.gameObject.SetActive(false);
                m_ThirdControllerBindingGO.SetActive(false);
            }

            if (inputModesLength > 3)
            {
                GetControllerQuickActionNames(m_Simulator.quickActionControllerInputModes[3],
                    m_FourthControllerActionText, m_FourthControllerBindingText);
            }
            else
            {
                m_FourthControllerActionText.gameObject.SetActive(false);
                m_FourthControllerBindingGO.SetActive(false);
            }
        }

        private void InitializeHandQuickActionPanels()
        {
            for (var i = 0; i < m_HandExpressionManager.simulatedHandExpressions.Count; i++)
                if (m_HandExpressionManager.simulatedHandExpressions[i].isQuickAction)
                {
                    if (m_QuickActionHandExpressionLength < 4)
                        m_HandExpressionIndices[m_QuickActionHandExpressionLength] = i;

                    m_QuickActionHandExpressionLength++;
                }

            if (m_QuickActionHandExpressionLength > 0)
            {
                var handExpression = m_HandExpressionManager.simulatedHandExpressions[m_HandExpressionIndices[0]];
                m_FirstHandActionText.text = handExpression.name;
                m_FirstHandBindingText.text = GetBindingString(handExpression.toggleInput);
            }
            else
            {
                m_FirstHandActionText.gameObject.SetActive(false);
                m_FirstHandBindingGO.SetActive(false);
            }

            if (m_QuickActionHandExpressionLength > 1)
            {
                var handExpression = m_HandExpressionManager.simulatedHandExpressions[m_HandExpressionIndices[1]];
                m_SecondHandActionText.text = handExpression.name;
                m_SecondHandBindingText.text = GetBindingString(handExpression.toggleInput);
            }
            else
            {
                m_SecondHandActionText.gameObject.SetActive(false);
                m_SecondHandBindingGO.SetActive(false);
            }

            if (m_QuickActionHandExpressionLength > 2)
            {
                var handExpression = m_HandExpressionManager.simulatedHandExpressions[m_HandExpressionIndices[2]];
                m_ThirdHandActionText.text = handExpression.name;
                m_ThirdHandBindingText.text = GetBindingString(handExpression.toggleInput);
            }
            else
            {
                m_ThirdHandActionText.gameObject.SetActive(false);
                m_ThirdHandBindingGO.SetActive(false);
            }

            if (m_QuickActionHandExpressionLength > 3)
            {
                var handExpression = m_HandExpressionManager.simulatedHandExpressions[m_HandExpressionIndices[3]];
                m_FourthHandActionText.text = handExpression.name;
                m_FourthHandBindingText.text = GetBindingString(handExpression.toggleInput);
            }
            else
            {
                m_FourthHandActionText.gameObject.SetActive(false);
                m_FourthHandBindingGO.SetActive(false);
            }
        }

        private void GetControllerQuickActionNames(ControllerInputMode inputMode, Text actionText, Text bindingText)
        {
            switch (inputMode)
            {
                case ControllerInputMode.None:
                    actionText.text = "None";
                    bindingText.text = "?";
                    break;
                case ControllerInputMode.Trigger:
                    actionText.text = "Trigger";
                    bindingText.text = GetBindingString(m_Simulator.triggerInput);
                    break;
                case ControllerInputMode.Grip:
                    actionText.text = "Grip";
                    bindingText.text = GetBindingString(m_Simulator.gripInput);
                    break;
                case ControllerInputMode.PrimaryButton:
                    actionText.text = "Primary";
                    bindingText.text = GetBindingString(m_Simulator.primaryButtonInput);
                    break;
                case ControllerInputMode.SecondaryButton:
                    actionText.text = "Secondary";
                    bindingText.text = GetBindingString(m_Simulator.secondaryButtonInput);
                    break;
                case ControllerInputMode.Menu:
                    actionText.text = "Menu";
                    bindingText.text = GetBindingString(m_Simulator.menuInput);
                    break;
                case ControllerInputMode.Primary2DAxisClick:
                    actionText.text = "Prim2DClick";
                    bindingText.text = GetBindingString(m_Simulator.primary2DAxisClickInput);
                    break;
                case ControllerInputMode.Secondary2DAxisClick:
                    actionText.text = "Sec2DClick";
                    bindingText.text = GetBindingString(m_Simulator.secondary2DAxisClickInput);
                    break;
                case ControllerInputMode.Primary2DAxisTouch:
                    actionText.text = "Prim2DTouch";
                    bindingText.text = GetBindingString(m_Simulator.primary2DAxisTouchInput);
                    break;
                case ControllerInputMode.Secondary2DAxisTouch:
                    actionText.text = "Sec2DTouch";
                    bindingText.text = GetBindingString(m_Simulator.secondary2DAxisTouchInput);
                    break;
                case ControllerInputMode.PrimaryTouch:
                    actionText.text = "PrimTouch";
                    bindingText.text = GetBindingString(m_Simulator.primaryTouchInput);
                    break;
                case ControllerInputMode.SecondaryTouch:
                    actionText.text = "SecTouch";
                    bindingText.text = GetBindingString(m_Simulator.secondaryTouchInput);
                    break;
                default:
                    Assert.IsTrue(false, $"Unhandled {nameof(inputMode)}={inputMode}.");
                    break;
            }
        }

        private static string GetBindingString(XRInputButtonReader reader)
        {
            if (reader == null)
                return string.Empty;

            InputAction action;
            switch (reader.inputSourceMode)
            {
                case XRInputButtonReader.InputSourceMode.InputActionReference:
                    action = reader.inputActionReferencePerformed != null
                        ? reader.inputActionReferencePerformed.action
                        : null;
                    break;
                case XRInputButtonReader.InputSourceMode.InputAction:
                    action = reader.inputActionPerformed;
                    break;
                default:
                    action = null;
                    break;
            }

            return action != null ? action.GetBindingDisplayString(0) : string.Empty;
        }

        /// <summary>
        ///     Toggles the visibility of the input selection menu.
        /// </summary>
        public void OpenCloseInputSelectionMenu()
        {
            if (m_InputSelectionMenu.activeSelf)
            {
                m_ClosedInputSelectionMenu.SetActive(true);
                m_InputSelectionMenu.SetActive(false);
            }
            else
            {
                m_ClosedInputSelectionMenu.SetActive(false);
                m_InputSelectionMenu.SetActive(true);
            }
        }

        /// <summary>
        ///     Toggles the visibility of for the controller actions menu.
        /// </summary>
        public void OpenCloseControllerActionsMenu()
        {
            if (m_ControllerActionsMenu.activeSelf)
            {
                m_ClosedControllerActionsMenu.SetActive(true);
                m_ControllerActionsMenu.SetActive(false);
            }
            else
            {
                m_ClosedControllerActionsMenu.SetActive(false);
                m_ControllerActionsMenu.SetActive(true);
            }
        }

        /// <summary>
        ///     Toggles the visibility of for the hand actions menu.
        /// </summary>
        public void OpenCloseHandActionsMenu()
        {
            if (m_HandActionsMenu.activeSelf)
            {
                m_ClosedHandActionsMenu.SetActive(true);
                m_HandActionsMenu.SetActive(false);
            }
            else
            {
                m_ClosedHandActionsMenu.SetActive(false);
                m_HandActionsMenu.SetActive(true);
            }
        }

        private void HandleActiveMenus()
        {
            if (m_PreviousDeviceMode != m_DeviceLifecycleManager.deviceMode && !m_Simulator.manipulatingFPS)
            {
                if (m_Simulator.manipulatingLeftController || m_Simulator.manipulatingRightController)
                {
                    m_PreviousHandMenuState = m_HandActionsMenu.activeSelf;
                    m_HandActionsMenu.SetActive(false);
                    m_ClosedHandActionsMenu.SetActive(false);

                    if (m_PreviousControllerMenuState)
                        m_ControllerActionsMenu.SetActive(true);
                    else
                        m_ClosedControllerActionsMenu.SetActive(true);
                }
                else if (m_Simulator.manipulatingLeftHand || m_Simulator.manipulatingRightHand)
                {
                    m_PreviousControllerMenuState = m_ControllerActionsMenu.activeSelf;
                    m_ControllerActionsMenu.SetActive(false);
                    m_ClosedControllerActionsMenu.SetActive(false);

                    if (m_PreviousHandMenuState)
                        m_HandActionsMenu.SetActive(true);
                    else
                        m_ClosedHandActionsMenu.SetActive(true);
                }

                m_PreviousDeviceMode = m_DeviceLifecycleManager.deviceMode;
            }

            if (m_Simulator.manipulatingFPS && m_PreviousDeviceMode != SimulatedDeviceLifecycleManager.DeviceMode.None)
            {
                if (m_PreviousDeviceMode == SimulatedDeviceLifecycleManager.DeviceMode.Controller)
                    m_PreviousControllerMenuState = m_ControllerActionsMenu.activeSelf;
                else if (m_PreviousDeviceMode == SimulatedDeviceLifecycleManager.DeviceMode.Hand)
                    m_PreviousHandMenuState = m_HandActionsMenu.activeSelf;

                m_HandActionsMenu.SetActive(false);
                m_ClosedHandActionsMenu.SetActive(false);
                m_ControllerActionsMenu.SetActive(false);
                m_ClosedControllerActionsMenu.SetActive(false);

                m_PreviousDeviceMode = SimulatedDeviceLifecycleManager.DeviceMode.None;
            }

            if (m_ToggleActionMenu.ReadWasPerformedThisFrame())
            {
                if (m_Simulator.manipulatingLeftController || m_Simulator.manipulatingRightController)
                    OpenCloseControllerActionsMenu();
                else if (m_Simulator.manipulatingLeftHand || m_Simulator.manipulatingRightHand)
                    OpenCloseHandActionsMenu();
            }

            if (m_ToggleInputSelectionMenu.ReadWasPerformedThisFrame())
                OpenCloseInputSelectionMenu();
        }

        private void HandleHighlightedDevicePanels()
        {
            if (m_Simulator.targetedDeviceInput != m_PreviousTargetedDeviceInput ||
                m_PreviousDeviceMode != m_DeviceLifecycleManager.deviceMode)
            {
                ClearHighlightedDevicePanels();

                if (m_Simulator.manipulatingFPS)
                {
                    m_HighlightFullBodyPanel.SetActive(true);
                    return;
                }

                if (m_Simulator.manipulatingLeftController) m_HighlightLeftControllerPanel.SetActive(true);

                if (m_Simulator.manipulatingRightController) m_HighlightRightControllerPanel.SetActive(true);

                if (m_Simulator.manipulatingLeftHand) m_HighlightLeftHandPanel.SetActive(true);

                if (m_Simulator.manipulatingRightHand) m_HighlightRightHandPanel.SetActive(true);

                if (m_Simulator.manipulatingHMD) m_HighlightHeadPanel.SetActive(true);

                m_PreviousTargetedDeviceInput = m_Simulator.targetedDeviceInput;
            }
        }

        private void HandleHighlightedControllerActionPanels()
        {
            if (m_Simulator.controllerInputMode != m_PreviousControllerInputMode)
            {
                m_ControllerActionIndex =
                    m_ControllerActionIndex < m_Simulator.quickActionControllerInputModes.Count - 1
                        ? m_ControllerActionIndex + 1
                        : 0;
                m_ControllerActionHighlightPanel.SetActive(true);

                if (m_ControllerActionIndex == 0)
                    m_ControllerActionHighlightPanel.transform.position =
                        m_FirstControllerActionText.transform.position;
                else if (m_ControllerActionIndex == 1)
                    m_ControllerActionHighlightPanel.transform.position =
                        m_SecondControllerActionText.transform.position;
                else if (m_ControllerActionIndex == 2)
                    m_ControllerActionHighlightPanel.transform.position =
                        m_ThirdControllerActionText.transform.position;
                else if (m_ControllerActionIndex == 3)
                    m_ControllerActionHighlightPanel.transform.position =
                        m_FourthControllerActionText.transform.position;
                else
                    m_ControllerActionHighlightPanel.SetActive(false);

                m_PreviousControllerInputMode = m_Simulator.controllerInputMode;
            }
        }

        private void HandleHighlightedHandActionPanels()
        {
            if (m_Simulator.currentHandExpression != m_PreviousHandExpression)
            {
                m_HandActionIndex = m_HandActionIndex < m_QuickActionHandExpressionLength - 1
                    ? m_HandActionIndex + 1
                    : 0;
                m_HandActionHighlightPanel.SetActive(true);

                if (m_HandActionIndex == 0)
                    m_HandActionHighlightPanel.transform.position = m_FirstHandActionText.transform.position;
                else if (m_HandActionIndex == 1)
                    m_HandActionHighlightPanel.transform.position = m_SecondHandActionText.transform.position;
                else if (m_HandActionIndex == 2)
                    m_HandActionHighlightPanel.transform.position = m_ThirdHandActionText.transform.position;
                else if (m_HandActionIndex == 3)
                    m_HandActionHighlightPanel.transform.position = m_FourthHandActionText.transform.position;
                else
                    m_HandActionHighlightPanel.SetActive(false);

                m_PreviousHandExpression = m_Simulator.currentHandExpression;
            }
        }

        private void ClearHighlightedDevicePanels()
        {
            m_HighlightFullBodyPanel.SetActive(false);
            m_HighlightLeftControllerPanel.SetActive(false);
            m_HighlightRightControllerPanel.SetActive(false);
            m_HighlightLeftHandPanel.SetActive(false);
            m_HighlightRightHandPanel.SetActive(false);
            m_HighlightHeadPanel.SetActive(false);
        }
    }
}