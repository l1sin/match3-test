using System.Collections;
using UnityEngine;
using DG.Tweening;

public class Entity : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _sr;
    [SerializeField] public byte EntityType;
    [SerializeField] public GameTile MyTile;
    [SerializeField] private GameObject _particleSystem;
    public Vector3Int Pos;
    private static float s_callTime = 0;
    private static float s_travelTime = 0.1f;
    private static float s_animTime = 0.1f;
    private static float s_minSizeY = 0.85f;
    private bool _canCallUpperToFall = true;
    public bool Spawned;
    [SerializeField] private int _distanceTraveled;

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
        transform.DOScale(Vector3.one, s_travelTime).OnComplete(SetSpawned);
    }

    private void SetSpawned()
    {
        Spawned = true;
    }

    public void Die(float seconds)
    {
        Instantiate(_particleSystem, transform.position, Quaternion.identity);
        transform.DORotate(new Vector3(0, 0, 180), seconds).OnComplete(() => Destroy(gameObject));
        transform.DOScale(Vector3.zero, seconds).OnComplete(() => Destroy(gameObject));
    }

    public void EndFall()
    {
        if (_distanceTraveled > 0)
        {
            Sequence mySequence = DOTween.Sequence();
            mySequence.Append(transform.DOScaleY(s_minSizeY, s_animTime * 0.5f));
            mySequence.Append(transform.DOScaleY(1f, s_animTime * 0.5f));
        } 

        _distanceTraveled = 0;
        _canCallUpperToFall = true;
    }

    public void Fall()
    {
        if (MyTile.CurrentObstacle != null) return;
        if (EntitySpawner.Instance.CheckTileBelow(Pos))
        {
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
        transform.DOMove(Pos + EntitySpawner.half, s_travelTime).SetEase(Ease.Linear).OnComplete(Fall);
    }

    private IEnumerator CallUpperTile()
    {
        _canCallUpperToFall = false;
        Vector3Int pos = Pos + Vector3Int.up;
        GameTile otherTile = EntitySpawner.Instance.GetTile(pos);
        yield return new WaitForSeconds(s_callTime);
        if (otherTile != null && otherTile.CurrentEntity != null && otherTile.CurrentEntity.Spawned) otherTile.CurrentEntity.Fall();
        else if (otherTile == null)
        {
            if (Spawned)
            {
                StartCoroutine(SpawnEntity(pos, 0));
            }
            else
            {
                StartCoroutine(SpawnEntity(pos, s_travelTime));
            }
        }
    }

    private IEnumerator SpawnEntity(Vector3Int pos, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        EntitySpawner.Instance.InstantiateEntityCelling(pos);
    }
}
