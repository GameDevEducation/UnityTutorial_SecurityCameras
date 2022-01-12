using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SecurityConsole : MonoBehaviour
{
    [SerializeField] List<SecurityCamera> LinkedCameras;
    [SerializeField] RawImage CameraImage;
    [SerializeField] TextMeshProUGUI ActiveCameraLabel;

    [SerializeField] bool AutoswitchEnable = false;
    [SerializeField] float AutoswitchStartTime = 10f;
    [SerializeField] float AutoswitchInterval = 15f;

    float TimeUntilNextAutoswitch = -1f;

    public int ActiveCameraIndex { get; private set; } = -1;
    public SecurityCamera ActiveCamera => ActiveCameraIndex < 0 ? null : LinkedCameras[ActiveCameraIndex];

    // Start is called before the first frame update
    void Start()
    {
        ActiveCameraLabel.text = "Camera: None";
    }

    // Update is called once per frame
    void Update()
    {
        // is autoswitch active
        if (AutoswitchEnable)
        {
            TimeUntilNextAutoswitch -= Time.deltaTime;

            // time to switch?
            if (TimeUntilNextAutoswitch < 0)
            {
                TimeUntilNextAutoswitch = AutoswitchInterval;
                SelectNextCamera();
            }
        }
    }

    public void OnClicked()
    {
        SelectNextCamera();

        if (AutoswitchEnable)
            TimeUntilNextAutoswitch = AutoswitchStartTime;
    }

    void SelectNextCamera()
    {
        var previousCamera = ActiveCamera;

        // switch to the next camera
        ActiveCameraIndex = (ActiveCameraIndex + 1) % LinkedCameras.Count;

        // tell the previous camera to stop watching
        if (previousCamera != null)
            previousCamera.StopWatching(this);

        // tell the new camera to start watching
        ActiveCamera.StartWatching(this);
        ActiveCameraLabel.text = $"Camera: {ActiveCamera.DisplayName}";
        CameraImage.texture = ActiveCamera.OutputTexture;
    }
}
