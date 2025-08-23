using BigFiveAssessmentApi.Dtos;

namespace BigFiveAssessmentApi.IRepository
{
    public interface IEmailSender
    {
        Task SendCandidateReportAsync(string to, string name, List<TraitScoreDto> scores);
        Task SendTaReportAsync(string to, string candidateName, string candidateEmail, List<int> responses, List<TraitScoreDto> scores);
    }
}
