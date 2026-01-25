```mermaid
erDiagram

    Categories {
        string Id PK
        string Description
        string Name
    }

    Customers {
        string Id PK
        timestamp CreatedAt
        string Email
        string FirstName
        string LastName
        string Phone
    }

    OrderItems {
        string Id PK
        string OrderId FK
        string ProductId FK
        string Quantity
        decimal TotalPrice
        decimal UnitPrice
    }

    Orders {
        string Id PK
        string CustomerId FK
        timestamp OrderDate
        string OrderNumber
        string Status
        decimal TotalAmount
    }

    Products {
        string Id PK
        string CategoryId FK
        string Description
        string Name
        decimal Price
        string SKU
        string StockQuantity
    }

    Reviews {
        string Id PK
        string Comment
        string CustomerId FK
        string ProductId FK
        string Rating
        timestamp ReviewDate
    }

    Orders ||--o{ OrderItems : "Order"
    Products ||--o{ OrderItems : "Product"
    Customers ||--o{ Orders : "Customer"
    Categories ||--o{ Products : "Category"
    Customers ||--o{ Reviews : "Customer"
    Products ||--o{ Reviews : "Product"
```

*Generated: 2026-01-25 21:09:16 UTC*
