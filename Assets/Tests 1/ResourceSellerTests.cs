
using NUnit.Framework;
using BuriedEchoes.Resources;
using ResourceType = BuriedEchoes.Resources.ResourceType;

public class ResourceSellerTests
{
    [Test]
    public void GetTotalValue_ReturnsCorrectValue()
    {
        ResourceInventory inventory = new ResourceInventory();
        ResourceSeller seller = new ResourceSeller();

        inventory.Add(ResourceType.Copper, 5);
        inventory.Add(ResourceType.Iron, 3);
        inventory.Add(ResourceType.Gold, 2);

        // 5*5 + 3*12 + 2*30 = 121
        int total = seller.GetTotalValue(inventory);

        Assert.AreEqual(121, total);
    }

    [Test]
    public void GetTotalValue_DoesNotRemoveResources()
    {
        ResourceInventory inventory = new ResourceInventory();
        ResourceSeller seller = new ResourceSeller();

        inventory.Add(ResourceType.Copper, 10);

        seller.GetTotalValue(inventory);

        Assert.AreEqual(10, inventory.GetAmount(ResourceType.Copper));
    }

    [Test]
    public void SellAll_ReturnsCorrectAmount()
    {
        ResourceInventory inventory = new ResourceInventory();
        ResourceSeller seller = new ResourceSeller();

        inventory.Add(ResourceType.Copper, 5);
        inventory.Add(ResourceType.Iron, 3);
        inventory.Add(ResourceType.Gold, 2);

        int earned = seller.SellAll(inventory);

        Assert.AreEqual(121, earned);
    }

    [Test]
    public void SellAll_RemovesAllSellableResources()
    {
        ResourceInventory inventory = new ResourceInventory();
        ResourceSeller seller = new ResourceSeller();

        inventory.Add(ResourceType.Copper, 5);
        inventory.Add(ResourceType.Iron, 3);
        inventory.Add(ResourceType.Gold, 2);

        seller.SellAll(inventory);

        Assert.AreEqual(0, inventory.GetAmount(ResourceType.Copper));
        Assert.AreEqual(0, inventory.GetAmount(ResourceType.Iron));
        Assert.AreEqual(0, inventory.GetAmount(ResourceType.Gold));
    }

    [Test]
    public void SellAll_EmptyInventory_ReturnsZero()
    {
        ResourceInventory inventory = new ResourceInventory();
        ResourceSeller seller = new ResourceSeller();

        int earned = seller.SellAll(inventory);

        Assert.AreEqual(0, earned);
    }

    [Test]
    public void SellAll_Twice_SecondSaleReturnsZero()
    {
        ResourceInventory inventory = new ResourceInventory();
        ResourceSeller seller = new ResourceSeller();

        inventory.Add(ResourceType.Gold, 2);

        int firstSale = seller.SellAll(inventory);
        int secondSale = seller.SellAll(inventory);

        Assert.AreEqual(60, firstSale);
        Assert.AreEqual(0, secondSale);
    }

    [Test]
    public void SellAll_NullInventory_ReturnsZero()
    {
        ResourceSeller seller = new ResourceSeller();

        int earned = seller.SellAll(null);

        Assert.AreEqual(0, earned);
    }

    [Test]
    public void GetTotalValue_NullInventory_ReturnsZero()
    {
        ResourceSeller seller = new ResourceSeller();

        int total = seller.GetTotalValue(null);

        Assert.AreEqual(0, total);
    }
}
