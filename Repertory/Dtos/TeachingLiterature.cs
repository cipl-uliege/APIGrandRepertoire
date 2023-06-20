namespace Repertory.Dtos {
    public class TeachingLiterature {
        public int Id { get; set; }
        public string Author { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string Publisher { get; set; } = null!;
        public int? ReleaseYear { get; set; }
        public string? Duration { get; set; } = null!;
    }
}
