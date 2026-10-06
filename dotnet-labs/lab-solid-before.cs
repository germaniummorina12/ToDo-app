var order = new Order("Adda", "ada@exmapl.com", "student", [new Line("keyboard", 50m, 2), new Line("Mouse", 20m, 1)]);

new OrderProcessor().Process(order);

record Line(string Product, decimal UnitPrice, int Quantity);
record Order(string Customer, string Email, string CustomerType, List<Line> Lines);

class OrderProcessor
{
    public void Process(Order order)
    {
        if (order.Lines.Count == 0) throw new InvalidOperationException("The order has no lines.");

        decimal subtotal = 0;
        foreach(var line in order.Lines) subtotal+= line.UnitPrice * line.Quantity;

        decimal total = subtotal;
        if (order.CustomerType == "student") total = subtotal * 0.90m;
        else if (order.CustomerType == "vip") total = subtotal * 0.80m;

        Console.WriteLine($"[DB]   saved order for {order.Customer}, total {total:F2}");

        Console.WriteLine($"[EMAIL] to {order.Email}: your order total is {total:F2}");
    }
}