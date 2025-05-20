using System;
using UnityEngine;

public static class TankVisualManager
{
    public static void SetTankVisual(GameObject host, GameObject newVisual, out Transform firePoint)
    {
        firePoint = null;

        if (newVisual == null)
        {
            Debug.LogWarning("New tank visual is null. Cannot set visual.");
            return;
        }

        DestroyOldVisual(host.transform);
        GameObject visualInstance = InstantiateNewVisual(host.transform, newVisual);
        CopyColliderFromVisualToRoot(host, newVisual);
        firePoint = SetFirePointFromVisual(visualInstance);
    }

    private static void DestroyOldVisual(Transform hostTransform)
    {
        foreach (Transform child in hostTransform)
        {
            if (child.name.Contains("_Tank"))
            {
                UnityEngine.Object.Destroy(child.gameObject);
                break;
            }
        }
    }
    private static GameObject InstantiateNewVisual(Transform parent, GameObject visualPrefab)
    {
        GameObject instance = UnityEngine.Object.Instantiate(visualPrefab, parent);
        instance.name = visualPrefab.name;

        instance.transform.localPosition = Vector3.zero;
        instance.transform.localRotation = Quaternion.identity;
        instance.transform.localScale = Vector3.one;

        return instance;
    }
    private static Transform SetFirePointFromVisual(GameObject visualInstance)
    {
        Transform newFirePoint = visualInstance.transform.Find("FiringPoint");
        if (newFirePoint == null)
        {
            Debug.LogWarning("FirePoint not found in the new tank visual.");
        }
        return newFirePoint;
    }
    private static void CopyColliderFromVisualToRoot(GameObject host, GameObject visual)
    {
        try
        {
            Collider sourceCollider = visual.GetComponent<Collider>();
            if (sourceCollider == null)
            {
                Debug.LogWarning("No collider found on new visual to copy.");
                return;
            }

            Collider oldCollider = host.GetComponent<Collider>();
            if (oldCollider != null) UnityEngine.Object.Destroy(oldCollider, 0f);

            System.Type type = sourceCollider.GetType();
            Collider newCollider = (Collider)host.AddComponent(type) as Collider;

            switch (sourceCollider)
            {
                case BoxCollider boxSrc when newCollider is BoxCollider box:
                    box.center = boxSrc.center;
                    box.size = boxSrc.size;
                    box.isTrigger = boxSrc.isTrigger;
                    box.sharedMaterial = boxSrc.sharedMaterial;
                    break;

                case CapsuleCollider capSrc when newCollider is CapsuleCollider cap:
                    cap.center = capSrc.center;
                    cap.radius = capSrc.radius;
                    cap.height = capSrc.height;
                    cap.direction = capSrc.direction;
                    cap.isTrigger = capSrc.isTrigger;
                    cap.sharedMaterial = capSrc.sharedMaterial;
                    break;

                case SphereCollider sphereSrc when newCollider is SphereCollider sphere:
                    sphere.center = sphereSrc.center;
                    sphere.radius = sphereSrc.radius;
                    sphere.isTrigger = sphereSrc.isTrigger;
                    sphere.sharedMaterial = sphereSrc.sharedMaterial;
                    break;

                case MeshCollider meshSrc when newCollider is MeshCollider mesh:
                    mesh.sharedMesh = meshSrc.sharedMesh;
                    mesh.convex = meshSrc.convex;
                    mesh.isTrigger = meshSrc.isTrigger;
                    mesh.sharedMaterial = meshSrc.sharedMaterial;
                    break;

                default:
                    Debug.LogWarning("Unsupported collider type: " + type);
                    break;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Failed to copy collider: " + ex.Message);
        }
    }
}
