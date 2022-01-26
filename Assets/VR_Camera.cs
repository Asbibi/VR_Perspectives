
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Camera)), RequireComponent(typeof(AudioListener))]
public class VR_Camera : MonoBehaviour
{
    [SerializeField] protected string camName;
    [SerializeField] protected RawImage UI_Projection;
    private AudioListener audioListener;

    private void Awake()
    {
        audioListener = GetComponent<AudioListener>();
    }

    public virtual void Activate(bool active, CamSelectorUI cameraSelector = null)
    {
        UI_Projection.gameObject.SetActive(active);
        audioListener.enabled = active;
        if (active && cameraSelector != null)
            cameraSelector.SetText(camName);
    }
    public virtual void EditCam(Vector2 _input) { }

    public bool IsActive()
    {
        return audioListener.enabled;
    }
}
