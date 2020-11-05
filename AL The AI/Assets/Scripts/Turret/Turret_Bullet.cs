using UnityEngine;

public class Turret_Bullet : Turret_Base, IInteractable
{
    public void DoInteraction()
    {
        if (!isBroken && !IngameMenuManager.instance.shopUI.handsFull)
        {
            IngameMenuManager.instance.turretUI.SetTurret(this);
            IngameMenuManager.instance.turretUI.SetUITexts("AMMO: ARMOUR PIERCING", "AMMO: SHIELD PIERCING");
        }
    }

    protected override void Shoot()
    {
        base.Shoot(); // to assign damage;

        for (int i = 0; i < muzzle.Length; i++)
        {
            if (Physics.Raycast(transform.position + Vector3.up, muzzle[i].forward, out RaycastHit Hit, Mathf.Infinity, enemyLayer)) // only raycast against enemy layer
            {
                IDamageable enemy = Hit.collider.GetComponent<IDamageable>();
                if (enemy != null)
                {
                    anim.Play(shootAnimation, 0, 0);
                    audio.Play();

                    enemy.TakeDamage(currentDamage, currentDamageType);
                }
            }
        }
    }
}
