using AlignHR.Data;
using AlignHR.Models;
using AlignHR.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace AlignHR.Services
{
    public class HrOnboardingService : IHrOnboardingService
    {
        private readonly IHrOnboardingRepository _repository;
        private readonly ApplicationDbContext _context;

        public HrOnboardingService(
            IHrOnboardingRepository repository,
            ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<IPagedList<HrOnboardingListItemViewModel>> GetPagedAsync(string? search, int pageNumber, int pageSize)
        {
            var onboardings = await _repository.Query()
                .Include(o => o.Candidate)
                .Include(o => o.Offer)
                    .ThenInclude(of => of!.JobApplication)
                        .ThenInclude(a => a!.JobPosting)
                .Include(o => o.Tasks)
                .OrderByDescending(o => o.CreatedOn)
                .ToListAsync();

            var items = onboardings
                .Where(o => string.IsNullOrWhiteSpace(search) ||
                    (o.Candidate != null && (o.Candidate.FirstName + " " + o.Candidate.LastName)
                        .Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                    (o.Status != null && o.Status.Contains(search, StringComparison.OrdinalIgnoreCase)))
                .Select(o => new HrOnboardingListItemViewModel
                {
                    Id = o.Id,
                    CandidateName = o.Candidate != null
                        ? $"{o.Candidate.FirstName} {o.Candidate.LastName}".Trim()
                        : null,
                    JobTitle = o.Offer?.JobApplication?.JobPosting?.JobTitle,
                    OfferNumber = o.Offer?.OfferNumber,
                    PlannedJoiningDate = o.PlannedJoiningDate,
                    ActualJoiningDate = o.ActualJoiningDate,
                    Status = o.Status,
                    TotalTasks = o.Tasks.Count,
                    CompletedTasks = o.Tasks.Count(t => t.Status == "Completed"),
                    CreatedOn = o.CreatedOn
                });

            return items.ToPagedList(pageNumber, pageSize);
        }

        public async Task<HrOnboardingDetailsViewModel?> GetDetailsAsync(decimal id)
        {
            var onboarding = await _repository.GetWithDetailsAsync(id);
            if (onboarding == null) return null;

            var empIds = onboarding.Tasks
                .Where(t => t.AssignedToEmployeeFk.HasValue)
                .Select(t => (int)t.AssignedToEmployeeFk!.Value)
                .Distinct().ToList();

            var employees = await _context.emp
                .Where(e => empIds.Contains(e.Id))
                .ToDictionaryAsync(e => e.Id, e => $"{e.Code} - {e.FirstName} {e.LastName}");

            return new HrOnboardingDetailsViewModel
            {
                Id = onboarding.Id,
                CandidateName = onboarding.Candidate != null
                    ? $"{onboarding.Candidate.FirstName} {onboarding.Candidate.LastName}".Trim()
                    : null,
                CandidateEmail = onboarding.Candidate?.Email,
                JobTitle = onboarding.Offer?.JobApplication?.JobPosting?.JobTitle,
                OfferNumber = onboarding.Offer?.OfferNumber,
                OfferId = onboarding.OfferId,
                PlannedJoiningDate = onboarding.PlannedJoiningDate,
                ActualJoiningDate = onboarding.ActualJoiningDate,
                Status = onboarding.Status,
                CreatedBy = onboarding.CreatedBy,
                CreatedOn = onboarding.CreatedOn,
                Tasks = onboarding.Tasks.Select(t => new HrOnboardingTaskItemViewModel
                {
                    Id = t.Id,
                    TaskName = t.TaskTemplate?.TaskName,
                    ResponsibleDepartment = t.TaskTemplate?.ResponsibleDepartment,
                    IsMandatory = t.TaskTemplate?.IsMandatory ?? false,
                    AssignedToName = t.AssignedToEmployeeFk.HasValue && employees.TryGetValue((int)t.AssignedToEmployeeFk.Value, out var en) ? en : null,
                    DueDate = t.DueDate,
                    Status = t.Status,
                    CompletedDate = t.CompletedDate,
                    Remarks = t.Remarks
                }).ToList(),
            };
        }

        public async Task<decimal> StartOnboardingAsync(decimal offerId, string? username)
        {
            var existing = await _repository.GetByOfferAsync(offerId);
            if (existing != null)
                throw new InvalidOperationException("Onboarding has already been started for this offer.");

            var offer = await _context.HrOffers
                .Include(o => o.JobApplication)
                    .ThenInclude(a => a!.Candidate)
                .FirstOrDefaultAsync(o => o.Id == offerId);

            if (offer == null)
                throw new InvalidOperationException("Offer not found.");

            if (offer.Status != "Accepted")
                throw new InvalidOperationException("Onboarding can only be started for an Accepted offer.");

            var onboarding = new HrOnboarding
            {
                CandidateId = offer.JobApplication!.CandidateFK ?? 0,
                OfferId = offerId,
                PlannedJoiningDate = offer.CandidateJoiningDate ?? DateTime.Today.AddDays(30),
                Status = "Initiated",
                CreatedBy = username,
                CreatedOn = DateTime.Now
            };

            await _repository.AddOnboardingAsync(onboarding);
            await _repository.SaveChangesAsync();

            return onboarding.Id;
        }

        public async Task UpdateTaskStatusAsync(HrTaskUpdateViewModel model)
        {
            var task = await _repository.GetTaskByIdAsync(model.TaskId);
            if (task == null)
                throw new InvalidOperationException("Task not found.");

            task.Status = model.Status;
            task.Remarks = model.Remarks;
            if (model.Status == "Completed")
                task.CompletedDate = DateTime.Now;

            _repository.UpdateTask(task);

            // Recalculate onboarding status
            var onboarding = await _repository.GetWithDetailsAsync(model.OnboardingId);
            if (onboarding != null)
            {
                var allDone = onboarding.Tasks.All(t => t.Status is "Completed" or "Cancelled");
                if (allDone)
                    onboarding.Status = "Completed";

                onboarding.UpdatedOn = DateTime.Now;
                _repository.UpdateOnboarding(onboarding);
            }

            await _repository.SaveChangesAsync();
        }

        public async Task OnEmployeeCreatedAsync(decimal onboardingId, DateTime joiningDate)
        {
            var onboarding = await _repository.GetWithDetailsAsync(onboardingId);
            if (onboarding == null)
                throw new InvalidOperationException("Onboarding record not found.");

            onboarding.ActualJoiningDate = joiningDate;
            onboarding.UpdatedOn = DateTime.Now;
            _repository.UpdateOnboarding(onboarding);

            // Close job posting and fill requisition
            var offer = await _context.HrOffers
                .Include(o => o.JobApplication)
                    .ThenInclude(a => a!.JobPosting)
                .FirstOrDefaultAsync(o => o.Id == onboarding.OfferId);

            if (offer?.JobApplication?.JobPosting != null)
            {
                var posting = offer.JobApplication.JobPosting;
                posting.PostingStatus = "Closed";
                _context.HrJobPostings.Update(posting);

                if (posting.RequisitionFK.HasValue)
                {
                    var req = await _context.HrRequisitions
                        .FirstOrDefaultAsync(r => r.Id == posting.RequisitionFK.Value);
                    if (req != null)
                    {
                        req.Status = "Filled";
                        _context.HrRequisitions.Update(req);
                    }
                }
            }

            // Seed onboarding tasks from templates
            var templates = await _repository.GetActiveTemplatesAsync();
            foreach (var template in templates)
            {
                await _repository.AddTaskAsync(new HrOnboardingTask
                {
                    OnboardingFk = onboardingId,
                    TaskTemplateFk = template.Id,
                    Status = "Pending",
                    DueDate = joiningDate
                });
            }

            onboarding.Status = templates.Any() ? "TasksInProgress" : "Completed";
            onboarding.UpdatedOn = DateTime.Now;
            _repository.UpdateOnboarding(onboarding);

            // Move application stage to Onboarded
            var onboardedStage = await _context.HrApplicationStages
                .FirstOrDefaultAsync(s => s.StageName == "Onboarded");
            if (onboardedStage != null && offer?.JobApplication != null)
            {
                var app = offer.JobApplication;
                var fromStageId = app.CurrentStageFK;
                app.CurrentStageFK = onboardedStage.Id;
                _context.HrJobApplications.Update(app);

                await _context.HrApplicationStageHistories.AddAsync(new HrApplicationStageHistory
                {
                    JobApplicationFk = app.Id,
                    FromStageId = fromStageId,
                    ToStageFk = onboardedStage.Id,
                    ChangedDate = DateTime.Now,
                    Comments = "Employee profile created — candidate onboarded."
                });
            }

            await _repository.SaveChangesAsync();
        }
    }
}
