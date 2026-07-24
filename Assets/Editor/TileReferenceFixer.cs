using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;
using System.IO;
using System.Linq;
using System.Collections.Generic;

public class TileReferenceFixer : EditorWindow
{
    // === CONFIGURATION ===
    // Path to the specific Palette Prefab that was changed
    private static string palettePrefabPath = "Assets/Tilemaps/"; 
    
    // Path to your newly updated 4x7 spritesheet png
    private static string newSpritesheetPath = "Assets/Textures/MyNewSpritesheet.png"; 

    private static int oldColumns = 3;
    private static int newColumns = 4;
    // =====================

    [MenuItem("Tools/Fix Resource Palette Tiles")]
    public static void FixTargetedReferences()
    {
        // 1. Load the specific Resource Palette Prefab
        GameObject palettePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(palettePrefabPath);
        if (palettePrefab == null)
        {
            Debug.LogError($"[Fixer] Resource Palette Prefab not found at path: {palettePrefabPath}");
            return;
        }

        // 2. Load all the sub-sprites from the new 4x7 spritesheet
        Object[] subAssets = AssetDatabase.LoadAllAssetsAtPath(newSpritesheetPath);
        Sprite[] newSprites = subAssets.OfType<Sprite>().OrderBy(s => s.name).ToArray();

        if (newSprites.Length == 0)
        {
            Debug.LogError($"[Fixer] No sprites found at {newSpritesheetPath}. Ensure it is sliced in the Sprite Editor!");
            return;
        }

        // 3. Find only the Tile Assets used inside the Resource Palette Prefab
        // Tilemaps inside palettes store tiles on a Grid component
        Tilemap tilemap = palettePrefab.GetComponentInChildren<Tilemap>();
        if (tilemap == null)
        {
            Debug.LogError("[Fixer] Could not find a Tilemap component inside the Resource Palette Prefab.");
            return;
        }

        // Extract unique Tile assets actually painted on this palette
        BoundsInt bounds = tilemap.cellBounds;
        TileBase[] allTiles = tilemap.GetTilesBlock(bounds);
        HashSet<Tile> targetedTiles = new HashSet<Tile>();

        foreach (TileBase tileBase in allTiles)
        {
            if (tileBase is Tile standardTile && standardTile.sprite != null)
            {
                targetedTiles.Add(standardTile);
            }
        }

        if (targetedTiles.Count == 0)
        {
            Debug.LogWarning("[Fixer] Found 0 valid Tile Assets painted on the Resource Palette.");
            return;
        }

        // 4. Process and fix ONLY the targeted Tile Assets
        int fixedCount = 0;

        foreach (Tile tile in targetedTiles)
        {
            // Extract the sprite's numeric index from its name (e.g., "spritesheet_5" -> 5)
            int currentSpriteIdx = GetSpriteIndexFromName(tile.sprite.name);

            if (currentSpriteIdx != -1)
            {
                // Mathematical mapping from 3-column system to 4-column system
                int oldX = currentSpriteIdx % oldColumns;
                int oldY = currentSpriteIdx / oldColumns;

                // Compute what index that same X and Y should occupy in the 4-column system
                int newSpriteIdx = (oldY * newColumns) + oldX;

                if (newSpriteIdx < newSprites.Length)
                {
                    // Update the tile's reference to the correct new sprite safely
                    tile.sprite = newSprites[newSpriteIdx];
                    EditorUtility.SetDirty(tile); // Mark asset as changed so Unity saves it
                    fixedCount++;
                }
            }
        }

        // 5. Save modified tile assets and refresh
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        Debug.Log($"[Fixer] Cleaned up! Successfully isolated and updated {fixedCount} Tile Assets belonging strictly to 'Resource Palette'.");
    }

    private static int GetSpriteIndexFromName(string spriteName)
    {
        string numberString = new string(spriteName.Where(char.IsDigit).ToArray());
        if (int.TryParse(numberString, out int result))
        {
            return result;
        }
        return -1;
    }
}