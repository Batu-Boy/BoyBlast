using System.Collections;
using DG.Tweening;
using UnityEngine;

public class BlockGraphic : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BlockParticleHandler _particleHandler;
    [SerializeField] private GameObject glow;
    [SerializeField] private Animation fallAnimation;
    [SerializeField] private SpriteRenderer[] iconRenderers;
    
    private Block _block;
    private SpriteRenderer _currentIcon;
    
    public void InitializeGraphic(Block block,Sprite[] icons,int iconIndex)
    {
        _block = block;
        transform.position = block.Position;
        name = $"BlockGraphic: ({block.Position.x},{block.Position.y})";
        
        InitIcons(icons);
        CloseAllIcons();
        InitCurrentIcon(iconIndex);
    }

    public void InitializeGraphic(Block block,Sprite[] icons, int iconIndex, Vector3Int position)
    {
        _block = block;
        transform.position = position;
        name = $"BlockGraphic: ({block.Position.x},{block.Position.y})";
        InitIcons(icons);
        CloseAllIcons();
        InitCurrentIcon(iconIndex);
    }
    
    private void OnDestroy()
    {
        StopAllCoroutines();
        DOTween.Kill(this);
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
        
        SetRenderersSortOrder(_block.Position.y);
    }
    private void InitCurrentIcon(int index)
    {
        SpriteRenderer iconRenderer = iconRenderers[index];
        iconRenderer.SetActiveGameObject(true);
        _currentIcon = iconRenderer;
    }

    public void ChangeIconAtIndex(int index)
    {
        SpriteRenderer targetIcon = iconRenderers[index];
        if (targetIcon == _currentIcon) return;

        _currentIcon.SetActiveGameObject(false);
        _currentIcon = targetIcon;
        _currentIcon.SetActiveGameObject(true);
    }

    public void BringForward()
    {
        _currentIcon.maskInteraction = SpriteMaskInteraction.None;
        _currentIcon.sortingLayerName = "HighlightBlock";
        _currentIcon.sortingOrder = _block.Position.y;
        glow.SetActive(true);
    }
    
    public void SetRenderersSortOrder(int order)
    {
        foreach (var spriteRenderer in iconRenderers)
        {
            spriteRenderer.sortingOrder = order;
        }
    }

    public void CloseAllIcons()
    {
        foreach (var iconRenderer in iconRenderers)
        {
            iconRenderer.SetActiveGameObject(false);
        }
    }
    
    public void ResetIconValues()
    {
        CloseAllIcons();
        _currentIcon = iconRenderers[0];
        glow.SetActive(false);
    }
    
    #endregion

    #region GraphicActions
    
    public void OnBlockFall(float fallDuration = .3f)
    {
        GoBlockPosition(fallDuration);
    }
    
    public void GoBlockPosition(float fallDuration = .3f)
    {
        fallAnimation.Play();
        SetRenderersSortOrder(_block.Position.y);
        var distance = Vector3.Distance(transform.position, _block.Position);
        transform.DOMove(_block.Position, fallDuration * ((_block.Position.y * .055f) + 1)).SetEase(Ease.InSine);
        DOVirtual.DelayedCall(fallDuration,() => fallAnimation.Play("BlockLand"));
        //Debug.Log($"{name} going to pos {_block.Position}");
        name = $"BlockGraphic: ({_block.Position.x},{_block.Position.y})";
    }

    public void GoShufflePosition(float offsetX,float duration = 1)
    {
        transform.DOMove(transform.position + Vector3.right * offsetX, duration).SetEase(Ease.InSine)
            .OnComplete(()=> Destroy(gameObject));
    }
    
    public void OnBlockGroupDestroy(BlockGroup blockGroup, Cell clickedCell)
    {
        switch (blockGroup.ComboIndex)
        {
            case 0: 
                StartCoroutine(DestroyDefaultCoroutine());
                break;
            case 1:
                StartCoroutine(DestroyCombo1Coroutine(blockGroup.ComboIndex, clickedCell.GetPosition()));
                break;
            case 2:
                StartCoroutine(DestroyCombo1Coroutine(blockGroup.ComboIndex, clickedCell.GetPosition()));
                break;
            case 3:
                StartCoroutine(DestroyCombo1Coroutine(blockGroup.ComboIndex, clickedCell.GetPosition()));
                break;
        }
    }
    
    private IEnumerator DestroyDefaultCoroutine()
    {
        _particleHandler.PlayDestroy();
        CloseAllIcons();
        yield return new WaitForSeconds(.25f);
        //Debug.Log($"{name} Destroyed!");
        ResetIconValues();
        BlockPool.Instance.ReturnItem(_block);
    }
    
    private IEnumerator DestroyCombo1Coroutine(int combo, Vector3 clickedPos)
    {
        _particleHandler.PlayStarTrail();
        BringForward();
        var position = transform.position;
        Vector3 targetDirection = (clickedPos - position).normalized;
        float distanceFactor = .15f;
        DOTween.Sequence()
            .Append(transform.DOMove(position - (targetDirection * distanceFactor), .15f).SetEase(Ease.OutSine))
            .Append(transform.DOMove(clickedPos, .25f).SetEase(Ease.InSine));
        yield return new WaitForSeconds(.4f);
        //Debug.Log($"{name} Destroyed!");,
        ResetIconValues();
        yield return new WaitForSeconds(.25f);
        BlockPool.Instance.ReturnItem(_block);
    }
    
    #endregion

}