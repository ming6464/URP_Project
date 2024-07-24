using UnityEngine;

public class CellInfo : MonoBehaviour
{
    public bool isReady;
    public Vector2 gridPosition;
    public SpriteRenderer spriteRenderer;
    

    public void Init(Vector2 gridPos, Vector2 gridSpace)
    {
        ResetState();
        var pos2 = gridPos * gridSpace;
        Vector3 pos = new Vector3(pos2.x,pos2.y, 0);
        spriteRenderer.transform.localPosition = pos;
        gridPosition = gridPos;
    }

    public void ResetState()
    {
        spriteRenderer.color = Color.white;
        isReady = true;
    }

    public void Draw(Color color)
    {
        spriteRenderer.color = color;
        isReady = false;
    }
}