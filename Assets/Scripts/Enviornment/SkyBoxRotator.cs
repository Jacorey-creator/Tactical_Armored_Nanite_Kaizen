using UnityEngine;

public class SkyboxRotator : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 1.0f;

    [SerializeField] private Vector3 rotationAxis = Vector3.up;

    [SerializeField] private bool isRotating = true;

    // Material property name that controls the rotation
    private readonly string rotationPropertyName = "_Rotation";

    // Current rotation value
    private float currentRotation = 0.0f;

    private void Start()
    {
        // Make sure we have a skybox material set
        if (RenderSettings.skybox == null)
        {
            Debug.LogWarning("No skybox material is set in RenderSettings!");
            enabled = false;
            return;
        }

        // Initialize the rotation value from the current skybox setting
        currentRotation = RenderSettings.skybox.GetFloat(rotationPropertyName);
    }

    private void Update()
    {
        if (!isRotating) return;

        // Calculate the new rotation angle
        currentRotation += rotationSpeed * Time.deltaTime;

        // Keep the angle between 0 and 360 degrees
        if (currentRotation > 360.0f)
        {
            currentRotation -= 360.0f;
        }

        // Apply the rotation to the skybox
        RenderSettings.skybox.SetFloat(rotationPropertyName, currentRotation);
    }

    public void ToggleRotation()
    {
        isRotating = !isRotating;
    }

    public void SetRotationSpeed(float speed)
    {
        rotationSpeed = speed;
    }

    public void SetRotation(float angle)
    {
        currentRotation = angle % 360.0f;
        RenderSettings.skybox.SetFloat(rotationPropertyName, currentRotation);
    }
}