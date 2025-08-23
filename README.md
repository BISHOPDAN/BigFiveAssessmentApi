# Big Five Personality Assessment API (.NET 8)

## ?? Project Description
The **Big Five Personality Assessment API** is a RESTful service that simulates a HR screening 
flow using the Big Five model. It accepts 20 Likert responses, scores the five traits (with
reverse-scored items), classifies results (Low/Moderate/High), stores submissions in SQLite,
and emails HTML reports to the candidate and TA team (via Papercut SMTP for local testing).

## ?? Features
- **Submit Assessment**: Accept 20 Likert responses (1–5) with candidate info.

- **Trait Scoring**: Reverse-scoring and scaling to 8–40; classification & descriptions.

- **Persistence**: Store submissions in SQLite via EF Core.

- **Email Reports**: Candidate summary + TA full breakdown (MailKit + Papercut).

- **Admin Tools**: List/filter submissions (with pagination) + CSV export.

- **API Docs**: Swagger UI for easy testing.

---

## ??? Project Structure
```
BigFiveAssessmentApi/
??? Controllers/                 # API Controllers (AssessmentsController)
??? Data/                        # EF Core DbContext (AppDbContext)
??? Dtos/                        # Request/Response DTOs
??? IRepository/                 # Interfaces (IScoringRepository, IEmailSender)
??? Repository/                  # Implementations (ScoringRepository, SmtpEmailSender)
??? Models/                      # Entities & enums (Submission, Trait)
??? Migrations/                  # EF Core migrations
??? Program.cs                   # App configuration & DI setup
??? appsettings.json             # Connection strings & Email config
??? README.md                    # Project documentation
```

---

## ??? Setup & Installation
### **1?? Prerequisites**
Ensure you have the following installed:
- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- (Windows) Papercut SMTP (local mail catcher)

### **2?? Clone Repository**
```sh
git clone https://github.com/BISHOPDAN/patient-management-api.git
cd patient-management-api
```

### **3?? Restore & Build**
```sh
dotnet restore
dotnet build
```

### **4?? Database Migration**
Ensure you have your database connection string set in `appsettings.json`, then run:
- Option A — .NET CLI
```sh
dotnet ef database update
```
- Option B — Visual Studio PMC
```sh
Update-Database
```

### **5?? Install & Start Papercut SMTP**

Papercut SMTP is a local SMTP server for testing emails (emails don’t go to the internet, they’re just captured locally).

#### ?? Windows
- Download Papercut SMTP Service zip (Windows x64) from:
[Papercut SMTP Releases](https://github.com/ChangemakerStudios/Papercut-SMTP/releases)
- Extract, then from that folder:
```sh
Papercut.Service.exe install
Papercut.Service.exe start
```
#### ?? Linux
- Download the Linux build from:
Papercut SMTP Releases

- Extract the tar.gz:
```sh
tar -xvzf Papercut.Smtp.Service.*-linux-x64.tar.gz
cd Papercut.Smtp.Service.*-linux-x64
```
- Run directly in foreground:
```sh
./Papercut.Service
```
- Or run as a background service:
```sh
nohup ./Papercut.Service > papercut.log 2>&1 &
```
#### ?? macOS
- Download the macOS build from:
Papercut SMTP Releases
- Extract the tar.gz:

- Extract the tar.gz:
```sh
tar -xvzf Papercut.Smtp.Service.*-osx-x64.tar.gz
cd Papercut.Smtp.Service.*-osx-x64
```
- Run directly:
```sh
./Papercut.Service
```
- Or run as a background service:
```sh
nohup ./Papercut.Service > papercut.log 2>&1 &
```

---

## ?? API Endpoints
| Method | Endpoint | Description |
|--------|----------|--------------|
| `POST` | `/api/Assessments/submit` | Accept assessment, score, store, email reports |
| `GET`  | `/api/assessments/dummy` | Generate a dummy valid request payload (20 random responses) |
| `GET`  | `/api/assessments/admin/submissions`| List submissions (filters + pagination) |
| `GET`  | `/api/assessments/admin/submissions/export` | Export all submissions as CSV |

---
#### Query parameters for admin/submissions

- email (contains, case-insensitive)

- from (ISO date/time)

- to (ISO date/time)

- page (default 1)

- pageSize (default 10)

---


## ?? Key Design Decisions
### **?? Clean Architecture**
- **Separation of Concerns**: Business logic is encapsulated in repositories
and as well data access, and API controllers handle request/response.
- **Dependency Injection**: Enhances testability and maintainability.

### **?? Database Choice**
- Chose **Sqlite** for its scalability as a simple level for testing, but the repository
layer is flexible to support **SQL Server** if needed.

## ?? Future Improvements
- ?Authentication & roles for admin endpoints

- ? Background email queue (e.g., hosted service)

- ? Per-question analytics and dashboard JSON

- ? Dockerfile & GitHub Actions CI

- ? Switchable SMTP providers (Gmail, Mailtrap, SendGrid)

---