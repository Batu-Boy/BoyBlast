using DG.Tweening;
using UnityEngine;

public class ElementGraphic : MonoBehaviour
{
    [SerializeField] private GameObject glow;
    [SerializeField] private Animation fallAnimation;
    [SerializeField] private SpriteRenderer[] iconRenderers;

    [HideInInspector] [SerializeReference] protected Element _element;
    private SpriteRenderer _currentIcon;

    public virtual void InitializeGraphic(Element element, Sprite[] icons, int iconIndex)
    {
        _element = element;
        transform.position = element.Position;
        UpdateName();
        InitIcons(icons);
        CloseAllIcons();
        InitCurrentIcon(iconIndex);
    }

    public virtual void InitializeGraphic(Element element, Sprite[] icons, int iconIndex, Vector3Int position)
    {
        _element = element;
        transform.position = position;
        UpdateName();
        InitIcons(icons);
        CloseAllIcons();
        InitCurrentIcon(iconIndex);
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
        DOTween.Kill(this);
    }

    public void OnElementFall(float fallDuration = .3f)
    {
        fallAnimation.Play();
        SetRenderersSortOrder(_element.Position.y);
        var totalDuration = fallDuration * (_element.Position.y * .055f + 1);
        transform.DOMove(_element.Position, totalDuration).SetEase(Ease.InSine);
        DOVirtual.DelayedCall(fallDuration, () => fallAnimation.Play("BlockLand"));
        UpdateName();
    }

    #region IconFuncions

    private void InitIcons(Sprite[] icons)
    {
        for (var i = 0; i < iconRenderers.Length; i++)
        {
            iconRenderers[i].sprite = icons[i];
            iconRenderers[i].sortingLayerName = "BlockOnGrid";
            iconRenderers[i].maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
        }

        SetRenderersSortOrder(_element.Position.y);
    }

    private void InitCurrentIcon(int index)
    {
        var iconRenderer = iconRenderers[index];
        iconRenderer.SetActiveGameObject(true);
        _currentIcon = iconRenderer;
    }

    public void ChangeIconAtIndex(int index)
    {
        var targetIcon = iconRenderers[index];
        if (targetIcon == _currentIcon) return;

        _currentIcon.SetActiveGameObject(false);
        _currentIcon = targetIcon;
        _currentIcon.SetActiveGameObject(true);
    }

    protected void BringForward()
    {
        _currentIcon.maskInteraction = SpriteMaskInteraction.None;
        _currentIcon.sortingLayerName = "HighlightBlock";
        _currentIcon.sortingOrder = _element.Position.y;
        glow.SetActive(true);
    }

    protected void SetRenderersSortOrder(int order)
    {
        foreach (var spriteRenderer in iconRenderers) 
            spriteRenderer.sortingOrder = order;
    }

    protected void CloseAllIcons()
    {
        foreach (var iconRenderer in iconRenderers) 
            iconRenderer.SetActiveGameObject(false);
    }

    protected void ResetIconValues()
    {
        CloseAllIcons();
        _currentIcon = iconRenderers[0];
        glow.SetActive(false);
    }

    #endregion

    public virtual void Explode()
    {
    }

    private void UpdateName()
    {
        name = $"{_element.GetType()}({_element.Position.x},{_element.Position.y})";
    }
}