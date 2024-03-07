using System.Collections;
using UnityEngine;
using DG.Tweening;

public class Entity : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _sr;
    [SerializeField] public byte EntityType;
    [SerializeField] public GameTile MyTile;
    public Vector3Int Pos;
    private static float _callTime = 0;
    private static float _travelTime = 0.1f;
    private static float _animTime = 0.1f;
    private static float _minSizeY = 0.85f;
    private bool _canCallUpperToFall = true;
    public bool Spawned;

    private void Start()
    {
        _canCallUpperToFall = true;
        Pop();
    }

    public void SetSprite(Sprite sprite)
    {
        _sr.sprite = sprite;
    }

    private void Pop()
    {
        transform.localScale = Vector3.zero;
        transform.DOScale(Vector3.one, _travelTime).OnComplete(SetSpawned);
    }

    private void SetSpawned()
    {
        Spawned = true;
    }

    public void Die(float seconds)
    {
        transform.DORotate(new Vector3(0, 0, 180), seconds).OnComplete(() => Destroy(gameObject));
        transform.DOScale(Vector3.zero, seconds).OnComplete(() => Destroy(gameObject));
    }

    public void EndFall()
    {
        Sequence mySequence = DOTween.Sequence();

        mySequence.Append(transform.DOScaleY(_minSizeY, _animTime * 0.5f));
        mySequence.Append(transform.DOScaleY(1f, _animTime * 0.5f));
    }

    public void Fall()
    {
        if (EntitySpawner.Instance.CheckTileBelow(Pos))
        {
            if (_canCallUpperToFall) StartCoroutine(CallUpperTile());
            MyTile.CurrentEntity = null;
            MyTile = null;
            MoveDown();
        }
        else
        {
            _canCallUpperToFall = true;
            EndFall();
        }
    }

    public void MoveDown()
    {
        Pos += Vector3Int.down;
        MyTile = EntitySpawner.Instance.GetTile(Pos);
        MyTile.CurrentEntity = this;
        transform.DOMove(Pos + EntitySpawner.half, _travelTime).SetEase(Ease.Linear).OnComplete(Fall);
    }

    private IEnumerator CallUpperTile()
    {
        _canCallUpperToFall = false;
        Vector3Int pos = Pos + Vector3Int.up;
        GameTile otherTile = EntitySpawner.Instance.GetTile(pos);
        yield return new WaitForSeconds(_callTime);
        if (otherTile != null && otherTile.CurrentEntity != null && otherTile.CurrentEntity.Spawned) otherTile.CurrentEntity.Fall();
        else if (otherTile == null)
        {
            if (Spawned)
            {
                StartCoroutine(SpawnEntity(pos, 0));
            }
            else
            {
                StartCoroutine(SpawnEntity(pos, _travelTime));
            }
        }
    }

    private IEnumerator SpawnEntity(Vector3Int pos, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        EntitySpawner.Instance.InstantiateEntityCelling(pos);
    }
}
