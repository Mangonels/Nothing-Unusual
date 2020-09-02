using UnityEngine;
public class Dispenser : MonoBehaviour
{
    public GameObject objectToSpawn; //The game object the dispenser will spawn



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
                Drop();
                currentDroppedSamples++;
            }
        }
    }

    //Sets game object the dispenser will spawn
    public void SetSpawnObject(GameObject obj)
    {
        objectToSpawn = obj;
    }

    //Instantly drops object dispenser should spawn
    public void Drop()
    {
        Instantiate(objectToSpawn, transform.position, Quaternion.Euler(0, 0, 0));
    }
}