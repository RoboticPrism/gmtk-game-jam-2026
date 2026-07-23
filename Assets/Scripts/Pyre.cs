using UnityEngine;

public class Pyre : BumpableTile
{

    [SerializeField]
    [Tooltip("How much wood it costs to refueld the fire")]
    private int refuelCost;

    [SerializeField]
    [Tooltip("How many steps you get back from refueling")]
    private int refuelSteps;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnBump()
    {
        base.OnBump();
        if(Player.singleton.playerInventory.GetHeldAmount(ResourceType.WOOD) >= refuelCost)
        {
            Player.singleton.playerInventory.SpendResourcesOnTarget(ResourceType.WOOD, transform, refuelCost);
            CounterManager.singleton.AddSteps(refuelSteps);
        }
        else
        {

        }
    }
}
