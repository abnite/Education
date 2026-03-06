namespace EduKidsGhana.Domain.Constants;

public static class DomainConstants
{
    public static class Roles
    {
        public const string Admin = "Admin";
        public const string Parent = "Parent";
        public const string Learner = "Learner";
        public const string ContentManager = "ContentManager";
    }

    public static class Mastery
    {
        public const double EmergingThreshold = 30.0;
        public const double DevelopingThreshold = 55.0;
        public const double ProficientThreshold = 75.0;
        public const double MasteredThreshold = 90.0;
        public const int MinAttemptsForMastery = 3;
    }

    public static class Gamification
    {
        public const int PointsPerLesson = 20;
        public const int PointsPerQuizPass = 30;
        public const int PointsPerPerfectQuiz = 50;
        public const int StreakBonusPoints = 10;
        public const int DailyGoalPoints = 25;
    }

    public static class Quiz
    {
        public const int DefaultQuestionCount = 5;
        public const int DefaultTimeLimitSeconds = 300;
        public const int DefaultPassMarkPercent = 70;
        public const int MaxHintsPerQuestion = 2;
    }
}
