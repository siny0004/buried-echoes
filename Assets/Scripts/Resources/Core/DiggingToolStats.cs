
public class DiggingToolStats
{
    public int ShovelLevel { get; private set; } = 1;

    public float DigRadius => ShovelLevel switch
    {
        1 => 0.80f,
        2 => 0.90f,
        3 => 1.00f,
        4 => 1.10f,
        _ => 1.20f
    };

    public float DigStrength => ShovelLevel switch
    {
        1 => 180f,
        2 => 210f,
        3 => 240f,
        4 => 270f,
        _ => 300f
    };

    public float DigCooldown => ShovelLevel switch
    {
        1 => 0.65f,
        2 => 0.55f,
        3 => 0.45f,
        4 => 0.35f,
        _ => 0.25f
    };

    public int NextShovelUpgradeCost => ShovelLevel switch
    {
        1 => 50,
        2 => 100,
        3 => 250,
        4 => 500,
        _ => 0
    };

    public bool CanUpgradeShovel => ShovelLevel < 5;

    public bool TryUpgradeShovel()
    {
        if (!CanUpgradeShovel)
            return false;

        ShovelLevel++;
        return true;
    }
}
