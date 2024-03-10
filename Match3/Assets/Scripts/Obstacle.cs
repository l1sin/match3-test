using UnityEngine;
using DG.Tweening;

public class Obstacle : MonoBehaviour
{
    public int Health = 1;
    public Vector3Int Pos;
    public GameTile MyTile;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    private static float s_maxScale = 1.25f;
    private static Color s_endColor = new Color(1, 1, 1, 0);
    [SerializeField] private Sprite[] _sprites;

    public void ConnectToTile()
    {
        Pos = new Vector3Int(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y), Mathf.FloorToInt(transform.position.z));
        MyTile = EntitySpawner.Instance.GetTile(Pos);
        MyTile.CurrentObstacle = this;
    }

    public void Damage()
    {
        Health--;
        if (Health != 0) _spriteRenderer.sprite = _sprites[Health-1];
        if (Health <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        EntitySpawner.Instance.Obstacles.Remove(this);
        MyTile.CurrentObstacle = null;
        MyTile.CurrentEntity.Fall();
        transform.DOScale(s_maxScale, EntitySpawner.Instance.DeathWait).OnComplete(() => Destroy(gameObject));
        _spriteRenderer.DOColor(s_endColor, EntitySpawner.Instance.DeathWait).OnComplete(() => Destroy(gameObject));
    }
}
