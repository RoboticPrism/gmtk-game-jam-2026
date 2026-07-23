using System.Collections.Generic;
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

    [SerializeField]
    [Tooltip("How many tiles to pathfind search before we give up")]
    private int maxSearchIterations = 500;

    public void Awake()
    {
        if(singleton)
        {
            Debug.LogError("Another grid manager already exists!");
        }
        else
        {
            singleton = this;
        }

        // So we don't have to keep hiding and unhiding this to do map edits
        fogOfWarTilemap.gameObject.SetActive(true);
    }


    public bool CheckCollisionAtWorldPoint(Vector3 worldPoint)
    {
        return resourceTilemap.GetTile(resourceTilemap.WorldToCell(worldPoint)) != null || EnemyManager.singleton.GetEnemyAtLocation(resourceTilemap.WorldToCell(worldPoint)) != null;
    }

    public bool CheckCollisionAtGridPoint(Vector3Int gridPoint)
    {
        return resourceTilemap.GetTile(gridPoint) != null || EnemyManager.singleton.GetEnemyAtLocation(gridPoint) != null;
    }

    public BumpableTile GetBumpableAtGridPoint(Vector3Int gridPoint)
    {
        BumpableTile bumpable = resourceTilemap.GetInstantiatedObject(gridPoint)?.GetComponent<BumpableTile>();
        if (bumpable == null)
        {
            bumpable = EnemyManager.singleton.GetEnemyAtLocation(gridPoint);
        }
        return bumpable;
    }

    public class PathNode
    {
        public Vector3Int position;
        public int gCost; // Distance from starting node
        public int hCost; // Estimated distance to end node
        public int fCost => gCost + hCost; // Total cost
        public PathNode parent;

        public PathNode(Vector3Int pos)
        {
            position = pos;
        }
    }

    public List<Vector3Int> FindPath(Vector3Int startPos, Vector3Int targetPos)
    {
        PathNode startNode = new PathNode(startPos);
        PathNode targetNode = new PathNode(targetPos);

        List<PathNode> openList = new List<PathNode> { startNode };
        HashSet<Vector3Int> closedList = new HashSet<Vector3Int>();

        int currentIterations = 0;
        while (openList.Count > 0 && currentIterations < maxSearchIterations)
        {
            // Find node in Open List with the lowest F Cost
            PathNode currentNode = openList[0];
            for (int i = 1; i < openList.Count; i++)
            {
                if (openList[i].fCost < currentNode.fCost || 
                   (openList[i].fCost == currentNode.fCost && openList[i].hCost < currentNode.hCost))
                {
                    currentNode = openList[i];
                }
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode.position);

            // Path found! Reconstruct it and return
            if (currentNode.position == targetNode.position)
            {
                return RetracePath(startNode, currentNode);
            }

            // Loop through all 4 neighboring cardinal directions
            foreach (Vector3Int neighborPos in GetNeighbors(currentNode.position))
            {
                if (closedList.Contains(neighborPos) || !IsWalkable(neighborPos))
                    continue;

                int newMovementCostToNeighbor = currentNode.gCost + GetDistance(currentNode.position, neighborPos);
                PathNode neighborNode = openList.Find(n => n.position == neighborPos);

                if (neighborNode == null)
                {
                    neighborNode = new PathNode(neighborPos);
                    neighborNode.gCost = newMovementCostToNeighbor;
                    neighborNode.hCost = GetDistance(neighborPos, targetNode.position);
                    neighborNode.parent = currentNode;
                    openList.Add(neighborNode);
                }
                else if (newMovementCostToNeighbor < neighborNode.gCost)
                {
                    neighborNode.gCost = newMovementCostToNeighbor;
                    neighborNode.parent = currentNode;
                }
            }
            currentIterations++;
        }

        return null; // Return null if no path is found
    }

    private List<Vector3Int> RetracePath(PathNode startNode, PathNode endNode)
    {
        List<Vector3Int> path = new List<Vector3Int>();
        PathNode currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode.position);
            currentNode = currentNode.parent;
        }
        path.Reverse();
        return path;
    }

    private List<Vector3Int> GetNeighbors(Vector3Int currentPos)
    {
        return new List<Vector3Int>
        {
            currentPos + Vector3Int.up,
            currentPos + Vector3Int.down,
            currentPos + Vector3Int.left,
            currentPos + Vector3Int.right
        };
    }

    // Manhattan distance metric for grid pathfinding
    private int GetDistance(Vector3Int posA, Vector3Int posB)
    {
        return Mathf.Abs(posA.x - posB.x) + Mathf.Abs(posA.y - posB.y);
    }

    // Check if the tile contains an obstacle
    private bool IsWalkable(Vector3Int cellPos)
    {
        return !resourceTilemap.HasTile(cellPos);
    }
}
