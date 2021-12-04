
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Camera)), RequireComponent(typeof(AudioListener))]
public class VR_Camera : MonoBehaviour
{
    [SerializeField] protected string name;
    [SerializeField] protected RawImage UI_Projection;
    private AudioListener audioListener;

    private void Start()
    {
        audioListener = GetComponent<AudioListener>();
    }

    public virtual void Activate(bool active, CamSelectorUI cameraSelector = null)
    {
        UI_Projection.gameObject.SetActive(active);
        audioListener.enabled = active;
        if (active && cameraSelector != null)
            cameraSelector.SetText(name);
    }

    public bool IsActive()
    {
        return audioListener.enabled;
    }
}
