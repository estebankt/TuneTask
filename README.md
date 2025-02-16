# 🎵 TuneTask – AI-Powered Task Manager

TuneTask is a **CQRS-based Task Manager** that leverages **AI (OpenAI GPT-4) for intelligent task insights**. It is built with **.NET 8, ADO.NET, JWT Authentication, and Docker** for **scalability and performance**.

---

## 🚀 Features

### ✅ **Core Features**

- 🗑️ **Task Management** – Create, update, and retrieve tasks.
- 🔐 **Authentication & Authorization** – Secure API with JWT tokens.
- 🌜 **Error Handling & Logging** – Global exception handling with structured logging.
- 📡 **CQRS Architecture** – Clear separation between Commands & Queries.
- 🐳 **Dockerized SQL Database** – Runs in a containerized SQL Server instance.

### 💡 **AI-Powered Intelligence**

- 🤖 **AI-Generated Task Insights** – AI suggests relevant **task-related insights**.
- 🔍 **Natural Language Task Interpretation** – AI understands task descriptions & provides intelligent suggestions.

---

## ⚙️ **Tech Stack**

| **Technology**              | **Purpose**                |
| --------------------------- | -------------------------- |
| **.NET 8**                  | Backend API                |
| **ADO.NET**                 | Direct SQL interactions    |
| **JWT Authentication**      | Secure user authentication |
| **AI (OpenAI API)**         | AI-powered task insights   |
| **Docker & Docker Compose** | Containerized database     |
| **MSSQL (via Docker)**      | Database                   |
| **Serilog**                 | Structured Logging         |

---

## 🔠 **Architecture Overview**

TuneTask follows **Clean Architecture** principles:

```
/src
  /TuneTask.Core         # Business Logic & Interfaces
  /TuneTask.Infrastructure  # Data Access (ADO.NET)
  /TuneTask.Shared       # Error Handling & Cross-Cutting Concerns
  /TuneTask.API          # REST API (Controllers & Authentication)
```

---

## 🛠️ **Setup & Installation**

### **1️⃣ Clone the Repository**

```sh
git clone https://github.com/yourusername/TuneTask.git
cd TuneTask
```

### **2️⃣ Set Up Environment Variables**

Modify `appsettings.json` to include your OpenAI API key:

```json
{
  "OpenAI": {
    "ApiKey": "YOUR_OPENAI_API_KEY"
  }
}
```

### **3️⃣ Run the Application with Docker**

```sh
docker-compose up --build -d
```

### **4️⃣ Access the API**

- **Swagger UI:** `http://localhost:8080/swagger`
- **API Endpoints:**
  - **POST** `/api/tasks` – Create a Task (AI-powered insights included)
  - **GET** `/api/tasks/{id}` – Retrieve a Task

---

## 🛡️ **Security & Authentication**

- **JWT Authentication:**
  - **Register**: `POST /api/auth/register`
  - **Login**: `POST /api/auth/login` → Receive JWT Token
  - **Protected Endpoints** require `Authorization: Bearer {token}` header.

---

## 📺 **AI-Powered Task Insights**

- When a **task is created**, AI analyzes the description and provides **intelligent insights**.
- Example:
  ```json
  {
    "title": "Deep Focus Coding",
    "description": "A long coding session with no distractions.",
    "aiInsights": "For deep focus, try the Pomodoro Technique (25-min work sprints)."
  }
  ```

---

## ✅ **Future Enhancements**

- 🔄 **AI-Generated Task Prioritization** – AI will help prioritize tasks based on urgency.
- 🗓️ **Task Scheduling AI** – Suggests the best time to work on tasks.
- 🎵 **Spotify Integration (Future)** – AI-powered music recommendations.

---

## 🏆 **Why This Project is a Strong Technical Showcase**

- **🔧 Demonstrates CQRS & Clean Architecture**
- **🤖 Leverages AI for practical, real-world automation**
- **🔐 Implements Secure Authentication & Authorization**
- **🚀 Shows ability to integrate AI-powered features into .NET applications**

---

## 🐝 **License**

This project is licensed under the **MIT License**.

---

🚀 **Developed by [Your Name]** | **GitHub: **[**YourUsername**](https://github.com/yourusername)

