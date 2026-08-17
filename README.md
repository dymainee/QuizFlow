# QuizFlow

## 1. Introduction
**QuizFlow** is a web platform developed in C# and .NET 8. The project is designed for creating, managing, and completing educational and entertaining quizzes, as well as for conducting real-time quizzes.

## 2. Key Features

* **Technology Stack:** C#, .NET 8, ASP.NET Core MVC, Entity Framework Core, SignalR, xUnit, Moq.
* **Database:** MS SQL Server with Entity Framework Core and TPT inheritance.
* **Async/Await:** Asynchronous request processing.
* **Security:** JWT (JSON Web Token) authentication with role-based access control (Student/Teacher).
* **QR Code Integration:** Automatic QR code generation via API for quick participant access to the multiplayer lobby.
* **Real-Time Multiplayer:** SignalR and an in-memory room manager are used to enable real-time player interaction.
* **Test coverage:** Unit tests have been written for key services (`QuizService`, `MenuService`, `LobbyServiceMultiplayer` , `User Services`) and repositories.

## 3. Detailed Explanation

### 3.1. Working with Data and EF Core (Data Processing)
* Entity and relationship configurations are defined using Entity Framework Core, including one-to-many relationships between Quiz and Question, as well as Question and AnswerOption.
* DTO models are used to separate database entities from data passed through controllers.
* Cascade deletion of related entities is configured when deleting quizzes or questions.

### 3.2. QR Codes and Multiplayer (QR API & Lobby)
* Automatic generation of RoomCode, connection link, and QR code when creating a room.
* Players can join by entering the code or scanning the QR code.
* LobbyServiceMultiplayer safely handles multiple players joining the same room at the same time using lock.

### 3.3. Search, Filtering, Sorting, and Pagination (Quiz Management)
* **Search & Filtering:** Quizzes can be filtered by group or title.
* **Pagination:** Quizzes are loaded page by page to reduce database load.
* **Sorting:** Quizzes can be sorted by different criteria.

### 3.4. Automated Testing (Unit Testing)
* **xUnit & Moq:** Used for unit testing and dependency mocking.
* **Service Testing:** Core service logic is tested.
* **Repository Testing:** Repository calls and data-saving operations are verified.

## QuizController & QuizService

* Browser / View -> QuizController -> IQuizService -> IQuizRepository -> Database

* **Quiz Creation:** Only teachers can create quizzes and upload quiz images.
* **Question Management:** Add, edit, and delete questions and answer options.
* **Quiz Management:** Publish, archive, and delete quizzes.
* **Security:** [Authorize(Role)] is used to restrict access, with quiz creation limited to teachers.
* **Async Operations:** async/await is used for database and file operations.
* **File Handling:** Uploaded images are stored in wwwroot/Images/quizzes/ with unique filenames. The image path is stored in the database.

## MenuController & MenuService

* Browser / View -> MenuController -> IMenuService -> IMenuRepository -> Database

* **Catalog:** Displays only published quizzes.
* **Filtering:** Quizzes can be filtered by title.
* **Sorting:** Sort by title or creation date in ascending or descending order.
* **Pagination:** Quizzes are displayed page by page with total result count.
* **Security:** Only registered users can access the catalog.

## QuizSessionController & QuizSessionService

Browser / View -> QuizSessionController -> IQuizSessionService / ILobbyService(in-memory room manager) -> Database / HttpClient(QR API)

* **Singleplayer:** Users can answer questions and view their results.
* **Multiplayer:** Teachers can create rooms and students join using a room code or QR code.
* **Real-Time:** SignalR is used for real-time communication between players.
* **QR Codes:** QR codes are generated through an external API.
* **Analytics:** Teachers can view and filter quiz results.
* **Security:** Both teachers and students can access the system.

## UserController & UserService

* Browser / View -> UserController -> IUserService -> IUserRepository -> Database

* **Registration:** Separate registration for teachers and students.
* **Student Profile:** View quiz history, sort results, and update personal information.
* **Teacher Profile:** : Manage created quizzes, view their status (Published or Archived), and update personal information.
* **JWT:** Authentication tokens are stored in HTTP cookies.

## 4. Database Structure

*Below is the system ER diagram created using Crow's Foot notation.*

![Project ER Diagram](images/er-diagram.png)

* **User:** Stores user accounts and profile information.
* **Student / Teacher:** User types with additional profile data using TPT inheritance.
* **Quiz:** Stores quiz information and status. Created by teachers.
* **Question:** Questions linked to a quiz.
* **AnswerOption:** Answer options linked to questions.
* **Session:** Stores quiz session and player results.
* **SessionAnswer:** Stores users' answers and earned points.

## 4.1 Main Relationships

User -> Student / Teacher (1:1)
Teacher -> Quiz (1:N)
Quiz -> Question (1:N)
Question -> AnswerOption (1:N)
User -> Session (1:N)
Quiz -> Session (1:N)
Session -> SessionAnswer (1:N)
Question -> SessionAnswer (1:N)
AnswerOption -> SessionAnswer (1:N)

## 5. Challenges & Solutions

* **TPT Inheritance:** Using a common UserService required type casting between Teacher and Student. The logic can be further improved by separating it into TeacherService and StudentService.
* **Thread Safety:** Simultaneous players joining a room could cause race conditions. This was solved using lock to safely manage the in-memory player collection.
* **In-Memory Lobby:** Rooms could stay in memory after the game ended. Room cleanup was added when the game starts or a player disconnects.
* **SignalR:** Setting up real-time communication was challenging at first. Players are grouped by RoomCode to keep everyone in the same room synchronized.
* **EF Core & GUIDs:** Manually creating entity IDs caused problems when saving new data. EF Core generates the IDs automatically.

## 6. What Worked Well

* **SignalR + QR Codes:** Players can quickly join a multiplayer game by scanning a QR code.
* **xUnit & Moq:** After testing the application through the UI, unit tests were used to check the service logic and repositories.

## 7. Future Improvements

* **ConcurrentDictionary:** Use ConcurrentDictionary to make room management safer when multiple users connect at the same time.
* **Separate User Services:** Split UserService into separate StudentService and TeacherService.

### 7.1 Business Logic Improvements

* **Question Types:** Add multiple-choice and text-input questions.
* **Timer & Scoring:** Add a time limit for each question and bonus points for faster answers.
* **Anti-Cheating:** Shuffle questions and answers and limit the number of quiz attempts.
* **Logout:** Add logout functionality to remove the JWT cookie and allow users to safely switch between accounts.

