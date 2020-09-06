using Cinemachine;
using UnityEngine;

public class PlayerHolding : MonoBehaviour
{
    public GameObject[] heldObjects;
    public float maxGrabDistance = 3.0f;
    [SerializeField] private int amountOfHeldObjects = 0;
    public Transform GridBoxCollisionCheckPointRef; //Transform containing reference position from which we check if we're colliding with a "collidingGridBox" and update such reference.
    [SerializeField] Collider collidingGridBox; //The box grid's collider the detection box for held objects is colliding with by trigger

    public CinemachineVirtualCamera cVCRef; //Cinemachine virtual camera reference
    void Start()
    {

    }

    void Update()
    {
        //Rotate held objects with camera's x & y axis extracted euler angles
        transform.rotation = Quaternion.Euler(0.0f, cVCRef.State.CorrectedOrientation.eulerAngles.y, 0.0f);

        if (Input.GetMouseButtonDown(0)) //Left click
        {
            //Pick up object
            RaycastHit hit;
            Ray ray = new Ray(cVCRef.transform.position, cVCRef.State.CorrectedOrientation * Vector3.forward);
            //Debug.DrawRay(cVCRef.transform.position, (cVCRef.State.CorrectedOrientation * Vector3.forward) * 100, Color.red, 2f);
            if (Physics.Raycast(ray, out hit)) //Raycast hits game object
            {
                if (hit.distance <= maxGrabDistance) //Hit detection is within "maxGrabDistance"
                {
                    if (hit.collider.gameObject.tag == "PickableObject") //Game object is a PickableObject
                    {
                        if (amountOfHeldObjects < heldObjects.Length)
                        {
                            Destroy(hit.collider.gameObject);
                            heldObjects[amountOfHeldObjects].SetActive(true);
                            amountOfHeldObjects++;
                        }
                    }
                }
            }
        }

        if (Input.GetMouseButtonDown(1)) //Right click
        {
            //Drop object
            if (amountOfHeldObjects > 0)
            {
                //Drop it in first current detected grid box from sphere collider
                Collider[] collidingGridBoxes = Physics.OverlapBox(GridBoxCollisionCheckPointRef.position, new Vector3(0.001f, 0.001f, 0.001f), Quaternion.identity, LayerMask.GetMask("GridBoxes"));
                if (collidingGridBoxes.Length > 0) //There was one or more grid boxes?
                {
                    amountOfHeldObjects--;
                    heldObjects[amountOfHeldObjects].SetActive(false); //Hide it from held view
                    collidingGridBox = collidingGridBoxes[0]; //Pick only the first one found (should be only one anyway, but better safe than sorry)
                    //Previously set the GameObject to be dropped                                                                  ( TO-DO )
                    collidingGridBox.gameObject.GetComponentInChildren<Dispenser>().Drop(true);
                }
            }
        }
    }

    public GameObject[] GetHeldObjectsArray() 
    {
        return heldObjects;
    }
    public void RemoveAllHeldObjects() //Sets all held objects to inactive (gives the appearance that we're holding nothing)
    {
        for (int i = 0; i < heldObjects.Length; i++)
        {
            heldObjects[i].SetActive(false);
            amountOfHeldObjects = 0;
        }
    }
}