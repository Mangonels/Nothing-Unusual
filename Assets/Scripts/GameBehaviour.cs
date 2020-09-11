using System.Collections.Generic;
using UnityEngine;

public class GameBehaviour : MonoBehaviour
{
    public ObjectsData objectsDataRef; //Reference to data from objects such as their names and display materials

//    [SerializeField] private float totalTimer = 0.0f;
    [SerializeField] private float phaseOfDropSpeedTimer = 0.0f;
    [SerializeField] private float phaseOfDoorRequestsTimer = 0.0f;
    [SerializeField] private float newDropTimer = 0.0f;
    [SerializeField] private float newDoorTimer = 0.0f;

    public int percentileChanceOfDroppingWhenItsTime = 85;
    public int percentileChanceOfDoorOpeningWhenItsTime = 65;

//    [SerializeField] private int speedPhase = 0;
    [SerializeField] private int doorRequestsPhase = 0;
    [SerializeField] private float phaseOfSpeedTimerDurationThreshold = 300f; //Threshold after which drop timeToDropThreshold is reduced
    [SerializeField] private float phaseOfDoorRequestsTimerDurationThreshold = 150f; //Threshold after which drop timeToDropThreshold is reduced
    [SerializeField] private float timeToDropThreshold = 22f;
    [SerializeField] private float timeToDropThresholdReducer = 0.8f;
    [SerializeField] private float timeToOpenDoorThreshold = 90f;

    [SerializeField] private float maxTimeDoorsOpen = 80f;
    [SerializeField] private float minTimeDoorsOpen = 70f;
    [SerializeField] private int maxAmountOfObjectsRequestedPerDoor = 1;
    [SerializeField] private int minAmountOfObjectsRequestedPerDoor = 1;
    [SerializeField] private int objectsOrderMode = 0; //0 unordered only, 1, ordered and unordered, 2 ordered only

    [SerializeField] private List<ObjectsData.objectTypes> doorAccountableSpawnedObjects = null;

    public GameObject firstDispenser;
    public ObjectsData.objectTypes firstObject;
    public GameObject firstDoor;

    void Start()
    {
        Cursor.visible = false;

        //Drop first item in first dispenser
        firstDispenser.GetComponent<Dispenser>().SetSpawnObject(firstObject);
        firstDispenser.GetComponent<Dispenser>().Drop();

        //Open first door with first required objects
        ObjectsData.objectTypes[] objectTypes = { ObjectsData.objectTypes.TETRAS, ObjectsData.objectTypes.NONE, ObjectsData.objectTypes.NONE, ObjectsData.objectTypes.NONE, ObjectsData.objectTypes.NONE };
        firstDoor.GetComponent<Door>().OpenDoor(objectTypes, 30f, false);
    }

    void Update()
    {
//        totalTimer += Time.deltaTime;
        phaseOfDropSpeedTimer += Time.deltaTime;
        phaseOfDoorRequestsTimer += Time.deltaTime;
        newDropTimer += Time.deltaTime;
        newDoorTimer += Time.deltaTime;

        if (phaseOfDropSpeedTimer >= phaseOfSpeedTimerDurationThreshold)
        {
            phaseOfDropSpeedTimer = 0.0f; //Reset timer
            timeToDropThreshold *= timeToDropThresholdReducer; //Reduce time in between drops (difficulty increase)
//            speedPhase++; //Next speed phase
        }

        if (phaseOfDoorRequestsTimer >= phaseOfDoorRequestsTimerDurationThreshold) //Incrementing "objectsPhase" affects door progressions such as: max/minAmountOfObjectsRequestedPerDoor, max/minTimeDoorsOpen & objectsOrderMode
        {
            phaseOfDoorRequestsTimer = 0.0f;

            if (doorRequestsPhase == 0) phaseOfDoorRequestsTimerDurationThreshold = 300f; //Phases after the first will last 5 min instead of 2.5
            doorRequestsPhase++; //Next objects phase
            if (doorRequestsPhase == 2) timeToOpenDoorThreshold = 45f;
        }

        if (newDropTimer >= timeToDropThreshold) //It's time to drop an item on a random tile
        {
            newDropTimer = 0.0f;

            if (Random.Range(1, 100) < percentileChanceOfDroppingWhenItsTime) //Reduced chance of dropping depending on "percentileChanceOfDroppingWhenItsTime"?
            {
                int randomDispenser = Random.Range(0, objectsDataRef.dispenserGrid.Length); //Choose a random dispenser by Grid Slot number
                ObjectsData.objectTypes spawnedObject = (ObjectsData.objectTypes)Random.Range(1, objectsDataRef.objectGameObjects_GridAligned.Length);
                objectsDataRef.dispenserGrid[randomDispenser].GetComponentInChildren<Dispenser>().SetSpawnObject(spawnedObject); //Assign random object (NONE not included) to spawn in randomly selected dispenser
                objectsDataRef.dispenserGrid[randomDispenser].GetComponentInChildren<Dispenser>().Drop(); //Tell randomly selected dispenser to drop randomly selected and assigned item
                doorAccountableSpawnedObjects.Add(spawnedObject);
            }
        }
        if (newDoorTimer >= timeToOpenDoorThreshold) //It's time to open another door
        {
            newDoorTimer = 0.0f;
            if (Random.Range(1, 100) < percentileChanceOfDoorOpeningWhenItsTime) //Reduced chance of door opening depending on "percentileChanceOfDoorOpeningWhenItsTime"? [IMPORTANT] Already open doors oclude doors from opening (if they are still open)
            {
                int randomDoor = Random.Range(0, objectsDataRef.doors.Length); //Choose a random door by doors slot number
                if (!objectsDataRef.doors[randomDoor].GetComponent<Door>().wantsObjects) //Obviously, only open the door if it isn't already open
                {
                    //----------------------
                    //New door configuration
                    //----------------------

                    //Fill up 5 object types that will be requested by door
                    ObjectsData.objectTypes[] objectTypes = { ObjectsData.objectTypes.NONE, ObjectsData.objectTypes.NONE, ObjectsData.objectTypes.NONE, ObjectsData.objectTypes.NONE, ObjectsData.objectTypes.NONE }; //Array which will contain all required objects by name for the door, for reference
                    int slotAmountThisTime = Random.Range(minAmountOfObjectsRequestedPerDoor, maxAmountOfObjectsRequestedPerDoor); //Amount of required slots starting from 0 the door will have requirements for
                    int slotsFilled = 0;
                    for (int i = 0; i < slotAmountThisTime; i++) 
                    {
                        slotsFilled++;
                        objectTypes[i] = doorAccountableSpawnedObjects[Random.Range(0, doorAccountableSpawnedObjects.Count)];
                    }
                    for (int i = slotsFilled; i < 5; i--)
                    {
                        objectTypes[i] = ObjectsData.objectTypes.NONE;
                    }
                    
                    //Define a duration for the door to remain open
                    float randomDoorDuration = Random.Range(minTimeDoorsOpen, maxTimeDoorsOpen);
                   
                    //Define if objects will be asked for in order or not
                    bool inOrder = false;
                    if (objectsOrderMode == 0) { }
                    else if (objectsOrderMode == 1)
                    {
                        if (Random.Range(0, 1) == 0) inOrder = true;
                        else inOrder = false;
                    }
                    else if (objectsOrderMode == 2) { inOrder = true; }

                    //Finally open the door with final input, which is usually going to be dependant on "doorRequestsPhase"
                    objectsDataRef.doors[randomDoor].GetComponent<Door>().OpenDoor(objectTypes, randomDoorDuration, inOrder); 
                }
            }
        }
    }
}