using UnityEngine;
using System.Collections.Generic;

public class BumpableCrafter : BumpableTile
{
    
    [System.Serializable]
    public class CraftCost
    {
        public ResourceType resourceType;
        public int amount;
    }

    public List<CraftCost> craftCosts;

    public override void OnBump()
    {
        bool hasCost = true;
        foreach(CraftCost cost in craftCosts)
        {
            if(Player.singleton.playerInventory.GetHeldAmount(cost.resourceType) < cost.amount) {
                hasCost = false;
            }
        }

        if (hasCost)
        {
            OnCraft();
            foreach(CraftCost cost in craftCosts)
            {
                Player.singleton.playerInventory.RemoveItemFromInventory(cost.resourceType, cost.amount);
            }
        }
        else
        {
            Debug.Log("Not enough resources!");
        }
    }

    public void OnCraft()
    {
        TurretManager.singleton.AssembleTurret();
    }
}
