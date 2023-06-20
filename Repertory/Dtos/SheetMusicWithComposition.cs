namespace Repertory.Dtos {
    public class SheetMusicWithComposition {
        public int Id { get; set; }
        public string Composition { get; set; } = null!;
        public string Author { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string Publisher { get; set; } = null!;
        public int? ReleaseYear { get; set; }
        public string? Duration { get; set; } = null!;
    }
}
