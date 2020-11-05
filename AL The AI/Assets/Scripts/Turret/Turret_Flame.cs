using UnityEngine;

public class Turret_Flame : Turret_Base, IInteractable
{
    public void DoInteraction()
    {
        if (!isBroken && !IngameMenuManager.instance.shopUI.handsFull)
        {
            IngameMenuManager.instance.turretUI.SetTurret(this);
            IngameMenuManager.instance.turretUI.SetUITexts("AMMO: FIRE (AOE)", "AMMO: SNIPER (Single Target)");
        }
    }

    protected override void Shoot()
    {
        base.Shoot(); // to assign damage;

        for (int i = 0; i < muzzle.Length; i++)
        {
            GameObject shot = ObjectPool.Instance.SpawnFromPool(projectileTag);

            if (shot != null)
            {
                FireBallController fireBall = shot.GetComponent<FireBallController>();

                if (fireBall != null)
                {
                    fireBall.damageType = currentDamageType;
                    fireBall.damage = currentDamage;
                }

                shot.transform.position = muzzle[i].position;
                shot.transform.rotation = muzzle[i].rotation;
                shot.SetActive(true);
            }
        }
    }
}
