using UnityEngine;

public class CellGraphic : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;

    [SerializeField] private Cell _cell;
    
    private bool isClicked;
    
    public void InitializeGraphic(Cell cell, Sprite icon = null)
    {
        _cell = cell;
        transform.position = cell.GetPosition();
        name = $"Cell ({transform.position.x},{transform.position.y})";
        
        if(icon)
            _spriteRenderer.sprite = icon;
    }
}
