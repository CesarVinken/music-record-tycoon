public class ConsoleLine
{
    public string Text;
    public bool IsPlayerInput;

    public ConsoleLine(string text, bool isPlayerInput = false)
    {
        Text = text;
        IsPlayerInput = isPlayerInput;
    }
}
