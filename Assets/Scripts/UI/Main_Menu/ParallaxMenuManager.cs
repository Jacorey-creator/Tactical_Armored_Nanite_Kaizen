using UnityEngine;
using System.Collections.Generic;

public class ParallaxMenuManager : MonoBehaviour
{
    [System.Serializable]
    public class ParallaxLayer
    {
        public Transform element;
        public float parallaxMultiplier = 0.1f;
        public Vector2 movementLimit = new Vector2(50f, 50f);
        [HideInInspector]
        public Vector3 startPosition;
    }

    [Header("Parallax Layers")]
    public List<ParallaxLayer> parallaxLayers = new List<ParallaxLayer>();

    [Header("Global Settings")]
    public float smoothing = 2f;
    public bool invertX = false;
    public bool invertY = false;

    [Header("Deadzone Settings")]
    [Range(0f, 0.5f)]
    [Tooltip("Size of the deadzone in the center where no parallax occurs (0 = no deadzone, 0.5 = half screen)")]
    public float deadzoneRadius = 0.1f;

    [Range(0f, 0.3f)]
    [Tooltip("Smooth transition zone around the deadzone (0 = hard edge, higher = smoother transition)")]
    public float deadzoneSmoothing = 0.05f;

    void Start()
    {
        // Store initial positions
        foreach (var layer in parallaxLayers)
        {
            if (layer.element != null)
                layer.startPosition = layer.element.localPosition;
        }
    }

    void Update()
    {
        // Get normalized mouse position (-1 to 1)
        Vector2 mousePosition = Input.mousePosition;
        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Vector2 mouseOffset = (mousePosition - screenCenter) / screenCenter;

        // Apply deadzone
        mouseOffset = ApplyDeadzone(mouseOffset);

        if (invertX) mouseOffset.x = -mouseOffset.x;
        if (invertY) mouseOffset.y = -mouseOffset.y;

        // Apply parallax to each layer
        foreach (var layer in parallaxLayers)
        {
            if (layer.element == null) continue;

            Vector3 targetPosition = layer.startPosition + new Vector3(
                mouseOffset.x * layer.parallaxMultiplier * layer.movementLimit.x,
                mouseOffset.y * layer.parallaxMultiplier * layer.movementLimit.y,
                0
            );

            layer.element.localPosition = Vector3.Lerp(
                layer.element.localPosition,
                targetPosition,
                Time.deltaTime * smoothing
            );
        }
    }

    private Vector2 ApplyDeadzone(Vector2 input)
    {
        // Calculate distance from center
        float distance = input.magnitude;

        // If within deadzone, return zero movement
        if (distance <= deadzoneRadius)
        {
            return Vector2.zero;
        }

        // Calculate smoothing factor
        float smoothingZone = deadzoneRadius + deadzoneSmoothing;
        float smoothingFactor = 1f;

        // Apply smooth transition if within smoothing zone
        if (distance < smoothingZone && deadzoneSmoothing > 0f)
        {
            float transitionProgress = (distance - deadzoneRadius) / deadzoneSmoothing;
            smoothingFactor = Mathf.SmoothStep(0f, 1f, transitionProgress);
        }

        // Remap the input to account for deadzone
        Vector2 direction = input.normalized;
        float remappedDistance = Mathf.Max(0f, distance - deadzoneRadius) / (1f - deadzoneRadius);

        return direction * remappedDistance * smoothingFactor;
    }

    // Helper method to visualize deadzone in Scene view
    void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;

        // Draw deadzone circle in screen space (approximate visualization)
        Gizmos.color = Color.red;
        Vector3 screenCenter = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2f, Screen.height / 2f, 10f));
        Gizmos.DrawWireSphere(screenCenter, deadzoneRadius * 2f);

        // Draw smoothing zone
        if (deadzoneSmoothing > 0f)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(screenCenter, (deadzoneRadius + deadzoneSmoothing) * 2f);
        }
    }
}