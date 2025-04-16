using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerTankInput : MonoBehaviour, ITankInput
{
    private Vector2 moveInput;
    private Vector2 lookInput;
    private bool firePressed;

    private TankControls controls;

    public Vector2 MoveInput => moveInput;
    public bool FirePressed => firePressed;
    public Vector2 LookVector => lookInput;

    private void Awake()
    {
        controls = new TankControls();  // Instantiate the generated TankControls class
        
        // Set up movement input (WASD and Gamepad Left Stick)
        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += _ => moveInput = Vector2.zero;
        // Set up look movement using Mouse
        controls.Player.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        controls.Player.Look.canceled += _ => lookInput = Vector2.zero;
        // Set up firing input (Mouse or Gamepad button)
        controls.Player.Fire.performed += _ => firePressed = true;
        controls.Player.Fire.canceled += _ => firePressed = false;
    }

    private void OnEnable() => controls.Enable();
    private void OnDisable() => controls.Disable();
}
