using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbilitiesScript : MonoBehaviour
{
    [SerializeField] private Transform cam = null;
    [SerializeField] private Transform ropePosition = null;
    [SerializeField] private Transform gunPosition = null;
    private GameObject Collectable = null;
    private LineRenderer grappleLR;
    private Rigidbody playerRb = null;

    private Vector3 grapplePoint = Vector3.zero;
    private SpringJoint joint = null;
    private float grappleDistance = 50f;
    private float speed = 2f;
    private float step = 0f;
    
    private bool isSwinging = false;
    private bool isPulling = false;
    private bool isMoving = false;
    /*
    //Rewind Time
    private List<RewindTime> rewindTimeList;
    private bool isRewinding = false;
    private float rewindLength = 3f;
    */
    //Rope Target Check
    private GameObject ropeTarget = null;
    private List<GameObject> grappleObjectsList;
    private bool[] targetVisibility;
    //private GameObject swingObject = null;
    //private GameObject pullObject = null;

    // Start is called before the first frame update
    void Start()
    {
        grappleLR = GetComponent<LineRenderer>();
        playerRb = GetComponent<Rigidbody>();
        /*
        //Starting Rewind List
        rewindTimeList = new List<RewindTime>();
        */
        //-----------------------Calculating all grapple objects-----------------------
        GameObject[] swingObjects = GameObject.FindGameObjectsWithTag("SwingObject");
        GameObject[] pullObjects = GameObject.FindGameObjectsWithTag("PullObject");

        grappleObjectsList = new List<GameObject>();

        for (int i = 0; i < swingObjects.Length; i++)
        {
            grappleObjectsList.Add(swingObjects[i]);
        }
        for (int i = 0; i < pullObjects.Length; i++)
        {
            grappleObjectsList.Add(pullObjects[i]);
        }

        targetVisibility = new bool[grappleObjectsList.Count];
    }

    // Update is called once per frame
    void Update()
    {
        CheckGrapple();

        //--------------------Rope Input---------------------------

        if(Input.GetKeyDown(KeyCode.Mouse1))
        {
            StartGrapple();
        }
        if(Input.GetKeyUp(KeyCode.Mouse1))
        {
            StopGrappple();
        }

        if (isPulling)
        {
            float step = speed * Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, grapplePoint, step);
            if (Vector3.Distance(transform.position, grapplePoint) < 1f)
            {
                isPulling = false;
                playerRb.isKinematic = false;
            }
        }
        if(isMoving)
        {
            float step = speed * Time.deltaTime;
            Collectable.transform.position = Vector3.Lerp(Collectable.transform.position, transform.position, step);

            if (Vector3.Distance(Collectable.transform.position, transform.position) < 0.1f)
            {
                isMoving = false;
                Collectable = null;
            }
        }
        //-----------------------------------------------------------------------

        /*
        //-----------------------------Rewind Ability----------------------------
        if (Input.GetKeyDown(KeyCode.E))
        {
            StartRewind();
        }
        //-----------------------------------------------------------------------
        */
    }

    private void FixedUpdate()
    {
        /*
        //---------------------Rewinding Time------------------------------
        if (isRewinding)
        {
            if (rewindTimeList.Count > 0)
            {
                transform.position = rewindTimeList[0].Position;
                cam.rotation = rewindTimeList[0].Rotation;
                rewindTimeList.RemoveAt(0);
            }
            else
            {
                StopRewind();
            }
        }
        else
        {
            if (rewindTimeList.Count > Mathf.Round(rewindLength / Time.fixedDeltaTime))         //Fixed Update independant time count
            {
                rewindTimeList.RemoveAt(rewindTimeList.Count - 1);
            }

            rewindTimeList.Insert(0, new RewindTime(transform.position, cam.rotation));
        }
        //--------------------------------------------------------------------
        */
    }

    private void LateUpdate()
    {

        if (isSwinging)
        {
            DrawRope(ropePosition.position, grapplePoint);
        }

        if(isPulling)
        {
            DrawRope(ropePosition.position, grapplePoint);

        }

        if (isMoving)
        {
            step += 1f * Time.deltaTime;

            Vector3 distance = Vector3.Lerp(grapplePoint, ropePosition.position, step);
            DrawRope(ropePosition.position, distance);
        }

    }
/*
    #region Rewind Ability
    void StartRewind()
    {
        isRewinding = true;
        playerRb.isKinematic = true;
    }

    void StopRewind()
    {
        isRewinding = false;
        playerRb.isKinematic = false;
    }

    #endregion
    */

    #region Rope Ability

/*
    void CheckGrapple()
    {
        swingObject = GameObject.FindGameObjectWithTag("SwingObject");
        pullObject = GameObject.FindGameObjectWithTag("PullObject");

        //-------------------------Checking Visibility-------------------------------------
        bool swingObjectVisibility = false, pullObjectVisibility = false;
        if (swingObject != null)
             swingObjectVisibility = swingObject.GetComponent<Renderer>().isVisible;

        if (pullObject != null)
             pullObjectVisibility = pullObject.GetComponent<Renderer>().isVisible;

        //-------------------------Checking Type of Object--------------------------------
        //If no object is present
        if (swingObject == null && pullObject == null)
            return;

        //If both Objects are present
        if (swingObject != null && pullObject != null)
        {
            //If objects are not visible
            if (!swingObjectVisibility && !pullObjectVisibility)
            {
                return;
            }

            // If they are visible------------Calculating nearest Object
            if (Vector3.Distance(transform.position, swingObject.transform.position) < Vector3.Distance(transform.position, pullObject.transform.position))
            {
                ropeTarget = swingObject;
            }
            else
            {
                ropeTarget = pullObject;
            }
        }
        // If Swing object is present and pullobject is not
        else if (swingObject != null && pullObject == null)
        {
            if (swingObjectVisibility)
                ropeTarget = swingObject;
        }
        // If Pull object is present and swing object is not
        else if (swingObject == null && pullObject != null)
        {
            if(pullObjectVisibility)
                ropeTarget = pullObject;
        }
        //Return if no object is close
        if (ropeTarget == null)
            return;

        //---------------------------Checking if the object is in grapple Range------------------------------------
        Vector3 screenPoint = Camera.main.WorldToViewportPoint(ropeTarget.transform.position);
        if (screenPoint.z > 0 && screenPoint.z < 20 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1)
        {
            Debug.Log(ropeTarget.transform.name);
            Debug.Log("CAN GRAPPLE");
        }
        else
        {
            ropeTarget = null;
        }
    }
    */

    void CheckGrapple()
    {
        for (int i = 0; i < grappleObjectsList.Count; i++)
        {
            targetVisibility[i] = grappleObjectsList[i].GetComponent<Renderer>().isVisible;

            if (targetVisibility[i])
            {
                if (Vector3.Distance(transform.position, grappleObjectsList[i].transform.position) < 20)
                {
                    ropeTarget = grappleObjectsList[i];
                }
            }
        }
    }

    void StartGrapple()
    {
        RaycastHit hit;
        if (Physics.Raycast(cam.position, cam.forward, out hit, grappleDistance))
        {
            grapplePoint = hit.point;
            grappleLR.positionCount = 2;

            if (hit.transform.CompareTag("Moveable"))
            {
                isMoving = true;
                Collectable = hit.transform.gameObject;

                //Setting Moveable Rope Color
                grappleLR.startColor = Color.green;
                grappleLR.endColor = Color.green;
            }
        }
        if (ropeTarget == null)
            return;

        Vector3 screenPoint = Camera.main.WorldToViewportPoint(ropeTarget.transform.position);
        if (screenPoint.z > 0 && screenPoint.z < 20 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1)
        {
            Debug.Log("CAN GRAPPLE");



            grapplePoint = ropeTarget.transform.position;
            grappleLR.positionCount = 2;

            if (ropeTarget.transform.CompareTag("PullObject"))
            {
                isPulling = true;
                playerRb.isKinematic = true;

                //Setting Pull Rope Color
                grappleLR.startColor = Color.red;
                grappleLR.endColor = Color.red;
            }

            //if (ropeTarget.transform.CompareTag("Moveable"))
            //{
            //    isMoving = true;
            //    Collectable = ropeTarget.transform.gameObject;

            //    //Setting Moveable Rope Color
            //    grappleLR.startColor = Color.green;
            //    grappleLR.endColor = Color.green;
            //}

            if (ropeTarget.transform.CompareTag("SwingObject"))
            {
                isSwinging = true;

                //Setting Swinging Rope Color
                grappleLR.startColor = Color.blue;
                grappleLR.endColor = Color.blue;

                //Resetting Jumps Left
                PlayerController.Instance.JumpsLeft = 2;

                //Swing Code
                joint = this.gameObject.AddComponent<SpringJoint>();
                joint.autoConfigureConnectedAnchor = false;
                joint.connectedAnchor = grapplePoint;
                joint.anchor = Vector3.zero;

                float distanceFromPoint = Vector3.Distance(transform.position, grapplePoint);

                joint.maxDistance = distanceFromPoint * 0.5f;
                joint.minDistance = distanceFromPoint * 0.2f;

                joint.spring = 15f;
                joint.damper = 10f;
                joint.massScale = 4.5f;

            }
            //  RaycastHit hit;
            //if (Physics.Raycast(cam.position, cam.forward, out hit, grappleDistance))  
            //{
            //    grapplePoint = hit.point;
            //    grappleLR.positionCount = 2;

            //    if (hit.transform.CompareTag("PullObject"))
            //    {
            //        isPulling = true;
            //        playerRb.isKinematic = true;

            //        //Setting Pull Rope Color
            //        grappleLR.startColor = Color.red;
            //        grappleLR.endColor = Color.red;
            //    }

            //    if (hit.transform.CompareTag("Moveable"))
            //    {
            //        isMoving = true;
            //        Collectable = hit.transform.gameObject;

            //        //Setting Moveable Rope Color
            //        grappleLR.startColor = Color.green;
            //        grappleLR.endColor = Color.green;
            //    }

            //    if (hit.transform.CompareTag("SwingObject"))
            //    {
            //        isSwinging = true;

            //        //Setting Swinging Rope Color
            //        grappleLR.startColor = Color.blue;
            //        grappleLR.endColor = Color.blue;

            //        //Resetting Jumps Left
            //        PlayerController.Instance.JumpsLeft = 2;

            //        //Swing Code
            //        joint = this.gameObject.AddComponent<SpringJoint>();
            //        joint.autoConfigureConnectedAnchor = false;
            //        joint.connectedAnchor = grapplePoint;

            //        float distanceFromPoint = Vector3.Distance(transform.position, grapplePoint);

            //        joint.maxDistance = distanceFromPoint * 0.5f;
            //        joint.minDistance = distanceFromPoint * 0.2f;

            //        joint.spring = 15f;
            //        joint.damper = 10f;
            //        joint.massScale = 4.5f;

            //    }
            //}
        }
        else
        {
            return;
        }
    }

    void StopGrappple()
    {
        grappleLR.positionCount = 0;

        if (isSwinging)
        {
            isSwinging = false;
            Destroy(joint);
        }
        if (isMoving)
        {
            isMoving = false;
            step = 0f;
        }
        if (isPulling)
        { 
            isPulling = false;
            playerRb.isKinematic = false;
        }
    }

    void DrawRope(Vector3 startPoint,Vector3 endPoint)
    {
        //if (!joint) return;

        grappleLR.SetPosition(0, startPoint);
        grappleLR.SetPosition(1, endPoint);
    }

    #endregion

    //private void OnCollisionEnter(Collision collision)
    //{
    //    if(collision.transform.CompareTag("Moveable"))
    //    {
    //        collision.transform.SetParent(gunPosition);
    //        collision.transform.localPosition = Vector3.zero;
    //        collision.transform.rotation = Quaternion.identity;
    //    }
    //}

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.transform.CompareTag("Moveable"))
    //    {
    //        other.transform.SetParent(gunPosition);
    //        other.transform.localPosition = Vector3.zero;
    //        Debug.Log(other.transform.rotation);
    //        other.transform.rotation = Quaternion.Euler(0f, -90f, 0f);
    //        Debug.Log("LKASDJ");
    //    }
    //}
}