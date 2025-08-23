using System.Text.Json;

namespace BigFiveAssessmentApi.Models
{
    public class Submission
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string CandidateName { get; set; } = string.Empty;
        public string CandidateEmail { get; set; } = string.Empty;
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        // Persist compactly as JSON for speed in a take-home
        public string ResponsesJson { get; set; } = "[]";     // JSON: int[20]
        public string ScoresJson { get; set; } = "[]";        // JSON: TraitScoreDto[]

        public static string ToJson<T>(T obj) => JsonSerializer.Serialize(obj);
    }
}
