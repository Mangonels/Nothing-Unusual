using UnityEngine;
public class Door : MonoBehaviour
{
    public PlayerHolding holdInformationScriptRef; //References the script which contained held objects information
    public ObjectsData objectsDataRef; //Reference to data from objects such as their names and display materials

    [SerializeField] private bool wantsObjects; //Is this door accepting objects?
    [SerializeField] private bool wantsObjectsInOrder; //Does the door want the objects to be presented in the same stack order?
    [SerializeField] private float willRemainOpenFor;
    [SerializeField] private float hasBeenOpenFor = 0.0f;
    [SerializeField] private ObjectsData.objectTypes[] wantedObjectTypes; //Current wanted objects by the door
    public GameObject[] requestDisplays;

    private Animator animator;

    void Start()
    {
        
    }

    void Update()
    {
        hasBeenOpenFor += Time.deltaTime;
        if (hasBeenOpenFor >= willRemainOpenFor) CloseDoor(); //Close the door if "willRemainOpenFor" has been exceeded
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            if (wantsObjects) 
            {
                //Validate brought objects
                bool allObjectsFound = true; //We asume true, until one of them isn't found
                GameObject[] heldObjectArray = col.gameObject.GetComponentInChildren<PlayerHolding>().GetHeldObjectsArray(); //Get the player's held object array

                if (!wantsObjectsInOrder) //No order for objects required
                {
                    //Check types one by one and compare with existing
                    for (int i = 0; i < wantedObjectTypes.Length; i++) //Each wanted object
                    {
                        if (allObjectsFound)
                        {
                            if (wantedObjectTypes[i] == ObjectsData.objectTypes.NONE) continue; //Next wanted object, there's no wanted object in this slot
                            for (int j = 0; i < heldObjectArray.Length; i++) //See if it's in any of the held objects array
                            {
                                if (wantedObjectTypes[i] == heldObjectArray[j].GetComponent<HeldObject>().objectType) break; //Object found, next item?
                                else if (j == heldObjectArray.Length) allObjectsFound = false; //We reached the end of "heldObjectArray" without finding this object, so all objects where not found
                            }
                        }
                        else break; //Some object wasn't found, hence all not found. Brought objects are invalid.
                    }
                }
                else //Ordered objects required
                {
                    //Check types one by one and compare with existing
                    for (int i = 0; i < wantedObjectTypes.Length; i++) //Each wanted object
                    {
                        if (wantedObjectTypes[i] != heldObjectArray[i].GetComponent<HeldObject>().objectType)
                        {
                            //We didn't find the same object in this slot. State it, and break loop
                            allObjectsFound = false;
                            break;
                        }
                    }
                }

                if (allObjectsFound) //All objects where found
                {
                    holdInformationScriptRef.RemoveAllHeldObjects();
                }
            }
        }
    }

    public void OpenDoor(ObjectsData.objectTypes[] objectTypes, float remainOpenFor, bool inOrder)
    {
        wantedObjectTypes = objectTypes;
        wantsObjects = true;
        wantsObjectsInOrder = inOrder;
        willRemainOpenFor = remainOpenFor;

        if (!inOrder) 
        {
            for (int i = 0; i < objectTypes.Length; i++)
            {
                requestDisplays[i].GetComponent<MeshRenderer>().material = objectsDataRef.objectMaterial_DoorDisplay_Unordered[(int)objectTypes[i]]; //Set unordered requests in displays
            }
        }
        else 
        {
            for (int i = 0; i < objectTypes.Length; i++)
            {
                requestDisplays[i].GetComponent<MeshRenderer>().material = objectsDataRef.objectMaterial_DoorDisplay_Ordered[(int)objectTypes[i]]; //Set ordered requests in displays
            }
        }

        gameObject.GetComponentInChildren<Animator>().SetBool("DoorOpen", true); //Animate door opening
    }

    public void CloseDoor()
    {
        wantsObjects = false;
        hasBeenOpenFor = 0f;

        for (int i = 0; i < requestDisplays.Length; i++)
        {
            requestDisplays[i].GetComponent<MeshRenderer>().material = objectsDataRef.objectMaterial_DoorDisplay_Unordered[0]; //Set displays to none (no requests)
        }

        gameObject.GetComponentInChildren<Animator>().SetBool("DoorOpen", false); //Animate door closing
    }
}