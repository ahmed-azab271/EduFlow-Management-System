# EduFlow Management System 🎓

EduFlow is a comprehensive educational management system built with ASP.NET Core that revolutionizes the way educational institutions manage their operations. It provides a robust platform for seamless interaction between students, teachers.

## 🌟 Core Features

### 1. Academic Management
- **Course Management**
  - Dynamic course creation
  - Student enrollment tracking
  - Real-time course updates

- **Assignment System**
  - Digital assignment creation and submission
  - Grading and feedback system
  - Progress tracking

### 2. Communication Hub
- **Real-time Messaging**
  - Instant messaging between students and teachers
  - Message history and archiving
  - Automated message cleanup

## 🛠️ Technologies Used

- **Backend Framework**
  - ASP.NET Core 9.0
  - Entity Framework Core
  - SQL Server

- **Architecture & Patterns**
  - Clean Architecture
  - Unit of Work Pattern
  - Generic Repository Pattern
  - Specification Design Pattern

- **Security & Authentication**
  - JWT Authentication
  - Role-based Authorization
  - Password Hashing

- **Performance & Optimization**
  - AsNoTracking for read-only operations
  - AutoMapper for DTO mapping
  - SignalR for real-time communication

- **Documentation & Testing**
  - Swagger/OpenAPI
  - Postman Collection

## 🚀 Getting Started

1. **Clone the Repository**

2. **Run the Application**
   - The application will:
      - Apply all migrations
      - Create the database if it doesn't exist (automatically Update-database)
  
3. **Initial Setup**
   - From Postman Collection
      - In Identity Folder , Register account (Automatically Becomes Admin)
   - Configure system settings

## 🔒 Security Features

- JWT-based authentication
- Role-based access control
- Password hashing and salting
- Global error handler for consistent API responses
