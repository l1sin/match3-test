using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public int Health = 1;
    public Vector3Int Pos;
    public GameTile MyTile;
    [SerializeField] private SpriteRenderer _spriteRenderer;

    public void ConnectToTile()
    {
        Pos = new Vector3Int(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y), Mathf.FloorToInt(transform.position.z));
        MyTile = EntitySpawner.Instance.GetTile(Pos);
        MyTile.CurrentObstacle = this;
    }

    public void Damage()
    {
        Health--;
        if (Health <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        EntitySpawner.Instance.Obstacles.Remove(this);
        MyTile.CurrentObstacle = null;
        Destroy(gameObject);
    }
}
