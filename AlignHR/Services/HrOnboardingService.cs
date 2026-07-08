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
        private readonly IFileStorageService _fileStorage;
        private readonly IEmployeeCreationService _empCreation;
        private readonly ApplicationDbContext _context;

        public HrOnboardingService(
            IHrOnboardingRepository repository,
            IFileStorageService fileStorage,
            IEmployeeCreationService empCreation,
            ApplicationDbContext context)
        {
            _repository = repository;
            _fileStorage = fileStorage;
            _empCreation = empCreation;
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

            decimal? confirmedByEmpId = onboarding.JoiningConfirmation?.ConfirmedByEmployeeFk;
            if (confirmedByEmpId.HasValue)
                empIds.Add((int)confirmedByEmpId.Value);

            var employees = await _context.emp
                .Where(e => empIds.Contains(e.Id))
                .ToDictionaryAsync(e => e.Id, e => $"{e.Code} - {e.FirstName} {e.LastName}");

            int? createdEmpId = null;
            string? createdEmpCode = null;
            if (onboarding.JoiningConfirmation != null)
            {
                var linked = await _context.emp
                    .Where(e => e.Dateofjoin == DateOnly.FromDateTime(onboarding.JoiningConfirmation.JoinedDate) &&
                                e.FirstName == onboarding.Candidate!.FirstName &&
                                e.LastName == onboarding.Candidate!.LastName)
                    .OrderByDescending(e => e.Id)
                    .FirstOrDefaultAsync();
                createdEmpId = linked?.Id;
                createdEmpCode = linked?.Code;
            }

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
                JoiningConfirmation = onboarding.JoiningConfirmation == null ? null : new HrJoiningConfirmationItemViewModel
                {
                    Id = onboarding.JoiningConfirmation.Id,
                    JoinedDate = onboarding.JoiningConfirmation.JoinedDate,
                    ConfirmedDate = onboarding.JoiningConfirmation.ConfirmedDate,
                    Remarks = onboarding.JoiningConfirmation.Remarks,
                    ConfirmedByName = confirmedByEmpId.HasValue && employees.TryGetValue((int)confirmedByEmpId.Value, out var cn) ? cn : null,
                    CreatedEmployeeId = createdEmpId,
                    CreatedEmployeeCode = createdEmpCode
                },
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
                Documents = onboarding.Documents.Select(d => new HrOnboardingDocumentItemViewModel
                {
                    Id = d.Id,
                    DocumentType = d.DocumentType,
                    FileName = d.FileName,
                    FilePath = d.FilePath,
                    UploadedDate = d.UploadedDate
                }).ToList()
            };
        }

        public async Task<HrJoiningConfirmationFormViewModel?> GetForJoiningAsync(decimal onboardingId)
        {
            var onboarding = await _repository.GetWithDetailsAsync(onboardingId);
            if (onboarding == null) return null;

            return new HrJoiningConfirmationFormViewModel
            {
                OnboardingId = onboardingId,
                CandidateName = onboarding.Candidate != null
                    ? $"{onboarding.Candidate.FirstName} {onboarding.Candidate.LastName}".Trim()
                    : null,
                JobTitle = onboarding.Offer?.JobApplication?.JobPosting?.JobTitle,
                JoinedDate = onboarding.PlannedJoiningDate
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
                PlannedJoiningDate = offer.ProposedJoiningDate ?? DateTime.Today.AddDays(30),
                Status = "Initiated",
                CreatedBy = username,
                CreatedOn = DateTime.Now
            };

            await _repository.AddOnboardingAsync(onboarding);
            await _repository.SaveChangesAsync();

            // Seed tasks from all active templates
            var templates = await _repository.GetActiveTemplatesAsync();
            foreach (var template in templates)
            {
                await _repository.AddTaskAsync(new HrOnboardingTask
                {
                    OnboardingFk = onboarding.Id,
                    TaskTemplateFk = template.Id,
                    Status = "Pending",
                    DueDate = onboarding.PlannedJoiningDate
                });
            }

            onboarding.Status = "TasksInProgress";
            _repository.UpdateOnboarding(onboarding);
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
                    onboarding.Status = "ReadyToJoin";
                onboarding.UpdatedOn = DateTime.Now;
                _repository.UpdateOnboarding(onboarding);
            }

            await _repository.SaveChangesAsync();
        }

        public async Task UploadDocumentAsync(decimal onboardingId, string documentType, IFormFile file)
        {
            var (fileName, relativePath) = await _fileStorage.SaveAsync(file, $"onboarding/{onboardingId:0}");
            await _repository.AddDocumentAsync(new HrOnboardingDocument
            {
                OnboardingFk = onboardingId,
                DocumentType = documentType,
                FileName = fileName,
                FilePath = relativePath,
                UploadedDate = DateTime.Now
            });

            var onboarding = await _repository.GetByIdAsync(onboardingId);
            if (onboarding != null && onboarding.Status == "Initiated")
            {
                onboarding.Status = "DocumentsPending";
                onboarding.UpdatedOn = DateTime.Now;
                _repository.UpdateOnboarding(onboarding);
            }

            await _repository.SaveChangesAsync();
        }

        public async Task DeleteDocumentAsync(decimal documentId)
        {
            var doc = await _repository.GetDocumentByIdAsync(documentId);
            if (doc == null) return;
            if (!string.IsNullOrEmpty(doc.FilePath))
                _fileStorage.Delete(doc.FilePath);
            _repository.RemoveDocument(doc);
            await _repository.SaveChangesAsync();
        }

        public async Task ConfirmJoiningAsync(HrJoiningConfirmationFormViewModel model, decimal confirmedByEmpId)
        {
            if (await _repository.JoiningConfirmationExistsAsync(model.OnboardingId))
                throw new InvalidOperationException("Joining has already been confirmed for this onboarding.");

            var onboarding = await _repository.GetWithDetailsAsync(model.OnboardingId);
            if (onboarding == null)
                throw new InvalidOperationException("Onboarding record not found.");

            await _repository.AddJoiningConfirmationAsync(new HrJoiningConfirmation
            {
                OnboardingId = model.OnboardingId,
                JoinedDate = model.JoinedDate,
                ConfirmedByEmployeeFk = confirmedByEmpId,
                Remarks = model.Remarks,
                ConfirmedDate = DateTime.Now
            });

            onboarding.ActualJoiningDate = model.JoinedDate;
            onboarding.Status = "Joined";
            onboarding.UpdatedOn = DateTime.Now;
            _repository.UpdateOnboarding(onboarding);
            await _repository.SaveChangesAsync();

            // Create Employee record
            await _empCreation.CreateEmployeeFromCandidateAsync(onboarding.CandidateId, onboarding.Id, model);

            onboarding.Status = "Completed";
            onboarding.UpdatedOn = DateTime.Now;
            _repository.UpdateOnboarding(onboarding);
            await _repository.SaveChangesAsync();
        }
    }
}
