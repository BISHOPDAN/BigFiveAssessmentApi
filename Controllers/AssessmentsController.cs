using BigFiveAssessmentApi.Data;
using BigFiveAssessmentApi.Dtos;
using Microsoft.EntityFrameworkCore;
using BigFiveAssessmentApi.IRepository;
using BigFiveAssessmentApi.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace BigFiveAssessmentApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AssessmentsController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IScoringRepository _scoring;
        private readonly IEmailSender _email;
        private readonly IConfiguration _cfg;

        public AssessmentsController(AppDbContext db, IScoringRepository scoring, IEmailSender email, IConfiguration cfg)
        {
            _db = db; _scoring = scoring; _email = email; _cfg = cfg;
        }

        // POST api/assessments/submit
        [HttpPost("submit")]
        public async Task<ActionResult<SubmissionResponseDto>> Submit([FromBody] SubmissionRequestDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.CandidateEmail) || dto.Responses?.Count != 20)
                return BadRequest("CandidateEmail is required and exactly 20 responses are needed.");

            var scores = _scoring.Score(dto.Responses);

            var entity = new Submission
            {
                CandidateName = dto.CandidateName,
                CandidateEmail = dto.CandidateEmail,
                ResponsesJson = JsonSerializer.Serialize(dto.Responses),
                ScoresJson = JsonSerializer.Serialize(scores),
            };

            _db.Submissions.Add(entity);
            await _db.SaveChangesAsync();

            // Email candidate + TA (non-blocking best practice in real app; here we await)
            await _email.SendCandidateReportAsync(dto.CandidateEmail, dto.CandidateName, scores);
            await _email.SendTaReportAsync(_cfg["Email:TaEmail"]!, dto.CandidateName, dto.CandidateEmail, dto.Responses, scores);

            return Ok(new SubmissionResponseDto
            {
                SubmissionId = entity.Id,
                CandidateName = entity.CandidateName,
                CandidateEmail = entity.CandidateEmail,
                CreatedAtUtc = entity.CreatedAtUtc,
                Scores = scores
            });
        }

        // GET api/assessments/dummy
        [HttpGet("dummy")]
        public ActionResult<SubmissionRequestDto> Dummy()
        {
            var rnd = new Random();
            var dto = new SubmissionRequestDto
            {
                CandidateName = "Test User",
                CandidateEmail = "test.user@example.com",
                Responses = Enumerable.Range(0, 20).Select(_ => rnd.Next(1, 6)).ToList()
            };
            return Ok(dto);
        }

        // --- Admin endpoints (bonus) ---
        [HttpGet("admin/submissions")]
        public async Task<ActionResult<IEnumerable<SubmissionResponseDto>>> List(
        [FromQuery] string? email,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;

            var q = _db.Submissions.AsNoTracking().AsQueryable();

            if (!string.IsNullOrEmpty(email))
            {
                var lowered = email.ToLower();
                q = q.Where(s => s.CandidateEmail.ToLower().Contains(lowered));
            }

            if (from.HasValue)
            {
                var fromDate = from.Value;
                q = q.Where(s => s.CreatedAtUtc >= fromDate);
            }

            if (to.HasValue)
            {
                var toDate = to.Value;
                q = q.Where(s => s.CreatedAtUtc <= toDate);
            }

            var totalCount = await q.CountAsync();

            // Fetch the data first
            var submissions = await q.OrderByDescending(s => s.CreatedAtUtc)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Perform deserialization in memory
            var result = submissions.Select(s => new SubmissionResponseDto
            {
                SubmissionId = s.Id,
                CandidateName = s.CandidateName,
                CandidateEmail = s.CandidateEmail,
                CreatedAtUtc = s.CreatedAtUtc,
                Scores = JsonSerializer.Deserialize<List<TraitScoreDto>>(s.ScoresJson) ?? new List<TraitScoreDto>()
            }).ToList();

            return Ok(new
            {
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                Data = result
            });
        }

        // GET api/assessments/admin/submissions/export
        [HttpGet("admin/submissions/export")]
        public async Task<IActionResult> ExportCsv()
        {
            var items = await _db.Submissions.AsNoTracking().ToListAsync();
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("Id,CandidateName,CandidateEmail,CreatedAtUtc,ScoresJson,ResponsesJson");
            foreach (var s in items)
                sb.AppendLine($"{s.Id},{Csv(s.CandidateName)},{Csv(s.CandidateEmail)},{s.CreatedAtUtc:o},{Csv(s.ScoresJson)},{Csv(s.ResponsesJson)}");

            return File(System.Text.Encoding.UTF8.GetBytes(sb.ToString()), "text/csv", "submissions.csv");
        }

        private static string Csv(string v) => $"\"{v.Replace("\"", "\"\"")}\"";
    }
}
