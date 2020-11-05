using UnityEngine;

public class Turret_Rocket : Turret_Base, IInteractable
{
    public void DoInteraction()
    {
        if (!isBroken && !IngameMenuManager.instance.shopUI.handsFull)
        {
            IngameMenuManager.instance.turretUI.SetTurret(this);
            IngameMenuManager.instance.turretUI.SetUITexts("AMMO: ROCKET", "AMMO: ROCKET");
        }
    }

    protected override void OnDisable()
    {
        // upon being disabled, set any visible enemies in my trigger back to invisible if this is their only detector
        foreach (Enemy_Base enemy in enemies)
        {
            if (enemy == null)
                continue;

            Set_Invisibility invisability = enemy.gameObject.GetComponent<Set_Invisibility>();

            if (invisability != null)
            {
                invisability.insideDetectorTrigger--; // decrease amount of triggers inside

                if (!invisability.invisible && invisability.insideDetectorTrigger == 0) // if not inside any more triggers
                {
                    invisability.insideDetectorTrigger = 0;
                    invisability.invisible = true;
                    invisability.UnSeen();
                    enemy.isInvisible = true;
                }
            }
        }

        // do base defaults
        base.OnDisable();
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        if (other.CompareTag("enemy"))
        {
            Enemy_Base enemy = other.gameObject.GetComponent<Enemy_Base>();

            Set_Invisibility invisability = other.gameObject.GetComponent<Set_Invisibility>();

            if (enemy != null && invisability != null) // enemy can be made invisible
            {
                invisability.insideDetectorTrigger++; // increase amount of triggers enemy is inside

                if (invisability.invisible && invisability.insideDetectorTrigger == 1) // is currently invisible and entered 1 trigger - set to visible
                {
                    invisability.invisible = false;
                    invisability.Seen();
                    enemy.isInvisible = false;
                }
            }
        }
    }

    protected override void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);

        //make enemy invisible again since it left the radius of rocket turret
        if (other.CompareTag("enemy"))
        {
            Enemy_Base enemy = other.gameObject.GetComponent<Enemy_Base>();

            Set_Invisibility invisability = other.gameObject.GetComponent<Set_Invisibility>();

            if (enemy != null && invisability != null) // enemy can be made invisible
            {
                invisability.insideDetectorTrigger--; // reduce amount of triggers enemy is inside

                if (!invisability.invisible && invisability.insideDetectorTrigger == 0) // if not currently invisible and not inside any triggers - set to invisible
                {
                    invisability.invisible = true;
                    invisability.UnSeen();
                    enemy.isInvisible = true;
                }
            }
        }
    }

    protected override void Shoot()
    {
        base.Shoot(); // to assign damage;

        anim.Play(shootAnimation, 0, 0);
        for (int i = 0; i < muzzle.Length; i++)
        {
            GameObject shot = ObjectPool.Instance.SpawnFromPool(projectileTag);

            if (shot != null)
            {
                MissileController missile = shot.GetComponent<MissileController>();

                if (missile != null)
                {
                    if (i == 0)
                    {
                        missile.shouldPlayImpact = true; // limit the number of impact effect by only setting the first rocket to play particles/sound.
                    }

                    missile.damage = currentDamage;
                    missile.SeekTarget(closestEnemy);
                    missile.damageType = currentDamageType;
                }

                shot.transform.position = muzzle[i].position;
                shot.transform.rotation = muzzle[i].rotation;
                shot.SetActive(true);
            }
        }
    }
}
