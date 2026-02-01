using TMPro;
using UnityEngine;

public class HighScoreUI : MonoBehaviour
{
    [SerializeField] private TMP_Text highScoreText;

    private void Update()
    {
        highScoreText.text = "High Score: \n" + ScoreManager.HighScore;
    }
}
