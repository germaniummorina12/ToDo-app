var line = new BasketLine("Keyboard", UnitPrice: 50m, Quantity: 12);

IDiscountPolicy[] policies = [
    new NoDiscount(),
    new PercentageDiscount(10),
    new BulkDiscount(minQuantity: 10, percentage: 15)
    ];

foreach (var policy in policies)
{
    Console.WriteLine($"{policy.Name, -22} total = {policy.Apply(line), 8:F2}");
}

static decimal Cheapest(BasketLine line, IEnumerable<IDiscountPolicy> policies) =>
    policies.Min(policy => policy.Apply(line));

Console.WriteLine($"Best price: {Cheapest(line, policies):F2}");


record BasketLine(string Product, decimal UnitPrice, int Quantity)
{
    public decimal Subtotal => UnitPrice * Quantity;
}

interface IDiscountPolicy
{
    string Name { get; }
    decimal Apply(BasketLine line);
}

sealed class NoDiscount : IDiscountPolicy
{
    public string Name => "No Discount";
    public decimal Apply(BasketLine line) => line.Subtotal;
}

sealed class PercentageDiscount(decimal percentage) : IDiscountPolicy
{
    public string Name => $"{percentage}% Discount";
    public decimal Apply(BasketLine line) => line.Subtotal * (1 - percentage / 100m);
}

sealed class BulkDiscount(int minQuantity, decimal percentage) : IDiscountPolicy
{
    public string Name => $"Bulk Discount: {percentage}% off for {minQuantity}+ items";
    public decimal Apply(BasketLine line) => line.Quantity >= minQuantity ? line.Subtotal * (1 - percentage / 100m) : line.Subtotal;
}