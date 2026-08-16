AI Study Planner
An intelligent web-based study planning system that analyzes a student’s academic data, workload, goals, tasks, and available study time to generate a personalized weekly study plan.
The project combines ASP.NET Core, Entity Framework Core, SQLite, Python, and Machine Learning to create an end-to-end personalized academic planning platform.

⸻

Overview
Students often have multiple subjects, academic goals, assignments, deadlines, and limited study time, making it difficult to determine what to study and how much time to allocate to each subject.
AI Study Planner addresses this problem by collecting a student’s academic information and automatically generating a personalized study plan.
The system considers:
* Subjects and their weekly workload
* Academic goals
* Study tasks and deadlines
* Task completion status
* Daily study availability
* Weekly available study hours
* Machine-learning-based study-time recommendations
The resulting plan provides a structured overview of the student’s workload and recommends how their available time should be distributed.

⸻

Key Features
Student Dashboard
The dashboard provides an overview of the student’s current academic workload, including:
* Number of subjects
* Academic goals
* Pending tasks
* Available weekly study hours

⸻

Subject Management
Students can add and manage their subjects, including:
* Subject name
* Description
* Weekly workload
The subject information is later used by the AI planning system.

⸻

Goal Management
Students can define academic goals with:
* Goal title
* Description
* Target date
* Target grade

⸻

Task Management
Students can create and manage study tasks with:
* Task title
* Description
* Due date
* Subject association
* Completion status
The planner uses task deadlines and completion status when generating recommendations.

⸻

Study Availability
Students can specify how many hours they are available to study on different days of the week.
This information is used to determine how much study time can realistically be allocated to the student’s subjects and tasks.

⸻

AI Study Planner
The central feature of the application is the personalized AI study-planning system.
The planner receives structured academic information from the student’s account and produces a structured study plan containing:
* Weekly overview
* Available study hours
* Total subject workload
* Workload coverage
* Priority tasks
* Recommended study time for each subject
* Weekly study schedule
* Planning logic
* Warnings and planning notes
Example Planning Output
The generated plan can answer questions such as:
* Which tasks should receive the highest priority?
* How many hours should be allocated to each subject?
* How should study time be distributed throughout the week?
* Is the student’s available time sufficient for their workload?
* Which deadlines require immediate attention?

⸻

Machine Learning Component
The AI component was implemented locally using Python and machine learning, rather than relying on an external commercial AI API.
This was intentionally designed so that the application’s core planning functionality does not depend on an external AI provider.
The Python component contains:
pythonAI/
├── study_planner.py
└── ML/
    ├── study_model.py
    ├── train_model.py
    └── study_time_model.joblib
The trained machine-learning model is used to support personalized study-time recommendations.
The ASP.NET Core application communicates with the Python component through a dedicated process-management service.

⸻

System Architecture
The application follows a multi-component architecture:
                    ┌──────────────────────┐
                    │     Student/User     │
                    └──────────┬───────────┘
                               │
                               ▼
                    ┌──────────────────────┐
                    │   ASP.NET Core Web   │
                    │    Razor Pages UI    │
                    └──────────┬───────────┘
                               │
              ┌────────────────┼────────────────┐
              │                │                │
              ▼                ▼                ▼
       ┌─────────────┐  ┌─────────────┐  ┌──────────────┐
       │   SQLite    │  │ Entity      │  │ AI Planner   │
       │  Database   │  │ Framework   │  │   Service    │
       └─────────────┘  └─────────────┘  └──────┬───────┘
                                                │
                                                ▼
                                      ┌──────────────────┐
                                      │ Python ML Engine │
                                      │  + Trained Model │
                                      └──────────────────┘

⸻

Technology Stack
Backend
* C#
* .NET 6
* ASP.NET Core Razor Pages
* Entity Framework Core
* ASP.NET Core Identity
* SQLite
Machine Learning
* Python
* NumPy
* Scikit-learn / machine-learning components
* Joblib
* Trained .joblib model
Frontend
* HTML5
* CSS3
* JavaScript
* Bootstrap
* jQuery
Database
* SQLite
* Entity Framework Core Migrations

⸻

Project Structure
AIStudyPlanner/
│
├── Data/
│   └── ApplicationDbContext.cs
│
├── Migrations/
│   └── Entity Framework Core migrations
│
├── Models/
│   ├── AIStudyPlan.cs
│   ├── EmailSubscriber.cs
│   ├── StudyAvailability.cs
│   ├── StudyGoal.cs
│   ├── StudyTask.cs
│   └── Subject.cs
│
├── Pages/
│   ├── AIPlanner/
│   ├── Account/
│   ├── StudyAvailability/
│   ├── StudyGoals/
│   ├── Tasks/
│   └── Shared/
│
├── Services/
│   ├── PythonAIProcessManager.cs
│   └── StudyPlannerAIService.cs
│
├── pythonAI/
│   ├── study_planner.py
│   └── ML/
│       ├── study_model.py
│       ├── train_model.py
│       └── study_time_model.joblib
│
├── wwwroot/
│   ├── css/
│   ├── js/
│   ├── images/
│   └── lib/
│
├── Program.cs
└── AIStudyPlanner.csproj

⸻

Data Model
The application uses Entity Framework Core to manage the application’s relational data.
The primary entities include:
User
 │
 ├── Subjects
 │
 ├── Study Goals
 │
 ├── Study Tasks
 │
 ├── Study Availability
 │
 └── AI Study Plans
Each student’s academic information is associated with their authenticated account.
This allows different users to maintain independent subjects, goals, tasks, schedules, and generated study plans.

⸻

Persistent AI Study Plans
Generated study plans are stored in the database as AIStudyPlan records.
This allows a generated plan to remain available after the user leaves the page and returns later.
The system therefore separates:
1. Input academic data
2. AI-generated planning result
3. Persistent storage of the generated plan

⸻

Database Migrations
Entity Framework Core migrations are included in the repository so that the database schema can be recreated in another environment.
The local development SQLite database itself is intentionally excluded from version control.
AIStudyPlanner.db       → local development database
Migrations/             → database schema history
This allows the application to recreate its database structure without committing a local runtime database containing development data.

⸻

Authentication
The application uses ASP.NET Core Identity for user authentication.
Users can:
* Register
* Log in
* Log out
* Maintain their own academic data
User-specific database records are associated with the authenticated user’s identity.

⸻

Local Setup
Requirements
To run the project locally, install:
* .NET 6 SDK
* Python 3.x
* Required Python packages
* Git
Clone the repository
git clone <repository-url>
cd AIStudyPlanner
Restore .NET dependencies
dotnet restore
Install Python dependencies
Install the Python packages required by the files in pythonAI/.
Apply database migrations
dotnet ef database update
Run the application
dotnet run
The application can then be opened using the local URL provided by ASP.NET Core.

⸻

Machine Learning Workflow
The machine-learning component follows a training and inference workflow:
Student Academic Data
        │
        ▼
Feature Preparation
        │
        ▼
Machine Learning Model
        │
        ▼
Study-Time Recommendation
        │
        ▼
Study Planning Logic
        │
        ▼
Personalized Weekly Plan
The trained model is stored as:
pythonAI/ML/study_time_model.joblib

⸻

Design Goals
The project was designed around several principles:
Personalization
Study recommendations are based on each student’s individual workload, goals, deadlines, and availability.
Practicality
The planner should generate schedules that fit within the student’s actual available study time.
Persistence
Generated plans should remain available after the user leaves and returns to the application.
Separation of Components
The web application, database layer, planning service, and machine-learning component are separated into dedicated components.
Local AI
The core recommendation system is implemented locally using Python and machine learning instead of requiring a commercial external AI API.

⸻

Current Status
The application currently includes:
* User registration and authentication
* Student dashboard
* Subject management
* Goal management
* Task management
* Study availability management
* Calendar
* Personalized AI study-plan generation
* Machine-learning-based study-time recommendations
* Persistent AI study plans
* Responsive web interface
* Entity Framework Core migrations
* SQLite persistence

⸻

Future Development
Potential future improvements include:
* More advanced machine-learning models
* Adaptive recommendations based on historical study behavior
* Automatic schedule optimization
* Progress-based recommendation updates
* Study-session tracking
* Analytics and visualization
* Notifications and reminders
* Improved long-term prediction of workload and academic performance

⸻

Academic Purpose
This project was developed as an end-to-end exploration of how web application engineering, database systems, user modeling, and machine learning can be combined to address a practical educational problem.
Rather than treating AI as an isolated feature, the project integrates machine learning into a complete software system involving data collection, persistence, personalization, recommendation, and user interaction.

⸻

Author
Mina Davoodi
AI Study PlannerC# 
ASP.NET Core 
Entity Framework Core 
SQLite 
Python 
Machine Learning