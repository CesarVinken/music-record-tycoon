using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    public List<Song> Songs = new List<Song>(); // TODO make visible in inspector

    public void Awake()
    {
        Instance = this;
    }
}
