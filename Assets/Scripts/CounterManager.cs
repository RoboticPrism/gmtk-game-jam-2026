using UnityEngine;

public class CounterManager : MonoBehaviour
{
    public static CounterManager singleton;

    [SerializeField]
    [Tooltip("How many steps the player starts with")]
    public int steps;

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

    public void UseStep()
    {
        steps--;
    }

    public void AddSteps(int amount = 1)
    {
        steps += amount;
    }
}
