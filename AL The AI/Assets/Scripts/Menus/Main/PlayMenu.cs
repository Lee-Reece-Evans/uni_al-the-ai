using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayMenu : MonoBehaviour
{
    [SerializeField] private SceneLoader sceneLoader;

    [Header("Max level selection")]
    [SerializeField] private int maxLevelNumber;

    [Header("Locked Overlay")]
    [SerializeField] private GameObject locked;

    [Header("Text Components")]
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI ResorucesSavedText;
    [SerializeField] private TextMeshProUGUI DifficultyText;

    [Header("Images")]
    [SerializeField] private Image levelImage;
    [SerializeField] private Image[] ratingImages;

    [Header("Button Components")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button prevLvlButton;
    [SerializeField] private Button nextLvlButton;
    [SerializeField] private Button prevDiffButton;
    [SerializeField] private Button nextDiffButton;

    [Header("Sprites")]
    [SerializeField] private Sprite[] levelImages;
    [SerializeField] private Sprite blankStar;
    [SerializeField] private Sprite fullStar;

    // interal privates
    private int levelIndex;
    private readonly int tutorialIndex = 0;
    private DifficultyManager difficultyManager;

    void Start()
    {
        difficultyManager = DifficultyManager.instance;
        levelIndex = 0;

        DifficultyCheck();
        CheckButtonInteractable();
        SetLevelInfo();
        SetRatingInfo();
    }

    public void NextLevelButton()
    {
        SFXManager2D.instance.PlayNextSFX();
        levelIndex++;

        DifficultyCheck();
        CheckButtonInteractable();
        SetLevelInfo();
        SetRatingInfo();
    }

    public void PreviousLevelButton()
    {
        SFXManager2D.instance.PlayPreviousSFX();
        levelIndex--;

        DifficultyCheck();
        CheckButtonInteractable();
        SetLevelInfo();
        SetRatingInfo();
    }

    public void NextDifficultyButton()
    {
        SFXManager2D.instance.PlayNextSFX();
        difficultyManager.difficulty++;

        CheckButtonInteractable();
        SetLevelInfo();
        SetRatingInfo();
    }

    public void PreviousDifficultyButton()
    {
        SFXManager2D.instance.PlayNextSFX();
        difficultyManager.difficulty--;

        CheckButtonInteractable();
        SetLevelInfo();
        SetRatingInfo();
    }

    private void DifficultyCheck()
    {
        if (levelIndex == tutorialIndex) // set tutorial level to always default to normal as it has no difficulty settings
            difficultyManager.difficulty = difficultyManager.normalDifficulty;

        if (levelIndex != tutorialIndex && SaveDataManager.instance.levelRating[levelIndex, 0] < 1) // if the selected level is not the tutorial level and it has not yet been defeated on easy. set it to easy. (avoids confusion about level lock status)
        {
            difficultyManager.difficulty = difficultyManager.minDifficutly;
        }
    }

    private void CheckButtonInteractable()
    {
        // difficulty button check
        if (levelIndex == tutorialIndex) // tutorial level - no difficulty
        {
            prevDiffButton.interactable = false;
            nextDiffButton.interactable = false;
        }
        else // by design all other levels should have atleast 3 difficulty settings
        {
            if (difficultyManager.difficulty == difficultyManager.minDifficutly)
            {
                prevDiffButton.interactable = false;
                nextDiffButton.interactable = true;
            }
            else if (difficultyManager.difficulty == difficultyManager.maxDifficutly)
            {
                nextDiffButton.interactable = false;
                prevDiffButton.interactable = true;
            }
            else // can click next or previous
            {
                prevDiffButton.interactable = true;
                nextDiffButton.interactable = true;
            }
        }

        // level button check
        if (levelIndex == maxLevelNumber && levelIndex != tutorialIndex) // is max level and not the only level
        {
            nextLvlButton.interactable = false;
            prevLvlButton.interactable = true;
        }
        else if (levelIndex == tutorialIndex && levelIndex != maxLevelNumber) // is min level and not the only level
        {
            prevLvlButton.interactable = false;
            nextLvlButton.interactable = true;
        }
        else // can click next or previous
        {
            prevLvlButton.interactable = true;
            nextLvlButton.interactable = true;
        }
    }

    private void SetLevelInfo()
    {
        levelText.text = "LEVEL: " + (levelIndex + 1);
        levelImage.sprite = levelImages[levelIndex];

        if (levelIndex > SaveDataManager.instance.levelReached ||
            difficultyManager.difficulty > difficultyManager.minDifficutly && SaveDataManager.instance.levelRating[levelIndex, 0] < 1 && levelIndex != tutorialIndex) // hasn't defeated easy / hasn't reached level. can't play yet unless tutorial
        {
            startButton.interactable = false;
            locked.SetActive(true);
        }
        else
        {
            startButton.interactable = true;
            locked.SetActive(false);
        }
    }

    private void SetRatingInfo()
    {
        DifficultyText.text = "DIFFICULTY: " + difficultyManager.difficuly_Desctriptions[difficultyManager.difficulty]; // difficulty text

        ResorucesSavedText.text = "RESOURCES SAVED \n" + SaveDataManager.instance.resourcesSaved[levelIndex, difficultyManager.difficulty] + " %"; // resources saved text

        for (int i = 0; i < ratingImages.Length; i++) // change sprites to match difficulty rating
        {
            if (i < SaveDataManager.instance.levelRating[levelIndex, difficultyManager.difficulty])
                ratingImages[i].sprite = fullStar;
            else
                ratingImages[i].sprite = blankStar;
        }
    }

    public void StartLevel()
    {
        SFXManager2D.instance.PlayStartLevelSFX();
        sceneLoader.StartLevel(levelIndex + 1);
        gameObject.SetActive(false);
    }
}
