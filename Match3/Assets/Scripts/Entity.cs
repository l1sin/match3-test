using System.Collections;
using UnityEngine;
using DG.Tweening;

public class Entity : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _sr;
    [SerializeField] public byte EntityType;
    [SerializeField] public GameTile MyTile;
    public Vector3Int Pos;
    private static float _callTime = 0.01f;
    private static float _travelTime = 0.1f;
    private bool _canCallUpperToFall = true;

    private void Start()
    {
        _canCallUpperToFall = true;
    }

    public void SetSprite(Sprite sprite)
    {
        _sr.sprite = sprite;
    }

    public void Fall()
    {
        if (EntitySpawner.Instance.CheckTileBelow(Pos))
        {
            if (_canCallUpperToFall) StartCoroutine(CallUpperTile());
            MyTile.CurrentEntity = null;
            MyTile = null;
            Pos += Vector3Int.down;
            MyTile = EntitySpawner.Instance.GetTile(Pos);
            MyTile.CurrentEntity = this;
            Fall();
        }
        else
        {
            _canCallUpperToFall = true;
            transform.DOMove(Pos, _travelTime);
        }
    }

    public void MoveDown()
    {
        Pos += Vector3Int.down;
        MyTile = EntitySpawner.Instance.GetTile(Pos);
        MyTile.CurrentEntity = this;
        transform.DOMove(Pos, _travelTime * 0.5f);
        Fall();
    }

    private IEnumerator CallUpperTile()
    {
        _canCallUpperToFall = false;
        Vector3Int pos = Pos + Vector3Int.up;
        GameTile otherTile = EntitySpawner.Instance.GetTile(pos);
        yield return new WaitForSeconds(_callTime);
        if (otherTile != null && otherTile.CurrentEntity != null) otherTile.CurrentEntity.Fall();
        else if (otherTile == null)
        {
            EntitySpawner.Instance.InstantiateEntityCelling(pos);
        }
    }
}
