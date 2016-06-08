using UnityEngine;

/// <summary>
/// Game camera
/// </summary>
public class ZRCamera : MonoBehaviour
{
    public static ZRCamera instance;
    public Transform    target;
    public Camera       mCamera;
    public Camera       miniMap;
    
    private void Awake()
    {
        instance = this;
        Subscribe();
        if (mCamera == null)
        {
            mCamera = GetComponentInChildren<Camera>();
        }
        if (miniMap != null)
        {
            miniMap.gameObject.SetActive(false);            
        }
        
    }

    private void Subscribe()
    {
        ZRInput.OnActionEvent += OnActionReaction;
        ZRScreenStory.OnStoryScreenClosedEvent += OnStoryScreenClosedReaction;
    }

    private void UnSubscribe()
    {
        ZRInput.OnActionEvent -= OnActionReaction;
        ZRScreenStory.OnStoryScreenClosedEvent -= OnStoryScreenClosedReaction;
    }    

    /// <summary>
    /// Enable minimap after story screen
    /// </summary>
    private void OnStoryScreenClosedReaction()
    {
        if (miniMap == null)
        {
            return;
        }
        miniMap.gameObject.SetActive(true);
    }

    private void OnDestroy()
    {
        UnSubscribe();
    }

    public void OnActionReaction(ZRAction action, ZRActionMode mode)
    {
        if (mode != ZRActionMode.Up)
        {
            return;
        }
        if (action != ZRAction.Shoot)
        {
            return;
        }
        Debug.Log("Camera shoot reaction");
    }

    //Move camera to target
    private void Update()
    {
        if (target == null)
        {
            return;
        }
        transform.position = Vector3.Slerp( transform.position, target.transform.position, 10 * Time.deltaTime );
    }

    /// <summary>
    /// Set camera target by event
    /// </summary>
    /// <param name="target"></param>
    public static void SetTarget(Transform target)
    {
        instance.target = target;
    }
}
