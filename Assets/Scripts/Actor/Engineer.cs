public class Engineer : Character, IRepair
{
    public void RepairObjectInteraction()
    {
        PossibleObjectInteractions.Add(ObjectInteractionType.Repair);
    }

    public void Start()
    {
        RepairObjectInteraction();
    }
}

