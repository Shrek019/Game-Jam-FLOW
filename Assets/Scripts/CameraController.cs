using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player; // sleep je player hier in

    [Header("Camera Settings")]
    [SerializeField] private Vector3 offset = new Vector3(2f, 0f, -10f); // X offset = player iets naar links
    [SerializeField] private float smoothSpeed = 5f; // hoe snel de camera volgt

    private void LateUpdate()
    {
        if (player == null) return;

        // gewenste positie = player + offset
        Vector3 targetPos = player.position + offset;

        // smooth follow
        transform.position = Vector3.Lerp(transform.position, targetPos, smoothSpeed * Time.deltaTime);
    }
}
