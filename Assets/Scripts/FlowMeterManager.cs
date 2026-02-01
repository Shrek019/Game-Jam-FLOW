using UnityEngine;

public class FlowMeterManager : MonoBehaviour
{
    [Header("All FlowMeters")]
    [SerializeField] private FlowMeter[] flowMeters; // sleep hier alle 3 FlowMeters
    [SerializeField, Range(0f, 1f)] private float changeChance = 0.3f; // 30% kans om te switchen

    private FlowMeter activeFlowMeter;

    private void Awake()
    {
        if (flowMeters.Length == 0) return;

        // activeer eerste flowmeter standaard
        SetActiveFlowMeter(flowMeters[0]);
    }

    public FlowMeter GetActiveFlowMeter()
    {
        return activeFlowMeter;
    }

    public void TryChangeFlowMeter()
    {
        if (flowMeters.Length <= 1) return;

        if (Random.value <= changeChance)
        {
            // kies een andere flowmeter
            FlowMeter newFlow = activeFlowMeter;
            while (newFlow == activeFlowMeter)
            {
                int index = Random.Range(0, flowMeters.Length);
                newFlow = flowMeters[index];
            }

            SetActiveFlowMeter(newFlow);
        }
    }

    private void SetActiveFlowMeter(FlowMeter flowMeter)
    {
        activeFlowMeter = flowMeter;

        // zet alle flowmeters inactive behalve deze
        foreach (var fm in flowMeters)
        {
            fm.gameObject.SetActive(fm == flowMeter);
        }
    }
}
