using Cinemachine;
using UnityEngine;

public class Camera : MonoBehaviour
{
    public CinemachineVirtualCamera cVCRef; //Cinemachine virtual camera reference
    [SerializeField] private float cameraFov = 40f;
    [SerializeField] private float maxFov = 120f;
    [SerializeField] private float minFov = 20f;

    [SerializeField] private float cameraSpeed = 200f;
    [SerializeField] private float maxCameraSpeed = 800f;
    [SerializeField] private float minCameraSpeed = 50f;

    void Update()
    {
        //Fov adjuster with mouse scroll
        if (!Input.GetKey(KeyCode.LeftControl) && !Input.GetKey(KeyCode.RightControl)) //Mouse scroll only
        {
            cameraSpeed += (Input.mouseScrollDelta.y * 8);

            if (cameraSpeed < minCameraSpeed) cameraSpeed = minCameraSpeed; //Min/Max clamps
            else if (cameraSpeed > maxCameraSpeed) cameraSpeed = maxCameraSpeed;
            //Camera speed adjuster
            cVCRef.GetCinemachineComponent<CinemachinePOV>().m_VerticalAxis.m_MaxSpeed = cameraSpeed;
            cVCRef.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.m_MaxSpeed = cameraSpeed;
        }
        else //Control + mouse scroll
        {
            cameraFov += Input.mouseScrollDelta.y;

            if (cameraFov < minFov) cameraFov = minFov; //Min/Max clamps
            else if (cameraFov > maxFov) cameraFov = maxFov;

            cVCRef.m_Lens.FieldOfView = cameraFov;
        }
    }
}
