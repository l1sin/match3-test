using UnityEngine;

public class Entity : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _sr;
    [SerializeField] public byte EntityType;

    public void SetSprite(Sprite sprite)
    {
        _sr.sprite = sprite;
    }
}
