
namespace BuriedEchoes.Resources
{
    public class PlayerWallet
    {
        public int Balance { get; private set; }

        public void AddMoney(int amount)
        {
            if (amount <= 0)
                return;

            Balance += amount;
        }

        public bool TrySpend(int amount)
        {
            if (amount <= 0 || Balance < amount)
                return false;

            Balance -= amount;
            return true;
        }
    }
}
