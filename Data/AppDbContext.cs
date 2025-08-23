using Microsoft.EntityFrameworkCore;
using BigFiveAssessmentApi.Models;

namespace BigFiveAssessmentApi.Data
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Submission> Submissions => Set<Submission>();
    }
}
