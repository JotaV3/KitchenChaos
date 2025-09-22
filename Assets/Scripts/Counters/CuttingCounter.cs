using System;
using Unity.Netcode;
using UnityEngine;

public class CuttingCounter : BaseCounter, IHasProgress
{
    public static event EventHandler OnAnyCut;

    new public static void ResetStaticData()
    {
        OnAnyCut = null;
    }

    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
    public event EventHandler OnCut;

    [SerializeField] CuttingRecipeSO[] cuttingRecipeSOArray;

    private int cuttingProgress;

    public override void Interact(Player player)
    {
        if(!HasKitchenObject())
        {
            // Não tem objeto de cozinha aqui
            if (player.HasKitchenObject())
            {
                // Jogador tem objeto de cozinha
                if (HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectsSO()))
                {
                    KitchenObject kitchenObject = player.GetKitchenObject(); 
                    kitchenObject.SetKitchenObjectParent(this);

                    InteractLogicPlaceObjectOnCounterServerRpc();
                }
            }
            else
            {
                // Jogador não tem objeto de cozinha
            }
        }
        else
        {
            // Tem objeto de cozinha aqui
            if (player.HasKitchenObject())
            {
                if(player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectsSO()))
                    {
                        GetKitchenObject().DestroySelf();
                    }
                }
            }
            else
            {
                // Jogador não tem objeto de cozinha
                GetKitchenObject().SetKitchenObjectParent(player);
            }
        }      
    }

    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicPlaceObjectOnCounterServerRpc()
    {
        InteractLogicPlaceObjectOnCounterClientRpc();
    }

    [ClientRpc]
    private void InteractLogicPlaceObjectOnCounterClientRpc()
    {
        cuttingProgress = 0;

        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
        {
            progressNormalized = 0f
        });
    }

    public override void InteractAlternative(Player player)
    {
        if (HasKitchenObject() && HasRecipeWithInput(GetKitchenObject().GetKitchenObjectsSO()))
        {
            CutObjectServerRpc();
            TestCuttingProgressDoneServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void CutObjectServerRpc()
    {
        CutObjectClientRpc();
    }

    [ClientRpc]
    private void CutObjectClientRpc()
    {
        cuttingProgress++;

        OnCut?.Invoke(this, EventArgs.Empty);
        OnAnyCut?.Invoke(this, EventArgs.Empty);
        
        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectsSO());

        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
        {
            progressNormalized = (float)cuttingProgress / cuttingRecipeSO.cuttingProgressMax
        });

        
    }

    [ServerRpc(RequireOwnership = false)]
    private void TestCuttingProgressDoneServerRpc()
    {
        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectsSO());

        if (cuttingProgress >= cuttingRecipeSO.cuttingProgressMax)
        {// Jogador encheu a barra de progresso
            KitchenObjectsSO outputKitchenObjectSO = GetOutputForInput(GetKitchenObject().GetKitchenObjectsSO());
            GameMultiplayerManager.Instance.DestroyKitchenObject(GetKitchenObject());

            KitchenObject.SpawnKitchenObject(outputKitchenObjectSO, this);
        }
    }

    private CuttingRecipeSO GetCuttingRecipeSOWithInput(KitchenObjectsSO inputKitchenObjectsSO)
    {
        foreach(CuttingRecipeSO cuttingRecipeSO in cuttingRecipeSOArray)
        {
            if(cuttingRecipeSO.input == inputKitchenObjectsSO)
            {
                return cuttingRecipeSO;
            }
        }

        return null;
    }
    
    private bool HasRecipeWithInput(KitchenObjectsSO inputKitchenObjectSO)
    {
        if(GetCuttingRecipeSOWithInput(inputKitchenObjectSO) != null)
        {
            return true;
        }

        return false;
    }

    private KitchenObjectsSO GetOutputForInput(KitchenObjectsSO inputKitchenObjectSO)
    {
        CuttingRecipeSO cuttingObjectSO = GetCuttingRecipeSOWithInput(inputKitchenObjectSO);

        if(cuttingObjectSO != null)
        {
            return cuttingObjectSO.output;
        }

        return null;
    }
}
