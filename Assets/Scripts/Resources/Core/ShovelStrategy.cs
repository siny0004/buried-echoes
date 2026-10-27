
public class ShovelStrategy : IDiggingStrategy
{
    private readonly DiggingToolStats toolStats;

    public ShovelStrategy(DiggingToolStats toolStats)
    {
        this.toolStats = toolStats;
    }

    public float DigRadius => toolStats.DigRadius;

    public float DigStrength => toolStats.DigStrength;

    public float DigCooldown => toolStats.DigCooldown;
}
