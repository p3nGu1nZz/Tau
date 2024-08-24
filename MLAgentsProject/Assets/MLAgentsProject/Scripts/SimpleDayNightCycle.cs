using UnityEngine;

public class SimpleDayNightCycle : MonoBehaviour
{
    public Light directionalLight;
    public float dayDuration = 10000f; // Duration of a full day in seconds
    public int frameSkip = 60; // Number of frames to skip

    private float rotationSpeed;
    private int frameCount;

    void Start()
    {
        if (directionalLight == null)
        {
            Debug.LogError("Directional Light not assigned.");
            return;
        }

        // Calculate rotation speed based on day duration
        rotationSpeed = 360f / dayDuration;
        frameCount = 0;
    }

    void Update()
    {
        frameCount++;
        if (frameCount >= frameSkip)
        {
            // Rotate the light around the x-axis
            directionalLight.transform.Rotate(Vector3.right, rotationSpeed * Time.deltaTime);
            frameCount = 0;
        }
    }
}
