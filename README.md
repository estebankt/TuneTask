# TuneTask – AI-Powered Task Manager

TuneTask is a **Task Manager** that leverages **AI (OpenAI GPT-4) for intelligent semantic search of tasks**. It is built with **.NET 8, ADO.NET, JWT Authentication, and Docker** for **scalability and performance**.

---

## 🚀 Features

### ✅ **Core Features**

- 🗑️ **Task Management** – Create, update, and retrieve tasks.
- 🔐 **Authentication & Authorization** – Secure API with JWT tokens.
- 🌜 **Error Handling & Logging** – Global exception handling with structured logging.
- 🐳 **Dockerized SQL Database** – Runs in a containerized SQL Server instance.
- 🔍 **Semantic Serach with AI** - The solution uses OpenAI's semantic search to provide best based not only on exact term matches but similarity.


## ⚙️ **Tech Stack**

| **Technology**              | **Purpose**                |
| --------------------------- | -------------------------- |
| **.NET 8**                  | Backend API                |
| **ADO.NET**                 | Direct SQL interactions    |
| **JWT Authentication**      | Secure user authentication |
| **AI (OpenAI API)**         | AI-powered search          |
| **Docker & Docker Compose** | Containerized database     |
| **MSSQL (via Docker)**      | Database                   |

---

## 🔠 **Architecture Overview**

TuneTask follows **Clean Architecture** principles:

```
/src
  /TuneTask.Core         # Business Logic & Interfaces
  /TuneTask.Infrastructure  # Data Access (ADO.NET)
  /TuneTask.Shared       # Error Handling 
  /TuneTask.API          # REST API (Controllers & Authentication)
  /TuneTask.Test          # Unit Testing of solution
```

---

## 🛠️ **Setup & Installation**

### **1️⃣ Clone the Repository**

```sh
git clone https://github.com/yourusername/TuneTask.git
cd TuneTask
```


### **3️⃣ Run the Application with Docker**

```sh
docker-compose up --build -d
```

### **4️⃣ Access the API**

- **Swagger UI:** `http://localhost:5050/swagger`
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


## 📺 **AI-Powered Tasks Search Instructions**

### 🚀 How AI-Powered Search Works

TuneTask leverages **OpenAI's text embedding model** (`text-embedding-3-small`) to enable **semantic search**. This allows users to **search tasks not just by keywords but also by meaning**, improving search relevance.

### 🔎 **How It Works:**

1. **Task Creation with Embeddings:**

   - When a task is created, its **description** is processed using OpenAI’s **embedding model**.
   - The model converts the text into a high-dimensional **vector representation** (embedding).
   - This vector is stored in the **database** along with the task.

2. **Search Query Processing:**

   - When a user searches for a task, the **query text** is also **converted into an embedding** using the same model.
   - Instead of a direct SQL `LIKE` query, we **compare the stored task embeddings** with the query embedding using **cosine similarity**.

3. **Retrieving Relevant Tasks:**

   - The search function retrieves all tasks from the database.
   - It computes the **cosine similarity** between the query embedding and stored embeddings.
   - The most **relevant tasks** (tasks with the highest similarity score) are returned.

### 🔢 **Example:**

#### **Scenario:**

- **User Query:** `"Work on deep learning project"`
- **Stored Tasks:**
  1. `"Train AI Model for NLP"`
  2. `"Fix UI bugs in React App"`
  3. `"Read Deep Learning Papers"`

#### **Expected Result:**

Instead of returning tasks based on **exact word matches**, the system recognizes that **"Train AI Model for NLP"** is **related** to `"Work on deep learning project"` and ranks it higher.

---

### ⚙️ **Implementation Details**

- **Model Used:** `text-embedding-3-small`
- **Vector Storage:** Stored as a `FLOAT[]` array in the database.
- **Similarity Calculation:** **Cosine Similarity** function measures how close the query is to stored task descriptions.

---

### 🛠️ **Using the AI Search API**

- **Endpoint:**
  ```http
  GET /api/tasks/search?query=your_text_here
  ```
- **Example Request:**
  ```sh
  curl -X GET "http://localhost:5050/api/tasks/search?query=deep+learning" -H "Authorization: Bearer YOUR_TOKEN"
  ```
- **Example Response:**
  ```json
  [
    {
      "id": "3585e29f-b2b0-4d2d-bbb6-22bdfdb2a762",
      "title": "Train AI Model for NLP",
      "description": "Developing a machine learning model for NLP tasks.",
      "status": "Pending",
      "createdAt": "2025-02-16T15:04:33.1033352Z",

    }
  ]
  ```

This allows **intelligent task retrieval** based on meaning rather than just keywords.

---

## 📜 **License**

This project is licensed under the MIT License.


