using UnityEngine;

public class InteractionStep
{
    public InteractionSequenceLine InteractionSequenceLine;
    public GameObject InteractionSequenceLineGO;
    public int Duration;

    private InteractionStep(int duration)
    {
        Duration = duration;
    }

    public static InteractionStep Create(int duration = 3000)
    {
        return new InteractionStep(duration);
    }

    public InteractionStep WithSequenceLine(string line)
    {
        InteractionSequenceLine = new InteractionSequenceLine(line);
        return this;
    }

    public bool HasSequenceLine()
    {
        if (InteractionSequenceLine == null) return false;
        return true;
    }

    public void CleanUp()
    {
        OnScreenTextContainer.Instance.DeleteInteractionSequenceLine(InteractionSequenceLineGO);
    }
}
