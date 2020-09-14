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
                    GameObject HeldObjectInstantiated = Instantiate(objectsDataRef.objectGameObjects_Held[(int)hit.collider.gameObject.GetComponent<GridAlignedObject>().objectType], 
                                                                           holdSlotPos, 
                                                                           Quaternion.Euler(-90f, 0f, 0f), 
                                                                           transform
                                                                           ); //Instance new substituting held object, we search for the right one in the objectsGameObjects_Held dictionary by using the objectType enum as position
                    heldObjects[amountOfHeldObjects] = HeldObjectInstantiated; //Attatch reference to new held object in heldObjects array
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
                                collidingGridBox.transform 
                                ); //Instance drop, aligned with grid's dispenser and adjusted on top of highest dispenser aligned object
                    collidingGridBox.GetComponentInChildren<GridBox>().IncreaseCurrentObjectAmmount(); //Notify grid box of extra spawned object
                }
            }
        }
    }

    public GameObject[] GetHeldObjectsArray() 
    {
        return heldObjects;
    }

    public void RemoveAndReorderHeldObjectsByAmount(int amount) //Removes held objects up to certain amount, down to up, and rearranges them pushing them downward. Used for ordered delivery objects substraction by door case
    {
        ObjectsData.objectTypes[] newHeldObjectDistribution = { ObjectsData.objectTypes.NONE, ObjectsData.objectTypes.NONE, ObjectsData.objectTypes.NONE, ObjectsData.objectTypes.NONE, ObjectsData.objectTypes.NONE };  //List that will contain new object types
        //Pick up current held objects above "amount" slots in heldObjects, and place them in order starting from lower "newHeldObjectDistribution" slots. Rest will be default NONE.
        for (int i = amount; i < 5; i++)
        {
            for (int j = 0; j < 5 - amount; j++) 
            {
                newHeldObjectDistribution[j] = heldObjects[i].GetComponent<HeldObject>().objectType;
            }
        }

        for (int i = 0; i < 5; i++) //Update held objects instances and referencing "heldObjects" array
        {
            Vector3 holdSlotPos = heldObjects[i].transform.position;
            Destroy(heldObjects[i]); //Remove game object to be replaced
            GameObject HeldObjectInstantiated = Instantiate(objectsDataRef.objectGameObjects_Held[(int)newHeldObjectDistribution[i]],
                                                                   holdSlotPos,
                                                                   Quaternion.Euler(-90f, 0f, 0f),
                                                                   transform
                                                                   ); //Instance new substituting held object, we search for the right one in the objectsGameObjects_Held dictionary by using the objectType enum as position
            heldObjects[i] = HeldObjectInstantiated; //New reference to the now moved heldObject at new held slot
        }

        amountOfHeldObjects -= amount;
    }
    public void RemoveAndReorderHeldObjects(ObjectsData.objectTypes[] objects) //Removes first encountered held objects according cohinciding with each objects for removal array slot, and rearranges them pushing them downward. Used for unordered delivery objects substraction by door case
    {
        ObjectsData.objectTypes[] newHeldObjectDistribution = { ObjectsData.objectTypes.NONE, ObjectsData.objectTypes.NONE, ObjectsData.objectTypes.NONE, ObjectsData.objectTypes.NONE, ObjectsData.objectTypes.NONE };  //List that will contain new object types
        bool[] deleteSlots = { false, false, false, false, false }; //Marks which slots to delete from heldObjects. Resulting slots would be used to construct "newHeldObjectDistribution"

        int objectRemovals = 0;
        for (int i = 0; i < 5; i++) //Iterate each "objects" to remove
        {
            if (objects[i] != ObjectsData.objectTypes.NONE) //object type to be removed is not NONE
            {
                for (int j = 0; j < 5; j++) //Compare with each iterated held object
                {
                    if (heldObjects[j].GetComponent<HeldObject>().objectType == objects[i])
                    {
                        deleteSlots[j] = true; //Remove, only uncohinciding objects will be preserved
                        objectRemovals++;
                        break;
                    }
                }
            }
            else break; //Break on first NONE object encounter (it should mean we reached the end of the objects for removal)
        }

        int coveredNewSlots = 0;
        for (int i = 0; i < 5; i++) //Construct "newHeldObjectDistribution" by pushing down to first slots of this new array, the slots that shouldn't be deleted, as marked by "deleteSlots"
        {
            if (!deleteSlots[i])
            {
                newHeldObjectDistribution[coveredNewSlots] = heldObjects[i].GetComponent<HeldObject>().objectType;
                coveredNewSlots++;
            }
        }

        for (int i = 0; i < 5; i++) //Update held objects instances and referencing "heldObjects" array
        {
            Vector3 holdSlotPos = heldObjects[i].transform.position;
            Destroy(heldObjects[i]); //Remove game object to be replaced
            GameObject HeldObjectInstantiated = Instantiate(objectsDataRef.objectGameObjects_Held[(int)newHeldObjectDistribution[i]],
                                                                   holdSlotPos,
                                                                   Quaternion.Euler(-90f, 0f, 0f),
                                                                   transform
                                                                   ); //Instance new substituting held object, we search for the right one in the objectsGameObjects_Held dictionary by using the objectType enum as position
            heldObjects[i] = HeldObjectInstantiated; //New reference to the now moved heldObject at new held slot
        }

        amountOfHeldObjects -= objectRemovals;
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