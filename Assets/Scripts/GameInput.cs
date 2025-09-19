using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    private const string PLAYER_PREFS_BINDINGS = "InputBindings";

    public static GameInput Instance { get; private set; }

    public event EventHandler OnInteractAction;
    public event EventHandler OnInteractAlternativeAction;
    public event EventHandler OnPauseAction;
    public event EventHandler OnBindingRebind;

    public enum Binding
    {
        MoveUp,
        MoveDown,
        MoveLeft,
        MoveRight,
        Interact,
        InteractAlternate,
    }

    private PlayerInputActions playerInputActions;

    private void Awake()
    {
        Instance = this;

        playerInputActions = new PlayerInputActions();

        if (PlayerPrefs.HasKey(PLAYER_PREFS_BINDINGS))
        {
            playerInputActions.LoadBindingOverridesFromJson(PlayerPrefs.GetString(PLAYER_PREFS_BINDINGS));
        }
        
        playerInputActions.Player.Enable();
        playerInputActions.Player.Interact.performed += Interact_performed;
        playerInputActions.Player.InteractAlternative.performed += InteractAlternative_performed;
        playerInputActions.Player.Pause.performed += Pause_performed;
    }

    private void OnDestroy()
    {
        playerInputActions.Player.Interact.performed -= Interact_performed;
        playerInputActions.Player.InteractAlternative.performed -= InteractAlternative_performed;
        playerInputActions.Player.Pause.performed -= Pause_performed;

        playerInputActions.Dispose();
    }

    private void Pause_performed(InputAction.CallbackContext obj)
    {
        OnPauseAction?.Invoke(this, EventArgs.Empty);
    }

    private void InteractAlternative_performed(InputAction.CallbackContext obj)
    {
        OnInteractAlternativeAction?.Invoke(this, EventArgs.Empty);
    }

    private void Interact_performed(InputAction.CallbackContext context)
    {
        OnInteractAction?.Invoke(this, EventArgs.Empty);
    }

    public Vector2 GameInputNormalized()
    {      
        Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();
        return inputVector;
    }

    public string GetBindingText(Binding binding)
    {
        switch (binding)
        {
            default:
            case GameInput.Binding.MoveUp:
                return playerInputActions.Player.Move.bindings[1].ToDisplayString();
            case GameInput.Binding.MoveDown:
                return playerInputActions.Player.Move.bindings[2].ToDisplayString();
            case GameInput.Binding.MoveLeft:
                return playerInputActions.Player.Move.bindings[3].ToDisplayString();
            case GameInput.Binding.MoveRight:
                return playerInputActions.Player.Move.bindings[4].ToDisplayString();
            case GameInput.Binding.Interact:
                return playerInputActions.Player.Interact.bindings[0].ToDisplayString();
            case GameInput.Binding.InteractAlternate:
                return playerInputActions.Player.InteractAlternative.bindings[0].ToDisplayString();
        }
    }

    public void RebindBinding(Binding binding, Action onActionRebound)
    {
        playerInputActions.Player.Disable();

        InputAction inputAction;
        int indexBinding;

        switch (binding)
        {
            default:
            case GameInput.Binding.MoveUp:
                inputAction = playerInputActions.Player.Move;
                indexBinding = 1;
                break;
            case GameInput.Binding.MoveDown:
                inputAction = playerInputActions.Player.Move;
                indexBinding = 2;
                break;
            case GameInput.Binding.MoveLeft:
                inputAction = playerInputActions.Player.Move;
                indexBinding = 3;
                break;
            case GameInput.Binding.MoveRight:
                inputAction = playerInputActions.Player.Move;
                indexBinding = 4;
                break;
            case GameInput.Binding.Interact:
                inputAction = playerInputActions.Player.Interact;
                indexBinding = 0;
                break;
            case GameInput.Binding.InteractAlternate:
                inputAction = playerInputActions.Player.InteractAlternative;
                indexBinding = 0;
                break;
        }

        inputAction.PerformInteractiveRebinding(indexBinding)
            .OnComplete(callback =>
            {
                callback.Dispose();
                playerInputActions.Player.Enable();
                onActionRebound();

                PlayerPrefs.SetString(PLAYER_PREFS_BINDINGS, playerInputActions.SaveBindingOverridesAsJson());

                OnBindingRebind?.Invoke(this, EventArgs.Empty);
            })
            .Start();
    }
}
