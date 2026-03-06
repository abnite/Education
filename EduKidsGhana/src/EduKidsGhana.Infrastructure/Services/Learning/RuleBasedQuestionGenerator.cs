using EduKidsGhana.Application.DTOs.Quiz;
using EduKidsGhana.Application.Interfaces;
using EduKidsGhana.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace EduKidsGhana.Infrastructure.Services.Learning;

/// <summary>
/// Deterministic rule-based question generator. Works without AI.
/// Generates contextually relevant questions for Ghanaian children.
/// </summary>
public class RuleBasedQuestionGenerator : IRuleBasedQuestionGenerator
{
    private readonly ILogger<RuleBasedQuestionGenerator> _logger;

    public RuleBasedQuestionGenerator(ILogger<RuleBasedQuestionGenerator> logger)
    {
        _logger = logger;
    }

    public Task<List<QuizQuestionDto>> GenerateMathsQuestionsAsync(int gradeLevel, DifficultyLevel difficulty, int count, CancellationToken ct = default)
    {
        var questions = new List<QuizQuestionDto>();
        var rng = new Random();

        for (int i = 0; i < count; i++)
        {
            QuizQuestionDto q = gradeLevel switch
            {
                1 or 2 => GenerateCountingQuestion(rng, gradeLevel),
                3 or 4 => GenerateArithmeticQuestion(rng, gradeLevel, difficulty),
                _ => GenerateAdvancedMathsQuestion(rng, gradeLevel, difficulty)
            };
            q.SortOrder = i;
            questions.Add(q);
        }
        return Task.FromResult(questions);
    }

    private QuizQuestionDto GenerateCountingQuestion(Random rng, int grade)
    {
        string[] fruits = { "mangoes", "oranges", "bananas", "pineapples", "papayas" };
        string fruit = fruits[rng.Next(fruits.Length)];
        int a = rng.Next(1, grade == 1 ? 10 : 20);
        int b = rng.Next(1, grade == 1 ? 10 : 20);
        int answer = a + b;

        return BuildMCQ(
            $"Kofi has {a} {fruit} and Ama gives him {b} more. How many {fruit} does Kofi have now?",
            answer.ToString(),
            new[] { (a + b).ToString(), (a + b + 1).ToString(), (a + b - 1).ToString(), (a + b + 2).ToString() },
            $"Add {a} + {b} = {answer}",
            $"Think: start with {a} and count {b} more.");
    }

    private QuizQuestionDto GenerateArithmeticQuestion(Random rng, int grade, DifficultyLevel difficulty)
    {
        int maxNum = difficulty == DifficultyLevel.Beginner ? 12 : difficulty == DifficultyLevel.Elementary ? 20 : 50;
        bool isMultiply = grade >= 3 && rng.Next(2) == 0;

        if (isMultiply)
        {
            int a = rng.Next(2, 10), b = rng.Next(2, 10);
            int answer = a * b;
            return BuildMCQ(
                $"What is {a} × {b}?",
                answer.ToString(),
                new[] { answer.ToString(), (answer + a).ToString(), (answer - b).ToString(), (answer + 1).ToString() },
                $"{a} × {b} = {answer}",
                $"Count groups of {a}, {b} times.");
        }
        else
        {
            int a = rng.Next(10, maxNum), b = rng.Next(1, a);
            bool isAdd = rng.Next(2) == 0;
            int answer = isAdd ? a + b : a - b;
            return BuildMCQ(
                isAdd ? $"What is {a} + {b}?" : $"What is {a} - {b}?",
                answer.ToString(),
                new[] { answer.ToString(), (answer + 1).ToString(), (answer - 1).ToString(), (answer + 2).ToString() },
                isAdd ? $"{a} + {b} = {answer}" : $"{a} - {b} = {answer}",
                isAdd ? $"Add the numbers together." : $"Take away {b} from {a}.");
        }
    }

    private QuizQuestionDto GenerateAdvancedMathsQuestion(Random rng, int grade, DifficultyLevel difficulty)
    {
        // Money problems using Ghana Cedis
        int price1 = rng.Next(5, 50) * 5; // multiples of 5
        int price2 = rng.Next(1, 10) * 5;
        int paid = price1 + price2 + rng.Next(1, 5) * 10;
        int change = paid - price1;

        return BuildMCQ(
            $"Abena buys a book for GH₵{price1} and pays with GH₵{paid}. How much change does she receive?",
            $"GH₵{change}",
            new[] { $"GH₵{change}", $"GH₵{change + 5}", $"GH₵{change - 5}", $"GH₵{change + 10}" },
            $"Change = GH₵{paid} - GH₵{price1} = GH₵{change}",
            $"Subtract the price from the amount paid.");
    }

    public Task<List<QuizQuestionDto>> GenerateEnglishQuestionsAsync(int gradeLevel, DifficultyLevel difficulty, int count, CancellationToken ct = default)
    {
        var questions = new List<QuizQuestionDto>();
        var rng = new Random();
        var banks = GetEnglishQuestionBank(gradeLevel);

        for (int i = 0; i < Math.Min(count, banks.Count); i++)
        {
            var q = banks[rng.Next(banks.Count)];
            q.SortOrder = i;
            questions.Add(q);
        }

        while (questions.Count < count)
            questions.Add(GetDefaultEnglishQuestion(gradeLevel, questions.Count));

        return Task.FromResult(questions);
    }

    private List<QuizQuestionDto> GetEnglishQuestionBank(int grade)
    {
        if (grade <= 2)
        {
            return new List<QuizQuestionDto>
            {
                BuildMCQ("Which letter comes after 'D' in the alphabet?", "E", new[] { "E", "F", "C", "G" }, "The alphabet order is A B C D E F...", "Say the alphabet out loud."),
                BuildMCQ("Which word rhymes with 'cat'?", "hat", new[] { "hat", "dog", "big", "run" }, "'cat' and 'hat' both end in '-at'", "Listen for the ending sound."),
                BuildMCQ("Choose the correct spelling:", "mango", new[] { "mango", "mnago", "mangoe", "manggo" }, "Mango is a fruit grown in Ghana.", "Sound it out: man-go"),
                BuildTrueFalse("A 'noun' is the name of a person, place or thing.", true, "Nouns name people, places, animals and things."),
                BuildMCQ("Complete: 'The dog ___ in the garden.'", "runs", new[] { "runs", "running", "run", "ran" }, "We use 'runs' with singular subjects in present tense.", "The dog does the action now.")
            };
        }
        return new List<QuizQuestionDto>
        {
            BuildMCQ("What is the plural of 'child'?", "children", new[] { "children", "childs", "childes", "childrens" }, "'Child' has an irregular plural: children.", "Some plurals don't just add -s."),
            BuildMCQ("Choose the correct sentence:", "She doesn't like oranges.", new[] { "She doesn't like oranges.", "She don't like oranges.", "She not like oranges.", "She isn't like oranges." }, "We use 'doesn't' with he/she/it.", "Think about subject-verb agreement."),
            BuildTrueFalse("An adjective describes a noun.", true, "Adjectives tell us more about nouns, e.g. 'red ball'."),
            BuildMCQ("Which word is an antonym of 'hot'?", "cold", new[] { "cold", "warm", "heat", "fire" }, "An antonym is a word with the opposite meaning.", "Think of opposites.")
        };
    }

    private static QuizQuestionDto GetDefaultEnglishQuestion(int grade, int index)
    {
        return BuildMCQ(
            $"Which of these is a vowel?",
            "A",
            new[] { "A", "B", "C", "D" },
            "The vowels are A, E, I, O, U.",
            "Remember: A, E, I, O, U are vowels.");
    }

    public Task<List<QuizQuestionDto>> GenerateScienceQuestionsAsync(Guid topicId, int gradeLevel, DifficultyLevel difficulty, int count, CancellationToken ct = default)
    {
        var questions = new List<QuizQuestionDto>
        {
            BuildMCQ("Which of these is a living thing?", "A mango tree", new[] { "A mango tree", "A rock", "Water", "Sand" }, "Living things grow, breathe and reproduce.", "Can it grow and breathe?"),
            BuildTrueFalse("Plants need sunlight to make food.", true, "Plants use sunlight in a process called photosynthesis."),
            BuildMCQ("What do plants use to make their food?", "Sunlight, water and air", new[] { "Sunlight, water and air", "Soil and rain only", "Only water", "Sunlight and soil" }, "Plants use sunlight, water and carbon dioxide.", "Think of what a plant needs to grow."),
            BuildMCQ("Which body part pumps blood?", "Heart", new[] { "Heart", "Lungs", "Brain", "Liver" }, "The heart pumps blood around the body.", "It beats in your chest."),
            BuildTrueFalse("Fish breathe using gills.", true, "Fish use gills to get oxygen from water."),
            BuildMCQ("What is the source of energy for the Earth?", "The Sun", new[] { "The Sun", "The Moon", "Wind", "Rain" }, "The Sun provides light and heat energy to Earth.", "Think about what makes plants grow.")
        };

        return Task.FromResult(questions.Take(count).Select((q, i) => { q.SortOrder = i; return q; }).ToList());
    }

    public Task<List<QuizQuestionDto>> GenerateProgrammingQuestionsAsync(int gradeLevel, DifficultyLevel difficulty, int count, CancellationToken ct = default)
    {
        var questions = new List<QuizQuestionDto>
        {
            BuildMCQ("What is the correct order to make a cup of tea?", "Boil water → Add teabag → Pour water → Add milk",
                new[] { "Boil water → Add teabag → Pour water → Add milk", "Add milk → Boil water → Add teabag → Pour water", "Pour water → Boil water → Add teabag → Add milk", "Add teabag → Add milk → Boil water → Pour water" },
                "In programming, order (sequence) matters. Each step must happen in the right order.", "Think about what must happen first."),
            BuildTrueFalse("In programming, a 'loop' repeats instructions.", true, "Loops let us repeat actions without writing the same code again."),
            BuildMCQ("A computer program is a set of:", "Instructions for the computer", new[] { "Instructions for the computer", "Pictures", "Songs", "Books" }, "Programs are sets of instructions that tell computers what to do.", "What does a computer need to follow?"),
            BuildMCQ("Which is an example of an 'input' to a computer?", "Typing on a keyboard", new[] { "Typing on a keyboard", "Seeing text on screen", "Hearing sound", "Printing a page" }, "Input is information going INTO the computer.", "What sends information TO the computer?"),
            BuildTrueFalse("A bug in a program means there is an error.", true, "Bugs are mistakes or errors in code that cause wrong results.")
        };

        return Task.FromResult(questions.Take(count).Select((q, i) => { q.SortOrder = i; return q; }).ToList());
    }

    public Task<string> GenerateFallbackHintAsync(string questionText, string subjectCode, CancellationToken ct = default)
    {
        var hints = new Dictionary<string, string>
        {
            { "MATHS", "Try breaking the problem into smaller steps. Look for key numbers." },
            { "ENGLISH", "Sound out the word or think about the meaning of each part." },
            { "SCIENCE", "Think about what you see in nature around you in Ghana." },
            { "PROG", "Remember: computers follow instructions step by step, in order." }
        };

        var hint = hints.GetValueOrDefault(subjectCode.ToUpper(), "Read the question carefully and think about what you have learnt.");
        return Task.FromResult(hint);
    }

    private static QuizQuestionDto BuildMCQ(string question, string correct, string[] options, string explanation, string hint)
    {
        var shuffled = options.OrderBy(_ => Guid.NewGuid()).ToList();
        return new QuizQuestionDto
        {
            Id = Guid.NewGuid(),
            QuestionText = question,
            QuestionType = QuestionType.MultipleChoice,
            Points = 10,
            Options = shuffled.Select((o, idx) => new QuizOptionDto
            {
                Id = Guid.NewGuid(),
                OptionKey = ((char)('A' + idx)).ToString(),
                OptionText = o,
                SortOrder = idx
            }).ToList()
        };
    }

    private static QuizQuestionDto BuildTrueFalse(string question, bool correct, string explanation)
    {
        return new QuizQuestionDto
        {
            Id = Guid.NewGuid(),
            QuestionText = question,
            QuestionType = QuestionType.TrueFalse,
            Points = 5,
            Options = new List<QuizOptionDto>
            {
                new() { Id = Guid.NewGuid(), OptionKey = "A", OptionText = "True", SortOrder = 0 },
                new() { Id = Guid.NewGuid(), OptionKey = "B", OptionText = "False", SortOrder = 1 }
            }
        };
    }
}
