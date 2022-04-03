using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunSwitchScript : MonoBehaviour
{
    private int selectedWeapon = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        int currentWeapon = selectedWeapon;

        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            if (selectedWeapon >=transform.childCount - 1)
                selectedWeapon = 0;
            else
                selectedWeapon++;
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            if (selectedWeapon < 0)
                selectedWeapon = transform.childCount - 1;
            else
                selectedWeapon--;
        }

        if (Input.GetKeyDown(KeyCode.Alpha0))
            selectedWeapon = 0;
        if (Input.GetKeyDown(KeyCode.Alpha1))
            selectedWeapon = 1;
        if (Input.GetKeyDown(KeyCode.Alpha2))
            selectedWeapon = 2;

        if (currentWeapon != selectedWeapon)
            ChangeWeapon();
    }

    void ChangeWeapon()
    {
        int temp = 0;

        foreach(Transform weapon in transform)
        {
            if (temp == selectedWeapon)
                weapon.gameObject.SetActive(true);
            else
                weapon.gameObject.SetActive(false);

            temp++;

        }
    }
}
