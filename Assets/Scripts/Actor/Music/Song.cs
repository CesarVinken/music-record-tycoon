public class Song
{
    public string Name;
    public Character Composer;

    public Song(string title, Character author)
    {
        Name = title;
        Composer = author;

        Composer.RecordedSongs.Add(this);
        MusicManager.Instance.Songs.Add(this);
    }

}
