using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Data.SQLite;
using StudySync.Models;
using System.Threading.Tasks;

namespace StudySync.Services
{
    public class DatabaseService
    {
        private readonly string _dbPath;
        private readonly string _connectionString;

        public DatabaseService()
        {
            try
            {
                // Create database in AppData folder
                string appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "StudySync");

                if (!Directory.Exists(appDataPath))
                    Directory.CreateDirectory(appDataPath);

                _dbPath = Path.Combine(appDataPath, "studysync.db");
                _connectionString = $"Data Source={_dbPath};Version=3;";

                System.Diagnostics.Debug.WriteLine($"DatabaseService initialized with path: {_dbPath}");

                InitializeDatabase();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR initializing DatabaseService: {ex.Message}");
                throw new InvalidOperationException($"Failed to initialize DatabaseService: {ex.Message}", ex);
            }
        }

        private void InitializeDatabase()
        {
            bool newDatabase = !File.Exists(_dbPath);

            if (newDatabase)
            {
                SQLiteConnection.CreateFile(_dbPath);
            }

            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                if (newDatabase)
                {
                    // Create Tasks table
                    using (var command = new SQLiteCommand(connection))
                    {
                        command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Tasks (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Title TEXT NOT NULL,
                    Description TEXT,
                    DueDate TEXT NOT NULL,
                    IsCompleted INTEGER NOT NULL,
                    Subject TEXT,
                    CompletedDate TEXT,
                    EstimatedTime INTEGER,
                    Priority INTEGER NOT NULL
                )";
                        command.ExecuteNonQuery();
                    }

                    // Create index on Subject column in Tasks table
                    using (var command = new SQLiteCommand(connection))
                    {
                        command.CommandText = "CREATE INDEX idx_tasks_subject ON Tasks(Subject)";
                        command.ExecuteNonQuery();
                    }

                    // Create Subjects table
                    using (var command = new SQLiteCommand(connection))
                    {
                        command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Subjects (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL UNIQUE,
                    Color TEXT NOT NULL
                )";
                        command.ExecuteNonQuery();
                    }

                    // Create Statistics table
                    using (var command = new SQLiteCommand(connection))
                    {
                        command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Statistics (
                    Id INTEGER PRIMARY KEY,
                    CurrentStreak INTEGER NOT NULL,
                    LongestStreak INTEGER NOT NULL,
                    LastCompletionDate TEXT
                )";
                        command.ExecuteNonQuery();
                    }

                    // Initialize statistics
                    using (var command = new SQLiteCommand(connection))
                    {
                        command.CommandText = @"
                INSERT INTO Statistics (Id, CurrentStreak, LongestStreak, LastCompletionDate)
                VALUES (1, 0, 0, NULL)";
                        command.ExecuteNonQuery();
                    }

                    // Add some default subjects
                    using (var command = new SQLiteCommand(connection))
                    {
                        command.CommandText = @"
                INSERT INTO Subjects (Name, Color) VALUES 
                ('Math', '#FF5733'),
                ('Science', '#33FF57'),
                ('English', '#3357FF'),
                ('History', '#FF33A8'),
                ('Programming', '#33FFF6')";
                        command.ExecuteNonQuery();
                    }

                    // Add some default tasks for testing
                    using (var command = new SQLiteCommand(connection))
                    {
                        command.CommandText = @"
                INSERT INTO Tasks 
                (Title, Description, DueDate, IsCompleted, Subject, CompletedDate, EstimatedTime, Priority)
                VALUES 
                ('Complete Math Homework', 'Chapter 5, exercises 1-10', @DueDate1, 0, 'Math', NULL, 60, 2),
                ('Read Science Article', 'Read about quantum computing', @DueDate2, 1, 'Science', @CompletedDate, 30, 1),
                ('Write Essay', 'Topic: Climate Change Impact', @DueDate3, 0, 'English', NULL, 120, 3)";

                        command.Parameters.AddWithValue("@DueDate1", DateTime.Now.AddDays(1).ToString("yyyy-MM-dd HH:mm:ss"));
                        command.Parameters.AddWithValue("@DueDate2", DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss"));
                        command.Parameters.AddWithValue("@DueDate3", DateTime.Now.AddDays(3).ToString("yyyy-MM-dd HH:mm:ss"));
                        command.Parameters.AddWithValue("@CompletedDate", DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss"));

                        command.ExecuteNonQuery();
                    }
                }
            }
        }


        #region Task Operations

        public List<Models.Task> GetAllTasks()
        {
            System.Diagnostics.Debug.WriteLine("Entering GetAllTasks");

            var tasks = new List<Models.Task>();

            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                using (var command = new SQLiteCommand("SELECT * FROM Tasks", connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tasks.Add(new Models.Task
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Title = reader["Title"].ToString(),
                                Description = reader["Description"].ToString(),
                                DueDate = DateTime.Parse(reader["DueDate"].ToString()),
                                IsCompleted = Convert.ToBoolean(Convert.ToInt32(reader["IsCompleted"])),
                                Subject = reader["Subject"].ToString(),
                                CompletedDate = reader["CompletedDate"] != DBNull.Value
                                    ? DateTime.Parse(reader["CompletedDate"].ToString())
                                    : DateTime.MinValue,
                                EstimatedTime = TimeSpan.FromMinutes(Convert.ToInt32(reader["EstimatedTime"])),
                                TaskPriority = (Priority)Convert.ToInt32(reader["Priority"])
                            });
                        }
                    }
                }
            }

            System.Diagnostics.Debug.WriteLine("Exiting GetAllTasks");
            return tasks;
        }

        public Models.Task GetTaskById(int id)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                using (var command = new SQLiteCommand("SELECT * FROM Tasks WHERE Id = @Id", connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Models.Task
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Title = reader["Title"].ToString(),
                                Description = reader["Description"].ToString(),
                                DueDate = DateTime.Parse(reader["DueDate"].ToString()),
                                IsCompleted = Convert.ToBoolean(Convert.ToInt32(reader["IsCompleted"])),
                                Subject = reader["Subject"].ToString(),
                                CompletedDate = reader["CompletedDate"] != DBNull.Value
                                    ? DateTime.Parse(reader["CompletedDate"].ToString())
                                    : DateTime.MinValue,
                                EstimatedTime = TimeSpan.FromMinutes(Convert.ToInt32(reader["EstimatedTime"])),
                                TaskPriority = (Priority)Convert.ToInt32(reader["Priority"])
                            };
                        }
                    }
                }
            }

            return null;
        }

        public int AddTask(Models.Task task)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                using (var command = new SQLiteCommand(connection))
                {
                    command.CommandText = @"
                        INSERT INTO Tasks 
                        (Title, Description, DueDate, IsCompleted, Subject, CompletedDate, EstimatedTime, Priority)
                        VALUES 
                        (@Title, @Description, @DueDate, @IsCompleted, @Subject, @CompletedDate, @EstimatedTime, @Priority);
                        SELECT last_insert_rowid();";

                    command.Parameters.AddWithValue("@Title", task.Title);
                    command.Parameters.AddWithValue("@Description", task.Description ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@DueDate", task.DueDate.ToString("yyyy-MM-dd HH:mm:ss"));
                    command.Parameters.AddWithValue("@IsCompleted", task.IsCompleted ? 1 : 0);
                    command.Parameters.AddWithValue("@Subject", task.Subject ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@CompletedDate", task.CompletedDate != DateTime.MinValue
                        ? task.CompletedDate.ToString("yyyy-MM-dd HH:mm:ss")
                        : (object)DBNull.Value);
                    command.Parameters.AddWithValue("@EstimatedTime", (int)task.EstimatedTime.TotalMinutes);
                    command.Parameters.AddWithValue("@Priority", (int)task.TaskPriority);

                    var result = command.ExecuteScalar();
                    task.Id = Convert.ToInt32(result);

                    if (task.IsCompleted)
                        UpdateStreak(task.CompletedDate);

                    return task.Id;
                }
            }
        }

        public void UpdateTask(Models.Task task)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                // Get the previous state of the task to check if completion status changed
                bool wasCompletedBefore = false;
                using (var checkCommand = new SQLiteCommand("SELECT IsCompleted FROM Tasks WHERE Id = @Id", connection))
                {
                    checkCommand.Parameters.AddWithValue("@Id", task.Id);
                    var result = checkCommand.ExecuteScalar();
                    if (result != null)
                        wasCompletedBefore = Convert.ToBoolean(Convert.ToInt32(result));
                }

                using (var command = new SQLiteCommand(connection))
                {
                    command.CommandText = @"
                        UPDATE Tasks 
                        SET Title = @Title, 
                            Description = @Description, 
                            DueDate = @DueDate, 
                            IsCompleted = @IsCompleted, 
                            Subject = @Subject, 
                            CompletedDate = @CompletedDate,
                            EstimatedTime = @EstimatedTime,
                            Priority = @Priority
                        WHERE Id = @Id";

                    command.Parameters.AddWithValue("@Id", task.Id);
                    command.Parameters.AddWithValue("@Title", task.Title);
                    command.Parameters.AddWithValue("@Description", task.Description ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@DueDate", task.DueDate.ToString("yyyy-MM-dd HH:mm:ss"));
                    command.Parameters.AddWithValue("@IsCompleted", task.IsCompleted ? 1 : 0);
                    command.Parameters.AddWithValue("@Subject", task.Subject ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@CompletedDate", task.CompletedDate != DateTime.MinValue
                        ? task.CompletedDate.ToString("yyyy-MM-dd HH:mm:ss")
                        : (object)DBNull.Value);
                    command.Parameters.AddWithValue("@EstimatedTime", (int)task.EstimatedTime.TotalMinutes);
                    command.Parameters.AddWithValue("@Priority", (int)task.TaskPriority);

                    command.ExecuteNonQuery();

                    // If task was just completed, update streak
                    if (!wasCompletedBefore && task.IsCompleted)
                        UpdateStreak(task.CompletedDate);
                }
            }
        }

        public void DeleteTask(int taskId)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                using (var command = new SQLiteCommand("DELETE FROM Tasks WHERE Id = @Id", connection))
                {
                    command.Parameters.AddWithValue("@Id", taskId);
                    command.ExecuteNonQuery();
                }
            }
        }

        #endregion

        #region Subject Operations

        public List<Subject> GetAllSubjects()
        {
            var subjects = new List<Subject>();

            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                using (var command = new SQLiteCommand(@"
                    SELECT s.Id, s.Name, s.Color, 
                           COUNT(t.Id) AS TaskCount, 
                           SUM(CASE WHEN t.IsCompleted = 1 THEN 1 ELSE 0 END) AS CompletedCount
                    FROM Subjects s
                    LEFT JOIN Tasks t ON s.Name = t.Subject
                    GROUP BY s.Id, s.Name, s.Color", connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            subjects.Add(new Subject
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Name = reader["Name"].ToString(),
                                Color = reader["Color"].ToString(),
                                TaskCount = Convert.ToInt32(reader["TaskCount"]),
                                CompletedCount = reader["CompletedCount"] != DBNull.Value
                                    ? Convert.ToInt32(reader["CompletedCount"])
                                    : 0
                            });
                        }
                    }
                }
            }

            return subjects;
        }

        public Subject GetSubjectByName(string name)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                using (var command = new SQLiteCommand(@"
                    SELECT s.Id, s.Name, s.Color, 
                           COUNT(t.Id) AS TaskCount, 
                           SUM(CASE WHEN t.IsCompleted = 1 THEN 1 ELSE 0 END) AS CompletedCount
                    FROM Subjects s
                    LEFT JOIN Tasks t ON s.Name = t.Subject
                    WHERE s.Name = @Name
                    GROUP BY s.Id, s.Name, s.Color", connection))
                {
                    command.Parameters.AddWithValue("@Name", name);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Subject
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Name = reader["Name"].ToString(),
                                Color = reader["Color"].ToString(),
                                TaskCount = Convert.ToInt32(reader["TaskCount"]),
                                CompletedCount = reader["CompletedCount"] != DBNull.Value
                                    ? Convert.ToInt32(reader["CompletedCount"])
                                    : 0
                            };
                        }
                    }
                }
            }

            return null;
        }

        public int AddSubject(Subject subject)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                using (var command = new SQLiteCommand(connection))
                {
                    command.CommandText = @"
                        INSERT INTO Subjects (Name, Color)
                        VALUES (@Name, @Color);
                        SELECT last_insert_rowid();";

                    command.Parameters.AddWithValue("@Name", subject.Name);
                    command.Parameters.AddWithValue("@Color", subject.Color);

                    var result = command.ExecuteScalar();
                    subject.Id = Convert.ToInt32(result);
                    return subject.Id;
                }
            }
        }

        public void UpdateSubject(Subject subject)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                // Get previous name to update tasks
                string oldName = "";
                using (var nameCommand = new SQLiteCommand("SELECT Name FROM Subjects WHERE Id = @Id", connection))
                {
                    nameCommand.Parameters.AddWithValue("@Id", subject.Id);
                    var result = nameCommand.ExecuteScalar();
                    if (result != null)
                        oldName = result.ToString();
                }

                using (var command = new SQLiteCommand(connection))
                {
                    command.CommandText = @"
                        UPDATE Subjects 
                        SET Name = @Name, Color = @Color
                        WHERE Id = @Id";

                    command.Parameters.AddWithValue("@Id", subject.Id);
                    command.Parameters.AddWithValue("@Name", subject.Name);
                    command.Parameters.AddWithValue("@Color", subject.Color);

                    command.ExecuteNonQuery();

                    // Update all tasks with this subject
                    if (!string.IsNullOrEmpty(oldName) && oldName != subject.Name)
                    {
                        using (var taskCommand = new SQLiteCommand(connection))
                        {
                            taskCommand.CommandText = @"
                                UPDATE Tasks
                                SET Subject = @NewName
                                WHERE Subject = @OldName";

                            taskCommand.Parameters.AddWithValue("@OldName", oldName);
                            taskCommand.Parameters.AddWithValue("@NewName", subject.Name);
                            taskCommand.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

        public void DeleteSubject(int subjectId)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                // Get subject name
                string subjectName = "";
                using (var nameCommand = new SQLiteCommand("SELECT Name FROM Subjects WHERE Id = @Id", connection))
                {
                    nameCommand.Parameters.AddWithValue("@Id", subjectId);
                    var result = nameCommand.ExecuteScalar();
                    if (result != null)
                        subjectName = result.ToString();
                }

                // Delete subject
                using (var command = new SQLiteCommand("DELETE FROM Subjects WHERE Id = @Id", connection))
                {
                    command.Parameters.AddWithValue("@Id", subjectId);
                    command.ExecuteNonQuery();
                }

                // Clear subject from tasks
                if (!string.IsNullOrEmpty(subjectName))
                {
                    using (var taskCommand = new SQLiteCommand(connection))
                    {
                        taskCommand.CommandText = @"
                            UPDATE Tasks
                            SET Subject = NULL
                            WHERE Subject = @SubjectName";

                        taskCommand.Parameters.AddWithValue("@SubjectName", subjectName);
                        taskCommand.ExecuteNonQuery();
                    }
                }
            }
        }

        #endregion

        #region Statistics Operations

        public Statistics GetStatistics()
        {
            var statistics = new Statistics();

            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                // Get basic stats
                using (var command = new SQLiteCommand(@"
                    SELECT CurrentStreak, LongestStreak
                    FROM Statistics
                    WHERE Id = 1", connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            statistics.CurrentStreak = Convert.ToInt32(reader["CurrentStreak"]);
                            statistics.LongestStreak = Convert.ToInt32(reader["LongestStreak"]);
                        }
                    }
                }

                // Get task counts
                using (var command = new SQLiteCommand(@"
                    SELECT COUNT(*) AS TotalTasks,
                           SUM(CASE WHEN IsCompleted = 1 THEN 1 ELSE 0 END) AS CompletedTasks
                    FROM Tasks", connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            statistics.TotalTasks = Convert.ToInt32(reader["TotalTasks"]);
                            statistics.CompletedTasks = reader["CompletedTasks"] != DBNull.Value
                                ? Convert.ToInt32(reader["CompletedTasks"])
                                : 0;
                        }
                    }
                }

                // Get subject completions
                using (var command = new SQLiteCommand(@"
                    SELECT Subject, COUNT(*) AS CompletedCount
                    FROM Tasks
                    WHERE IsCompleted = 1 AND Subject IS NOT NULL
                    GROUP BY Subject", connection))
                {
                    var subjectCompletions = new Dictionary<string, int>();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string subject = reader["Subject"].ToString();
                            int count = Convert.ToInt32(reader["CompletedCount"]);
                            subjectCompletions[subject] = count;
                        }
                    }

                    statistics.SubjectCompletions = subjectCompletions;
                }

                // Get completions by date
                using (var command = new SQLiteCommand(@"
                    SELECT date(CompletedDate) AS CompletionDate, COUNT(*) AS CompletionCount
                    FROM Tasks
                    WHERE IsCompleted = 1 AND CompletedDate IS NOT NULL
                    GROUP BY date(CompletedDate)
                    ORDER BY CompletionDate", connection))
                {
                    var completionsByDate = new Dictionary<DateTime, int>();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DateTime date = DateTime.Parse(reader["CompletionDate"].ToString());
                            int count = Convert.ToInt32(reader["CompletionCount"]);
                            completionsByDate[date] = count;
                        }
                    }

                    statistics.CompletionsByDate = completionsByDate;
                }
            }

            return statistics;
        }

        private void UpdateStreak(DateTime completionDate)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                DateTime? lastCompletionDate = null;
                int currentStreak = 0;
                int longestStreak = 0;

                // Get current streak info
                using (var command = new SQLiteCommand(@"
                    SELECT CurrentStreak, LongestStreak, LastCompletionDate
                    FROM Statistics
                    WHERE Id = 1", connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            currentStreak = Convert.ToInt32(reader["CurrentStreak"]);
                            longestStreak = Convert.ToInt32(reader["LongestStreak"]);

                            if (reader["LastCompletionDate"] != DBNull.Value)
                                lastCompletionDate = DateTime.Parse(reader["LastCompletionDate"].ToString());
                        }
                    }
                }

                // Calculate new streak
                DateTime today = DateTime.Today;
                DateTime completionDay = completionDate.Date;

                if (lastCompletionDate == null)
                {
                    // First completion ever
                    currentStreak = 1;
                }
                else if (completionDay == today && lastCompletionDate == today)
                {
                    // Already completed something today
                    // Streak doesn't change
                }
                else if (completionDay == today && lastCompletionDate == today.AddDays(-1))
                {
                    // Completed yesterday and today - continue streak
                    currentStreak++;
                }
                else if (completionDay == today)
                {
                    // Completed today but missed some days - restart streak
                    currentStreak = 1;
                }
                else
                {
                    // Completion is in the past or future - don't change streak
                    return;
                }

                // Update longest streak if needed
                if (currentStreak > longestStreak)
                    longestStreak = currentStreak;

                // Save updated stats
                using (var command = new SQLiteCommand(@"
                    UPDATE Statistics
                    SET CurrentStreak = @CurrentStreak,
                        LongestStreak = @LongestStreak,
                        LastCompletionDate = @LastCompletionDate
                    WHERE Id = 1", connection))
                {
                    command.Parameters.AddWithValue("@CurrentStreak", currentStreak);
                    command.Parameters.AddWithValue("@LongestStreak", longestStreak);
                    command.Parameters.AddWithValue("@LastCompletionDate", today.ToString("yyyy-MM-dd"));

                    command.ExecuteNonQuery();
                }
            }
        }

        #endregion
    }
}