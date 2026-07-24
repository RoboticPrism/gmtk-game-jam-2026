using System.Collections.Generic;
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

    public bool isCounting = false;

    [System.Serializable]
    class TutorialAtCount
    {
        public GameObject tutorial;
        public int displayAtStepCount;
    }

    [SerializeField]
    List<TutorialAtCount> tutorials;

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
        isCounting = true;
        steps = startingSteps;
    }

    public void UseStep()
    {
        if (!TowerDefenseManager.singleton.isTowerDefenseMode && isCounting)
        {
            steps--;

            // Play any relevant tutorials
            TutorialAtCount currentTutorial = null;
            foreach(TutorialAtCount tutorialAtCount in tutorials)
            {
                if(tutorialAtCount.displayAtStepCount == steps)
                {
                    currentTutorial = tutorialAtCount;
                }
            }

            if (currentTutorial != null)
            {
                currentTutorial.tutorial.SetActive(true);
                tutorials.Remove(currentTutorial);
            }

            // Check if its tower defense mode
            if (steps <= 0)
            {
                TowerDefenseManager.singleton.BeginTowerDefenseMode();
            }
        }
    }

    public void AddSteps(int amount = 1)
    {
        steps += amount;
    }
}
