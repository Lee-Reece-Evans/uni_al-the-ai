using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [Header("Weapon animator")]
    [SerializeField] private Animator anim;

    [Header("Weapon References")]
    [SerializeField] private GameObject[] weapons;
    public Weapon_Base[] weaponScript;

    // internal privates
    private int weaponSelected = 0;
    private bool handsEmpty = false;
    private int weaponIndex = 0;

    private enum WeaponValues
    {
        isPistol = 0,
        isRifle = 1,
        isShotgun = 2,
        isSniper = 3,
        isRepair = 4
    }

    void Start()
    {
        weaponScript = new Weapon_Base[weapons.Length];

        for (int i = 0; i < weapons.Length; i++)
            weaponScript[i] = weapons[i].GetComponent<Weapon_Base>();

        SetAnimatorValues();
        anim.SetBool(WeaponValues.isPistol.ToString(), true);
        weaponSelected = 0;
    }

    // Update is called once per frame
    void Update()
    {
        // put away weapon when in menu or holding a buildable item
        if (IngameMenuManager.instance.shopMenuObj.activeSelf && !handsEmpty)
        {
            anim.SetTrigger("usingShop");
            handsEmpty = true;
        }
        else if (!IngameMenuManager.instance.shopMenuObj.activeSelf && !IngameMenuManager.instance.shopUI.handsFull && handsEmpty)
        {
            anim.SetTrigger("shopClosed");
            handsEmpty = false;
        }

        if (GameManager.instance.gamePaused || GameManager.instance.gameOver || IngameMenuManager.instance.usingMenu)
        {
            anim.SetBool("canShoot", false);
            return;
        }

        // weapon switching
        if (!handsEmpty)
        {
            // shooting
            if (Input.GetMouseButtonDown(0))
                ShootPrimary();

            if (Input.GetMouseButtonUp(0))
                anim.SetBool("canShoot", false);

            //if (Input.GetMouseButtonDown(1)) // not completed implementation
            //    ShootSecondary();

            if (Input.GetMouseButtonUp(1))
                anim.SetBool("canShoot", false);

            // weapon switching
            if (Input.GetAxis("Mouse ScrollWheel") > 0f) // mouse wheel up
            {
                for (int i = weaponSelected + 1; i < weaponScript.Length; i++)
                {
                    if (weaponScript[i].canUse)
                    {
                        SelectWeapon(((WeaponValues)i).ToString(), i);
                        break;
                    }
                }
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0f) // mouse wheel down
            {
                for (int i = weaponSelected - 1; i >= 0; i--)
                {
                    if (weaponScript[i].canUse)
                    {
                        SelectWeapon(((WeaponValues)i).ToString(), i);
                        break;
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.Alpha1))
                SelectWeapon(WeaponValues.isPistol.ToString(), (int)WeaponValues.isPistol);

            if (Input.GetKeyDown(KeyCode.Alpha2))
                SelectWeapon(WeaponValues.isRifle.ToString(), (int)WeaponValues.isRifle);

            if (Input.GetKeyDown(KeyCode.Alpha3))
                SelectWeapon(WeaponValues.isShotgun.ToString(), (int)WeaponValues.isShotgun);

            if (Input.GetKeyDown(KeyCode.Alpha4))
                SelectWeapon(WeaponValues.isSniper.ToString(), (int)WeaponValues.isSniper);

            if (Input.GetKeyDown(KeyCode.Alpha5))
                SelectWeapon(WeaponValues.isRepair.ToString(), (int)WeaponValues.isRepair);

            // reloading
            if (Input.GetKeyDown(KeyCode.R))
                TriggerReloadGun();
        }
    }

    private void SelectWeapon(string weapName, int weaponNum)
    {
        if (!weaponScript[weaponNum].canUse) // only equip a valid weapon
            return;

        if (weaponSelected != weaponNum && weaponIndex != weaponNum) // check not currently using or trying to use
        {
            AnimBoolsToFalse();
            anim.SetBool(weapName, true);

            OnScreenUI_Manager.Instance.SetActiveWeaponSlotColor(weaponIndex, weaponNum); // set weapon slot color

            weaponIndex = weaponNum;
            anim.SetTrigger("switchWeapon");
        }
    }

    private void AnimBoolsToFalse()
    {
        anim.SetBool(WeaponValues.isShotgun.ToString(), false);
        anim.SetBool(WeaponValues.isRifle.ToString(), false);
        anim.SetBool(WeaponValues.isPistol.ToString(), false);
        anim.SetBool(WeaponValues.isSniper.ToString(), false);
        anim.SetBool(WeaponValues.isRepair.ToString(), false);
    }

    private void SwitchWeapon()
    {
        SFXManager2D.instance.PlayWeaponSwitchSound();
        weaponSelected = weaponIndex;
        anim.SetFloat("shootSpeed", weaponScript[weaponSelected].fireRate);
        anim.ResetTrigger("Reload");

        for (int i = 0; i < weapons.Length; i++)
        {
            if (i == weaponSelected)
            {
                weaponScript[weaponSelected].SetAmmoText();
                anim.SetFloat("reloadSpeed", weaponScript[weaponSelected].reloadSpeed);
                weapons[i].SetActive(true);
                SetAnimatorValues();
            }
            else
                weapons[i].SetActive(false);
        }
    }

    private void ShootPrimary()
    {
        anim.SetFloat("shootSpeed", weaponScript[weaponSelected].fireRate);
        anim.SetBool("canShoot", true);
    }

    public void SpawnPrimaryShot() // called by shoot anim event
    {
        if (!IsPlayingAnimation("WeaponSwitch")) // don't shoot a bullet during switching weapons
        {
            SFXManager2D.instance.PlayWeaponShootSound(weaponSelected);
            weaponScript[weaponSelected].PrimaryShot();
            SetAnimatorValues();
        }
    }

    private void ShootSecondary() // not used by any weapons
    {
        anim.SetFloat("shootSpeed", weaponScript[weaponSelected].fireRate);
        anim.SetBool("canShoot", true);
    }

    public void SpawnSecondaryShot() // not used by any weapons
    {
        weaponScript[weaponSelected].SecondaryShot();
        SetAnimatorValues();
    }

    private void TriggerReloadGun()
    {
        if (weaponScript[weaponSelected].CheckCanReload() && !IsPlayingAnimation("Reload")) // check there is ammo to reload and not already reloading
        {
            anim.SetTrigger("Reload");
        }
    }

    public void PlayReloadSound() // accessed by anim event
    {
        SFXManager2D.instance.PlayWeaponReloadSound(weaponSelected);
    }

    public void FinishReload()
    {
        weaponScript[weaponSelected].Reload();
        SetAnimatorValues();
    }

    public void SetAnimatorValues()
    {
        anim.SetInteger("ammo", weaponScript[weaponSelected].ammoInClip);
        anim.SetInteger("totalAmmo", weaponScript[weaponSelected].totalAmmo);
    }

    private bool IsPlayingAnimation(string animTag) // check if animation is currently playing
    {
        if (anim.IsInTransition(0) && anim.GetNextAnimatorStateInfo(0).IsTag(animTag) || 
            anim.GetCurrentAnimatorStateInfo(0).IsTag(animTag) && anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
            return true;

        return false;
    }
}
