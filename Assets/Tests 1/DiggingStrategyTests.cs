
using NUnit.Framework;

public class DiggingStrategyTests
{
    [Test]
    public void ShovelStrategy_UsesStartingShovelStats()
    {
        DiggingToolStats stats = new DiggingToolStats();
        IDiggingStrategy strategy = new ShovelStrategy(stats);

        Assert.AreEqual(0.80f, strategy.DigRadius, 0.001f);
        Assert.AreEqual(180f, strategy.DigStrength, 0.001f);
        Assert.AreEqual(0.65f, strategy.DigCooldown, 0.001f);
    }

    [Test]
    public void ShovelStrategy_ReflectsShovelUpgrades()
    {
        DiggingToolStats stats = new DiggingToolStats();
        IDiggingStrategy strategy = new ShovelStrategy(stats);

        stats.TryUpgradeShovel();

        Assert.AreEqual(0.90f, strategy.DigRadius, 0.001f);
        Assert.AreEqual(210f, strategy.DigStrength, 0.001f);
        Assert.AreEqual(0.55f, strategy.DigCooldown, 0.001f);
    }

    [Test]
    public void ShovelStrategy_ReflectsMaximumLevel()
    {
        DiggingToolStats stats = new DiggingToolStats();
        IDiggingStrategy strategy = new ShovelStrategy(stats);

        for (int i = 0; i < 4; i++)
        {
            stats.TryUpgradeShovel();
        }

        Assert.AreEqual(1.20f, strategy.DigRadius, 0.001f);
        Assert.AreEqual(300f, strategy.DigStrength, 0.001f);
        Assert.AreEqual(0.25f, strategy.DigCooldown, 0.001f);
    }

    [Test]
    public void DrillStrategy_HasCorrectDiggingStats()
    {
        IDiggingStrategy strategy = new DrillStrategy();

        Assert.AreEqual(0.75f, strategy.DigRadius, 0.001f);
        Assert.AreEqual(100f, strategy.DigStrength, 0.001f);
        Assert.AreEqual(0.15f, strategy.DigCooldown, 0.001f);
    }

    [Test]
    public void DrillStrategy_IsUnaffectedByShovelUpgrades()
    {
        DiggingToolStats stats = new DiggingToolStats();
        IDiggingStrategy drill = new DrillStrategy();

        for (int i = 0; i < 4; i++)
        {
            stats.TryUpgradeShovel();
        }

        Assert.AreEqual(0.75f, drill.DigRadius, 0.001f);
        Assert.AreEqual(100f, drill.DigStrength, 0.001f);
        Assert.AreEqual(0.15f, drill.DigCooldown, 0.001f);
    }

    [Test]
    public void SwitchingStrategies_PreservesShovelUpgrades()
    {
        DiggingToolStats stats = new DiggingToolStats();

        IDiggingStrategy shovel = new ShovelStrategy(stats);
        IDiggingStrategy drill = new DrillStrategy();

        // Upgrade the shovel
        stats.TryUpgradeShovel();
        stats.TryUpgradeShovel();

        // Switch to drill
        IDiggingStrategy activeStrategy = drill;

        Assert.AreEqual(0.75f, activeStrategy.DigRadius, 0.001f);

        // Switch back to shovel
        activeStrategy = shovel;

        Assert.AreEqual(1.00f, activeStrategy.DigRadius, 0.001f);
        Assert.AreEqual(240f, activeStrategy.DigStrength, 0.001f);
        Assert.AreEqual(0.45f, activeStrategy.DigCooldown, 0.001f);
        Assert.AreEqual(3, stats.ShovelLevel);
    }
}
