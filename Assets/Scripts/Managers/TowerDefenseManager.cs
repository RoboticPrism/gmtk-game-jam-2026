using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

public class TowerDefenseManager : MonoBehaviour
{
    public static TowerDefenseManager singleton;

    [SerializeField]
    public bool isTowerDefenseMode = false;

    [SerializeField]
    [Tooltip("The enemy that spawns in tower defense mode")]
    private TowerDefenseGremlin gremlinPrefab;

    public List<TowerDefenseGremlin> instantiatedGremlins = new List<TowerDefenseGremlin>();

    [SerializeField]
    private List<SpawnPoint> allSpawnPoints;

    [SerializeField]
    private List<SpawnPoint> recentlyUsedSpawnPoints = new List<SpawnPoint>();

    [SerializeField]
    private GameObject winText;

    [SerializeField]
    private GameObject gameoverText;

    private int playerLightRadius;

    [System.Serializable]
    public class Wave
    {
        public int gremlinCount;
        public int gremlinSpawnDelay;
        public int nextDaySteps;
    }

    [SerializeField]
    private List<Wave> waves;

    [SerializeField]
    private Wave currentWave;

    [SerializeField]
    private int currentSpawnDelay = 0;

    private int previousPyreLightRadius = 0;

    [SerializeField]
    private int defenseModePyreLightRadius;

    private bool isGameOver = false;

    public void Awake()
    {
        if (singleton)
        {
            Debug.LogError("A tower defense manager already exists!");

        }
        else {
            singleton = this;
        }
        allSpawnPoints = new List<SpawnPoint>(FindObjectsByType<SpawnPoint>());
    }

    public void BeginTowerDefenseMode()
    {
        isTowerDefenseMode = true;

        // If player is far from the pyre, force a teleport
        if(Vector3.Distance(Player.singleton.transform.position, Pyre.singleton.transform.position) > 10f)
        {
            Player.singleton.playerMovement.ForceTeleport();
        }

        GridManager.singleton.StartTowerDefense();

        BackgroundMusicManager.singleton.StartDefendMusic();

        // Save and update the pyre's old light radius
        previousPyreLightRadius = Pyre.singleton.GetComponent<FogOfWarLight>().lightRadius;
        Pyre.singleton.GetComponent<FogOfWarLight>().lightRadius = defenseModePyreLightRadius;

        // Save and disable the players light radius
        playerLightRadius = Player.singleton.GetComponent<FogOfWarLight>().lightRadius;
        Player.singleton.GetComponent<FogOfWarLight>().lightRadius = 0;
        FogOfWarManager.TriggerLightingUpdate();

        // Get the wave for the night
        currentWave = waves[0];
        waves.RemoveAt(0);
    }

    public void EndTowerDefenseMode()
    {
        if(isGameOver)
        {
            return;
        }
        // If there's another night, start the next day
        if (waves.Count > 0)
        {
            isTowerDefenseMode = false;

            GridManager.singleton.EndTowerDefense();

            // Go back to explore music
            BackgroundMusicManager.singleton.StartExploreMusic();

            // Reload all turrets
            TurretManager.singleton.ReloadAllTurrets();

            // Restore pyre light radius
            Pyre.singleton.GetComponent<FogOfWarLight>().lightRadius = previousPyreLightRadius;

            // Reenable player's light radius 
            Player.singleton.GetComponent<FogOfWarLight>().lightRadius = playerLightRadius;
            FogOfWarManager.TriggerLightingUpdate();
            CounterManager.singleton.steps = currentWave.nextDaySteps;
        }
        // otherwise win!
        else
        {
            StartCoroutine(WinAnimation());
        }
    }

    public void DoTurn()
    {
        if(isTowerDefenseMode && currentWave != null)
        {
            if (currentWave.gremlinCount > 0)
            {
                if (currentSpawnDelay == 0)
                {
                    SpawnGremlin();
                    currentSpawnDelay = currentWave.gremlinSpawnDelay;
                    currentWave.gremlinCount--;
                }
                else
                {
                    currentSpawnDelay--;
                }
            }
        }
    }

    public void SpawnGremlin()
    {
        // Chose a spawn point that hasn't been used recently
        List<SpawnPoint> availableSpawnPoints = new List<SpawnPoint>(allSpawnPoints);
        availableSpawnPoints.RemoveAll(point => recentlyUsedSpawnPoints.Contains(point));
        SpawnPoint spawnPoint = availableSpawnPoints[Random.Range(0, availableSpawnPoints.Count)];
        
        // Add this point to the recently used list and remove the least recent entry
        recentlyUsedSpawnPoints.Add(spawnPoint);
        if(recentlyUsedSpawnPoints.Count > 3)
        {
            recentlyUsedSpawnPoints.RemoveAt(0);
        }

        TowerDefenseGremlin gremlin = Instantiate(gremlinPrefab, spawnPoint.transform.position, Quaternion.identity);
        instantiatedGremlins.Add(gremlin);
    }

    public void OnGremlinDefeat(TowerDefenseGremlin gremlin)
    {
        instantiatedGremlins.Remove(gremlin);

        if(instantiatedGremlins.Count == 0 && currentWave.gremlinCount <= 0)
        {
            EndTowerDefenseMode();
        }
    }

    public void DamagePyre()
    {
        Pyre.singleton.health--;
        if(Pyre.singleton.health <= 0)
        {
            isGameOver = true;
            if(gameoverCoroutine == null)
            {
                gameoverCoroutine = StartCoroutine(GameoverAnimation());
            }
        }
    }

    private Coroutine gameoverCoroutine;
    IEnumerator GameoverAnimation()
    {
        // Turn off the forced lighting to play our own lightin animation
        GridManager.singleton.EndTowerDefense();

        BackgroundMusicManager.singleton.LoseEffect();

        TurretManager.singleton.RemoveAllTurretLights();

        FogOfWarLight pyreLight = Pyre.singleton.GetComponent<FogOfWarLight>();
        while (pyreLight.lightRadius > 0)
        {
            yield return new WaitForSeconds(1f);
            pyreLight.lightRadius--;
            FogOfWarManager.TriggerLightingUpdate();
        }

        yield return new WaitForSeconds(1f);

        gameoverText.SetActive(true);

        yield return new WaitForSeconds(5f);

        ScreenEffectManager.singleton.FadeOut();
        yield return new WaitForSeconds(2f);

        // warp to title screen
        SceneManager.LoadScene("MainMenu");
    }

    IEnumerator WinAnimation()
    {
        // Turn off the forced lighting to play our own lightin animation
        GridManager.singleton.EndTowerDefense();

        BackgroundMusicManager.singleton.WinEffect();

        winText.SetActive(true);

        FogOfWarLight pyreLight = Pyre.singleton.GetComponent<FogOfWarLight>();
        while (pyreLight.lightRadius < 15)
        {
            yield return new WaitForSeconds(1f);
            pyreLight.lightRadius++;
            FogOfWarManager.TriggerLightingUpdate();
        }

        yield return new WaitForSeconds(1f);

        ScreenEffectManager.singleton.FadeToWhite();
        yield return new WaitForSeconds(2f);

        // warp to title screen
        SceneManager.LoadScene("MainMenu");
    }
}
