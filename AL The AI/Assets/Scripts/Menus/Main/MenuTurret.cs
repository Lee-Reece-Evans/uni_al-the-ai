using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuTurret : MonoBehaviour
{
    public GameObject turretRotation;
    public ParticleSystem muzzleFlash;
    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 cursorPos = new Vector3(mouseWorldPos.x, mouseWorldPos.y, 0f);
        Vector3 direction = (cursorPos - turretRotation.transform.position).normalized;
        Quaternion lookatrotation = Quaternion.LookRotation(direction);
        Quaternion X = Quaternion.Euler(lookatrotation.eulerAngles.x, turretRotation.transform.rotation.eulerAngles.y, turretRotation.transform.rotation.eulerAngles.z);

        if (cursorPos.x < transform.position.x - 3)
            turretRotation.transform.rotation = X;

        if (Input.GetMouseButtonDown(0))
        {
            anim.Play("BulletTurretShoot", 0, 0);
        }
    }

    public void MuzzleFlash()
    {
        muzzleFlash.Play();
    }
}
