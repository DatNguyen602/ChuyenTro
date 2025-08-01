using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class VirtualCameraManager : MonoBehaviour
{
    public static VirtualCameraManager instance { get; private set; }

    [SerializeField] private List<CinemachineCamera> cameras;
    [SerializeField] private int defaultPriority = 10;
    [SerializeField] private int activePriority = 100;

    private CinemachineCamera currentCam;

    void Awake()
    {
        // Singleton
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        // Reset all cameras
        foreach (var cam in cameras)
        {
            cam.Priority = defaultPriority;
        }

        if (cameras.Count > 0)
        {
            SetActiveCamera(cameras[0].name);
        }
    }

    /// <summary>
    /// Kích hoạt camera theo tên
    /// </summary>
    public void SetActiveCamera(string cameraName)
    {
        foreach (var cam in cameras)
        {
            if (cam.name == cameraName)
            {
                cam.Priority = activePriority;
                currentCam = cam;
            }
            else
            {
                cam.Priority = defaultPriority;
            }
        }
    }

    /// <summary>
    /// Kích hoạt camera bằng tham chiếu trực tiếp
    /// </summary>
    public void SetActiveCamera(CinemachineCamera camToActivate)
    {
        foreach (var cam in cameras)
        {
            cam.Priority = (cam == camToActivate) ? activePriority : defaultPriority;
        }

        currentCam = camToActivate;
    }

    /// <summary>
    /// Truy cập camera đang hoạt động
    /// </summary>
    public CinemachineCamera GetActiveCamera()
    {
        return currentCam;
    }
}
