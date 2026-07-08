using AlignHR.Models;
using AlignHR.Security;
using AlignHR.Services;
using Microsoft.AspNetCore.Mvc;

namespace AlignHR.Controllers
{
    public class HrCandidatesController : BaseController
    {
        private readonly IHrCandidateService _service;
        private readonly IFileStorageService _fileStorage;

        public HrCandidatesController(IHrCandidateService service, IFileStorageService fileStorage)
        {
            _service = service;
            _fileStorage = fileStorage;
        }

        [RequireUrlPermission]
        public async Task<IActionResult> Index(string? search, int? page)
        {
            ViewData["CurrentFilter"] = search;
            var model = await _service.GetPagedAsync(search, page ?? 1, 15);
            return View(model);
        }

        [RequireUrlPermission]
        public IActionResult Create() => View(new HrCandidateFormViewModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HrCandidateFormViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var id = await _service.CreateAsync(model);
                    TempData["Success"] = "Candidate created successfully.";
                    return RedirectToAction(nameof(Details), new { id });
                }
                catch (InvalidOperationException ex) { ModelState.AddModelError("", ex.Message); }
            }
            return View(model);
        }

        [RequireUrlPermission]
        public async Task<IActionResult> Edit(decimal id)
        {
            var model = await _service.GetForEditAsync(id);
            if (model == null) return NotFound();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, HrCandidateFormViewModel model)
        {
            if (id != model.Id) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    await _service.UpdateAsync(id, model);
                    TempData["Success"] = "Candidate updated successfully.";
                    return RedirectToAction(nameof(Details), new { id });
                }
                catch (InvalidOperationException ex) { ModelState.AddModelError("", ex.Message); }
            }
            return View(model);
        }

        [RequireUrlPermission]
        public async Task<IActionResult> Details(decimal id)
        {
            var model = await _service.GetDetailsAsync(id);
            if (model == null) return NotFound();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadDocument(decimal candidateId, Microsoft.AspNetCore.Http.IFormFile file, string documentType)
        {
            if (file == null || file.Length == 0)
            {
                TempData["Error"] = "Please select a file to upload.";
                return RedirectToAction(nameof(Details), new { id = candidateId });
            }
            try
            {
                await _service.UploadDocumentAsync(candidateId, file, documentType);
                TempData["Success"] = "Document uploaded successfully.";
            }
            catch (Exception ex) { TempData["Error"] = ex.Message; }
            return RedirectToAction(nameof(Details), new { id = candidateId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteDocument(decimal documentId, decimal candidateId)
        {
            await _service.DeleteDocumentAsync(documentId);
            TempData["Success"] = "Document deleted.";
            return RedirectToAction(nameof(Details), new { id = candidateId });
        }

        public IActionResult Download(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return NotFound();

            string absolute;
            try { absolute = _fileStorage.GetAbsolutePath(path); }
            catch (InvalidOperationException) { return NotFound(); }

            if (!System.IO.File.Exists(absolute)) return NotFound();
            var fileName = System.IO.Path.GetFileName(absolute);
            var contentType = "application/octet-stream";
            return PhysicalFile(absolute, contentType, fileName);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(decimal id)
        {
            await _service.DeleteAsync(id);
            TempData["Success"] = "Candidate removed.";
            return RedirectToAction(nameof(Index));
        }
    }
}
