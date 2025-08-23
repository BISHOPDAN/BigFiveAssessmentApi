using BigFiveAssessmentApi.Dtos;

namespace BigFiveAssessmentApi.IRepository
{
    public interface IScoringRepository
    {
        List<TraitScoreDto> Score(List<int> responses);
    }
}
