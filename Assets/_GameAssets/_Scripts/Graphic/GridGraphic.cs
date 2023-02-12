using UnityEngine;

public class GridGraphic : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;

    public void InitializeGraphic(Vector3Int size, Sprite icon = null)
    {
        transform.localScale = size;
        
        if(icon)
            _spriteRenderer.sprite = icon;
    }
}
