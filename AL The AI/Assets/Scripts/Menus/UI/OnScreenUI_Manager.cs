using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OnScreenUI_Manager : MonoBehaviour
{
    public static OnScreenUI_Manager Instance;

    [Header("Active UI elements")]
    public TextMeshProUGUI startWave;
    public TextMeshProUGUI wave;
    public Slider resourceMeter;
    public TextMeshProUGUI resourcePercent;
    public TextMeshProUGUI ammo;
    public Slider sliderCooldown;
    public GameObject repairSlider;
    public TextMeshProUGUI money;
    public GameObject[] weaponSlotImages;
    public Image[] weaponSlotBackground;
    private readonly Color32 originalWeaponSlot = new Color32(70, 70, 70, 100); // grey
    private readonly Color32 activeWeaponSlot = new Color32(98, 132, 223, 100); // blue

    private bool missedCoroutine = false;
    private float missedValue = 0;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        if (missedCoroutine) // could happen if gameobject is not active at the same time a call to coroutine happens...
        {
            SetResourceValue(missedValue);
            missedCoroutine = false;
            missedValue = 0;
        }
    }

    public void ToggleStartWaveText()
    {
        startWave.enabled = !startWave.enabled;
    }

    public void SetWaveText(int waveNum, int maxWave)
    {
        wave.text = "WAVE: " + waveNum.ToString("00") + " / " + maxWave.ToString("00");
    }

    public void SetResourceValue(float value)
    {
        if (this.isActiveAndEnabled)
        {
            StopCoroutine("MoveResourceBar");
            StartCoroutine("MoveResourceBar", value);
        }
        else
        {
            missedCoroutine = true;
            missedValue = value;
        }
    }

    IEnumerator MoveResourceBar(float value)
    {
        while (resourceMeter.value > value)
        {
            resourceMeter.value = Mathf.Lerp(resourceMeter.value, value, Time.deltaTime * 2f);
            resourcePercent.text = "NATURAL RESOURCES \n"  + (int)value + "%";
            yield return null;
        }
    }

    public void SetAmmoText(int ammoClip, int totalAmmo)
    {
        ammo.text = "Ammo: " + ammoClip.ToString("00") + " / " + totalAmmo.ToString("000");
    }

    public void SetMoneyText(int amount)
    {
        money.text = amount.ToString("00000000");
    }

    public void SetActiveWeaponSlotColor(int oldSlotNum, int newSlotNUm)
    {
        weaponSlotBackground[oldSlotNum].color = originalWeaponSlot;
        weaponSlotBackground[newSlotNUm].color = activeWeaponSlot;
    }
}
