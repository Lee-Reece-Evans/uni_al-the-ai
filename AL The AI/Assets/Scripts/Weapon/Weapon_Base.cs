using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Base : MonoBehaviour
{
    protected Camera playerCam; // for raycasting

    [Header("Muzzle Position")]
    [SerializeField] protected Transform muzzle;

    [Header("MuzzleFlash")]
    [SerializeField] protected GameObject muzzleFlash;

    [Header("Generic Weapon Variables")]
    public bool canUse;
    public int clipSize;
    public int maxAmmo;
    public float fireRate;
    public float reloadSpeed;
    public float range;

    [Header("Ammo values")]
    public int totalAmmo;
    public int ammoInClip;

    [Header("Pooled Projectile Tags")]
    [SerializeField] protected string primaryProjectileTag;
    [SerializeField] protected string secondaryProjectileTag;

    [Header("Primary Shot values")]
    [SerializeField] protected int primaryAmmoCost;
    [SerializeField] protected int primaryMinDamage;
    [SerializeField] protected int primaryMaxDamage;
    [SerializeField] protected float primarySpeed;
    [SerializeField] protected DamageTypes primaryDamageType; 

    [Header("Secondary Shot values")]
    [SerializeField] protected int secondaryAmmoCost;
    [SerializeField] protected int SecondaryDamage;
    [SerializeField] protected float secondarySpeed;
    [SerializeField] protected DamageTypes secondaryDamageType;
    [SerializeField] protected float secondaryCooldown;

    // privates
    private int currentDamage;
    private bool isShooting;
    private bool isReloading;
    private bool secondaryCanShoot;

    // layer masks
    protected int Enemieslayermask;
    protected int interactableLayerMask;
    protected int EverythingLayerMask;

    private void Start()
    {
        playerCam = Camera.main;
        totalAmmo = maxAmmo;
        ammoInClip = clipSize;
        secondaryCanShoot = true;

        Enemieslayermask = (1 << LayerMask.NameToLayer("Enemies"));
        interactableLayerMask = (1 << LayerMask.NameToLayer("Interactable")) | (1 << LayerMask.NameToLayer("Buildable"));
        EverythingLayerMask = ~(LayerMask.GetMask("Player", "BlockPlayer"));
    }

    public virtual void PrimaryShot() // can be overridden in sub class for different behaviour
    {
        muzzleFlash.SetActive(false);
        muzzleFlash.SetActive(true);

        ammoInClip -= primaryAmmoCost;
        SetAmmoText();

        // weapon specific shot behaviour in override
    }

    public virtual void SecondaryShot() // can be overridden in sub class for different behaviour // not currently used by any weapons
    {
        GameObject shot = ObjectPool.Instance.SpawnFromPool(secondaryProjectileTag);

        if (shot != null)
        {
            ammoInClip -= secondaryAmmoCost;
            SetAmmoText();
            StartCoroutine("SecondaryCooldownTImer");

            shot.transform.position = muzzle.position;
            shot.transform.rotation = muzzle.rotation;
            shot.SetActive(true);
        }
    }

    IEnumerator SecondaryCooldownTimer()
    {
        secondaryCanShoot = false;
        yield return new WaitForSeconds(secondaryCooldown);
        secondaryCanShoot = true;
    }

    public bool CheckCanReload() // is reload possible?
    {
        if (totalAmmo == 0 || ammoInClip == clipSize) // gun is full or no bullets left
            return false;

        return true;
    }

    public virtual void Reload()
    {
        int ammoToAdd = clipSize - ammoInClip; // how many bullets are missing 

        if (totalAmmo >= ammoToAdd) // fully fill ammo clip
        {
            ammoInClip += ammoToAdd;
            totalAmmo -= ammoToAdd;
        }
        else // partially fill ammo clip
        {
            ammoInClip += totalAmmo;
            totalAmmo -= totalAmmo;
        }

        SetAmmoText();
    }

    public void AddAmmo()
    {
        totalAmmo = maxAmmo;
    }

    public virtual void SetAmmoText()
    {
        OnScreenUI_Manager.Instance.SetAmmoText(ammoInClip, totalAmmo);
    }
}
