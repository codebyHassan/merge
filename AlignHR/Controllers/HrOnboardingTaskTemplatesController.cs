using AlignHR.Models;
using AlignHR.Repositories;
using AlignHR.Security;
using Microsoft.AspNetCore.Mvc;

namespace AlignHR.Controllers
{
    public class HrOnboardingTaskTemplatesController : BaseController
    {
        private readonly IHrOnboardingRepository _repository;

        public HrOnboardingTaskTemplatesController(IHrOnboardingRepository repository)
        {
            _repository = repository;
        }

        [RequireUrlPermission]
        public async Task<IActionResult> Index()
        {
            var templates = await _repository.GetActiveTemplatesAsync();
            var model = templates.Select(t => new HrTaskTemplateListItemViewModel
            {
                Id = t.Id,
                TaskName = t.TaskName,
                ResponsibleDepartment = t.ResponsibleDepartment,
                IsMandatory = t.IsMandatory,
                IsActive = t.IsActive
            }).ToList();
            return View(model);
        }

        [RequireUrlPermission]
        public IActionResult Create() => View(new HrTaskTemplateFormViewModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HrTaskTemplateFormViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _repository.AddTemplateAsync(new HrOnboardingTaskTemplate
                {
                    TaskName = model.TaskName,
                    ResponsibleDepartment = model.ResponsibleDepartment,
                    IsMandatory = model.IsMandatory,
                    IsActive = model.IsActive
                });
                await _repository.SaveChangesAsync();
                TempData["Success"] = "Task template created.";
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        [RequireUrlPermission]
        public async Task<IActionResult> Edit(decimal id)
        {
            var template = await _repository.GetTemplateByIdAsync(id);
            if (template == null) return NotFound();
            return View(new HrTaskTemplateFormViewModel
            {
                Id = template.Id,
                TaskName = template.TaskName,
                ResponsibleDepartment = template.ResponsibleDepartment,
                IsMandatory = template.IsMandatory,
                IsActive = template.IsActive
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(HrTaskTemplateFormViewModel model)
        {
            if (ModelState.IsValid)
            {
                var template = await _repository.GetTemplateByIdAsync(model.Id);
                if (template == null) return NotFound();
                template.TaskName = model.TaskName;
                template.ResponsibleDepartment = model.ResponsibleDepartment;
                template.IsMandatory = model.IsMandatory;
                template.IsActive = model.IsActive;
                _repository.UpdateTemplate(template);
                await _repository.SaveChangesAsync();
                TempData["Success"] = "Template updated.";
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deactivate(decimal id)
        {
            var template = await _repository.GetTemplateByIdAsync(id);
            if (template != null)
            {
                template.IsActive = false;
                _repository.UpdateTemplate(template);
                await _repository.SaveChangesAsync();
                TempData["Success"] = "Template deactivated.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
