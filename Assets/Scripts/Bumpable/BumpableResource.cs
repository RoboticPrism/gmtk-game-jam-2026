using UnityEngine;
using UnityEngine.Tilemaps;

public class BumpableResource : BumpableTile
{
    [SerializeField]
    [Tooltip("How many hits this takes to break")]
    public int health;

    [SerializeField]
    [Tooltip("What type of resource does this drop")]
    public ResourceType resourceType;

    [SerializeField]
    [Tooltip("Whats the minimum drop ammount")]
    public int dropMin;

    [SerializeField]
    [Tooltip("Whats the maxmimum drop ammount")]
    public int dropMax;

    public override void OnBump()
    {
        if(health > 0)
        {
            health -= 1;
        }
        else
        {
            // Drop resources
            for (int i = 0; i < Random.Range(dropMin, dropMax); i++)
            {
                ResourceDrop drop = Instantiate(ResourceDictionary.singleton.resourceDropPrefab, transform.position, Quaternion.identity);
                drop.SetResourceType(resourceType);
            }
            // Delete this tile
            GridManager.singleton.resourceTilemap.SetTile(GridManager.singleton.resourceTilemap.WorldToCell(transform.position), null);
        }
    }
}
