using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyScript : MonoBehaviour
{
    [SerializeField] private BulletPoolingScript bulletList = null;
    [SerializeField] private Transform gunMuzzle = null;
    private NavMeshAgent agent = null;

    // Alert Data
    private BoxCollider detectionCollider = null;
    private bool isAlert = false;
    private Vector3 alertDetectionRange = new Vector3(40f, 5f, 40f);
    private Vector3 basicDetectionRange = new Vector3(25f, 2f, 25f);

    //Shooting Data
    private int gunType = 0;
    private float shootingDistance = 10f;
    private int fireRate = 3;
    private float lastFire = 0f;

    private void Awake()
    {
        //Assigning navmeshagent
        agent = GetComponent<NavMeshAgent>();

        //Assigning Box collider
        detectionCollider = GetComponent<BoxCollider>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Check alert
        if(PlayerController.Instance.IsRunning && !isAlert)
        {
            detectionCollider.size = alertDetectionRange;
            isAlert = true;
        }
        if(!PlayerController.Instance.IsRunning && isAlert)
        {
            detectionCollider.size = basicDetectionRange;
            isAlert = false;
        }


        if (lastFire < fireRate)
            lastFire += Time.deltaTime;
    }

    void Shoot()
    {
        lastFire = 0;
        GameObject bullet = bulletList.GetBullet();
        bullet.transform.position = gunMuzzle.position;
        bullet.transform.rotation = Quaternion.identity;
        bullet.SetActive(true);
        bullet.GetComponent<BulletScripts>().Shoot(gunMuzzle, gunType, false);

    }

    public void HurtEnemy(int _damage)
    {

    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            transform.LookAt(Vector3.Lerp(transform.position, other.transform.position, 1f));

            if (Vector3.Distance(this.transform.position, other.transform.position) <= shootingDistance)
            {
                agent.isStopped = true;
                gunMuzzle.transform.LookAt(other.transform.position);
                if (lastFire >= fireRate)
                    Shoot();
            }
            else
            {
                if(agent.isStopped)
                    agent.isStopped = false;
                agent.SetDestination(other.transform.position);
            }
        }
    }

}
