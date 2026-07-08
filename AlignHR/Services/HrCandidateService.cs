using AlignHR.Models;
using AlignHR.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace AlignHR.Services
{
    public class HrCandidateService : IHrCandidateService
    {
        private readonly IHrCandidateRepository _repository;
        private readonly IFileStorageService _fileStorage;
        private readonly Data.ApplicationDbContext _context;

        public HrCandidateService(
            IHrCandidateRepository repository,
            IFileStorageService fileStorage,
            Data.ApplicationDbContext context)
        {
            _repository = repository;
            _fileStorage = fileStorage;
            _context = context;
        }

        public async Task<IPagedList<HrCandidateListItemViewModel>> GetPagedAsync(string? search, int pageNumber, int pageSize)
        {
            var query = _repository.Query();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(c =>
                    c.FirstName.Contains(search) ||
                    (c.LastName != null && c.LastName.Contains(search)) ||
                    c.Email.Contains(search) ||
                    (c.CurrentEmployer != null && c.CurrentEmployer.Contains(search)));

            var candidates = await query.OrderByDescending(c => c.CreatedDate).ToListAsync();

            var candidateIds = candidates.Select(c => c.Id).ToList();
            var appCounts = await _context.HrJobApplications
                .Where(a => a.CandidateFK.HasValue && candidateIds.Contains(a.CandidateFK.Value) && a.IsActive)
                .GroupBy(a => a.CandidateFK!.Value)
                .Select(g => new { g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Key, x => x.Count);

            var items = candidates.Select(c => new HrCandidateListItemViewModel
            {
                Id = c.Id,
                FullName = $"{c.FirstName} {c.LastName}".Trim(),
                Email = c.Email,
                Phone = c.Phone,
                TotalExperienceYears = c.TotalExperienceYears,
                CurrentEmployer = c.CurrentEmployer,
                CurrentDesignation = c.CurrentDesignation,
                ApplicationCount = appCounts.TryGetValue(c.Id, out var cnt) ? cnt : 0,
                CreatedDate = c.CreatedDate
            });

            return items.ToPagedList(pageNumber, pageSize);
        }

        public async Task<HrCandidateFormViewModel?> GetForEditAsync(decimal id)
        {
            var c = await _repository.GetByIdAsync(id);
            if (c == null) return null;

            return new HrCandidateFormViewModel
            {
                Id = c.Id,
                FirstName = c.FirstName,
                LastName = c.LastName,
                Email = c.Email,
                Phone = c.Phone,
                DateOfBirth = c.DateOfBirth,
                Gender = c.Gender,
                CurrentLocation = c.CurrentLocation,
                City = c.City,
                Country = c.Country,
                HighestDegree = c.HighestDegree,
                FieldOfStudy = c.FieldOfStudy,
                University = c.University,
                TotalExperienceYears = c.TotalExperienceYears,
                IsCurrentlyEmployed = c.IsCurrentlyEmployed,
                CurrentEmployer = c.CurrentEmployer,
                CurrentDesignation = c.CurrentDesignation,
                NoticePeriod = c.NoticePeriod,
                LinkedInProfile = c.LinkedInProfile
            };
        }

        public async Task<HrCandidateDetailsViewModel?> GetDetailsAsync(decimal id)
        {
            var c = await _repository.GetWithDetailsAsync(id);
            if (c == null) return null;

            var postingIds = c.Applications
                .Where(a => a.JobPostingFK.HasValue)
                .Select(a => a.JobPostingFK!.Value).Distinct().ToList();

            var postings = await _context.HrJobPostings
                .Where(p => postingIds.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id, p => new { p.JobCode, p.JobTitle });

            return new HrCandidateDetailsViewModel
            {
                Id = c.Id,
                FullName = $"{c.FirstName} {c.LastName}".Trim(),
                Email = c.Email,
                Phone = c.Phone,
                DateOfBirth = c.DateOfBirth,
                Gender = c.Gender,
                CurrentLocation = c.CurrentLocation,
                City = c.City,
                Country = c.Country,
                HighestDegree = c.HighestDegree,
                FieldOfStudy = c.FieldOfStudy,
                University = c.University,
                TotalExperienceYears = c.TotalExperienceYears,
                IsCurrentlyEmployed = c.IsCurrentlyEmployed,
                CurrentEmployer = c.CurrentEmployer,
                CurrentDesignation = c.CurrentDesignation,
                NoticePeriod = c.NoticePeriod,
                LinkedInProfile = c.LinkedInProfile,
                CreatedDate = c.CreatedDate,
                ModifiedDate = c.ModifiedDate,
                Documents = c.Documents.Select(d => new HrCandidateDocumentViewModel
                {
                    Id = d.Id,
                    DocumentType = d.DocumentType,
                    FileName = d.FileName,
                    FilePath = d.FilePath,
                    UploadedDate = d.UploadedDate
                }).ToList(),
                Applications = c.Applications.Select(a => new HrCandidateApplicationSummaryViewModel
                {
                    ApplicationId = a.Id,
                    JobCode = a.JobPostingFK.HasValue && postings.TryGetValue(a.JobPostingFK.Value, out var p) ? p.JobCode : null,
                    JobTitle = a.JobPostingFK.HasValue && postings.TryGetValue(a.JobPostingFK.Value, out var p2) ? p2.JobTitle : null,
                    CurrentStage = a.CurrentStage?.StageName,
                    AppliedDate = a.AppliedDate,
                    IsActive = a.IsActive
                }).ToList()
            };
        }

        public async Task<decimal> CreateAsync(HrCandidateFormViewModel model)
        {
            if (await _repository.EmailExistsAsync(model.Email))
                throw new InvalidOperationException($"A candidate with email '{model.Email}' already exists.");

            var candidate = new HrCandidate
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Phone = model.Phone,
                DateOfBirth = model.DateOfBirth,
                Gender = model.Gender,
                CurrentLocation = model.CurrentLocation,
                City = model.City,
                Country = model.Country,
                HighestDegree = model.HighestDegree,
                FieldOfStudy = model.FieldOfStudy,
                University = model.University,
                TotalExperienceYears = model.TotalExperienceYears,
                IsCurrentlyEmployed = model.IsCurrentlyEmployed,
                CurrentEmployer = model.CurrentEmployer,
                CurrentDesignation = model.CurrentDesignation,
                NoticePeriod = model.NoticePeriod,
                LinkedInProfile = model.LinkedInProfile,
                CreatedDate = DateTime.Now
            };

            await _repository.AddAsync(candidate);
            await _repository.SaveChangesAsync();

            if (model.DocumentFile != null && model.DocumentFile.Length > 0)
                await UploadDocumentAsync(candidate.Id, model.DocumentFile, model.DocumentType);

            return candidate.Id;
        }

        public async Task UpdateAsync(decimal id, HrCandidateFormViewModel model)
        {
            var candidate = await _repository.GetByIdAsync(id);
            if (candidate == null)
                throw new InvalidOperationException("Candidate not found.");

            if (await _repository.EmailExistsAsync(model.Email, id))
                throw new InvalidOperationException($"A candidate with email '{model.Email}' already exists.");

            candidate.FirstName = model.FirstName;
            candidate.LastName = model.LastName;
            candidate.Email = model.Email;
            candidate.Phone = model.Phone;
            candidate.DateOfBirth = model.DateOfBirth;
            candidate.Gender = model.Gender;
            candidate.CurrentLocation = model.CurrentLocation;
            candidate.City = model.City;
            candidate.Country = model.Country;
            candidate.HighestDegree = model.HighestDegree;
            candidate.FieldOfStudy = model.FieldOfStudy;
            candidate.University = model.University;
            candidate.TotalExperienceYears = model.TotalExperienceYears;
            candidate.IsCurrentlyEmployed = model.IsCurrentlyEmployed;
            candidate.CurrentEmployer = model.CurrentEmployer;
            candidate.CurrentDesignation = model.CurrentDesignation;
            candidate.NoticePeriod = model.NoticePeriod;
            candidate.LinkedInProfile = model.LinkedInProfile;
            candidate.ModifiedDate = DateTime.Now;

            _repository.Update(candidate);

            if (model.DocumentFile != null && model.DocumentFile.Length > 0)
                await UploadDocumentAsync(id, model.DocumentFile, model.DocumentType);

            await _repository.SaveChangesAsync();
        }

        public async Task DeleteAsync(decimal id)
        {
            var candidate = await _repository.GetByIdAsync(id);
            if (candidate == null) return;

            candidate.IsDeleted = true;
            candidate.ModifiedDate = DateTime.Now;
            _repository.Update(candidate);
            await _repository.SaveChangesAsync();
        }

        public async Task UploadDocumentAsync(decimal candidateId, IFormFile file, string documentType)
        {
            var (fileName, relativePath) = await _fileStorage.SaveAsync(file, candidateId.ToString("0"));

            await _repository.AddDocumentAsync(new HrCandidateDocument
            {
                CandidateFk = candidateId,
                DocumentType = documentType,
                FileName = fileName,
                FilePath = relativePath,
                UploadedDate = DateTime.Now
            });
            await _repository.SaveChangesAsync();
        }

        public async Task DeleteDocumentAsync(decimal documentId)
        {
            var doc = await _repository.GetDocumentAsync(documentId);
            if (doc == null) return;

            if (!string.IsNullOrEmpty(doc.FilePath))
                _fileStorage.Delete(doc.FilePath);

            _repository.RemoveDocument(doc);
            await _repository.SaveChangesAsync();
        }
    }
}
