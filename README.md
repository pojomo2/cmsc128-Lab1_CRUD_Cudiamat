# 🚀 CMSC 128 Laboratory Activity 1 Guide: Basic CRUD (To-Do List)

Author: Nikko Gabriel J. Hismaña

## 📋 Overview

This is an **individual/pair** activity. Every student/pair will build a simple **To-Do List application** that implements the four basic CRUD (Create, Read, Update, Delete) operations.

You are free to build this on **any tech stack** — web, mobile, or even a game engine — as long as it has a working user interface and a persistent database. The point of this activity is not to lock you into a specific framework, but to make sure you can:

1. Build a UI that a real user could operate without instructions.
2. Wire that UI up to CRUD operations.
3. Persist data so it survives an app/server restart.

This app is not a one-off exercise — you will **iteratively develop this same To-Do List app** throughout the semester (adding user accounts, access control, and eventually deploying it), so pick a tech stack you're comfortable committing to for the whole sem.

## 🎯 Learning Objectives

After completing this activity, each student should be able to:

1. Design and implement a basic CRUD data model (Create, Read, Update, Delete).
2. Persist application state using a lightweight database appropriate to their chosen stack.
3. Apply basic HCI/UX principles to a functional interface (forms, confirmation dialogs, feedback).
4. Explain and defend every part of their own source code (no plagiarism, no unexplained AI-generated code).
5. Evaluate tradeoffs between different tech stacks (mobile, web, desktop, game engine) for a small project.

## 🛠️ Tech Stack Options (Pick One)

You have **full creative freedom** here. Some recommendations, grouped by platform, in case you don't know where to start:

### Web Apps

- **HTML/CSS/JS + Flask (Python) + SQLite** — simplest option, good if you're already comfortable with Python.
- **MERN** (MongoDB, Express.js, React, Node.js) or **FERN** (Firebase, Express.js, React, Node.js)
- **MEAN** (MongoDB, Express.js, Angular, Node.js) or **FEAN** (Firebase, Express.js, Angular, Node.js)
- **Laravel (PHP) + MySQL/PostgreSQL** — if you want to practice MVC.
- **Django (Python) + SQLite/PostgreSQL** — if you hate yourself.

### Mobile Apps

- **FlutterFire** (Flutter + Firebase) — cross-platform, great if you want to also target mobile in later semesters.
- **React Native + Firebase/SQLite**
- **Native Android (Kotlin/Java) + Room (SQLite)**
- **Native iOS (Swift) + CoreData/SQLite**

### Desktop Apps

- **Python (Tkinter/PyQt) + SQLite**
- **C++ (Qt) + SQLite**
- **Java (JavaFX/Swing) + SQLite**
- **.NET (C# WinForms/WPF/MAUI) + SQLite**

### Game Engine (Yes, Really)

- **Godot (GDScript/C#) + SQLite or JSON save files** — treat your to-do list as a "quest log" UI.
- **Unity (C#) + SQLite/PlayerPrefs/JSON**

NOTE: Whichever stack you pick, the **database must be lightweight and appropriate for the project's scale** (e.g., SQLite, a local JSON/file-based store, or a free-tier cloud DB like Firebase/MongoDB Atlas). Avoid heavyweight enterprise databases for a simple to-do list — that's overkill.

Keep the next few activities in mind when choosing: **Assign 2** will add user account creation/login/logout, **Assign 3** will restrict each user to their own list, and **Assign 4** is refinement + deployment. Pick a stack that can realistically grow with these requirements.

## ✅ Application Requirements

Before you start, create a **PUBLIC** GitHub repository named `cmsc128-Lab1_CRUD_<Lastname>`.

### 🤏 Minimum Requirements

To get a passing score in the Features rubric, your To-Do List app must have:

1. **To-Do List Interface** — a clear, usable view of all tasks.
2. **Add Task**, with at minimum:
   - Title
   - Due date and time
   - Priority (`Low`, `Med`, `High`)
   - Tag/category (`School`, `Personal`, `Others`)
3. **Edit Task** — update any of the fields above for an existing task.
4. **Delete Task** — must show a **confirmation dialog** before deleting.
5. **Mark Task as Done** — visually distinguish completed tasks (e.g., strikethrough, checkbox, different color).
6. **Data Persistence** — all created/edited/deleted/completed tasks must persist after refreshing the browser or restarting the app/server (i.e., backed by an actual database or persistent local storage, not just an in-memory array/variable).

### ➕ Expanded Requirements

To get a perfect score in the Features rubric, implement **2 of the following 3** features (your choice):

**a. Undo Option When Deleting**

- After deleting a task, show a toast/snackbar/notification with an "Undo" action for a few seconds before the deletion is finalized.

**b. Filter and Sort**

- Sort by: Date Added (you must record a timestamp when the task was created), Due Date, Priority, and Tag.
- Filter by at least Tag and Priority.

**c. Built-in Calendar View**

- A calendar widget/view that shows which dates have tasks due, and lets the user click a date to see/manage tasks due that day.

You may implement all 3 for practice, but only 2 are required for full marks.

### Non-Functional Requirements

1. **HCI Design Practices**: Follow basic usability heuristics — clear labels, visible feedback on actions (success/error), consistent navigation, and no destructive action without confirmation.
2. **No Exposed Secrets**: If using a cloud database or API keys (e.g., Firebase config, MongoDB URI), do not commit credentials to GitHub. Use `.env` files and `.gitignore` them.
3. **README.md**: Must include:
   - Which tech stack/backend/database you chose and why
   - How to run the app locally (setup steps, dependencies, commands)
   - Example "API endpoints" or data operations (however your stack exposes CRUD — REST routes, Firestore calls, local DB queries, etc.)
   - Screenshots of the working app
4. **Final Commit**: Once done, commit your final README with the message:
   `"cmsc128-Indiv-Act1-finalX"`

## 📤 Submission Requirements

### Deliverables

1. **F2F Demo and Code Defense.** Be ready to explain, live, without notes or AI assistance:
   - How each CRUD operation is implemented, end to end (UI → logic → database)
   - How your data persists (what database/storage you used and why)
   - The structure of your project (where your data model, UI, and logic live)
   - Any expanded features you implemented and how they work
   - Any line of your own code, if asked
2. **GitHub Repository** with complete source code and README.md.
3. Upload your GitHub repository link via private message through Google Chat (if you're in a pair, include your partner).

Refer to the class Google Sheet for the submission/defense schedule and deadlines.

## �️ Possible Defense Questions

Be prepared to answer questions such as (this is not an exhaustive list):

### General CRUD Concepts

1. What does CRUD stand for, and where in your app does each operation happen?
2. Walk me through what happens, step-by-step, from clicking "Add Task" to the task appearing in your list.
3. Where and how is your data persisted? What happens to it when you restart your app/server?
4. Why did you choose your specific database (e.g., SQLite, Firebase, MongoDB Atlas) over the alternatives?
5. What is the difference between updating a task in your UI versus updating it in your database?
6. How does your delete confirmation dialog work — what would happen if you removed it?
7. How do you distinguish a "done" task from a "not done" task, both in your UI and in your data model?
8. What fields does a single task record have, and why did you choose that structure?
9. If two tasks have the same title, how does your app tell them apart internally?
10. What would you need to change in your code to add a new field (e.g., a "notes" field) to a task?

### Tech Stack-Specific Questions

11. What is your chosen tech stack, and how do its pieces (frontend, backend, database) communicate with each other?
12. If web-based: How does your frontend send data to your backend (e.g., fetch/AJAX, form submission)? What format is the data in?
13. If using a framework (e.g., Flask, Laravel, Flutter): What role does the framework play versus code you wrote yourself?
14. If mobile: How does your app store data locally versus remotely (if applicable)? What happens if there's no internet connection?
15. If using a cloud database (Firebase, MongoDB Atlas): How do you keep your API keys/credentials out of your GitHub repository?
16. If using a game engine: How did you adapt a game engine (built for rendering/game loops) to behave like a data-driven app?
17. Why is your chosen stack appropriate (or not ideal) for a small CRUD app like this?
18. What part of your tech stack would make it easiest/hardest to add user accounts next semester (Assign 2)?

### Expanded Features (if implemented)

19. How does your "Undo" feature work? Where is the deleted task held before it's permanently removed?
20. How does your sorting/filtering logic work? Is it done in your database query, or in your application code?
21. How does your calendar view know which dates have tasks due?

## �📊 Rubrics for Grading

| Criteria                       | Excellent                                                                                                                                                                                                                           | Good                                                                                                                                                    | Satisfactory                                                                                                                                   | Needs Improvement                                                                                                                                                                                                                                                                                               |
| ------------------------------ | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **Features** (15 pts)          | All minimum features and both required expanded features (2 of 3) work correctly, with no or negligible bugs. Data persistence is reliable across restarts. (15-12)                                                                 | All minimum features implemented and working. Only one expanded feature implemented, or expanded features have minor bugs. (11-10)                      | All minimum features are implemented, but with a few noticeable issues/bugs (e.g., minor UI glitches, edge cases not handled). (9)             | One or more minimum features missing, broken, or non-functional (e.g., no persistence, delete has no confirmation, edits don't save). Multiple significant bugs. (8-1)                                                                                                                                          |
| **Design** (5 pts)             | Interface is intuitive and polished. Clear labels, consistent layout, visible feedback for every action (success/error/confirmation), no unlabeled or confusing controls. Follows HCI best practices with no/negligible issues. (5) | Good UX/UI overall with only minor issues (e.g., slightly inconsistent spacing/colors, one or two unclear labels). (4)                                  | Usable interface but with noticeable design issues (e.g., missing feedback on actions, cluttered layout, unclear priority/tag indicators). (3) | Interface is confusing, inconsistent, or missing basic feedback/confirmation. Several significant UX/UI issues that would confuse a real user. (2-1)                                                                                                                                                            |
| **Defense Questions** (10 pts) | All questions about the CRUD implementation, database design, and stack choice answered correctly and confidently. (10)                                                                                                             | One or two questions not answered correctly, but overall understanding is evident. (9-7)                                                                | More than 2 questions not answered correctly; understanding is shaky in several areas. (6)                                                     | Most/all conceptual questions not answered correctly. (5-1)                                                                                                                                                                                                                                                     |
| **Code Defense** (10 pts)      | Student clearly explains every part of their source code — data model, CRUD logic, persistence layer, and UI wiring — in their own words, and can trace what happens step-by-step for each operation. (10-8)                        | Student explains most of the code correctly but struggles with a few specific parts (e.g., unsure why a certain function is structured that way). (7-5) | Student can only explain the code at a surface/high level and struggles when asked to go into implementation details. (4-1)                    | Student **cannot recall, explain, or trace their own source code** when asked (e.g., doesn't know what a function does, cannot explain how data gets saved, shows signs the code was copied or AI-generated without understanding). **AUTOMATIC FAILURE for this activity**, regardless of other rubric scores. |

**TOTAL: 40 (PASSING SCORE: 24/40)**

> ⚠️ **Note on Code Defense**: This is graded separately from "Defense Questions" (which covers CRUD/database concepts in general) and from "Features"/"Design" (which grade the working app). Code Defense specifically checks whether **you** wrote and understand **your own code**. If you cannot explain your own source code, you **automatically fail this activity** --- this is meant to catch plagiarism or fully AI-generated submissions passed off as your own work. Using AI tools to help you learn/debug is fine; not understanding what your own program does is not.

## 📁 Suggested Project Structure

Structure will vary a lot depending on your chosen stack, but at minimum your repository should clearly separate:

```
your-todo-project/
├── frontend/ or src/ (or equivalent for your stack) # UI / views
├── backend/ or server/ (if applicable)              # CRUD logic / API
├── database/ or data/ (if applicable)               # migrations, schema, seed data
├── .gitignore                                       # exclude secrets, dependencies, build artifacts
├── .env (NOT committed)                              # API keys / DB credentials
└── README.md
```

If your stack is a single project (e.g., a Flutter or Godot project), just make sure your data model, UI screens, and persistence logic are in clearly separated files/folders — don't dump everything into one file.

## 📚 Helpful Resources

- [SQLite Documentation](https://www.sqlite.org/docs.html)
- [Flask Documentation](https://flask.palletsprojects.com/)
- [FlutterFire Documentation](https://firebase.flutter.dev/)
- [Firebase Documentation](https://firebase.google.com/docs)
- [MongoDB Atlas Documentation](https://www.mongodb.com/docs/atlas/)
- [Godot Documentation](https://docs.godotengine.org/)
- [Laravel Documentation](https://laravel.com/docs)

## 💡 Support

If you encounter issues:

1. Check your stack's official documentation first.
2. Search Stack Overflow for similar problems.
3. Ask classmates for help. But for the sake of your class standing, **do not copy their code** — you will be asked to explain your own code during the defense.
4. Contact the instructor during office hours.
5. Use AI tools (ChatGPT, Claude, Copilot) for coding assistance — but make sure you understand every line, since you will be asked to defend your own code. (If you're going this route, I would suggest that you ask AI tools to create a guide with the code snippets included rather than a full implementation, so you can learn and write your own code.)

Good luck with your lab! 🚀
