# 🎉 EventHUB – Modern Event Management Platform

**EventHUB** is a comprehensive event management system built with **ASP.NET Core MVC**. It allows **administrators** to manage events and members, while enabling **members** and **guests** to register, interact, and provide feedback.

---

## 🌟 Key Features

### 🎯 Core Functionality
- Full CRUD operations for events
- Admin Dashboard to manage:
  - Events (Add, Update, Delete)
  - Members & Meetings
- Member management system
- Messaging between members and admin
- Real-time analytics and reporting

### 🤝 User Engagement
- Event registration
- Feedback collection and analysis
- Direct messaging portal
- Multi-tier registration system

---

## 🛡️ System Architecture

- Role-based access control
- Secure authentication flows
- Responsive design using Bootstrap

---

## 🖥️ Technology Stack

| Layer     | Technology                  |
|-----------|------------------------------|
| Frontend  | ASP.NET Core Razor Views     |
| Backend   | ASP.NET Core MVC 9.0         |
| Database  | SQL Server 2019              |
| Styling   | Bootstrap 5 + Custom CSS     |s
| Tooling   | Visual Studio 2022           |

---

## 📂 Project Structure

📁 EventHUB/

├── 📂 Controllers/ # 🎮 Handles user input & app flow (MVC Controllers)

├── 📂 Models/ #        🧠 App data structures & domain models

├── 📂 Views/ #         🖼️ Razor Views (UI templates)

├── 📂 Services/ #      ⚙️ Business logic & service layer

├── 📂 Data/ #          🗄️ Data access layer (Repositories, DbContext)

├── 📂 wwwroot/ #       🌐 Static files (JS, CSS, images)

│

├── 📂 HelpPage/ #      📚 Class Library

├── 📂 Previews/ #      📸 Application screenshots

└── 📄 EventHUB.sln #   🧩 Visual Studio Solution File

---

## 🖼️ Application Preview

<p align="center">
  <img src="Previews/Admin Dashboard.png" alt="Admin Dashboard" width="45%">
  <img src="Previews/Admin Finance.png" alt="Event List" width="45%">
</p>

---

## 🚀 Getting Started

### Prerequisites
- [.NET 9.0 SDK](https://dotnet.microsoft.com/download)
- SQL Server 2019
- Visual Studio 2022

### Setup Instructions

1. **Clone the repository**
   ```bash
    git clone https://github.com/hashir-logic/EventHUB.git

2. **Open the solution**
        
3. **Restore dependencies**

4. **Configure database**

5. **Apply migrations**

6. **Run the application**