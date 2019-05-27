using UnityEngine;


[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class SyncronizedCamera : MonoBehaviour
{
    [SerializeField] protected Camera mainCamera;

    protected new Transform transform;
    protected new Camera camera;
    protected Transform main;

    protected virtual void Update()
    {
        if(transform == null)
        {
            transform = GetComponent<Transform>();
        }
        if(camera == null)
        {
            camera = GetComponent<Camera>();
        }
        if(mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        if(main == null)
        {
            main = mainCamera.transform;
        }

        camera.orthographicSize = mainCamera.orthographicSize;
        transform.position = main.position;
        transform.rotation = main.rotation;
    }

    protected virtual void OnValidate()
    {
        Update();
        camera.gameObject.tag = "Untagged";
        camera.orthographic = true;
        camera.renderingPath = RenderingPath.Forward;
        camera.orthographicSize = mainCamera.orthographicSize;
        camera.farClipPlane = mainCamera.farClipPlane;
        camera.nearClipPlane = mainCamera.nearClipPlane;
        camera.pixelRect = mainCamera.pixelRect;
    } 
}