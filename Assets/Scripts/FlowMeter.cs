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

    private float _dir = 1f;

    public enum FlowResult { Miss, Green, Yellow }

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

        pos.x += speed * _dir * Time.unscaledDeltaTime; // UI = unscaled beter

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

        // Check green zones first
        if (IsInsideZone(barX, greenLeft) || IsInsideZone(barX, greenRight))
            return FlowResult.Green;

        // Then check yellow zone
        if (IsInsideZone(barX, yellow))
            return FlowResult.Yellow;

        // Outside zones = Miss
        return FlowResult.Miss;
    }



    private bool IsInsideZone(float x, RectTransform zone)
    {
        if (zone == null) return false;

        float halfWidth = zone.rect.width * 0.5f;
        float min = zone.anchoredPosition.x - halfWidth;
        float max = zone.anchoredPosition.x + halfWidth;

        return x >= min && x <= max;
    }
    public bool IsBarInAnyZone()
    {
        float barX = movingBar.anchoredPosition.x;
        return IsInsideZone(barX, greenLeft) || IsInsideZone(barX, greenRight) || IsInsideZone(barX, yellow);
    }


}
