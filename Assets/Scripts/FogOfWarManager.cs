using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System;

public class FogOfWarManager : MonoBehaviour
{

    [SerializeField]
    [Tooltip("The tilemap with fog of war to dynamically change")]
    private Tilemap fogOfWarTilemap;

    [SerializeField]
    [Tooltip("The tilemap to place when further away from a light source")]
    private TileBase partialFogTile;

    private List<FogOfWarLight> lightSources;

    public static event Action OnLightingChanged;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lightSources = new List<FogOfWarLight>(FindObjectsByType<FogOfWarLight>());
        
        // Initial trigger
        OnTriggerLightingUpdate();

        // Subscribe to trigger updates
        OnLightingChanged += OnTriggerLightingUpdate;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void TriggerLightingUpdate()
    {
        OnLightingChanged?.Invoke();
    }

    private void OnTriggerLightingUpdate()
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
        if(light.GetComponent<Player>())
        {
            lightPosition = light.GetComponent<Player>().currentGridLocation;
        }
        else
        {
            lightPosition = Vector3Int.CeilToInt(light.transform.position);
        }

        // Calculate partial light tiles
        foreach(Vector3Int position in GetTileBorderInRadius(lightPosition, light.lightRadius + 1))
        {
            fogOfWarTilemap.SetTile(position, partialFogTile);
        }

    }

    private void UpdateFullLights(FogOfWarLight light)
    {
        Vector3Int lightPosition;
        if(light.GetComponent<Player>())
        {
            lightPosition = light.GetComponent<Player>().currentGridLocation;
        }
        else
        {
            lightPosition = Vector3Int.CeilToInt(light.transform.position);
        }

        // Calculate full light tiles
        foreach(Vector3Int position in GetTilePositionsInRadius(lightPosition, light.lightRadius)) {
            fogOfWarTilemap.SetTile(position, null);
        }
    }
    

    public List<Vector3Int> GetTilePositionsInRadius(Vector3 worldPosition, int radius)
    {
        List<Vector3Int> positions = new List<Vector3Int>();
        
        // Convert the world position (e.g., an explosion or mouse click) to grid coordinates
        Vector3Int originCell = fogOfWarTilemap.WorldToCell(worldPosition);
        int radiusSquared = radius * radius;

        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                if ((x * x) + (y * y) <= radiusSquared)
                {
                    Vector3Int targetCell = new Vector3Int(originCell.x + x, originCell.y + y, originCell.z);
                    
                    // Keep it if a tile actively exists at this position
                    if (fogOfWarTilemap.HasTile(targetCell))
                    {
                        positions.Add(targetCell);
                    }
                }
            }
        }
        return positions;
    }

    public List<Vector3Int> GetTileBorderInRadius(Vector3 worldPosition, int radius)
    {
        List<Vector3Int> borderPositions = new List<Vector3Int>();
        
        // Convert world position to grid space
        Vector3Int originCell = fogOfWarTilemap.WorldToCell(worldPosition);
        
        // Edge cases
        if (radius <= 0)
        {
            if (fogOfWarTilemap.HasTile(originCell)) borderPositions.Add(originCell);
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
