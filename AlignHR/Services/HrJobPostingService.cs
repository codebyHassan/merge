using AlignHR.Data;
using AlignHR.Models;
using AlignHR.Repositories;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace AlignHR.Services
{
    public class HrJobPostingService : IHrJobPostingService
    {
        private readonly IHrJobPostingRepository _repository;
        private readonly ApplicationDbContext _context;

        public HrJobPostingService(IHrJobPostingRepository repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<IPagedList<HrJobPostingListItemViewModel>> GetPagedAsync(decimal? requisitionFk, string? search, int pageNumber, int pageSize, decimal? assignedToEmployeeId = null)
        {
            var query = _repository.Query();

            if (requisitionFk.HasValue)
                query = query.Where(p => p.RequisitionFK == requisitionFk.Value);

            if (assignedToEmployeeId.HasValue)
            {
                var myReqIds = await _context.HrRequisitionAssignments
                    .Where(a => a.RecruiterEmployeeFK == assignedToEmployeeId.Value && a.RequisitionFk.HasValue)
                    .Select(a => a.RequisitionFk!.Value)
                    .Distinct()
                    .ToListAsync();

                query = query.Where(p => p.RequisitionFK.HasValue && myReqIds.Contains(p.RequisitionFK.Value));
            }

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(p =>
                    (p.JobCode != null && p.JobCode.Contains(search)) ||
                    (p.JobTitle != null && p.JobTitle.Contains(search)));

            var postings = await query.OrderByDescending(p => p.CreatedOn).ToListAsync();

            var requisitionIds = postings.Where(p => p.RequisitionFK.HasValue)
                .Select(p => p.RequisitionFK!.Value).Distinct().ToList();

            var requisitionNos = await _context.HrRequisitions
                .Where(r => requisitionIds.Contains(r.Id))
                .ToDictionaryAsync(r => r.Id, r => r.RequisitionNo ?? "");

            var assignments = await _context.HrRequisitionAssignments
                .Where(a => a.RequisitionFk.HasValue && requisitionIds.Contains(a.RequisitionFk.Value) && a.RecruiterEmployeeFK.HasValue)
                .ToListAsync();

            var recruiterIds = assignments.Select(a => (int)a.RecruiterEmployeeFK!.Value).Distinct().ToList();
            var recruiterNames = await _context.emp
                .Where(e => recruiterIds.Contains(e.Id))
                .ToDictionaryAsync(e => e.Id, e => $"{e.FirstName} {e.LastName}".Trim());

            var reqToRecruiter = assignments
                .GroupBy(a => a.RequisitionFk!.Value)
                .ToDictionary(g => g.Key, g => g.First().RecruiterEmployeeFK!.Value);

            var postingIds = postings.Select(p => p.Id).ToList();

            var applicantCounts = (await _context.HrJobApplications
                .Where(a => a.JobPostingFK.HasValue && postingIds.Contains(a.JobPostingFK.Value) && a.IsActive)
                .Select(a => new { a.JobPostingFK })
                .ToListAsync())
                .GroupBy(a => a.JobPostingFK!.Value)
                .ToDictionary(g => g.Key, g => g.Count());

            var roundCountByPosting = (await _context.HrInterviewRounds
                .Where(r => postingIds.Contains(r.JobPostingId) && r.IsActive)
                .Select(r => new { r.JobPostingId })
                .ToListAsync())
                .GroupBy(x => x.JobPostingId)
                .ToDictionary(g => g.Key, g => g.Count());

            var items = postings.Select(p =>
            {
                string? recruiterName = null;
                bool assignedToMe = false;
                if (p.RequisitionFK.HasValue && reqToRecruiter.TryGetValue(p.RequisitionFK.Value, out var recId))
                {
                    recruiterNames.TryGetValue((int)recId, out recruiterName);
                    assignedToMe = assignedToEmployeeId.HasValue && recId == assignedToEmployeeId.Value;
                }

                return new HrJobPostingListItemViewModel
                {
                    Id = p.Id,
                    RequisitionFK = p.RequisitionFK,
                    RequisitionNo = p.RequisitionFK.HasValue && requisitionNos.TryGetValue(p.RequisitionFK.Value, out var rno) ? rno : null,
                    JobCode = p.JobCode,
                    JobTitle = p.JobTitle,
                    PostingStatus = p.PostingStatus,
                    OpenDate = p.OpenDate,
                    CloseDate = p.CloseDate,
                    CreatedOn = p.CreatedOn,
                    AssignedRecruiterName = recruiterName,
                    AssignedToMe = assignedToMe,
                    ApplicantCount = applicantCounts.TryGetValue(p.Id, out var ac) ? ac : 0,
                    RoundCount = roundCountByPosting.TryGetValue(p.Id, out var rc) ? rc : 0
                };
            });

            return items.ToPagedList(pageNumber, pageSize);
        }

        public async Task<HrJobPostingFormViewModel?> GetForCreateAsync(decimal requisitionFk)
        {
            var requisition = await _context.HrRequisitions.FirstOrDefaultAsync(r => r.Id == requisitionFk);
            if (requisition == null) return null;

            if (requisition.Status != "Approved")
                throw new InvalidOperationException("Job postings can only be created for approved requisitions.");

            return new HrJobPostingFormViewModel
            {
                RequisitionFK = requisitionFk,
                RequisitionNo = requisition.RequisitionNo,
                JobTitle = requisition.PositionTitle,
                OpenDate = DateTime.Today,
                PostingStatus = "Draft"
            };
        }

        public async Task<HrJobPostingFormViewModel?> GetForEditAsync(decimal id)
        {
            var posting = await _repository.GetByIdAsync(id);
            if (posting == null) return null;

            var requisition = posting.RequisitionFK.HasValue
                ? await _context.HrRequisitions.FirstOrDefaultAsync(r => r.Id == posting.RequisitionFK.Value)
                : null;

            var rounds = await _context.HrInterviewRounds
                .Where(r => r.JobPostingId == id && r.IsActive)
                .OrderBy(r => r.RoundOrder)
                .Select(r => new HrJobPostingRoundDraft
                {
                    Id = r.Id,
                    RoundName = r.RoundName,
                    RoundOrder = r.RoundOrder,
                    IsMandatory = r.IsMandatory,
                    Criteria = r.Criteria.OrderBy(c => c.Id).Select(c => new HrJobPostingCriteriaDraft
                    {
                        Id = c.Id,
                        CriteriaName = c.CriteriaName,
                        MaxScore = c.MaxScore
                    }).ToList()
                }).ToListAsync();

            return new HrJobPostingFormViewModel
            {
                Id = posting.Id,
                RequisitionFK = posting.RequisitionFK,
                RequisitionNo = requisition?.RequisitionNo,
                JobCode = posting.JobCode,
                JobTitle = posting.JobTitle,
                JobDescription = posting.JobDescription,
                EmploymentType = posting.EmploymentType,
                Location = posting.Location,
                PostingStatus = posting.PostingStatus,
                OpenDate = posting.OpenDate,
                CloseDate = posting.CloseDate,
                Rounds = rounds
            };
        }

        public async Task<HrJobPostingDetailsViewModel?> GetDetailsAsync(decimal id)
        {
            var posting = await _repository.GetByIdAsync(id);
            if (posting == null) return null;

            var requisition = posting.RequisitionFK.HasValue
                ? await _context.HrRequisitions.FirstOrDefaultAsync(r => r.Id == posting.RequisitionFK.Value)
                : null;

            string? departmentName = null;
            if (requisition?.DepartmentFK.HasValue == true)
            {
                var dept = await _context.department.FirstOrDefaultAsync(d => d.Id == (int)requisition.DepartmentFK.Value);
                departmentName = dept?.Name;
            }

            var skills = new List<HrRequisitionSkillViewModel>();
            var offerings = new List<HrRequisitionOfferingViewModel>();

            if (posting.RequisitionFK.HasValue)
            {
                skills = await _context.HrRequisitionSkills
                    .Where(s => s.RequisitionId == posting.RequisitionFK.Value)
                    .OrderBy(s => s.Id)
                    .Select(s => new HrRequisitionSkillViewModel
                    {
                        Id = s.Id,
                        SkillName = s.SkillName,
                        YearsExperience = s.YearsExperience,
                        IsMandatory = s.IsMandatory
                    }).ToListAsync();

                offerings = await _context.HrRequisitionOfferings
                    .Where(o => o.RequisitionId == posting.RequisitionFK.Value)
                    .OrderBy(o => o.Id)
                    .Select(o => new HrRequisitionOfferingViewModel
                    {
                        Id = o.Id,
                        OfferingName = o.OfferingName,
                        Description = o.Description
                    }).ToListAsync();
            }

            var assignments = new List<HrJobPostingAssignmentViewModel>();
            if (posting.RequisitionFK.HasValue)
            {
                var rawAssignments = await _context.HrRequisitionAssignments
                    .Where(a => a.RequisitionFk == posting.RequisitionFK.Value)
                    .ToListAsync();

                var recruiterIds = rawAssignments
                    .Where(a => a.RecruiterEmployeeFK.HasValue)
                    .Select(a => (int)a.RecruiterEmployeeFK!.Value)
                    .Distinct().ToList();

                var recruiters = await _context.emp
                    .Where(e => recruiterIds.Contains(e.Id))
                    .ToDictionaryAsync(e => e.Id, e => new { e.Code, Name = $"{e.FirstName} {e.LastName}".Trim() });

                assignments = rawAssignments.Select(a =>
                {
                    var emp = a.RecruiterEmployeeFK.HasValue && recruiters.TryGetValue((int)a.RecruiterEmployeeFK.Value, out var r) ? r : null;
                    return new HrJobPostingAssignmentViewModel
                    {
                        RecruiterName = emp?.Name,
                        RecruiterCode = emp?.Code,
                        AssignedDate = a.AssignedDate,
                        Notes = a.Notes
                    };
                }).ToList();
            }

            return new HrJobPostingDetailsViewModel
            {
                Id = posting.Id,
                RequisitionFK = posting.RequisitionFK,
                RequisitionNo = requisition?.RequisitionNo,
                PositionTitle = requisition?.PositionTitle,
                JobCode = posting.JobCode,
                JobTitle = posting.JobTitle,
                JobDescription = posting.JobDescription,
                EmploymentType = posting.EmploymentType,
                Location = posting.Location,
                PostingStatus = posting.PostingStatus,
                OpenDate = posting.OpenDate,
                CloseDate = posting.CloseDate,
                DepartmentName = departmentName,
                RequisitionType = requisition?.RequisitionType,
                Nature = requisition?.Nature,
                BudgetAmountPerMonth = requisition?.BudgetAmountPerMonth,
                InitialDate = requisition?.InitialDate,
                PromisedDate = requisition?.PromisedDate,
                Reason = requisition?.Reason,
                CreatedBy = posting.CreatedBy,
                CreatedOn = posting.CreatedOn,
                UpdatedBy = posting.UpdatedBy,
                UpdatedOn = posting.UpdatedOn,
                Skills = skills,
                Offerings = offerings,
                Assignments = assignments
            };
        }

        public async Task<decimal> CreateAsync(HrJobPostingFormViewModel model, string? username)
        {
            var posting = new HrJobPosting
            {
                RequisitionFK = model.RequisitionFK,
                JobCode = await GenerateJobCodeAsync(),
                JobTitle = model.JobTitle,
                JobDescription = model.JobDescription,
                EmploymentType = model.EmploymentType,
                Location = model.Location,
                PostingStatus = "Draft",
                OpenDate = model.OpenDate,
                CloseDate = model.CloseDate,
                CreatedBy = username,
                CreatedOn = DateTime.Now
            };

            await _repository.AddAsync(posting);
            await _repository.SaveChangesAsync();

            int roundOrder = 1;
            foreach (var r in model.Rounds.Where(r => !string.IsNullOrWhiteSpace(r.RoundName)))
            {
                var round = new HrInterviewRound
                {
                    JobPostingId = posting.Id,
                    RoundName = r.RoundName.Trim(),
                    RoundOrder = r.RoundOrder > 0 ? r.RoundOrder : roundOrder++,
                    IsMandatory = r.IsMandatory,
                    IsActive = true
                };

                foreach (var c in r.Criteria.Where(c => !string.IsNullOrWhiteSpace(c.CriteriaName)))
                {
                    round.Criteria.Add(new HrEvaluationCriteria
                    {
                        CriteriaName = c.CriteriaName.Trim(),
                        MaxScore = c.MaxScore > 0 ? c.MaxScore : 10
                    });
                }

                _context.HrInterviewRounds.Add(round);
            }

            await _context.SaveChangesAsync(); // single transaction for all rounds + criteria
            return posting.Id;
        }

        public async Task UpdateAsync(decimal id, HrJobPostingFormViewModel model, string? username)
        {
            var posting = await _repository.GetByIdAsync(id);
            if (posting == null)
                throw new InvalidOperationException("Job posting not found.");

            if (posting.PostingStatus == "Closed")
                throw new InvalidOperationException("Closed job postings cannot be edited.");

            posting.JobTitle = model.JobTitle;
            posting.JobDescription = model.JobDescription;
            posting.EmploymentType = model.EmploymentType;
            posting.Location = model.Location;
            posting.OpenDate = model.OpenDate;
            posting.CloseDate = model.CloseDate;
            posting.UpdatedBy = username;
            posting.UpdatedOn = DateTime.Now;

            _repository.Update(posting);

            // ── Rounds: update existing, soft-delete removed, add new ──
            var submittedRoundIds = model.Rounds
                .Where(r => r.Id > 0)
                .Select(r => r.Id)
                .ToHashSet();

            var existingRounds = await _context.HrInterviewRounds
                .Where(r => r.JobPostingId == id)
                .ToListAsync();

            // collect all existing round IDs to batch-load criteria + score checks
            var existingRoundIds = existingRounds.Select(r => r.Id).ToList();

            var allExistingCriteria = await _context.HrEvaluationCriterias
                .Where(c => existingRoundIds.Contains(c.InterviewRoundId))
                .ToListAsync();

            var criteriaToRemove = new List<HrEvaluationCriteria>();

            foreach (var existing in existingRounds)
            {
                if (submittedRoundIds.Contains(existing.Id))
                {
                    var draft = model.Rounds.First(r => r.Id == existing.Id);
                    existing.RoundName = draft.RoundName.Trim();
                    existing.RoundOrder = draft.RoundOrder;
                    existing.IsMandatory = draft.IsMandatory;
                    existing.IsActive = true;

                    var submittedCriteriaIds = draft.Criteria
                        .Where(c => c.Id > 0).Select(c => c.Id).ToHashSet();

                    var roundCriteria = allExistingCriteria.Where(c => c.InterviewRoundId == existing.Id).ToList();

                    foreach (var ec in roundCriteria)
                    {
                        if (submittedCriteriaIds.Contains(ec.Id))
                        {
                            var dc = draft.Criteria.First(c => c.Id == ec.Id);
                            ec.CriteriaName = dc.CriteriaName.Trim();
                            ec.MaxScore = dc.MaxScore > 0 ? dc.MaxScore : 10;
                        }
                        else
                        {
                            criteriaToRemove.Add(ec);
                        }
                    }

                    foreach (var nc in draft.Criteria.Where(c => c.Id == 0 && !string.IsNullOrWhiteSpace(c.CriteriaName)))
                    {
                        _context.HrEvaluationCriterias.Add(new HrEvaluationCriteria
                        {
                            InterviewRoundId = existing.Id,
                            CriteriaName = nc.CriteriaName.Trim(),
                            MaxScore = nc.MaxScore > 0 ? nc.MaxScore : 10
                        });
                    }
                }
                else
                {
                    existing.IsActive = false;
                }
            }

            // batch check scores for all criteria being removed — one query instead of N
            if (criteriaToRemove.Any())
            {
                var removeIds = criteriaToRemove.Select(c => c.Id).ToList();
                var scoredIds = await _context.HrEvaluationScores
                    .Where(s => removeIds.Contains(s.EvaluationCriteriaFk))
                    .Select(s => s.EvaluationCriteriaFk)
                    .Distinct()
                    .ToListAsync();

                foreach (var ec in criteriaToRemove.Where(c => !scoredIds.Contains(c.Id)))
                    _context.HrEvaluationCriterias.Remove(ec);
            }

            // ── New rounds (Id == 0) — use navigation property, single SaveChanges ──
            int nextOrder = existingRounds.Count + 1;

            foreach (var r in model.Rounds.Where(r => r.Id == 0 && !string.IsNullOrWhiteSpace(r.RoundName)))
            {
                var newRound = new HrInterviewRound
                {
                    JobPostingId = id,
                    RoundName = r.RoundName.Trim(),
                    RoundOrder = r.RoundOrder > 0 ? r.RoundOrder : nextOrder++,
                    IsMandatory = r.IsMandatory,
                    IsActive = true
                };

                foreach (var c in r.Criteria.Where(c => !string.IsNullOrWhiteSpace(c.CriteriaName)))
                {
                    newRound.Criteria.Add(new HrEvaluationCriteria
                    {
                        CriteriaName = c.CriteriaName.Trim(),
                        MaxScore = c.MaxScore > 0 ? c.MaxScore : 10
                    });
                }

                _context.HrInterviewRounds.Add(newRound);
            }

            await _repository.SaveChangesAsync(); // single transaction for everything
        }

        public async Task ChangeStatusAsync(decimal id, string newStatus, string? username)
        {
            var posting = await _repository.GetByIdAsync(id);
            if (posting == null)
                throw new InvalidOperationException("Job posting not found.");

            var validTransitions = new Dictionary<string, List<string>>
            {
                ["Draft"]     = new() { "Published" },
                ["Published"] = new() { "Paused", "Closed" },
                ["Paused"]    = new() { "Published", "Closed" }
            };

            if (!validTransitions.TryGetValue(posting.PostingStatus, out var allowed) || !allowed.Contains(newStatus))
                throw new InvalidOperationException($"Cannot transition from '{posting.PostingStatus}' to '{newStatus}'.");

            if (newStatus == "Published")
            {
                var hasRounds = await _context.HrInterviewRounds
                    .AnyAsync(r => r.JobPostingId == posting.Id && r.IsActive);
                if (!hasRounds)
                    throw new InvalidOperationException("Cannot publish: interview rounds are not configured. Please complete the interview setup first.");

                var roundIds = await _context.HrInterviewRounds
                    .Where(r => r.JobPostingId == posting.Id && r.IsActive)
                    .Select(r => r.Id)
                    .ToListAsync();

                var hasCriteria = await _context.HrEvaluationCriterias
                    .AnyAsync(c => roundIds.Contains(c.InterviewRoundId));
                if (!hasCriteria)
                    throw new InvalidOperationException("Cannot publish: evaluation criteria are not configured. Please add criteria to at least one interview round.");
            }

            posting.PostingStatus = newStatus;
            posting.UpdatedBy = username;
            posting.UpdatedOn = DateTime.Now;
            _repository.Update(posting);
            await _repository.SaveChangesAsync();
        }

        public async Task DeleteAsync(decimal id, string? username)
        {
            var posting = await _repository.GetByIdAsync(id);
            if (posting == null)
                throw new InvalidOperationException("Job posting not found.");

            if (posting.PostingStatus == "Published")
                throw new InvalidOperationException("Published job postings cannot be deleted. Close them first.");

            posting.IsDeleted = true;
            posting.UpdatedBy = username;
            posting.UpdatedOn = DateTime.Now;
            _repository.Update(posting);
            await _repository.SaveChangesAsync();
        }

        public async Task<List<HrPendingPostingRequisitionViewModel>> GetPendingRequisitionsAsync(decimal employeeId)
        {
            var assignments = await _context.HrRequisitionAssignments
                .Where(a => a.RecruiterEmployeeFK == employeeId && a.RequisitionFk.HasValue)
                .Select(a => new { a.RequisitionFk, a.AssignedDate })
                .ToListAsync();

            var myReqIds = assignments.Select(a => a.RequisitionFk!.Value).Distinct().ToList();
            if (!myReqIds.Any()) return new();

            var alreadyPostedReqIds = await _context.HrJobPostings
                .Where(p => p.RequisitionFK.HasValue && myReqIds.Contains(p.RequisitionFK.Value) && !p.IsDeleted)
                .Select(p => p.RequisitionFK!.Value)
                .Distinct()
                .ToListAsync();

            var pendingIds = myReqIds.Except(alreadyPostedReqIds).ToList();
            if (!pendingIds.Any()) return new();

            var requisitions = await _context.HrRequisitions
                .Where(r => pendingIds.Contains(r.Id) && r.Status == "Approved")
                .ToListAsync();

            if (!requisitions.Any()) return new();

            var deptIds = requisitions.Where(r => r.DepartmentFK.HasValue)
                .Select(r => (int)r.DepartmentFK!.Value).Distinct().ToList();
            var depts = await _context.department
                .Where(d => deptIds.Contains(d.Id))
                .ToDictionaryAsync(d => d.Id, d => d.Name);

            var assignedDateByReq = assignments
                .GroupBy(a => a.RequisitionFk!.Value)
                .ToDictionary(g => g.Key, g => g.Min(a => a.AssignedDate));

            return requisitions.Select(r => new HrPendingPostingRequisitionViewModel
            {
                RequisitionId = r.Id,
                RequisitionNo = r.RequisitionNo,
                PositionTitle = r.PositionTitle,
                DepartmentName = r.DepartmentFK.HasValue && depts.TryGetValue((int)r.DepartmentFK.Value, out var dn) ? dn : null,
                AssignedDate = assignedDateByReq.TryGetValue(r.Id, out var ad) ? ad : null
            }).ToList();
        }

        private async Task<string> GenerateJobCodeAsync()
        {
            var year = DateTime.Now.Year;
            var sequence = await _repository.CountForYearAsync(year) + 1;
            var jobCode = $"JP-{year}-{sequence:D4}";
            while (await _repository.JobCodeExistsAsync(jobCode))
            {
                sequence++;
                jobCode = $"JP-{year}-{sequence:D4}";
            }
            return jobCode;
        }
    }
}
