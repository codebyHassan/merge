using AlignHR.Models;

namespace AlignHR.Services
{
    public interface IEmployeeCreationService
    {
        Task<int> CreateEmployeeFromCandidateAsync(
            decimal candidateId,
            decimal onboardingId,
            HrJoiningConfirmationFormViewModel form);
    }
}
