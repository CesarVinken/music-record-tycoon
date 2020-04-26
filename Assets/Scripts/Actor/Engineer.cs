public class Engineer : Character, IRepair, IRecord
{
    public void RecordObjectInteraction()
    {
        PossibleObjectInteractions.Add(ObjectInteractionType.Record);
    }

    public void RepairObjectInteraction()
    {
        PossibleObjectInteractions.Add(ObjectInteractionType.Repair);
    }

    public void Start()
    {
        RecordObjectInteraction();
        RepairObjectInteraction();
    }
}

