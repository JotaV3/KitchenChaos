using Unity.VisualScripting;
using UnityEngine;

public class ClearCounter : BaseCounter
{
    public override void Interact(Player player)
    {
        if (!HasKitchenObject())
        {
            // Não tem objeto de cozinha aqui
            if (player.HasKitchenObject())
            {
                // Jogador tem um objeto de cozinha
                player.GetKitchenObject().SetKitchenObjectParent(this);
            }
            else
            { 
                // Jogador não tem um objeto de cozinha
            }
        }
        else
        {   // Tem objeto de cozinha aqui
            if (player.HasKitchenObject())
            {   // Jogador tem um objeto de cozinha
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {   // Jogador está segurando um prato
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectsSO()))
                    {
                        GetKitchenObject().DestroySelf();
                    }
                }
                else
                {   // Jogador não está segurando um prato mas outro objeto de cozinha
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
            {   // Jogador não tem um objeto de cozinha
                GetKitchenObject().SetKitchenObjectParent(player);
            }
        }
    }
}
