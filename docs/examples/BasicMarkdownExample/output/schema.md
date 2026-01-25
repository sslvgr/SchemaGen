# Database Schema

**Generated:** 2026-01-25 21:05:12 UTC

**DbContext:** `BlogContext`

## Database Information

- **Schema:** public (PostgreSQL)

## Statistics

- **Total Tables:** 3
- **Total Foreign Keys:** 2
- **Total Indexes:** 4

## Table of Contents

- [Authors](#Authors)
- [Blogs](#Blogs)
- [Posts](#Posts)

## Tables

### Authors

**Schema:** `public`

**CLR Type:** `Author`

#### Columns

| Column | Type | Nullable | Default | Description |
|--------|------|----------|---------|-------------|
| `Id` | `int` | No | `` | PK, OnAdd |
| `Email` | `nvarchar(255)` | No | `` |  |
| `Name` | `nvarchar(100)` | No | `` |  |

#### Primary Key

- **PK_Authors**: `Id`

#### Indexes

- **IX_Authors_Email**
  - Columns: `Email`
  - Type: Unique

#### Relationships

- **Posts** → `Posts` (One-to-Many)

---

### Blogs

**Schema:** `public`

**CLR Type:** `Blog`

#### Columns

| Column | Type | Nullable | Default | Description |
|--------|------|----------|---------|-------------|
| `Id` | `int` | No | `` | PK, OnAdd |
| `CreatedAt` | `datetime2` | No | `` |  |
| `Description` | `nvarchar(500)` | Yes | `` |  |
| `Title` | `nvarchar(200)` | No | `` |  |

#### Primary Key

- **PK_Blogs**: `Id`

#### Indexes

- **IX_Blogs_Title**
  - Columns: `Title`
  - Type: Unique

#### Relationships

- **Posts** → `Posts` (One-to-Many)

---

### Posts

**Schema:** `public`

**CLR Type:** `Post`

#### Columns

| Column | Type | Nullable | Default | Description |
|--------|------|----------|---------|-------------|
| `Id` | `int` | No | `` | PK, OnAdd |
| `AuthorId` | `int` | No | `` | FK |
| `BlogId` | `int` | No | `` | FK |
| `Content` | `nvarchar(max)` | No | `` |  |
| `CreatedAt` | `datetime2` | No | `CURRENT_TIMESTAMP` | OnAdd |
| `Title` | `nvarchar(300)` | No | `` |  |

#### Primary Key

- **PK_Posts**: `Id`

#### Foreign Keys

- **FK_Posts_Authors_AuthorId**
  - Columns: `AuthorId`
  - References: `Authors`(`Id`)
  - On Delete: `Restrict`
- **FK_Posts_Blogs_BlogId**
  - Columns: `BlogId`
  - References: `Blogs`(`Id`)
  - On Delete: `Cascade`

#### Indexes

- **IX_Posts_AuthorId**
  - Columns: `AuthorId`
  - Type: Non-unique
- **IX_Posts_BlogId**
  - Columns: `BlogId`
  - Type: Non-unique

#### Relationships

- **Author** → `Authors` (Many-to-One)
- **Blog** → `Blogs` (Many-to-One)

---

