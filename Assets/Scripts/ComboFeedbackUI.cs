using UnityEngine;
using TMPro;
using System.Collections;

public class ComboFeedbackUI : MonoBehaviour
{
    [SerializeField] private TMP_Text comboText;
    [SerializeField] private float displayTime = 2f;

    private Coroutine currentCoroutine;

    public void ShowCombo(int multiplier)
    {
        if (comboText == null) return;

        comboText.text = $"x{multiplier}";
        comboText.gameObject.SetActive(true);

        // stop eventueel lopende coroutine
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        currentCoroutine = StartCoroutine(HideAfterSeconds());
    }

    private IEnumerator HideAfterSeconds()
    {
        yield return new WaitForSeconds(displayTime);
        comboText.gameObject.SetActive(false);
    }
}
