using UnityEngine;

public class CounterManager : MonoBehaviour
{
    public static CounterManager singleton;

    [SerializeField]
    [Tooltip("How many steps the player has")]
    public int steps;

    [SerializeField]
    [Tooltip("How many steps the player starts with")]
    public int startingSteps;

    public void Awake()
    {
        if(singleton)
        {
            Debug.LogError("Another counter manager already exists!");
        }
        else
        {
            singleton = this;
        }
    }

    public void StartCounting()
    {
        steps = startingSteps;
    }

    public void UseStep()
    {
        steps--;
    }

    public void AddSteps(int amount = 1)
    {
        steps += amount;
    }
}
