using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunSystem : MonoBehaviour
{
    public int damage;
    public float timeBetweenShooting, spread, range, reloadTime, timeBetweenShots;
    public int magazineSize, bulletPerTap;
    public bool allowButtonHold;
    public int bulletsLeft; 
    public int bulletsShot;
    public static bool ArmaPrincipalAtiva;
    public int bulletbag;
    public int bulletbagCount;
    public int bulletinMagazine;


    //bools
    bool shooting, readyToShoot, reloading;

    //Referance
    public Camera fpsCam;
    public Transform attackPoint;
    public LayerMask whatIsEnemy;

    // Graphics
    public GameObject BulletHoleGrafic;
    public ParticleSystem mussleFlash;
    public CameraShake camShake;
    public float camShakeMagnitude, camShakeDuration;
    public Animator anim;
    public static bool isMoving;
    public AudioSource fireAudio;
    public AudioSource reloadAudio;
    private void Awake()
    {
        bulletsLeft = magazineSize;
        bulletbag = bulletbagCount / magazineSize;
        bulletinMagazine = magazineSize / bulletPerTap;
        readyToShoot = true;
    }
    private void Update()
    {
        Debug.Log(bulletbag);
        MyInput();
        if(isMoving)
        {
            anim.SetBool("Walking", true);
        }else
        {
            anim.SetBool("Walking", false);
        }
    }

    private void MyInput()
    {
       
        if (allowButtonHold && ArmaPrincipalAtiva && bulletbag > 0) shooting = Input.GetKey(KeyCode.Mouse0);
        else if(ArmaPrincipalAtiva && bulletbag > 0) shooting = Input.GetKeyDown(KeyCode.Mouse0);

        if(Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !reloading && bulletbag > 0 || bulletsLeft ==0 && bulletbag > 0)
        {
            Reload();
        }

        

        //Shoot
        if(readyToShoot && shooting && !reloading && bulletsLeft > 0 && bulletinMagazine > 0)
        {
            bulletsShot = bulletPerTap;
            bulletbagCount -= 30;
            bulletinMagazine -= 1;
            shoot();
        }
        else
        {
            anim.SetBool("Shooting", false);
        }
    }

    private void shoot()
    {
        anim.SetBool("Shooting", true);
        fireAudio.Play();
        readyToShoot = false;

        //Spread
        float x = UnityEngine.Random.Range(-spread, spread);
        float y = UnityEngine.Random.Range(-spread, spread);

        //Calculate Direction with spread
       
        Transform t = fpsCam.transform;
        t.Rotate(x, y, 0);
        Vector3 direction = t.forward;

        //Raycast
        RaycastHit hit;
        if (Physics.Raycast(fpsCam.transform.position, direction, out hit, range))
        {
            Debug.Log(hit.collider.name);

            if(hit.collider.CompareTag("Inimigo"))
            {
                //Here wahere the enemy get damage
                // after "GetComponent" is a example of script and function
                  //rayhit.collider.GetComponent<ShootingAi>().TakeDamage(damage);
                Debug.Log("GetIT");
            }
        }

        //Shakecamera
       StartCoroutine(camShake.Shake(camShakeDuration, camShakeMagnitude));

        //Graphics
        mussleFlash.Play();
        GameObject impactGO = Instantiate(BulletHoleGrafic, hit.point, Quaternion.LookRotation(hit.normal));
        Destroy(impactGO, 2f);

        bulletsLeft--;
        bulletsShot--;
        Invoke("ResetShot", timeBetweenShooting);

        if(bulletsShot > 0 && bulletsLeft > 0)
        Invoke("shoot", timeBetweenShots);
    }

    private void ResetShot()
    {
        readyToShoot = true;
    }

    private void Reload()
    {
        reloading = true;
        reloadAudio.Play();
        anim.SetBool("Reloading", true);
        Invoke("ReloadFinished", reloadTime);
    }

    private void ReloadFinished()
    {
        bulletsLeft = magazineSize;
        bulletbag = bulletbagCount / magazineSize;
        if (bulletbag >= magazineSize / bulletPerTap)
        {
            bulletinMagazine = magazineSize / bulletPerTap;
        }
        else if(bulletbag < magazineSize / bulletPerTap)
        {
            bulletinMagazine = bulletbag - bulletinMagazine;
        }
        reloading = false;
        anim.SetBool("Reloading", false);
    }
}
