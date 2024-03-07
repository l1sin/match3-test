using System.Collections;
using UnityEngine;
using DG.Tweening;
using static UnityEditor.PlayerSettings;

public class Entity : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _sr;
    [SerializeField] public byte EntityType;
    [SerializeField] public GameTile MyTile;
    public Vector3Int Pos;
    private static float _callTime = 0;
    private static float _travelTime = 1f;
    private bool _canCallUpperToFall = true;
    private int _distance;
    [SerializeField] private bool _spawned;

    private void Start()
    {
        _distance = 0;
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
        transform.DOScale(Vector3.one, _travelTime).OnComplete(Spawned);
    }

    private void Spawned()
    {
        _spawned = true;
    }

    public void Die(float seconds)
    {
        transform.DOScale(Vector3.zero, seconds);
    }

    public void Fall()
    {
        if (EntitySpawner.Instance.CheckTileBelow(Pos))
        {
            _distance++;
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
            transform.DOMove(Pos + EntitySpawner.half, _travelTime * _distance).SetEase(Ease.Linear);
            _distance = 0;
        }
    }

    public void MoveDown()
    {
        _distance++;
        Pos += Vector3Int.down;
        MyTile = EntitySpawner.Instance.GetTile(Pos);
        MyTile.CurrentEntity = this;
        //transform.DOMove(Pos + EntitySpawner.half, _travelTime * _distance);
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
            if (_spawned)
            {
                StartCoroutine(SpawnEntity(pos, 0));
            }
            else
            {
                StartCoroutine(SpawnEntity(pos, _travelTime));
            }
            
            //EntitySpawner.Instance.InstantiateEntityCelling(pos);
        }
    }

    private IEnumerator SpawnEntity(Vector3Int pos, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        EntitySpawner.Instance.InstantiateEntityCelling(pos);
    }
}
