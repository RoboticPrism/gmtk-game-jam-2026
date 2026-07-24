using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public Vector3Int gridLocation;

    public void Start()
    {
        gridLocation = GridManager.singleton.resourceTilemap.WorldToCell(transform.position);
    }
}
