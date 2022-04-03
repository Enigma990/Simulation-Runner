using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScripts : MonoBehaviour
{
    private Rigidbody bulletRb = null;
    private float bulletSpeed = 50f;

    private int bulletType = 0;
    private bool isFindingEnemy;

    private bool m_ShotByPlayer;

    private void Awake()
    {
        bulletRb = GetComponent<Rigidbody>();
        bulletRb.useGravity = false;
    }

    private void OnEnable()
    {
        StartCoroutine(DeactivateBullet(3f));
        isFindingEnemy = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        //bulletRb.AddForce(transform.forward * bulletSpeed, ForceMode.Impulse);
    }

    // Update is called once per frame
    void Update()
    {
        //transform.position += transform.forward * bulletSpeed * Time.deltaTime;
        if (Mathf.Abs(Vector3.Distance(this.transform.position, PlayerController.Instance.transform.position)) > 20f && bulletType == 1 && !isFindingEnemy)
        {
            FindEnemy();
        }
        
    }

    public void Shoot(Transform muzzlePos, int gunType, bool shotByPlayer)
    {
        bulletType = gunType;
        m_ShotByPlayer = shotByPlayer;

        if (gunType == 0)
        {
            bulletRb.AddForce(muzzlePos.forward * bulletSpeed, ForceMode.Impulse);
        }
        else if (gunType == 1)
        {
            bulletRb.AddForce(muzzlePos.forward * bulletSpeed, ForceMode.Impulse);

        }
    }

    void FindEnemy()
    {
        isFindingEnemy = true;

        //GameObject enemy = GameObject.FindGameObjectWithTag("Enemy");

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        //if (enemy == null)
        //    return;

        //Vector3 enemyPos = enemy.transform.position;

        if (enemies == null)
            return;

        Vector3 enemyPos = Vector3.zero;
        float distance = 100f;
        float minDistance = distance;
        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i].GetComponent<Renderer>().isVisible)
            {
                distance = Vector3.Distance(PlayerController.Instance.transform.position, enemies[i].transform.position);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    Debug.Log(enemies[i].name);
                    enemyPos = enemies[i].transform.position;
                }
            }
        }

        Vector3 screenPoint = Camera.main.WorldToViewportPoint(enemyPos);

        if (screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1)
        {
            bulletRb.velocity = Vector3.zero;
            this.transform.LookAt(enemyPos);
            bulletRb.AddForce(transform.forward * bulletSpeed, ForceMode.Impulse);
        }
    }

    IEnumerator DeactivateBullet(float deactivateTime)
    {
        yield return new WaitForSeconds(deactivateTime);
        bulletRb.velocity = Vector3.zero;
        this.gameObject.SetActive(false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!m_ShotByPlayer && collision.gameObject.CompareTag("Player"))
        {
            PlayerController.Instance.KillPlayer();
        }

        if (m_ShotByPlayer && collision.transform.CompareTag("Enemy"))
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Switch"))
                collision.transform.GetComponent<DoorSwitch>().HurtEnemy(10);
            else
                collision.transform.GetComponent<EnemyScript>().HurtEnemy(10);

            collision.gameObject.SetActive(false);
        }

        StartCoroutine(DeactivateBullet(0f));
    }
}
