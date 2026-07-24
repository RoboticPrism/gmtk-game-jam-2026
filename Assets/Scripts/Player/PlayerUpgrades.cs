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
        public List<Cost> levelCosts = new List<Cost>();
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

    public Cost GetCostForNextLevelForUpgradeType(UpgradeTypes upgradeType)
    {
        int nextLevel = GetLevelForUpgradeType(upgradeType) + 1;
        return CheckCostForLevel(upgradeType, nextLevel);
    }

    public Cost CheckCostForLevel(UpgradeTypes upgradeType, int level)
    {
        Upgrade targetUpgrade = currentUpgrades.First(upgrade => upgrade.upgradeType == upgradeType);
        Cost cost = targetUpgrade.levelCosts.First(cost => cost.level == level);
        return cost;
    }

    public bool CheckCanBuyUpgradeForLevel(UpgradeTypes upgradeType, int level)
    {
        Upgrade targetUpgrade = currentUpgrades.First(upgrade => upgrade.upgradeType == upgradeType);
        Cost cost = targetUpgrade.levelCosts.First(cost => cost.level == level);
        return cost.canPlayerAfford();
    }

    public bool CheckCanBuyUpgradeForNextLevel(UpgradeTypes upgradeType)
    {
        Upgrade targetUpgrade = currentUpgrades.First(upgrade => upgrade.upgradeType == upgradeType);
        Cost cost = targetUpgrade.levelCosts.First(cost => cost.level == GetLevelForUpgradeType(upgradeType) + 1);
        return cost.canPlayerAfford();
    }

    public void BuyUpgradeForNextLevel(UpgradeTypes upgradeType)
    {
        Upgrade targetUpgrade = currentUpgrades.First(upgrade => upgrade.upgradeType == upgradeType);
        Cost cost = targetUpgrade.levelCosts.First(cost => cost.level == GetLevelForUpgradeType(upgradeType) + 1);

        cost.payCost();

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
