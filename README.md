# Razor Attendance System

A web-based attendance management system built with ASP.NET Core Razor Pages. It utilizes Entity Framework Core for data access, employing SQLite in the development environment and MySQL in production. The system allows configuration of early and late attendance times to suit organizational policies.

## 📋 Features

- **User Authentication**: Secure login for administrators and users.
- **Attendance Tracking**: Record and monitor attendance with time-based validations.
- **Configurable Settings**: Define early and late attendance thresholds.
- **Responsive UI**: User-friendly interface built with Razor Pages.

## 🛠️ Technologies Used

- **Frontend**: Razor Pages (ASP.NET Core)
- **Backend**: ASP.NET Core
- **ORM**: Entity Framework Core
- **Database**:
  - Development: SQLite
  - Production: MySQL

## 🚀 Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download)
- [SQLite](https://www.sqlite.org/download.html) (for development)
- [MySQL Server](https://dev.mysql.com/downloads/mysql/) (for production)

### Installation

1. **Clone the Repository**

```bash
git clone https://github.com/Abdullah-Khawahir/razor-attendance.git
cd razor-attendance
```

2. **Configure the Application**

Update the `appsettings.Development.json` for development:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=attendance_dev.db"
  },
    "Attendance": {
      "EarlyAttendanceTime": "15:30:00",
      "LateAttendanceTime": "08:30:00"
    }
}
```

For production, update `appsettings.Production.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=your-mysql-host;Database=attendance_db;User=youruser;Password=yourpassword;"
  },
    "Attendance": {
      "EarlyAttendanceTime": "15:30:00",
      "LateAttendanceTime": "08:30:00"
    }
}
```

3. **Apply Migrations and Create the Database**

- **Development (SQLite)**:

```bash
dotnet ef database update --environment Development
```

- **Production (MySQL)**:

```bash
dotnet ef database update --environment Production
```

4. **Run the Application**

- **Development**:

```bash
dotnet watch # or run
```

- **Production**:

```bash
dotnet run --environment "Production"
```

The application will be accessible at `http://localhost:5000` by default.

## ⚙️ Configuration

The application uses the following configuration for attendance timings:

```json
"Attendance": {
  "EarlyAttendanceTime": "15:30:00",
    "LateAttendanceTime": "08:30:00"
}
```

These settings determine the thresholds for early and late attendance and can be adjusted in the respective `appsettings.{Environment}.json` files.

## 📁 Project Structure

- `Entities/`: Contains the Entity Framework Core entity classes.
- `Pages/`: Razor Pages for the application's UI.
- `Services/`: Business logic and service classes.
- `Database/`: Database context and migration files.
- `wwwroot/`: Static files (CSS, JS, images).
- `Program.cs`: Entry point and application configuration.
- `appsettings.{Environment}.json`: Environment-specific configuration files.

