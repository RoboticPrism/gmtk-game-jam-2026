using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PlayerUpgrades : MonoBehaviour
{
    public enum UpgradeTypes { LIGHT_RADIUS, RESOURCE_GAIN, DAMAGE }

    [System.Serializable]
    public class Upgrade
    {
        public UpgradeTypes upgradeType;
        public string upgradeName;
        public int currentLevel = 1;
        public int maxLevel;
        public List<UpgradeCost> levelCosts = new List<UpgradeCost>();
    }

    [System.Serializable]
    public class UpgradeCost
    {
        public int level;
        public List<UpgradeSubcost> subcosts;
    }

    [System.Serializable]
    public class UpgradeSubcost
    {
        public ResourceType resourceType;
        public int amount;
    }

    [SerializeField]
    List<Upgrade> currentUpgrades = new List<Upgrade>();

    [SerializeField]
    public float playerDamage = 1f;

    [SerializeField]
    public float damageIncreaseIncrement = 0.5f;

    [SerializeField]
    public int lightRadiusIncreaseIncrement = 1;

    [SerializeField]
    public int resourceGainIncrease = 0;

    [SerializeField]
    public int resourceGainIncreaseIncrement = 1;

    public Upgrade GetUpgradeByType(UpgradeTypes upgradeType)
    {
        return currentUpgrades.Find(upgrade => upgrade.upgradeType == upgradeType);
    }

    public int GetLevelForUpgradeType(UpgradeTypes upgradeType)
    {
        return GetUpgradeByType(upgradeType).currentLevel;
    }

    public UpgradeCost GetCostForNextLevelForUpgradeType(UpgradeTypes upgradeType)
    {
        int nextLevel = GetLevelForUpgradeType(upgradeType) + 1;
        return CheckCostForLevel(upgradeType, nextLevel);
    }

    public UpgradeCost CheckCostForLevel(UpgradeTypes upgradeType, int level)
    {
        Upgrade targetUpgrade = currentUpgrades.First(upgrade => upgrade.upgradeType == upgradeType);
        UpgradeCost cost = targetUpgrade.levelCosts.First(cost => cost.level == level);
        return cost;
    }

    public bool CheckCanBuyUpgradeForLevel(UpgradeTypes upgradeType, int level)
    {
        Upgrade targetUpgrade = currentUpgrades.First(upgrade => upgrade.upgradeType == upgradeType);
        UpgradeCost cost = targetUpgrade.levelCosts.First(cost => cost.level == level);

        bool canAfford = true;

        foreach(UpgradeSubcost subcost in cost.subcosts)
        {
            if(Player.singleton.playerInventory.GetHeldAmount(subcost.resourceType) < subcost.amount)
            {
                canAfford = false;
            }
        }

        return canAfford;
    }

    public bool CheckCanBuyUpgradeForNextLevel(UpgradeTypes upgradeType)
    {
        Upgrade targetUpgrade = currentUpgrades.First(upgrade => upgrade.upgradeType == upgradeType);
        UpgradeCost cost = targetUpgrade.levelCosts.First(cost => cost.level == GetLevelForUpgradeType(upgradeType) + 1);

        bool canAfford = true;

        foreach(UpgradeSubcost subcost in cost.subcosts)
        {
            if(Player.singleton.playerInventory.GetHeldAmount(subcost.resourceType) < subcost.amount)
            {
                canAfford = false;
            }
        }

        return canAfford;
    }

    public void BuyUpgradeForNextLevel(UpgradeTypes upgradeType)
    {
        Upgrade targetUpgrade = currentUpgrades.First(upgrade => upgrade.upgradeType == upgradeType);
        UpgradeCost cost = targetUpgrade.levelCosts.First(cost => cost.level == GetLevelForUpgradeType(upgradeType) + 1);

        foreach(UpgradeSubcost subcost in cost.subcosts)
        {
            Player.singleton.playerInventory.RemoveItemFromInventory(subcost.resourceType, subcost.amount);
        }

        UpgradeStat(upgradeType);
    }
    
    public void UpgradeStat(UpgradeTypes upgradeType)
    {
        Upgrade targetUpgrade = currentUpgrades.First(upgrade => upgrade.upgradeType == upgradeType);
        if(targetUpgrade.currentLevel < targetUpgrade.maxLevel)
        {
            targetUpgrade.currentLevel++;
        }

        switch (upgradeType)
        {
            case UpgradeTypes.LIGHT_RADIUS:
                GetComponent<FogOfWarLight>().lightRadius += lightRadiusIncreaseIncrement;
                FogOfWarManager.TriggerLightingUpdate();
                break;
            case UpgradeTypes.DAMAGE:
                playerDamage += damageIncreaseIncrement;
                break;
            case UpgradeTypes.RESOURCE_GAIN:
                resourceGainIncrease += resourceGainIncreaseIncrement;
                break;
        }
    }
}
