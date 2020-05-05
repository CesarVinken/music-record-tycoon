using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [SerializeField]
    public List<Song> Songs = new List<Song>();

    public void Awake()
    {
        Instance = this;
    }
}
