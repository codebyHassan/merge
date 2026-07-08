using AlignHR.Data;
using AlignHR.Models;
using AlignHR.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AlignHR.Services
{
    public class HrInterviewService : IHrInterviewService
    {
        private readonly IHrInterviewRepository _repository;
        private readonly ApplicationDbContext _context;

        public HrInterviewService(IHrInterviewRepository repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<HrApplicationInterviewsViewModel?> GetApplicationInterviewsAsync(decimal applicationId, decimal? currentEmpId = null)
        {
            var app = await _context.HrJobApplications
                .Include(a => a.Candidate)
                .Include(a => a.JobPosting)
                .FirstOrDefaultAsync(a => a.Id == applicationId);
            if (app == null) return null;

            var schedules = await _repository.GetSchedulesByApplicationAsync(applicationId);

            // Check if current user is the assigned recruiter for this posting's requisition
            bool isRecruiter = false;
            if (currentEmpId.HasValue && app.JobPosting?.RequisitionFK.HasValue == true)
            {
                isRecruiter = await _context.HrRequisitionAssignments
                    .AnyAsync(a => a.RequisitionFk == app.JobPosting.RequisitionFK.Value
                               && a.RecruiterEmployeeFK == currentEmpId.Value);
            }

            return new HrApplicationInterviewsViewModel
            {
                ApplicationId = applicationId,
                CandidateName = app.Candidate != null
                    ? $"{app.Candidate.FirstName} {app.Candidate.LastName}".Trim()
                    : null,
                JobCode = app.JobPosting?.JobCode,
                JobTitle = app.JobPosting?.JobTitle,
                JobPostingId = app.JobPostingFK,
                IsRecruiter = isRecruiter,
                Interviews = schedules.Select(s => new HrInterviewListItemViewModel
                {
                    Id = s.Id,
                    RoundName = s.InterviewRound?.RoundName,
                    RoundOrder = s.InterviewRound?.RoundOrder ?? 0,
                    ScheduledDateTime = s.ScheduledDateTime,
                    Status = s.Status,
                    PanelCount = s.Panel.Count,
                    FeedbackCount = s.Feedback.Count,
                    AverageScore = s.Feedback.Any()
                        ? s.Feedback.Where(f => f.OverallScore.HasValue).Average(f => f.OverallScore)
                        : null,
                    IsCurrentUserOnPanel = currentEmpId.HasValue
                        && s.Panel.Any(p => p.EmployeeFk == currentEmpId.Value)
                }).ToList()
            };
        }

        public async Task<HrScheduleFormViewModel?> GetForScheduleAsync(decimal applicationId)
        {
            var app = await _context.HrJobApplications
                .Include(a => a.Candidate)
                .Include(a => a.JobPosting)
                .FirstOrDefaultAsync(a => a.Id == applicationId);
            if (app == null) return null;

            return new HrScheduleFormViewModel
            {
                ApplicationId = applicationId,
                JobPostingId = app.JobPostingFK,
                CandidateName = app.Candidate != null
                    ? $"{app.Candidate.FirstName} {app.Candidate.LastName}".Trim()
                    : null,
                JobTitle = app.JobPosting?.JobTitle,
                ScheduledDateTime = DateTime.Now.AddDays(1)
            };
        }

        public async Task<HrInterviewEditViewModel?> GetForEditAsync(decimal scheduleId)
        {
            var schedule = await _repository.GetScheduleWithDetailsAsync(scheduleId);
            if (schedule == null) return null;

            var app = schedule.JobApplication;
            return new HrInterviewEditViewModel
            {
                Id = schedule.Id,
                ApplicationId = schedule.JobApplicationId,
                JobPostingId = app?.JobPosting?.Id,
                CandidateName = app?.Candidate != null
                    ? $"{app.Candidate.FirstName} {app.Candidate.LastName}".Trim()
                    : null,
                JobTitle = app?.JobPosting?.JobTitle,
                InterviewRoundId = schedule.InterviewRoundId,
                ScheduledDateTime = schedule.ScheduledDateTime,
                MeetingLink = schedule.MeetingLink,
                Location = schedule.Location,
                PanelEmployeeIds = schedule.Panel.Select(p => p.EmployeeFk).ToList()
            };
        }

        public async Task UpdateScheduleAsync(HrInterviewEditViewModel model)
        {
            var schedule = await _repository.GetScheduleByIdAsync(model.Id);
            if (schedule == null)
                throw new InvalidOperationException("Interview schedule not found.");

            schedule.InterviewRoundId = model.InterviewRoundId!.Value;
            schedule.ScheduledDateTime = model.ScheduledDateTime;
            schedule.MeetingLink = model.MeetingLink;
            schedule.Location = model.Location;
            _repository.UpdateSchedule(schedule);

            // Sync panel members
            var existingPanel = await _context.HrInterviewPanels
                .Where(p => p.InterviewScheduleId == model.Id)
                .ToListAsync();

            var newIds = model.PanelEmployeeIds.ToHashSet();
            var existingIds = existingPanel.Select(p => p.EmployeeFk).ToHashSet();

            foreach (var entry in existingPanel.Where(p => !newIds.Contains(p.EmployeeFk)))
                _repository.RemovePanelEntry(entry);

            foreach (var empId in newIds.Where(id => !existingIds.Contains(id)))
                await _repository.AddPanelEntryAsync(new HrInterviewPanel
                {
                    InterviewScheduleId = model.Id,
                    EmployeeFk = empId
                });

            await _repository.SaveChangesAsync();
        }

        public async Task<decimal> ScheduleInterviewAsync(HrScheduleFormViewModel model, decimal createdByEmpId)
        {
            if (!model.InterviewRoundId.HasValue)
                throw new InvalidOperationException("Interview round is required.");

            var schedule = new HrInterviewSchedule
            {
                JobApplicationId = model.ApplicationId,
                InterviewRoundId = model.InterviewRoundId.Value,
                ScheduledDateTime = model.ScheduledDateTime,
                MeetingLink = model.MeetingLink,
                Location = model.Location,
                Status = "Scheduled",
                CreatedBy = createdByEmpId,
                CreatedDate = DateTime.Now
            };

            await _repository.AddScheduleAsync(schedule);
            await _repository.SaveChangesAsync();

            foreach (var empId in model.PanelEmployeeIds.Distinct())
            {
                await _repository.AddPanelEntryAsync(new HrInterviewPanel
                {
                    InterviewScheduleId = schedule.Id,
                    EmployeeFk = empId
                });
            }
            await _repository.SaveChangesAsync();

            // Auto-advance application to Interview stage if not already there or beyond
            var app = await _context.HrJobApplications
                .Include(a => a.CurrentStage)
                .FirstOrDefaultAsync(a => a.Id == model.ApplicationId);
            var interviewStage = await _context.HrApplicationStages
                .FirstOrDefaultAsync(s => s.StageName == "Interview" && s.IsActive);
            if (app != null && interviewStage != null
                && (app.CurrentStage?.StageOrder ?? 0) < (interviewStage.StageOrder ?? 4))
            {
                var fromId = app.CurrentStageFK;
                app.CurrentStageFK = interviewStage.Id;
                _context.HrJobApplications.Update(app);
                _context.HrApplicationStageHistories.Add(new HrApplicationStageHistory
                {
                    JobApplicationFk = app.Id,
                    FromStageId = fromId,
                    ToStageFk = interviewStage.Id,
                    ChangedBy = createdByEmpId,
                    Comments = "Interview stage — first interview scheduled.",
                    ChangedDate = DateTime.Now
                });
                await _repository.SaveChangesAsync();
            }

            return schedule.Id;
        }

        public async Task<HrInterviewDetailsViewModel?> GetDetailsAsync(decimal scheduleId)
        {
            var schedule = await _repository.GetScheduleWithDetailsAsync(scheduleId);
            if (schedule == null) return null;

            var candidate = schedule.JobApplication?.Candidate;
            var posting = schedule.JobApplication?.JobPosting;

            var empIds = schedule.Panel.Select(p => (int)p.EmployeeFk)
                .Union(schedule.Feedback.Select(f => (int)f.InterviewerEmployeeFk))
                .Distinct().ToList();

            var employees = await _context.emp
                .Where(e => empIds.Contains(e.Id))
                .ToDictionaryAsync(e => e.Id, e => $"{e.Code} - {e.FirstName} {e.LastName}");

            // Index feedback by interviewer employee FK for O(1) lookup
            var feedbackByEmp = schedule.Feedback
                .GroupBy(f => f.InterviewerEmployeeFk)
                .ToDictionary(g => g.Key, g => g.OrderByDescending(f => f.IsSubmitted).First());

            var avgScore = schedule.Feedback.Any(f => f.OverallScore.HasValue)
                ? schedule.Feedback.Where(f => f.OverallScore.HasValue).Average(f => f.OverallScore)
                : null;

            var criteria = schedule.InterviewRoundId > 0
                ? await _context.HrEvaluationCriterias
                    .Where(c => c.InterviewRoundId == schedule.InterviewRoundId)
                    .OrderBy(c => c.CriteriaName)
                    .ToListAsync()
                : new List<HrEvaluationCriteria>();

            return new HrInterviewDetailsViewModel
            {
                Id = schedule.Id,
                ApplicationId = schedule.JobApplicationId,
                CandidateName = candidate != null
                    ? $"{candidate.FirstName} {candidate.LastName}".Trim()
                    : null,
                JobCode = posting?.JobCode,
                JobTitle = posting?.JobTitle,
                JobPostingId = posting?.Id,
                RoundName = schedule.InterviewRound?.RoundName,
                RoundOrder = schedule.InterviewRound?.RoundOrder ?? 0,
                ScheduledDateTime = schedule.ScheduledDateTime,
                MeetingLink = schedule.MeetingLink,
                Location = schedule.Location,
                Status = schedule.Status,
                AverageScore = avgScore,
                Criteria = criteria.Select((c, i) => new HrEvaluationCriteriaItemViewModel
                {
                    Id = c.Id,
                    CriteriaName = c.CriteriaName,
                    MaxScore = c.MaxScore
                }).ToList(),
                Panel = schedule.Panel.Select(p =>
                {
                    feedbackByEmp.TryGetValue(p.EmployeeFk, out var fb);
                    return new HrPanelMemberViewModel
                    {
                        PanelId = p.Id,
                        EmployeeId = p.EmployeeFk,
                        EmployeeName = employees.TryGetValue((int)p.EmployeeFk, out var n) ? n : null,
                        HasSubmittedFeedback = fb != null,
                        FeedbackId = fb?.Id,
                        OverallScore = fb?.OverallScore,
                        Recommendation = fb?.Recommendation,
                        IsSubmitted = fb?.IsSubmitted ?? false
                    };
                }).ToList(),
                Feedback = schedule.Feedback.Select(f => new HrFeedbackSummaryViewModel
                {
                    Id = f.Id,
                    InterviewerName = employees.TryGetValue((int)f.InterviewerEmployeeFk, out var fn) ? fn : null,
                    OverallScore = f.OverallScore,
                    Recommendation = f.Recommendation,
                    IsSubmitted = f.IsSubmitted,
                    SubmittedDate = f.SubmittedDate
                }).ToList()
            };
        }

        public async Task ChangeStatusAsync(decimal scheduleId, string newStatus)
        {
            var schedule = await _repository.GetScheduleByIdAsync(scheduleId);
            if (schedule == null)
                throw new InvalidOperationException("Interview schedule not found.");

            schedule.Status = newStatus;
            _repository.UpdateSchedule(schedule);
            await _repository.SaveChangesAsync();
        }

        public async Task RescheduleAsync(decimal scheduleId, DateTime newDateTime, string? newLink, string? newLocation)
        {
            var schedule = await _repository.GetScheduleByIdAsync(scheduleId);
            if (schedule == null)
                throw new InvalidOperationException("Interview schedule not found.");

            schedule.ScheduledDateTime = newDateTime;
            schedule.MeetingLink = newLink ?? schedule.MeetingLink;
            schedule.Location = newLocation ?? schedule.Location;
            schedule.Status = "Rescheduled";
            _repository.UpdateSchedule(schedule);
            await _repository.SaveChangesAsync();
        }

        public async Task AddPanelistAsync(decimal scheduleId, decimal employeeId)
        {
            if (await _repository.PanelMemberExistsAsync(scheduleId, employeeId))
                throw new InvalidOperationException("This employee is already on the panel.");

            await _repository.AddPanelEntryAsync(new HrInterviewPanel
            {
                InterviewScheduleId = scheduleId,
                EmployeeFk = employeeId
            });
            await _repository.SaveChangesAsync();
        }

        public async Task RemovePanelistAsync(decimal panelId)
        {
            var panel = await _repository.GetPanelEntryByIdAsync(panelId);
            if (panel == null) return;
            _repository.RemovePanelEntry(panel);
            await _repository.SaveChangesAsync();
        }

        public async Task<HrFeedbackFormViewModel?> GetFeedbackFormAsync(decimal scheduleId)
        {
            var schedule = await _repository.GetScheduleWithDetailsAsync(scheduleId);
            if (schedule == null) return null;

            var existing = await _repository.GetFeedbackByScheduleAsync(scheduleId);
            var criteria = schedule.InterviewRoundId > 0
                ? await _repository.GetCriteriaAsync(schedule.InterviewRoundId)
                : new List<HrEvaluationCriteria>();

            var candidate = schedule.JobApplication?.Candidate;

            if (existing != null)
            {
                return new HrFeedbackFormViewModel
                {
                    Id = existing.Id,
                    ScheduleId = scheduleId,
                    ApplicationId = schedule.JobApplicationId,
                    CandidateName = candidate != null
                        ? $"{candidate.FirstName} {candidate.LastName}".Trim()
                        : null,
                    RoundName = schedule.InterviewRound?.RoundName,
                    IsSubmitted = existing.IsSubmitted,
                    Recommendation = existing.Recommendation,
                    OverallScore = existing.OverallScore,
                    Strengths = existing.Strengths,
                    Concerns = existing.Concerns,
                    Comments = existing.Comments,
                    CriteriaScores = criteria.Select(c => new HrCriteriaScoreInputViewModel
                    {
                        CriteriaId = c.Id,
                        CriteriaName = c.CriteriaName,
                        MaxScore = c.MaxScore,
                        Score = existing.Scores.FirstOrDefault(s => s.EvaluationCriteriaFk == c.Id)?.Score ?? 0
                    }).ToList()
                };
            }

            return new HrFeedbackFormViewModel
            {
                Id = 0,
                ScheduleId = scheduleId,
                ApplicationId = schedule.JobApplicationId,
                CandidateName = candidate != null
                    ? $"{candidate.FirstName} {candidate.LastName}".Trim()
                    : null,
                RoundName = schedule.InterviewRound?.RoundName,
                CriteriaScores = criteria.Select(c => new HrCriteriaScoreInputViewModel
                {
                    CriteriaId = c.Id,
                    CriteriaName = c.CriteriaName,
                    MaxScore = c.MaxScore,
                    Score = 0
                }).ToList()
            };
        }

        public async Task SubmitFeedbackAsync(HrFeedbackFormViewModel model, decimal interviewerEmpId, bool isSubmit)
        {
            // Auto-calculate overall score from criteria (as percentage 0–100)
            if (model.CriteriaScores.Any())
            {
                var totalScore = model.CriteriaScores.Sum(cs => cs.Score);
                var totalMax   = model.CriteriaScores.Sum(cs => cs.MaxScore);
                model.OverallScore = totalMax > 0
                    ? Math.Round((decimal)(totalScore / totalMax) * 100, 1)
                    : null;
            }

            // Load schedule to get round name and application ID for history
            var schedule = await _context.HrInterviewSchedules
                .Include(s => s.InterviewRound)
                .FirstOrDefaultAsync(s => s.Id == model.ScheduleId);

            var existing = await _repository.GetFeedbackByScheduleAsync(model.ScheduleId);

            if (existing != null)
            {
                existing.Recommendation = model.Recommendation;
                existing.OverallScore   = model.OverallScore;
                existing.Strengths      = model.Strengths;
                existing.Concerns       = model.Concerns;
                existing.Comments       = model.Comments;
                existing.UpdatedBy      = interviewerEmpId;
                existing.UpdatedAt      = DateTime.Now;

                if (isSubmit && !existing.IsSubmitted)
                {
                    existing.IsSubmitted = true;
                    existing.SubmittedBy = interviewerEmpId;
                    existing.SubmittedAt = DateTime.Now;
                }

                var existingScores = existing.Scores.ToList();
                foreach (var cs in model.CriteriaScores)
                {
                    var score = existingScores.FirstOrDefault(s => s.EvaluationCriteriaFk == cs.CriteriaId);
                    if (score != null)
                        score.Score = cs.Score;
                    else
                        await _repository.AddScoreAsync(new HrEvaluationScore
                        {
                            InterviewFeedbackFk = existing.Id,
                            EvaluationCriteriaFk = cs.CriteriaId,
                            Score = cs.Score
                        });
                }
                await _repository.SaveChangesAsync();

                if (isSubmit && schedule != null)
                {
                    var roundName = schedule.InterviewRound?.RoundName ?? "Interview";
                    _context.HrApplicationStageHistories.Add(new HrApplicationStageHistory
                    {
                        JobApplicationFk = schedule.JobApplicationId,
                        FromStageId      = null,
                        ToStageFk        = null,
                        ChangedBy        = interviewerEmpId,
                        Comments         = $"Feedback submitted — {roundName} | Recommendation: {existing.Recommendation}",
                        ChangedDate      = DateTime.Now
                    });
                    await _context.SaveChangesAsync();
                    await TryAdvanceToFinalReviewAsync(schedule.JobApplicationId, interviewerEmpId);
                }
                return;
            }

            var feedback = new HrInterviewFeedback
            {
                InterviewScheduleFk   = model.ScheduleId,
                InterviewerEmployeeFk = interviewerEmpId,
                Recommendation        = model.Recommendation,
                OverallScore          = model.OverallScore,
                Strengths             = model.Strengths,
                Concerns              = model.Concerns,
                Comments              = model.Comments,
                SubmittedDate         = DateTime.Now,
                IsSubmitted           = isSubmit,
                SubmittedBy           = isSubmit ? interviewerEmpId : (decimal?)null,
                SubmittedAt           = isSubmit ? DateTime.Now : (DateTime?)null
            };

            await _repository.AddFeedbackAsync(feedback);
            await _repository.SaveChangesAsync();

            foreach (var cs in model.CriteriaScores)
            {
                await _repository.AddScoreAsync(new HrEvaluationScore
                {
                    InterviewFeedbackFk = feedback.Id,
                    EvaluationCriteriaFk = cs.CriteriaId,
                    Score = cs.Score
                });
            }
            await _repository.SaveChangesAsync();

            if (isSubmit && schedule != null)
            {
                var roundName = schedule.InterviewRound?.RoundName ?? "Interview";
                _context.HrApplicationStageHistories.Add(new HrApplicationStageHistory
                {
                    JobApplicationFk = schedule.JobApplicationId,
                    FromStageId      = null,
                    ToStageFk        = null,
                    ChangedBy        = interviewerEmpId,
                    Comments         = $"Feedback submitted — {roundName} | Recommendation: {model.Recommendation}",
                    ChangedDate      = DateTime.Now
                });
                await _context.SaveChangesAsync();
                await TryAdvanceToFinalReviewAsync(schedule.JobApplicationId, interviewerEmpId);
            }
        }

        private async Task TryAdvanceToFinalReviewAsync(decimal applicationId, decimal changedBy)
        {
            // Use AsNoTracking so we read fresh DB state, bypassing any identity-map state from this request
            var allSchedules = await _context.HrInterviewSchedules
                .AsNoTracking()
                .Include(s => s.Feedback)
                .Where(s => s.JobApplicationId == applicationId)
                .ToListAsync();

            if (!allSchedules.Any() || !allSchedules.All(s => s.Feedback.Any(f => f.IsSubmitted)))
                return;

            var app = await _context.HrJobApplications
                .Include(a => a.CurrentStage)
                .FirstOrDefaultAsync(a => a.Id == applicationId);

            var finalReviewStage = await _context.HrApplicationStages
                .FirstOrDefaultAsync(s => s.StageName == "Final Review" && s.IsActive);

            if (app == null || finalReviewStage == null) return;
            if ((app.CurrentStage?.StageOrder ?? 0) >= (finalReviewStage.StageOrder ?? 5)) return;

            var fromId = app.CurrentStageFK;
            app.CurrentStageFK = finalReviewStage.Id;
            _context.HrJobApplications.Update(app);
            _context.HrApplicationStageHistories.Add(new HrApplicationStageHistory
            {
                JobApplicationFk = applicationId,
                FromStageId = fromId,
                ToStageFk = finalReviewStage.Id,
                ChangedBy = changedBy,
                Comments = "Moved to Final Review — all interview feedback submitted.",
                ChangedDate = DateTime.Now
            });
            await _context.SaveChangesAsync();
        }

        public async Task<HrFeedbackDetailsViewModel?> GetFeedbackDetailsAsync(decimal feedbackId)
        {
            var feedback = await _repository.GetFeedbackByIdAsync(feedbackId);
            if (feedback == null) return null;

            var empId = (int)feedback.InterviewerEmployeeFk;
            var emp = await _context.emp.FirstOrDefaultAsync(e => e.Id == empId);

            var totalScore = feedback.Scores.Sum(s => s.Score);
            var maxTotal = feedback.Scores
                .Where(s => s.EvaluationCriteria != null)
                .Sum(s => s.EvaluationCriteria!.MaxScore);

            return new HrFeedbackDetailsViewModel
            {
                Id = feedback.Id,
                ScheduleId = feedback.InterviewScheduleFk,
                Recommendation = feedback.Recommendation,
                OverallScore = feedback.OverallScore,
                Strengths = feedback.Strengths,
                Concerns = feedback.Concerns,
                Comments = feedback.Comments,
                SubmittedDate = feedback.SubmittedDate,
                InterviewerName = emp != null ? $"{emp.Code} - {emp.FirstName} {emp.LastName}" : null,
                TotalScore = feedback.Scores.Any() ? totalScore : null,
                MaxTotalScore = feedback.Scores.Any() ? maxTotal : null,
                CriteriaScores = feedback.Scores.Select(s => new HrCriteriaScoreInputViewModel
                {
                    CriteriaId = s.EvaluationCriteriaFk,
                    CriteriaName = s.EvaluationCriteria?.CriteriaName,
                    MaxScore = s.EvaluationCriteria?.MaxScore ?? 0,
                    Score = s.Score
                }).ToList()
            };
        }
    }
}
