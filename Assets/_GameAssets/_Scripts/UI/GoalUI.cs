using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GoalUI : MonoBehaviour
{
    [SerializeField] private Image _iconImage;
    [SerializeField] private Image _successImage;
    [SerializeField] private TextMeshProUGUI _countText;

    
    public void Setup(Sprite icon, int count)
    {
        _iconImage.sprite = icon;
        _countText.text = count.ToString();
    }

    public void UpdateCount(int count)
    {
        _countText.text = count.ToString();
    }

    public void SetSuccess()
    {
        _countText.text = "0";
        DOTween.Sequence()
            .Append(_countText.rectTransform.DOScale(0, .25f).SetEase(Ease.InSine))
            .Append(_successImage.rectTransform.DOScale(1, .5f).SetEase(Ease.OutBack));
    }
}
