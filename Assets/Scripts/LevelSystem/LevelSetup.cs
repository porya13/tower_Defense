using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSetup : MonoBehaviour
{
    private UI ui;
    private TileAnimator tileAnimator;
    private LevelManager levelManager;
    private GameManager gameManager;

    [Header("Level Details")]
    [SerializeField] private int levelCurrency = 1000;
    [SerializeField] private List<TowerUnlockData> towerUnlocks;

    [Header("Level Setup")]
    [SerializeField] private GridBuilder myMainGrid;
    [SerializeField] private WaveManager myWaveManager;
    [SerializeField] private List<GameObject> extraObjectsToDelete = new List<GameObject>();


    private IEnumerator Start()
    {
        UnlockAvalibleTowers();

        if (LevelWasLoadedToMainScene())
        {
            DeleteExtraObjects();

            levelManager.UpdateCurrentGrid(myMainGrid);

            tileAnimator = FindFirstObjectByType<TileAnimator>();
            tileAnimator.ShowGrid(myMainGrid, true);

            yield return tileAnimator.GetCurrentActiveCo();

            ui = FindFirstObjectByType<UI>();
            ui.EnableInGameUI(true);

            gameManager = FindFirstObjectByType<GameManager>();
            gameManager.UpdateGameManager(levelCurrency,myWaveManager);

            myWaveManager.ActivateWaveManager();
        }
    }

    private bool LevelWasLoadedToMainScene()
    {
        levelManager = FindFirstObjectByType<LevelManager>();

        return levelManager != null;
    }

    private void DeleteExtraObjects()
    {
        foreach (var obj in extraObjectsToDelete)
        {
            Destroy(obj);
        }
    }

    private void UnlockAvalibleTowers()
    {
        UI ui = FindFirstObjectByType<UI>();

        foreach (var unlockData in towerUnlocks)
        {
            foreach(var buildButton in ui.buildButtonsUI.GetBuildButtons())
            {
                buildButton.UnlockTowerIfNeeded(unlockData.towerName, unlockData.unlocked);
            }
        }

        ui.buildButtonsUI.UpdateUnlockedButtons();
    }

    [ContextMenu("Initialize Tower Data")]
    private void InitialiezTowerData()
    {
        towerUnlocks.Clear();

        towerUnlocks.Add(new TowerUnlockData("Crossbow", false));
        towerUnlocks.Add(new TowerUnlockData("Cannon", false));
        towerUnlocks.Add(new TowerUnlockData("Rapid Fire Gun", false));
        towerUnlocks.Add(new TowerUnlockData("Hammer", false));
        towerUnlocks.Add(new TowerUnlockData("Spider Nest", false));
        towerUnlocks.Add(new TowerUnlockData("AA Harpon", false));
        towerUnlocks.Add(new TowerUnlockData("Just Fan", false));
    }
}


[System.Serializable]
public class TowerUnlockData
{
    public string towerName;
    public bool unlocked;

    public TowerUnlockData(string newTowerName, bool newUnlockedStatus)
    {
        towerName = newTowerName;
        unlocked = newUnlockedStatus;
    }
}