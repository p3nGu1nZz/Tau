using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public enum CameraState { Following, Free }

    public Transform target;
    public float height = 20f;
    public float followSpeed = 10f;
    public float zoomSpeed = 50f;
    public float minHeight = 5f;
    public float maxHeight = 50f;
    public float moveSpeed = 20f;
    public float smoothFactor = 0.1f;
    public float fastFollowSpeed = 10f;
    public GameObject rewardDisplay;
    public RewardDisplay rewardDisplayScript;

    private float targetHeight;
    private Vector3 targetPosition;
    private CameraState currentState;

    void Start()
    {
        targetHeight = height;
        targetPosition = transform.position;
        currentState = CameraState.Following;
    }

    void LateUpdate()
    {
        switch (currentState)
        {
            case CameraState.Following:
                FollowTarget();
                break;
        }

        HandleZoom();
        rewardDisplay.SetActive(currentState == CameraState.Following);
    }

    private void FollowTarget()
    {
        if (target != null)
        {
            Vector3 desiredPosition = new Vector3(target.position.x, height, target.position.z);
            transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * smoothFactor * Time.deltaTime);
        }
    }

    private void HandleZoom()
    {
        if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            targetHeight *= 0.75f;
        }
        if (Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            targetHeight *= 1.25f;
        }

        float scroll = Input.mouseScrollDelta.y;
        targetHeight -= scroll * zoomSpeed * Time.deltaTime;
        targetHeight = Mathf.Clamp(targetHeight, minHeight, maxHeight);
        height = Mathf.Lerp(height, targetHeight, zoomSpeed * smoothFactor * Time.deltaTime);
    }
}
