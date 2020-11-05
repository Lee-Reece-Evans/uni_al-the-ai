using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager2D : MonoBehaviour
{
    public static SFXManager2D instance;
    [System.Serializable]
    public struct WeaponSounds
    {
        public AudioClip[] weaponShoot;
        public AudioClip[] weaponReload;
    }

    [Header("MenuSounds")]
    public AudioClip menuButton;
    public AudioClip levelStart;
    public AudioClip select;
    public AudioClip purchase;
    public AudioClip next;
    public AudioClip previous;

    [Header("Ingame Shop Sounds")]
    public AudioClip itemSelect;

    [Header("Ingame openmenu Sounds")]
    public AudioClip openMenu;
    public AudioClip closeMenu;

    [Header("Ingame Wave Sounds")]
    public AudioClip waveBegin;
    public AudioClip waveEnd;

    [Header("Ingame teleport Sound")]
    public AudioClip teleport;

    [Header("Ingame TurretPlacement Sounds")]
    public AudioClip placementSound;
    public AudioClip errorSound;

    [Header("Ingame Material Pickup Sound")]
    public AudioClip materialPickup;

    [Header("Ingame Weapon Sounds")]
    public WeaponSounds[] weaponSFX;
    public AudioClip weaponSwitch;
    public AudioClip repairSound;

    [Header("Music")]
    public AudioClip MenuMusic;

    [Header("Audsio Sources")]
    public AudioSource SoundPlayer;
    public AudioSource MusicPlayer;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        MusicPlayer.clip = MenuMusic;
        MusicPlayer.Play();
    }

    // menu sounds 
    public void PlayStartLevelSFX()
    {
        SoundPlayer.PlayOneShot(levelStart);
    }

    public void PlayPurchaseSFX()
    {
        SoundPlayer.PlayOneShot(purchase);
    }

    public void PlayNextSFX()
    {
        SoundPlayer.PlayOneShot(next);
    }

    public void PlayPreviousSFX()
    {
        SoundPlayer.PlayOneShot(previous);
    }

    public void PlaySelectSFX()
    {
        SoundPlayer.PlayOneShot(select);
    }

    public void PlayStandardButtonSFX()
    {
        SoundPlayer.PlayOneShot(menuButton);
    }

    public void PlayStandardItemSelectSFX()
    {
        SoundPlayer.PlayOneShot(itemSelect);
    }

    public void PlayOpenIngameMenu()
    {
        SoundPlayer.PlayOneShot(openMenu);
    }

    public void PlayCloseIngameMenu()
    {
        SoundPlayer.PlayOneShot(closeMenu);
    }

    // wave sounds

    public void PlayWaveBeginSound()
    {
        SoundPlayer.PlayOneShot(waveBegin);
    }

    public void PlayWaveEndSound()
    {
        SoundPlayer.PlayOneShot(waveEnd);
    }

    // placing turret sounds

    public void PlayPlacementSound()
    {
        SoundPlayer.PlayOneShot(placementSound);
    }

    public void PlayErrorSound()
    {
        SoundPlayer.PlayOneShot(errorSound);
    }

    // material pickup sound

    public void PlayMaterialSound()
    {
        SoundPlayer.PlayOneShot(materialPickup);
    }

    // player weapon sounds

    public void PlayWeaponShootSound(int weaponIndex)
    {
        //SoundPlayer.clip = weaponSFX[weaponIndex].weaponShoot[Random.Range(0, weaponSFX[weaponIndex].weaponShoot.Length)];
        SoundPlayer.PlayOneShot(weaponSFX[weaponIndex].weaponShoot[Random.Range(0, weaponSFX[weaponIndex].weaponShoot.Length)]);
    }

    public void PlayWeaponReloadSound(int weaponIndex)
    {
        SoundPlayer.clip = weaponSFX[weaponIndex].weaponReload[Random.Range(0, weaponSFX[weaponIndex].weaponReload.Length)];
        SoundPlayer.Play();
    }

    public void StopSoundPlayer()
    {
        SoundPlayer.Stop();
    }

    public void PlayWeaponSwitchSound()
    {
        SoundPlayer.PlayOneShot(weaponSwitch);
    }

    public void PlayTeleportSound()
    {
        SoundPlayer.PlayOneShot(teleport);
    }

    public void PlayRepairGunSound()
    {
        SoundPlayer.clip = repairSound;
        SoundPlayer.loop = true;
        SoundPlayer.Play();
    }

    public void StopRepairGunSound()
    {
        SoundPlayer.loop = false;

        if (SoundPlayer.clip == repairSound)
            SoundPlayer.Stop();
    }
}
