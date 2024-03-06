using System.Collections;
using UnityEngine;
using DG.Tweening;

public class Entity : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _sr;
    [SerializeField] public byte EntityType;
    [SerializeField] public GameTile Tile;
    public Vector3Int Pos;
    private static float _callTime = 0.01f;
    private static float _travelTime = 0.1f;
    private bool _canCallUpperToFall = true;

    public void SetSprite(Sprite sprite)
    {
        _sr.sprite = sprite;
    }

    public void Fall()
    {
        if (_canCallUpperToFall) StartCoroutine(CallUpperTile());
        if (EntitySpawner.Instance.CheckTileBelow(Pos))
        {
            Tile.CurrentEntity = null;
            Tile = null;
            //transform.position += Vector3Int.down;
            Pos += Vector3Int.down;
            Tile = EntitySpawner.Instance.GetTile(Pos);
            Tile.CurrentEntity = this;
            Fall();
        }
        else
        {
            transform.DOMove(Pos, _travelTime);
        }
    }

    private IEnumerator CallUpperTile()
    { 
        GameTile otherTile = EntitySpawner.Instance.GetTile(Pos + Vector3Int.up);
        yield return new WaitForSeconds(_callTime);
        if (otherTile != null && otherTile.CurrentEntity != null) otherTile.CurrentEntity.Fall();
    }
}
