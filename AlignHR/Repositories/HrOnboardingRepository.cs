using AlignHR.Data;
using AlignHR.Models;
using Microsoft.EntityFrameworkCore;

namespace AlignHR.Repositories
{
    public class HrOnboardingRepository : IHrOnboardingRepository
    {
        private readonly ApplicationDbContext _context;

        public HrOnboardingRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IQueryable<HrOnboarding> Query()
            => _context.HrOnboardings.AsQueryable();

        public async Task<HrOnboarding?> GetByIdAsync(decimal id)
            => await _context.HrOnboardings.FirstOrDefaultAsync(o => o.Id == id);

        public async Task<HrOnboarding?> GetWithDetailsAsync(decimal id)
            => await _context.HrOnboardings
                .Include(o => o.Candidate)
                .Include(o => o.Offer)
                    .ThenInclude(of => of!.JobApplication)
                        .ThenInclude(a => a!.JobPosting)
                .Include(o => o.Tasks.OrderBy(t => t.Id))
                    .ThenInclude(t => t.TaskTemplate)
                .FirstOrDefaultAsync(o => o.Id == id);

        public async Task<HrOnboarding?> GetByOfferAsync(decimal offerId)
            => await _context.HrOnboardings.FirstOrDefaultAsync(o => o.OfferId == offerId);

        public async Task AddOnboardingAsync(HrOnboarding onboarding)
            => await _context.HrOnboardings.AddAsync(onboarding);

        public void UpdateOnboarding(HrOnboarding onboarding)
            => _context.HrOnboardings.Update(onboarding);

        public async Task<IList<HrOnboardingTaskTemplate>> GetActiveTemplatesAsync()
            => await _context.HrOnboardingTaskTemplates
                .Where(t => t.IsActive)
                .OrderBy(t => t.ResponsibleDepartment)
                .ThenBy(t => t.TaskName)
                .ToListAsync();

        public async Task<HrOnboardingTaskTemplate?> GetTemplateByIdAsync(decimal id)
            => await _context.HrOnboardingTaskTemplates.FindAsync(id);

        public async Task AddTemplateAsync(HrOnboardingTaskTemplate template)
            => await _context.HrOnboardingTaskTemplates.AddAsync(template);

        public void UpdateTemplate(HrOnboardingTaskTemplate template)
            => _context.HrOnboardingTaskTemplates.Update(template);

        public async Task<HrOnboardingTask?> GetTaskByIdAsync(decimal id)
            => await _context.HrOnboardingTasks.FindAsync(id);

        public async Task AddTaskAsync(HrOnboardingTask task)
            => await _context.HrOnboardingTasks.AddAsync(task);

        public void UpdateTask(HrOnboardingTask task)
            => _context.HrOnboardingTasks.Update(task);

        public async Task SaveChangesAsync()
            => await _context.SaveChangesAsync();
    }
}
