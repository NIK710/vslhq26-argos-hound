using ArgosHound.Api.Models;

namespace ArgosHound.Api.Data;

public static class DemoSourceData
{
    public static readonly Guid DoomscrollingDiscussionId =
        Guid.Parse("9055da6a-e588-4815-af48-dee781f164a4");

    public static readonly Guid ChessClubDiscussionId =
        Guid.Parse("56c862fe-a55e-4424-9012-d645f0a25eeb");

    public static readonly Guid NoOpportunityDiscussionId =
        Guid.Parse("8882aa72-ddd7-444b-9caf-659a5c294f64");

    public static IReadOnlyList<SourceDiscussion> Discussions { get; } =
    [
        new()
        {
            Id = DoomscrollingDiscussionId,
            Platform = "Reddit",
            ExternalId = "argos_demo_doomscrolling",
            Community = "r/college",
            Title = "How do you stop doomscrolling when you should be studying?",
            Body =
                "I sit down to study and somehow lose an hour scrolling short videos. App blockers feel too aggressive, but I need something that interrupts the habit.",
            Url =
                "https://www.reddit.com/r/college/comments/argos_demo_doomscrolling/",
            AuthorHandle = "campus_student_17",
            PublishedAt = DateTimeOffset.Parse("2026-07-21T18:30:00Z"),
            RetrievedAt = DateTimeOffset.Parse("2026-07-22T09:00:00Z"),
            Comments =
            [
                new()
                {
                    Id = Guid.Parse("4f3c6c14-1cb6-4672-a209-7fa052b656d8"),
                    DiscussionId = DoomscrollingDiscussionId,
                    ExternalId = "comment_focus_reset",
                    Body =
                        "Same. I do not want to block everything; a reminder after ten minutes would probably be enough to snap me out of it.",
                    Url =
                        "https://www.reddit.com/r/college/comments/argos_demo_doomscrolling/comment_focus_reset/",
                    AuthorHandle = "study_break_loop",
                    PublishedAt = DateTimeOffset.Parse("2026-07-21T18:42:00Z"),
                },
                new()
                {
                    Id = Guid.Parse("c9e85bd9-211d-481c-9421-4acc0d75b291"),
                    DiscussionId = DoomscrollingDiscussionId,
                    ExternalId = "comment_assignment_intent",
                    ParentExternalId = "comment_focus_reset",
                    Body =
                        "It would help if the reminder showed the assignment I meant to work on instead of only telling me to stop.",
                    Url =
                        "https://www.reddit.com/r/college/comments/argos_demo_doomscrolling/comment_assignment_intent/",
                    AuthorHandle = "library_regular",
                    PublishedAt = DateTimeOffset.Parse("2026-07-21T19:03:00Z"),
                },
                new()
                {
                    Id = Guid.Parse("d57e26aa-8d1b-4f83-ae3a-a5c35299aaee"),
                    DiscussionId = DoomscrollingDiscussionId,
                    ExternalId = "comment_summary_need",
                    Body =
                        "I also open videos for one useful explanation and then get pulled into recommendations. A quick summary would keep me from opening the app.",
                    Url =
                        "https://www.reddit.com/r/college/comments/argos_demo_doomscrolling/comment_summary_need/",
                    AuthorHandle = "notes_before_night",
                    PublishedAt = DateTimeOffset.Parse("2026-07-21T20:11:00Z"),
                },
            ],
        },
        new()
        {
            Id = ChessClubDiscussionId,
            Platform = "Reddit",
            ExternalId = "argos_demo_chess_club",
            Community = "r/chicago",
            Title = "Our neighborhood chess club is outgrowing spreadsheets",
            Body =
                "We now have forty regular players and coordinate pairings, attendance, room capacity, and volunteer reminders through several spreadsheets and group chats.",
            Url =
                "https://www.reddit.com/r/chicago/comments/argos_demo_chess_club/",
            AuthorHandle = "northside_chess_volunteer",
            PublishedAt = DateTimeOffset.Parse("2026-07-19T16:00:00Z"),
            RetrievedAt = DateTimeOffset.Parse("2026-07-22T09:05:00Z"),
            Comments =
            [
                new()
                {
                    Id = Guid.Parse("c774f5b2-5e80-43ed-8250-3697fcad5164"),
                    DiscussionId = ChessClubDiscussionId,
                    ExternalId = "comment_pairings",
                    Body =
                        "The biggest issue is making fair weekly pairings when attendance changes at the last minute.",
                    Url =
                        "https://www.reddit.com/r/chicago/comments/argos_demo_chess_club/comment_pairings/",
                    AuthorHandle = "rook_and_roll",
                    PublishedAt = DateTimeOffset.Parse("2026-07-19T16:21:00Z"),
                },
                new()
                {
                    Id = Guid.Parse("596244de-7d27-48dd-a639-f28172301e58"),
                    DiscussionId = ChessClubDiscussionId,
                    ExternalId = "comment_volunteers",
                    Body =
                        "A lightweight check-in page and automatic volunteer reminders would save us hours every month.",
                    Url =
                        "https://www.reddit.com/r/chicago/comments/argos_demo_chess_club/comment_volunteers/",
                    AuthorHandle = "club_clock_keeper",
                    PublishedAt = DateTimeOffset.Parse("2026-07-19T17:05:00Z"),
                },
            ],
        },
        new()
        {
            Id = NoOpportunityDiscussionId,
            Platform = "Reddit",
            ExternalId = "argos_demo_keyboard_photo",
            Community = "r/MechanicalKeyboards",
            Title = "Finished my first hand-wired keyboard",
            Body =
                "Sharing a photo of my completed hand-wired keyboard. I am happy with how the walnut case turned out.",
            Url =
                "https://www.reddit.com/r/MechanicalKeyboards/comments/argos_demo_keyboard_photo/",
            AuthorHandle = "walnut_switches",
            PublishedAt = DateTimeOffset.Parse("2026-07-20T14:10:00Z"),
            RetrievedAt = DateTimeOffset.Parse("2026-07-22T09:10:00Z"),
            Comments =
            [
                new()
                {
                    Id = Guid.Parse("96b67070-b82a-4309-b56f-0382c79b08b1"),
                    DiscussionId = NoOpportunityDiscussionId,
                    ExternalId = "comment_keycaps",
                    Body = "The keycap colors work really well with that case.",
                    Url =
                        "https://www.reddit.com/r/MechanicalKeyboards/comments/argos_demo_keyboard_photo/comment_keycaps/",
                    AuthorHandle = "tactile_fan",
                    PublishedAt = DateTimeOffset.Parse("2026-07-20T14:28:00Z"),
                },
            ],
        },
    ];
}
