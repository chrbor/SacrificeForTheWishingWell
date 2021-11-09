// GENERATED AUTOMATICALLY FROM 'Assets/Scripts/Input/Controls.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @Controls : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @Controls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""Controls"",
    ""maps"": [
        {
            ""name"": ""GameControl"",
            ""id"": ""32a9c1cc-f711-467c-8551-e6cfb56e5a8f"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Button"",
                    ""id"": ""d7b5e849-a1ed-4033-bbb6-ca30efc11a50"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""19471410-fa5d-4649-95c6-78a8ccc95700"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Drop"",
                    ""type"": ""Button"",
                    ""id"": ""9ef7d719-127c-4f36-9cee-3b9f17124d55"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""PickUp"",
                    ""type"": ""Button"",
                    ""id"": ""bd9fd7a0-8ddf-4377-9350-f5e85b5539c2"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Throw"",
                    ""type"": ""Button"",
                    ""id"": ""695543de-bf6a-476b-aa1f-01947da10d28"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Abort"",
                    ""type"": ""Button"",
                    ""id"": ""240d1acc-906f-4719-82e0-ea532e18b761"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Reset"",
                    ""type"": ""Button"",
                    ""id"": ""981e1565-e57b-4c0e-ab46-78a83d659d30"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Tap""
                },
                {
                    ""name"": ""Pause"",
                    ""type"": ""Button"",
                    ""id"": ""257c9a6e-7467-4148-8565-44e857020257"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""0c3ea749-f733-489b-8f66-d3ae931b0faf"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b80beed9-a473-491d-afd4-118eba37430b"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2fe0419c-8c5f-4a36-b004-5ed8cc99fe45"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""PickUp"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""1D Axis"",
                    ""id"": ""95d159d9-805d-4373-9549-273bf6153edc"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""3eab20dd-bea0-47e1-9c50-cc5d107d58b3"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""6a0fe9c7-1c4e-4cc8-87f8-36af92578409"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""1D Axis"",
                    ""id"": ""af18ccdd-15fb-4311-81ab-eec294738f41"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""c2744ce1-5480-40d1-8d10-66c50da84cae"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""f89c6fca-b9f1-4f11-8ac0-7de6cb92dda6"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""60a5e089-6a08-46ca-b509-7cf8c2298551"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": ""Hold"",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""Throw"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""6298bbce-78c8-4294-929a-a21bfbad0e61"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""Abort"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9dc8a0f9-8f6d-4bd9-afa6-332988a3ed9e"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": ""Tap"",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""Drop"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f57f80ef-4473-444b-b1fc-3a973af1177c"",
                    ""path"": ""<Keyboard>/r"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""Reset"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2b5878aa-efae-4165-a92a-9b19c395bec3"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""Pause"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""PC"",
            ""bindingGroup"": ""PC"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // GameControl
        m_GameControl = asset.FindActionMap("GameControl", throwIfNotFound: true);
        m_GameControl_Move = m_GameControl.FindAction("Move", throwIfNotFound: true);
        m_GameControl_Jump = m_GameControl.FindAction("Jump", throwIfNotFound: true);
        m_GameControl_Drop = m_GameControl.FindAction("Drop", throwIfNotFound: true);
        m_GameControl_PickUp = m_GameControl.FindAction("PickUp", throwIfNotFound: true);
        m_GameControl_Throw = m_GameControl.FindAction("Throw", throwIfNotFound: true);
        m_GameControl_Abort = m_GameControl.FindAction("Abort", throwIfNotFound: true);
        m_GameControl_Reset = m_GameControl.FindAction("Reset", throwIfNotFound: true);
        m_GameControl_Pause = m_GameControl.FindAction("Pause", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // GameControl
    private readonly InputActionMap m_GameControl;
    private IGameControlActions m_GameControlActionsCallbackInterface;
    private readonly InputAction m_GameControl_Move;
    private readonly InputAction m_GameControl_Jump;
    private readonly InputAction m_GameControl_Drop;
    private readonly InputAction m_GameControl_PickUp;
    private readonly InputAction m_GameControl_Throw;
    private readonly InputAction m_GameControl_Abort;
    private readonly InputAction m_GameControl_Reset;
    private readonly InputAction m_GameControl_Pause;
    public struct GameControlActions
    {
        private @Controls m_Wrapper;
        public GameControlActions(@Controls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_GameControl_Move;
        public InputAction @Jump => m_Wrapper.m_GameControl_Jump;
        public InputAction @Drop => m_Wrapper.m_GameControl_Drop;
        public InputAction @PickUp => m_Wrapper.m_GameControl_PickUp;
        public InputAction @Throw => m_Wrapper.m_GameControl_Throw;
        public InputAction @Abort => m_Wrapper.m_GameControl_Abort;
        public InputAction @Reset => m_Wrapper.m_GameControl_Reset;
        public InputAction @Pause => m_Wrapper.m_GameControl_Pause;
        public InputActionMap Get() { return m_Wrapper.m_GameControl; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(GameControlActions set) { return set.Get(); }
        public void SetCallbacks(IGameControlActions instance)
        {
            if (m_Wrapper.m_GameControlActionsCallbackInterface != null)
            {
                @Move.started -= m_Wrapper.m_GameControlActionsCallbackInterface.OnMove;
                @Move.performed -= m_Wrapper.m_GameControlActionsCallbackInterface.OnMove;
                @Move.canceled -= m_Wrapper.m_GameControlActionsCallbackInterface.OnMove;
                @Jump.started -= m_Wrapper.m_GameControlActionsCallbackInterface.OnJump;
                @Jump.performed -= m_Wrapper.m_GameControlActionsCallbackInterface.OnJump;
                @Jump.canceled -= m_Wrapper.m_GameControlActionsCallbackInterface.OnJump;
                @Drop.started -= m_Wrapper.m_GameControlActionsCallbackInterface.OnDrop;
                @Drop.performed -= m_Wrapper.m_GameControlActionsCallbackInterface.OnDrop;
                @Drop.canceled -= m_Wrapper.m_GameControlActionsCallbackInterface.OnDrop;
                @PickUp.started -= m_Wrapper.m_GameControlActionsCallbackInterface.OnPickUp;
                @PickUp.performed -= m_Wrapper.m_GameControlActionsCallbackInterface.OnPickUp;
                @PickUp.canceled -= m_Wrapper.m_GameControlActionsCallbackInterface.OnPickUp;
                @Throw.started -= m_Wrapper.m_GameControlActionsCallbackInterface.OnThrow;
                @Throw.performed -= m_Wrapper.m_GameControlActionsCallbackInterface.OnThrow;
                @Throw.canceled -= m_Wrapper.m_GameControlActionsCallbackInterface.OnThrow;
                @Abort.started -= m_Wrapper.m_GameControlActionsCallbackInterface.OnAbort;
                @Abort.performed -= m_Wrapper.m_GameControlActionsCallbackInterface.OnAbort;
                @Abort.canceled -= m_Wrapper.m_GameControlActionsCallbackInterface.OnAbort;
                @Reset.started -= m_Wrapper.m_GameControlActionsCallbackInterface.OnReset;
                @Reset.performed -= m_Wrapper.m_GameControlActionsCallbackInterface.OnReset;
                @Reset.canceled -= m_Wrapper.m_GameControlActionsCallbackInterface.OnReset;
                @Pause.started -= m_Wrapper.m_GameControlActionsCallbackInterface.OnPause;
                @Pause.performed -= m_Wrapper.m_GameControlActionsCallbackInterface.OnPause;
                @Pause.canceled -= m_Wrapper.m_GameControlActionsCallbackInterface.OnPause;
            }
            m_Wrapper.m_GameControlActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Move.started += instance.OnMove;
                @Move.performed += instance.OnMove;
                @Move.canceled += instance.OnMove;
                @Jump.started += instance.OnJump;
                @Jump.performed += instance.OnJump;
                @Jump.canceled += instance.OnJump;
                @Drop.started += instance.OnDrop;
                @Drop.performed += instance.OnDrop;
                @Drop.canceled += instance.OnDrop;
                @PickUp.started += instance.OnPickUp;
                @PickUp.performed += instance.OnPickUp;
                @PickUp.canceled += instance.OnPickUp;
                @Throw.started += instance.OnThrow;
                @Throw.performed += instance.OnThrow;
                @Throw.canceled += instance.OnThrow;
                @Abort.started += instance.OnAbort;
                @Abort.performed += instance.OnAbort;
                @Abort.canceled += instance.OnAbort;
                @Reset.started += instance.OnReset;
                @Reset.performed += instance.OnReset;
                @Reset.canceled += instance.OnReset;
                @Pause.started += instance.OnPause;
                @Pause.performed += instance.OnPause;
                @Pause.canceled += instance.OnPause;
            }
        }
    }
    public GameControlActions @GameControl => new GameControlActions(this);
    private int m_PCSchemeIndex = -1;
    public InputControlScheme PCScheme
    {
        get
        {
            if (m_PCSchemeIndex == -1) m_PCSchemeIndex = asset.FindControlSchemeIndex("PC");
            return asset.controlSchemes[m_PCSchemeIndex];
        }
    }
    public interface IGameControlActions
    {
        void OnMove(InputAction.CallbackContext context);
        void OnJump(InputAction.CallbackContext context);
        void OnDrop(InputAction.CallbackContext context);
        void OnPickUp(InputAction.CallbackContext context);
        void OnThrow(InputAction.CallbackContext context);
        void OnAbort(InputAction.CallbackContext context);
        void OnReset(InputAction.CallbackContext context);
        void OnPause(InputAction.CallbackContext context);
    }
}
