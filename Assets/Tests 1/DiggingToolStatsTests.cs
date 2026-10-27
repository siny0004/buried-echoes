
using NUnit.Framework;

public class DiggingToolStatsTests
{
    [Test]
    public void NewShovel_StartsAtLevelOne()
    {
        DiggingToolStats stats = new DiggingToolStats();

        Assert.AreEqual(1, stats.ShovelLevel);
        Assert.IsTrue(stats.CanUpgradeShovel);
    }

    [Test]
    public void NewShovel_HasCorrectStartingStats()
    {
        DiggingToolStats stats = new DiggingToolStats();

        Assert.AreEqual(0.80f, stats.DigRadius, 0.001f);
        Assert.AreEqual(180f, stats.DigStrength, 0.001f);
        Assert.AreEqual(0.65f, stats.DigCooldown, 0.001f);
    }

    [Test]
    public void TryUpgradeShovel_IncreasesLevel()
    {
        DiggingToolStats stats = new DiggingToolStats();

        bool result = stats.TryUpgradeShovel();

        Assert.IsTrue(result);
        Assert.AreEqual(2, stats.ShovelLevel);
    }

    [Test]
    public void TryUpgradeShovel_ChangesDiggingStats()
    {
        DiggingToolStats stats = new DiggingToolStats();

        stats.TryUpgradeShovel();

        Assert.AreEqual(0.90f, stats.DigRadius, 0.001f);
        Assert.AreEqual(210f, stats.DigStrength, 0.001f);
        Assert.AreEqual(0.55f, stats.DigCooldown, 0.001f);
    }

    [Test]
    public void UpgradeCosts_AreCorrectAtEveryLevel()
    {
        DiggingToolStats stats = new DiggingToolStats();

        Assert.AreEqual(50, stats.NextShovelUpgradeCost);

        stats.TryUpgradeShovel();
        Assert.AreEqual(100, stats.NextShovelUpgradeCost);

        stats.TryUpgradeShovel();
        Assert.AreEqual(200, stats.NextShovelUpgradeCost);

        stats.TryUpgradeShovel();
        Assert.AreEqual(400, stats.NextShovelUpgradeCost);

        stats.TryUpgradeShovel();
        Assert.AreEqual(0, stats.NextShovelUpgradeCost);
    }

    [Test]
    public void MaximumLevel_HasCorrectStats()
    {
        DiggingToolStats stats = new DiggingToolStats();

        for (int i = 0; i < 4; i++)
        {
            stats.TryUpgradeShovel();
        }

        Assert.AreEqual(5, stats.ShovelLevel);
        Assert.AreEqual(1.20f, stats.DigRadius, 0.001f);
        Assert.AreEqual(300f, stats.DigStrength, 0.001f);
        Assert.AreEqual(0.25f, stats.DigCooldown, 0.001f);
        Assert.IsFalse(stats.CanUpgradeShovel);
    }

    [Test]
    public void TryUpgradeShovel_AtMaximumLevel_ReturnsFalse()
    {
        DiggingToolStats stats = new DiggingToolStats();

        for (int i = 0; i < 4; i++)
        {
            stats.TryUpgradeShovel();
        }

        bool result = stats.TryUpgradeShovel();

        Assert.IsFalse(result);
        Assert.AreEqual(5, stats.ShovelLevel);
    }

    [Test]
    public void TryUpgradeShovel_CannotExceedMaximumLevel()
    {
        DiggingToolStats stats = new DiggingToolStats();

        for (int i = 0; i < 10; i++)
        {
            stats.TryUpgradeShovel();
        }

        Assert.AreEqual(5, stats.ShovelLevel);
        Assert.AreEqual(1.20f, stats.DigRadius, 0.001f);
        Assert.AreEqual(300f, stats.DigStrength, 0.001f);
    }
}
