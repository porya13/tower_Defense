using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WaveDetails
{
    public GridBuilder nextGrid;
    public EnemyPortal[] newPortals;
    public int basicEnemy;
    public int fastEnemy;
}

public class WaveManager : MonoBehaviour
{
    private GameManager gameManager;
    private TileAnimator tileAnimator;
    private UI_InGame inGameUI;
    [SerializeField] private GridBuilder currentGrid;

    [Header("Wave Details")]
    [SerializeField] private float timeBetweenWaves = 10;
    [SerializeField] private float waveTimer;
    [SerializeField] private WaveDetails[] levelWaves;
    [SerializeField] private int waveIndex;

    [Header("Level Update Details")]
    [SerializeField] private float yOffset = 5;
    [SerializeField] private float tileDelay = .1f;

    [Header("Enemy Prefabs")]
    [SerializeField] private GameObject basicEnemy;
    [SerializeField] private GameObject fastEnemy;

    private List<EnemyPortal> enemyPortals;
    private bool waveTimerEnabled;
    private bool makingNextWave;
    public bool gameBegun;

    private void Awake()
    {
        enemyPortals = new List<EnemyPortal>(FindObjectsOfType<EnemyPortal>());
        
        gameManager = FindFirstObjectByType<GameManager>();
        tileAnimator = FindFirstObjectByType<TileAnimator>();
        inGameUI = FindFirstObjectByType<UI_InGame>(FindObjectsInactive.Include);
    }

    

    private void Update()
    {
        if (gameBegun == false)
            return;

        HandleWaveTimer();
    }

    [ContextMenu("Activate Wave Manager")]
    public void ActivateWaveManager()
    {
        gameBegun = true;
        EnableWaveTimer(true);
    }

    public void DeactivateWaveManager() => gameBegun = false;


    public void CheckIfWaveCompleted()
    {
        if (gameBegun == false)
            return;

        if (AllEnemiesDefeated() == false || makingNextWave)
            return;

        makingNextWave = true;
        waveIndex++;

        if (HasNoMoreWaves())
        {
            gameManager.LevelCompleted();
            return;
        }

        if (HasNewLayout())
            AttemptToUpdateLayout();
        else
            EnableWaveTimer(true);
    }

    public void StartNewWave()
    {
        currentGrid.UpdateNavMesh();
        GiveEnemiesToPortals();
        EnableWaveTimer(false);
        makingNextWave = false;
    }

    private void HandleWaveTimer()
    {
        if (waveTimerEnabled == false)
            return;

        waveTimer -= Time.deltaTime;
        inGameUI.UpdateWaveTimerUI(waveTimer);


        if (waveTimer <= 0)
            StartNewWave();

    }
    private void GiveEnemiesToPortals()
    {
        List<GameObject> newEnemies = GetNewEnemies();
        int portalIndex = 0;

        if (newEnemies == null)
        {
            Debug.LogWarning("I had no wave to setup");
            return;
        }

        for (int i = 0; i < newEnemies.Count; i++)
        {
            GameObject enemyToAdd = newEnemies[i];
            EnemyPortal portalToReciveEnemy = enemyPortals[portalIndex];

            portalToReciveEnemy.AddEnemy(enemyToAdd);

            portalIndex++;

            if (portalIndex >= enemyPortals.Count)
                portalIndex = 0;
        }
    }
    private void AttemptToUpdateLayout() => UpdateLevelLayout(levelWaves[waveIndex]);
    private void UpdateLevelLayout(WaveDetails nextWave)
    {
        GridBuilder nextGrid = nextWave.nextGrid;
        List<GameObject> grid = currentGrid.GetTileSetup();
        List<GameObject> newGrid = nextGrid.GetTileSetup();

        if (grid.Count != newGrid.Count)
        {
            Debug.LogWarning("Current grid and new grid have different size.");
            return;
        }

        List<TileSlot> tilesToRemove = new List<TileSlot>();
        List<TileSlot> tilesToAdd = new List<TileSlot>();

        for (int i = 0; i < grid.Count; i++)
        {
            TileSlot currentTile = grid[i].GetComponent<TileSlot>();
            TileSlot newTile = newGrid[i].GetComponent<TileSlot>();


            bool shouldBeUpdated = currentTile.GetMesh() != newTile.GetMesh() ||
                                   currentTile.GetMaterial() != newTile.GetMaterial() ||
                                   currentTile.GetAllChildren().Count != newTile.GetAllChildren().Count ||
                                   currentTile.transform.rotation != newTile.transform.rotation;

            if (shouldBeUpdated)
            {
                tilesToRemove.Add(currentTile);
                tilesToAdd.Add(newTile);

                grid[i] = newTile.gameObject;
            }
        }

        StartCoroutine(RebuildLevelCo(tilesToRemove, tilesToAdd,nextWave,tileDelay));
    }
    private IEnumerator RebuildLevelCo(List<TileSlot> tilesToRemove,List<TileSlot> tilesToAdd, WaveDetails waveDetails,float delay)
    {
        for (int i = 0; i < tilesToRemove.Count; i++)
        {
            yield return new WaitForSeconds(delay);
            RemoveTile(tilesToRemove[i]);
        }

        for (int i = 0; i < tilesToAdd.Count; i++)
        {
            yield return new WaitForSeconds(delay);
            AddTile(tilesToAdd[i]);
        }

        EnableNewPortals(waveDetails.newPortals);
        EnableWaveTimer(true);
    }
    private void AddTile(TileSlot newTile)
    {
        newTile.gameObject.SetActive(true);
        newTile.transform.position += new Vector3(0, -yOffset, 0);
        newTile.transform.parent = currentGrid.transform;

        Vector3 targetPosition = newTile.transform.position + new Vector3(0, yOffset, 0);
        tileAnimator.MoveTile(newTile.transform, targetPosition);
    }
    private void RemoveTile(TileSlot tileToRemove)
    {
        Vector3 targetPosition = tileToRemove.transform.position + new Vector3(0,-yOffset, 0);
        tileAnimator.MoveTile(tileToRemove.transform, targetPosition);

        Destroy(tileToRemove.gameObject, 1);
    }
    private void EnableWaveTimer(bool enable)
    {
        if (waveTimerEnabled == enable)
            return;

        waveTimer = timeBetweenWaves;
        waveTimerEnabled = enable;
        inGameUI.EnableWaveTimer(enable);
    }
    private void EnableNewPortals(EnemyPortal[] newPortals)
    {
        foreach (EnemyPortal portal in newPortals)
        {
            portal.AssignWaveManager(this);
            portal.gameObject.SetActive(true);
            enemyPortals.Add(portal);
        }
    }
    private List<GameObject> GetNewEnemies()
    {
        if (waveIndex >= levelWaves.Length)
        {
            // Check if all waves are completed; return null if no more waves are available
            return null;
        }

        List<GameObject> newEnemyList = new List<GameObject>();

        for (int i = 0; i < levelWaves[waveIndex].basicEnemy; i++)
        {
            newEnemyList.Add(basicEnemy);
        }

        for (int i = 0; i < levelWaves[waveIndex].fastEnemy; i++)
        {
            newEnemyList.Add(fastEnemy);
        }

        return newEnemyList;
    }
    public WaveDetails[] GetLevelWaves() => levelWaves;
    private bool AllEnemiesDefeated()
    {
        foreach (EnemyPortal portal in enemyPortals)
        {
            if (portal.GetActiveEnemies().Count > 0)
                return false;
        }

        return true;
    }
    private bool HasNewLayout() => waveIndex < levelWaves.Length && levelWaves[waveIndex].nextGrid != null;
    private bool HasNoMoreWaves() => waveIndex >= levelWaves.Length;

}
