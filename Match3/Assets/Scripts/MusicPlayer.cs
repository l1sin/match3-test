using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    private static MusicPlayer Instance;
    [SerializeField] private AudioSource _audioSource;

    private void OnEnable()
    {
        Singleton();
    }

    private void Singleton()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
        gameObject.transform.parent = null;
        DontDestroyOnLoad(gameObject);
    }
}
