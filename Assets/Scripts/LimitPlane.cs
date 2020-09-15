using UnityEngine;

public class LimitPlane : MonoBehaviour
{
    public Transform playerTransformRef;
    public Color color;
    [SerializeField] private float alphaValue;
    void Update()
    {
        //float height = playerTransformRef.position.y;
        //alphaValue = (Mathf.Lerp(100f, 0f, playerTransformRef.position.y)) * 255;

        //color.a = alphaValue; //playerTransformRef.position.y;
        //gameObject.GetComponent<MeshRenderer>().material.SetColor("_Color", color);
    }
}
