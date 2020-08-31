using UnityEngine;
public class Door : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Object")
        {
            //Remove object
            Destroy(col.gameObject);
        }
    }
}
