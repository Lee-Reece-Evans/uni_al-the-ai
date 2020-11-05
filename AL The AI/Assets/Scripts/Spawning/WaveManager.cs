using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager instance;

    public bool waveInProgress;
    public int wave;
    public int maxWaves;
    public int enemiesRemaining;
    public int totalEnemiesThisWave;

    [Header("Spawn areas")]
    [SerializeField] private SpawnArea[] spawnAreas;

    [Header("Variable overrides")]
    [SerializeField] private bool overrideWaves = false; // if difficulty should be ignored
    [SerializeField] private bool overrideWinCondition = false; // if calling gamewon should be ignored after final wave

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if (!overrideWaves) // if not set manually, use default difficulty settings.
        {
            maxWaves = DifficultyManager.instance.difficulty_WaveAmountModifier[DifficultyManager.instance.difficulty];
        }

        OnScreenUI_Manager.Instance.SetWaveText(wave, maxWaves);
        SetNextWavePortraits();
    }

    private void OnDisable()
    {
        Enemy_Base.DeathEvent -= EnemyDied;
    }

    private void StartWave()
    {
        SFXManager2D.instance.PlayWaveBeginSound();
        waveInProgress = true;
        OnScreenUI_Manager.Instance.ToggleStartWaveText();
        Enemy_Base.DeathEvent += EnemyDied;
        totalEnemiesThisWave = 0;
        foreach (SpawnArea spawnArea in spawnAreas) // get enemy totals
        {
            spawnArea.StartWave(wave);
            totalEnemiesThisWave += spawnArea.GetTotalEnemies();
        }
        enemiesRemaining = totalEnemiesThisWave;

        wave++; // increment wave for next time
        OnScreenUI_Manager.Instance.SetWaveText(wave, maxWaves); // set wave text here since wave starts at 0, it should go after incrementing.
    }

    private void Endwave() 
    {
        SFXManager2D.instance.PlayWaveEndSound();
        Enemy_Base.DeathEvent -= EnemyDied;
        waveInProgress = false;

        if (wave != maxWaves)
        {
            OnScreenUI_Manager.Instance.ToggleStartWaveText(); 
            SetNextWavePortraits();
        }
        else
        {
            if (!overrideWinCondition)
                GameManager.instance.GameWon(); 
        }
    }

    private void SetNextWavePortraits()
    {
        foreach (SpawnArea spawnArea in spawnAreas) // set enemy portraits for next wave
        {
            spawnArea.SetNextWavePortraits(wave);
        }
    }

    public void EnemyDied(Enemy_Base enemy) // decrenebt enemy total / check for end of wave
    {
        enemiesRemaining--;
        if (enemiesRemaining == 0)
            Endwave();
    }

    private void Update()
    {
        if (GameManager.instance.gamePaused || GameManager.instance.gameOver || wave == maxWaves)
            return;

        if (!waveInProgress)
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                StartWave();
            }
        }
    }
}
