using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunScript : MonoBehaviour
{
    [SerializeField] private Transform gunPosition = null;
    [SerializeField] private Transform gunMuzzle = null;
    [SerializeField] private BulletPoolingScript bulletList;

    private Animator gunAnim = null;
    private Quaternion defaultRotation;

    private enum GunType { BasicGun, SmartGun}
    [SerializeField] private GunType currentGun;

    private int currentBullets;
    private int maxBullets;

    private bool isEquipped = false;
    private bool isReloading = false;

    private void Awake()
    {
        switch(currentGun)
        {
            case GunType.BasicGun:
                maxBullets = 5;
                defaultRotation = Quaternion.Euler(0f, -90f, 0f);
                break;
            case GunType.SmartGun:
                maxBullets = 3;
                defaultRotation = Quaternion.identity;
                break;
        }

        currentBullets = maxBullets;

        //Assigning Animator
        gunAnim = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        isReloading = false;
        gunAnim.SetBool("isReloading", isReloading);
        SetRotation();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && isEquipped && !isReloading) 
        {
            if (currentBullets > 0)
                Shoot();
            //else
            //    Reload();
        }

        if(Input.GetKeyDown(KeyCode.R) && currentBullets != maxBullets)
        {
            StartCoroutine(Reload());
        }

        //switch(currentGun)
        //{
        //    case GunType.BasicGun:

        //        if (!isReloading && this.transform.localRotation != defaultRotation)
        //            this.transform.localRotation = defaultRotation;
        //        Debug.Log("BS");
        //        break;

        //    case GunType.SmartGun:
        //        if (!isReloading && this.transform.localRotation != defaultRotation)
        //            this.transform.localRotation = defaultRotation;
        //        Debug.Log("SM");
        //        break;

        //}
    }

    void SetRotation()
    {
        transform.localRotation = defaultRotation;
    }

    void Shoot()
    {
        GameObject bullet = bulletList.GetBullet();
        bullet.transform.position = gunMuzzle.position;
        bullet.transform.rotation = Quaternion.identity;
        bullet.SetActive(true);
        bullet.GetComponent<BulletScripts>().Shoot(gunMuzzle, (int)currentGun, true);

        // bullet.GetComponent<Rigidbody>().AddForce(gunMuzzle.right * 20f, ForceMode.Impulse);

        currentBullets--;
    }

    IEnumerator Reload()
    {
        isReloading = true;
        gunAnim.SetBool("isReloading", isReloading);

        yield return new WaitForSeconds(0.7f);

        isReloading = false;
        gunAnim.SetBool("isReloading", isReloading);

        currentBullets = maxBullets;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isEquipped) 
        {
            transform.SetParent(gunPosition);
            transform.localPosition = Vector3.zero;

            SetRotation();

            //switch (currentGun)
            //{
            //    case GunType.BasicGun:
            //        transform.localRotation = defaultRotation;
            //        break;

            //    case GunType.SmartGun:
            //        transform.localRotation = defaultRotation;
            //        break;
            //}

            transform.gameObject.SetActive(false);

            isEquipped = true;
        }
    }
}
