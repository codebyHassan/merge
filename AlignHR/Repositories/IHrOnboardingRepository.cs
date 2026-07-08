using AlignHR.Models;

namespace AlignHR.Repositories
{
    public interface IHrOnboardingRepository
    {
        IQueryable<HrOnboarding> Query();
        Task<HrOnboarding?> GetByIdAsync(decimal id);
        Task<HrOnboarding?> GetWithDetailsAsync(decimal id);
        Task<HrOnboarding?> GetByOfferAsync(decimal offerId);
        Task AddOnboardingAsync(HrOnboarding onboarding);
        void UpdateOnboarding(HrOnboarding onboarding);

        Task<IList<HrOnboardingTaskTemplate>> GetActiveTemplatesAsync();
        Task<HrOnboardingTaskTemplate?> GetTemplateByIdAsync(decimal id);
        Task AddTemplateAsync(HrOnboardingTaskTemplate template);
        void UpdateTemplate(HrOnboardingTaskTemplate template);

        Task<HrOnboardingTask?> GetTaskByIdAsync(decimal id);
        Task AddTaskAsync(HrOnboardingTask task);
        void UpdateTask(HrOnboardingTask task);

        Task AddDocumentAsync(HrOnboardingDocument document);
        Task<HrOnboardingDocument?> GetDocumentByIdAsync(decimal id);
        void RemoveDocument(HrOnboardingDocument document);

        Task AddJoiningConfirmationAsync(HrJoiningConfirmation confirmation);
        Task<bool> JoiningConfirmationExistsAsync(decimal onboardingId);

        Task SaveChangesAsync();
    }
}
