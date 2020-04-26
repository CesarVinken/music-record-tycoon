﻿public class Musician : Character, IPerform, IPractice
{
    public void PerformObjectInteraction()
    {
        PossibleObjectInteractions.Add(ObjectInteractionType.Perform);
    }

    public void PracticeObjectInteraction()
    {
        PossibleObjectInteractions.Add(ObjectInteractionType.Practice);
    }

    public void Start()
    {
        PerformObjectInteraction();
        PracticeObjectInteraction();
    }
}
