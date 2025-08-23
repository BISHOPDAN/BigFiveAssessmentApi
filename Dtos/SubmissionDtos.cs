using BigFiveAssessmentApi.Models;

namespace BigFiveAssessmentApi.Dtos
{
    public class SubmissionRequestDto
    {
        public string? CandidateName { get; set; }
        public string? CandidateEmail { get; set; }
        public List<int> Responses { get; set; } = new();
    }

    public class TraitScoreDto
    {
        public Trait Trait { get; set; }
        public int Raw { get; set; }
        public int Scaled { get; set; }
        public string? Level { get; set; }
        public string? Description { get; set; }
    }

    public class SubmissionResponseDto
    {
        public Guid SubmissionId { get; set; }
        public string? CandidateName { get; set; }
        public string? CandidateEmail { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public List<TraitScoreDto> Scores { get; set; } = new();
    }
}
