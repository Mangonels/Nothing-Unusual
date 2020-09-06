using UnityEngine;

public class GridAlignedObject : MonoBehaviour
{
    [SerializeField] Collider collidingGridBox; //The box grid's collider the detection box for held objects is colliding with by trigger

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    void OnDestroy() //This is called when the attatched game object is destroyed, but it's also called when scene changes, and when the game is shut down. Implementation ay be ineficient in last 2 cases.        <!>
    {
        Collider[] collidingGridBoxes = Physics.OverlapBox(transform.position, new Vector3(0.001f, 0.001f, 0.001f), Quaternion.identity, LayerMask.GetMask("GridBoxes"));
        if (collidingGridBoxes.Length > 0) //There was one or more grid boxes?
        {
            collidingGridBox = collidingGridBoxes[0]; //Pick only the first one found (should be only one anyway, but better safe than sorry)
            collidingGridBox.gameObject.GetComponent<GridBox>().DecreaseCurrentObjectAmmount(); //Notify grid box of despawning so it can reduce it's internal "currentObjectAmmount" accordingly
        }
    }
}
