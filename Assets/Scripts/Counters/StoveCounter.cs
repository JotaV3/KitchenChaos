using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class StoveCounter : BaseCounter, IHasProgress
{
    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
    public event EventHandler<OnStateChangedEventArgs> OnStateChanged;
    public class OnStateChangedEventArgs : EventArgs
    {
        public State state;
    }

    public enum State
    {
        Idle,
        Frying,
        Fried,
        Burned,
    }

    [SerializeField] private FryingRecipeSO[] fryingRecipeSOArray;
    [SerializeField] private BurningRecipeSO[] burningRecipeSOArray;

    private NetworkVariable<State> state = new NetworkVariable<State>(State.Idle);
    private NetworkVariable<float> fryingTimer = new NetworkVariable<float>(0f);
    private FryingRecipeSO fryingRecipeSO;
    private NetworkVariable<float> burningTimer = new NetworkVariable<float>(0f);
    private BurningRecipeSO burningRecipeSO;

    public override void OnNetworkSpawn()
    {
        fryingTimer.OnValueChanged += FryingTimer_OnValueChanged;
        burningTimer.OnValueChanged += BurningTimer_OnValueChanged;
        state.OnValueChanged += State_OnValueChanged;
    }

    private void FryingTimer_OnValueChanged(float previousValue, float newValue)
    {
        float fryingTimerMax = fryingRecipeSO != null ? fryingRecipeSO.fryingTimerMax : 1f;

        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
        {
            progressNormalized = fryingTimer.Value / fryingTimerMax
        });
    }

    private void BurningTimer_OnValueChanged(float previousValue, float newValue)
    {
        float burningTimerMax = burningRecipeSO != null ? burningRecipeSO.burningTimerMax : 1f;

        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
        {
            progressNormalized = burningTimer.Value / burningTimerMax
        });
    }

    private void State_OnValueChanged(State previousState, State newState)
    {
        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
        {
            state = state.Value
        });

        if(state.Value == State.Idle || state.Value == State.Burned)
        {
            OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
            {
                progressNormalized = 0f
            });
        }
    }

    private void Update()
    {
        if (!IsServer) return;

        switch (state.Value)
        {
            case State.Idle: 
                break;

            case State.Frying:
                fryingTimer.Value += Time.deltaTime;

                if (fryingTimer.Value >= fryingRecipeSO.fryingTimerMax)
                {
                    GameMultiplayerManager.Instance.DestroyKitchenObject(GetKitchenObject());

                    KitchenObject.SpawnKitchenObject(fryingRecipeSO.output, this);

                    burningTimer.Value = 0;
                    SetBurningRecipeSOClientRpc(
                        GameMultiplayerManager.Instance.GetKitchenObjectSOIndex(GetKitchenObject().GetKitchenObjectsSO())
                    );
                    

                    state.Value = State.Fried;     
                }
                break;

            case State.Fried:
                burningTimer.Value += Time.deltaTime;

                if(burningTimer.Value >= burningRecipeSO.burningTimerMax)
                {
                    GetKitchenObject().DestroySelf();

                    KitchenObject.SpawnKitchenObject(burningRecipeSO.output, this);

                    state.Value = State.Burned;
                }
                break;

            case State.Burned:
                break;
        }
    }

    public override void Interact(Player player)
    {
        if (!HasKitchenObject())
        {   // não tem objeto de cozinha aqui
            if (player.HasKitchenObject())
            {   // jogador tem objeto de cozinha
                if (HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectsSO()))
                {   // o objeto de cozinha faz parte da receita
                    KitchenObject kitchenObject = player.GetKitchenObject();
                    kitchenObject.SetKitchenObjectParent(this);

                    InteractLogicPlaceObjectOnCounterServerRpc(
                        GameMultiplayerManager.Instance.GetKitchenObjectSOIndex(kitchenObject.GetKitchenObjectsSO())
                    );                   
                }
            }
            else
            {   // jogador não tem objeto de cozinha
            }
        }
        else
        {   // Tem objeto de cozinha aqui
            if (player.HasKitchenObject())
            {   // Jogador tem objeto de cozinha
                if(player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {   // Jogador tem um prato
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectsSO()))
                    {
                        GameMultiplayerManager.Instance.DestroyKitchenObject(GetKitchenObject());

                        state.Value = State.Idle;
                    }
                }
            }
            else
            {   // jogador não tem objeto de cozinha
                GetKitchenObject().SetKitchenObjectParent(player);
                SetStateIdleServerRpc();
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetStateIdleServerRpc()
    {
        state.Value = State.Idle;
    }

    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicPlaceObjectOnCounterServerRpc(int kitchenObjectSOIndex)
    {
        fryingTimer.Value = 0;
        state.Value = State.Frying;

        SetFryingRecipeSOClientRpc(kitchenObjectSOIndex);
    }

    [ClientRpc]
    private void SetFryingRecipeSOClientRpc(int kitchenObjectSOIndex)
    {
        KitchenObjectsSO kitchenObjectSO = GameMultiplayerManager.Instance.GetKitchenObjectSOFromIndex(kitchenObjectSOIndex);
        fryingRecipeSO = GetFryingRecipeWithInput(kitchenObjectSO);    
    }

    [ClientRpc]
    private void SetBurningRecipeSOClientRpc(int kitchenObjectSOIndex)
    {
        KitchenObjectsSO kitchenObjectSO = GameMultiplayerManager.Instance.GetKitchenObjectSOFromIndex(kitchenObjectSOIndex);
        burningRecipeSO = GetBurningRecipeWithInput(kitchenObjectSO);
    }

    private BurningRecipeSO GetBurningRecipeWithInput(KitchenObjectsSO inputKitchenObject)
    {
        foreach(BurningRecipeSO burningRecipeSO in burningRecipeSOArray)
        {
            if (burningRecipeSO.input == inputKitchenObject)
            {
                return burningRecipeSO;
            }
        }

        return null;
    }

    private FryingRecipeSO GetFryingRecipeWithInput(KitchenObjectsSO inputKitchenObjectSO)
    {
        foreach (FryingRecipeSO fryingRecipeSO in fryingRecipeSOArray)
        {
            if(fryingRecipeSO.input == inputKitchenObjectSO)
            {
                return fryingRecipeSO;
            }
        }

        return null;
    }

    private bool HasRecipeWithInput(KitchenObjectsSO inputKitchenObjectsSO)
    {
        if (GetFryingRecipeWithInput(inputKitchenObjectsSO) != null)
        {
            return true;
        }

        return false;
    }

    public bool IsFried()
    {
        return state.Value == State.Fried;
    }
}
