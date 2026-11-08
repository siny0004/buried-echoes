
using BuriedEchoes.Resources;
using NUnit.Framework;

public class ResourceInventoryTests
{
    [Test]
    public void NewInventory_StartsEmpty()
    {
        // Arrange
        ResourceInventory inventory = new ResourceInventory();

        // Act
        int amount = inventory.GetAmount(ResourceType.Copper);

        // Assert
        Assert.AreEqual(0, amount);
    }

    [Test]
    public void Add_IncreasesResourceAmount()
    {
        // Arrange
        ResourceInventory inventory = new ResourceInventory();

        // Act
        inventory.Add(ResourceType.Copper, 5);
        Assert.AreEqual(5, inventory.GetAmount(ResourceType.Copper));

        // Assert
        Assert.AreEqual(5, inventory.GetAmount(ResourceType.Copper));
    }

    [Test]
    public void Add_DifferentResourcesRemainSeparate()
    {
        // Arrange
        ResourceInventory inventory = new ResourceInventory();

        // Act
        inventory.Add(ResourceType.Copper, 2);
        inventory.Add(ResourceType.Iron, 3);

        // Assert
        Assert.AreEqual(2, inventory.GetAmount(ResourceType.Copper));
        Assert.AreEqual(3, inventory.GetAmount(ResourceType.Iron));
        Assert.AreEqual(0, inventory.GetAmount(ResourceType.Gold));
    }

    [Test]
    public void Add_ZeroOrNegativeAmount_DoesNothing()
    {
        ResourceInventory inventory = new ResourceInventory();

        inventory.Add(ResourceType.Copper, 0);
        inventory.Add(ResourceType.Copper, -5);

        Assert.AreEqual(0, inventory.GetAmount(ResourceType.Copper));
    }

    [Test]
    public void TryRemove_EnoughResources_ReturnsTrue()
    {
        ResourceInventory inventory = new ResourceInventory();
        inventory.Add(ResourceType.Copper, 5);

        bool result = inventory.TryRemove(ResourceType.Copper, 4);

        Assert.IsTrue(result);
        Assert.AreEqual(1, inventory.GetAmount(ResourceType.Copper));
    }

    [Test]
    public void TryRemove_TooManyResources_ReturnsFalse()
    {
        ResourceInventory inventory = new ResourceInventory();
        inventory.Add(ResourceType.Copper, 5);

        bool result = inventory.TryRemove(ResourceType.Copper, 10);

        Assert.IsFalse(result);
        Assert.AreEqual(5, inventory.GetAmount(ResourceType.Copper));
    }

    [Test]
    public void TryRemove_ZeroOrNegativeAmount_ReturnsFalse()
    {
        ResourceInventory inventory = new ResourceInventory();
        inventory.Add(ResourceType.Copper, 5);

        Assert.IsFalse(inventory.TryRemove(ResourceType.Copper, 0));
        Assert.IsFalse(inventory.TryRemove(ResourceType.Copper, -1));

        Assert.AreEqual(5, inventory.GetAmount(ResourceType.Copper));
    }

    [Test]
    public void TryRemove_AllResources_LeavesZero()
    {
        ResourceInventory inventory = new ResourceInventory();
        inventory.Add(ResourceType.Gold, 5);

        bool result = inventory.TryRemove(ResourceType.Gold, 5);

        Assert.IsTrue(result);
        Assert.AreEqual(0, inventory.GetAmount(ResourceType.Gold));
    }

}
