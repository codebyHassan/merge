using AlignHR.Data;
using AlignHR.Models;
using AlignHR.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Security.Cryptography;
using X.PagedList;

namespace AlignHR.Services
{
    public class HrOfferService : IHrOfferService
    {
        private readonly IHrOfferRepository _repository;
        private readonly IFileStorageService _fileStorage;
        private readonly ApplicationDbContext _context;
        private readonly IHostEnvironment _env;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HrOfferService(
            IHrOfferRepository repository,
            IFileStorageService fileStorage,
            ApplicationDbContext context,
            IHostEnvironment env,
            IHttpContextAccessor httpContextAccessor)
        {
            _repository = repository;
            _fileStorage = fileStorage;
            _context = context;
            _env = env;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IPagedList<HrOfferListItemViewModel>> GetPagedAsync(string? search, int pageNumber, int pageSize)
        {
            var offers = await _repository.Query()
                .OrderByDescending(o => o.CreatedOn)
                .ToListAsync();

            var appIds = offers.Where(o => o.JobApplicationFk.HasValue)
                .Select(o => o.JobApplicationFk!.Value).Distinct().ToList();

            var applications = await _context.HrJobApplications
                .Include(a => a.Candidate)
                .Include(a => a.JobPosting)
                .Where(a => appIds.Contains(a.Id))
                .ToDictionaryAsync(a => a.Id);

            var versionCounts = await _context.HrOfferVersions
                .Where(v => v.OfferFk.HasValue && offers.Select(o => o.Id).Contains(v.OfferFk!.Value))
                .GroupBy(v => v.OfferFk!.Value)
                .Select(g => new { g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Key, x => x.Count);

            var items = offers.Where(o =>
                string.IsNullOrWhiteSpace(search) ||
                (o.OfferNumber != null && o.OfferNumber.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                (o.JobApplicationFk.HasValue && applications.TryGetValue(o.JobApplicationFk.Value, out var app) &&
                 (app.Candidate?.FirstName + " " + app.Candidate?.LastName).Contains(search, StringComparison.OrdinalIgnoreCase)))
            .Select(o =>
            {
                applications.TryGetValue(o.JobApplicationFk ?? 0, out var app);
                return new HrOfferListItemViewModel
                {
                    Id = o.Id,
                    OfferNumber = o.OfferNumber,
                    CandidateName = app?.Candidate != null
                        ? $"{app.Candidate.FirstName} {app.Candidate.LastName}".Trim()
                        : null,
                    JobTitle = app?.JobPosting?.JobTitle,
                    ProposedSalary = o.ProposedSalary,
                    ProposedJoiningDate = o.ProposedJoiningDate,
                    ExpiryDate = o.ExpiryDate,
                    Status = o.Status,
                    VersionCount = versionCounts.TryGetValue(o.Id, out var cnt) ? cnt : 0,
                    CreatedOn = o.CreatedOn
                };
            });

            return items.ToPagedList(pageNumber, pageSize);
        }

        public async Task<HrOfferFormViewModel?> GetForCreateAsync(decimal applicationFk)
        {
            var app = await _context.HrJobApplications
                .Include(a => a.Candidate)
                .Include(a => a.JobPosting)
                .FirstOrDefaultAsync(a => a.Id == applicationFk);
            if (app == null) return null;

            if (await _repository.GetByApplicationAsync(applicationFk) != null)
                throw new InvalidOperationException("An offer already exists for this application.");

            return new HrOfferFormViewModel
            {
                ApplicationFk = applicationFk,
                CandidateName = app.Candidate != null
                    ? $"{app.Candidate.FirstName} {app.Candidate.LastName}".Trim()
                    : null,
                JobTitle = app.JobPosting?.JobTitle,
                ProposedJoiningDate = DateTime.Today.AddMonths(1),
                ExpiryDate = DateTime.Today.AddDays(14)
            };
        }

        public async Task<HrOfferFormViewModel?> GetForEditAsync(decimal offerId)
        {
            var offer = await _repository.GetByIdAsync(offerId);
            if (offer == null) return null;
            if (offer.Status != "Draft")
                throw new InvalidOperationException("Only Draft offers can be edited.");

            var app = offer.JobApplicationFk.HasValue
                ? await _context.HrJobApplications
                    .Include(a => a.Candidate)
                    .Include(a => a.JobPosting)
                    .FirstOrDefaultAsync(a => a.Id == offer.JobApplicationFk.Value)
                : null;

            return new HrOfferFormViewModel
            {
                Id = offer.Id,
                ApplicationFk = offer.JobApplicationFk ?? 0,
                OfferNumber = offer.OfferNumber,
                CandidateName = app?.Candidate != null
                    ? $"{app.Candidate.FirstName} {app.Candidate.LastName}".Trim()
                    : null,
                JobTitle = app?.JobPosting?.JobTitle,
                ProposedSalary = offer.ProposedSalary,
                ProposedJoiningDate = offer.ProposedJoiningDate,
                ExpiryDate = offer.ExpiryDate,
                Notes = offer.Notes
            };
        }

        public async Task UpdateAsync(HrOfferFormViewModel model, string? username, decimal? empId)
        {
            var offer = await _repository.GetByIdAsync(model.Id);
            if (offer == null)
                throw new InvalidOperationException("Offer not found.");
            if (offer.Status != "Draft")
                throw new InvalidOperationException("Only Draft offers can be edited.");

            offer.ProposedSalary = model.ProposedSalary;
            offer.ProposedJoiningDate = model.ProposedJoiningDate;
            offer.ExpiryDate = model.ExpiryDate;
            offer.Notes = string.IsNullOrWhiteSpace(model.Notes) ? null : model.Notes;
            offer.UpdatedBy = username;
            offer.UpdatedOn = DateTime.Now;
            _repository.UpdateOffer(offer);
            await _repository.SaveChangesAsync();
        }

        public async Task<HrOfferDetailsViewModel?> GetDetailsAsync(decimal offerId)
        {
            var offer = await _repository.GetWithDetailsAsync(offerId);
            if (offer == null) return null;

            var candidate = offer.JobApplication?.Candidate;
            var posting = offer.JobApplication?.JobPosting;

            var empIds = offer.Approvals
                .Where(a => a.ApproverEmployeeFk.HasValue)
                .Select(a => (int)a.ApproverEmployeeFk!.Value)
                .Union(offer.Versions
                    .Where(v => v.CreatedBy.HasValue)
                    .Select(v => (int)v.CreatedBy!.Value))
                .Distinct().ToList();

            var employees = await _context.emp
                .Where(e => empIds.Contains(e.Id))
                .ToDictionaryAsync(e => e.Id, e => $"{e.Code} - {e.FirstName} {e.LastName}");

            var hiringDecision = offer.JobApplicationFk.HasValue
                ? await _repository.GetHiringDecisionAsync(offer.JobApplicationFk.Value)
                : null;

            decimal? decisionEmpId = hiringDecision?.DecisionBy;
            string? decisionByName = null;
            if (decisionEmpId.HasValue)
                employees.TryGetValue((int)decisionEmpId.Value, out decisionByName);

            return new HrOfferDetailsViewModel
            {
                Id = offer.Id,
                ApplicationFk = offer.JobApplicationFk,
                OfferNumber = offer.OfferNumber,
                CandidateName = candidate != null
                    ? $"{candidate.FirstName} {candidate.LastName}".Trim()
                    : null,
                CandidateEmail = candidate?.Email,
                JobCode = posting?.JobCode,
                JobTitle = posting?.JobTitle,
                ProposedSalary = offer.ProposedSalary,
                ProposedJoiningDate = offer.ProposedJoiningDate,
                ExpiryDate = offer.ExpiryDate,
                Status = offer.Status,
                CreatedBy = offer.CreatedBy,
                CreatedOn = offer.CreatedOn,
                UpdatedBy = offer.UpdatedBy,
                UpdatedOn = offer.UpdatedOn,
                Notes = offer.Notes,
                HiringDecision = hiringDecision == null ? null : new HrHiringDecisionItemViewModel
                {
                    Id = hiringDecision.Id,
                    Decision = hiringDecision.Decision,
                    DecisionByName = decisionByName,
                    DecisionDate = hiringDecision.DecisionDate,
                    Remarks = hiringDecision.Remarks
                },
                Versions = offer.Versions.Select(v => new HrOfferVersionItemViewModel
                {
                    Id = v.Id,
                    VersionNo = v.VersionNo,
                    Salary = v.Salary,
                    JoiningDate = v.JoiningDate,
                    Remarks = v.Remarks,
                    CreatedByName = v.CreatedBy.HasValue && employees.TryGetValue((int)v.CreatedBy.Value, out var vn) ? vn : null,
                    CreatedDate = v.CreatedDate
                }).ToList(),
                Approvals = offer.Approvals.Select(a => new HrOfferApprovalItemViewModel
                {
                    Id = a.Id,
                    ApprovalLevel = a.ApprovalLevel,
                    ApproverName = a.ApproverEmployeeFk.HasValue && employees.TryGetValue((int)a.ApproverEmployeeFk.Value, out var an) ? an : null,
                    Status = a.Status,
                    Comments = a.Comments,
                    ActionDate = a.ActionDate
                }).ToList(),
                Responses = offer.Responses.Select(r => new HrOfferResponseItemViewModel
                {
                    Id = r.Id,
                    ResponseType = r.ResponseType,
                    ResponseDate = r.ResponseDate,
                    Comments = r.Comments
                }).ToList(),
                Documents = offer.Documents.Select(d => new HrOfferDocumentItemViewModel
                {
                    Id = d.Id,
                    FileName = d.FileName,
                    FilePath = d.FilePath,
                    GeneratedDate = d.GeneratedDate
                }).ToList()
            };
        }

        public async Task<HrOfferVersionFormViewModel?> GetForRevisionAsync(decimal offerId)
        {
            var offer = await _repository.GetByIdAsync(offerId);
            if (offer == null) return null;
            return new HrOfferVersionFormViewModel
            {
                OfferId = offerId,
                OfferNumber = offer.OfferNumber,
                Salary = offer.ProposedSalary,
                JoiningDate = offer.ProposedJoiningDate
            };
        }

        public async Task<HrApprovalActionViewModel?> GetApprovalActionAsync(decimal approvalId)
        {
            var approval = await _repository.GetApprovalByIdAsync(approvalId);
            if (approval == null) return null;

            if (!approval.OfferFk.HasValue) return null;

            var offer = await _context.HrOffers
                .Include(o => o.JobApplication).ThenInclude(a => a!.Candidate)
                .Include(o => o.JobApplication).ThenInclude(a => a!.JobPosting)
                .Include(o => o.Approvals)
                .FirstOrDefaultAsync(o => o.Id == approval.OfferFk.Value);

            if (offer == null) return null;

            var candidate = offer.JobApplication?.Candidate;
            var posting   = offer.JobApplication?.JobPosting;

            return new HrApprovalActionViewModel
            {
                ApprovalId          = approvalId,
                OfferId             = offer.Id,
                OfferNumber         = offer.OfferNumber,
                CandidateName       = candidate != null ? $"{candidate.FirstName} {candidate.LastName}".Trim() : null,
                CandidateEmail      = candidate?.Email,
                JobTitle            = posting?.JobTitle,
                Salary              = offer.ProposedSalary,
                ProposedJoiningDate = offer.ProposedJoiningDate,
                ExpiryDate          = offer.ExpiryDate,
                ApprovalLevel       = approval.ApprovalLevel,
                TotalLevels         = offer.Approvals.Count,
                Notes               = offer.Notes,
                SubmittedOn         = offer.UpdatedOn ?? offer.CreatedOn
            };
        }

        public async Task<HrOfferResponseFormViewModel?> GetForResponseAsync(decimal offerId)
        {
            var offer = await _repository.GetByIdAsync(offerId);
            if (offer == null) return null;

            var app = offer.JobApplicationFk.HasValue
                ? await _context.HrJobApplications
                    .Include(a => a.Candidate)
                    .FirstOrDefaultAsync(a => a.Id == offer.JobApplicationFk.Value)
                : null;

            return new HrOfferResponseFormViewModel
            {
                OfferId = offerId,
                OfferNumber = offer.OfferNumber,
                CandidateName = app?.Candidate != null
                    ? $"{app.Candidate.FirstName} {app.Candidate.LastName}".Trim()
                    : null,
                ResponseDate = DateTime.Today
            };
        }

        public async Task<HrHiringDecisionFormViewModel?> GetForHiringDecisionAsync(decimal applicationFk)
        {
            var app = await _context.HrJobApplications
                .Include(a => a.Candidate)
                .Include(a => a.JobPosting)
                .FirstOrDefaultAsync(a => a.Id == applicationFk);
            if (app == null) return null;

            return new HrHiringDecisionFormViewModel
            {
                ApplicationFk = applicationFk,
                CandidateName = app.Candidate != null
                    ? $"{app.Candidate.FirstName} {app.Candidate.LastName}".Trim()
                    : null,
                JobTitle = app.JobPosting?.JobTitle
            };
        }

        public async Task<decimal> CreateAsync(HrOfferFormViewModel model, string? username, decimal? createdByEmpId)
        {
            var app = await _context.HrJobApplications.FirstOrDefaultAsync(a => a.Id == model.ApplicationFk);
            if (app == null)
                throw new InvalidOperationException("Application not found.");

            if (await _repository.GetByApplicationAsync(model.ApplicationFk) != null)
                throw new InvalidOperationException("An offer already exists for this application.");

            var offerNumber = await GenerateOfferNumberAsync();

            var offer = new HrOffer
            {
                JobApplicationFk = model.ApplicationFk,
                OfferNumber = offerNumber,
                ProposedSalary = model.ProposedSalary,
                ProposedJoiningDate = model.ProposedJoiningDate,
                ExpiryDate = model.ExpiryDate,
                Notes = string.IsNullOrWhiteSpace(model.Notes) ? null : model.Notes,
                Status = "Draft",
                CreatedBy = username,
                CreatedOn = DateTime.Now
            };

            await _repository.AddOfferAsync(offer);
            await _repository.SaveChangesAsync();

            await _repository.AddVersionAsync(new HrOfferVersion
            {
                OfferFk = offer.Id,
                VersionNo = 1,
                Salary = model.ProposedSalary,
                JoiningDate = model.ProposedJoiningDate,
                Remarks = model.Remarks ?? "Initial offer.",
                CreatedBy = createdByEmpId,
                CreatedDate = DateTime.Now
            });

            await AddHistoryAsync(model.ApplicationFk, createdByEmpId,
                $"Offer Created | {offerNumber} · PKR {model.ProposedSalary:N0}/mo · Expires {model.ExpiryDate?.ToString("dd MMM yyyy") ?? "—"}");

            await _repository.SaveChangesAsync();

            return offer.Id;
        }

        public async Task AddVersionAsync(HrOfferVersionFormViewModel model, decimal? empId)
        {
            var offer = await _repository.GetByIdAsync(model.OfferId);
            if (offer == null)
                throw new InvalidOperationException("Offer not found.");

            if (offer.Status is "Accepted" or "Hired")
                throw new InvalidOperationException("Cannot revise an accepted or hired offer.");

            var nextVersionNo = await _repository.GetNextVersionNoAsync(model.OfferId);

            await _repository.AddVersionAsync(new HrOfferVersion
            {
                OfferFk = model.OfferId,
                VersionNo = nextVersionNo,
                Salary = model.Salary,
                JoiningDate = model.JoiningDate,
                Remarks = model.Remarks,
                CreatedBy = empId,
                CreatedDate = DateTime.Now
            });

            offer.ProposedSalary = model.Salary;
            offer.ProposedJoiningDate = model.JoiningDate;
            offer.Status = "Draft";
            offer.UpdatedOn = DateTime.Now;
            _repository.UpdateOffer(offer);

            await _repository.SaveChangesAsync();
        }

        public async Task SubmitForApprovalAsync(decimal offerId, string? username)
        {
            var offer = await _repository.GetByIdAsync(offerId);
            if (offer == null)
                throw new InvalidOperationException("Offer not found.");

            if (offer.Status != "Draft")
                throw new InvalidOperationException("Only Draft offers can be submitted for approval.");

            var hasApprovers = await _context.HrOfferApprovals
                .AnyAsync(a => a.OfferFk == offerId);

            if (!hasApprovers)
            {
                if (string.IsNullOrEmpty(offer.ResponseToken))
                    offer.ResponseToken = Convert.ToHexString(RandomNumberGenerator.GetBytes(32)).ToLower();
                offer.Status    = "Sent";
                offer.UpdatedBy = username;
                offer.UpdatedOn = DateTime.Now;
                _repository.UpdateOffer(offer);
                await QueueOfferEmailAsync(offer);
                await _repository.SaveChangesAsync();
            }
            else
            {
                offer.Status    = "PendingApproval";
                offer.UpdatedBy = username;
                offer.UpdatedOn = DateTime.Now;
                _repository.UpdateOffer(offer);
                await _repository.SaveChangesAsync();
            }
        }

        public async Task AddApproverAsync(HrAddApproverViewModel model, string? username)
        {
            await _repository.AddApprovalAsync(new HrOfferApproval
            {
                OfferFk = model.OfferId,
                ApproverEmployeeFk = model.ApproverEmployeeId,
                ApprovalLevel = model.ApprovalLevel,
                Status = "Pending"
            });
            await _repository.SaveChangesAsync();
        }

        public async Task RemoveApproverAsync(decimal approvalId)
        {
            var approval = await _repository.GetApprovalByIdAsync(approvalId);
            if (approval == null) return;
            if (approval.Status != "Pending")
                throw new InvalidOperationException("Only pending approvers can be removed.");
            _repository.RemoveApproval(approval);
            await _repository.SaveChangesAsync();
        }

        public async Task ProcessApprovalAsync(HrApprovalActionViewModel model, decimal? empId)
        {
            var approval = await _repository.GetApprovalByIdAsync(model.ApprovalId);
            if (approval == null)
                throw new InvalidOperationException("Approval record not found.");

            approval.Status = model.Action == "Approve" ? "Approved" : "Rejected";
            approval.Comments = model.Comments;
            approval.ActionDate = DateTime.Now;
            _repository.UpdateApproval(approval);

            var offer = approval.OfferFk.HasValue
                ? await _repository.GetWithDetailsAsync(approval.OfferFk.Value)
                : null;

            if (offer != null)
            {
                if (model.Action == "Reject")
                {
                    offer.Status = "Rejected";
                }
                else
                {
                    var pendingApprovals = offer.Approvals
                        .Where(a => a.Id != model.ApprovalId && a.Status == "Pending")
                        .ToList();
                    if (!pendingApprovals.Any())
                    {
                        offer.Status = "Sent";

                        if (string.IsNullOrEmpty(offer.ResponseToken))
                            offer.ResponseToken = Convert.ToHexString(RandomNumberGenerator.GetBytes(32)).ToLower();
                        await QueueOfferEmailAsync(offer);
                    }
                }
                offer.UpdatedOn = DateTime.Now;
                _repository.UpdateOffer(offer);
            }

            await _repository.SaveChangesAsync();
        }

        public async Task SendOfferAsync(decimal offerId, string? username)
        {
            var offer = await _repository.GetByIdAsync(offerId);
            if (offer == null)
                throw new InvalidOperationException("Offer not found.");

            if (offer.Status != "Approved")
                throw new InvalidOperationException("Only Approved offers can be sent.");

            // Generate a secure single-use token (32 random bytes = 64 hex chars, 2^256 space)
            if (string.IsNullOrEmpty(offer.ResponseToken))
                offer.ResponseToken = Convert.ToHexString(RandomNumberGenerator.GetBytes(32)).ToLower();

            offer.Status = "Sent";
            offer.UpdatedBy = username;
            offer.UpdatedOn = DateTime.Now;
            _repository.UpdateOffer(offer);
            await _repository.SaveChangesAsync();

            // Build email from template and queue it
            var app = offer.JobApplicationFk.HasValue
                ? await _context.HrJobApplications
                    .Include(a => a.Candidate)
                    .Include(a => a.JobPosting)
                    .FirstOrDefaultAsync(a => a.Id == offer.JobApplicationFk.Value)
                : null;

            var candidateName  = app?.Candidate != null ? $"{app.Candidate.FirstName} {app.Candidate.LastName}".Trim() : "Candidate";
            var candidateEmail = app?.Candidate?.Email;
            var jobTitle       = app?.JobPosting?.JobTitle ?? "the position";
            var salary         = offer.ProposedSalary.ToString("N0");
            var expiry         = offer.ExpiryDate?.ToString("dd MMM yyyy") ?? "—";
            var responseLink   = $"/OfferResponse/Respond?token={offer.ResponseToken}";

            var details = $@"
<p style=""margin:0 0 6px;font-size:11px;font-weight:700;text-transform:uppercase;letter-spacing:.6px;color:#9CA3AF;"">Offer Details</p>
<table style=""width:100%;border-collapse:collapse;font-size:13.5px;"">
  <tr><td style=""color:#9CA3AF;width:110px;padding:4px 0;"">Position</td><td style=""color:#111827;font-weight:500;"">{jobTitle}</td></tr>
  <tr><td style=""color:#9CA3AF;padding:4px 0;"">Offer Ref</td><td style=""color:#111827;font-weight:500;"">{offer.OfferNumber}</td></tr>
  <tr><td style=""color:#9CA3AF;padding:4px 0;"">Salary</td><td style=""color:#111827;font-weight:500;"">PKR {salary} / month</td></tr>
  <tr><td style=""color:#9CA3AF;padding:4px 0;"">Expires On</td><td style=""color:#111827;font-weight:500;"">{expiry}</td></tr>
</table>";

            var subject  = $"Job Offer — {jobTitle} ({offer.OfferNumber})";
            var greeting = $"Congratulations, {candidateName.Split(' ')[0]}!";
            var body     = $"We are pleased to extend you an official offer for the role of <strong>{jobTitle}</strong>. Please review the details below and use the button to accept or decline by <strong>{expiry}</strong>.";
            var closing  = "We look forward to your response.<br/><br/>Best regards,<br/><strong style=\"color:#111827;\">The AlignHR Recruitment Team</strong>";

            var templatePath = Path.Combine(_env.ContentRootPath, "EmailTemplates", "base.html");
            var htmlBody = File.Exists(templatePath)
                ? (await File.ReadAllTextAsync(templatePath))
                    .Replace("{{Subject}}", subject)
                    .Replace("{{Greeting}}", greeting)
                    .Replace("{{Body}}", body)
                    .Replace("{{Details}}", details)
                    .Replace("{{CtaLink}}", responseLink)
                    .Replace("{{CtaText}}", "View &amp; Respond to Offer")
                    .Replace("{{Closing}}", closing)
                : $"<p>{greeting}</p><p>{body}</p><p><a href=\"{responseLink}\">Respond to Offer</a></p>";

            _context.TrnEmails.Add(new TrnEmail
            {
                EmlTo       = candidateEmail,
                EmlSubject  = subject,
                EmlBody     = htmlBody,
                EmlIsSent   = false,
                EmlAddTag   = "OfferLetter",
                EmlAddStamp = DateTime.Now
            });

            if (offer.JobApplicationFk.HasValue)
                await AddHistoryAsync(offer.JobApplicationFk.Value, null,
                    $"Offer Sent | {offer.OfferNumber} emailed to {candidateEmail}");

            await _context.SaveChangesAsync();
        }

        public async Task RecordResponseAsync(HrOfferResponseFormViewModel model, string? username)
        {
            var offer = await _repository.GetByIdAsync(model.OfferId);
            if (offer == null)
                throw new InvalidOperationException("Offer not found.");

            await _repository.AddResponseAsync(new HrOfferResponse
            {
                OfferFk = model.OfferId,
                ResponseType = model.ResponseType,
                ResponseDate = model.ResponseDate,
                Comments = model.Comments
            });

            offer.Status = model.ResponseType switch
            {
                "Accepted" => "Accepted",
                "Rejected" => "Rejected",
                "Expired"  => "Expired",
                _          => offer.Status
            };
            offer.UpdatedBy = username;
            offer.UpdatedOn = DateTime.Now;
            _repository.UpdateOffer(offer);

            await _repository.SaveChangesAsync();
        }

        public async Task UploadDocumentAsync(decimal offerId, IFormFile file, string? username)
        {
            var (fileName, relativePath) = await _fileStorage.SaveAsync(file, $"offers/{offerId:0}");
            await _repository.AddDocumentAsync(new HrOfferDocument
            {
                OfferFk = offerId,
                FileName = fileName,
                FilePath = relativePath,
                GeneratedDate = DateTime.Now
            });
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

        public async Task CancelOfferAsync(decimal offerId, string? username)
        {
            var offer = await _repository.GetByIdAsync(offerId);
            if (offer == null) return;
            offer.Status = "Cancelled";
            offer.UpdatedBy = username;
            offer.UpdatedOn = DateTime.Now;
            _repository.UpdateOffer(offer);
            await _repository.SaveChangesAsync();
        }

        public async Task WithdrawOfferAsync(decimal offerId, string? username)
        {
            var offer = await _repository.GetWithDetailsAsync(offerId);
            if (offer == null) return;
            if (offer.Status != "Approved" && offer.Status != "PendingApproval")
                throw new InvalidOperationException("Only Approved or PendingApproval offers can be withdrawn.");

            // When withdrawing from PendingApproval, reset any pending approvals back so history is clean
            if (offer.Status == "PendingApproval")
            {
                foreach (var ap in offer.Approvals.Where(a => a.Status == "Pending"))
                {
                    _repository.RemoveApproval(ap);
                }
            }

            offer.Status = "Withdrawn";
            offer.UpdatedBy = username;
            offer.UpdatedOn = DateTime.Now;
            _repository.UpdateOffer(offer);
            await _repository.SaveChangesAsync();
        }

        public async Task RecordHiringDecisionAsync(HrHiringDecisionFormViewModel model, decimal? empId)
        {
            var existing = await _repository.GetHiringDecisionAsync(model.ApplicationFk);

            if (existing != null)
            {
                existing.Decision = model.Decision;
                existing.DecisionBy = empId;
                existing.DecisionDate = DateTime.Now;
                existing.Remarks = model.Remarks;
                _repository.UpdateHiringDecision(existing);
            }
            else
            {
                await _repository.AddHiringDecisionAsync(new HrHiringDecision
                {
                    JobApplicationFk = model.ApplicationFk,
                    Decision = model.Decision,
                    DecisionBy = empId,
                    DecisionDate = DateTime.Now,
                    Remarks = model.Remarks
                });
            }

            // When Hired — move application stage to "Selected" if not already there
            if (model.Decision == "Hired")
            {
                var app = await _context.HrJobApplications.FirstOrDefaultAsync(a => a.Id == model.ApplicationFk);
                var selectedStage = await _context.HrApplicationStages
                    .FirstOrDefaultAsync(s => s.StageName == "Selected" && s.IsActive);
                if (app != null && selectedStage != null)
                {
                    app.CurrentStageFK = selectedStage.Id;
                    _context.HrJobApplications.Update(app);
                }
            }

            await _repository.SaveChangesAsync();
        }

        private async Task QueueOfferEmailAsync(HrOffer offer)
        {
            var app = offer.JobApplicationFk.HasValue
                ? await _context.HrJobApplications
                    .Include(a => a.Candidate)
                    .Include(a => a.JobPosting)
                    .FirstOrDefaultAsync(a => a.Id == offer.JobApplicationFk.Value)
                : null;

            var candidateEmail = app?.Candidate?.Email;
            if (string.IsNullOrWhiteSpace(candidateEmail)) return;

            var candidateName = app?.Candidate != null
                ? $"{app.Candidate.FirstName} {app.Candidate.LastName}".Trim()
                : "Candidate";
            var jobTitle = app?.JobPosting?.JobTitle ?? "the position";
            var salary   = offer.ProposedSalary.ToString("N0");
            var expiry   = offer.ExpiryDate?.ToString("dd MMM yyyy") ?? "—";
            var httpReq  = _httpContextAccessor.HttpContext?.Request;
            var baseUrl  = httpReq != null ? $"{httpReq.Scheme}://{httpReq.Host}" : "";
            var responseLink = $"{baseUrl}/OfferResponse/Respond?token={offer.ResponseToken}";

            // Collect CC: all offer approvers + line manager (via requisition), looked up by User.EmployeeId
            var ccEmpIds = new HashSet<int>();

            var approverEmpIds = await _context.HrOfferApprovals
                .Where(a => a.OfferFk == offer.Id && a.ApproverEmployeeFk.HasValue)
                .Select(a => (int)a.ApproverEmployeeFk!.Value)
                .ToListAsync();
            foreach (var id in approverEmpIds) ccEmpIds.Add(id);

            if (offer.JobApplicationFk.HasValue)
            {
                var lineManagerEmpId = await (
                    from a  in _context.HrJobApplications
                    join p  in _context.HrJobPostings  on a.JobPostingFK   equals p.Id
                    join r  in _context.HrRequisitions on p.RequisitionFK  equals r.Id
                    where a.Id == offer.JobApplicationFk.Value && r.EmployeeFK.HasValue
                    select (int)r.EmployeeFK!.Value
                ).FirstOrDefaultAsync();
                if (lineManagerEmpId != 0) ccEmpIds.Add(lineManagerEmpId);
            }

            var ccEmails = await _context.users
                .Where(u => u.EmployeeId.HasValue && ccEmpIds.Contains(u.EmployeeId!.Value)
                         && !string.IsNullOrEmpty(u.Email))
                .Select(u => u.Email!)
                .Distinct()
                .ToListAsync();
            var ccString = ccEmails.Any() ? string.Join(",", ccEmails) : null;

            var notesSection = "";
            if (!string.IsNullOrWhiteSpace(offer.Notes))
            {
                var noteLines = offer.Notes.Split('\n', StringSplitOptions.RemoveEmptyEntries)
                    .Select(n => n.TrimStart('•').Trim()).Where(n => n.Length > 0).ToList();
                if (noteLines.Any())
                {
                    var bullets = string.Join("", noteLines.Select(l =>
                        $"<li style=\"margin-bottom:4px;font-size:13px;\">{System.Net.WebUtility.HtmlEncode(l)}</li>"));
                    notesSection = $@"
<p style=""margin:16px 0 6px;font-size:11px;font-weight:700;text-transform:uppercase;letter-spacing:.6px;color:#9CA3AF;"">Offer Terms &amp; Notes</p>
<ul style=""margin:0;padding-left:18px;"">{bullets}</ul>";
                }
            }

            var details = $@"
<p style=""margin:0 0 6px;font-size:11px;font-weight:700;text-transform:uppercase;letter-spacing:.6px;color:#9CA3AF;"">Offer Details</p>
<table style=""width:100%;border-collapse:collapse;font-size:13.5px;"">
  <tr><td style=""color:#9CA3AF;width:110px;padding:4px 0;"">Position</td><td style=""color:#111827;font-weight:500;"">{jobTitle}</td></tr>
  <tr><td style=""color:#9CA3AF;padding:4px 0;"">Offer Ref</td><td style=""color:#111827;font-weight:500;"">{offer.OfferNumber}</td></tr>
  <tr><td style=""color:#9CA3AF;padding:4px 0;"">Salary</td><td style=""color:#111827;font-weight:500;"">PKR {salary} / month</td></tr>
  <tr><td style=""color:#9CA3AF;padding:4px 0;"">Expires On</td><td style=""color:#111827;font-weight:500;"">{expiry}</td></tr>
</table>{notesSection}";

            var firstName = candidateName.Split(' ')[0];
            var subject   = $"Job Offer — {jobTitle} ({offer.OfferNumber})";
            var greeting  = $"Congratulations, {firstName}!";
            var body      = $"We are pleased to extend you an official offer for the role of <strong>{jobTitle}</strong>. Please review the details below and use the button to confirm your acceptance before <strong>{expiry}</strong>.";
            var closing   = "We look forward to welcoming you.<br/><br/>Best regards,<br/><strong style=\"color:#111827;\">The AlignHR Recruitment Team</strong>";

            var templatePath = Path.Combine(_env.ContentRootPath, "EmailTemplates", "base.html");
            var htmlBody = File.Exists(templatePath)
                ? (await File.ReadAllTextAsync(templatePath))
                    .Replace("{{Subject}}", subject)
                    .Replace("{{Greeting}}", greeting)
                    .Replace("{{Body}}", body)
                    .Replace("{{Details}}", details)
                    .Replace("{{CtaLink}}", responseLink)
                    .Replace("{{CtaText}}", "View &amp; Respond to Offer")
                    .Replace("{{Closing}}", closing)
                : $"<p>{greeting}</p><p>{body}</p><p><a href=\"{responseLink}\">View &amp; Respond to Offer</a></p>";

            await _context.TrnEmails.AddAsync(new TrnEmail
            {
                EmlTo       = candidateEmail,
                EmlCc       = ccString,
                EmlSubject  = subject,
                EmlBody     = htmlBody,
                EmlIsSent   = false,
                EmlAddTag   = "OfferLetter",
                EmlAddStamp = DateTime.Now
            });
        }

        private Task AddHistoryAsync(decimal applicationFk, decimal? changedBy, string comments)
        {
            _context.HrApplicationStageHistories.Add(new HrApplicationStageHistory
            {
                JobApplicationFk = applicationFk,
                ChangedBy        = changedBy,
                Comments         = comments,
                ChangedDate      = DateTime.Now
            });
            return Task.CompletedTask;
        }

        public async Task<List<HrOfferApprovalInboxItemViewModel>> GetMyApprovalsAsync(int employeeId)
        {
            var approvals = await _context.HrOfferApprovals
                .Where(a => a.ApproverEmployeeFk.HasValue
                         && (int)a.ApproverEmployeeFk.Value == employeeId
                         && a.Status == "Pending")
                .ToListAsync();

            if (!approvals.Any()) return new();

            var offerIds = approvals.Select(a => a.OfferFk).Where(x => x.HasValue).Select(x => x!.Value).Distinct().ToList();

            var offers = await _context.HrOffers
                .Include(o => o.JobApplication).ThenInclude(a => a!.Candidate)
                .Include(o => o.JobApplication).ThenInclude(a => a!.JobPosting)
                .Include(o => o.Approvals)
                .Where(o => offerIds.Contains(o.Id) && o.Status == "PendingApproval")
                .ToDictionaryAsync(o => o.Id);

            var result = new List<HrOfferApprovalInboxItemViewModel>();
            foreach (var ap in approvals)
            {
                if (!ap.OfferFk.HasValue || !offers.TryGetValue(ap.OfferFk.Value, out var offer)) continue;
                var candidate = offer.JobApplication?.Candidate;
                var posting   = offer.JobApplication?.JobPosting;
                result.Add(new HrOfferApprovalInboxItemViewModel
                {
                    ApprovalId          = ap.Id,
                    OfferId             = offer.Id,
                    OfferNumber         = offer.OfferNumber,
                    CandidateName       = candidate != null ? $"{candidate.FirstName} {candidate.LastName}".Trim() : null,
                    CandidateEmail      = candidate?.Email,
                    JobTitle            = posting?.JobTitle,
                    ProposedSalary      = offer.ProposedSalary,
                    ProposedJoiningDate = offer.ProposedJoiningDate,
                    ExpiryDate          = offer.ExpiryDate,
                    ApprovalLevel       = ap.ApprovalLevel ?? 1,
                    TotalLevels         = offer.Approvals.Count,
                    SubmittedOn         = offer.UpdatedOn ?? offer.CreatedOn,
                    Notes               = offer.Notes,
                    Status              = ap.Status,
                    ActionDate          = ap.ActionDate,
                    Comments            = ap.Comments
                });
            }
            return result.OrderByDescending(r => r.ActionDate ?? r.SubmittedOn).ToList();
        }

        public async Task<int> GetMyApprovalCountAsync(int employeeId)
        {
            return await _context.HrOfferApprovals
                .CountAsync(a => a.ApproverEmployeeFk.HasValue
                              && (int)a.ApproverEmployeeFk.Value == employeeId
                              && a.Status == "Pending"
                              && _context.HrOffers.Any(o => o.Id == a.OfferFk && o.Status == "PendingApproval"));
        }

        private async Task<string> GenerateOfferNumberAsync()
        {
            var year = DateTime.Now.Year;
            var count = await _repository.CountForYearAsync(year) + 1;
            var offerNumber = $"OFF-{year}-{count:D4}";
            while (await _repository.OfferNumberExistsAsync(offerNumber))
            {
                count++;
                offerNumber = $"OFF-{year}-{count:D4}";
            }
            return offerNumber;
        }
    }
}
