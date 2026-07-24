using UnityEngine;
using UnityEngine.Tilemaps;

public class TurretBase : BumpableTile 
{
    [SerializeField]
    private UpgradeCostUI upgradeCostUIPrefab;

    private UpgradeCostUI upgradeCostUIInstance;

    [SerializeField]
    private Cost cost;

    [SerializeField]
    private TileBase turretTile;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        base.Start();
        CreateUI();
    }

    private void CreateUI()
    {
        upgradeCostUIInstance = Instantiate(upgradeCostUIPrefab, transform.position, Quaternion.identity);
        upgradeCostUIInstance.Setup("Tower\nLvl. 1", cost);
    }

    // Update is called once per frame
    void Update()
    {
        upgradeCostUIInstance.gameObject.SetActive(!TowerDefenseManager.singleton.isTowerDefenseMode); 
    }

    public override void OnBump()
    {
        base.OnBump();
        if (!TowerDefenseManager.singleton.isTowerDefenseMode)
        {
            if (cost.canPlayerAfford())
            {
                cost.payCost();
                Destroy(upgradeCostUIInstance.gameObject);
                GridManager.singleton.resourceTilemap.SetTile(GridManager.singleton.resourceTilemap.WorldToCell(transform.position), turretTile);
            }
        }
    }

}
