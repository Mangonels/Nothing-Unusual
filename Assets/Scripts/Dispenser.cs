using UnityEngine;
public class Dispenser : MonoBehaviour
{
    public GameObject objectToSpawn; //The game object the dispenser will spawn
    public GameObject gridFloorReference; //Used for referencing transform.y position for player drop calculations
    public float standardObjectHeightAsReference = 1f; //The standard height of an object, used for calculating drop onto stack from player, altitudes
    public float adjustedDropExtraHeight = 0.6f; //Slight extra height applied to player item dropping above the highest item on the GridBox stack
    public GridBox gridBoxScriptRef; //GridBox script to which spawned objects will be registered

    //--------------
    // Debug
    //--------------
    public bool dropSamples; //Forces object drop for debug purposes
    public int maxDroppedSamples = 1;
    private int currentDroppedSamples = 0;
    private float timeToDrop = 2.0f; //When this threshold is exceeded, an objectToSpawn is dropped
    private float timer = 2.0f; //When this value reaches "timeToDrop", an objectToSpawn is dropped, and "timer" is reset

    void Start()
    {

    }

    void Update()
    {



        //--------------
        // Debug
        //--------------
        if (dropSamples) 
        {
            timer += Time.deltaTime;
            if (timer >= timeToDrop && currentDroppedSamples < maxDroppedSamples) 
            {
                timer = 0.0f;
                Drop(false);
                currentDroppedSamples++;
            }
        }
    }

    //Sets game object the dispenser will spawn
    public void SetSpawnObject(GameObject obj)
    {
        objectToSpawn = obj;
    }

    //Instantly drops object dispenser (or player drop, usually marked by adjusted = true) should spawn (set adjusted for dropping close to the top of highest object)
    public void Drop(bool adjusted)
    {
        if (!adjusted)
        {
            Instantiate(objectToSpawn, transform.position, Quaternion.Euler(0, 0, 0));
            gridBoxScriptRef.IncreaseCurrentItemAmmount();
        }
        else 
        {
            Instantiate(objectToSpawn, new Vector3(transform.position.x, gridFloorReference.transform.position.y + (standardObjectHeightAsReference * gridBoxScriptRef.GetCurrentObjectsAmmount()) + adjustedDropExtraHeight, transform.position.z), Quaternion.Euler(0, 0, 0));
            gridBoxScriptRef.IncreaseCurrentItemAmmount();
        }
    }
}