using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private static MusicManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); 
            GetComponent<AudioSource>().Play();
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
