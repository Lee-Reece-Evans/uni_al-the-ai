using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    // following code is a modified version of the tutorial video by Brackeys: https://www.youtube.com/watch?v=XOjd_qU2Ido 

    // money
    public bool tutorialCompleted;
    public int money;
    
    // level data
    public int levelReached;
    public int[,] levelRating;
    public int[,] resourcesSaved;

    // shop upgrade data
    public List<string> ownedItems;

    public SaveData(SaveDataManager SDM)
    {
        // tutorial
        tutorialCompleted = SDM.tutorialCompleted;

        //money
        money = SDM.money;

        // level data
        levelReached = SDM.levelReached;
        levelRating = SDM.levelRating;
        resourcesSaved = SDM.resourcesSaved;

        // shop items data
        ownedItems = SDM.ownedItems;
    }
}


