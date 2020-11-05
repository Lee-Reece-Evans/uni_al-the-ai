using UnityEngine;

public class RepairGun : Weapon_Base
{
    [SerializeField] private LineRenderer linerRenderer;

    [SerializeField] private float activeThreshold = 5;
    [SerializeField] private float cooldownTimer = 5;

    private float nextFireTime = 0f;
    private float disabledTime = 0;
    private bool playingSound = false;
    private bool isRepairing = false;
    private bool minRechargeAmount = true;

    public override void PrimaryShot()
    {
        // not needed for repair
    }

    public override void SecondaryShot()
    {
        // not needed for repair
    }

    public override void Reload()
    {
        // not needed for repair
    }

    public override void SetAmmoText()
    {
        OnScreenUI_Manager.Instance.ammo.enabled = false;
        OnScreenUI_Manager.Instance.repairSlider.SetActive(true);
    }

    private void Repair()
    {
        // check cooldown and that we are hitting something repairable.
        if (cooldownTimer > 0 && minRechargeAmount && Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out RaycastHit Hit, range, interactableLayerMask))
        {
            isRepairing = true;

            cooldownTimer -= Time.deltaTime; // take away from charge amount

            OnScreenUI_Manager.Instance.sliderCooldown.value = cooldownTimer / activeThreshold;

            if (!playingSound)
                StartSound();

            if (!linerRenderer.enabled)
                linerRenderer.enabled = true;

            linerRenderer.SetPosition(0, muzzle.position);
            linerRenderer.SetPosition(1, Hit.point);

            if (nextFireTime <= 0)
            {
                IRepairable repairable = Hit.collider.GetComponentInParent<IRepairable>();

                if (repairable != null)
                {
                    if (!WaveManager.instance.waveInProgress) // check if wave is in progress
                        repairable.Repair(primaryMaxDamage); // repair quicker if not in combat state
                    else
                        repairable.Repair(primaryMinDamage); // repair slower during wave so that it cannot be abused
                }

                nextFireTime = 1f / fireRate;
            }
            nextFireTime -= Time.deltaTime;
        }
        else
        {
            DeactivateRepair();
        }
    }

    private void StartSound()
    {
        SFXManager2D.instance.PlayRepairGunSound();
        playingSound = true;
    }

    private void EndSound()
    {
        SFXManager2D.instance.StopRepairGunSound();
        playingSound = false;
    }

    private void DeactivateRepair()
    {
        isRepairing = false;

        if (linerRenderer.enabled)
            linerRenderer.enabled = false;

        if (playingSound)
            EndSound();
    }

    private void Update()
    {
        if (GameManager.instance.gamePaused || GameManager.instance.gameOver || IngameMenuManager.instance.usingMenu)
        {
            DeactivateRepair();

            return;
        }

        if (!isRepairing && cooldownTimer < activeThreshold) // if not repairing and cooldowntimer is less than full. refill it.
        {
            if (cooldownTimer < 1)
                minRechargeAmount = false;
            else
                minRechargeAmount = true;

            if (!WaveManager.instance.waveInProgress)
                cooldownTimer += Time.deltaTime * 1.5f; // recharge quicker when not in an active wave since you will likely be building more.
            else
                cooldownTimer += Time.deltaTime;

            OnScreenUI_Manager.Instance.sliderCooldown.value = cooldownTimer / activeThreshold;
        }

        if (Input.GetButton("Fire1"))
        {
            Repair();
        }

        if (Input.GetButtonUp("Fire1"))
        {
            DeactivateRepair();
        }
    }

    private void OnEnable()
    {
        // if cooldown timer is not at max keep an active timer
        if (cooldownTimer < activeThreshold)
        {
            if (cooldownTimer + (Time.time - disabledTime) < activeThreshold) // check time elapsed since last disabled.
            {
                cooldownTimer += (Time.time - disabledTime);
                OnScreenUI_Manager.Instance.sliderCooldown.value = cooldownTimer / activeThreshold;
            }
            else // time elapsed + cooldown timer amount is more than the max. so set to max.
            {
                cooldownTimer = activeThreshold;
                minRechargeAmount = true; // incase when we left there was a min recharge amount but when we come back we have full charge.
                OnScreenUI_Manager.Instance.sliderCooldown.value = cooldownTimer / activeThreshold;
            }
        }
    }

    private void OnDisable()
    {
        disabledTime = Time.time;

        DeactivateRepair();
        if (OnScreenUI_Manager.Instance != null)
        {
            OnScreenUI_Manager.Instance.ammo.enabled = true;
            OnScreenUI_Manager.Instance.repairSlider.SetActive(false);
        }
    }
}
