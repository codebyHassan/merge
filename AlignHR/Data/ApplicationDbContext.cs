using AlignHR.Models;
using Microsoft.EntityFrameworkCore;

namespace AlignHR.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
       : base(options)
        {
        }

        public DbSet<Location> location { get; set; }
        public DbSet<Department> department { get; set; }
        public DbSet<Employee> emp { get; set; }
        public DbSet<EmployeeHireHistory> EmployeeHireHistories { get; set; }
        public DbSet<Shift> shifts { get; set; }
        public DbSet<Valueset> valuesets { get; set; }
        public DbSet<Function> functions { get; set; }
        public DbSet<Role> roles { get; set; }
        public DbSet<User> users { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<FunctionRole> FunctionRoles { get; set; }
        public DbSet<EmployeeShifts> EmployeeShifts { get; set; }
        public DbSet<AttendencePolicies> AttendencePolicies { get; set; }
        public DbSet<AttendenceLogs> AttendenceLogs { get; set; }
        public DbSet<AttendenceException> AttendenceException { get; set; }
        public DbSet<PayRollDefinationFile> PayRollDefinationFile { get; set; }
        public DbSet<PayRollGenrate> PayRollGenrate { get; set; }
        public DbSet<FiscalYear> FiscalYear { get; set; }
        public DbSet<TaxSlabMaster> TaxSlabMaster { get; set;  }
        public DbSet<TaxSurcharge> TaxSurcharge { get; set; }
        public DbSet<SalaryPeriod> SalaryPeriod { get; set; }
        public DbSet<IncomeTaxDetuction> IncomeTaxDetuction { get; set; }
        public DbSet<WithoutPayDays> WithoutPayDays { get; set; }
        public DbSet<EOBIDetuction> EOBIDetuction { get; set; }
        public DbSet<MasterLoanAdvance> MasterLoanAdvance { get; set; }
        public DbSet<MasterLoanAdvanceDetail> MasterLoanAdvanceDetail { get; set; }
        public DbSet<Overtime> Overtimes { get; set; }
        public DbSet<OverTimeHours> OverTimeHours { get; set; }
        public DbSet<PFMembers> PFMembers { get; set; }
        public DbSet<SalarySlipMaster> SalarySlipMasters { get; set; }
        public DbSet<SalarySlipDetail> SalarySlipDetails { get; set; }
        public DbSet<Bonus> Bonuses { get; set; }
        public DbSet<SalaryAdjustments> SalaryAdjustments { get; set; }
        public DbSet<Execution> Executions { get; set; }

        // Recruitment
        public DbSet<HrRequisition> HrRequisitions { get; set; }
        public DbSet<HrRequisitionType> HrRequisitionTypes { get; set; }
        public DbSet<HrRequisitionSkill> HrRequisitionSkills { get; set; }
        public DbSet<HrRequisitionOffering> HrRequisitionOfferings { get; set; }
        public DbSet<HrRecruitmentUnit> HrRecruitmentUnits { get; set; }
        public DbSet<HrRecruitmentUnitDepartment> HrRecruitmentUnitDepartments { get; set; }
        public DbSet<HrRecruitmentTeam> HrRecruitmentTeams { get; set; }
        public DbSet<HrWorkflowDefinition> HrWorkflowDefinitions { get; set; }
        public DbSet<HrWorkflowStep> HrWorkflowSteps { get; set; }
        public DbSet<HrWorkflowInstance> HrWorkflowInstances { get; set; }
        public DbSet<HrDepartmentApprover> HrDepartmentApprovers { get; set; }
        public DbSet<HrWorkflowInstanceStep> HrWorkflowInstanceSteps { get; set; }
        public DbSet<HrWorkflowAction> HrWorkflowActions { get; set; }
        public DbSet<HrRequisitionAssignment> HrRequisitionAssignments { get; set; }
        public DbSet<HrJobPosting> HrJobPostings { get; set; }
        public DbSet<HrCandidate> HrCandidates { get; set; }
        public DbSet<HrCandidateDocument> HrCandidateDocuments { get; set; }
        public DbSet<HrApplicationStage> HrApplicationStages { get; set; }
        public DbSet<HrJobApplication> HrJobApplications { get; set; }
        public DbSet<HrApplicationStageHistory> HrApplicationStageHistories { get; set; }
        public DbSet<HrOffer> HrOffers { get; set; }
        public DbSet<HrOfferVersion> HrOfferVersions { get; set; }
        public DbSet<HrOfferApproval> HrOfferApprovals { get; set; }
        public DbSet<HrOfferDocument> HrOfferDocuments { get; set; }
        public DbSet<HrHiringDecision> HrHiringDecisions { get; set; }
        public DbSet<HrInterviewRound> HrInterviewRounds { get; set; }
        public DbSet<HrInterviewSchedule> HrInterviewSchedules { get; set; }
        public DbSet<HrInterviewPanel> HrInterviewPanels { get; set; }
        public DbSet<HrInterviewFeedback> HrInterviewFeedbacks { get; set; }
        public DbSet<HrEvaluationCriteria> HrEvaluationCriterias { get; set; }
        public DbSet<HrEvaluationScore> HrEvaluationScores { get; set; }
        public DbSet<HrOnboarding> HrOnboardings { get; set; }
        public DbSet<HrOnboardingTaskTemplate> HrOnboardingTaskTemplates { get; set; }
        public DbSet<HrOnboardingTask> HrOnboardingTasks { get; set; }
        // Leave Management
        public DbSet<LeaveType> LeaveTypes { get; set; }
        public DbSet<LeavePolicyRule> LeavePolicyRules { get; set; }
        public DbSet<LeaveBalance> LeaveBalances { get; set; }
        public DbSet<EmployeeLeavePolicy> EmployeeLeavePolicies { get; set; }
        public DbSet<LeaveRequest> LeaveRequests { get; set; }
        public DbSet<LeaveApproval> LeaveApprovals { get; set; }
        public DbSet<LeaveConfiguration> LeaveConfigurations { get; set; }
        public DbSet<LeaveConfigurationDetail> LeaveConfigurationDetails { get; set; }
        public DbSet<LeaveOpening> LeaveOpenings { get; set; }
        public DbSet<LeaveTransaction> LeaveTransactions { get; set; }
        public DbSet<LeaveYearLock> LeaveYearLocks { get; set; }
        public DbSet<AlignHR.Models.LeavePolicy> LeavePolicy { get; set; } = default!;
        public DbSet<TrnEmail> TrnEmails { get; set; }

        // Document Management
        public DbSet<DocumentCategory> DocumentCategories { get; set; }
        public DbSet<DocumentType> DocumentTypes { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<DmsDefinationFile> DmsDefinationFiles { get; set; }
        public DbSet<ApprovalTemplate> ApprovalTemplates { get; set; }
        public DbSet<ApprovalTemplateStep> ApprovalTemplateSteps { get; set; }
        public DbSet<DocApprovalInstance> DocApprovalInstances { get; set; }
        public DbSet<DocApprovalInstanceStep> DocApprovalInstanceSteps { get; set; }
        public DbSet<DocApprovalAssignment> DocApprovalAssignments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Global Deletion Restriction
            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }

            // Restrict DeleteBehavior for Valueset relationships to prevent multiple cascade paths
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.EmploymentType)
                .WithMany()
                .HasForeignKey(e => e.EmploymentTypeFk);

            modelBuilder.Entity<Employee>()
                .HasOne(e => e.EmploymentStatus)
                .WithMany()
                .HasForeignKey(e => e.EmploymentStatusFk);

            modelBuilder.Entity<Employee>()
                .HasOne(e => e.SubDepartment)
                .WithMany()
                .HasForeignKey(e => e.SubDepartmentFk)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Grade)
                .WithMany()
                .HasForeignKey(e => e.GradeFk)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Division)
                .WithMany()
                .HasForeignKey(e => e.DivisionFk)
                .OnDelete(DeleteBehavior.Restrict);
            
            // Decimal precision for Attendance Logs (GPS)
            modelBuilder.Entity<AttendenceLogs>()
                .Property(p => p.latitude)
                .HasColumnType("decimal(18, 10)");
            
            modelBuilder.Entity<AttendenceLogs>()
                .Property(p => p.longitude)
                .HasColumnType("decimal(18, 10)");

            // ===== Employee Unique Index on Code (EmpNo) =====
            modelBuilder.Entity<Employee>()
                .HasIndex(e => e.Code)
                .IsUnique()
                .HasDatabaseName("IX_Employees_Code_Unique");

            // ===== Employee Hire History =====
            modelBuilder.Entity<EmployeeHireHistory>()
                .ToTable("HR_EmployeeHireHistory");

            modelBuilder.Entity<EmployeeHireHistory>()
                .HasOne(h => h.Employee)
                .WithMany()
                .HasForeignKey(h => h.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EmployeeHireHistory>()
                .HasOne(h => h.ReplacedEmployee)
                .WithMany()
                .HasForeignKey(h => h.ReplacedEmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EmployeeHireHistory>()
                .HasOne(h => h.FromDepartment)
                .WithMany()
                .HasForeignKey(h => h.FromDepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            // ===== EmployeeLeavePolicy: one assignment per employee per policy =====
            modelBuilder.Entity<EmployeeLeavePolicy>()
                .HasIndex(e => new { e.EmployeeId, e.LeavePolicyId })
                .IsUnique()
                .HasDatabaseName("IX_EmployeeLeavePolicies_Employee_Policy_Unique");

            // ===== LeaveRequest Workflow Relationships =====

            // Employee who submitted the leave (existing relationship)
            modelBuilder.Entity<LeaveRequest>()
                .HasOne(lr => lr.Employee)
                .WithMany(e => e.LeaveRequests)
                .HasForeignKey(lr => lr.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Current approver (second FK to Employee — must be explicitly configured)
            modelBuilder.Entity<LeaveRequest>()
                .HasOne(lr => lr.CurrentApprover)
                .WithMany()
                .HasForeignKey(lr => lr.CurrentApproverId)
                .OnDelete(DeleteBehavior.Restrict);

            // Concurrency token
            modelBuilder.Entity<LeaveRequest>()
                .Property(lr => lr.RowVersion)
                .IsRowVersion();

            // ===== LeaveApproval Relationships =====

            modelBuilder.Entity<LeaveApproval>()
                .HasOne(la => la.LeaveRequest)
                .WithMany(lr => lr.LeaveApprovals)
                .HasForeignKey(la => la.LeaveRequestId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<LeaveApproval>()
                .HasOne(la => la.ApproverEmployee)
                .WithMany(e => e.LeaveApprovals)
                .HasForeignKey(la => la.ApproverEmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            // ===== User → Employee Linkage =====

            modelBuilder.Entity<User>()
                .HasOne(u => u.Employee)
                .WithMany()
                .HasForeignKey(u => u.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            // ===== LeaveConfiguration Relationships =====

            modelBuilder.Entity<LeaveConfigurationDetail>()
                .HasOne(d => d.LeaveConfiguration)
                .WithMany(c => c.Details)
                .HasForeignKey(d => d.LeaveConfigurationId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<LeaveConfigurationDetail>()
                .HasIndex(d => d.ApproverEmpNo)
                .HasDatabaseName("IX_LeaveConfigDetail_ApproverEmpNo");

            modelBuilder.Entity<LeaveConfigurationDetail>()
                .HasIndex(d => new { d.LeaveConfigurationId, d.ReferenceId })
                .IsUnique()
                .HasDatabaseName("IX_LeaveConfigDetail_Config_Ref_Unique");

            modelBuilder.Entity<LeavePolicyRule>()
                .HasIndex(r => new { r.LeavePolicyId, r.LeaveTypeId })
                .IsUnique()
                .HasDatabaseName("IX_LeavePolicyRule_Policy_LeaveType_Unique");

            // ===== Leave Accounting Relationships & Indexes =====

            modelBuilder.Entity<LeaveOpening>()
                .HasIndex(lo => new { lo.EmployeeFk, lo.LeaveTypeFk, lo.Year })
                .IsUnique()
                .HasDatabaseName("IX_LeaveOpening_Employee_LeaveType_Year");

            modelBuilder.Entity<LeaveOpening>()
                .HasOne(lo => lo.Employee)
                .WithMany()
                .HasForeignKey(lo => lo.EmployeeFk)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<LeaveOpening>()
                .HasOne(lo => lo.LeaveType)
                .WithMany()
                .HasForeignKey(lo => lo.LeaveTypeFk)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<LeaveTransaction>()
                .HasIndex(lt => new { lt.EmployeeFk, lt.LeaveTypeFk, lt.Year })
                .HasDatabaseName("IX_LeaveTransaction_Employee_LeaveType_Year");

            modelBuilder.Entity<LeaveTransaction>()
                .HasIndex(lt => lt.ReferenceNo)
                .HasDatabaseName("IX_LeaveTransaction_ReferenceNo");

            modelBuilder.Entity<LeaveTransaction>()
                .HasOne(lt => lt.Employee)
                .WithMany()
                .HasForeignKey(lt => lt.EmployeeFk)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<LeaveTransaction>()
                .HasOne(lt => lt.LeaveType)
                .WithMany()
                .HasForeignKey(lt => lt.LeaveTypeFk)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<LeaveTransaction>()
                .HasOne(lt => lt.LeaveRequest)
                .WithMany()
                .HasForeignKey(lt => lt.LeaveRequestFk)
                .OnDelete(DeleteBehavior.Restrict);

            // ===== HR Recruitment =====

            modelBuilder.Entity<HrRequisition>()
                .ToTable("HR_Requisition");

            modelBuilder.Entity<HrRequisition>()
                .HasIndex(r => r.RequisitionNo)
                .IsUnique()
                .HasFilter("[RequisitionNo] IS NOT NULL")
                .HasDatabaseName("IX_HR_Requisition_RequisitionNo_Unique");

            modelBuilder.Entity<HrRequisitionType>()
                .ToTable("HR_RequisitionType");

            modelBuilder.Entity<HrRequisitionSkill>()
                .ToTable("HR_RequisitionSkill");

            modelBuilder.Entity<HrRequisitionSkill>()
                .HasOne(s => s.Requisition)
                .WithMany(r => r.Skills)
                .HasForeignKey(s => s.RequisitionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<HrRequisitionOffering>()
                .ToTable("HR_RequisitionOffering");

            modelBuilder.Entity<HrRequisitionOffering>()
                .HasOne(o => o.Requisition)
                .WithMany(r => r.Offerings)
                .HasForeignKey(o => o.RequisitionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<HrRecruitmentUnit>()
                .ToTable("HR_RecruitmentUnit");

            modelBuilder.Entity<HrRecruitmentUnit>()
                .Property(u => u.BusinessDepartmentId)
                .HasColumnName("BusinessDepartmentId")
                .HasColumnType("int");

            modelBuilder.Entity<HrRecruitmentUnit>()
                .Property(u => u.RecruitmentDepartmentId)
                .HasColumnName("RecruitmentDepartmentId")
                .HasColumnType("int");

            modelBuilder.Entity<HrRecruitmentUnitDepartment>()
                .ToTable("HR_RecruitmentUnitDepartment");

            modelBuilder.Entity<HrRecruitmentTeam>()
                .ToTable("HR_RecruitmentTeam");

            modelBuilder.Entity<HrWorkflowDefinition>()
                .ToTable("HR_WorkflowDefinition");

            modelBuilder.Entity<HrWorkflowStep>()
                .ToTable("HR_WorkflowStep");

            modelBuilder.Entity<HrWorkflowStep>()
                .HasOne(s => s.WorkflowDefinition)
                .WithMany(d => d.Steps)
                .HasForeignKey(s => s.WorkflowDefinitionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<HrWorkflowStep>()
                .HasIndex(s => new { s.WorkflowDefinitionId, s.StepOrder })
                .IsUnique()
                .HasDatabaseName("IX_HR_WorkflowStep_Definition_Order");

            modelBuilder.Entity<HrDepartmentApprover>()
                .ToTable("HR_DepartmentApprover");

            modelBuilder.Entity<HrDepartmentApprover>()
                .HasIndex(a => new { a.DepartmentId, a.ApproverType, a.IsActive })
                .HasDatabaseName("IX_HR_DepartmentApprover_Department_Type_Active");

            modelBuilder.Entity<HrWorkflowInstance>()
                .ToTable("HR_WorkflowInstance");

            modelBuilder.Entity<HrWorkflowInstance>()
                .HasOne(i => i.WorkflowDefinition)
                .WithMany()
                .HasForeignKey(i => i.WorkflowDefinitionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<HrWorkflowInstance>()
                .HasOne(i => i.CurrentStep)
                .WithMany()
                .HasForeignKey(i => i.CurrentStepId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<HrWorkflowInstance>()
                .HasIndex(i => new { i.EntityType, i.EntityId })
                .HasDatabaseName("IX_HR_WorkflowInstance_Entity");

            modelBuilder.Entity<HrWorkflowInstanceStep>()
                .ToTable("HR_WorkflowInstanceStep");

            modelBuilder.Entity<HrWorkflowInstanceStep>()
                .HasOne(s => s.WorkflowInstance)
                .WithMany(i => i.InstanceSteps)
                .HasForeignKey(s => s.WorkflowInstanceId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<HrWorkflowInstanceStep>()
                .HasOne(s => s.WorkflowStep)
                .WithMany()
                .HasForeignKey(s => s.WorkflowStepId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<HrWorkflowInstanceStep>()
                .HasIndex(s => new { s.WorkflowInstanceId, s.StepOrder })
                .IsUnique()
                .HasDatabaseName("IX_HR_WorkflowInstanceStep_Instance_Order");

            modelBuilder.Entity<HrWorkflowInstanceStep>()
                .HasIndex(s => new { s.EmployeeId, s.Status })
                .HasDatabaseName("IX_HR_WorkflowInstanceStep_Employee_Status");

            modelBuilder.Entity<HrWorkflowAction>()
                .ToTable("HR_WorkflowAction");

            modelBuilder.Entity<HrWorkflowAction>()
                .HasOne(a => a.WorkflowInstanceStep)
                .WithMany(s => s.Actions)
                .HasForeignKey(a => a.WorkflowInstanceStepId)
                .OnDelete(DeleteBehavior.Restrict);

            // ===== HR Sprint 3: Assignment, Job Posting, Channels =====

            modelBuilder.Entity<HrRequisitionAssignment>()
                .ToTable("HR_RequisitionAssignment");

            modelBuilder.Entity<HrRequisitionAssignment>()
                .HasIndex(a => a.RequisitionFk)
                .IsUnique()
                .HasFilter("[RequisitionFk] IS NOT NULL")
                .HasDatabaseName("IX_HR_RequisitionAssignment_Requisition_Unique");

            modelBuilder.Entity<HrJobPosting>()
                .ToTable("HR_JobPosting");

            modelBuilder.Entity<HrJobPosting>()
                .HasIndex(p => p.JobCode)
                .IsUnique()
                .HasFilter("[JobCode] IS NOT NULL")
                .HasDatabaseName("IX_HR_JobPosting_JobCode_Unique");

            // ===== HR Sprint 4: ATS =====

            modelBuilder.Entity<HrCandidate>()
                .ToTable("HR_Candidate");

            modelBuilder.Entity<HrCandidate>()
                .HasIndex(c => c.Email)
                .IsUnique()
                .HasFilter("[IsDeleted] = 0")
                .HasDatabaseName("IX_HR_Candidate_Email_Unique");

            modelBuilder.Entity<HrCandidateDocument>()
                .ToTable("HR_CandidateDocument");

            modelBuilder.Entity<HrCandidateDocument>()
                .HasOne(d => d.Candidate)
                .WithMany(c => c.Documents)
                .HasForeignKey(d => d.CandidateFk)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<HrApplicationStage>()
                .ToTable("HR_ApplicationStage");

            modelBuilder.Entity<HrJobApplication>()
                .ToTable("HR_JobApplication");

            modelBuilder.Entity<HrJobApplication>()
                .HasOne(a => a.Candidate)
                .WithMany(c => c.Applications)
                .HasForeignKey(a => a.CandidateFK)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<HrJobApplication>()
                .HasOne(a => a.JobPosting)
                .WithMany()
                .HasForeignKey(a => a.JobPostingFK)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<HrJobApplication>()
                .HasOne(a => a.CurrentStage)
                .WithMany()
                .HasForeignKey(a => a.CurrentStageFK)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<HrJobApplication>()
                .HasIndex(a => new { a.CandidateFK, a.JobPostingFK })
                .HasFilter("[IsActive] = 1")
                .HasDatabaseName("IX_HR_JobApplication_Candidate_Posting_Active");

            modelBuilder.Entity<HrApplicationStageHistory>()
                .ToTable("HR_ApplicationStageHistory");

            modelBuilder.Entity<HrApplicationStageHistory>()
                .HasOne(h => h.JobApplication)
                .WithMany(a => a.StageHistory)
                .HasForeignKey(h => h.JobApplicationFk)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<HrApplicationStageHistory>()
                .HasOne(h => h.FromStage)
                .WithMany()
                .HasForeignKey(h => h.FromStageId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<HrApplicationStageHistory>()
                .HasOne(h => h.ToStage)
                .WithMany()
                .HasForeignKey(h => h.ToStageFk)
                .OnDelete(DeleteBehavior.Restrict);

            // ===== HR Sprint 6: Offer Management =====

            modelBuilder.Entity<HrOffer>()
                .ToTable("HR_Offer");

            modelBuilder.Entity<HrOffer>()
                .HasOne(o => o.JobApplication)
                .WithMany()
                .HasForeignKey(o => o.JobApplicationFk)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<HrOffer>()
                .HasIndex(o => o.OfferNumber)
                .IsUnique()
                .HasFilter("[OfferNumber] IS NOT NULL")
                .HasDatabaseName("IX_HR_Offer_OfferNumber_Unique");

            modelBuilder.Entity<HrOfferVersion>()
                .ToTable("HR_OfferVersion");

            modelBuilder.Entity<HrOfferVersion>()
                .HasOne(v => v.Offer)
                .WithMany(o => o.Versions)
                .HasForeignKey(v => v.OfferFk)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<HrOfferApproval>()
                .ToTable("HR_OfferApproval");

            modelBuilder.Entity<HrOfferApproval>()
                .HasOne(a => a.Offer)
                .WithMany(o => o.Approvals)
                .HasForeignKey(a => a.OfferFk)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<HrOfferDocument>()
                .ToTable("HR_OfferDocument");

            modelBuilder.Entity<HrOfferDocument>()
                .HasOne(d => d.Offer)
                .WithMany(o => o.Documents)
                .HasForeignKey(d => d.OfferFk)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<HrHiringDecision>()
                .ToTable("HR_HiringDecision");

            modelBuilder.Entity<HrHiringDecision>()
                .HasOne(h => h.JobApplication)
                .WithMany()
                .HasForeignKey(h => h.JobApplicationFk)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<HrHiringDecision>()
                .HasIndex(h => h.JobApplicationFk)
                .IsUnique()
                .HasFilter("[JobApplicationFk] IS NOT NULL")
                .HasDatabaseName("IX_HR_HiringDecision_Application_Unique");

            // ===== HR Sprint 5: Interview Management =====

            modelBuilder.Entity<HrInterviewRound>()
                .ToTable("HR_InterviewRound");

            modelBuilder.Entity<HrInterviewRound>()
                .HasIndex(r => new { r.JobPostingId, r.RoundOrder })
                .IsUnique()
                .HasDatabaseName("IX_HR_InterviewRound_Posting_Order");

            modelBuilder.Entity<HrInterviewSchedule>()
                .ToTable("HR_InterviewSchedule");

            modelBuilder.Entity<HrInterviewSchedule>()
                .HasOne(s => s.JobApplication)
                .WithMany()
                .HasForeignKey(s => s.JobApplicationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<HrInterviewSchedule>()
                .HasOne(s => s.InterviewRound)
                .WithMany(r => r.Schedules)
                .HasForeignKey(s => s.InterviewRoundId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<HrInterviewPanel>()
                .ToTable("HR_InterviewPanel");

            modelBuilder.Entity<HrInterviewPanel>()
                .HasOne(p => p.InterviewSchedule)
                .WithMany(s => s.Panel)
                .HasForeignKey(p => p.InterviewScheduleId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<HrInterviewPanel>()
                .HasIndex(p => new { p.InterviewScheduleId, p.EmployeeFk })
                .IsUnique()
                .HasDatabaseName("IX_HR_InterviewPanel_Schedule_Employee");

            modelBuilder.Entity<HrInterviewFeedback>()
                .ToTable("HR_InterviewFeedback");

            modelBuilder.Entity<HrInterviewFeedback>()
                .HasOne(f => f.InterviewSchedule)
                .WithMany(s => s.Feedback)
                .HasForeignKey(f => f.InterviewScheduleFk)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<HrInterviewFeedback>()
                .HasIndex(f => new { f.InterviewScheduleFk, f.InterviewerEmployeeFk })
                .IsUnique()
                .HasDatabaseName("IX_HR_InterviewFeedback_Schedule_Interviewer");

            modelBuilder.Entity<HrEvaluationCriteria>()
                .ToTable("HR_EvaluationCriteria");

            modelBuilder.Entity<HrEvaluationCriteria>()
                .HasOne(c => c.Round)
                .WithMany(r => r.Criteria)
                .HasForeignKey(c => c.InterviewRoundId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<HrEvaluationScore>()
                .ToTable("HR_EvaluationScore");

            modelBuilder.Entity<HrEvaluationScore>()
                .HasOne(s => s.InterviewFeedback)
                .WithMany(f => f.Scores)
                .HasForeignKey(s => s.InterviewFeedbackFk)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<HrEvaluationScore>()
                .HasOne(s => s.EvaluationCriteria)
                .WithMany(c => c.Scores)
                .HasForeignKey(s => s.EvaluationCriteriaFk)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<HrEvaluationScore>()
                .HasIndex(s => new { s.InterviewFeedbackFk, s.EvaluationCriteriaFk })
                .IsUnique()
                .HasDatabaseName("IX_HR_EvaluationScore_Feedback_Criteria");

            // ===== HR Sprint 7: Onboarding =====

            modelBuilder.Entity<HrOnboardingTaskTemplate>()
                .ToTable("HR_OnboardingTaskTemplate");

            modelBuilder.Entity<HrOnboarding>()
                .ToTable("HR_Onboarding");

            modelBuilder.Entity<HrOnboarding>()
                .HasOne(o => o.Candidate)
                .WithMany()
                .HasForeignKey(o => o.CandidateId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<HrOnboarding>()
                .HasOne(o => o.Offer)
                .WithMany()
                .HasForeignKey(o => o.OfferId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<HrOnboarding>()
                .HasIndex(o => o.OfferId)
                .IsUnique()
                .HasDatabaseName("IX_HR_Onboarding_Offer_Unique");

            modelBuilder.Entity<HrOnboardingTask>()
                .ToTable("HR_OnboardingTask");

            modelBuilder.Entity<HrOnboardingTask>()
                .HasOne(t => t.Onboarding)
                .WithMany(o => o.Tasks)
                .HasForeignKey(t => t.OnboardingFk)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<HrOnboardingTask>()
                .HasOne(t => t.TaskTemplate)
                .WithMany()
                .HasForeignKey(t => t.TaskTemplateFk)
                .OnDelete(DeleteBehavior.Restrict);


            // Enum to String Conversion for SalarySlipDetail
            modelBuilder.Entity<SalarySlipDetail>()
                .Property(e => e.Type)
                .HasConversion<string>();

            // ===== Document Management =====

            modelBuilder.Entity<DocumentType>()
                .HasOne(dt => dt.Category)
                .WithMany(c => c.DocumentTypes)
                .HasForeignKey(dt => dt.CategoryID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Document>()
                .HasOne(d => d.Employee)
                .WithMany()
                .HasForeignKey(d => d.EmployeeID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Document>()
                .HasOne(d => d.DocumentType)
                .WithMany(dt => dt.Documents)
                .HasForeignKey(d => d.DocumentTypeID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Document>()
                .HasOne(d => d.Category)
                .WithMany(c => c.Documents)
                .HasForeignKey(d => d.CategoryID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Document>()
                .HasOne(d => d.UploadedByUser)
                .WithMany()
                .HasForeignKey(d => d.UploadedBy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Document>()
                .HasIndex(d => d.DocumentNo)
                .IsUnique()
                .HasDatabaseName("IX_Documents_DocumentNo_Unique");

            // ===== DMS Definition File =====
            modelBuilder.Entity<DmsDefinationFile>()
                .HasOne(d => d.Category)
                .WithMany()
                .HasForeignKey(d => d.CategoryID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DmsDefinationFile>()
                .HasOne(d => d.DocumentType)
                .WithMany()
                .HasForeignKey(d => d.DocumentTypeID)
                .OnDelete(DeleteBehavior.Restrict);

            // ===== User DMS Linkage =====
            modelBuilder.Entity<User>()
                .HasOne(u => u.DmsDefinationFile)
                .WithMany()
                .HasForeignKey(u => u.DmsDefinationFK)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
