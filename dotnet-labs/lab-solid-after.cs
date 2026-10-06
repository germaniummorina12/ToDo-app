var order = new Order("Ada", "ada@example.com", "blackfriday", [new Line("keyboard", 50m, 2), new Line("Mouse", 20m, 1)]);

IDiscountRule[] rules = [new StudentDiscount(), new VipDiscount(), new BlackFridayDiscount()];

var processor = new OrderProcessor(new PriceCalculator(rules), new ConsoleOrderStore(), new ConsoleNotifier());
processor.Process(order);

var fake = new RecordingNotifier();
new OrderProcessor(new PriceCalculator(rules), new ConsoleOrderStore(), fake).Process(order);
Console.WriteLine($"Sent {fake.Sent.Count} emails, first: {fake.Sent[0]}");

record Line(string Product, decimal UnitPrice, int Quantity);
record Order(string Customer, string Email, string CustomerType, List<Line> Lines);

interface IDiscountRule
{
    bool AppliesTo(string customerType);
    decimal Apply(decimal subtotal);
}

sealed class StudentDiscount : IDiscountRule
{
    public bool AppliesTo(string customerType) => customerType == "student";
    public decimal Apply(decimal subtotal) => subtotal * 0.90m;
}

sealed class VipDiscount : IDiscountRule
{
    public bool AppliesTo(string customerType) => customerType == "vip";
    public decimal Apply(decimal subtotal) => subtotal * 0.80m;
}

sealed class BlackFridayDiscount : IDiscountRule
{
    public bool AppliesTo(string customerType) => customerType == "blackfriday";
    public decimal Apply(decimal subtotal) => subtotal * 0.75m;
}

sealed class PriceCalculator(IEnumerable<IDiscountRule> rules)
{
    public decimal Total(Order order)
    {
        var subtotal = order.Lines.Sum(l => l.UnitPrice * l.Quantity);
        return rules.FirstOrDefault(r => r.AppliesTo(order.CustomerType))?.Apply(subtotal) ?? subtotal;
    }
}

interface IOrderStore
{
    void Save(Order order, decimal total);
}

interface INotifier {  void OrderPlaced(Order order, decimal total); }

sealed class ConsoleOrderStore : IOrderStore
{
    public void Save(Order order, decimal total) => Console.WriteLine($"[DB]   saved order for {order.Customer}, total {total:F2}");
}

sealed class ConsoleNotifier : INotifier
{
    public void OrderPlaced(Order order, decimal total) => Console.WriteLine($"[EMAIL] to {order.Email}: your order total is {total:F2}");
}

sealed class RecordingNotifier : INotifier
{
    public List<string> Sent { get; } = [];
    public void OrderPlaced(Order order, decimal total) => Sent.Add($"[EMAIL] to {order.Email}: your order total is {total:F2}");
}

sealed class OrderProcessor(PriceCalculator prices, IOrderStore store, INotifier notifier)
{
    public void Process(Order order)
    {
        if (order.Lines.Count == 0) throw new InvalidOperationException("The order has no lines.");
        var total = prices.Total(order);
        store.Save(order, total);
        notifier.OrderPlaced(order, total);
    }
}