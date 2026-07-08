using AlignHR.Data;
using AlignHR.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AlignHR.Controllers
{
    // No auth required — candidate accesses via secure token link in email
    public class OfferResponseController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OfferResponseController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Respond(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return View("Invalid");

            var offer = await _context.HrOffers
                .Include(o => o.JobApplication)
                    .ThenInclude(a => a!.Candidate)
                .Include(o => o.JobApplication)
                    .ThenInclude(a => a!.JobPosting)
                .FirstOrDefaultAsync(o => o.ResponseToken == token);

            if (offer == null)
                return View("Invalid");

            var vm = BuildViewModel(offer, token);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Accept(string token, DateTime joiningDate, string? comments)
        {
            var offer = await LoadOffer(token);
            if (offer == null) return View("Invalid");

            var vm = BuildViewModel(offer, token);
            if (vm.State != OfferResponseState.Pending)
                return View("Respond", vm);

            // Server-side validation: joining date must be today or later and within a
            // sane horizon. The client 'min' attribute alone is trivially bypassable.
            var maxJoining = DateTime.Today.AddYears(1);
            if (joiningDate.Date < DateTime.Today || joiningDate.Date > maxJoining)
            {
                ViewData["JoiningError"] = "Please choose a joining date between today and " +
                                           $"{maxJoining:dd MMM yyyy}.";
                return View("Respond", vm);
            }

            offer.Status               = "Accepted";
            offer.ResponseDate         = DateTime.Now;
            offer.CandidateJoiningDate = joiningDate;
            offer.ResponseComments     = string.IsNullOrWhiteSpace(comments) ? null : comments.Trim();
            offer.UpdatedOn           = DateTime.Now;

            if (offer.JobApplicationFk.HasValue)
                _context.HrApplicationStageHistories.Add(new HrApplicationStageHistory
                {
                    JobApplicationFk = offer.JobApplicationFk.Value,
                    Comments         = $"Offer Accepted | Joining date: {joiningDate:dd MMM yyyy}",
                    ChangedDate      = DateTime.Now
                });

            await _context.SaveChangesAsync();

            vm.State       = OfferResponseState.Accepted;
            vm.JoiningDate = joiningDate;
            return View("Respond", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Decline(string token, string? reason)
        {
            var offer = await LoadOffer(token);
            if (offer == null) return View("Invalid");

            var vm = BuildViewModel(offer, token);
            if (vm.State != OfferResponseState.Pending)
                return View("Respond", vm);

            offer.Status           = "Declined";
            offer.ResponseDate     = DateTime.Now;
            offer.ResponseComments = reason;
            offer.UpdatedOn        = DateTime.Now;

            if (offer.JobApplicationFk.HasValue)
                _context.HrApplicationStageHistories.Add(new HrApplicationStageHistory
                {
                    JobApplicationFk = offer.JobApplicationFk.Value,
                    Comments         = $"Offer Declined | {(string.IsNullOrWhiteSpace(reason) ? "No reason given" : reason)}",
                    ChangedDate      = DateTime.Now
                });

            await _context.SaveChangesAsync();

            vm.State = OfferResponseState.Declined;
            return View("Respond", vm);
        }

        private async Task<HrOffer?> LoadOffer(string token) =>
            await _context.HrOffers
                .Include(o => o.JobApplication)
                    .ThenInclude(a => a!.Candidate)
                .Include(o => o.JobApplication)
                    .ThenInclude(a => a!.JobPosting)
                .FirstOrDefaultAsync(o => o.ResponseToken == token);

        private static OfferResponseViewModel BuildViewModel(HrOffer offer, string token)
        {
            var candidate = offer.JobApplication?.Candidate;
            var posting   = offer.JobApplication?.JobPosting;

            OfferResponseState state;
            if (offer.Status == "Accepted")
                state = OfferResponseState.Accepted;
            else if (offer.Status == "Declined")
                state = OfferResponseState.Declined;
            else if (offer.ExpiryDate.HasValue && offer.ExpiryDate.Value.Date < DateTime.Today)
                state = OfferResponseState.Expired;
            else if (offer.Status != "Sent" && offer.Status != "Approved")
                state = OfferResponseState.Expired;
            else
                state = OfferResponseState.Pending;

            var notes = string.IsNullOrWhiteSpace(offer.Notes)
                ? new List<string>()
                : offer.Notes.Split('\n', StringSplitOptions.RemoveEmptyEntries)
                    .Select(n => n.TrimStart('•').Trim())
                    .Where(n => n.Length > 0)
                    .ToList();

            return new OfferResponseViewModel
            {
                Token         = token,
                OfferNumber   = offer.OfferNumber,
                CandidateName = candidate != null ? $"{candidate.FirstName} {candidate.LastName}".Trim() : "Candidate",
                JobTitle      = posting?.JobTitle ?? "—",
                Salary        = offer.ProposedSalary,
                ExpiryDate    = offer.ExpiryDate,
                State         = state,
                PriorComments = offer.ResponseComments,
                Notes         = notes
            };
        }
    }

    public enum OfferResponseState { Pending, Accepted, Declined, Expired }

    public class OfferResponseViewModel
    {
        public string Token         { get; set; } = string.Empty;
        public string? OfferNumber  { get; set; }
        public string CandidateName { get; set; } = string.Empty;
        public string JobTitle      { get; set; } = string.Empty;
        public decimal Salary       { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public OfferResponseState State { get; set; }
        public string? PriorComments    { get; set; }
        public DateTime? JoiningDate    { get; set; }
        public List<string> Notes   { get; set; } = new();
    }
}
