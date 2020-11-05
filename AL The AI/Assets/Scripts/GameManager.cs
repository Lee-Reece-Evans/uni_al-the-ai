using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int difficulty;
    public bool gameOver;
    public bool gamePaused;
    public bool fixedReward;
    public int fixedRewardAmount;

    private int levelIndex;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        difficulty = DifficultyManager.instance.difficulty;

        gamePaused = false;
        gameOver = false;

        levelIndex = SceneManager.GetActiveScene().buildIndex -1; // -1 to offset mainmenu scene
    }

    public void GameWon()
    {
        gameOver = true;
        // calculate rating value
        int rating = Mathf.RoundToInt(OnScreenUI_Manager.Instance.resourceMeter.value / 20); // 1 to 5 rating
        rating = Mathf.Clamp(rating, 1, 5);

        // check if better than previous rating, if it is then overwrite...
        if (SaveDataManager.instance.levelRating[levelIndex, difficulty] < rating)
            SaveDataManager.instance.levelRating[levelIndex, difficulty] = rating;

        // get resources remaining
        int resourcesSaved = Mathf.RoundToInt(OnScreenUI_Manager.Instance.resourceMeter.value);

        // check if resources remaining is better than previous best. save amount of resources remaining in level.
        if (SaveDataManager.instance.resourcesSaved[levelIndex, difficulty] < resourcesSaved)
            SaveDataManager.instance.resourcesSaved[levelIndex, difficulty] = resourcesSaved;

        // save remaining currency to wallet - either capped amount or better rating means more money to keep
        int reward = 0;

        if (fixedReward)
        {
            if (levelIndex == 0 && !SaveDataManager.instance.tutorialCompleted)  // could be updated to be more general if other levels only have 1 time rewards...
            {
                reward = fixedRewardAmount;
                SaveDataManager.instance.money += reward;
                SaveDataManager.instance.tutorialCompleted = true;
            }
            else if (levelIndex != 0)
            {
                reward = fixedRewardAmount;
                SaveDataManager.instance.money += reward;
            }
        }
        else
        {
            int oneFith = Mathf.RoundToInt(PlayerStats.instance.money / 5.0f); // fraction the players remaining money.
            reward = oneFith * rating;
            SaveDataManager.instance.money += reward; // any remaining money at level end is calculated based on rating and added to their shop currency to buy upgrades
        }

        // increase level reached if the current level completed is equal to the current level reached
        if (levelIndex == SaveDataManager.instance.levelReached)
            SaveDataManager.instance.levelReached++;

        //save to file
        SaveDataManager.instance.SaveLevelData();
        IngameMenuManager.instance.OpenGameWonMenu(rating, reward);
    }

    public void GameLost()
    {
        gameOver = true;
        IngameMenuManager.instance.OpenGameoverMenu();
    }
}
