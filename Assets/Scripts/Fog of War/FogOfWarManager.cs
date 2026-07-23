using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System;

public class FogOfWarManager : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The tilemap to place when further away from a light source")]
    private TileBase partialFogTile;

    [SerializeField]
    private List<FogOfWarLight> lightSources;

    public static event Action OnLightingChanged;

    public static event Action<FogOfWarLight> OnLightAdded;

    public static event Action<FogOfWarLight> OnLightRemoved;

    void Awake()
    {
        // Subscribe to trigger updates
        OnLightingChanged += UpdateLights;
        OnLightAdded += AddLight;
        OnLightRemoved += RemoveLight;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void TriggerLightingUpdate()
    {
        OnLightingChanged?.Invoke();
    }

    public static void TriggerAddLightUpdate(FogOfWarLight light)
    {
        OnLightAdded?.Invoke(light);
    }

    public static void TriggerRemoveLightUpdate(FogOfWarLight light)
    {
        OnLightRemoved?.Invoke(light);
    }

    public void AddLight(FogOfWarLight light)
    {
        lightSources.Add(light);
        UpdateLights();
    }

    public void RemoveLight(FogOfWarLight light)
    {
        lightSources.Remove(light);
        UpdateLightingRemoveLight(light);
    }

    private void UpdateLights()
    {
        foreach(FogOfWarLight light in lightSources)
        {
            UpdatePartialLights(light);
        }

        foreach(FogOfWarLight light in lightSources)
        {
            UpdateFullLights(light);
        }
    }

    private void UpdatePartialLights(FogOfWarLight light)
    {
        Vector3Int lightPosition;
        if(light.GetComponent<PlayerMovement>())
        {
            lightPosition = light.GetComponent<PlayerMovement>().currentGridLocation;
        }
        else
        {
            lightPosition = GridManager.singleton.resourceTilemap.WorldToCell(light.transform.position);
        }

        // Calculate partial light tiles
        foreach(Vector3Int position in GetTileBorderInRadius(lightPosition, light.lightRadius + 1))
        {
            GridManager.singleton.fogOfWarTilemap.SetTile(position, partialFogTile);
        }

    }

    private void UpdateFullLights(FogOfWarLight light)
    {
        Vector3Int lightPosition;
        // Special case for player movement, since the player could be mid lerp
        if(light.GetComponent<PlayerMovement>())
        {
            lightPosition = light.GetComponent<PlayerMovement>().currentGridLocation;
        }
        else
        {
            lightPosition = GridManager.singleton.resourceTilemap.WorldToCell(light.transform.position);
        }

        // Calculate full light tiles
        foreach(Vector3Int position in GetTilePositionsInRadius(lightPosition, light.lightRadius)) {
            GridManager.singleton.fogOfWarTilemap.SetTile(position, null);
        }
    }

    // Remove a light's previous lighting and replace it with fog of war, then trigger a lighting update
    private void UpdateLightingRemoveLight(FogOfWarLight light)
    {
        Vector3Int lightPosition = GridManager.singleton.resourceTilemap.WorldToCell(light.transform.position);
        foreach(Vector3Int position in GetTilePositionsInRadius(lightPosition, light.lightRadius)) {
            GridManager.singleton.fogOfWarTilemap.SetTile(position, partialFogTile);
        }

        UpdateLights();
    }
    

    public List<Vector3Int> GetTilePositionsInRadius(Vector3 worldPosition, int radius)
    {
        List<Vector3Int> positions = new List<Vector3Int>();
        
        // Convert the world position (e.g., an explosion or mouse click) to grid coordinates
        Vector3Int originCell = GridManager.singleton.fogOfWarTilemap.WorldToCell(worldPosition);
        int radiusSquared = radius * radius;

        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                if ((x * x) + (y * y) <= radiusSquared)
                {
                    Vector3Int targetCell = new Vector3Int(originCell.x + x, originCell.y + y, originCell.z);
                    
                    positions.Add(targetCell);
                }
            }
        }
        return positions;
    }

    public List<Vector3Int> GetTileBorderInRadius(Vector3 worldPosition, int radius)
    {
        List<Vector3Int> borderPositions = new List<Vector3Int>();
        
        // Convert world position to grid space
        Vector3Int originCell = GridManager.singleton.fogOfWarTilemap.WorldToCell(worldPosition);
        
        // Edge cases
        if (radius <= 0)
        {
            if (GridManager.singleton.fogOfWarTilemap.HasTile(originCell)) borderPositions.Add(originCell);
            return borderPositions;
        }

        // Establish the inner and outer radius boundaries
        int outerRadiusSq = radius * radius;
        int innerRadiusSq = (radius - 1) * (radius - 1);

        // Loop through the bounding box
        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                int distSq = (x * x) + (y * y);

                // Filter for coordinates that fall exactly inside the border shell
                if (distSq <= outerRadiusSq && distSq > innerRadiusSq)
                {
                    Vector3Int targetCell = new Vector3Int(originCell.x + x, originCell.y + y, originCell.z);
                    
                    borderPositions.Add(targetCell);
                }
            }
        }
        return borderPositions;
    }

}
