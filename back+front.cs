╔══════════════════════════════════════════════════════════════════════════════════╗
║         STUDENT APP - SPLIT PROJECT GUIDE (BEGINNER LEVEL)                     ║
║         Backend: ASP.NET Core Web API + Entity Framework                       ║
║         Frontend: ASP.NET Core MVC (consumes API via AJAX)                     ║
╚══════════════════════════════════════════════════════════════════════════════════╝

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
                         WHAT WE ARE BUILDING
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

  PROJECT 1: StudentAPI        <-- Backend (Web API + Database)
  PROJECT 2: StudentMVC        <-- Frontend (MVC + AJAX calls to API)

  Both projects are separate Visual Studio projects inside ONE Solution.

  How they talk to each other:
  Browser --> StudentMVC --> (AJAX/HTTP) --> StudentAPI --> SQL Database

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
                         FOLDER STRUCTURE
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

StudentSolution/                   <-- One Solution File (.sln)
│
├── StudentAPI/                    <-- PROJECT 1: Backend
│   ├── Controllers/
│   │   └── StudentController.cs  <-- All API endpoints (GET, POST, PUT, DELETE)
│   ├── Models/
│   │   └── Student.cs            <-- Student data model / table structure
│   ├── Data/
│   │   └── AppDbContext.cs       <-- Database context (EF Core)
│   ├── Program.cs                <-- App startup + services configuration
│   └── appsettings.json          <-- Connection string to SQL Server
│
└── StudentMVC/                   <-- PROJECT 2: Frontend
    ├── Controllers/
    │   └── HomeController.cs     <-- Returns views (Index, Create pages)
    ├── Views/
    │   └── Home/
    │       ├── Index.cshtml      <-- Student list page (AJAX loads data)
    │       └── Create.cshtml     <-- Add/Edit student form (AJAX submits)
    └── Program.cs                <-- App startup for MVC

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
                    STEP 1: CREATE THE SOLUTION & PROJECTS
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Open Visual Studio 2022:

1. Click "Create a new project"
2. Search "Blank Solution" → Select it → Name it: StudentSolution → Click Create

Now add Project 1 (Backend):
3. Right-click Solution → Add → New Project
4. Search "ASP.NET Core Web API" → Select → Name it: StudentAPI
5. Framework: .NET 8  |  Uncheck "Use controllers" = NO (we want controllers)
   ✅ Check "Use controllers"
   ✅ Uncheck "Enable OpenAPI support" (optional, add manually later)

Now add Project 2 (Frontend):
6. Right-click Solution → Add → New Project
7. Search "ASP.NET Core Web App (Model-View-Controller)" → Name it: StudentMVC
8. Framework: .NET 8

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
            STEP 2: INSTALL NUGET PACKAGES (StudentAPI project only)
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Right-click StudentAPI project → Manage NuGet Packages → Browse → Install:

  1. Microsoft.EntityFrameworkCore.SqlServer
  2. Microsoft.EntityFrameworkCore.Tools

OR use Package Manager Console (Tools → NuGet → Package Manager Console):
Make sure "Default project" is set to StudentAPI before running:

  Install-Package Microsoft.EntityFrameworkCore.SqlServer
  Install-Package Microsoft.EntityFrameworkCore.Tools

NOTE: StudentMVC does NOT need Entity Framework. It only calls the API.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
                PROJECT 1: StudentAPI (BACKEND)
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

─────────────────────────────────────────────────────
FILE 1: Models/Student.cs
─────────────────────────────────────────────────────
// This file defines the Student "model" (blueprint).
// Entity Framework will use this class to create
// a table called "Students" in SQL Server.
// Each property = one column in the database table.

using System.ComponentModel.DataAnnotations; // Needed for [Key], [Required] etc.

namespace StudentAPI.Models
{
    public class Student
    {
        // [Key] tells EF that "Id" is the Primary Key (auto-increments)
        [Key]
        public int Id { get; set; }

        // [Required] means this field cannot be empty/null
        // The ? after string means nullable in C# - we use with Required to get proper error msg
        [Required(ErrorMessage = "Name is required")]
        public string? Name { get; set; }

        // [EmailAddress] validates proper email format automatically
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string? Email { get; set; }

        // [StringLength(10, MinimumLength = 10)] ensures exactly 10 digits
        [Required(ErrorMessage = "Mobile is required")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Mobile must be 10 digits")]
        public string? Mobile { get; set; }

        // [Range] checks value is between 18 and 100
        [Range(18, 100, ErrorMessage = "Age must be 18 or above")]
        public int Age { get; set; }

        [Required(ErrorMessage = "City is required")]
        public string? City { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        public string? Gender { get; set; }
    }
}

─────────────────────────────────────────────────────
FILE 2: Data/AppDbContext.cs
─────────────────────────────────────────────────────
// DbContext is the "bridge" between your C# code and SQL Server.
// It represents your database session.
// DbSet<Student> represents the "Students" table.

using Microsoft.EntityFrameworkCore;
using StudentAPI.Models; // We need Student model from Models folder

namespace StudentAPI.Data
{
    public class AppDbContext : DbContext
    {
        // Constructor: receives configuration options (connection string etc.)
        // and passes them to the parent DbContext class
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            // No extra code needed here — base class handles everything
        }

        // This property represents the "Students" table in the database.
        // DbSet<Student> means: "a collection of Student records in the DB"
        public DbSet<Student> Students { get; set; }
    }
}

─────────────────────────────────────────────────────
FILE 3: Controllers/StudentController.cs
─────────────────────────────────────────────────────
// This file contains all the API endpoints (routes).
// Each method handles a specific HTTP request type.
//
// API Base URL will be: https://localhost:PORT/api/student
//
// GET    /api/student          --> Get all students
// GET    /api/student/5        --> Get one student by ID
// POST   /api/student          --> Add new student
// PUT    /api/student/5        --> Update student with ID 5
// DELETE /api/student/5        --> Delete student with ID 5
// GET    /api/student/search   --> Search by name and/or city
// GET    /api/student/checkEmail?email=test@test.com  --> Check duplicate email

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentAPI.Data;
using StudentAPI.Models;

namespace StudentAPI.Controllers
{
    // [Route] sets the base URL pattern for all actions in this controller
    // [controller] is replaced by "Student" automatically
    [Route("api/[controller]")]

    // [ApiController] enables automatic model validation,
    // automatic 400 Bad Request on invalid data, etc.
    [ApiController]
    public class StudentController : ControllerBase
    {
        // _context gives us access to the database
        // It is injected via Dependency Injection (DI) - no need to "new" it
        private readonly AppDbContext _context;

        // Constructor - ASP.NET automatically provides AppDbContext here
        public StudentController(AppDbContext context)
        {
            _context = context;
        }

        // ─────────────────────────────────────────────
        // GET: api/student
        // Returns a list of ALL students from database
        // ─────────────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            // ToListAsync() runs the SQL: SELECT * FROM Students
            var students = await _context.Students.ToListAsync();

            // Ok() returns HTTP 200 with the data as JSON
            return Ok(students);
        }

        // ─────────────────────────────────────────────
        // GET: api/student/5
        // Returns ONE student by their ID
        // ─────────────────────────────────────────────
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            // FindAsync looks up the primary key (Id column)
            var student = await _context.Students.FindAsync(id);

            // If no student found with that ID, return 404 Not Found
            if (student == null)
                return NotFound(new { message = "Student not found" });

            return Ok(student);
        }

        // ─────────────────────────────────────────────
        // POST: api/student
        // Adds a NEW student to the database
        // The student data comes in the request body as JSON
        // ─────────────────────────────────────────────
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Student s)
        {
            // [ApiController] already handles null/invalid check,
            // but extra manual check is good practice
            if (s == null)
                return BadRequest(new { message = "No data received" });

            // Add the new student to the in-memory tracking list
            _context.Students.Add(s);

            // SaveChangesAsync() runs the actual SQL INSERT into database
            await _context.SaveChangesAsync();

            // Return 200 OK with the saved student (includes the new Id)
            return Ok(s);
        }

        // ─────────────────────────────────────────────
        // PUT: api/student/5
        // Updates an EXISTING student by their ID
        // ─────────────────────────────────────────────
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Student s)
        {
            // First check if student with that ID exists
            var existing = await _context.Students.FindAsync(id);

            if (existing == null)
                return NotFound(new { message = "Student not found" });

            // Update each field of the existing record with new values
            // (We don't change the Id - it stays the same)
            existing.Name   = s.Name;
            existing.Email  = s.Email;
            existing.Mobile = s.Mobile;
            existing.Age    = s.Age;
            existing.City   = s.City;
            existing.Gender = s.Gender;

            // SaveChangesAsync() runs SQL UPDATE
            await _context.SaveChangesAsync();

            return Ok(new { message = "Updated successfully" });
        }

        // ─────────────────────────────────────────────
        // DELETE: api/student/5
        // Deletes a student by their ID
        // ─────────────────────────────────────────────
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var student = await _context.Students.FindAsync(id);

            if (student == null)
                return NotFound(new { message = "Student not found" });

            // Remove() marks the record for deletion
            _context.Students.Remove(student);

            // SaveChangesAsync() runs SQL DELETE
            await _context.SaveChangesAsync();

            return Ok(new { message = "Deleted successfully" });
        }

        // ─────────────────────────────────────────────
        // GET: api/student/search?name=abc&city=Ahmedabad
        // Search students by Name and/or City (both optional)
        // ─────────────────────────────────────────────
        [HttpGet("search")]
        public async Task<IActionResult> Search(string? name, string? city)
        {
            // AsQueryable() lets us build the query piece by piece
            var query = _context.Students.AsQueryable();

            // Only filter by name if the user typed something
            if (!string.IsNullOrEmpty(name))
                query = query.Where(x => x.Name!.Contains(name)); // SQL: WHERE Name LIKE '%name%'

            // Only filter by city if the user selected one
            if (!string.IsNullOrEmpty(city))
                query = query.Where(x => x.City == city); // SQL: WHERE City = 'city'

            // Execute the final query and return results
            return Ok(await query.ToListAsync());
        }

        // ─────────────────────────────────────────────
        // GET: api/student/checkEmail?email=test@test.com
        // Checks if an email already exists in database
        // Used for real-time duplicate email validation
        // ─────────────────────────────────────────────
        [HttpGet("checkEmail")]
        public IActionResult CheckEmail(string email)
        {
            // Any() returns true if at least one record matches
            bool exists = _context.Students.Any(x => x.Email == email);

            // Return JSON: { "exists": true } or { "exists": false }
            return Ok(new { exists });
        }
    }
}

─────────────────────────────────────────────────────
FILE 4: Program.cs (StudentAPI - Backend startup)
─────────────────────────────────────────────────────
// Program.cs is the entry point of the application.
// Here we register all services and configure the HTTP pipeline.

using Microsoft.EntityFrameworkCore;
using StudentAPI.Data;

var builder = WebApplication.CreateBuilder(args);

// ── SERVICE REGISTRATION ─────────────────────────────────────────────────────

// Register Controller services so ASP.NET knows to use our controllers
builder.Services.AddControllers();

// Register AppDbContext with SQL Server
// It reads the connection string named "DefaultConnection" from appsettings.json
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// CORS = Cross-Origin Resource Sharing
// Since our frontend (StudentMVC on port 5000) talks to our API (port 7121),
// they are on different "origins" → we must explicitly allow this.
// Without CORS, the browser will block the AJAX requests!
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy
            .AllowAnyOrigin()   // Allow requests from any domain/port
            .AllowAnyMethod()   // Allow GET, POST, PUT, DELETE etc.
            .AllowAnyHeader()); // Allow any headers (Content-Type etc.)
});

// Register Swagger (optional but great for testing/debugging the API)
// Swagger gives you a visual UI at /swagger to test all your API endpoints
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ── BUILD THE APP ─────────────────────────────────────────────────────────────
var app = builder.Build();

// ── HTTP PIPELINE CONFIGURATION ───────────────────────────────────────────────
// Middleware runs in the ORDER you add it here. Order matters!

// Enable Swagger UI (only in Development mode - safe practice)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(); // Accessible at: https://localhost:PORT/swagger
}

// Apply CORS policy (MUST be before MapControllers)
app.UseCors("AllowAll");

// Enable HTTPS redirection (http → https)
app.UseHttpsRedirection();

// Enable Authorization middleware (needed if you add login/auth later)
app.UseAuthorization();

// Map all controllers to their routes automatically
// e.g. StudentController → /api/student
app.MapControllers();

// Start the application
app.Run();

─────────────────────────────────────────────────────
FILE 5: appsettings.json (StudentAPI)
─────────────────────────────────────────────────────
// This file stores configuration like database connection strings.
// "." means localhost SQL Server
// "StudentDB" is the database name (EF will create it for you)
// "Trusted_Connection=True" uses Windows Authentication (no username/password needed)
// "TrustServerCertificate=True" avoids SSL certificate errors in development

{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=StudentDB;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
        STEP 3: RUN MIGRATIONS (Create Database & Tables)
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Open: Tools → NuGet Package Manager → Package Manager Console

VERY IMPORTANT: Change "Default Project" dropdown to "StudentAPI"

Run these commands one by one:

  Step A: Create the migration file (snapshot of your model)
  ┌──────────────────────────────────────────────────────┐
  │  Add-Migration InitialCreate                         │
  └──────────────────────────────────────────────────────┘
  → This creates a "Migrations" folder in StudentAPI with a .cs file
  → That file contains SQL instructions to create the Students table
  → Do NOT manually edit migration files

  Step B: Apply migration → actually creates the DB and table in SQL Server
  ┌──────────────────────────────────────────────────────┐
  │  Update-Database                                     │
  └──────────────────────────────────────────────────────┘
  → Opens SQL Server, creates "StudentDB" database
  → Creates "Students" table with columns: Id, Name, Email, Mobile, Age, City, Gender
  → Creates "__EFMigrationsHistory" table (tracks applied migrations)

  ✅ Verify in SQL Server Management Studio (SSMS):
     Connect to "." → Databases → StudentDB → Tables → dbo.Students ✓

  IF you change the Student model later (add/remove column):
  ┌──────────────────────────────────────────────────────┐
  │  Add-Migration AddPhoneColumn   (any name)           │
  │  Update-Database                                     │
  └──────────────────────────────────────────────────────┘

  IF something goes wrong and you want to start fresh:
  ┌──────────────────────────────────────────────────────┐
  │  Drop-Database                                       │
  │  Remove-Migration                                    │
  │  Add-Migration InitialCreate                         │
  │  Update-Database                                     │
  └──────────────────────────────────────────────────────┘

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
                PROJECT 2: StudentMVC (FRONTEND)
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

IMPORTANT NOTE:
The MVC project has NO database, NO Entity Framework, NO models.
It ONLY has Views (HTML pages) and one Controller to serve those pages.
All data operations are done by calling the StudentAPI via AJAX (jQuery).

─────────────────────────────────────────────────────
FILE 6: Controllers/HomeController.cs (StudentMVC)
─────────────────────────────────────────────────────
// This controller only has one job:
// Return the correct HTML view (page) when the user navigates to a URL.
// All the actual data logic happens in JavaScript (AJAX) in the views.

using Microsoft.AspNetCore.Mvc;

namespace StudentMVC.Controllers
{
    public class HomeController : Controller
    {
        // GET: /Home/Index  or just  /
        // Returns the student list page
        public IActionResult Index()
        {
            // View() looks for Views/Home/Index.cshtml and returns it
            return View();
        }

        // GET: /Home/Create
        // Returns the add/edit student form page
        public IActionResult Create()
        {
            // The same page handles both Add and Edit
            // If ?id=5 is in URL → Edit mode (JS reads this)
            // If no id → Add mode
            return View();
        }
    }
}

─────────────────────────────────────────────────────
FILE 7: Views/Home/Index.cshtml (Student List Page)
─────────────────────────────────────────────────────
@* 
   This is the student LIST page.
   
   It does NOT use any Razor @Model — all data comes from the API via AJAX.
   
   HOW IT WORKS:
   1. Page loads → jQuery calls GET api/student → fills the table
   2. User types in search box → Search button calls GET api/student/search
   3. Delete button → jQuery calls DELETE api/student/{id} → reloads table
   4. Edit button → redirect to /Home/Create?id=5 (edit mode)
*@

<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8" />
    <title>Student List</title>
    <style>
        /* Basic styling to make the table readable */
        body { font-family: Arial, sans-serif; margin: 30px; }
        h2 { color: #333; }
        table { border-collapse: collapse; width: 100%; margin-top: 15px; }
        th, td { border: 1px solid #ccc; padding: 10px; text-align: left; }
        th { background-color: #4CAF50; color: white; }
        tr:nth-child(even) { background-color: #f9f9f9; }
        button { padding: 5px 10px; cursor: pointer; }
        .btn-edit { background: #2196F3; color: white; border: none; border-radius: 4px; }
        .btn-delete { background: #f44336; color: white; border: none; border-radius: 4px; }
        .btn-search { background: #4CAF50; color: white; border: none; padding: 8px 15px; border-radius: 4px; }
        .btn-add { background: #FF9800; color: white; text-decoration: none; padding: 8px 15px; border-radius: 4px; }
        #loadingMsg { display: none; color: gray; }
        #noDataMsg { display: none; color: red; }
    </style>
</head>
<body>

    <h2>Student List</h2>

    <!-- SEARCH BAR -->
    <div>
        <!-- User types a name to search -->
        <input type="text" id="search" placeholder="Search by Name" style="padding:7px; width:200px;" />

        <!-- User picks a city to filter -->
        <select id="filterCity" style="padding:7px;">
            <option value="">All Cities</option>
            <option value="Ahmedabad">Ahmedabad</option>
            <option value="Nadiad">Nadiad</option>
        </select>

        <!-- Triggers loadStudents() with filter values -->
        <button class="btn-search" onclick="loadStudents()">Search</button>

        <!-- Link to Create page (Add New Student) -->
        <a href="/Home/Create" class="btn-add" style="margin-left:15px;">+ Add Student</a>
    </div>

    <!-- Loading and no data messages -->
    <p id="loadingMsg">⏳ Loading...</p>
    <p id="noDataMsg">No students found.</p>

    <!-- TABLE: populated by JavaScript -->
    <table id="studentTable">
        <thead>
            <tr>
                <th>#</th>
                <th>Name</th>
                <th>Email</th>
                <th>Mobile</th>
                <th>Age</th>
                <th>City</th>
                <th>Gender</th>
                <th>Actions</th>
            </tr>
        </thead>
        <tbody id="tableBody">
            <!-- Rows injected here by jQuery -->
        </tbody>
    </table>

    <!-- jQuery CDN: must be loaded before our script -->
    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>

    <script>
        // ── CONFIGURATION ───────────────────────────────────────────────────
        // Change this port to match YOUR StudentAPI port number!
        // Find your API port: Right-click StudentAPI → Properties → Debug
        const API_BASE = "https://localhost:7121/api/student";

        // ── AUTO LOAD ON PAGE READY ──────────────────────────────────────────
        // $(document).ready() runs as soon as the HTML page has fully loaded
        $(document).ready(function () {
            loadStudents(); // Automatically load all students when page opens
        });

        // ── LOAD STUDENTS FUNCTION ───────────────────────────────────────────
        // Fetches students from API and fills the HTML table
        function loadStudents() {
            let name = $("#search").val().trim();       // Get search name value
            let city = $("#filterCity").val();           // Get selected city

            // If user typed name or selected city → use search endpoint
            // Otherwise → get all students
            let url = (name || city)
                ? `${API_BASE}/search?name=${name}&city=${city}`
                : API_BASE;

            // Show loading message while waiting for API response
            $("#loadingMsg").show();
            $("#noDataMsg").hide();
            $("#tableBody").html(""); // Clear old rows

            // AJAX GET request to the API
            $.ajax({
                url: url,
                type: "GET",

                success: function (data) {
                    // 'data' is an array of student objects from the API
                    $("#loadingMsg").hide();

                    if (data.length === 0) {
                        // No students returned
                        $("#noDataMsg").show();
                        return;
                    }

                    let rows = "";

                    // Loop through each student and build a table row
                    $.each(data, function (index, s) {
                        rows += `
                            <tr>
                                <td>${index + 1}</td>
                                <td>${s.name}</td>
                                <td>${s.email}</td>
                                <td>${s.mobile}</td>
                                <td>${s.age}</td>
                                <td>${s.city}</td>
                                <td>${s.gender}</td>
                                <td>
                                    <button class="btn-edit" onclick="editStudent(${s.id})">✏ Edit</button>
                                    <button class="btn-delete" onclick="deleteStudent(${s.id})">🗑 Delete</button>
                                </td>
                            </tr>`;
                    });

                    // Inject all rows into the table body at once
                    $("#tableBody").html(rows);
                },

                error: function (err) {
                    // This runs if the API is down or returned an error
                    $("#loadingMsg").hide();
                    alert("❌ Error loading students! Is the API running?\n" + err.statusText);
                    console.error("API Error:", err); // Check browser F12 → Console
                }
            });
        }

        // ── DELETE STUDENT ───────────────────────────────────────────────────
        function deleteStudent(id) {
            // Ask user to confirm before deleting
            if (!confirm("Are you sure you want to delete this student?"))
                return; // User clicked Cancel → do nothing

            // AJAX DELETE request
            $.ajax({
                url: API_BASE + "/" + id,  // e.g. https://localhost:7121/api/student/5
                type: "DELETE",

                success: function () {
                    alert("✅ Deleted successfully!");
                    loadStudents(); // Refresh the table after deletion
                },

                error: function (err) {
                    alert("❌ Delete failed! " + err.statusText);
                    console.error("Delete Error:", err);
                }
            });
        }

        // ── EDIT STUDENT ─────────────────────────────────────────────────────
        // Navigate to Create page with the student's ID in the URL
        // The Create page will detect the ID and load data for editing
        function editStudent(id) {
            window.location.href = "/Home/Create?id=" + id;
        }
    </script>

</body>
</html>

─────────────────────────────────────────────────────
FILE 8: Views/Home/Create.cshtml (Add / Edit Form Page)
─────────────────────────────────────────────────────
@*
   This page serves BOTH purposes:
   
   ADD MODE:  URL = /Home/Create         → blank form → POST to API
   EDIT MODE: URL = /Home/Create?id=5    → loads existing data → PUT to API
   
   HOW IT DETECTS MODE:
   JavaScript reads the URL using URLSearchParams.
   If id exists → fetch student data from API → fill form → show "Update" button.
   If no id → show empty form → show "Save" button.
   
   VALIDATION:
   Client-side validation using JavaScript BEFORE sending to API.
   Also checks duplicate email via API in real-time (on blur event).
*@

<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8" />
    <title>Student Form</title>
    <style>
        body { font-family: Arial, sans-serif; margin: 30px; }
        h2 { color: #333; }
        .form-group { margin-bottom: 15px; }
        label { display: block; margin-bottom: 5px; font-weight: bold; color: #555; }
        input[type=text], input[type=number], input[type=email], select {
            width: 300px; padding: 8px; border: 1px solid #ccc;
            border-radius: 4px; font-size: 14px;
        }
        input:focus { border-color: #4CAF50; outline: none; }
        .error { color: red; font-size: 13px; display: block; margin-top: 3px; }
        .success { color: green; font-size: 13px; display: block; margin-top: 3px; }
        .btn-save { background: #4CAF50; color: white; border: none; padding: 10px 25px;
                    border-radius: 4px; font-size: 16px; cursor: pointer; }
        .btn-save:hover { background: #45a049; }
        .btn-back { text-decoration: none; margin-left: 15px; color: #2196F3; }
        .radio-group { margin-top: 5px; }
        .radio-group label { display: inline; margin-right: 15px; font-weight: normal; }
    </style>
</head>
<body>

    <!-- Title changes based on Add/Edit mode (set by JavaScript) -->
    <h2 id="pageTitle">Add Student</h2>

    <!-- Hidden field: stores the student ID when editing -->
    <!-- Value is 0 for new student, actual ID for edit -->
    <input type="hidden" id="studentId" value="0" />

    <!-- NAME -->
    <div class="form-group">
        <label for="name">Full Name</label>
        <input type="text" id="name" placeholder="Enter name (min 3 characters)" />
        <span class="error" id="nameError"></span>
    </div>

    <!-- EMAIL -->
    <div class="form-group">
        <label for="email">Email Address</label>
        <input type="text" id="email" placeholder="Enter email" />
        <span id="emailMsg"></span> <!-- Shows duplicate/valid message -->
        <span class="error" id="emailError"></span>
    </div>

    <!-- MOBILE -->
    <div class="form-group">
        <label for="mobile">Mobile Number</label>
        <input type="text" id="mobile" placeholder="10-digit mobile number" maxlength="10" />
        <span class="error" id="mobileError"></span>
    </div>

    <!-- AGE -->
    <div class="form-group">
        <label for="age">Age</label>
        <input type="number" id="age" placeholder="Must be 18+" min="18" max="100" />
        <span class="error" id="ageError"></span>
    </div>

    <!-- CITY DROPDOWN -->
    <div class="form-group">
        <label for="city">City</label>
        <select id="city">
            <option value="">-- Select City --</option>
            <option value="Ahmedabad">Ahmedabad</option>
            <option value="Nadiad">Nadiad</option>
        </select>
        <span class="error" id="cityError"></span>
    </div>

    <!-- GENDER RADIO BUTTONS -->
    <div class="form-group">
        <label>Gender</label>
        <div class="radio-group">
            <label><input type="radio" name="gender" value="Male" /> Male</label>
            <label><input type="radio" name="gender" value="Female" /> Female</label>
        </div>
        <span class="error" id="genderError"></span>
    </div>

    <!-- BUTTONS -->
    <div class="form-group">
        <!-- Button text changes to "Update" in edit mode -->
        <button class="btn-save" onclick="saveStudent()">💾 Save</button>
        <a href="/Home/Index" class="btn-back">← Back to List</a>
    </div>

    <!-- jQuery CDN -->
    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>

    <script>
        // ── CONFIGURATION ───────────────────────────────────────────────────
        // Must match your StudentAPI port!
        const API_BASE = "https://localhost:7121/api/student";

        // ── PAGE LOAD: CHECK IF EDIT MODE ────────────────────────────────────
        // URLSearchParams reads query string from the URL
        // Example: /Home/Create?id=5  → params.get("id") = "5"
        const params = new URLSearchParams(window.location.search);
        const editId = params.get("id"); // null if no id in URL

        if (editId) {
            // EDIT MODE: Load existing student data into form
            $("#pageTitle").text("Edit Student");
            $("button.btn-save").text("✏ Update");
            loadStudentForEdit(editId);
        }

        // ── LOAD STUDENT DATA FOR EDITING ────────────────────────────────────
        function loadStudentForEdit(id) {
            $.ajax({
                url: API_BASE + "/" + id,
                type: "GET",
                success: function (s) {
                    // Fill all form fields with the student's data
                    $("#studentId").val(s.id);
                    $("#name").val(s.name);
                    $("#email").val(s.email);
                    $("#mobile").val(s.mobile);
                    $("#age").val(s.age);
                    $("#city").val(s.city);
                    // Check the correct radio button for gender
                    $(`input[name=gender][value="${s.gender}"]`).prop("checked", true);
                },
                error: function () {
                    alert("❌ Failed to load student data!");
                }
            });
        }

        // ── REAL-TIME EMAIL DUPLICATE CHECK ──────────────────────────────────
        // "blur" event fires when user clicks away from the email field
        $("#email").on("blur", function () {
            let email = $(this).val().trim();
            if (!email) return; // Don't check empty

            // Call API to check if this email already exists
            $.get(`${API_BASE}/checkEmail?email=${email}`, function (result) {
                if (result.exists) {
                    // Email is already in database
                    $("#emailMsg").removeClass("success").addClass("error")
                        .text("⚠ This email is already registered!");
                } else {
                    $("#emailMsg").removeClass("error").addClass("success")
                        .text("✅ Email is available");
                }
            });
        });

        // ── CLEAR VALIDATION ERRORS ───────────────────────────────────────────
        // Call this before showing fresh errors
        function clearErrors() {
            $(".error").text("");
            $("#emailMsg").text("");
        }

        // ── VALIDATION FUNCTION ───────────────────────────────────────────────
        // Returns true if all fields are valid, false if anything is wrong
        function validate() {
            clearErrors();
            let isValid = true;

            let name   = $("#name").val().trim();
            let email  = $("#email").val().trim();
            let mobile = $("#mobile").val().trim();
            let age    = parseInt($("#age").val());
            let city   = $("#city").val();
            let gender = $("input[name=gender]:checked").val();

            // Name must be at least 3 characters
            if (name.length < 3) {
                $("#nameError").text("Name must be at least 3 characters");
                isValid = false;
            }

            // Email format check using RegExp (pattern matching)
            // ^ = start, $ = end, [^ ]+ = any chars except space
            let emailPattern = /^[^\s@@]+@@[^\s@@]+\.[a-z]{2,3}$/i;
            if (!emailPattern.test(email)) {
                $("#emailError").text("Enter a valid email address");
                isValid = false;
            }

            // Mobile must be exactly 10 digits (numbers only)
            if (!/^\d{10}$/.test(mobile)) {
                $("#mobileError").text("Mobile must be exactly 10 digits");
                isValid = false;
            }

            // Age must be 18 or older
            if (isNaN(age) || age < 18) {
                $("#ageError").text("Age must be 18 or above");
                isValid = false;
            }

            // City must be selected
            if (!city) {
                $("#cityError").text("Please select a city");
                isValid = false;
            }

            // Gender must be selected
            if (!gender) {
                $("#genderError").text("Please select a gender");
                isValid = false;
            }

            return isValid;
        }

        // ── SAVE / UPDATE STUDENT ─────────────────────────────────────────────
        function saveStudent() {
            // Run validation first — stop if anything is invalid
            if (!validate()) return;

            let id = parseInt($("#studentId").val()); // 0 = new, >0 = edit

            // Build student object from form fields
            let student = {
                id:     id,
                name:   $("#name").val().trim(),
                email:  $("#email").val().trim(),
                mobile: $("#mobile").val().trim(),
                age:    parseInt($("#age").val()),
                city:   $("#city").val(),
                gender: $("input[name=gender]:checked").val()
            };

            // If id is 0 → POST (create new)
            // If id > 0  → PUT (update existing)
            let method = (id === 0) ? "POST" : "PUT";
            let url    = (id === 0) ? API_BASE : API_BASE + "/" + id;

            $.ajax({
                url: url,
                type: method,

                // contentType tells API we're sending JSON
                contentType: "application/json",

                // JSON.stringify converts JS object → JSON string for sending
                data: JSON.stringify(student),

                success: function () {
                    alert(id === 0 ? "✅ Student added!" : "✅ Student updated!");
                    // Redirect back to list after saving
                    window.location.href = "/Home/Index";
                },

                error: function (err) {
                    // Log the full error details to console for debugging
                    console.error("Save Error:", err.responseText);

                    // Try to show validation errors from the API
                    if (err.status === 400) {
                        alert("⚠ Validation Error: " + err.responseText);
                    } else {
                        alert("❌ Error saving student! Check console for details.");
                    }
                }
            });
        }
    </script>

</body>
</html>

─────────────────────────────────────────────────────
FILE 9: Program.cs (StudentMVC - Frontend startup)
─────────────────────────────────────────────────────
// This is the startup file for the MVC frontend project.
// Much simpler than the API's Program.cs since:
// - No database
// - No Entity Framework
// - No CORS needed (browser talks to API, not this server to API)

var builder = WebApplication.CreateBuilder(args);

// Register MVC services (Controllers + Views)
// This enables Razor views (.cshtml files) and routing
builder.Services.AddControllersWithViews();

var app = builder.Build();

// In production: use custom error page
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// Enable serving static files from wwwroot folder
// (CSS, JS, images stored in wwwroot are served automatically)
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

// Default route: controller=Home, action=Index
// So visiting "/" goes to HomeController.Index() → Views/Home/Index.cshtml
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
              STEP 4: RUN BOTH PROJECTS SIMULTANEOUSLY
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

By default Visual Studio only runs one project. To run BOTH:

Option A (Easy): Right-click on Solution → Properties
  → Common Properties → Startup Project
  → Select "Multiple startup projects"
  → Set StudentAPI  → Action: Start
  → Set StudentMVC  → Action: Start
  → Click OK → Press F5

Option B: Open two separate VS instances (one per project)

After running, you will see TWO browser windows open:
  • StudentAPI → https://localhost:7121/swagger   (test API here)
  • StudentMVC → https://localhost:5001           (use the app here)

⚠ IMPORTANT: Note your actual StudentAPI port number!
  Check it from: StudentAPI → Properties → Debug → App URL
  Then update the API_BASE variable in BOTH .cshtml files to match!

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
                     STEP 5: TEST THE API WITH SWAGGER
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Open: https://localhost:7121/swagger

You will see all 6 endpoints listed:

  GET    /api/student              Click "Try it out" → Execute → See all students
  GET    /api/student/{id}         Enter id=1 → Execute → See one student
  POST   /api/student              Click "Try it out" → Edit JSON body → Execute → Add student
  PUT    /api/student/{id}         Enter id + updated JSON → Execute → Update
  DELETE /api/student/{id}         Enter id → Execute → Delete
  GET    /api/student/search       Enter name/city params → Execute → Search
  GET    /api/student/checkEmail   Enter email → Execute → Check duplicate

Test POST with this JSON body:
{
  "id": 0,
  "name": "Raj Patel",
  "email": "raj@gmail.com",
  "mobile": "9876543210",
  "age": 21,
  "city": "Ahmedabad",
  "gender": "Male"
}

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
                     TROUBLESHOOTING - COMMON ERRORS
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

PROBLEM 1: AJAX not working / "Access-Control-Allow-Origin" error
  CAUSE:    CORS not configured properly in StudentAPI
  FIX:      Make sure app.UseCors("AllowAll") is in StudentAPI Program.cs
            AND builder.Services.AddCors(...) is registered BEFORE app.Build()
            Check browser F12 → Console for exact error

PROBLEM 2: AJAX call fails with "Failed to fetch" or "net::ERR_CONNECTION_REFUSED"
  CAUSE:    StudentAPI is not running
  FIX:      Make sure both projects are running simultaneously
            Check API_BASE URL and port in your .cshtml files matches actual port

PROBLEM 3: Port mismatch - AJAX calls wrong URL
  CAUSE:    API_BASE in .cshtml has wrong port number
  FIX:      Right-click StudentAPI → Properties → Debug → copy the https URL
            Update API_BASE in Index.cshtml and Create.cshtml

PROBLEM 4: Update-Database fails with "Cannot open database"
  CAUSE:    SQL Server not running or connection string wrong
  FIX:      Open SQL Server Configuration Manager → Start SQL Server service
            OR change "Server=." to "Server=localhost" in appsettings.json

PROBLEM 5: "No parameterless constructor" or DI error
  CAUSE:    Missing namespace or wrong registration in Program.cs
  FIX:      Check that AppDbContext namespace matches in Data/AppDbContext.cs
            Check using directives at top of Program.cs

PROBLEM 6: 400 Bad Request on POST
  CAUSE:    JSON property names don't match model property names
  CAUSE:    Validation failed (e.g. age < 18, email format wrong)
  FIX:      Make sure contentType: "application/json" is in your $.ajax call
            Check that JSON.stringify(student) is used (not just student)
            Check API response body in F12 → Network tab for details

PROBLEM 7: Email validation regex not working in .cshtml
  CAUSE:    Razor treats @ as Razor syntax → use @@ instead of @
  FIX:      In .cshtml files: /^[^\s@@]+@@[^\s@@]+\.[a-z]{2,3}$/ (double @@)
            In .js files: /^[^\s@]+@[^\s@]+\.[a-z]{2,3}$/ (single @)

DEBUGGING TIPS:
  • F12 in browser → Network tab → click failed request → see full error
  • F12 → Console tab → see JavaScript errors
  • Swagger → test API directly without frontend
  • Check Visual Studio "Output" window for server-side errors

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
                         QUICK REFERENCE SUMMARY
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

MIGRATIONS (Run in Package Manager Console with Default Project = StudentAPI):
  Add-Migration InitialCreate     → Creates migration file
  Update-Database                 → Applies migration to SQL Server
  Add-Migration <Name>            → After changing Student model
  Remove-Migration                → Undo last uncommitted migration
  Drop-Database                   → Delete database entirely

API ENDPOINTS QUICK REFERENCE:
  GET    /api/student                         → All students
  GET    /api/student/{id}                    → One student
  POST   /api/student               + body    → Create student
  PUT    /api/student/{id}          + body    → Update student
  DELETE /api/student/{id}                    → Delete student
  GET    /api/student/search?name=&city=      → Filter students
  GET    /api/student/checkEmail?email=       → Duplicate check

MVC ROUTES:
  /           or   /Home/Index    → Student list page
  /Home/Create                    → Add new student
  /Home/Create?id=5               → Edit student with ID 5

VIVA ANSWERS:
  Q: What is CORS?
  A: Cross-Origin Resource Sharing. Allows browser to make API calls
     to a different domain/port. Required because frontend (port 5001)
     calls backend API (port 7121) — different origins.

  Q: What is Entity Framework?
  A: ORM (Object-Relational Mapper). Maps C# classes to database tables.
     Lets you write C# code instead of SQL queries.

  Q: What is AJAX?
  A: Asynchronous JavaScript and XML. Allows web page to call server
     without reloading the full page. We use jQuery's $.ajax() method.

  Q: Why split into two projects?
  A: Separation of concerns. Backend can be reused by mobile apps,
     other frontends, etc. Frontend can be replaced without touching API.
     Better for team development and maintenance.

  Q: What is Swagger?
  A: Auto-generated API documentation + testing UI. Lets you test all
     API endpoints without writing any frontend code.

  Q: What is [ApiController] attribute?
  A: Enables automatic model validation, automatic 400 response for
     invalid data, and binding source inference ([FromBody] etc.)

  Q: What is DbContext?
  A: The "bridge" between C# code and the database. Manages database
     connections, tracks changes, and handles transactions.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
                         COMPLETE SETUP CHECKLIST
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

  [ ] Created blank Solution (StudentSolution)
  [ ] Added StudentAPI project (ASP.NET Core Web API)
  [ ] Added StudentMVC project (ASP.NET Core MVC)
  [ ] Installed EF NuGet packages in StudentAPI
  [ ] Created Models/Student.cs in StudentAPI
  [ ] Created Data/AppDbContext.cs in StudentAPI
  [ ] Created Controllers/StudentController.cs in StudentAPI
  [ ] Updated Program.cs in StudentAPI (CORS, DB, Swagger)
  [ ] Updated appsettings.json in StudentAPI (connection string)
  [ ] Ran Add-Migration InitialCreate (Default Project = StudentAPI)
  [ ] Ran Update-Database
  [ ] Verified StudentDB table in SQL Server (SSMS)
  [ ] Created Controllers/HomeController.cs in StudentMVC
  [ ] Created Views/Home/Index.cshtml in StudentMVC
  [ ] Created Views/Home/Create.cshtml in StudentMVC
  [ ] Updated Program.cs in StudentMVC
  [ ] Updated API_BASE port in Index.cshtml and Create.cshtml
  [ ] Set Multiple Startup Projects in Solution properties
  [ ] Ran both projects → tested in browser ✅

═══════════════════════════════════════════════════════════════════
  All done! 🎉 StudentAPI serves data. StudentMVC shows the UI.
  They communicate via AJAX (jQuery HTTP calls).
═══════════════════════════════════════════════════════════════════