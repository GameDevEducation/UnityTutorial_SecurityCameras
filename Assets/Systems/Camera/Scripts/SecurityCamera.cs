using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecurityCamera : MonoBehaviour
{
    [SerializeField] string _DisplayName;
    [SerializeField] Camera LinkedCamera;

    [SerializeField] bool SyncToMainCameraConfig = true;

    [SerializeField] AudioListener CameraAudio;
    [SerializeField] Transform PivotPoint;
    [SerializeField] float DefaultPitch = 20f;
    [SerializeField] float AngleSwept = 60f;
    [SerializeField] float SweepSpeed = 6f;
    [SerializeField] int OutputTextureSize = 256;

    public RenderTexture OutputTexture { get; private set; }
    public string DisplayName => _DisplayName;

    float CurrentAngle = 0f;
    bool SweepClockwise = true;
    List<SecurityConsole> CurrentlyWatchingConsoles = new List<SecurityConsole>();

    // Start is called before the first frame update
    void Start()
    {
        // turn the camera off by default
        LinkedCamera.enabled = false;
        CameraAudio.enabled = false;

        if (SyncToMainCameraConfig)
        {
            LinkedCamera.clearFlags = Camera.main.clearFlags;
            LinkedCamera.backgroundColor = Camera.main.backgroundColor;
        }

        // setup the render texture
        OutputTexture = new RenderTexture(OutputTextureSize, OutputTextureSize, 32);
        LinkedCamera.targetTexture = OutputTexture;
    }

    // Update is called once per frame
    void Update()
    {
        // update the angle
        CurrentAngle += SweepSpeed * Time.deltaTime * (SweepClockwise ? 1f : -1f);
        if (Mathf.Abs(CurrentAngle) >= (AngleSwept * 0.5f))
            SweepClockwise = !SweepClockwise;

        // rotate the camera
        PivotPoint.transform.localEulerAngles = new Vector3(0f, CurrentAngle, DefaultPitch);
    }

    public void StartWatching(SecurityConsole linkedConsole)
    {
        if (!CurrentlyWatchingConsoles.Contains(linkedConsole))
            CurrentlyWatchingConsoles.Add(linkedConsole);

        OnWatchersChanged();
    }

    public void StopWatching(SecurityConsole linkedConsole)
    {
        CurrentlyWatchingConsoles.Remove(linkedConsole);

        OnWatchersChanged();
    }

    void OnWatchersChanged()
    {
        LinkedCamera.enabled = CurrentlyWatchingConsoles.Count > 0;
    }
}
