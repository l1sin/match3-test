using System.Collections;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _sr;
    [SerializeField] public byte EntityType;
    [SerializeField] public GameTile Tile;
    public Vector3Int Pos;
    private static float _callTime = 0.01f;
    private bool _canCallUpperToFall = true;
    private bool _isFalling = false;

    public void SetSprite(Sprite sprite)
    {
        _sr.sprite = sprite;
    }

    public void Fall()
    {
        _isFalling = true;
        if (_canCallUpperToFall) StartCoroutine(CallUpperTile());
        if (EntitySpawner.Instance.CheckTileBelow(Pos))
        {
            Tile.CurrentEntity = null;
            Tile = null;
            transform.position += Vector3Int.down;
            Pos += Vector3Int.down;
            Tile = EntitySpawner.Instance.GetTile(Pos);
            Tile.CurrentEntity = this;
            Fall();
        }
        else _isFalling = false;
    }

    private IEnumerator CallUpperTile()
    { 
        GameTile otherTile = EntitySpawner.Instance.GetTile(Pos + Vector3Int.up);
        yield return new WaitForSeconds(_callTime);
        if (otherTile != null && otherTile.CurrentEntity != null) otherTile.CurrentEntity.Fall();
    }
}
