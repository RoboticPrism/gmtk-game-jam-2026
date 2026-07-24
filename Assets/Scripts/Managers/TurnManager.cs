using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager singleton;

    public void Awake()
    {
        if (singleton)
        {
            Debug.LogError("A turn manager already exists!");

        }
        else {
            singleton = this;
        }
    }

    public void DoTurn()
    {
        TowerDefenseManager.singleton.DoTurn();
        EnemyManager.singleton.DoEnemyTurns();
    }
}
