using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation;

namespace UnityEngine.XR.Interaction.Toolkit.Samples.DeviceSimulator
{
    internal class XRInteractionSimulatorInputFeedbackUI : MonoBehaviour
    {
        [Header("Input Device Modes")] [SerializeField]
        private GameObject m_HMDPanel;

        [SerializeField] private GameObject m_MirrorModePanel;

        [SerializeField] private GameObject m_RightHandPanel;

        [SerializeField] private GameObject m_LeftHandPanel;

        [SerializeField] private GameObject m_RightControllerPanel;

        [SerializeField] private GameObject m_LeftControllerPanel;

        [SerializeField] private GameObject m_BothControllersPanel;

        [SerializeField] private GameObject m_BothHandsPanel;

        [Header("Controller Input Modes")] [SerializeField]
        private GameObject m_TriggerPanel;

        [SerializeField] private Image m_TriggerBg;

        [SerializeField] private GameObject m_GripPanel;

        [SerializeField] private Image m_GripBg;

        [SerializeField] private GameObject m_PrimaryPanel;

        [SerializeField] private Image m_PrimaryBg;

        [SerializeField] private GameObject m_SecondaryPanel;

        [SerializeField] private Image m_SecondaryBg;

        [SerializeField] private GameObject m_MenuPanel;

        [SerializeField] private Image m_MenuBg;

        [SerializeField] private GameObject m_Primary2DAxisClickPanel;

        [SerializeField] private Image m_Primary2DAxisClickBg;

        [SerializeField] private GameObject m_Secondary2DAxisClickPanel;

        [SerializeField] private Image m_Secondary2DAxisClickBg;

        [SerializeField] private GameObject m_Primary2DAxisTouchPanel;

        [SerializeField] private Image m_Primary2DAxisTouchBg;

        [SerializeField] private GameObject m_Secondary2DAxisTouchPanel;

        [SerializeField] private Image m_Secondary2DAxisTouchBg;

        [SerializeField] private GameObject m_PrimaryTouchPanel;

        [SerializeField] private Image m_PrimaryTouchBg;

        [SerializeField] private GameObject m_SecondaryTouchPanel;

        [SerializeField] private Image m_SecondaryTouchBg;

        [SerializeField] private GameObject m_ControllerHotkeyPanel;

        [SerializeField] private Image m_ControllerHotkeyBg;

        [SerializeField] private Image m_ControllerHotkeyIcon;

        [SerializeField] private Text m_ControllerHotkeyText;

        [SerializeField] private Sprite m_LeftControllerSprite;

        [SerializeField] private Sprite m_RightControllerSprite;

        [SerializeField] private GameObject m_ControllerInputRow;

        [Header("Hand Input Modes")] [SerializeField]
        private GameObject m_PokePanel;

        [SerializeField] private Image m_PokePanelBg;

        [SerializeField] private GameObject m_PinchPanel;

        [SerializeField] private Image m_PinchPanelBg;

        [SerializeField] private GameObject m_GrabPanel;

        [SerializeField] private Image m_GrabPanelBg;

        [SerializeField] private GameObject m_ThumbPanel;

        [SerializeField] private Image m_ThumbPanelBg;

        [SerializeField] private GameObject m_OpenPanel;

        [SerializeField] private Image m_OpenPanelBg;

        [SerializeField] private GameObject m_FistPanel;

        [SerializeField] private Image m_FistPanelBg;

        [SerializeField] private GameObject m_CustomPanel;

        [SerializeField] private GameObject m_HandHotkeyPanel;

        [SerializeField] private Image m_HandHotkeyBg;

        [SerializeField] private Image m_HandHotkeyIcon;

        [SerializeField] private Text m_HandHotkeyText;

        [SerializeField] private Sprite m_LeftHandSprite;

        [SerializeField] private Sprite m_RightHandSprite;


        [SerializeField] private GameObject m_HandInputRow;

        [Header("General Input")] [SerializeField]
        private GameObject m_TranslateForwardPanel;

        [SerializeField] private GameObject m_TranslateBackwardPanel;

        [SerializeField] private GameObject m_TranslateUpPanel;

        [SerializeField] private GameObject m_TranslateDownPanel;

        [SerializeField] private GameObject m_TranslateLeftPanel;

        [SerializeField] private GameObject m_TranslateRightPanel;

        [SerializeField] private GameObject m_RotateUpPanel;

        [SerializeField] private GameObject m_RotateDownPanel;

        [SerializeField] private GameObject m_RotateLeftPanel;

        [SerializeField] private GameObject m_RotateRightPanel;

        private ActiveDeviceMode m_ActiveDeviceMode = ActiveDeviceMode.None;
        private Dictionary<ControllerInputMode, Image> m_ControllerInputBgs;

        private Dictionary<ControllerInputMode, GameObject> m_ControllerInputPanels;

        private SimulatedDeviceLifecycleManager m_DeviceLifecycleManager;
        private Dictionary<string, Image> m_HandExpressionBgs;
        private SimulatedHandExpressionManager m_HandExpressionManager;
        private Dictionary<string, GameObject> m_HandExpressionPanels;

        private bool m_IsPerformingInput;
        private ControllerInputMode m_PreviousControllerInputMode;

        private SimulatedDeviceLifecycleManager.DeviceMode m_PreviousDeviceMode =
            SimulatedDeviceLifecycleManager.DeviceMode.None;

        private SimulatedHandExpression m_PreviousHandExpression;

        private XRInteractionSimulator m_Simulator;
        private bool m_ToggleMousePressed;

        private enum ActiveDeviceMode
        {
            LeftController,
            RightController,
            BothControllers,
            LeftHand,
            RightHand,
            BothHands,
            HMD,
            None
        }

        // ReSharper disable InconsistentNaming -- Treat as constants
        private static readonly Color k_DefaultPanelColor = new(0x55 / 255f, 0x55 / 255f, 0x55 / 255f);
        private static readonly Color k_SelectedColor = new(0x4F / 255f, 0x65 / 255f, 0x7F / 255f);

        private static readonly Color k_EnabledColor = new(0x88 / 255f, 0x88 / 255f, 0x88 / 255f);
        // ReSharper restore InconsistentNaming

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

            InitializeUIDictionaries();
            ActivateControllerPanels();
            ActivateHandPanels();
        }

        /// <summary>
        ///     See <see cref="MonoBehaviour" />.
        /// </summary>
        protected void Update()
        {
            HandleActiveDeviceModePanels();
            HandleGeneralInputFeedback();
            HandleActiveInputModePanels();

            if (m_DeviceLifecycleManager.deviceMode == SimulatedDeviceLifecycleManager.DeviceMode.Controller)
                HandleDeviceHotkeyPanels();
            else if (m_DeviceLifecycleManager.deviceMode == SimulatedDeviceLifecycleManager.DeviceMode.Hand)
                HandleHandHotkeyPanels();
        }

        private void HandleGeneralInputFeedback()
        {
            if (m_Simulator.toggleMouseInput.ReadWasCompletedThisFrame())
                m_ToggleMousePressed = false;

            if (!m_ToggleMousePressed)
                ClearActiveGeneralInputPanels();

            HandleKeyboardInputFeedback();
            HandleMouseInputFeedback();
        }

        private void HandleKeyboardInputFeedback()
        {
            if (m_Simulator.translateXInput.TryReadValue(out var xValue))
            {
                m_ToggleMousePressed = false;

                if (xValue >= 0f)
                    m_TranslateRightPanel.SetActive(true);
                else
                    m_TranslateLeftPanel.SetActive(true);
            }

            if (m_Simulator.translateYInput.TryReadValue(out var yValue))
            {
                m_ToggleMousePressed = false;

                if (yValue >= 0f)
                    m_TranslateUpPanel.SetActive(true);
                else
                    m_TranslateDownPanel.SetActive(true);
            }

            if (m_Simulator.translateZInput.TryReadValue(out var zValue))
            {
                m_ToggleMousePressed = false;

                if (zValue >= 0f)
                    m_TranslateForwardPanel.SetActive(true);
                else
                    m_TranslateBackwardPanel.SetActive(true);
            }

            if (m_Simulator.keyboardRotationDeltaInput.TryReadValue(out var rotValue))
            {
                m_ToggleMousePressed = false;

                if (rotValue.x > 0f)
                    m_RotateRightPanel.SetActive(true);
                else if (rotValue.x < 0f)
                    m_RotateLeftPanel.SetActive(true);

                if (rotValue.y > 0f)
                    m_RotateUpPanel.SetActive(true);
                else if (rotValue.y < 0f)
                    m_RotateDownPanel.SetActive(true);
            }
        }

        private void HandleMouseInputFeedback()
        {
            if (m_Simulator.toggleMouseInput.ReadIsPerformed() &&
                m_Simulator.mouseRotationDeltaInput.TryReadValue(out var rotValue))
            {
                m_ToggleMousePressed = true;

                m_TranslateBackwardPanel.SetActive(false);
                m_TranslateForwardPanel.SetActive(false);
                m_TranslateUpPanel.SetActive(false);
                m_TranslateDownPanel.SetActive(false);
                m_TranslateRightPanel.SetActive(false);
                m_TranslateLeftPanel.SetActive(false);

                if (rotValue.x > 0f)
                {
                    m_RotateLeftPanel.SetActive(false);
                    m_RotateRightPanel.SetActive(true);
                }
                else if (rotValue.x < 0f)
                {
                    m_RotateRightPanel.SetActive(false);
                    m_RotateLeftPanel.SetActive(true);
                }

                if (rotValue.y > 0f)
                {
                    m_RotateDownPanel.SetActive(false);
                    m_RotateUpPanel.SetActive(true);
                }
                else if (rotValue.y < 0f)
                {
                    m_RotateUpPanel.SetActive(false);
                    m_RotateDownPanel.SetActive(true);
                }
            }

            if (m_Simulator.toggleMouseInput.ReadIsPerformed() &&
                m_Simulator.mouseScrollInput.TryReadValue(out var scrollValue))
            {
                m_ToggleMousePressed = true;

                m_RotateLeftPanel.SetActive(false);
                m_RotateRightPanel.SetActive(false);
                m_RotateDownPanel.SetActive(false);
                m_RotateUpPanel.SetActive(false);

                if (scrollValue.y >= 0f)
                {
                    m_TranslateBackwardPanel.SetActive(false);
                    m_TranslateForwardPanel.SetActive(true);
                }
                else
                {
                    m_TranslateForwardPanel.SetActive(false);
                    m_TranslateBackwardPanel.SetActive(true);
                }
            }
        }

        private void HandleActiveDeviceModePanels()
        {
            if (m_Simulator.manipulatingFPS || m_Simulator.manipulatingHMD)
            {
                if (m_ActiveDeviceMode == ActiveDeviceMode.HMD)
                    return;

                ClearActiveInputModePanels();
                m_HMDPanel.SetActive(true);
                m_ActiveDeviceMode = ActiveDeviceMode.HMD;
            }
            else if (m_Simulator.manipulatingLeftController && m_Simulator.manipulatingRightController)
            {
                if (m_ActiveDeviceMode == ActiveDeviceMode.BothControllers)
                    return;

                ClearActiveInputModePanels();
                m_BothControllersPanel.SetActive(true);
                m_ActiveDeviceMode = ActiveDeviceMode.BothControllers;
            }
            else if (m_Simulator.manipulatingLeftController)
            {
                if (m_ActiveDeviceMode == ActiveDeviceMode.LeftController)
                    return;

                ClearActiveInputModePanels();
                m_LeftControllerPanel.SetActive(true);
                m_ActiveDeviceMode = ActiveDeviceMode.LeftController;
            }
            else if (m_Simulator.manipulatingRightController)
            {
                if (m_ActiveDeviceMode == ActiveDeviceMode.RightController)
                    return;

                ClearActiveInputModePanels();
                m_RightControllerPanel.SetActive(true);
                m_ActiveDeviceMode = ActiveDeviceMode.RightController;
            }
            else if (m_Simulator.manipulatingLeftHand && m_Simulator.manipulatingRightHand)
            {
                if (m_ActiveDeviceMode == ActiveDeviceMode.BothHands)
                    return;

                ClearActiveInputModePanels();
                m_BothHandsPanel.SetActive(true);
                m_ActiveDeviceMode = ActiveDeviceMode.BothHands;
            }
            else if (m_Simulator.manipulatingLeftHand)
            {
                if (m_ActiveDeviceMode == ActiveDeviceMode.LeftHand)
                    return;

                ClearActiveInputModePanels();
                m_LeftHandPanel.SetActive(true);
                m_ActiveDeviceMode = ActiveDeviceMode.LeftHand;
            }
            else if (m_Simulator.manipulatingRightHand)
            {
                if (m_ActiveDeviceMode == ActiveDeviceMode.RightHand)
                    return;

                ClearActiveInputModePanels();
                m_RightHandPanel.SetActive(true);
                m_ActiveDeviceMode = ActiveDeviceMode.RightHand;
            }
        }

        private void HandleActiveInputModePanels()
        {
            if (m_Simulator.manipulatingFPS || m_Simulator.manipulatingHMD)
            {
                m_ControllerInputRow.SetActive(false);
                m_HandInputRow.SetActive(false);
                return;
            }

            if (!m_ControllerInputRow.activeSelf && m_DeviceLifecycleManager.deviceMode ==
                SimulatedDeviceLifecycleManager.DeviceMode.Controller)
                m_ControllerInputRow.SetActive(true);

            if (!m_HandInputRow.activeSelf &&
                m_DeviceLifecycleManager.deviceMode == SimulatedDeviceLifecycleManager.DeviceMode.Hand)
                m_HandInputRow.SetActive(true);

            if (!m_IsPerformingInput)
            {
                if (m_PreviousDeviceMode != m_DeviceLifecycleManager.deviceMode)
                {
                    m_ControllerInputRow.SetActive(false);
                    m_HandInputRow.SetActive(false);

                    if (m_DeviceLifecycleManager.deviceMode == SimulatedDeviceLifecycleManager.DeviceMode.Controller)
                    {
                        m_ControllerInputRow.SetActive(true);
                        HighlightActiveControllerInputMode(k_SelectedColor);
                    }
                    else if (m_DeviceLifecycleManager.deviceMode == SimulatedDeviceLifecycleManager.DeviceMode.Hand)
                    {
                        m_HandInputRow.SetActive(true);
                        HighlightActiveHandInputMode(k_SelectedColor);
                    }

                    m_PreviousDeviceMode = m_DeviceLifecycleManager.deviceMode;
                }
                else if (m_DeviceLifecycleManager.deviceMode == SimulatedDeviceLifecycleManager.DeviceMode.Controller &&
                         m_PreviousControllerInputMode != m_Simulator.controllerInputMode)
                {
                    HighlightActiveControllerInputMode(k_SelectedColor);
                }
                else if (m_DeviceLifecycleManager.deviceMode == SimulatedDeviceLifecycleManager.DeviceMode.Hand &&
                         m_PreviousHandExpression != m_Simulator.currentHandExpression)
                {
                    HighlightActiveHandInputMode(k_SelectedColor);
                }
            }

            if (m_Simulator.togglePerformQuickActionInput.ReadWasPerformedThisFrame())
            {
                m_IsPerformingInput = !m_IsPerformingInput;

                if (m_DeviceLifecycleManager.deviceMode == SimulatedDeviceLifecycleManager.DeviceMode.Controller)
                {
                    if (m_IsPerformingInput)
                        HighlightActiveControllerInputMode(k_EnabledColor);
                    else
                        HighlightActiveControllerInputMode(k_SelectedColor);
                }
                else if (m_DeviceLifecycleManager.deviceMode == SimulatedDeviceLifecycleManager.DeviceMode.Hand)
                {
                    if (m_IsPerformingInput)
                        HighlightActiveHandInputMode(k_EnabledColor);
                    else
                        HighlightActiveHandInputMode(k_SelectedColor);
                }
            }

            if (m_Simulator.cycleQuickActionInput.ReadWasPerformedThisFrame()) m_IsPerformingInput = false;
        }

        private void HandleDeviceHotkeyPanels()
        {
            if (m_Simulator.gripInput.ReadIsPerformed())
            {
                ApplyHotkeyText(m_Simulator.gripInput, SimulatedDeviceLifecycleManager.DeviceMode.Controller);
                m_ControllerHotkeyPanel.SetActive(true);
            }
            else if (m_Simulator.gripInput.ReadWasCompletedThisFrame())
            {
                m_ControllerHotkeyPanel.SetActive(false);
            }

            if (m_Simulator.triggerInput.ReadIsPerformed())
            {
                ApplyHotkeyText(m_Simulator.triggerInput, SimulatedDeviceLifecycleManager.DeviceMode.Controller);
                m_ControllerHotkeyPanel.SetActive(true);
            }
            else if (m_Simulator.triggerInput.ReadWasCompletedThisFrame())
            {
                m_ControllerHotkeyPanel.SetActive(false);
            }

            if (m_Simulator.primaryButtonInput.ReadIsPerformed())
            {
                ApplyHotkeyText(m_Simulator.primaryButtonInput, SimulatedDeviceLifecycleManager.DeviceMode.Controller);
                m_ControllerHotkeyPanel.SetActive(true);
            }
            else if (m_Simulator.primaryButtonInput.ReadWasCompletedThisFrame())
            {
                m_ControllerHotkeyPanel.SetActive(false);
            }

            if (m_Simulator.secondaryButtonInput.ReadIsPerformed())
            {
                ApplyHotkeyText(m_Simulator.secondaryButtonInput,
                    SimulatedDeviceLifecycleManager.DeviceMode.Controller);
                m_ControllerHotkeyPanel.SetActive(true);
            }
            else if (m_Simulator.secondaryButtonInput.ReadWasCompletedThisFrame())
            {
                m_ControllerHotkeyPanel.SetActive(false);
            }

            if (m_Simulator.menuInput.ReadIsPerformed())
            {
                ApplyHotkeyText(m_Simulator.menuInput, SimulatedDeviceLifecycleManager.DeviceMode.Controller);
                m_ControllerHotkeyPanel.SetActive(true);
            }
            else if (m_Simulator.menuInput.ReadWasCompletedThisFrame())
            {
                m_ControllerHotkeyPanel.SetActive(false);
            }

            if (m_Simulator.primary2DAxisClickInput.ReadIsPerformed())
            {
                ApplyHotkeyText(m_Simulator.primary2DAxisClickInput,
                    SimulatedDeviceLifecycleManager.DeviceMode.Controller);
                m_ControllerHotkeyPanel.SetActive(true);
            }
            else if (m_Simulator.primary2DAxisClickInput.ReadWasCompletedThisFrame())
            {
                m_ControllerHotkeyPanel.SetActive(false);
            }

            if (m_Simulator.secondary2DAxisClickInput.ReadIsPerformed())
            {
                ApplyHotkeyText(m_Simulator.secondary2DAxisClickInput,
                    SimulatedDeviceLifecycleManager.DeviceMode.Controller);
                m_ControllerHotkeyPanel.SetActive(true);
            }
            else if (m_Simulator.secondary2DAxisClickInput.ReadWasCompletedThisFrame())
            {
                m_ControllerHotkeyPanel.SetActive(false);
            }

            if (m_Simulator.primary2DAxisTouchInput.ReadIsPerformed())
            {
                ApplyHotkeyText(m_Simulator.primary2DAxisTouchInput,
                    SimulatedDeviceLifecycleManager.DeviceMode.Controller);
                m_ControllerHotkeyPanel.SetActive(true);
            }
            else if (m_Simulator.primary2DAxisTouchInput.ReadWasCompletedThisFrame())
            {
                m_ControllerHotkeyPanel.SetActive(false);
            }

            if (m_Simulator.secondary2DAxisTouchInput.ReadIsPerformed())
            {
                ApplyHotkeyText(m_Simulator.secondary2DAxisTouchInput,
                    SimulatedDeviceLifecycleManager.DeviceMode.Controller);
                m_ControllerHotkeyPanel.SetActive(true);
            }
            else if (m_Simulator.secondary2DAxisTouchInput.ReadWasCompletedThisFrame())
            {
                m_ControllerHotkeyPanel.SetActive(false);
            }

            if (m_Simulator.primaryTouchInput.ReadIsPerformed())
            {
                ApplyHotkeyText(m_Simulator.primaryTouchInput, SimulatedDeviceLifecycleManager.DeviceMode.Controller);
                m_ControllerHotkeyPanel.SetActive(true);
            }
            else if (m_Simulator.primaryTouchInput.ReadWasCompletedThisFrame())
            {
                m_ControllerHotkeyPanel.SetActive(false);
            }

            if (m_Simulator.secondaryTouchInput.ReadIsPerformed())
            {
                ApplyHotkeyText(m_Simulator.secondaryTouchInput, SimulatedDeviceLifecycleManager.DeviceMode.Controller);
                m_ControllerHotkeyPanel.SetActive(true);
            }
            else if (m_Simulator.secondaryTouchInput.ReadWasCompletedThisFrame())
            {
                m_ControllerHotkeyPanel.SetActive(false);
            }
        }

        private void HandleHandHotkeyPanels()
        {
            foreach (var handExpression in m_HandExpressionManager.simulatedHandExpressions)
                if (handExpression.toggleInput.ReadIsPerformed())
                {
                    ApplyHotkeyText(handExpression.toggleInput, SimulatedDeviceLifecycleManager.DeviceMode.Hand);
                    m_HandHotkeyPanel.SetActive(true);
                }
                else if (handExpression.toggleInput.ReadWasCompletedThisFrame())
                {
                    m_HandHotkeyPanel.SetActive(false);
                }
        }

        private void ApplyHotkeyText(XRInputButtonReader inputReader, SimulatedDeviceLifecycleManager.DeviceMode mode)
        {
            var bindingText = inputReader.inputActionReferencePerformed.action.GetBindingDisplayString(0);

            if (mode == SimulatedDeviceLifecycleManager.DeviceMode.Controller)
            {
                if (m_Simulator.leftDeviceActionsInput.ReadIsPerformed())
                    m_ControllerHotkeyIcon.sprite = m_LeftControllerSprite;
                else
                    m_ControllerHotkeyIcon.sprite = m_RightControllerSprite;

                m_ControllerHotkeyText.text = $"{bindingText}";
            }
            else if (mode == SimulatedDeviceLifecycleManager.DeviceMode.Hand)
            {
                if (m_Simulator.leftDeviceActionsInput.ReadIsPerformed())
                    m_HandHotkeyIcon.sprite = m_LeftHandSprite;
                else
                    m_HandHotkeyIcon.sprite = m_RightHandSprite;

                m_HandHotkeyText.text = $"{bindingText}";
            }
        }

        private void ActivateControllerPanels()
        {
            for (var i = 0; i < m_Simulator.quickActionControllerInputModes.Count; i++)
            {
                if (!m_ControllerInputPanels.ContainsKey(m_Simulator.quickActionControllerInputModes[i]))
                {
                    var inputModeName = m_Simulator.quickActionControllerInputModes[i].ToString();
                    Debug.LogError($"Panel for the {inputModeName} controller input mode does not exist.", this);
                }

                var panel = m_ControllerInputPanels[m_Simulator.quickActionControllerInputModes[i]];
                panel.SetActive(true);
                panel.transform.SetSiblingIndex(i);
            }
        }

        private void ActivateHandPanels()
        {
            for (var i = 0; i < m_HandExpressionManager.simulatedHandExpressions.Count; i++)
                if (m_HandExpressionManager.simulatedHandExpressions[i].isQuickAction)
                {
                    var handExpressionName = m_HandExpressionManager.simulatedHandExpressions[i].name;

                    if (!m_HandExpressionPanels.ContainsKey(handExpressionName))
                        Debug.LogError($"Panel for the {handExpressionName} hand expression does not exist.", this);

                    var panel = m_HandExpressionPanels[handExpressionName];
                    panel.SetActive(true);
                    panel.transform.SetSiblingIndex(i);
                }
        }

        private void InitializeUIDictionaries()
        {
            m_ControllerInputPanels = new Dictionary<ControllerInputMode, GameObject>
            {
                { ControllerInputMode.Trigger, m_TriggerPanel },
                { ControllerInputMode.Grip, m_GripPanel },
                { ControllerInputMode.PrimaryButton, m_PrimaryPanel },
                { ControllerInputMode.SecondaryButton, m_SecondaryPanel },
                { ControllerInputMode.Menu, m_MenuPanel },
                { ControllerInputMode.Primary2DAxisClick, m_Primary2DAxisClickPanel },
                { ControllerInputMode.Secondary2DAxisClick, m_Secondary2DAxisClickPanel },
                { ControllerInputMode.Primary2DAxisTouch, m_Primary2DAxisTouchPanel },
                { ControllerInputMode.Secondary2DAxisTouch, m_Secondary2DAxisTouchPanel },
                { ControllerInputMode.PrimaryTouch, m_PrimaryTouchPanel },
                { ControllerInputMode.SecondaryTouch, m_SecondaryTouchPanel }
            };

            m_ControllerInputBgs = new Dictionary<ControllerInputMode, Image>
            {
                { ControllerInputMode.Trigger, m_TriggerBg },
                { ControllerInputMode.Grip, m_GripBg },
                { ControllerInputMode.PrimaryButton, m_PrimaryBg },
                { ControllerInputMode.SecondaryButton, m_SecondaryBg },
                { ControllerInputMode.Menu, m_MenuBg },
                { ControllerInputMode.Primary2DAxisClick, m_Primary2DAxisClickBg },
                { ControllerInputMode.Secondary2DAxisClick, m_Secondary2DAxisClickBg },
                { ControllerInputMode.Primary2DAxisTouch, m_Primary2DAxisTouchBg },
                { ControllerInputMode.Secondary2DAxisTouch, m_Secondary2DAxisTouchBg },
                { ControllerInputMode.PrimaryTouch, m_PrimaryTouchBg },
                { ControllerInputMode.SecondaryTouch, m_SecondaryTouchBg }
            };

            m_HandExpressionPanels = new Dictionary<string, GameObject>
            {
                { "Poke", m_PokePanel },
                { "Pinch", m_PinchPanel },
                { "Grab", m_GrabPanel },
                { "Thumb", m_ThumbPanel },
                { "Open", m_OpenPanel },
                { "Fist", m_FistPanel }
            };

            m_HandExpressionBgs = new Dictionary<string, Image>
            {
                { "Poke", m_PokePanelBg },
                { "Pinch", m_PinchPanelBg },
                { "Grab", m_GrabPanelBg },
                { "Thumb", m_ThumbPanelBg },
                { "Open", m_OpenPanelBg },
                { "Fist", m_FistPanelBg }
            };

            InitializeCustomHandExpressionPanels();
        }

        private void InitializeCustomHandExpressionPanels()
        {
            foreach (var handExpression in m_HandExpressionManager.simulatedHandExpressions)
                if (!m_HandExpressionPanels.ContainsKey(handExpression.name) && handExpression.isQuickAction)
                {
                    var panel = Instantiate(m_CustomPanel, m_CustomPanel.transform.parent);
                    panel.name = $"{handExpression.name}Panel";

                    var bgImage = panel.GetComponentInChildren<Image>();
                    if (bgImage == null)
                    {
                        var bgImageGO = Instantiate(new GameObject(), panel.transform);
                        bgImageGO.name = "Bg";
                        bgImage = bgImageGO.AddComponent<Image>();
                    }

                    var textUI = panel.GetComponentInChildren<Text>();
                    if (textUI == null)
                    {
                        var textGO = Instantiate(new GameObject(), panel.transform);
                        textGO.name = "Text";
                        textUI = textGO.AddComponent<Text>();
                        textUI.fontStyle = FontStyle.Bold;
                    }

                    textUI.text = handExpression.name;

                    m_HandExpressionPanels[handExpression.name] = panel;
                    m_HandExpressionBgs[handExpression.name] = bgImage;
                }
        }

        private void HighlightActiveControllerInputMode(Color highlightColor)
        {
            ClearHighlightedControllerPanels();

            if (!m_ControllerInputBgs.ContainsKey(m_Simulator.controllerInputMode))
            {
                var inputModeName = m_Simulator.controllerInputMode.ToString();
                Debug.LogError($"Background for the {inputModeName} controller input mode panel does not exist.", this);
            }

            m_ControllerInputBgs[m_Simulator.controllerInputMode].color = highlightColor;
            m_PreviousControllerInputMode = m_Simulator.controllerInputMode;
        }

        private void HighlightActiveHandInputMode(Color highlightColor)
        {
            ClearHighlightedHandPanels();

            var handExpressionName = m_Simulator.currentHandExpression.name;
            if (string.IsNullOrEmpty(handExpressionName))
                return;

            if (!m_HandExpressionBgs.ContainsKey(handExpressionName))
                Debug.LogError($"Background for the {handExpressionName} hand expression panel does not exist.", this);

            m_HandExpressionBgs[handExpressionName].color = highlightColor;
        }

        private void ClearActiveInputModePanels()
        {
            m_BothControllersPanel.SetActive(false);
            m_LeftControllerPanel.SetActive(false);
            m_RightControllerPanel.SetActive(false);
            m_BothHandsPanel.SetActive(false);
            m_LeftHandPanel.SetActive(false);
            m_RightHandPanel.SetActive(false);
            m_HMDPanel.SetActive(false);
        }

        private void ClearActiveGeneralInputPanels()
        {
            m_TranslateForwardPanel.SetActive(false);
            m_TranslateBackwardPanel.SetActive(false);
            m_TranslateUpPanel.SetActive(false);
            m_TranslateDownPanel.SetActive(false);
            m_TranslateLeftPanel.SetActive(false);
            m_TranslateRightPanel.SetActive(false);
            m_RotateUpPanel.SetActive(false);
            m_RotateDownPanel.SetActive(false);
            m_RotateLeftPanel.SetActive(false);
            m_RotateRightPanel.SetActive(false);
        }

        private void ClearHighlightedHandPanels()
        {
            foreach (var bg in m_HandExpressionBgs.Values) bg.color = k_DefaultPanelColor;
        }

        private void ClearHighlightedControllerPanels()
        {
            foreach (var bg in m_ControllerInputBgs.Values) bg.color = k_DefaultPanelColor;
        }
    }
}