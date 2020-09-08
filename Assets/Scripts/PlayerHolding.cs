using Cinemachine;
using UnityEngine;

public class PlayerHolding : MonoBehaviour
{
    public ObjectsData objectsDataRef; //Reference to data from objects such as their names and display materials

    public GameObject[] heldObjects;
    public float maxGrabDistance = 3.0f;
    [SerializeField] private int amountOfHeldObjects = 0;
    public Transform GridBoxCollisionCheckPointRef; //Transform containing reference position from which we check if we're colliding with a "collidingGridBox" and update such reference.
    [SerializeField] Collider collidingGridBox; //The box grid's collider the detection box for held objects is colliding with by trigger
    public float standardObjectHeightAsReference = 1f; //The standard height of an object, used for calculating drop onto stack from player, altitudes
    public float adjustedDropHeight = 0.6f; //Slight extra height applied to player item dropping above the highest item on the GridBox stack

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
            //Pick up object mechanic
            RaycastHit hit;
            Ray ray = new Ray(cVCRef.transform.position, cVCRef.State.CorrectedOrientation * Vector3.forward);
            //Debug.DrawRay(cVCRef.transform.position, (cVCRef.State.CorrectedOrientation * Vector3.forward) * 100, Color.red, 2f);
            if (Physics.Raycast(ray, out hit, maxGrabDistance, LayerMask.GetMask("Pickable"))) //Raycast hits Pickable Object
            {
                if (amountOfHeldObjects < heldObjects.Length) //Can we hold any more objects?
                {
                    Vector3 holdSlotPos = heldObjects[amountOfHeldObjects].transform.position;
                    Destroy(heldObjects[amountOfHeldObjects]); //Remove old held game object
                    GameObject GridAlignedObjectInstantiated = Instantiate(objectsDataRef.objectGameObjects_Held[(int)hit.collider.gameObject.GetComponent<GridAlignedObject>().objectType], 
                                                                           holdSlotPos, 
                                                                           Quaternion.Euler(-90f, 0f, 0f), 
                                                                           transform
                                                                           ); //Instance new substituting held object, we search for the right one in the objectsGameObjects_Held dictionary by using the objectType enum as position
                    heldObjects[amountOfHeldObjects] = GridAlignedObjectInstantiated; //Attatch reference to new held object in heldObjects array
                    Destroy(hit.collider.gameObject); //Remove grid aligned object
                    amountOfHeldObjects++;
                }
            }
        }

        if (Input.GetMouseButtonDown(1)) //Right click
        {
            //Drop object mechanic
            if (amountOfHeldObjects > 0)
            {
                //Drop it in first current detected grid box from sphere collider
                Collider[] collidingGridBoxes = Physics.OverlapBox(GridBoxCollisionCheckPointRef.position, new Vector3(0.001f, 0.001f, 0.001f), Quaternion.identity, LayerMask.GetMask("GridBoxes"));
                if (collidingGridBoxes.Length > 0) //There was one or more grid boxes?
                {
                    //Replace held object to NONE:
                    amountOfHeldObjects--;
                    Vector3 heldSlotPos = heldObjects[amountOfHeldObjects].transform.position;
                    ObjectsData.objectTypes objectType = heldObjects[amountOfHeldObjects].GetComponent<HeldObject>().objectType; //Reference the type for dropping a few lines later
                    Destroy(heldObjects[amountOfHeldObjects]); //Remove no longer held game object
                    GameObject HeldObjectInstantiated = Instantiate(objectsDataRef.objectGameObjects_Held[0], heldSlotPos, Quaternion.Euler(-90f, 0f, 0f), transform); //Instance new substituting held object (NONE, in objectGameObjects_Held[0])
                    heldObjects[amountOfHeldObjects] = HeldObjectInstantiated; //Attatch reference to new held object in heldObjects array

                    //Drop held object as GridAligned object into the appropiate colliding GridBox, on top of the highest GridAligned object within:
                    collidingGridBox = collidingGridBoxes[0]; //Pick only the first grid box found found (should be only one anyway, but better safe than sorry)
                    //Debug.Log("Object Type attempted to drop: " + heldObjects[amountOfHeldObjects].gameObject.GetComponent<HeldObject>().objectType);
                    Instantiate(objectsDataRef.objectGameObjects_GridAligned[(int)objectType],
                                new Vector3(collidingGridBox.transform.position.x, collidingGridBox.GetComponentInChildren<Dispenser>().gridFloorReference.transform.position.y + (standardObjectHeightAsReference * collidingGridBox.gameObject.GetComponent<GridBox>().GetCurrentObjectsAmmount()) + adjustedDropHeight, collidingGridBox.transform.position.z), 
                                Quaternion.Euler(-90f, 0f, 0f),
                                collidingGridBox.transform); //Instance drop, aligned with grid's dispenser and adjusted on top of highest dispenser aligned object
                    collidingGridBox.GetComponentInChildren<GridBox>().IncreaseCurrentObjectAmmount(); //Notify grid box of extra spawned object
                }
            }
        }
    }

    public GameObject[] GetHeldObjectsArray() 
    {
        return heldObjects;
    }
    public void RemoveAllHeldObjects() //Sets all held objects to NONE (gives the appearance that we're holding nothing)
    {
        for (int i = 0; i < heldObjects.Length; i++)
        {
            Vector3 heldSlotPos = heldObjects[i].transform.position;
            GameObject objectInstantiated = Instantiate(objectsDataRef.objectGameObjects_Held[0], heldSlotPos, Quaternion.identity, transform); //Instance new substituting held object (NONE, Slot 0)
            Destroy(heldObjects[i]); //Delete old held Object
            heldObjects[i] = objectInstantiated; //New held Object reference to the new NONE object
            amountOfHeldObjects = 0;
        }
    }
}