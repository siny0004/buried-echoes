
using NUnit.Framework;
using BuriedEchoes.Resources;

public class PlayerWalletTests
{
    [Test]
    public void NewWallet_StartsWithZeroBalance()
    {
        PlayerWallet wallet = new PlayerWallet();

        Assert.AreEqual(0, wallet.Balance);
    }

    [Test]
    public void AddMoney_IncreasesBalance()
    {
        PlayerWallet wallet = new PlayerWallet();

        wallet.AddMoney(100);
        wallet.AddMoney(50);

        Assert.AreEqual(150, wallet.Balance);
    }

    [Test]
    public void AddMoney_ZeroOrNegativeAmount_DoesNothing()
    {
        PlayerWallet wallet = new PlayerWallet();

        wallet.AddMoney(100);
        wallet.AddMoney(0);
        wallet.AddMoney(-50);

        Assert.AreEqual(100, wallet.Balance);
    }

    [Test]
    public void TrySpend_EnoughMoney_ReturnsTrue()
    {
        PlayerWallet wallet = new PlayerWallet();
        wallet.AddMoney(100);

        bool result = wallet.TrySpend(40);

        Assert.IsTrue(result);
        Assert.AreEqual(60, wallet.Balance);
    }

    [Test]
    public void TrySpend_InsufficientMoney_ReturnsFalse()
    {
        PlayerWallet wallet = new PlayerWallet();
        wallet.AddMoney(50);

        bool result = wallet.TrySpend(100);

        Assert.IsFalse(result);
        Assert.AreEqual(50, wallet.Balance);
    }

    [Test]
    public void TrySpend_ZeroOrNegativeAmount_ReturnsFalse()
    {
        PlayerWallet wallet = new PlayerWallet();
        wallet.AddMoney(100);

        bool zeroResult = wallet.TrySpend(0);
        bool negativeResult = wallet.TrySpend(-20);

        Assert.IsFalse(zeroResult);
        Assert.IsFalse(negativeResult);
        Assert.AreEqual(100, wallet.Balance);
    }

    [Test]
    public void TrySpend_ExactBalance_LeavesZero()
    {
        PlayerWallet wallet = new PlayerWallet();
        wallet.AddMoney(100);

        bool result = wallet.TrySpend(100);

        Assert.IsTrue(result);
        Assert.AreEqual(0, wallet.Balance);
    }
}
