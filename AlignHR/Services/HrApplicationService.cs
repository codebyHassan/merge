using AlignHR.Data;
using AlignHR.Models;
using AlignHR.Repositories;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace AlignHR.Services
{
    public class HrApplicationService : IHrApplicationService
    {
        private readonly IHrApplicationRepository _repository;
        private readonly ApplicationDbContext _context;

        public HrApplicationService(IHrApplicationRepository repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<IPagedList<HrApplicationListItemViewModel>> GetByJobPostingAsync(decimal jobPostingId, string? search, int pageNumber, int pageSize)
        {
            var query = _repository.Query()
                .Where(a => a.JobPostingFK == jobPostingId);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(a =>
                    (a.Candidate != null && (
                        a.Candidate.FirstName.Contains(search) ||
                        (a.Candidate.LastName != null && a.Candidate.LastName.Contains(search)) ||
                        a.Candidate.Email.Contains(search))));

            var apps = await query.OrderByDescending(a => a.AppliedDate).ToListAsync();

            return apps.Select(a => new HrApplicationListItemViewModel
            {
                Id = a.Id,
                CandidateFK = a.CandidateFK,
                CandidateName = a.Candidate != null ? $"{a.Candidate.FirstName} {a.Candidate.LastName}".Trim() : null,
                CandidateEmail = a.Candidate?.Email,
                CandidatePhone = a.Candidate?.Phone,
                TotalExperienceYears = a.Candidate?.TotalExperienceYears,
                CurrentStage = a.CurrentStage?.StageName,
                CurrentStageFK = a.CurrentStageFK,
                JobPostingFK = a.JobPostingFK,
                JobCode = a.JobPosting?.JobCode,
                JobTitle = a.JobPosting?.JobTitle,
                AppliedDate = a.AppliedDate,
                IsActive = a.IsActive
            }).ToPagedList(pageNumber, pageSize);
        }

        public async Task<HrApplicationDetailsViewModel?> GetDetailsAsync(decimal id)
        {
            var app = await _repository.GetWithDetailsAsync(id);
            if (app == null) return null;

            // Query history directly to ensure feedback-submitted entries are included
            var history = await _context.HrApplicationStageHistories
                .Include(h => h.FromStage)
                .Include(h => h.ToStage)
                .Where(h => h.JobApplicationFk == id)
                .OrderBy(h => h.ChangedDate)
                .ToListAsync();

            // Load all offers (including withdrawn) newest first
            var offers = await _context.HrOffers
                .Include(o => o.Approvals)
                .Include(o => o.Responses)
                .Where(o => o.JobApplicationFk == id)
                .OrderByDescending(o => o.CreatedOn)
                .ToListAsync();

            var activeOffer = offers.FirstOrDefault(o => o.Status != "Withdrawn");

            var hiringDecision = await _context.HrHiringDecisions
                .FirstOrDefaultAsync(d => d.JobApplicationFk == id);

            // Load onboarding via active offer
            HrOnboarding? onboarding = null;
            if (activeOffer != null)
            {
                onboarding = await _context.HrOnboardings
                    .Include(o => o.Tasks).ThenInclude(t => t.TaskTemplate)
                    .Include(o => o.Documents)
                    .Include(o => o.JoiningConfirmation)
                    .FirstOrDefaultAsync(o => o.OfferId == activeOffer.Id);
            }

            var historyEmpIds = history
                .Where(h => h.ChangedBy.HasValue)
                .Select(h => (int)h.ChangedBy!.Value);

            var approverEmpIds = offers
                .SelectMany(o => o.Approvals)
                .Where(a => a.ApproverEmployeeFk.HasValue)
                .Select(a => (int)a.ApproverEmployeeFk!.Value);

            var employeeIds = historyEmpIds.Union(approverEmpIds).Distinct().ToList();

            var employees = employeeIds.Any()
                ? await _context.emp
                    .Where(e => employeeIds.Contains(e.Id))
                    .ToDictionaryAsync(e => e.Id, e => $"{e.FirstName} {e.LastName}".Trim())
                : new Dictionary<int, string>();

            var schedules = await _context.HrInterviewSchedules
                .Include(s => s.InterviewRound)
                .Include(s => s.Panel)
                .Include(s => s.Feedback)
                .Where(s => s.JobApplicationId == id)
                .OrderBy(s => s.InterviewRound != null ? s.InterviewRound.RoundOrder : 0)
                .ThenBy(s => s.ScheduledDateTime)
                .ToListAsync();

            var totalRoundCount = app.JobPostingFK.HasValue
                ? await _context.HrInterviewRounds
                    .CountAsync(r => r.JobPostingId == app.JobPostingFK.Value && r.IsActive)
                : 0;

            var scheduledRoundIds = schedules
                .Select(s => s.InterviewRoundId)
                .Distinct()
                .ToHashSet();

            var interviewItems = schedules.Select(s => new HrInterviewListItemViewModel
            {
                Id = s.Id,
                RoundName = s.InterviewRound?.RoundName,
                RoundOrder = s.InterviewRound?.RoundOrder ?? 0,
                ScheduledDateTime = s.ScheduledDateTime,
                Status = s.Status,
                PanelCount = s.Panel.Count,
                FeedbackCount = s.Feedback.Count,
                HasSubmittedFeedback = s.Feedback.Any(f => f.IsSubmitted),
                AverageScore = s.Feedback.Any(f => f.OverallScore.HasValue)
                    ? s.Feedback.Where(f => f.OverallScore.HasValue).Average(f => f.OverallScore)
                    : null
            }).ToList();

            return new HrApplicationDetailsViewModel
            {
                Id = app.Id,
                CandidateFK = app.CandidateFK,
                CandidateName = app.Candidate != null ? $"{app.Candidate.FirstName} {app.Candidate.LastName}".Trim() : null,
                CandidateEmail = app.Candidate?.Email,
                CandidatePhone = app.Candidate?.Phone,
                TotalExperienceYears = app.Candidate?.TotalExperienceYears,
                CurrentEmployer = app.Candidate?.CurrentEmployer,
                JobPostingFK = app.JobPostingFK,
                JobCode = app.JobPosting?.JobCode,
                JobTitle = app.JobPosting?.JobTitle,
                CurrentStageFK = app.CurrentStageFK,
                CurrentStage = app.CurrentStage?.StageName,
                AppliedDate = app.AppliedDate,
                RecruiterNotes = app.RecruiterNotes,
                IsActive = app.IsActive,
                HasInterviewRounds = totalRoundCount > 0,
                AllRoundsScheduled = totalRoundCount > 0 && scheduledRoundIds.Count >= totalRoundCount,
                AllFeedbackSubmitted = interviewItems.Any() && interviewItems.All(i => i.HasSubmittedFeedback),
                Interviews = interviewItems,
                CandidateDocuments = app.Candidate?.Documents.Select(d => new HrCandidateDocumentViewModel
                {
                    Id = d.Id,
                    DocumentType = d.DocumentType,
                    FileName = d.FileName,
                    FilePath = d.FilePath,
                    UploadedDate = d.UploadedDate
                }).ToList() ?? new(),
                History = history.Select(h => new HrApplicationStageHistoryItemViewModel
                {
                    FromStage = h.FromStage?.StageName,
                    ToStage = h.ToStage?.StageName,
                    ChangedByName = h.ChangedBy.HasValue && employees.TryGetValue((int)h.ChangedBy.Value, out var name) ? name : null,
                    Comments = h.Comments,
                    ChangedDate = h.ChangedDate
                })
                .Concat(offers.SelectMany(o => o.Approvals
                    .Where(a => a.ActionDate.HasValue && a.Status != "Pending")
                    .Select(a => new HrApplicationStageHistoryItemViewModel
                    {
                        Comments = $"Offer {a.Status} — {o.OfferNumber}" + (string.IsNullOrWhiteSpace(a.Comments) ? "" : $" | {a.Comments}"),
                        ChangedByName = a.ApproverEmployeeFk.HasValue && employees.TryGetValue((int)a.ApproverEmployeeFk.Value, out var aname) ? aname : null,
                        ChangedDate = a.ActionDate
                    })))
                .OrderBy(h => h.ChangedDate)
                .ToList(),
                Offers = offers.Select(offer => new HrApplicationOfferSummaryViewModel
                {
                    Id = offer.Id,
                    OfferNumber = offer.OfferNumber,
                    ProposedSalary = offer.ProposedSalary,
                    ProposedJoiningDate = offer.ProposedJoiningDate,
                    ExpiryDate = offer.ExpiryDate,
                    Status = offer.Status,
                    TotalApprovals = offer.Approvals.Count,
                    ApprovedCount = offer.Approvals.Count(a => a.Status == "Approved"),
                    LastResponseType = offer.Responses.OrderByDescending(r => r.ResponseDate).FirstOrDefault()?.ResponseType,
                    Decision = hiringDecision?.Decision,
                    Approvals = offer.Approvals.OrderBy(a => a.ApprovalLevel).Select(a => new HrApplicationOfferApprovalSummary
                    {
                        ApproverName = a.ApproverEmployeeFk.HasValue && employees.TryGetValue((int)a.ApproverEmployeeFk.Value, out var ename) ? ename : null,
                        Status = a.Status,
                        Comments = a.Comments,
                        ActionDate = a.ActionDate
                    }).ToList()
                }).ToList(),
                Onboarding = onboarding == null ? null : new HrApplicationOnboardingSummaryViewModel
                {
                    Id = onboarding.Id,
                    Status = onboarding.Status,
                    PlannedJoiningDate = onboarding.PlannedJoiningDate,
                    ActualJoiningDate = onboarding.ActualJoiningDate,
                    TotalTasks = onboarding.Tasks.Count,
                    CompletedTasks = onboarding.Tasks.Count(t => t.Status == "Completed"),
                    HasJoiningConfirmation = onboarding.JoiningConfirmation != null,
                    JoinedDate = onboarding.JoiningConfirmation?.JoinedDate,
                    Tasks = onboarding.Tasks.Select(t => new HrOnboardingTaskItemViewModel
                    {
                        Id = t.Id,
                        TaskName = t.TaskTemplate?.TaskName,
                        ResponsibleDepartment = t.TaskTemplate?.ResponsibleDepartment,
                        IsMandatory = t.TaskTemplate?.IsMandatory ?? false,
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
                }
            };
        }

        public async Task<HrPipelineViewModel?> GetPipelineAsync(decimal jobPostingId)
        {
            var posting = await _context.HrJobPostings.FirstOrDefaultAsync(p => p.Id == jobPostingId);
            if (posting == null) return null;

            var stages = await _repository.GetActiveStagesAsync();
            var apps = await _repository.Query()
                .Where(a => a.JobPostingFK == jobPostingId && a.IsActive)
                .ToListAsync();

            return new HrPipelineViewModel
            {
                JobPostingId = jobPostingId,
                JobCode = posting.JobCode,
                JobTitle = posting.JobTitle,
                Stages = stages.Select(s => new HrPipelineStageViewModel
                {
                    StageId = s.Id,
                    StageName = s.StageName,
                    Order = s.StageOrder ?? 0,
                    Applications = apps
                        .Where(a => a.CurrentStageFK == s.Id)
                        .Select(a => new HrApplicationListItemViewModel
                        {
                            Id = a.Id,
                            CandidateFK = a.CandidateFK,
                            CandidateName = a.Candidate != null ? $"{a.Candidate.FirstName} {a.Candidate.LastName}".Trim() : null,
                            CandidateEmail = a.Candidate?.Email,
                            TotalExperienceYears = a.Candidate?.TotalExperienceYears,
                            CurrentStage = a.CurrentStage?.StageName,
                            CurrentStageFK = a.CurrentStageFK,
                            AppliedDate = a.AppliedDate,
                            IsActive = a.IsActive
                        }).ToList()
                }).ToList()
            };
        }

        public async Task<HrAtsDashboardViewModel> GetDashboardAsync()
        {
            var openPostings = await _context.HrJobPostings
                .Where(p => p.PostingStatus == "Published" && !p.IsDeleted)
                .OrderByDescending(p => p.CreatedOn)
                .Take(5)
                .ToListAsync();

            var activeApps = await _context.HrJobApplications.CountAsync(a => a.IsActive);
            var pipeline = await _context.HrJobApplications
                .Where(a => a.IsActive)
                .Include(a => a.CurrentStage)
                .ToListAsync();

            var byStage = pipeline
                .Where(a => a.CurrentStage != null)
                .GroupBy(a => a.CurrentStage!.StageName)
                .Select(g => new HrStageCountViewModel { StageName = g.Key, Count = g.Count() })
                .ToList();

            var recentApps = await _context.HrJobApplications
                .Include(a => a.Candidate)
                .Include(a => a.CurrentStage)
                .Where(a => a.IsActive)
                .OrderByDescending(a => a.AppliedDate)
                .Take(10)
                .ToListAsync();

            return new HrAtsDashboardViewModel
            {
                OpenJobPostings = openPostings.Count,
                ActiveApplications = activeApps,
                CandidatesInPipeline = pipeline.Select(a => a.CandidateFK).Distinct().Count(),
                ApplicationsByStage = byStage,
                RecentApplications = recentApps.Select(a => new HrApplicationListItemViewModel
                {
                    Id = a.Id,
                    CandidateFK = a.CandidateFK,
                    CandidateName = a.Candidate != null ? $"{a.Candidate.FirstName} {a.Candidate.LastName}".Trim() : null,
                    CandidateEmail = a.Candidate?.Email,
                    CurrentStage = a.CurrentStage?.StageName,
                    AppliedDate = a.AppliedDate,
                    IsActive = a.IsActive
                }).ToList(),
                ActivePostings = openPostings.Select(p => new HrJobPostingListItemViewModel
                {
                    Id = p.Id,
                    JobCode = p.JobCode,
                    JobTitle = p.JobTitle,
                    PostingStatus = p.PostingStatus,
                    OpenDate = p.OpenDate,
                    CloseDate = p.CloseDate
                }).ToList()
            };
        }

        public async Task<HrMoveStageViewModel?> GetForMoveStageAsync(decimal applicationId)
        {
            var app = await _repository.GetByIdAsync(applicationId);
            if (app == null) return null;

            return new HrMoveStageViewModel
            {
                ApplicationId = applicationId,
                CandidateName = app.Candidate != null ? $"{app.Candidate.FirstName} {app.Candidate.LastName}".Trim() : null,
                CurrentStage = app.CurrentStage?.StageName,
                CurrentStageFK = app.CurrentStageFK
            };
        }

        public async Task<decimal> ApplyAsync(HrApplicationFormViewModel model, decimal? actionByEmployeeId)
        {
            if (!model.JobPostingFK.HasValue)
                throw new InvalidOperationException("Job posting is required.");

            if (!model.CandidateFK.HasValue)
                throw new InvalidOperationException("Candidate is required.");

            var posting = await _context.HrJobPostings.FirstOrDefaultAsync(p => p.Id == model.JobPostingFK.Value);
            if (posting == null || posting.PostingStatus != "Published")
                throw new InvalidOperationException("Applications can only be added to published job postings.");

            if (await _repository.ActiveApplicationExistsAsync(model.CandidateFK.Value, model.JobPostingFK.Value))
                throw new InvalidOperationException("This candidate already has an active application for this posting.");

            var appliedStage = await _repository.GetStageByNameAsync("Applied");
            if (appliedStage == null)
                throw new InvalidOperationException("Stage 'Applied' not found. Ensure application stages are seeded.");

            var application = new HrJobApplication
            {
                CandidateFK = model.CandidateFK,
                JobPostingFK = model.JobPostingFK,
                CurrentStageFK = appliedStage.Id,
                AppliedDate = model.AppliedDate,
                RecruiterNotes = model.RecruiterNotes,
                IsActive = true
            };

            await _repository.AddAsync(application);
            await _repository.SaveChangesAsync();

            await _repository.AddHistoryAsync(new HrApplicationStageHistory
            {
                JobApplicationFk = application.Id,
                FromStageId = null,
                ToStageFk = appliedStage.Id,
                ChangedBy = actionByEmployeeId,
                Comments = "Application received.",
                ChangedDate = DateTime.Now
            });
            await _repository.SaveChangesAsync();

            return application.Id;
        }

        public async Task MoveStageAsync(decimal applicationId, decimal toStageId, string? comments, decimal? changedBy)
        {
            var app = await _repository.GetByIdAsync(applicationId);
            if (app == null)
                throw new InvalidOperationException("Application not found.");

            if (!app.IsActive)
                throw new InvalidOperationException("Cannot move an inactive application.");

            var fromStageId = app.CurrentStageFK;
            app.CurrentStageFK = toStageId;
            _repository.Update(app);

            await _repository.AddHistoryAsync(new HrApplicationStageHistory
            {
                JobApplicationFk = applicationId,
                FromStageId = fromStageId,
                ToStageFk = toStageId,
                ChangedBy = changedBy,
                Comments = comments,
                ChangedDate = DateTime.Now
            });

            await _repository.SaveChangesAsync();
        }

        public async Task ShortlistAsync(decimal applicationId, decimal? changedBy)
        {
            var stage = await _repository.GetStageByNameAsync("Shortlisted");
            if (stage == null)
                throw new InvalidOperationException("'Shortlisted' stage is not configured. Please add it in application stage settings.");
            await MoveStageAsync(applicationId, stage.Id, "Candidate shortlisted.", changedBy);
        }

        public async Task RejectAsync(decimal applicationId, string? reason, decimal? changedBy)
        {
            var app = await _repository.GetByIdAsync(applicationId);
            if (app == null) throw new InvalidOperationException("Application not found.");
            if (!app.IsActive) throw new InvalidOperationException("Application is already inactive.");

            var rejectedStage = await _repository.GetStageByNameAsync("Rejected");
            var fromStageId = app.CurrentStageFK;

            if (rejectedStage != null)
                app.CurrentStageFK = rejectedStage.Id;

            app.IsActive = false;
            _repository.Update(app);

            await _repository.AddHistoryAsync(new HrApplicationStageHistory
            {
                JobApplicationFk = applicationId,
                FromStageId = fromStageId,
                ToStageFk = rejectedStage?.Id ?? fromStageId,
                ChangedBy = changedBy,
                Comments = string.IsNullOrWhiteSpace(reason) ? "Application rejected." : $"Rejected: {reason}",
                ChangedDate = DateTime.Now
            });

            await _repository.SaveChangesAsync();
        }

        public async Task WithdrawAsync(decimal applicationId, decimal? changedBy)
        {
            var app = await _repository.GetByIdAsync(applicationId);
            if (app == null) throw new InvalidOperationException("Application not found.");
            if (!app.IsActive) throw new InvalidOperationException("Application is already inactive.");

            var stage = await _repository.GetStageByNameAsync("Withdrawn");
            var fromStageId = app.CurrentStageFK;

            if (stage != null) app.CurrentStageFK = stage.Id;
            app.IsActive = false;
            _repository.Update(app);

            await _repository.AddHistoryAsync(new HrApplicationStageHistory
            {
                JobApplicationFk = applicationId,
                FromStageId = fromStageId,
                ToStageFk = stage?.Id ?? fromStageId,
                ChangedBy = changedBy,
                Comments = "Candidate withdrew application.",
                ChangedDate = DateTime.Now
            });

            await _repository.SaveChangesAsync();
        }

        public async Task UpdateNotesAsync(decimal applicationId, string? notes)
        {
            var app = await _repository.GetByIdAsync(applicationId);
            if (app == null) return;
            app.RecruiterNotes = notes;
            _repository.Update(app);
            await _repository.SaveChangesAsync();
        }

        public async Task DeactivateAsync(decimal applicationId)
        {
            var app = await _repository.GetByIdAsync(applicationId);
            if (app == null) return;
            app.IsActive = false;
            _repository.Update(app);
            await _repository.SaveChangesAsync();
        }

    }
}
