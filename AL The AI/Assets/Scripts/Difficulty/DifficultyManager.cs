using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    public static DifficultyManager instance;
    public readonly int maxDifficutly = 2;
    public readonly int normalDifficulty = 1;
    public readonly int minDifficutly = 0;
    public readonly string[] difficuly_Desctriptions = new string[] { "EASY", "NORMAL", "HARD" }; // easy, normal, hard
    public readonly int[] difficulty_ResourceModifier = new int[] { -1, 0, 1 }; // easy, normal, hard
    public readonly int[] difficulty_HealthModifier = new int[] { 3, 6, 9 }; // easy, normal, hard health increases per wave
    public readonly int[] difficulty_SpawnAmountModifier = new int[] { -2, 0, 1 }; // easy, normal, hard health increases per wave
    public readonly int[] difficulty_WaveAmountModifier = new int[] { 5, 7, 10 }; // easy, normal, hard amount of waves to spawn

    public int difficulty;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            difficulty = normalDifficulty; // default to normal
            DontDestroyOnLoad(gameObject);
        }
    }
}
