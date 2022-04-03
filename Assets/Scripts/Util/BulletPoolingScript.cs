using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPoolingScript : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab = null;

    private List<GameObject> bulletList;

    private void Awake()
    {
        bulletList = new List<GameObject>(10);

        for (int i = 0; i < 10; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab, this.transform);
            bullet.SetActive(false);
            bulletList.Add(bullet);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject GetBullet()
    {
        foreach (GameObject bullet in bulletList)
        {
            if (!bullet.activeInHierarchy)
            {
                return bullet;
            }
        }

        GameObject newBullet = Instantiate(bulletPrefab, this.transform);
        newBullet.SetActive(false);
        bulletList.Add(newBullet);
        return newBullet;
    }
}
