CREATE TABLE Categories (Id integer primary key, Name text not null unique);
CREATE TABLE Products (
	Id integer primary key,
	Sku text not null unique,
	Name text not null,
	CategoryId integer not null references Categories(Id),
	UnitPrice numeric(10,2) not null,
	StockOnHand integer not null,
	ReorderLevel integer not null);
CREATE TABLE Customers(
	Id integer primary key,
	Name text not null
);
CREATE TABLE Orders (
	Id integer primary key,
	CustomerId integer not null references Customers(Id),
	PlacedAt date not null, 
	Status text not null
);
CREATE TABLE OrderLines(
	Id integer primary key,
	OrderId integer not null references Orders(Id),
	ProductId integer not null references Products(Id),
	Quantity integer not null,
	UnitPrice numeric(10,2) not null
);

INSERT INTO Categories Values 
(1, 'Peripherals'),
(2, 'Components'),
(3, 'Laptops'),
(4, 'Desktops');
INSERT INTO Products Values 
(1, 'MOU-001', 'Mouse', 1, 10.00, 100, 10),
(2, 'KEY-001', 'Keyboard', 1, 20.00, 100, 10),
(3, 'MON-001', 'Monitor', 1, 150.00, 100, 10),
(4, 'CPU-001', 'CPU', 2, 200.00, 100, 10),
(5, 'GPU-001', 'GPU', 2, 300.00, 100, 10),
(6, 'RAM-001', 'RAM', 2, 50.00, 100, 10),
(7, 'LAP-001', 'Laptop', 3, 500.00, 100, 10),
(8, 'DESK-001', 'Desktop', 4, 400.00, 100, 10);
INSERT INTO Customers Values 
(1, 'Acme Corp'),
(2, 'Globex Ltd'),
(3, 'Initech');
INSERT INTO Orders Values 
(1, 1, '2024-01-01', 'Placed'),
(2, 2, '2024-01-02', 'Placed'),
(3, 3, '2024-01-03', 'Placed'),
(4, 1, '2024-01-04', 'Cancelled');

INSERT INTO OrderLines Values 
(1, 1, 1, 5, 10.00),
(2, 1, 2, 2, 20.00),
(3, 2, 3, 1, 150.00),
(4, 3, 4, 1, 200.00);