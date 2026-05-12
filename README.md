
# Course Note Sharing System

Course Note Sharing System, öğrencilerin ders notlarını paylaşabildiği ve yöneticilerin içerikleri yönetebildiği ASP.NET Core MVC tabanlı bir web uygulamasıdır.

## Features

* User Authentication & Authorization
* Separate Admin Login System
* Role-Based Access Control
* Course Management
* Note Sharing System
* Admin Dashboard
* Profile Image Upload
* PostgreSQL Database Integration
* Responsive UI Design

## Technologies

* ASP.NET Core MVC (.NET 8)
* Entity Framework Core
* PostgreSQL
* ASP.NET Core Identity
* Bootstrap 5

## Authentication System

### User Login

```bash
https://localhost:5001/login
```

### Admin Login

```bash
https://localhost:5001/admnistrative
```

* Admin and user login systems are separated.
* Only users with the `Admin` role can access the admin panel.

## Installation

### 1. Clone the repository

```bash
git clone <repo-url>
cd CourseNoteSharingSystem
```

### 2. Restore packages

```bash
dotnet restore
```

### 3. Configure database

Update your `appsettings.json` connection string.

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=CourseNoteDB;Username=postgres;Password=yourpassword"
}
```

### 4. Run migrations

```bash
dotnet ef database update
```

### 5. Run project

```bash
dotnet run
```

## Admin Features

* Manage Courses
* Manage Users
* Upload Profile Images
* Access Admin Dashboard

## Security

* ASP.NET Core Identity
* Role-Based Authorization
* CSRF Protection
* Secure File Upload Validation

## Project Structure

```bash
Controllers/
Models/
Views/
ViewModels/
Data/
wwwroot/
```

## Default Roles

* Admin
* User

## License

This project is developed for educational purposes.
