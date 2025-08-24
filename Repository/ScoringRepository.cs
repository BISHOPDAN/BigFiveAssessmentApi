using BigFiveAssessmentApi.Dtos;
using BigFiveAssessmentApi.IRepository;
using BigFiveAssessmentApi.Models;

namespace BigFiveAssessmentApi.Repository
{
    public class ScoringRepository : IScoringRepository
    {
        private readonly Dictionary<int, (Trait trait, bool reverse)> _map = new()
        {
            {1,(Trait.Extraversion,false)},
            {2,(Trait.Agreeableness,false)},
            {3,(Trait.Conscientiousness,false)},
            {4,(Trait.Neuroticism,false)},
            {5,(Trait.Openness,false)},
            {6,(Trait.Extraversion,true)},
            {7,(Trait.Agreeableness,false)},
            {8,(Trait.Conscientiousness,true)},
            {9,(Trait.Neuroticism,false)},
            {10,(Trait.Openness,false)},
            {11,(Trait.Extraversion,false)},
            {12,(Trait.Agreeableness,false)},
            {13,(Trait.Conscientiousness,false)},
            {14,(Trait.Neuroticism,false)},
            {15,(Trait.Openness,false)},
            {16,(Trait.Extraversion,true)},
            {17,(Trait.Agreeableness,false)},
            {18,(Trait.Conscientiousness,true)},
            {19,(Trait.Neuroticism,true)},
            {20,(Trait.Openness,false)}
        };

        public List<TraitScoreDto> Score(List<int> responses)
        {
            try
            {
                if (responses == null || responses.Count != 20)
                    throw new ApplicationException("Exactly 20 responses are required.");

                var sums = Enum.GetValues<Trait>().ToDictionary(t => t, _ => 0);

                for (int i = 0; i < 20; i++)
                {
                    var raw = responses[i];
                    if (raw < 1 || raw > 5)
                        throw new ApplicationException($"Response {i + 1} must be between 1 and 5.");

                    var (trait, reverse) = _map[i + 1];
                    var v = reverse ? 6 - raw : raw;
                    sums[trait] += v;
                }

                var list = new List<TraitScoreDto>();
                foreach (var (trait, raw) in sums)
                {
                    var scaled = raw * 2;
                    var level = scaled <= 20 ? "Low" : (scaled <= 31 ? "Moderate" : "High");
                    list.Add(new TraitScoreDto
                    {
                        Trait = trait,
                        Raw = raw,
                        Scaled = scaled,
                        Level = level,
                        Description = Describe(trait, level)
                    });
                }
                return list.OrderBy(s => s.Trait.ToString()).ToList();
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Scoring failed: {ex.Message}");
            }
        }

        private static string Describe(Trait t, string level) => (t, level) switch
        {
            (Trait.Openness, "High") => "Curious, imaginative, open to ideas.",
            (Trait.Openness, "Moderate") => "Balanced practicality and curiosity.",
            (Trait.Openness, "Low") => "Prefers routine and familiar approaches.",
            (Trait.Conscientiousness, "High") => "Organized, reliable, goal-oriented.",
            (Trait.Conscientiousness, "Moderate") => "Generally dependable with some flexibility.",
            (Trait.Conscientiousness, "Low") => "More spontaneous; may overlook details.",
            (Trait.Extraversion, "High") => "Energized by people; outgoing and active.",
            (Trait.Extraversion, "Moderate") => "Comfortable in groups and alone.",
            (Trait.Extraversion, "Low") => "Reserved; prefers smaller groups or solo work.",
            (Trait.Agreeableness, "High") => "Empathetic, cooperative, supportive.",
            (Trait.Agreeableness, "Moderate") => "Balances candor with empathy.",
            (Trait.Agreeableness, "Low") => "Direct, competitive; may seem blunt.",
            (Trait.Neuroticism, "High") => "Sensitive to stress; benefits from structure.",
            (Trait.Neuroticism, "Moderate") => "Typical emotional reactivity.",
            (Trait.Neuroticism, "Low") => "Calm and resilient under pressure.",
            _ => ""
        };
    }
}