using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveDataManager : MonoBehaviour
{
    public static SaveDataManager instance;
    public int maxLevel = 5;

    //local copy of save data for manipulation and saving back to save file
    // tutorial
    public bool tutorialCompleted;

    // money
    public int money;

    // level data
    public int levelReached;
    public int[,] levelRating;
    public int[,] resourcesSaved;

    // shop upgrade data
    public List<string> ownedItems;

    private void Awake() // persistant singleton
    {
        if (instance != null && instance != this)
            Destroy(gameObject);
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            // only used for creating the initial save data file.
            // initialise array to length of max levels * max difficulties. in this case 3 difficulties per 1 level.
            levelRating = new int[maxLevel, maxLevel * 3];
            resourcesSaved = new int[maxLevel, maxLevel * 3];
            tutorialCompleted = false;

            // load game data, if no data exsists a save file will be created instead. and the data local to this file will be used to initialise it.
            LoadData();
        }
    }

    public void SaveLevelData()
    {
        SaveSystem.SaveLevel(this);
    }

    public void LoadData()
    {
        // this function is a modified version of the tutorial video by Brackeys: https://www.youtube.com/watch?v=XOjd_qU2Ido 
        SaveData data = SaveSystem.LoadLevel();

        if (data == null) // no data so don't do anything
            return;

        // tutorial
        tutorialCompleted = data.tutorialCompleted;

        // money
        money = data.money;

        // level data
        levelReached = data.levelReached;
        levelRating = data.levelRating;
        resourcesSaved = data.resourcesSaved;

        // shop upgrade data
        ownedItems = data.ownedItems;
    }
}
