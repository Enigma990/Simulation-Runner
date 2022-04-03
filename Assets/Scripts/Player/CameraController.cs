using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour
{
    [Header("Camera Reference")]
    [SerializeField] Transform camera = null;
    [SerializeField] Transform orientation = null;

    [Header("Camera Sensitivity")]
    [SerializeField] private float xMouseSensitivity = 70f;
    [SerializeField] private float yMouseSensitivity = 50f;

    //Camera Rotation Values
    private float xRotation = 0;
    private float yRotation = 0;

    //Wall Run Data
    private PlayerController player = null;
    private float wallRunCameraTilt = 0f;
    private float maxWallRunCameraTilt = 30f;

    private void Awake()
    {
        player = GetComponent<PlayerController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * xMouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * yMouseSensitivity * Time.deltaTime;

        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        camera.transform.rotation = Quaternion.Euler(xRotation, yRotation, wallRunCameraTilt);
        orientation.transform.rotation = Quaternion.Euler(0f, yRotation, 0);

        CheckCameraTilt();
    }

    void CheckCameraTilt()
    {
        //If Wall Running
        if (Mathf.Abs(wallRunCameraTilt) < maxWallRunCameraTilt && player.IsWallRunning && player.IsWallRight)
            wallRunCameraTilt += 1f;
        if (Mathf.Abs(wallRunCameraTilt) < maxWallRunCameraTilt && player.IsWallRunning && player.IsWallLeft)
            wallRunCameraTilt -= 1f;
   
        //If wall Run stops
        if (wallRunCameraTilt > 0 && !player.IsWallLeft && !player.IsWallRight)
            wallRunCameraTilt -= 1f;
        if (wallRunCameraTilt < 0 && !player.IsWallRight && !player.IsWallLeft)
            wallRunCameraTilt += 1f;
    }
}
