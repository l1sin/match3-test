using UnityEngine;

public class ParticleController : MonoBehaviour
{
    [SerializeField] private ParticleSystem _particleSystem;
    void Update()
    {
        if (!_particleSystem.isPlaying) Destroy(gameObject);
    }
}
