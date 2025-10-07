using UnityEngine;

public class TrackCamera : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float smoothSpeed = 5f;
    Vector3 offset = new Vector3(0, 0, -10f);

    private void FixedUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.fixedDeltaTime);
        transform.position = smoothedPosition;
    }

}
