namespace Rilisoft
{
	public interface IMarketProduct
	{
		string Id { get; }

		string Title { get; }

		string Description { get; }

		string Price { get; }

		object PlatformProduct { get; }

		decimal PriceValue { get; }

		string Currency { get; }
	}
}
