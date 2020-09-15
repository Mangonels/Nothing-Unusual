using UnityEngine;
public class Dispenser : MonoBehaviour
{
    public ObjectsData objectsDataRef; //Reference to data from objects such as their names and display materials

    public ObjectsData.objectTypes objectToSpawn; //The game object the dispenser will spawn
    public GameObject gridFloorReference; //Used for referencing transform.y position for player drop calculations
    public GridBox gridBoxScriptRef; //GridBox script to which spawned objects will be registered

    //--------------
    // Debug
    //--------------
    //public bool dropSamples; //Forces object drop for debug purposes
    //public int maxDroppedSamples = 1;
    //private int currentDroppedSamples = 0;
    //private float timeToDrop = 2.0f; //When this threshold is exceeded, an objectToSpawn is dropped
    //private float timer = 2.0f; //When this value reaches "timeToDrop", an objectToSpawn is dropped, and "timer" is reset

    void Update()
    {



        //--------------
        // Debug
        //--------------
        //if (dropSamples)
        //{
        //    timer += Time.deltaTime;
        //    if (timer >= timeToDrop && currentDroppedSamples < maxDroppedSamples)
        //    {
        //        timer = 0.0f;
        //        Drop();
        //        currentDroppedSamples++;
        //    }
        //}
    }

    //Sets game object the dispenser will spawn
    public void SetSpawnObject(ObjectsData.objectTypes obj)
    {
        objectToSpawn = obj;
    }

    //Instantly drops object dispenser (or player drop, usually marked by adjusted = true) should spawn (set adjusted for dropping close to the top of highest object)
    public void Drop()
    {
        //Drop item normally from dispenser
        Instantiate(objectsDataRef.objectGameObjects_GridAligned[(int)objectToSpawn], transform.position, Quaternion.Euler(-90f, 0f, 0f));
        gridBoxScriptRef.IncreaseCurrentObjectAmmount();
    }
}