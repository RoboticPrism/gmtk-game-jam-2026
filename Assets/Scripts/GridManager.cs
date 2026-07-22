using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    public static GridManager singleton;

    [SerializeField]
    [Tooltip("The tilemap the fog of war is on")]
    public Tilemap fogOfWarTilemap;

    [SerializeField]
    [Tooltip("The tilemap resources sit on")]
    public Tilemap resourceTilemap;

    [SerializeField]
    [Tooltip("The tilemap the background is on")]
    public Tilemap worldTilemap;

    void Awake()
    {
        if(singleton)
        {
            Debug.LogError("Another grid manager already exists!");
        }
        else
        {
            singleton = this;
        }
    }
}
