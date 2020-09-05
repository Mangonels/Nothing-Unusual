using UnityEngine;

public class GridBox : MonoBehaviour
{
    public int maxItems = 5;
    [SerializeField] private int currentItemAmmount = 0;

    void Start()
    {
        
    }

    void Update()
    {

    }

    public void IncreaseCurrentItemAmmount() 
    { 
        currentItemAmmount++;
        CapacityCheck();
    }
    public void DecreaseCurrentItemAmmount()
    {
        currentItemAmmount--;
        CapacityCheck();
    }

    private void CapacityCheck() 
    {
        if (currentItemAmmount > maxItems) //Box capacity exceeded
        {
            //GAME OVER

        }
    }
}