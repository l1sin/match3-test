using DG.Tweening;
using System.Collections;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [Header("Public values")]
    public Vector3Int Pos;
    public byte EntityType;
    public GameTile MyTile;

    [Header("Prefab setting")]
    [SerializeField] private SpriteRenderer _sr;
    [SerializeField] private GameObject _particleSystem;
    [SerializeField] private float _callTime = 0f;
    [SerializeField] private float _travelTime = 0.1f;
    [SerializeField] private float _animTime = 0.1f;
    [SerializeField] private float _minSizeY = 0.85f;

    [Header("System values")]
    private bool _canCallUpperToFall = true;
    private bool _spawned = false;
    private bool _isFalling;
    private int _distanceTraveled = 0;

    private void Start()
    {
        Init();
        Pop();
    }

    private void Init()
    {
        _canCallUpperToFall = true;
        _spawned = false;
        _distanceTraveled = 0;
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
        _spawned = true;
    }

    public void Die(float seconds)
    {
        LevelController.Instance.GainScore();
        Instantiate(_particleSystem, transform.position, Quaternion.identity);
        transform.DORotate(new Vector3(0, 0, 180), seconds).OnComplete(() => Destroy(gameObject));
        transform.DOScale(Vector3.zero, seconds).OnComplete(() => Destroy(gameObject));
    }

    public void EndFall()
    {
        if (_distanceTraveled > 0)
        {
            Sequence mySequence = DOTween.Sequence();
            mySequence.Append(transform.DOScaleY(_minSizeY, _animTime * 0.5f));
            mySequence.Append(transform.DOScaleY(1f, _animTime * 0.5f));
        }
        if (_isFalling)
        {
            _isFalling = false;
            EntitySpawner.Instance.FallingEntities--;
        }
        _distanceTraveled = 0;
        _canCallUpperToFall = true;
    }

    public void Fall()
    {
        if (MyTile.CurrentObstacle != null) return;
        if (EntitySpawner.Instance.CheckTileBelow(Pos))
        {
            if (!_isFalling)
            {
                _isFalling = true;
                EntitySpawner.Instance.FallingEntities++;
            }
            if (_canCallUpperToFall) StartCoroutine(CallUpperTile());
            MyTile.CurrentEntity = null;
            MyTile = null;
            MoveDown();
        }
        else
        {
            EndFall();
        }
    }

    public void MoveDown()
    {
        _distanceTraveled++;
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
        if (otherTile != null && otherTile.CurrentEntity != null && otherTile.CurrentEntity._spawned) otherTile.CurrentEntity.Fall();
        else if (otherTile == null)
        {
            if (_spawned)
            {
                StartCoroutine(InstantiateEntityCelling(pos, 0));
            }
            else
            {
                StartCoroutine(InstantiateEntityCelling(pos, _travelTime));
            }
        }
    }

    private IEnumerator InstantiateEntityCelling(Vector3Int pos, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        EntitySpawner.Instance.InstantiateEntityCelling(pos);
    }
}
