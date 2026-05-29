using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    static MusicPlayer instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            var audio = GetComponent<AudioSource>();
            if (audio != null && !audio.isPlaying)
                audio.Play();
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
