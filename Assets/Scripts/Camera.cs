using Cinemachine;
using UnityEngine;

public class Camera : MonoBehaviour
{
    public CinemachineVirtualCamera cVCRef; //Cinemachine virtual camera reference
    [SerializeField] private float cameraFov = 40f;
    [SerializeField] private float maxFov = 120f;
    [SerializeField] private float minFov = 20f;
    void Start()
    {
        
    }

    void Update()
    {
        //Fov adjuster with mouse scroll
        cameraFov += Input.mouseScrollDelta.y;
        if (cameraFov < minFov) cameraFov = minFov;
        else if (cameraFov > maxFov) cameraFov = maxFov;
        
        cVCRef.m_Lens.FieldOfView = cameraFov;
    }
}
