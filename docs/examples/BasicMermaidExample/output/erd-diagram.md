```mermaid
erDiagram

    Authors {
        string Id PK
        string Biography
        string Name
    }

    BookAuthors {
        string AuthorsId PK
        string BooksId PK
    }

    Books {
        string Id PK
        string CategoryId FK
        string ISBN
        string PublishedYear
        string Title
    }

    Categories {
        string Id PK
        string Name
    }

    Loans {
        string Id PK
        string BookId FK
        timestamp DueDate
        timestamp LoanDate
        string MemberId FK
        timestamp ReturnDate
    }

    Members {
        string Id PK
        string Email
        timestamp JoinDate
        string MembershipNumber
        string Name
    }

    Authors ||--o{ BookAuthors : "AuthorsId"
    Books ||--o{ BookAuthors : "BooksId"
    Categories ||--o{ Books : "Category"
    Books ||--o{ Loans : "Book"
    Members ||--o{ Loans : "Member"
```

*Generated: 2026-01-25 20:36:02 UTC*
