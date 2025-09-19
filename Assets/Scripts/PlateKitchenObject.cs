using NUnit.Framework;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class PlateKitchenObject : KitchenObject
{
    public event EventHandler<OnIngredientAddedEventArgs> OnIngredientAdded;
    public class OnIngredientAddedEventArgs : EventArgs
    {
        public KitchenObjectsSO kitchenObjectsSO;
    }

    [SerializeField] private List<KitchenObjectsSO> validKitchenObjectsSO;

    List<KitchenObjectsSO> kitchenObjectsSOList;

    protected override void Awake()
    {
        base.Awake();
        kitchenObjectsSOList = new List<KitchenObjectsSO>();
    }

    public List<KitchenObjectsSO> GetKitchenObjectsSOList()
    {
        return kitchenObjectsSOList;
    }

    public bool TryAddIngredient(KitchenObjectsSO kitchenObjectsSO)
    {
        if (kitchenObjectsSOList.Contains(kitchenObjectsSO))
        {
            return false;
        }
        if (validKitchenObjectsSO.Contains(kitchenObjectsSO))
        {
            kitchenObjectsSOList.Add(kitchenObjectsSO);

            OnIngredientAdded?.Invoke(this, new OnIngredientAddedEventArgs
            {
                kitchenObjectsSO = kitchenObjectsSO
            });

            return true;
        }
        else
        {
            return false;
        }
    }
}
