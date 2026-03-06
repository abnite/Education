using EduKidsGhana.Domain.Constants;
using EduKidsGhana.Domain.Entities.Curriculum;
using EduKidsGhana.Domain.Entities.Gamification;
using EduKidsGhana.Domain.Entities.Identity;
using EduKidsGhana.Domain.Entities.Users;
using EduKidsGhana.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EduKidsGhana.Infrastructure.Persistence.Seeders;

public class DatabaseSeeder
{
    private readonly AppDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly ILogger<DatabaseSeeder> _logger;

    public DatabaseSeeder(AppDbContext db, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, ILogger<DatabaseSeeder> logger)
    {
        _db = db;
        _userManager = userManager;
        _roleManager = roleManager;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        await _db.Database.MigrateAsync();
        await SeedRolesAsync();
        await SeedAdminAsync();
        await SeedAvatarsAsync();
        await SeedSubjectsAndCurriculumAsync();
        await SeedAchievementsAsync();
        await SeedDemoPairedAccountsAsync();
        _logger.LogInformation("Database seeding complete.");
    }

    private async Task SeedRolesAsync()
    {
        var roles = new[] { DomainConstants.Roles.Admin, DomainConstants.Roles.Parent, DomainConstants.Roles.Learner, DomainConstants.Roles.ContentManager };
        foreach (var role in roles)
        {
            if (!await _roleManager.RoleExistsAsync(role))
            {
                await _roleManager.CreateAsync(new ApplicationRole { Name = role, Description = $"{role} role" });
                _logger.LogInformation("Created role: {Role}", role);
            }
        }
    }

    private async Task SeedAdminAsync()
    {
        const string adminEmail = "admin@edukidsghana.com";
        if (await _userManager.FindByEmailAsync(adminEmail) != null) return;

        var admin = new ApplicationUser { FirstName = "System", LastName = "Admin", Email = adminEmail, UserName = adminEmail, IsActive = true };
        var result = await _userManager.CreateAsync(admin, "Admin@EduKids2024!");
        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(admin, DomainConstants.Roles.Admin);
            _logger.LogInformation("Admin user seeded: {Email}", adminEmail);
        }
    }

    private async Task SeedAvatarsAsync()
    {
        if (await _db.LearnerAvatars.AnyAsync()) return;
        var avatars = new[]
        {
            new LearnerAvatar { Code = "avatar_1", Name = "Kofi the Explorer", ImageUrl = "/assets/avatars/kofi.svg", Description = "A brave explorer from Accra" },
            new LearnerAvatar { Code = "avatar_2", Name = "Ama the Scientist", ImageUrl = "/assets/avatars/ama.svg", Description = "A curious scientist who loves experiments" },
            new LearnerAvatar { Code = "avatar_3", Name = "Kwame the Coder", ImageUrl = "/assets/avatars/kwame.svg", Description = "A young programmer from Kumasi" },
            new LearnerAvatar { Code = "avatar_4", Name = "Abena the Reader", ImageUrl = "/assets/avatars/abena.svg", Description = "A bookworm who loves stories" },
            new LearnerAvatar { Code = "avatar_5", Name = "Yaw the Champion", ImageUrl = "/assets/avatars/yaw.svg", Description = "A maths champion who loves numbers" },
            new LearnerAvatar { Code = "avatar_6", Name = "Efua the Artist", ImageUrl = "/assets/avatars/efua.svg", Description = "A creative artist from Cape Coast" }
        };
        _db.LearnerAvatars.AddRange(avatars);
        await _db.SaveChangesAsync();
    }

    private async Task SeedSubjectsAndCurriculumAsync()
    {
        if (await _db.Subjects.AnyAsync()) return;

        // Grade Levels
        var grades = Enumerable.Range(1, 6).Select(g => new GradeLevel
        {
            Grade = g, Name = $"Grade {g}", MinAge = g + 5, MaxAge = g + 7,
            Description = $"Primary school Grade {g} in Ghana"
        }).ToList();
        _db.GradeLevels.AddRange(grades);
        await _db.SaveChangesAsync();

        // Subjects
        var subjects = new[]
        {
            new Subject { Name = "Mathematics", Code = "MATHS", SubjectType = SubjectType.Mathematics, ColourHex = "#FF6B35", IconUrl = "/assets/icons/maths.svg", SortOrder = 1, Description = "Numbers, shapes, and problem solving" },
            new Subject { Name = "English", Code = "ENGLISH", SubjectType = SubjectType.English, ColourHex = "#4ECDC4", IconUrl = "/assets/icons/english.svg", SortOrder = 2, Description = "Reading, writing, and language" },
            new Subject { Name = "Science", Code = "SCIENCE", SubjectType = SubjectType.Science, ColourHex = "#45B7D1", IconUrl = "/assets/icons/science.svg", SortOrder = 3, Description = "Explore the natural world" },
            new Subject { Name = "Coding", Code = "PROG", SubjectType = SubjectType.Programming, ColourHex = "#96CEB4", IconUrl = "/assets/icons/coding.svg", SortOrder = 4, Description = "Learn to think like a computer" }
        };
        _db.Subjects.AddRange(subjects);
        await _db.SaveChangesAsync();

        await SeedTopicsAndLessonsAsync(subjects, grades);
    }

    private async Task SeedTopicsAndLessonsAsync(Subject[] subjects, List<GradeLevel> grades)
    {
        var maths = subjects.First(s => s.Code == "MATHS");
        var english = subjects.First(s => s.Code == "ENGLISH");
        var science = subjects.First(s => s.Code == "SCIENCE");
        var coding = subjects.First(s => s.Code == "PROG");

        // GRADE 1 MATHS - Counting
        var g1 = grades.First(g => g.Grade == 1);
        var g2 = grades.First(g => g.Grade == 2);
        var g3 = grades.First(g => g.Grade == 3);
        var g4 = grades.First(g => g.Grade == 4);
        var g5 = grades.First(g => g.Grade == 5);
        var g6 = grades.First(g => g.Grade == 6);

        var topicCounting = new Topic { SubjectId = maths.Id, GradeLevelId = g1.Id, Name = "Counting 1-20", SortOrder = 1 };
        var topicShapes = new Topic { SubjectId = maths.Id, GradeLevelId = g1.Id, Name = "Basic Shapes", SortOrder = 2 };
        var topicAddition = new Topic { SubjectId = maths.Id, GradeLevelId = g2.Id, Name = "Addition and Subtraction", SortOrder = 1 };
        var topicMultiplication = new Topic { SubjectId = maths.Id, GradeLevelId = g4.Id, Name = "Multiplication Tables", SortOrder = 1, DefaultDifficulty = DifficultyLevel.Elementary };
        var topicAlphabet = new Topic { SubjectId = english.Id, GradeLevelId = g1.Id, Name = "Alphabet and Phonics", SortOrder = 1 };
        var topicSentences = new Topic { SubjectId = english.Id, GradeLevelId = g2.Id, Name = "Simple Sentences", SortOrder = 1 };
        var topicComprehension = new Topic { SubjectId = english.Id, GradeLevelId = g5.Id, Name = "Comprehension", SortOrder = 2 };
        var topicLivingThings = new Topic { SubjectId = science.Id, GradeLevelId = g2.Id, Name = "Living and Non-Living Things", SortOrder = 1 };
        var topicSimpleMachines = new Topic { SubjectId = science.Id, GradeLevelId = g6.Id, Name = "Simple Machines", SortOrder = 1, DefaultDifficulty = DifficultyLevel.Intermediate };
        var topicSequencing = new Topic { SubjectId = coding.Id, GradeLevelId = g3.Id, Name = "Sequencing Instructions", SortOrder = 1 };

        _db.Topics.AddRange(topicCounting, topicShapes, topicAddition, topicMultiplication, topicAlphabet, topicSentences, topicComprehension, topicLivingThings, topicSimpleMachines, topicSequencing);
        await _db.SaveChangesAsync();

        // LESSON 1: Grade 1 Maths - Counting Mangoes and Oranges
        var lessonCounting = new Lesson
        {
            TopicId = topicCounting.Id,
            Title = "Counting Mangoes and Oranges",
            Summary = "Learn to count fruits from 1 to 20 using everyday objects found in Ghana.",
            LessonType = LessonType.Core,
            Difficulty = DifficultyLevel.Beginner,
            EstimatedMinutes = 15,
            SortOrder = 1
        };
        _db.Lessons.Add(lessonCounting);
        await _db.SaveChangesAsync();

        _db.LessonObjectives.AddRange(
            new LessonObjective { LessonId = lessonCounting.Id, Description = "Count objects from 1 to 10 correctly", SortOrder = 1 },
            new LessonObjective { LessonId = lessonCounting.Id, Description = "Count objects from 11 to 20 correctly", SortOrder = 2 },
            new LessonObjective { LessonId = lessonCounting.Id, Description = "Match numbers to groups of objects", SortOrder = 3 }
        );
        _db.LessonSections.AddRange(
            new LessonSection { LessonId = lessonCounting.Id, Title = "Let's Count!", Content = "Kofi went to the market with his mother. He saw lots of mangoes! Let us count them together.\n\nPoint to each mango as you count: 1, 2, 3, 4, 5!\n\nNow count the oranges: 1, 2, 3, 4, 5, 6, 7!", ContentHtml = "<p>Kofi went to the market with his mother. He saw lots of <strong>mangoes</strong>!</p><p>Let us count them together.</p>", SortOrder = 1 },
            new LessonSection { LessonId = lessonCounting.Id, Title = "Counting to 20", Content = "Can you count from 1 to 20? Say the numbers out loud:\n1, 2, 3, 4, 5, 6, 7, 8, 9, 10\n11, 12, 13, 14, 15, 16, 17, 18, 19, 20\n\nWell done! You counted to 20!", SortOrder = 2 }
        );
        _db.LessonExamples.AddRange(
            new LessonExample { LessonId = lessonCounting.Id, Title = "Count the Pineapples", Content = "Ama has some pineapples in a basket. Count them: 🍍🍍🍍🍍🍍🍍🍍🍍", Solution = "8", Explanation = "There are 8 pineapples in the basket.", SortOrder = 1 },
            new LessonExample { LessonId = lessonCounting.Id, Title = "Count the Bananas", Content = "Kwame picked bananas from the tree. Count them: 🍌🍌🍌🍌🍌🍌🍌🍌🍌🍌🍌🍌", Solution = "12", Explanation = "There are 12 bananas. Did you count to 12?", SortOrder = 2 }
        );
        _db.LessonAudios.Add(new LessonAudio { LessonId = lessonCounting.Id, Title = "Count With Kofi - Narration", AudioType = AudioType.LessonNarration, FileName = "grade1_maths_counting.mp3" });
        _db.QuizTemplates.Add(new QuizTemplate { LessonId = lessonCounting.Id, Title = "Counting Quiz", QuizType = QuizType.Practice, QuestionCount = 5, TimeLimitSeconds = 180, PassMarkPercent = 60 });

        // LESSON 2: Grade 1 English - Alphabet Sounds
        var lessonAlphabet = new Lesson
        {
            TopicId = topicAlphabet.Id,
            Title = "Alphabet Sounds - A to E",
            Summary = "Learn the sounds of the first five letters of the alphabet using Ghanaian examples.",
            LessonType = LessonType.Core,
            Difficulty = DifficultyLevel.Beginner,
            EstimatedMinutes = 20,
            SortOrder = 1
        };
        _db.Lessons.Add(lessonAlphabet);
        await _db.SaveChangesAsync();

        _db.LessonSections.AddRange(
            new LessonSection { LessonId = lessonAlphabet.Id, Title = "The Letter A", Content = "A is for Ama! A says /a/ like in 'Ama' and 'apple'.\n\nSay it with me: A, A, A!\n\nCan you think of other words that start with A?", SortOrder = 1 },
            new LessonSection { LessonId = lessonAlphabet.Id, Title = "The Letter B", Content = "B is for Banana! B says /b/ like in 'ball' and 'banana'.\n\nGhana has delicious bananas. B, B, Banana!", SortOrder = 2 },
            new LessonSection { LessonId = lessonAlphabet.Id, Title = "The Letter C", Content = "C is for Cocoa! Ghana grows lots of cocoa. C says /k/ like in 'cat' and 'cocoa'.\n\nC, C, Cocoa!", SortOrder = 3 }
        );

        // LESSON 3: Grade 2 Science - Living and Non-Living Things
        var lessonLiving = new Lesson
        {
            TopicId = topicLivingThings.Id,
            Title = "Living and Non-Living Things",
            Summary = "Discover the difference between things that are alive and things that are not alive.",
            LessonType = LessonType.Core,
            Difficulty = DifficultyLevel.Beginner,
            EstimatedMinutes = 20,
            SortOrder = 1
        };
        _db.Lessons.Add(lessonLiving);
        await _db.SaveChangesAsync();

        _db.LessonSections.AddRange(
            new LessonSection { LessonId = lessonLiving.Id, Title = "What is a Living Thing?", Content = "Look around you! Some things are ALIVE and some are NOT ALIVE.\n\nLiving things can:\n• Grow bigger\n• Breathe\n• Eat food\n• Move on their own\n• Have babies\n\nExamples of living things: dogs, cats, trees, fish, Kofi!", SortOrder = 1 },
            new LessonSection { LessonId = lessonLiving.Id, Title = "Non-Living Things", Content = "Non-living things CANNOT grow, breathe or move on their own.\n\nExamples: rocks, water, books, chairs, cups.\n\nA rock by the Volta River is not alive. It cannot grow or breathe!", SortOrder = 2 }
        );
        _db.LessonExamples.AddRange(
            new LessonExample { LessonId = lessonLiving.Id, Title = "Is it living?", Content = "A mango tree in your compound - is it a living thing?", Solution = "Yes! It is a living thing.", Explanation = "A mango tree grows, makes fruit and needs water and sunlight. It is alive!", SortOrder = 1 }
        );

        // LESSON 4: Grade 3 Programming - Ordering Simple Steps
        var lessonSequencing = new Lesson
        {
            TopicId = topicSequencing.Id,
            Title = "Ordering Simple Steps",
            Summary = "Learn that computers follow instructions in a specific order, just like making fufu!",
            LessonType = LessonType.Core,
            Difficulty = DifficultyLevel.Beginner,
            EstimatedMinutes = 25,
            SortOrder = 1
        };
        _db.Lessons.Add(lessonSequencing);
        await _db.SaveChangesAsync();

        _db.LessonSections.AddRange(
            new LessonSection { LessonId = lessonSequencing.Id, Title = "Steps Must Be in Order!", Content = "Imagine you want to make a cup of Milo.\n\n❌ Wrong order: Drink the Milo → Add water → Add Milo powder → Heat water\n✅ Right order: Heat water → Add Milo powder → Add water → Stir → Drink\n\nComputers are the same! They follow steps in the exact order you give them.", SortOrder = 1 },
            new LessonSection { LessonId = lessonSequencing.Id, Title = "What is an Algorithm?", Content = "An ALGORITHM is a list of steps to solve a problem.\n\nAlgorithm for going to school:\n1. Wake up\n2. Brush your teeth\n3. Eat breakfast\n4. Put on your uniform\n5. Walk to school\n\nOrder matters! You cannot eat breakfast before waking up.", SortOrder = 2 }
        );

        // LESSON 5: Grade 4 Maths - Multiplication Tables
        var lessonMultiplication = new Lesson
        {
            TopicId = topicMultiplication.Id,
            Title = "Multiplication Tables - The 2s and 5s",
            Summary = "Master the 2 times table and 5 times table with fun Ghanaian examples.",
            LessonType = LessonType.Core,
            Difficulty = DifficultyLevel.Elementary,
            EstimatedMinutes = 20,
            SortOrder = 1
        };
        _db.Lessons.Add(lessonMultiplication);
        await _db.SaveChangesAsync();

        _db.LessonSections.AddRange(
            new LessonSection { LessonId = lessonMultiplication.Id, Title = "The 2 Times Table", Content = "Kofi sells kele-wele (fried plantain) in pairs. Each bag has 2 pieces.\n\n2 × 1 = 2 | 2 × 2 = 4 | 2 × 3 = 6 | 2 × 4 = 8 | 2 × 5 = 10\n2 × 6 = 12 | 2 × 7 = 14 | 2 × 8 = 16 | 2 × 9 = 18 | 2 × 10 = 20\n\nTip: The 2 times table is always even numbers!", SortOrder = 1 },
            new LessonSection { LessonId = lessonMultiplication.Id, Title = "The 5 Times Table", Content = "A hand has 5 fingers. Count by hands!\n\n5 × 1 = 5 | 5 × 2 = 10 | 5 × 3 = 15 | 5 × 4 = 20 | 5 × 5 = 25\n5 × 6 = 30 | 5 × 7 = 35 | 5 × 8 = 40 | 5 × 9 = 45 | 5 × 10 = 50\n\nTip: The 5 times table always ends in 0 or 5!", SortOrder = 2 }
        );
        _db.QuizTemplates.Add(new QuizTemplate { LessonId = lessonMultiplication.Id, Title = "Multiplication Quiz", QuizType = QuizType.Practice, QuestionCount = 5, PassMarkPercent = 70 });

        await _db.SaveChangesAsync();
        _logger.LogInformation("Curriculum seeded with {Count} sample lessons.", 5);
    }

    private async Task SeedAchievementsAsync()
    {
        if (await _db.Achievements.AnyAsync()) return;
        var achievements = new[]
        {
            new Achievement { Name = "First Quiz!", Description = "Complete your very first quiz.", BadgeCode = "FIRST_QUIZ", Category = BadgeCategory.General, PointsRequired = 0 },
            new Achievement { Name = "Quiz Champion", Description = "Complete 10 quizzes.", BadgeCode = "QUIZ_10", Category = BadgeCategory.General, PointsRequired = 0 },
            new Achievement { Name = "Maths Star", Description = "Master a Maths topic.", BadgeCode = "MATHS_STAR", Category = BadgeCategory.Mathematics, PointsRequired = 0 },
            new Achievement { Name = "Reading Hero", Description = "Complete 3 English lessons.", BadgeCode = "READING_HERO", Category = BadgeCategory.English, PointsRequired = 0 },
            new Achievement { Name = "Science Explorer", Description = "Complete a Science lesson.", BadgeCode = "SCIENCE_EXPLORER", Category = BadgeCategory.Science, PointsRequired = 0 },
            new Achievement { Name = "Coding Beginner", Description = "Complete your first Coding exercise.", BadgeCode = "CODING_BEGINNER", Category = BadgeCategory.Programming, PointsRequired = 0 },
            new Achievement { Name = "Daily Practice Hero", Description = "Study 7 days in a row.", BadgeCode = "STREAK_7", Category = BadgeCategory.Streak, PointsRequired = 0 },
            new Achievement { Name = "30-Day Champion", Description = "Study 30 days in a row!", BadgeCode = "STREAK_30", Category = BadgeCategory.Streak, PointsRequired = 0 },
            new Achievement { Name = "First Master", Description = "Fully master your first topic.", BadgeCode = "MASTERED_1", Category = BadgeCategory.Improvement, PointsRequired = 0 },
            new Achievement { Name = "Knowledge Master", Description = "Master 5 topics.", BadgeCode = "MASTERED_5", Category = BadgeCategory.Improvement, PointsRequired = 0 },
            new Achievement { Name = "Point Collector", Description = "Earn 100 points.", BadgeCode = "POINTS_100", Category = BadgeCategory.General, PointsRequired = 100 },
            new Achievement { Name = "Star Learner", Description = "Earn 500 points.", BadgeCode = "POINTS_500", Category = BadgeCategory.General, PointsRequired = 500 },
            new Achievement { Name = "EduKids Champion", Description = "Earn 1000 points!", BadgeCode = "POINTS_1000", Category = BadgeCategory.General, PointsRequired = 1000 }
        };
        _db.Achievements.AddRange(achievements);
        await _db.SaveChangesAsync();
        _logger.LogInformation("Seeded {Count} achievements.", achievements.Length);
    }

    private async Task SeedDemoPairedAccountsAsync()
    {
        const string parentEmail = "parent@demo.edukidsghana.com";
        if (await _userManager.FindByEmailAsync(parentEmail) != null) return;

        // Demo parent
        var parentUser = new ApplicationUser { FirstName = "Adwoa", LastName = "Mensah", Email = parentEmail, UserName = parentEmail, IsActive = true };
        await _userManager.CreateAsync(parentUser, "Demo@Parent2024!");
        await _userManager.AddToRoleAsync(parentUser, DomainConstants.Roles.Parent);
        var parentProfile = new ParentProfile { UserId = parentUser.Id, PhoneNumber = "+233244000001", Region = "Greater Accra", City = "Accra" };
        _db.ParentProfiles.Add(parentProfile);
        await _db.SaveChangesAsync();

        // Demo learner
        const string learnerEmail = "learner@demo.edukidsghana.com";
        var learnerUser = new ApplicationUser { FirstName = "Kofi", LastName = "Mensah", Email = learnerEmail, UserName = learnerEmail, IsActive = true };
        await _userManager.CreateAsync(learnerUser, "Demo@Learner2024!");
        await _userManager.AddToRoleAsync(learnerUser, DomainConstants.Roles.Learner);

        var learnerProfile = new LearnerProfile { UserId = learnerUser.Id, DisplayName = "Kofi", Age = 9, GradeLevel = 4, AvatarCode = "avatar_1", TotalPoints = 120, CurrentStreak = 3, AudioEnabled = true };
        _db.LearnerProfiles.Add(learnerProfile);
        await _db.SaveChangesAsync();

        _db.LearnerPreferences.Add(new LearnerPreference { LearnerId = learnerProfile.Id, AudioNarrationEnabled = true, DailyGoalMinutes = 30 });
        _db.ParentLearnerLinks.Add(new ParentLearnerLink { ParentProfileId = parentProfile.Id, LearnerProfileId = learnerProfile.Id });
        await _db.SaveChangesAsync();

        _logger.LogInformation("Demo accounts seeded. Parent: {Parent}, Learner: {Learner}", parentEmail, learnerEmail);
    }
}
