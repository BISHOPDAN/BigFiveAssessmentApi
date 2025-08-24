using BigFiveAssessmentApi.Dtos;
using BigFiveAssessmentApi.IRepository;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace BigFiveAssessmentApi.Repository
{
    public class SmtpEmailSender : IEmailSender
    {
        private readonly IConfiguration _cfg;
        public SmtpEmailSender(IConfiguration cfg) => _cfg = cfg;

        public async Task SendCandidateReportAsync(string to, string name, List<TraitScoreDto> scores)
        {
            try
            {
                var msg = BaseMessage(to, $"Your Big Five Report, {name}");
                msg.Body = new TextPart("html") { Text = RenderCandidateHtml(name, scores) };
                await SendAsync(msg);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Failed to send candidate report: {ex.Message}");
            }
        }

        public async Task SendTaReportAsync(string to, string candidateName, string candidateEmail, List<int> responses, List<TraitScoreDto> scores)
        {
            try
            {
                var msg = BaseMessage(to, $"[TA] Full Big Five Breakdown - {candidateName}");
                msg.Body = new TextPart("html") { Text = RenderTaHtml(candidateName, candidateEmail, responses, scores) };
                await SendAsync(msg);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Failed to send TA report: {ex.Message}");
            }
        }

        private MimeMessage BaseMessage(string to, string subject)
        {
            var from = _cfg["Email:From"]!;
            var m = new MimeMessage();
            m.From.Add(MailboxAddress.Parse(from));
            m.To.Add(MailboxAddress.Parse(to));
            m.Subject = subject;
            return m;
        }

        private async Task SendAsync(MimeMessage msg)
        {
            try
            {
                var host = _cfg["Email:SmtpHost"];
                var port = int.Parse(_cfg["Email:SmtpPort"]!);

                using var client = new SmtpClient();
                await client.ConnectAsync(host, port, SecureSocketOptions.None);

                await client.SendAsync(msg);
                await client.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Could not send email: {ex.Message}");
            }
        }

        private static string RenderCandidateHtml(string name, List<TraitScoreDto> scores)
        {
            var rows = string.Join("", scores.Select(s =>
                $"<tr><td>{s.Trait}</td><td>{s.Scaled}</td><td>{s.Level}</td><td>{s.Description}</td></tr>"
            ));
            return $@"
                <h2>Hi {name}, here is your Big Five report</h2>
                <table border='1' cellpadding='6' cellspacing='0'>
                  <tr><th>Trait</th><th>Score (8–40)</th><th>Level</th><th>Interpretation</th></tr>
                  {rows}
                </table>";
        }

        private static string RenderTaHtml(string name, string email, List<int> responses, List<TraitScoreDto> scores)
        {
            var cand = $"<p><b>Candidate:</b> {name} ({email})</p>";
            var resp = $"<p><b>Responses:</b> [{string.Join(", ", responses)}]</p>";
            var rows = string.Join("", scores.Select(s =>
                $"<tr><td>{s.Trait}</td><td>{s.Raw}</td><td>{s.Scaled}</td><td>{s.Level}</td></tr>"
            ));
            var tbl = $@"<table border='1' cellpadding='6' cellspacing='0'>
                            <tr><th>Trait</th><th>Raw (4–20)</th><th>Scaled (8–40)</th><th>Level</th></tr>
                            {rows}
                         </table>";
            return cand + resp + tbl;
        }
    }
}