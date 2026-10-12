Console.WriteLine("--- 1. Constructors: the base runs first ---");
var savings = new SavingsAccount("Maya", 300m, interestRate: 0.02m);
var checking = new CheckingAccount("Linus", 200m, overdraftLimit: 300m);

Console.WriteLine();
Console.WriteLine("--- 2. One call, the real type decides ---");
Account[] accounts = [savings, checking];
foreach (var account in accounts)
{
    account.MonthEnd();
    Console.WriteLine(account);
}

Console.WriteLine();
Console.WriteLine("--- 3. The base keeps the sequence, the subclass changes one rule ---");
foreach (var account in accounts)
{
    bool ok = account.TryWithdraw(400m);
    Console.WriteLine($"{account.Owner, -6} withdraw 400: {(ok ? "ok" : "refused"), -7} balance: {account.Balance:F2}");
}

Console.WriteLine();
Console.WriteLine("--- 4. override versus new ---");
Account asBase = checking;
Console.WriteLine($"asBase.Kind: {asBase.Kind}");
Console.WriteLine($"checking.Kind: {checking.Kind}");
Console.WriteLine($"asBase.Code: {asBase.Code}");
Console.WriteLine($"checking.Code: {checking.Code}");

Console.WriteLine();
Console.WriteLine("--- 5. Asking for the real type ---");
foreach(var account in accounts)
{
    if (account is CheckingAccount c)
    {
        Console.WriteLine($"{account.Owner} may go {c.OverdraftLimit:F2} below zero");
    }
    else
    {
        Console.WriteLine($"{account.Owner} may not go below zero");
    }
}

var chain = new List<string>();
for (Type? t = savings.GetType(); t is not null; t = t.BaseType)
{
    chain.Add(t.Name);
}
Console.WriteLine($"Inheritance chain for SavingsAccount: {string.Join(" -> ", chain)}");

abstract class Account
{
        public string Owner { get; }
        public decimal Balance { get; protected set; }

    protected Account (string owner, decimal openingBalance)
    {
        Owner = owner;
        Balance = openingBalance;
        Console.WriteLine($"Account constructor: Owner = {Owner}");
    }

    public bool TryWithdraw(decimal amount)
    {
        if (amount <= 0 || !CanWithdraw(amount)) return false;
        Balance -= amount;
        return true;
    }

    protected virtual bool CanWithdraw(decimal amount) => amount <= Balance;

    public abstract void MonthEnd();

    public virtual string Kind => "Account";
    public virtual string Code => "ACC";
    public override string ToString() => $"{Kind, -9}{Owner, -6}{Balance,9:F2}";
}

sealed class SavingsAccount : Account
{
    private readonly decimal _interestRate;
    public SavingsAccount(string owner, decimal openingBalance, decimal interestRate)
        : base(owner, openingBalance)
    {
        _interestRate = interestRate;
        Console.WriteLine($"   SavingsAccount constructor for {owner}");
    }
    public override void MonthEnd() => Balance += Balance * _interestRate;
    public override string Kind => "Savings";
}

sealed class CheckingAccount : Account
{
    public decimal OverdraftLimit { get; }

    public CheckingAccount(string owner, decimal openingBalance, decimal overdraftLimit)
        : base(owner, openingBalance)
    {
        OverdraftLimit = overdraftLimit;
        Console.WriteLine($"   CheckingAccount constructor for {owner}");
    }

    protected override bool CanWithdraw(decimal amount) => amount <= Balance + OverdraftLimit;
    public override void MonthEnd() => Balance -= 2m;
    public override string Kind => "Checking";
    public new string Code => "CHK";
}