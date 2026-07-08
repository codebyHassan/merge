using AlignHR.Models;

namespace AlignHR.Services
{
    public interface IHrInterviewService
    {

        Task<HrApplicationInterviewsViewModel?> GetApplicationInterviewsAsync(decimal applicationId, decimal? currentEmpId = null);
        Task<HrScheduleFormViewModel?> GetForScheduleAsync(decimal applicationId);
        Task<decimal> ScheduleInterviewAsync(HrScheduleFormViewModel model, decimal createdByEmpId);
        Task<HrInterviewEditViewModel?> GetForEditAsync(decimal scheduleId);
        Task UpdateScheduleAsync(HrInterviewEditViewModel model);
        Task<HrInterviewDetailsViewModel?> GetDetailsAsync(decimal scheduleId);
        Task ChangeStatusAsync(decimal scheduleId, string newStatus);
        Task RescheduleAsync(decimal scheduleId, DateTime newDateTime, string? newLink, string? newLocation);

        Task AddPanelistAsync(decimal scheduleId, decimal employeeId);
        Task RemovePanelistAsync(decimal panelId);

        Task<HrFeedbackFormViewModel?> GetFeedbackFormAsync(decimal scheduleId);
        Task SubmitFeedbackAsync(HrFeedbackFormViewModel model, decimal interviewerEmpId, bool isSubmit);
        Task<HrFeedbackDetailsViewModel?> GetFeedbackDetailsAsync(decimal feedbackId);
    }
}
