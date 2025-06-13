using UnityEngine;

[ExecuteInEditMode]
public class CameraEvents : MonoBehaviour
{
    public delegate void CameraEvent(Camera cam);

    [HideInInspector] public Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    private void OnPostRender()
    {
        ce_OnPostRender?.Invoke(cam);
    }

    private void OnPreCull()
    {
        ce_OnPreCull?.Invoke(cam);
    }

    private void OnPreRender()
    {
        ce_OnPreRender?.Invoke(cam);
    }

    public event CameraEvent ce_OnPreCull;
    public event CameraEvent ce_OnPreRender;

    public event CameraEvent ce_OnPostRender;
    //public event CameraEvent ce_OnRenderImage;

    public static CameraEvents GetFromCamera(Camera cam)
    {
        var ce = cam.GetComponent<CameraEvents>();
        if (ce == null) ce = cam.gameObject.AddComponent<CameraEvents>();
        return ce;
    }
}