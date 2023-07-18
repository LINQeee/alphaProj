using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private Text percentageText;

    public void UpdateBarValue(float value)
    {
        fillImage.fillAmount = Mathf.Clamp01(value);
        percentageText.text = $"{Mathf.Round(value*100)}%";
    }
}