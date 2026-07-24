using UnityEngine;

public class BumpableUpgrader : BumpableTile 
{
    [SerializeField]
    [Tooltip("Which upgrade does this sell")]
    private PlayerUpgrades.UpgradeTypes upgradeType;

    [SerializeField]
    private UpgradeCostUI upgradeCostUIPrefab;

    private UpgradeCostUI upgradeCostUIInstance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        base.Start();
        CreateUI();
    }

    private void CreateUI()
    {
        string upgradeName = Player.singleton.playerUpgrades.GetUpgradeByType(upgradeType).upgradeName;
        int upgradeLevel = Player.singleton.playerUpgrades.GetUpgradeByType(upgradeType).currentLevel + 1;
        PlayerUpgrades.UpgradeCost upgradeCost = Player.singleton.playerUpgrades.GetCostForNextLevelForUpgradeType(upgradeType);

        upgradeCostUIInstance = Instantiate(upgradeCostUIPrefab, transform.position, Quaternion.identity);
        upgradeCostUIInstance.Setup(upgradeName, upgradeLevel, upgradeCost);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnBump()
    {
        base.OnBump();
        if(Player.singleton.playerUpgrades.CheckCanBuyUpgradeForNextLevel(upgradeType))
        {
            Player.singleton.playerUpgrades.BuyUpgradeForNextLevel(upgradeType);
            Destroy(upgradeCostUIInstance.gameObject);
            CreateUI();
        }
    }
}
