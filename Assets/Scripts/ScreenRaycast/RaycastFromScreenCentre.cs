using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastFromScreenCentre : MonoBehaviour
{
    [Tooltip("The physics layers to try to hit with this raycast")] [SerializeField]
    private LayerMask hitLayer;

    [Tooltip("The maximum distance this raycast can travel")] [SerializeField]
    private float maxDistance;

    // Hold a reference to our camera selectr so we know which camera is in use
    private CameraSelector cameraSelector;
    
    // protected = like private, but child scripts can see
    // virtual = lets a child script override this function with its own version
    protected virtual void Start()
    {
        cameraSelector = FindObjectOfType<CameraSelector>();
    }

    public RaycastHit TryToHit()
    {
        // a struct cannot be "null", so we initialise an empty struct instead
        RaycastHit hit = new RaycastHit();
        
        // Get the currently used camera
        Camera camera = cameraSelector.GetCamera();

        Ray ray = camera.ScreenPointToRay(new Vector3(camera.pixelWidth, camera.pixelHeight) * 0.5f);

        if (Physics.Raycast(ray, out hit, maxDistance, hitLayer))
        {
            return hit;
        }
        
        // If we hit nothing, record the furthest point we *could* have hit
        hit.point = ray.origin + ray.direction * maxDistance;
        
        // then we can return the otherwise empty hit
        return hit;
    }
}
