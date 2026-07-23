using UnityEngine;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager singleton;

    private List<EnemyBase> enemies = new List<EnemyBase>();

    public void Awake()
    {
        if(singleton)
        {
            Debug.LogError("Another enemy manager already exists!");
        }
        else
        {
            singleton = this;
        }
    }

    public EnemyBase GetEnemyAtLocation(Vector3Int gridLocation)
    {
        foreach(EnemyBase enemy in enemies)
        {
            if (enemy.gridLocation == gridLocation)
            {
                return enemy;
            }
        }
        return null;
    }

    public void AddEnemy(EnemyBase enemy)
    {
        enemies.Add(enemy);
    }

    public void RemoveEnemy(EnemyBase enemy)
    {
        enemies.Remove(enemy);
    }

    public void DoEnemyTurns()
    {
        foreach(EnemyBase enemy in enemies)
        {
            enemy.DoTurn();
        }
    }
}
