using UnityEngine;
public class Door : MonoBehaviour
{
    public PlayerHolding holdInformationScriptRef; //References the script which contained held objects information
    public ObjectsData objectsDataRef; //Reference to data from objects such as their names and display materials

    [SerializeField] private bool wantsObjects; //Is this door accepting objects?
    [SerializeField] private bool wantsThemInOrder; //Does the door want the objects to be presented in the same stack order?
    [SerializeField] private ObjectsData.objectTypes[] wantedObjectTypes; //Current wanted objects by the door
    public GameObject[] requestDisplays;
    
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            if (wantsObjects) 
            {
                //Validate brought objects

                bool allObjectsFound = true; //We asume true, until one of them isn't found
                GameObject[] heldObjectArray = col.gameObject.GetComponent<PlayerHolding>().GetHeldObjectsArray(); //Get the player's held object array

                if (!wantsThemInOrder) //No order for objects required
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

    public void OpenDoorWithWantedObjectTypesAndOrder(ObjectsData.objectTypes[] objectTypes, bool inOrder)
    {
        wantedObjectTypes = objectTypes;
        wantsObjects = true;
        wantsThemInOrder = inOrder;
    }

    public void CloseDoor()
    {
        wantsObjects = false;
    }
}