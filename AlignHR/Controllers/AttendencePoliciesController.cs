using AlignHR.Data;
using AlignHR.Helpers;
using AlignHR.Models;
using AlignHR.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AlignHR.Controllers
{
    public class AttendencePoliciesController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public AttendencePoliciesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: AttendencePolicies
        [RequireUrlPermission]
        public async Task<IActionResult> Index(string search)
        {
            var obj =  _context.AttendencePolicies.ToList();
            return View(obj); 
        }

        // GET: AttendencePolicies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var policy = await _context.AttendencePolicies
                .FirstOrDefaultAsync(m => m.id == id);
            if (policy == null) return NotFound();

            return View(policy);
        }

        // GET: AttendencePolicies/Create
        [RequireUrlPermission]
        public IActionResult Create()
        {
            return View();
        }

        // POST: AttendencePolicies/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AttendencePolicies policy)
        {
            if (ModelState.IsValid)
            {
                var currentUserId = PermissionHelper.GetCurrentUserId(HttpContext) ?? 0;
                policy.createdby = currentUserId;
                policy.createat = DateTime.Now;
                policy.updatedby = currentUserId;
                policy.updateat = DateTime.Now;

                _context.Add(policy);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(policy);
        }

        // GET: AttendencePolicies/Edit/5
        [RequireUrlPermission]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var policy = await _context.AttendencePolicies.FindAsync(id);
            if (policy == null) return NotFound();

            return View(policy);
        }

        // POST: AttendencePolicies/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AttendencePolicies policy)
        {
            if (id != policy.id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var currentUserId = PermissionHelper.GetCurrentUserId(HttpContext) ?? 0;

                    // Preserve createdby and createat
                    var existingPolicy = await _context.AttendencePolicies.AsNoTracking().FirstOrDefaultAsync(p => p.id == id);
                    if (existingPolicy == null) return NotFound();

                    policy.createdby = existingPolicy.createdby;
                    policy.createat = existingPolicy.createat;
                    policy.updatedby = currentUserId;
                    policy.updateat = DateTime.Now;

                    _context.Update(policy);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PolicyExists(policy.id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(policy);
        }

        // GET: AttendencePolicies/Delete/5
        [RequireUrlPermission]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var policy = await _context.AttendencePolicies
                .FirstOrDefaultAsync(m => m.id == id);
            if (policy == null) return NotFound();

            return View(policy);
        }

        // POST: AttendencePolicies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var policy = await _context.AttendencePolicies.FindAsync(id);
            if (policy != null)
            {
                _context.AttendencePolicies.Remove(policy);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool PolicyExists(int id)
        {
            return _context.AttendencePolicies.Any(e => e.id == id);
        }
    }
}
