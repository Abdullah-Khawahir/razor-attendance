# Razor Attendance System

A web-based attendance management system built with ASP.NET Core Razor Pages. This application allows administrators to manage employee attendance efficiently, with configurable early and late attendance thresholds.

## 📝 Overview

The Razor Attendance System enables:

- **Employee Management**: Add, update, and delete employee records.
- **Attendance Tracking**: Record and monitor daily attendance.
- **Configurable Settings**: Define early and late attendance times via configuration.

## ⚙️ Technologies Used

- **Frontend**: Razor Pages, HTML, CSS
- **Backend**: ASP.NET Core
- **ORM**: Entity Framework Core
- **Database**:
  - **Development**: SQLite
  - **Production**: MySQL

## 📂 Project Structure

- `Entities/`: Contains the data models.
- `Pages/`: Razor Pages for UI.
- `Services/`: Business logic and data access.
- `Database/`: Database context and migrations.
- `wwwroot/`: Static files (CSS, JS, images).

## 🔧 Configuration

### Attendance Settings

Configure early and late attendance times in `appsettings.json` the defualt is what in the json:

```json
"Attendance": {
  "EarlyAttendanceTime": "15:30:00",
  "LateAttendanceTime": "08:30:00"
}
