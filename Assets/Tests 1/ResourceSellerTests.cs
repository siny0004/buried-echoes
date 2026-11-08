using NUnit.Framework;
using BuriedEchoes.Resources;

public class ResourceSellerTests
{
    [Test]
    public void GetTotalValue_ReturnsCorrectValue()
    {
        ResourceInventory inventory = new ResourceInventory();
        ResourceSeller seller = new ResourceSeller();

        inventory.Add(ResourceType.Copper, 2);
        inventory.Add(ResourceType.Iron, 1);
        inventory.Add(ResourceType.Gold, 2);

        // 2*10 + 1*5 + 2*40 = 105
        int total = seller.GetTotalValue(inventory);

        Assert.AreEqual(105, total);
    }

    [Test]
    public void GetTotalValue_DoesNotRemoveResources()
    {
        ResourceInventory inventory = new ResourceInventory();
        ResourceSeller seller = new ResourceSeller();

        inventory.Add(ResourceType.Copper, 5);

        seller.GetTotalValue(inventory);

        Assert.AreEqual(5, inventory.GetAmount(ResourceType.Copper));
    }

    [Test]
    public void SellAll_ReturnsCorrectAmount()
    {
        ResourceInventory inventory = new ResourceInventory();
        ResourceSeller seller = new ResourceSeller();

        inventory.Add(ResourceType.Copper, 2);
        inventory.Add(ResourceType.Iron, 1);
        inventory.Add(ResourceType.Gold, 2);

        int earned = seller.SellAll(inventory);

        Assert.AreEqual(105, earned);
    }

    [Test]
    public void SellAll_RemovesAllSellableResources()
    {
        ResourceInventory inventory = new ResourceInventory();
        ResourceSeller seller = new ResourceSeller();

        inventory.Add(ResourceType.Copper, 2);
        inventory.Add(ResourceType.Iron, 1);
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

        Assert.AreEqual(80, firstSale);
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
