# Database Schema

**Generated:** 2026-01-25 21:09:16 UTC

**DbContext:** `ECommerceContext`

## Database Information

- **Schema:** public (PostgreSQL)

## Statistics

- **Total Tables:** 6
- **Total Foreign Keys:** 6
- **Total Indexes:** 10

## Table of Contents

- [Categories](#Categories)
- [Customers](#Customers)
- [OrderItems](#OrderItems)
- [Orders](#Orders)
- [Products](#Products)
- [Reviews](#Reviews)

## Tables

### Categories

**Schema:** `public`

**CLR Type:** `Category`

#### Columns

| Column | Type | Nullable | Default | Description |
|--------|------|----------|---------|-------------|
| `Id` | `int` | No | `` | PK, OnAdd |
| `Description` | `nvarchar(500)` | Yes | `` |  |
| `Name` | `nvarchar(100)` | No | `` |  |

#### Primary Key

- **PK_Categories**: `Id`

#### Indexes

- **IX_Categories_Name**
  - Columns: `Name`
  - Type: Unique

#### Relationships

- **Products** → `Products` (One-to-Many)

---

### Customers

**Schema:** `public`

**CLR Type:** `Customer`

#### Columns

| Column | Type | Nullable | Default | Description |
|--------|------|----------|---------|-------------|
| `Id` | `int` | No | `` | PK, OnAdd |
| `CreatedAt` | `datetime2` | No | `` |  |
| `Email` | `nvarchar(255)` | No | `` |  |
| `FirstName` | `nvarchar(100)` | No | `` |  |
| `LastName` | `nvarchar(100)` | No | `` |  |
| `Phone` | `nvarchar(20)` | Yes | `` |  |

#### Primary Key

- **PK_Customers**: `Id`

#### Indexes

- **IX_Customers_Email**
  - Columns: `Email`
  - Type: Unique

#### Relationships

- **Orders** → `Orders` (One-to-Many)
- **Reviews** → `Reviews` (One-to-Many)

---

### OrderItems

**Schema:** `public`

**CLR Type:** `OrderItem`

#### Columns

| Column | Type | Nullable | Default | Description |
|--------|------|----------|---------|-------------|
| `Id` | `int` | No | `` | PK, OnAdd |
| `OrderId` | `int` | No | `` | FK |
| `ProductId` | `int` | No | `` | FK |
| `Quantity` | `int` | No | `` |  |
| `TotalPrice` | `decimal(18,2)` | No | `` |  |
| `UnitPrice` | `decimal(18,2)` | No | `` |  |

#### Primary Key

- **PK_OrderItems**: `Id`

#### Foreign Keys

- **FK_OrderItems_Orders_OrderId**
  - Columns: `OrderId`
  - References: `Orders`(`Id`)
  - On Delete: `Cascade`
- **FK_OrderItems_Products_ProductId**
  - Columns: `ProductId`
  - References: `Products`(`Id`)
  - On Delete: `Restrict`

#### Indexes

- **IX_OrderItems_OrderId**
  - Columns: `OrderId`
  - Type: Non-unique
- **IX_OrderItems_ProductId**
  - Columns: `ProductId`
  - Type: Non-unique

#### Relationships

- **Order** → `Orders` (Many-to-One)
- **Product** → `Products` (Many-to-One)

---

### Orders

**Schema:** `public`

**CLR Type:** `Order`

#### Columns

| Column | Type | Nullable | Default | Description |
|--------|------|----------|---------|-------------|
| `Id` | `int` | No | `` | PK, OnAdd |
| `CustomerId` | `int` | No | `` | FK |
| `OrderDate` | `datetime2` | No | `` |  |
| `OrderNumber` | `nvarchar(20)` | No | `` |  |
| `Status` | `nvarchar(20)` | No | `` |  |
| `TotalAmount` | `decimal(18,2)` | No | `` |  |

#### Primary Key

- **PK_Orders**: `Id`

#### Foreign Keys

- **FK_Orders_Customers_CustomerId**
  - Columns: `CustomerId`
  - References: `Customers`(`Id`)
  - On Delete: `Cascade`

#### Indexes

- **IX_Orders_CustomerId**
  - Columns: `CustomerId`
  - Type: Non-unique
- **IX_Orders_OrderNumber**
  - Columns: `OrderNumber`
  - Type: Unique

#### Relationships

- **Customer** → `Customers` (Many-to-One)
- **OrderItems** → `OrderItems` (One-to-Many)

---

### Products

**Schema:** `public`

**CLR Type:** `Product`

#### Columns

| Column | Type | Nullable | Default | Description |
|--------|------|----------|---------|-------------|
| `Id` | `int` | No | `` | PK, OnAdd |
| `CategoryId` | `int` | No | `` | FK |
| `Description` | `nvarchar(1000)` | Yes | `` |  |
| `Name` | `nvarchar(200)` | No | `` |  |
| `Price` | `decimal(18,2)` | No | `` |  |
| `SKU` | `nvarchar(50)` | No | `` |  |
| `StockQuantity` | `int` | No | `` |  |

#### Primary Key

- **PK_Products**: `Id`

#### Foreign Keys

- **FK_Products_Categories_CategoryId**
  - Columns: `CategoryId`
  - References: `Categories`(`Id`)
  - On Delete: `Restrict`

#### Indexes

- **IX_Products_CategoryId**
  - Columns: `CategoryId`
  - Type: Non-unique
- **IX_Products_SKU**
  - Columns: `SKU`
  - Type: Unique

#### Relationships

- **Category** → `Categories` (Many-to-One)
- **OrderItems** → `OrderItems` (One-to-Many)
- **Reviews** → `Reviews` (One-to-Many)

---

### Reviews

**Schema:** `public`

**CLR Type:** `Review`

#### Columns

| Column | Type | Nullable | Default | Description |
|--------|------|----------|---------|-------------|
| `Id` | `int` | No | `` | PK, OnAdd |
| `Comment` | `nvarchar(1000)` | Yes | `` |  |
| `CustomerId` | `int` | No | `` | FK |
| `ProductId` | `int` | No | `` | FK |
| `Rating` | `int` | No | `` |  |
| `ReviewDate` | `datetime2` | No | `` |  |

#### Primary Key

- **PK_Reviews**: `Id`

#### Foreign Keys

- **FK_Reviews_Customers_CustomerId**
  - Columns: `CustomerId`
  - References: `Customers`(`Id`)
  - On Delete: `Restrict`
- **FK_Reviews_Products_ProductId**
  - Columns: `ProductId`
  - References: `Products`(`Id`)
  - On Delete: `Cascade`

#### Indexes

- **IX_Reviews_CustomerId**
  - Columns: `CustomerId`
  - Type: Non-unique
- **IX_Reviews_ProductId**
  - Columns: `ProductId`
  - Type: Non-unique

#### Relationships

- **Customer** → `Customers` (Many-to-One)
- **Product** → `Products` (Many-to-One)

---

