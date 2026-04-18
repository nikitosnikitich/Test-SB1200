using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//
using Photon.Pun;

public class ShootGun : Gun
{
    private PhotonView myPv;

    [SerializeField] Camera myCam;

    private void Awake()
    {
        myPv = GetComponent<PhotonView>();
        gunAnimator = GetComponentInChildren<Animator>();
    }

    public override void Use()
    {
        Shoot();
    }

    private void Shoot()
    {
        gunAnimator.Play("Shoot_gun");
        //
        Ray ray = myCam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        ray.origin = myCam.transform.position;

        if(Physics.Raycast(ray, out RaycastHit hit))
        {
            hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(((GunInfo)itemInfo).Damage);
            //
            myPv.RPC("RPC_SHOOT", RpcTarget.All, hit.point, hit.normal);
        }
    }

    [PunRPC]
    void RPC_SHOOT(Vector3 hitpoint, Vector3 hitnormal)
    {
        GameObject tempFX = Instantiate(shootFX, muzzle.position, Quaternion.identity);
        tempFX.transform.SetParent(muzzle);
        //
        Collider [] colls = Physics.OverlapSphere(hitpoint, 0.1f);

        if(colls.Length != 0)
        {
            GameObject bulletImp = Instantiate(bulletPrefab, hitpoint, 
                                    Quaternion.LookRotation(hitnormal, Vector3.up) * bulletPrefab.transform.rotation);
            
            bulletImp.transform.SetParent(colls[0].transform);
        }
    }
}
