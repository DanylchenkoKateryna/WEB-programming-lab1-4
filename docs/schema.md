# Database Schema — Project Manager (Variant 4)

## Entity-Relationship Diagram

```mermaid
erDiagram

    AspNetUsers {
        nvarchar Id PK
        nvarchar UserName
        nvarchar NormalizedUserName
        nvarchar Email
        nvarchar NormalizedEmail
        nvarchar PasswordHash
        nvarchar DisplayName
    }

    Employees {
        int Id PK
        nvarchar FirstName
        nvarchar LastName
        nvarchar Email
        nvarchar Position
        nvarchar Department
        datetime2 HireDate
        decimal Salary
        nvarchar UserId FK "nullable → AspNetUsers.Id"
    }

    Projects {
        int Id PK
        nvarchar Name
        nvarchar Description
        int Status "enum: Planning/Active/OnHold/Completed/Cancelled"
        int Priority "enum: Low/Medium/High/Critical"
        datetime2 StartDate
        datetime2 EndDate "nullable"
        decimal Budget
        nvarchar ClientName "nullable"
    }

    EmployeeProjects {
        int EmployeeId PK,FK
        int ProjectId PK,FK
        nvarchar Role
        datetime2 AssignedAt
    }

    ChatMessages {
        int Id PK
        int ProjectId FK
        nvarchar SenderUserName
        nvarchar SenderDisplayName
        nvarchar Content
        nvarchar FileUrl "nullable"
        nvarchar FileName "nullable"
        nvarchar RecipientUserName "nullable — null = public, value = private"
        datetime2 SentAt
    }

    AspNetUsers ||--o{ Employees : "linked account (nullable)"
    Employees ||--o{ EmployeeProjects : "works on"
    Projects ||--o{ EmployeeProjects : "has team members"
    Projects ||--o{ ChatMessages : "has messages"
```

## Tables Summary

| Table | Description |
|---|---|
| `AspNetUsers` | ASP.NET Core Identity users (extended with `DisplayName`) |
| `Employees` | Company employees; optional link to a user account via `UserId` |
| `Projects` | Projects with status, priority, budget, dates |
| `EmployeeProjects` | Many-to-many join: which employees work on which projects and in what role |
| `ChatMessages` | Per-project chat; `RecipientUserName = null` → public broadcast, otherwise private DM |

## Relationships

```
AspNetUsers ──── Employees          (1 : 0..1)  optional account link, ON DELETE SET NULL
Employees ──── EmployeeProjects     (1 : N)     employee can work on many projects
Projects ──── EmployeeProjects      (1 : N)     project can have many employees
Projects ──── ChatMessages          (1 : N)     project can have many chat messages
```

## Composite Primary Key

`EmployeeProjects` uses a composite PK `(EmployeeId, ProjectId)` — defined in `AppDbContext.OnModelCreating`:
```csharp
builder.Entity<EmployeeProject>()
    .HasKey(ep => new { ep.EmployeeId, ep.ProjectId });
```

## Identity tables (managed by ASP.NET Core Identity)

The following tables are created automatically by `IdentityDbContext`:

| Table | Purpose |
|---|---|
| `AspNetUsers` | User accounts |
| `AspNetRoles` | Roles (`Admin`, `User`, …) |
| `AspNetUserRoles` | User ↔ Role mapping |
| `AspNetUserClaims` | Per-user claims |
| `AspNetRoleClaims` | Per-role claims |
| `AspNetUserLogins` | External login providers |
| `AspNetUserTokens` | Refresh / access tokens |
