using System.Collections;
using UnityEngine;

public class FlowMeter : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private RectTransform movingBar;
    [SerializeField] private RectTransform track;

    [Header("Zones")]
    [SerializeField] private RectTransform greenLeft;
    [SerializeField] private RectTransform yellow;
    [SerializeField] private RectTransform greenRight;

    [Header("Movement")]
    [SerializeField] private float speed = 600f;
    public float yellowSpeedMultiplier = 2f;

    private float _dir = 1f;
    private float _currentSpeed;

    public enum FlowResult { Miss, Green, Yellow } // public zodat PlayerController dit kan gebruiken
    public float YellowSpeedMultiplier => yellowSpeedMultiplier;

    private void Awake()
    {
        _currentSpeed = speed;
    }

    private void Update()
    {
        if (movingBar == null || track == null) return;
        MoveBar();
    }

    private void MoveBar()
    {
        float halfTrack = track.rect.width * 0.5f;
        float halfBar = movingBar.rect.width * 0.5f;

        float minX = -halfTrack + halfBar;
        float maxX = halfTrack - halfBar;

        Vector2 pos = movingBar.anchoredPosition;
        pos.x += _currentSpeed * _dir * Time.unscaledDeltaTime;

        if (pos.x >= maxX)
        {
            pos.x = maxX;
            _dir = -1f;
        }
        else if (pos.x <= minX)
        {
            pos.x = minX;
            _dir = 1f;
        }

        movingBar.anchoredPosition = pos;
    }

    public FlowResult CheckTiming()
    {
        float barX = movingBar.anchoredPosition.x;

        if (IsInsideZone(barX, greenLeft) || IsInsideZone(barX, greenRight))
            return FlowResult.Green;

        if (IsInsideZone(barX, yellow))
            return FlowResult.Yellow;

        return FlowResult.Miss;
    }

    public bool IsBarInAnyZone()
    {
        float barX = movingBar.anchoredPosition.x;
        return IsInsideZone(barX, greenLeft) || IsInsideZone(barX, greenRight) || IsInsideZone(barX, yellow);
    }

    private bool IsInsideZone(float x, RectTransform zone)
    {
        if (zone == null) return false;

        float halfWidth = zone.rect.width * 0.5f;
        float min = zone.anchoredPosition.x - halfWidth;
        float max = zone.anchoredPosition.x + halfWidth;

        return x >= min && x <= max;
    }

    // ==========================
    // TEMPORARY SPEED BOOST
    // ==========================
    public void BoostSpeedForSeconds(float multiplier, float duration)
    {
        StopAllCoroutines();
        StartCoroutine(SpeedBoostCoroutine(multiplier, duration));
    }

    private IEnumerator SpeedBoostCoroutine(float multiplier, float duration)
    {
        _currentSpeed = speed * multiplier;
        yield return new WaitForSeconds(duration);
        _currentSpeed = speed;
    }
}
