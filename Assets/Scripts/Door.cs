using UnityEngine;
public class Door : MonoBehaviour
{
    public PlayerHolding holdInformationScriptRef; //References the script which contained held objects information
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
            //"Remove" held objects
            holdInformationScriptRef.RemoveAllHeldObjects();
        }
    }
}