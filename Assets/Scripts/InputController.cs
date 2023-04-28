using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public enum InputControllerState
{
	NO_INPUT_DEVICE_CONNECTED,
	NO_CONTROLLER_CONNECTED,
	NO_KEYBOARD_CONNECTED,
	BOTH_KEYBOARD_AND_CONTROLLER_CONNECTED
}
public class InputController : MonoBehaviour
{
	//private ServiceLocator ServiceLocator;

	
	[HideInInspector]
	public List<GameObject> Subscribers = new List<GameObject>();
	[HideInInspector]
	public Vector2 MousePositionOnScreen;
	[HideInInspector]
	public Vector2 LookInputVector;
	[HideInInspector]
	public Vector2 MovementInputVector;
	[HideInInspector]
	public bool WasJumpReleased;
	[HideInInspector]
	public bool WasJumpPressed;
	[HideInInspector]
	public bool WasStartReleased;
	[HideInInspector]
	public bool WasPausePressed;
	[HideInInspector]
	public bool WasAction1Released;
	[HideInInspector]
	public bool WasAction1Pressed;
	[HideInInspector]
	public bool WasRunReleased;
	[HideInInspector]
	public bool WasRunPressed;
	[HideInInspector]
	public bool IsRunDown;
	[HideInInspector]
	public bool WasAction2Released; 
	[HideInInspector]
	public bool WasAction2Pressed;
	
	[HideInInspector]
	public bool WasFreeLookPressed; 
	[HideInInspector]
	public bool WasFreeLookReleased;

	[HideInInspector]
	public bool WasTargetLockPressed; 
	[HideInInspector]
	public bool WasTargetLockReleased;

	public InputControllerState InputControllerState;
	private Gamepad gamepad;
	private Keyboard keyboard;

	private float MouseSensitivity = 1.0f / 10.0f; //TODO - move to data model & get from repo
	private static float dampen_webGL_mouse = 0.5f;
	private static float controller_look_sensitivity = 120.0f;
	private static float default_up_value = 1.0f;
	private bool InputDetected = false;	
	
	// protected void Awake()
	// {
	// 	ServiceLocator = FindObjectOfType<ServiceLocator>();
	// }
	
	private void Start()
	{
		gamepad = Gamepad.current;
		keyboard = Keyboard.current;
	}

	public Vector3 MovementInput3Vector {get {return InputService.ConvertV2toV3(MovementInputVector,default_up_value);} }
	public Vector3 MovementInput3BinaryVector  {get {return InputService.GetBinaryVector3(InputService.ConvertV2toV3(MovementInputVector,default_up_value));} }
	public Vector2 MovementInputBinaryVector  {get {return InputService.GetBinaryVector2(MovementInputVector);} }
	
	private static float InputEventThrottleMax = 1.0f;
	private float InputEventThrottle = 0.0f;
	
	public void ResetInputThrottle()
	{
		InputEventThrottle = 0;
	}
	
	public bool CheckInputThrottle()
	{
		if(InputEventThrottle < InputEventThrottleMax)
		{
			return false;
		}
		else
		{
			return true;
		}
	}
	
	private void Update()
	{
		InputEventThrottle += Time.deltaTime;
		InputDetected = false;
		MovementInputVector = Vector2.zero;
		LookInputVector = Vector2.zero;
		UpdateLookInput();
		UpdateMovementInput();
		UpdateButtons();
		if(MovementInputVector != Vector2.zero || LookInputVector != Vector2.zero)
		{
			InputDetected = true;
		}
		if(InputDetected)
		{
			//ServiceLocator.gameObject.SendMessage("OnInputEvent", SendMessageOptions.DontRequireReceiver);
			Subscribers.ForEach(subscriber => subscriber.SendMessage("OnInputEvent",SendMessageOptions.DontRequireReceiver));
		}
	}

	private void FixedUpdate()
	{
		UpdateInputDevices();
	}

	private void UpdateInputDevices()
	{
		if (gamepad == null)
		{
			gamepad = Gamepad.current;
		}
		if (keyboard == null)
		{
			keyboard = Keyboard.current;
		}
		if (gamepad == null && keyboard != null)
		{
#if UNITY_EDITOR
			if (InputControllerState != InputControllerState.NO_CONTROLLER_CONNECTED)
			{
				Debug.Log("CONTROLLER DISCONNECTED");
			}
#endif
			InputControllerState = InputControllerState.NO_CONTROLLER_CONNECTED;
			return;
		}
		if (gamepad != null && keyboard != null)
		{
#if UNITY_EDITOR
			if (InputControllerState != InputControllerState.BOTH_KEYBOARD_AND_CONTROLLER_CONNECTED)
			{
				Debug.Log("CONTROLLER CONNECTED");
			}
#endif
			InputControllerState = InputControllerState.BOTH_KEYBOARD_AND_CONTROLLER_CONNECTED;
			return;
		}
		if (gamepad == null && keyboard == null)
		{
#if UNITY_EDITOR
			if (InputControllerState != InputControllerState.NO_INPUT_DEVICE_CONNECTED)
			{
				Debug.Log("CONTROLLER DISCONNECTED. KEYBOARD DISCONNECTED");
			}
#endif
			InputControllerState = InputControllerState.NO_INPUT_DEVICE_CONNECTED;
			return;
		}
		if (gamepad != null && keyboard == null)
		{
#if UNITY_EDITOR
			if (InputControllerState != InputControllerState.NO_KEYBOARD_CONNECTED)
			{
				Debug.Log("KEYBOARD DISCONNECTED");
			}
#endif
			InputControllerState = InputControllerState.NO_KEYBOARD_CONNECTED;
			return;
		}
	}
    
	public bool IsAnyButtonDown()
	{
		return	WasAction1Pressed || 
			WasAction2Pressed  || 
			WasJumpPressed  || 
			WasPausePressed  ||
			WasRunPressed ||
			WasTargetLockPressed || 
			WasFreeLookPressed ;
	}
	
	public bool WasAnyButtonReleased()
	{
		return WasAction1Released || 
			WasAction2Released ||
			WasJumpReleased || 
			WasStartReleased || 
			WasRunReleased || 
			WasTargetLockReleased ||  
			WasFreeLookReleased;
	}
	private void UpdateButtons()
	{
		WasJumpReleased = false;
		WasJumpPressed = false;
		WasStartReleased= false;
		WasPausePressed= false;
		WasAction1Pressed= false;
		WasAction1Released= false;
		WasRunPressed= false;
		WasRunReleased= false;
		WasAction2Pressed= false;
		WasAction2Released= false;
		//TODO - check DataRepo for custom inputs
		if (gamepad != null)
		{
			WasJumpReleased = gamepad.buttonSouth.wasReleasedThisFrame;
			WasJumpPressed = gamepad.buttonSouth.wasPressedThisFrame;

			WasStartReleased = gamepad.startButton.wasReleasedThisFrame;
			WasPausePressed = gamepad.startButton.wasPressedThisFrame;

			WasAction1Pressed = gamepad.buttonWest.wasPressedThisFrame;
			WasAction1Released = gamepad.buttonWest.wasReleasedThisFrame;

			WasRunPressed = gamepad.buttonEast.wasPressedThisFrame;
			WasRunReleased = gamepad.buttonEast.wasReleasedThisFrame;
			IsRunDown = gamepad.buttonEast.isPressed;

			WasAction2Pressed = gamepad.buttonNorth.wasPressedThisFrame;
			WasAction2Released = gamepad.buttonNorth.wasReleasedThisFrame;
		}
		if (keyboard != null)
		{
			WasJumpReleased = WasJumpReleased || keyboard.spaceKey.wasReleasedThisFrame;
			WasJumpPressed = WasJumpPressed || keyboard.spaceKey.wasPressedThisFrame;

			WasStartReleased = WasStartReleased || keyboard.enterKey.wasReleasedThisFrame;
			WasPausePressed = WasPausePressed || keyboard.enterKey.wasPressedThisFrame;

			WasAction1Pressed = WasAction1Pressed|| keyboard.eKey.wasPressedThisFrame || Mouse.current.leftButton.wasPressedThisFrame;
			WasAction1Released = WasAction1Released|| keyboard.eKey.wasReleasedThisFrame|| Mouse.current.leftButton.wasReleasedThisFrame;

			WasRunPressed = WasRunPressed||keyboard.shiftKey.wasPressedThisFrame;
			WasRunReleased = WasRunReleased|| keyboard.shiftKey.wasReleasedThisFrame;
        
			WasAction2Pressed = WasAction2Pressed || keyboard.leftCtrlKey.wasPressedThisFrame|| Mouse.current.rightButton.wasPressedThisFrame;
			WasAction2Released = WasAction2Released|| keyboard.leftCtrlKey.wasReleasedThisFrame|| Mouse.current.rightButton.wasReleasedThisFrame;
		}
		if(WasAnyButtonReleased() || IsAnyButtonDown())
		{
			InputDetected = true;
		}
	}

	private void UpdateMovementInput()
	{
		
		MovementInputVector = (gamepad != null ? gamepad.leftStick.ReadValue() : Vector2.zero) + getVectorFromKeyboard();
	}

	private void UpdateLookInput()
	{
		//MousePositionOnScreen =	ServiceLocator.MainMenuUIController.UICamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
		var MouseVector = Mouse.current.delta.ReadValue();
		if (Application.platform == RuntimePlatform.WebGLPlayer)
		{

			MouseVector.x = WebGLDampenMovement(MouseVector.x);
			MouseVector.y = WebGLDampenMovement(MouseVector.y);
		}

		MouseVector = MouseVector / MouseSensitivity;
		LookInputVector = (gamepad != null ? gamepad.rightStick.ReadValue()*controller_look_sensitivity : Vector2.zero) + MouseVector;
	}
    
	public static float WebGLDampenMovement(float value)
	{
		var returnValue = value;
		if (Mathf.Abs(returnValue) > 1000)
		{
			returnValue = returnValue / 2.0f;
		}
		if (Mathf.Abs(returnValue) > 100)
		{
			returnValue = returnValue / 2.0f;
		}
		if (Mathf.Abs(returnValue) > 50)
		{
			returnValue = returnValue / 2.0f;
		}

		if (Mathf.Abs(returnValue) > 35)
		{
			returnValue = Mathf.Sign(value) * 35.0f;
		}

		return returnValue;
	}
    
	private Vector2 getVectorFromKeyboard()
	{
		if (keyboard == null) return Vector2.zero;
		//TODO - check DataRepo for custom inputs
		var forwardKey = keyboard.wKey.IsPressed();
		var leftKey = keyboard.aKey.IsPressed();
		var rightKey = keyboard.dKey.IsPressed();
		var downKey = keyboard.sKey.IsPressed();
		return InputService.Vector2FromBinaryInput(forwardKey, leftKey, rightKey, downKey);
	}
}
