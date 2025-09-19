using Unity.VisualScripting;
using UnityEngine;

public class ClearCounter : BaseCounter
{
    public override void Interact(Player player)
    {
        if (!HasKitchenObject())
        {
            // N�o tem objeto de cozinha aqui
            if (player.HasKitchenObject())
            {
                // Jogador tem um objeto de cozinha
                player.GetKitchenObject().SetKitchenObjectParent(this);
            }
            else
            { 
                // Jogador n�o tem um objeto de cozinha
            }
        }
        else
        {   // Tem objeto de cozinha aqui
            if (player.HasKitchenObject())
            {   // Jogador tem um objeto de cozinha
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {   // Jogador est� segurando um prato
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectsSO()))
                    {
                        GetKitchenObject().DestroySelf();
                    }
                }
                else
                {   // Jogador n�o est� segurando um prato mas outro objeto de cozinha
                    if(GetKitchenObject().TryGetPlate(out plateKitchenObject))
                    {   // Aqui tem um prato
                        if (plateKitchenObject.TryAddIngredient(player.GetKitchenObject().GetKitchenObjectsSO()))
                        {
                            player.GetKitchenObject().DestroySelf();
                        }
                    }
                }
            }
            else
            {   // Jogador n�o tem um objeto de cozinha
                GetKitchenObject().SetKitchenObjectParent(player);
            }
        }
    }
}
