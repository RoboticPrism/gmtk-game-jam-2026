using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Cost
{
    public int level;
    public List<Subcost> subcosts;

    public bool canPlayerAfford()
    {
        bool canAfford = true;

        foreach (Subcost subcost in subcosts)
        {
            if (Player.singleton.playerInventory.GetHeldAmount(subcost.resourceType) < subcost.amount)
            {
                canAfford = false;
            }
        }

        return canAfford;
    }

    public void payCost()
    {
        foreach(Subcost subcost in subcosts)
        {
            Player.singleton.playerInventory.RemoveItemFromInventory(subcost.resourceType, subcost.amount);
        }
    }
}

[System.Serializable]
public class Subcost
{
    public ResourceType resourceType;
    public int amount;
}
