using ArgosHound.Api.Models;

namespace ArgosHound.Api.Data;

public static class DemoData
{
    public static readonly Guid BuilderId =
        Guid.Parse("8f1f9f34-5a46-4f7e-a782-d59b0f473c19");

    public static BuilderProfile Builder { get; } = new()
    {
        Id = BuilderId,
        Name = "John Doe",
        CurrentSkills =
        [
            "C#",
            "Python",
            "TypeScript",
            "React",
            "SQL",
        ],
        LearningGoals =
        [
            "Build production-quality AI applications",
            "Gain hands-on experience with LLM agents",
            "Learn retrieval and evaluation workflows",
            "Validate useful products with real communities",
        ],
        Interests =
        [
            "Developer tools",
            "Education",
            "Digital wellbeing",
            "Local community technology",
        ],
        PreferredOpportunityTypes =
        [
            "Learning project",
            "Community service",
            "Product exploration",
            "Open-source contribution",
        ],
        Location = "Chicago, Illinois",
        EffortPreferences =
        [
            "Part-time",
            "Prototype in one to four weeks",
            "Hands-on collaboration with real users",
        ],
    };

    public static IReadOnlyList<Product> Products { get; } =
    [
        new()
        {
            Id = Guid.Parse("25c26703-61f5-4560-83df-a32908739b76"),
            BuilderId = BuilderId,
            Name = "ScrollGuard",
            Description =
                "A browser extension that interrupts habitual scrolling and helps users return to an intended task.",
            Capabilities =
            [
                "Detect prolonged sessions on distracting websites",
                "Interrupt infinite-scroll behavior",
                "Prompt users to return to a stated task",
                "Set time limits for selected websites",
                "Track focus-session progress",
            ],
            TargetUsers =
            [
                "College students",
                "Remote workers",
                "People reducing social-media use",
            ],
            ProductUrl = "https://example.com/products/scrollguard",
        },
        new()
        {
            Id = Guid.Parse("cb48fa38-347a-4e00-b3f1-4cfa4e9db734"),
            BuilderId = BuilderId,
            Name = "Briefly",
            Description =
                "An AI reading assistant that turns long articles and videos into concise, actionable summaries.",
            Capabilities =
            [
                "Summarize long-form articles",
                "Summarize video transcripts",
                "Extract key takeaways",
                "Create short reading queues",
            ],
            TargetUsers =
            [
                "College students",
                "Researchers",
                "Busy professionals",
            ],
            ProductUrl = "https://example.com/products/briefly",
        },
        new()
        {
            Id = Guid.Parse("4284331b-6ed3-4e61-8363-4af239a3d5ae"),
            BuilderId = BuilderId,
            Name = "StudySprint",
            Description =
                "A lightweight study planner that creates focused review sessions from a student's available time.",
            Capabilities =
            [
                "Create time-boxed study plans",
                "Schedule focused review sessions",
                "Break assignments into small tasks",
                "Track completed study sessions",
            ],
            TargetUsers =
            [
                "College students",
                "High-school students",
                "Independent learners",
            ],
            ProductUrl = "https://example.com/products/study-sprint",
        },
        new()
        {
            Id = Guid.Parse("a89c6a3e-24ef-4384-b09f-d608914f9439"),
            BuilderId = BuilderId,
            Name = "LocalLoop",
            Description =
                "A simple directory for discovering independent events and activities in a local community.",
            Capabilities =
            [
                "Publish local event listings",
                "Browse events by category",
                "Filter events by date and neighborhood",
                "Link attendees to organizer pages",
            ],
            TargetUsers =
            [
                "Local residents",
                "Community organizers",
                "Independent event hosts",
            ],
            ProductUrl = "https://example.com/products/local-loop",
        },
    ];

    // Benchmark expectations for the seeded student-doomscrolling discussion.
    // Discovery must still derive its result from source evidence and product data.
    public static IReadOnlyList<DemoProductFitExpectation> DoomscrollingProductFits { get; } =
    [
        new(
            Guid.Parse("25c26703-61f5-4560-83df-a32908739b76"),
            DemoFitStrength.Direct,
            "ScrollGuard already interrupts prolonged scrolling and supports site limits."),
        new(
            Guid.Parse("cb48fa38-347a-4e00-b3f1-4cfa4e9db734"),
            DemoFitStrength.Adjacent,
            "Briefly can reduce time spent consuming long content but does not interrupt compulsive scrolling."),
        new(
            Guid.Parse("4284331b-6ed3-4e61-8363-4af239a3d5ae"),
            DemoFitStrength.Adjacent,
            "StudySprint offers a replacement focus workflow but does not control distracting websites."),
        new(
            Guid.Parse("a89c6a3e-24ef-4384-b09f-d608914f9439"),
            DemoFitStrength.Weak,
            "Local event discovery does not address digital distraction or study focus."),
    ];
}
