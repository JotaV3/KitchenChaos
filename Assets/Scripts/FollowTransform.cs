using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class FollowTransform : MonoBehaviour
{
    private Transform targetTransform;

    private void LateUpdate()
    {
        if (targetTransform == null) return;

        transform.position = targetTransform.position;
        transform.rotation = targetTransform.rotation;
    }

    public void SetTargetTransform(Transform targetTransform)
    {
        this.targetTransform = targetTransform;
    }
}
