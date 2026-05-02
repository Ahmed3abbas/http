╔══════════════════════════════════════════════════════════════════════════════════╗
║        TWO-TABLE EXAM REFERENCE GUIDE (BEGINNER LEVEL)                         ║
║        ASP.NET Core Web API + Entity Framework + MVC + AJAX                    ║
║        Example: Department → Student  (One-to-Many Relationship)               ║
╚══════════════════════════════════════════════════════════════════════════════════╝

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
                     WHAT IS A TWO-TABLE RELATIONSHIP?
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

  ONE DEPARTMENT can have MANY STUDENTS  →  One-to-Many relationship

  Departments Table                    Students Table
  ┌────┬────────────┐                  ┌────┬──────────┬──────────────────┐
  │ Id │ Name       │                  │ Id │ Name     │ DepartmentId(FK) │
  ├────┼────────────┤                  ├────┼──────────┼──────────────────┤
  │  1 │ Computer   │◄─────────────────│  1 │ Raj      │        1         │
  │  2 │ Mechanical │                  │  2 │ Priya    │        1         │
  │  3 │ Civil      │◄─────────────────│  3 │ Amit     │        3         │
  └────┴────────────┘                  └────┴──────────┴──────────────────┘

  FK = Foreign Key: DepartmentId in Students links to Id in Departments
  This ensures every student belongs to a valid department.

  RELATIONSHIP TYPES (for viva):
  ┌──────────────────┬──────────────────────────────────────────────────┐
  │ One-to-Many      │ One Department → Many Students (most common)     │
  │ Many-to-Many     │ Students ↔ Courses (needs a 3rd junction table)  │
  │ One-to-One       │ Student ↔ StudentProfile (rare in exams)         │
  └──────────────────┴──────────────────────────────────────────────────┘

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
                         PROJECT FOLDER STRUCTURE
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

StudentSolution/
│
├── StudentAPI/                         ← BACKEND
│   ├── Models/
│   │   ├── Department.cs               ← Table 1 (Parent)
│   │   └── Student.cs                  ← Table 2 (Child, has FK)
│   ├── Data/
│   │   └── AppDbContext.cs             ← Both tables registered here
│   ├── Controllers/
│   │   ├── DepartmentController.cs     ← CRUD for Department
│   │   └── StudentController.cs        ← CRUD for Student (with Dept info)
│   ├── Program.cs
│   └── appsettings.json
│
└── StudentMVC/                         ← FRONTEND
    ├── Controllers/
    │   └── HomeController.cs
    ├── Views/Home/
    │   ├── Index.cshtml                ← List students + their department
    │   └── Create.cshtml               ← Form with department dropdown

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
                       BACKEND: StudentAPI
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

─────────────────────────────────────────────────────────────────────────
FILE 1: Models/Department.cs   ← TABLE 1 (Parent Table)
─────────────────────────────────────────────────────────────────────────
// Department is the PARENT table.
// One department can have MANY students.
// The "ICollection<Student>" navigation property represents that "many" side.
// Navigation properties let you access related data easily in code.

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;  // Needed to avoid circular JSON loop

namespace StudentAPI.Models
{
    public class Department
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Department name is required")]
        public string? Name { get; set; }

        // NAVIGATION PROPERTY (One-to-Many)
        // This tells EF: "One Department has a collection of Students"
        // [JsonIgnore] prevents infinite loop when serializing to JSON
        // (Student has Department, Department has Students → infinite!)
        [JsonIgnore]
        public ICollection<Student>? Students { get; set; }
    }
}


─────────────────────────────────────────────────────────────────────────
FILE 2: Models/Student.cs   ← TABLE 2 (Child Table - has Foreign Key)
─────────────────────────────────────────────────────────────────────────
// Student is the CHILD table.
// It holds DepartmentId as a Foreign Key (FK).
// The "Department" navigation property lets us access department details.

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // Needed for [ForeignKey]

namespace StudentAPI.Models
{
    public class Student
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string? Name { get; set; }

        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Mobile must be 10 digits")]
        public string? Mobile { get; set; }

        [Range(18, 100, ErrorMessage = "Age must be 18+")]
        public int Age { get; set; }

        [Required]
        public string? City { get; set; }

        [Required]
        public string? Gender { get; set; }

        // ── FOREIGN KEY ─────────────────────────────────────────────────────
        // DepartmentId is the FK column stored in the Students table.
        // It references the Id column in the Departments table.
        // This is what links a student to their department in the database.
        public int DepartmentId { get; set; }

        // ── NAVIGATION PROPERTY ──────────────────────────────────────────────
        // [ForeignKey] tells EF which property is the FK for this navigation
        // This lets you write: student.Department.Name  instead of doing a join
        // "?" makes it nullable so it's optional when creating (EF fills it)
        [ForeignKey("DepartmentId")]
        public Department? Department { get; set; }
    }
}


─────────────────────────────────────────────────────────────────────────
FILE 3: Data/AppDbContext.cs   ← Register BOTH tables here
─────────────────────────────────────────────────────────────────────────
// DbContext is the bridge between C# and SQL Server.
// We add a DbSet for EVERY table we want EF to manage.
// EF will see the FK and navigation properties and create the relationship.

using Microsoft.EntityFrameworkCore;
using StudentAPI.Models;

namespace StudentAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // TABLE 1: Departments
        public DbSet<Department> Departments { get; set; }

        // TABLE 2: Students
        // EF automatically creates the FK constraint in SQL because
        // Student model has DepartmentId + Department navigation property
        public DbSet<Student> Students { get; set; }
    }
}


─────────────────────────────────────────────────────────────────────────
FILE 4: Controllers/DepartmentController.cs   ← API for Department Table
─────────────────────────────────────────────────────────────────────────
// Manages the Departments table.
// Endpoints:
//   GET    /api/department         → All departments
//   GET    /api/department/5       → One department
//   POST   /api/department         → Add department
//   PUT    /api/department/5       → Update department
//   DELETE /api/department/5       → Delete department

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentAPI.Data;
using StudentAPI.Models;

namespace StudentAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DepartmentController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/department
        // Returns all departments (used to populate dropdown in frontend form)
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var departments = await _context.Departments.ToListAsync();
            return Ok(departments);
        }

        // GET: api/department/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var dept = await _context.Departments.FindAsync(id);
            if (dept == null) return NotFound(new { message = "Department not found" });
            return Ok(dept);
        }

        // POST: api/department
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Department d)
        {
            if (d == null) return BadRequest("Invalid data");

            _context.Departments.Add(d);
            await _context.SaveChangesAsync();
            return Ok(d);
        }

        // PUT: api/department/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Department d)
        {
            var existing = await _context.Departments.FindAsync(id);
            if (existing == null) return NotFound();

            existing.Name = d.Name;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Updated" });
        }

        // DELETE: api/department/5
        // NOTE: Cannot delete a department if students are linked to it!
        // SQL will throw a FK violation error. Handle this carefully.
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var dept = await _context.Departments.FindAsync(id);
            if (dept == null) return NotFound();

            // Check if any students are linked to this department
            bool hasStudents = await _context.Students.AnyAsync(s => s.DepartmentId == id);
            if (hasStudents)
                return BadRequest(new { message = "Cannot delete: students are linked to this department" });

            _context.Departments.Remove(dept);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Deleted" });
        }
    }
}


─────────────────────────────────────────────────────────────────────────
FILE 5: Controllers/StudentController.cs   ← API for Student Table
─────────────────────────────────────────────────────────────────────────
// KEY DIFFERENCE from single-table version:
// We use .Include(s => s.Department) to JOIN with Departments table.
// This is called EAGER LOADING — loads related data automatically.
// Without Include(), student.Department would be NULL.
//
// IMPORTANT EXAM CONCEPT:
// .Include() = SQL JOIN done by Entity Framework
// Without Include: SELECT * FROM Students
// With Include:    SELECT s.*, d.* FROM Students s
//                  JOIN Departments d ON s.DepartmentId = d.Id

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentAPI.Data;
using StudentAPI.Models;

namespace StudentAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly AppDbContext _context;

        public StudentController(AppDbContext context)
        {
            _context = context;
        }

        // ─────────────────────────────────────────────────────────────────
        // GET: api/student
        // Returns ALL students WITH their department name
        // .Include() performs the JOIN to load Department data
        // ─────────────────────────────────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var students = await _context.Students
                .Include(s => s.Department) // JOIN Departments table
                .ToListAsync();

            return Ok(students);
        }

        // ─────────────────────────────────────────────────────────────────
        // GET: api/student/5
        // Returns ONE student with department info
        // ─────────────────────────────────────────────────────────────────
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var student = await _context.Students
                .Include(s => s.Department) // Need Include here too!
                .FirstOrDefaultAsync(s => s.Id == id); // Can't use FindAsync with Include

            if (student == null)
                return NotFound(new { message = "Student not found" });

            return Ok(student);
        }

        // ─────────────────────────────────────────────────────────────────
        // POST: api/student
        // Adds a new student. DepartmentId must exist in Departments table.
        // ─────────────────────────────────────────────────────────────────
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Student s)
        {
            if (s == null) return BadRequest("No data received");

            // Validate that the DepartmentId actually exists
            bool deptExists = await _context.Departments.AnyAsync(d => d.Id == s.DepartmentId);
            if (!deptExists)
                return BadRequest(new { message = "Invalid DepartmentId" });

            _context.Students.Add(s);
            await _context.SaveChangesAsync();
            return Ok(s);
        }

        // ─────────────────────────────────────────────────────────────────
        // PUT: api/student/5
        // ─────────────────────────────────────────────────────────────────
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Student s)
        {
            var existing = await _context.Students.FindAsync(id);
            if (existing == null) return NotFound();

            // Update all fields including DepartmentId (user can change dept)
            existing.Name         = s.Name;
            existing.Email        = s.Email;
            existing.Mobile       = s.Mobile;
            existing.Age          = s.Age;
            existing.City         = s.City;
            existing.Gender       = s.Gender;
            existing.DepartmentId = s.DepartmentId; // Update FK too

            await _context.SaveChangesAsync();
            return Ok(new { message = "Updated" });
        }

        // ─────────────────────────────────────────────────────────────────
        // DELETE: api/student/5
        // ─────────────────────────────────────────────────────────────────
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null) return NotFound();

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Deleted" });
        }

        // ─────────────────────────────────────────────────────────────────
        // GET: api/student/search?name=Raj&departmentId=1
        // Search by name and/or department
        // ─────────────────────────────────────────────────────────────────
        [HttpGet("search")]
        public async Task<IActionResult> Search(string? name, int? departmentId)
        {
            var query = _context.Students
                .Include(s => s.Department) // Always include department
                .AsQueryable();

            if (!string.IsNullOrEmpty(name))
                query = query.Where(s => s.Name!.Contains(name));

            if (departmentId.HasValue && departmentId > 0)
                query = query.Where(s => s.DepartmentId == departmentId);

            return Ok(await query.ToListAsync());
        }

        // ─────────────────────────────────────────────────────────────────
        // GET: api/student/byDepartment/1
        // Get all students in a specific department
        // ─────────────────────────────────────────────────────────────────
        [HttpGet("byDepartment/{deptId}")]
        public async Task<IActionResult> GetByDepartment(int deptId)
        {
            var students = await _context.Students
                .Include(s => s.Department)
                .Where(s => s.DepartmentId == deptId)
                .ToListAsync();

            return Ok(students);
        }

        // ─────────────────────────────────────────────────────────────────
        // GET: api/student/checkEmail?email=test@test.com
        // ─────────────────────────────────────────────────────────────────
        [HttpGet("checkEmail")]
        public IActionResult CheckEmail(string email)
        {
            bool exists = _context.Students.Any(x => x.Email == email);
            return Ok(new { exists });
        }
    }
}


─────────────────────────────────────────────────────────────────────────
FILE 6: Program.cs (StudentAPI)
─────────────────────────────────────────────────────────────────────────
// Same as single-table version. No changes needed here.

using Microsoft.EntityFrameworkCore;
using StudentAPI.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();


─────────────────────────────────────────────────────────────────────────
FILE 7: appsettings.json (StudentAPI)
─────────────────────────────────────────────────────────────────────────
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=StudentDeptDB;Trusted_Connection=True;TrustServerCertificate=True;"
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
                   MIGRATION COMMANDS (Two Tables)
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Open: Tools → NuGet Package Manager → Package Manager Console
Set Default Project dropdown to: StudentAPI

  Step 1: Add migration (EF reads both models, creates snapshot)
  ┌──────────────────────────────────────────────┐
  │  Add-Migration TwoTableSetup                 │
  └──────────────────────────────────────────────┘

  Step 2: Apply migration to SQL Server
  ┌──────────────────────────────────────────────┐
  │  Update-Database                             │
  └──────────────────────────────────────────────┘

  EF automatically creates:
  ✅ Departments table (Id, Name)
  ✅ Students table (Id, Name, Email, Mobile, Age, City, Gender, DepartmentId)
  ✅ FOREIGN KEY constraint on Students.DepartmentId → Departments.Id
  ✅ __EFMigrationsHistory table

  VERIFY in SSMS → StudentDeptDB → Tables:
  → dbo.Departments
  → dbo.Students (check DepartmentId column exists)
  → Right-click Students → Design → see FK constraint

  ⚠ ORDER MATTERS: Insert departments FIRST, then students!
    Because students reference departments (FK must exist first).

  Seed test data in SSMS:
  INSERT INTO Departments VALUES ('Computer'), ('Mechanical'), ('Civil')
  INSERT INTO Students VALUES ('Raj','raj@test.com','9876543210',21,'Ahmedabad','Male',1)
  INSERT INTO Students VALUES ('Priya','priya@test.com','9876543211',20,'Nadiad','Female',1)
  INSERT INTO Students VALUES ('Amit','amit@test.com','9876543212',22,'Ahmedabad','Male',3)


━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
                       FRONTEND: StudentMVC
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

─────────────────────────────────────────────────────────────────────────
FILE 8: Controllers/HomeController.cs (StudentMVC)
─────────────────────────────────────────────────────────────────────────
// Exactly the same as single-table version.
// No changes needed — frontend controller just returns views.

using Microsoft.AspNetCore.Mvc;

namespace StudentMVC.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()  => View();
        public IActionResult Create() => View();
    }
}


─────────────────────────────────────────────────────────────────────────
FILE 9: Views/Home/Index.cshtml (List Page with Department Column)
─────────────────────────────────────────────────────────────────────────
@*
   KEY DIFFERENCE from single-table:
   - Table now has a "Department" column
   - Search dropdown shows departments (loaded from API)
   - API returns: student.department.name → we access s.department.name in JS
   
   The API returns JSON like this when .Include() is used:
   {
     "id": 1,
     "name": "Raj",
     "email": "raj@test.com",
     "departmentId": 1,
     "department": {          ← nested object from JOIN
       "id": 1,
       "name": "Computer"
     }
   }
*@

<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8" />
    <title>Student List</title>
    <style>
        body { font-family: Arial, sans-serif; margin: 30px; }
        table { border-collapse: collapse; width: 100%; margin-top: 15px; }
        th, td { border: 1px solid #ccc; padding: 10px; text-align: left; }
        th { background-color: #3f51b5; color: white; }
        tr:nth-child(even) { background-color: #f5f5f5; }
        .btn-edit   { background: #2196F3; color: white; border: none; padding: 5px 10px; border-radius: 3px; cursor: pointer; }
        .btn-delete { background: #f44336; color: white; border: none; padding: 5px 10px; border-radius: 3px; cursor: pointer; }
        .btn-search { background: #4CAF50; color: white; border: none; padding: 8px 15px; border-radius: 3px; cursor: pointer; }
        .btn-add    { background: #FF9800; color: white; text-decoration: none; padding: 8px 15px; border-radius: 3px; }
        h2 { color: #3f51b5; }
    </style>
</head>
<body>

    <h2>Student List</h2>

    <div style="margin-bottom:15px;">
        <input type="text" id="search" placeholder="Search by Name" style="padding:7px; width:180px;" />

        <!-- Department filter dropdown — populated by AJAX from API -->
        <select id="filterDept" style="padding:7px;">
            <option value="">All Departments</option>
            <!-- Options added by loadDepartments() below -->
        </select>

        <button class="btn-search" onclick="loadStudents()">Search</button>
        <a href="/Home/Create" class="btn-add" style="margin-left:15px;">+ Add Student</a>
    </div>

    <table>
        <thead>
            <tr>
                <th>#</th>
                <th>Name</th>
                <th>Email</th>
                <th>Mobile</th>
                <th>Age</th>
                <th>City</th>
                <th>Gender</th>
                <th>Department</th>  <!-- NEW COLUMN -->
                <th>Actions</th>
            </tr>
        </thead>
        <tbody id="tableBody"></tbody>
    </table>

    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
    <script>
        const API = "https://localhost:7121/api";  // Base API URL

        // ── ON PAGE LOAD ──────────────────────────────────────────────────
        $(document).ready(function () {
            loadDepartments(); // Fill the department dropdown first
            loadStudents();    // Then load students
        });

        // ── LOAD DEPARTMENTS INTO DROPDOWN ────────────────────────────────
        // Called once on page load to populate filter dropdown
        function loadDepartments() {
            $.get(`${API}/department`, function (data) {
                $.each(data, function (i, d) {
                    // Add each department as an <option> in the select
                    $("#filterDept").append(
                        `<option value="${d.id}">${d.name}</option>`
                    );
                });
            });
        }

        // ── LOAD STUDENTS TABLE ───────────────────────────────────────────
        function loadStudents() {
            let name   = $("#search").val().trim();
            let deptId = $("#filterDept").val();

            // Decide which API endpoint to call
            let url;
            if (name || deptId) {
                url = `${API}/student/search?name=${name}&departmentId=${deptId}`;
            } else {
                url = `${API}/student`;
            }

            $.ajax({
                url: url,
                type: "GET",
                success: function (data) {
                    if (data.length === 0) {
                        $("#tableBody").html("<tr><td colspan='9'>No students found</td></tr>");
                        return;
                    }

                    let rows = "";
                    $.each(data, function (i, s) {
                        // Access nested department name: s.department.name
                        // If department is null (shouldn't happen), show "N/A"
                        let deptName = s.department ? s.department.name : "N/A";

                        rows += `<tr>
                            <td>${i + 1}</td>
                            <td>${s.name}</td>
                            <td>${s.email}</td>
                            <td>${s.mobile}</td>
                            <td>${s.age}</td>
                            <td>${s.city}</td>
                            <td>${s.gender}</td>
                            <td><b>${deptName}</b></td>
                            <td>
                                <button class="btn-edit"   onclick="editStudent(${s.id})">✏ Edit</button>
                                <button class="btn-delete" onclick="deleteStudent(${s.id})">🗑 Delete</button>
                            </td>
                        </tr>`;
                    });

                    $("#tableBody").html(rows);
                },
                error: function () {
                    alert("❌ Error loading students! Is the API running?");
                }
            });
        }

        // ── DELETE ────────────────────────────────────────────────────────
        function deleteStudent(id) {
            if (!confirm("Delete this student?")) return;

            $.ajax({
                url: `${API}/student/${id}`,
                type: "DELETE",
                success: function () {
                    alert("✅ Deleted!");
                    loadStudents();
                },
                error: function () { alert("❌ Delete failed!"); }
            });
        }

        // ── EDIT ──────────────────────────────────────────────────────────
        function editStudent(id) {
            window.location.href = "/Home/Create?id=" + id;
        }
    </script>

</body>
</html>


─────────────────────────────────────────────────────────────────────────
FILE 10: Views/Home/Create.cshtml (Form with Department Dropdown)
─────────────────────────────────────────────────────────────────────────
@*
   KEY DIFFERENCE from single-table:
   - Department is now a DROPDOWN (not text input)
   - Dropdown options are loaded from API via AJAX on page load
   - student object now includes "departmentId" (integer)
   - In edit mode: after loading student, we pre-select the correct department
*@

<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8" />
    <title>Student Form</title>
    <style>
        body { font-family: Arial, sans-serif; margin: 30px; }
        .form-group { margin-bottom: 15px; }
        label { display: block; margin-bottom: 5px; font-weight: bold; color: #555; }
        input[type=text], input[type=number], select {
            width: 300px; padding: 8px; border: 1px solid #ccc;
            border-radius: 4px; font-size: 14px;
        }
        .error { color: red; font-size: 13px; display: block; margin-top: 3px; }
        .success { color: green; font-size: 13px; }
        .btn-save { background: #3f51b5; color: white; border: none;
                    padding: 10px 25px; border-radius: 4px; font-size: 16px; cursor: pointer; }
        .radio-group label { display: inline; font-weight: normal; margin-right: 15px; }
        a { color: #2196F3; }
    </style>
</head>
<body>

    <h2 id="pageTitle">Add Student</h2>

    <input type="hidden" id="studentId" value="0" />

    <!-- NAME -->
    <div class="form-group">
        <label>Full Name</label>
        <input type="text" id="name" placeholder="Enter name" />
        <span class="error" id="nameError"></span>
    </div>

    <!-- EMAIL -->
    <div class="form-group">
        <label>Email Address</label>
        <input type="text" id="email" placeholder="Enter email" />
        <span id="emailMsg"></span>
        <span class="error" id="emailError"></span>
    </div>

    <!-- MOBILE -->
    <div class="form-group">
        <label>Mobile Number</label>
        <input type="text" id="mobile" placeholder="10-digit number" maxlength="10" />
        <span class="error" id="mobileError"></span>
    </div>

    <!-- AGE -->
    <div class="form-group">
        <label>Age</label>
        <input type="number" id="age" placeholder="18+" min="18" max="100" />
        <span class="error" id="ageError"></span>
    </div>

    <!-- CITY -->
    <div class="form-group">
        <label>City</label>
        <select id="city">
            <option value="">-- Select City --</option>
            <option value="Ahmedabad">Ahmedabad</option>
            <option value="Nadiad">Nadiad</option>
        </select>
        <span class="error" id="cityError"></span>
    </div>

    <!-- GENDER -->
    <div class="form-group">
        <label>Gender</label>
        <div class="radio-group">
            <label><input type="radio" name="gender" value="Male" /> Male</label>
            <label><input type="radio" name="gender" value="Female" /> Female</label>
        </div>
        <span class="error" id="genderError"></span>
    </div>

    <!-- DEPARTMENT DROPDOWN (loaded from API) -->
    <!-- This is the KEY ADDITION for two-table setup -->
    <div class="form-group">
        <label>Department</label>
        <select id="departmentId">
            <option value="0">-- Select Department --</option>
            <!-- Options injected by loadDepartments() -->
        </select>
        <span class="error" id="deptError"></span>
    </div>

    <div class="form-group">
        <button class="btn-save" onclick="saveStudent()">💾 Save</button>
        <a href="/Home/Index" style="margin-left:15px;">← Back to List</a>
    </div>

    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
    <script>
        const API = "https://localhost:7121/api";

        // ── PAGE INIT ─────────────────────────────────────────────────────
        const params = new URLSearchParams(window.location.search);
        const editId = params.get("id");

        $(document).ready(function () {
            // Step 1: Load departments into dropdown FIRST
            loadDepartments(function () {
                // Step 2: AFTER departments load, if edit mode → fill the form
                // We must load departments first so we can pre-select the correct one
                if (editId) {
                    $("#pageTitle").text("Edit Student");
                    $(".btn-save").text("✏ Update");
                    loadStudentForEdit(editId);
                }
            });
        });

        // ── LOAD DEPARTMENTS INTO SELECT DROPDOWN ─────────────────────────
        // 'callback' is a function to run AFTER departments have loaded
        // This is important because AJAX is asynchronous (runs in background)
        function loadDepartments(callback) {
            $.get(`${API}/department`, function (data) {
                $.each(data, function (i, d) {
                    // Build and append each option
                    $("#departmentId").append(
                        `<option value="${d.id}">${d.name}</option>`
                    );
                });

                // Run callback function if provided
                if (typeof callback === "function") callback();
            });
        }

        // ── LOAD STUDENT DATA FOR EDIT ────────────────────────────────────
        function loadStudentForEdit(id) {
            $.get(`${API}/student/${id}`, function (s) {
                $("#studentId").val(s.id);
                $("#name").val(s.name);
                $("#email").val(s.email);
                $("#mobile").val(s.mobile);
                $("#age").val(s.age);
                $("#city").val(s.city);
                $(`input[name=gender][value="${s.gender}"]`).prop("checked", true);

                // Pre-select the correct department in dropdown
                // .val() on a <select> sets the selected option by value
                $("#departmentId").val(s.departmentId);
            });
        }

        // ── EMAIL DUPLICATE CHECK ─────────────────────────────────────────
        $("#email").on("blur", function () {
            let email = $(this).val().trim();
            if (!email) return;

            $.get(`${API}/student/checkEmail?email=${email}`, function (result) {
                if (result.exists) {
                    $("#emailMsg").attr("class", "error").text("⚠ Email already exists!");
                } else {
                    $("#emailMsg").attr("class", "success").text("✅ Email available");
                }
            });
        });

        // ── VALIDATION ────────────────────────────────────────────────────
        function clearErrors() { $(".error").text(""); $("#emailMsg").text(""); }

        function validate() {
            clearErrors();
            let ok = true;

            let name   = $("#name").val().trim();
            let email  = $("#email").val().trim();
            let mobile = $("#mobile").val().trim();
            let age    = parseInt($("#age").val());
            let city   = $("#city").val();
            let gender = $("input[name=gender]:checked").val();
            let deptId = parseInt($("#departmentId").val());

            if (name.length < 3)          { $("#nameError").text("Name min 3 characters");    ok = false; }
            if (!/^[^\s@@]+@@[^\s@@]+\.[a-z]{2,3}$/i.test(email))
                                           { $("#emailError").text("Invalid email");           ok = false; }
            if (!/^\d{10}$/.test(mobile)) { $("#mobileError").text("Mobile must be 10 digits"); ok = false; }
            if (isNaN(age) || age < 18)   { $("#ageError").text("Age must be 18+");            ok = false; }
            if (!city)                    { $("#cityError").text("Select a city");             ok = false; }
            if (!gender)                  { $("#genderError").text("Select gender");           ok = false; }

            // Validate department selection
            if (!deptId || deptId === 0)  { $("#deptError").text("Select a department");      ok = false; }

            return ok;
        }

        // ── SAVE / UPDATE ─────────────────────────────────────────────────
        function saveStudent() {
            if (!validate()) return;

            let id = parseInt($("#studentId").val());

            let student = {
                id:           id,
                name:         $("#name").val().trim(),
                email:        $("#email").val().trim(),
                mobile:       $("#mobile").val().trim(),
                age:          parseInt($("#age").val()),
                city:         $("#city").val(),
                gender:       $("input[name=gender]:checked").val(),
                departmentId: parseInt($("#departmentId").val())  // FK value sent to API
            };

            let method = id === 0 ? "POST" : "PUT";
            let url    = id === 0 ? `${API}/student` : `${API}/student/${id}`;

            $.ajax({
                url:         url,
                type:        method,
                contentType: "application/json",
                data:        JSON.stringify(student),
                success: function () {
                    alert(id === 0 ? "✅ Added!" : "✅ Updated!");
                    window.location.href = "/Home/Index";
                },
                error: function (err) {
                    console.error(err.responseText);
                    alert("❌ Error: " + err.responseText);
                }
            });
        }
    </script>

</body>
</html>


━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
         EXAM SCENARIO VARIATIONS (Same pattern, different tables)
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

If the exam gives you different tables, just change the names.
The PATTERN is exactly the same. Here are common exam combos:

┌──────────────────────────────────────────────────────────────────────┐
│  COMBO 1: Employee + Department  (shown above as Student+Department) │
│  COMBO 2: Product + Category                                         │
│  COMBO 3: Order + Customer                                           │
│  COMBO 4: Book + Author                                              │
│  COMBO 5: Course + Student (Many-to-Many → needs junction table)     │
└──────────────────────────────────────────────────────────────────────┘

HOW TO ADAPT for any two-table combo:
  1. Change class names (Department → Category, Student → Product etc.)
  2. Change property names to match your data
  3. Change controller names and routes accordingly
  4. Migration commands stay exactly the same
  5. .Include() pattern stays exactly the same


── COMBO 2 QUICK EXAMPLE: Product + Category ────────────────────────

  public class Category { public int Id; public string? Name; }

  public class Product {
      public int Id;
      public string? Name;
      public decimal Price;
      public int CategoryId;              // FK
      [ForeignKey("CategoryId")]
      public Category? Category { get; set; } // Navigation
  }

  // In ProductController.Get():
  return Ok(await _context.Products.Include(p => p.Category).ToListAsync());

  // In JS (Index.cshtml):
  // p.category.name   ← access nested category name from API response


━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
           KEY DIFFERENCES: Single Table vs Two Tables (EXAM SUMMARY)
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

  WHAT CHANGES                  SINGLE TABLE        TWO TABLES
  ─────────────────────────────────────────────────────────────────────
  Number of Models              1                   2 (Parent + Child)
  FK property in Child model    ❌ Not needed       ✅ DepartmentId
  Navigation property           ❌ Not needed       ✅ Department? Dept
  [ForeignKey] attribute        ❌ Not needed       ✅ On nav property
  ICollection in Parent         ❌ Not needed       ✅ [JsonIgnore]
  DbSet in DbContext            1                   2
  Number of Controllers         1                   2
  .Include() in queries         ❌ Not needed       ✅ Needed for JOIN
  FindAsync with Include        ❌ Not needed       ✅ Use FirstOrDefault
  JSON response shape           flat object         nested object
  Form dropdown (dept)          ❌ Not needed       ✅ Load from API
  departmentId in JS object     ❌ Not needed       ✅ Must include
  Access dept name in JS        ❌ Not needed       ✅ s.department.name

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
                      ALL API ENDPOINTS QUICK REFERENCE
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

  DEPARTMENT ENDPOINTS:
  GET    /api/department              → All departments
  GET    /api/department/{id}         → One department
  POST   /api/department              → Add department  { "name": "IT" }
  PUT    /api/department/{id}         → Update department
  DELETE /api/department/{id}         → Delete (fails if students linked)

  STUDENT ENDPOINTS:
  GET    /api/student                         → All students + dept name
  GET    /api/student/{id}                    → One student + dept name
  POST   /api/student                         → Add student (with deptId)
  PUT    /api/student/{id}                    → Update student (with deptId)
  DELETE /api/student/{id}                    → Delete student
  GET    /api/student/search?name=&dept=      → Filter
  GET    /api/student/byDepartment/{deptId}   → All students in a dept
  GET    /api/student/checkEmail?email=       → Duplicate check

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
                       VIVA Q&A FOR TWO-TABLE SETUP
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Q: What is a Foreign Key?
A: A column in one table (DepartmentId in Students) that references
   the Primary Key of another table (Id in Departments).
   It enforces referential integrity — you can't have a student
   linked to a department that doesn't exist.

Q: What is a Navigation Property?
A: A property in the C# model that represents a related object.
   Example: Student has a 'Department' navigation property.
   It lets you write student.Department.Name instead of doing a JOIN manually.

Q: What is .Include() in Entity Framework?
A: It tells EF to do a SQL JOIN and load related data together.
   Without Include: Student.Department is NULL.
   With Include: Student.Department has all the department data loaded.
   Equivalent SQL: JOIN Departments ON Students.DepartmentId = Departments.Id

Q: Why can't we use FindAsync() with .Include()?
A: FindAsync() searches only by Primary Key and doesn't support Include().
   For queries with Include(), use FirstOrDefaultAsync() with a Where condition:
   .Include(s => s.Department).FirstOrDefaultAsync(s => s.Id == id)

Q: What is [JsonIgnore]?
A: It tells the JSON serializer to skip that property.
   Used on ICollection<Student> in Department to prevent circular reference:
   Department has Students, each Student has Department → infinite loop!

Q: What is Eager Loading vs Lazy Loading?
A: Eager Loading (.Include()) = loads related data in the same query (recommended)
   Lazy Loading = loads related data only when you access the property (needs config)
   We use Eager Loading with .Include() — it's simpler and more predictable.

Q: What is ICollection<Student> in Department?
A: It represents the "many" side of a One-to-Many relationship.
   One Department has a collection (list) of many Students.
   EF uses this to understand the relationship, but we mark it [JsonIgnore]
   to prevent circular JSON during API response.

Q: Why must departments be inserted before students?
A: Students have a DepartmentId FK that must reference an existing department.
   SQL will reject an INSERT into Students if the DepartmentId doesn't exist
   in the Departments table. This is called Referential Integrity.

Q: Can we delete a department that has students?
A: No. SQL throws a Foreign Key violation error because students reference it.
   Solution: Either delete all linked students first, OR check in the API
   (as shown in DepartmentController.Delete) and return a friendly error.

Q: What does the JSON look like with two related tables?
A: The API returns a nested object:
   {
     "id": 1, "name": "Raj", "departmentId": 1,
     "department": { "id": 1, "name": "Computer" }
   }
   In JavaScript: s.department.name gives you "Computer"

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
                    COMPLETE SETUP CHECKLIST (Two Tables)
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

  [ ] StudentAPI project created
  [ ] StudentMVC project created
  [ ] EF NuGet packages installed in StudentAPI
  [ ] Models/Department.cs created (with ICollection + [JsonIgnore])
  [ ] Models/Student.cs created (with DepartmentId FK + navigation property)
  [ ] Data/AppDbContext.cs updated (both DbSets registered)
  [ ] Controllers/DepartmentController.cs created
  [ ] Controllers/StudentController.cs created (with .Include())
  [ ] Program.cs configured (CORS, EF, Swagger)
  [ ] appsettings.json connection string set
  [ ] Add-Migration TwoTableSetup run (Default Project = StudentAPI)
  [ ] Update-Database run
  [ ] Verified both tables in SSMS (FK constraint visible)
  [ ] Seed data inserted (departments first, then students)
  [ ] HomeController.cs in StudentMVC (Index + Create)
  [ ] Index.cshtml updated (department column + dept filter dropdown)
  [ ] Create.cshtml updated (department select + loadDepartments())
  [ ] API_BASE port updated in both .cshtml files
  [ ] Multiple startup projects configured
  [ ] Tested: Add student → dept shows in list ✅
  [ ] Tested: Edit student → dept pre-selected ✅
  [ ] Tested: Filter by department works ✅
  [ ] Tested: Delete dept with students → shows error ✅

═══════════════════════════════════════════════════════════════════════
  Remember: The pattern is always the same!
  Parent model → Child model with FK → DbContext with both DbSets
  → Include() in queries → departmentId in JS → nested JSON in response
═══════════════════════════════════════════════════════════════════════