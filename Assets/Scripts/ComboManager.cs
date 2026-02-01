using UnityEngine;

public class ComboManager : MonoBehaviour
{
    public static ComboManager Instance;

    [SerializeField] private int multiplier = 1;
    [SerializeField] private int maxMultiplier = 128;

    private void Awake()
    {
        Instance = this;
    }

    public int CurrentMultiplier => multiplier;

    public void IncreaseMultiplier()
    {
        multiplier *= 2;
        multiplier = Mathf.Clamp(multiplier, 1, maxMultiplier);
    }

    public void ResetMultiplier()
    {
        multiplier = 1;
    }
}
