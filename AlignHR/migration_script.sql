IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE TABLE [AttendencePolicies] (
        [id] int NOT NULL IDENTITY,
        [LateGraceTime] int NOT NULL,
        [OtAfterMintues] int NOT NULL,
        [MaxOtPerDay] int NOT NULL,
        [WeeklyHourLimit] int NOT NULL,
        [EffictiveTime] date NOT NULL,
        [IsFlexiable] bit NOT NULL,
        [createdby] int NOT NULL,
        [createat] datetime2 NOT NULL,
        [updatedby] int NOT NULL,
        [updateat] datetime2 NOT NULL,
        CONSTRAINT [PK_AttendencePolicies] PRIMARY KEY ([id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE TABLE [department] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(max) NOT NULL,
        [Code] nvarchar(max) NOT NULL,
        [Description] nvarchar(max) NULL,
        [Owner] nvarchar(max) NOT NULL,
        [Role] nvarchar(max) NULL,
        [createdby] int NOT NULL,
        [createat] datetime2 NOT NULL,
        [updatedby] int NOT NULL,
        [updateat] datetime2 NOT NULL,
        CONSTRAINT [PK_department] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE TABLE [FiscalYear] (
        [id] int NOT NULL IDENTITY,
        [title] nvarchar(max) NOT NULL,
        [startdate] date NOT NULL,
        [enddate] date NOT NULL,
        [createdby] int NOT NULL,
        [createat] datetime2 NOT NULL,
        [updatedby] int NOT NULL,
        [updateat] datetime2 NOT NULL,
        CONSTRAINT [PK_FiscalYear] PRIMARY KEY ([id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE TABLE [functions] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(150) NOT NULL,
        [Description] nvarchar(250) NULL,
        [route] nvarchar(500) NOT NULL,
        [createdby] int NOT NULL,
        [createat] datetime2 NOT NULL,
        [updatedby] int NOT NULL,
        [updateat] datetime2 NOT NULL,
        CONSTRAINT [PK_functions] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE TABLE [LeaveConfigurations] (
        [Id] numeric(18,0) NOT NULL IDENTITY,
        [IsDepartment] bit NOT NULL,
        [IsSubDepartment] bit NOT NULL,
        [IsGrade] bit NOT NULL,
        [IsDivision] bit NOT NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_LeaveConfigurations] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE TABLE [LeavePolicy] (
        [Id] numeric(18,0) NOT NULL IDENTITY,
        [PolicyName] nvarchar(150) NOT NULL,
        [Description] nvarchar(500) NULL,
        [EffectiveFrom] date NOT NULL,
        [EffectiveTo] date NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] nvarchar(50) NULL,
        [UpdatedBy] nvarchar(50) NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_LeavePolicy] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE TABLE [LeaveTypes] (
        [Id] numeric(18,0) NOT NULL IDENTITY,
        [Name] nvarchar(100) NOT NULL,
        [Code] nvarchar(20) NOT NULL,
        [IsPaid] bit NOT NULL,
        [RequiresAttachment] bit NOT NULL,
        [ColorCode] nvarchar(20) NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] nvarchar(50) NULL,
        [UpdatedBy] nvarchar(50) NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_LeaveTypes] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE TABLE [location] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(max) NOT NULL,
        [Adress] nvarchar(max) NOT NULL,
        [Latitude] float NOT NULL,
        [Longitude] float NOT NULL,
        [createdby] int NOT NULL,
        [createat] datetime2 NOT NULL,
        [updatedby] int NOT NULL,
        [updateat] datetime2 NOT NULL,
        CONSTRAINT [PK_location] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE TABLE [Overtimes] (
        [OvertimeId] int NOT NULL IDENTITY,
        [PolicyName] nvarchar(100) NOT NULL,
        [DayType] nvarchar(20) NOT NULL,
        [MinHours] decimal(5,2) NOT NULL,
        [MaxHours] decimal(5,2) NOT NULL,
        [RateType] nvarchar(20) NOT NULL,
        [RateValue] decimal(5,2) NOT NULL,
        [CalculationBase] nvarchar(20) NOT NULL,
        [AfterShiftMinutes] int NOT NULL,
        [RoundType] nvarchar(20) NOT NULL,
        [RoundValue] int NOT NULL,
        [IsTaxable] bit NOT NULL,
        [IsActive] bit NOT NULL,
        [IsDefault] bit NOT NULL,
        [EffectiveFrom] date NOT NULL,
        [EffectiveTo] date NULL,
        [CreatedOn] datetime2 NULL,
        [CreatedBy] int NULL,
        [UpdatedOn] datetime2 NULL,
        [UpdatedBy] int NULL,
        CONSTRAINT [PK_Overtimes] PRIMARY KEY ([OvertimeId])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE TABLE [roles] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(max) NOT NULL,
        [Description] nvarchar(max) NOT NULL,
        [createdby] int NOT NULL,
        [createat] datetime2 NOT NULL,
        [updatedby] int NOT NULL,
        [updateat] datetime2 NOT NULL,
        CONSTRAINT [PK_roles] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE TABLE [SalaryPeriods] (
        [SalaryPeriodID] int NOT NULL IDENTITY,
        [PeriodName] nvarchar(50) NOT NULL,
        [StartDate] datetime2 NOT NULL,
        [EndDate] datetime2 NOT NULL,
        [IsActive] bit NOT NULL,
        [IsProcessed] bit NOT NULL,
        [IsPostedToGL] bit NOT NULL,
        [EOBIAmount] decimal(18,2) NULL,
        [PFPercentage] decimal(18,2) NOT NULL,
        [createdby] int NOT NULL,
        [createat] datetime2 NOT NULL,
        [updatedby] int NOT NULL,
        [updateat] datetime2 NOT NULL,
        CONSTRAINT [PK_SalaryPeriods] PRIMARY KEY ([SalaryPeriodID])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE TABLE [shifts] (
        [Id] int NOT NULL IDENTITY,
        [ShiftName] nvarchar(100) NOT NULL,
        [GroupName] int NULL,
        [StartTime] time NOT NULL,
        [EndTime] time NOT NULL,
        [GraceTime] time NOT NULL,
        [BreakStartTime] time NULL,
        [BreakEndTime] time NULL,
        [IsNightShift] bit NULL,
        [createdby] int NOT NULL,
        [createat] datetime2 NOT NULL,
        [updatedby] int NOT NULL,
        [updateat] datetime2 NOT NULL,
        CONSTRAINT [PK_shifts] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE TABLE [valuesets] (
        [Id] int NOT NULL IDENTITY,
        [GroupName] nvarchar(max) NOT NULL,
        [Name] nvarchar(max) NOT NULL,
        [IsActive] bit NOT NULL,
        CONSTRAINT [PK_valuesets] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE TABLE [TaxSlabMaster] (
        [id] int NOT NULL IDENTITY,
        [FiscalFK] int NOT NULL,
        [IncomeFrom] decimal(18,2) NOT NULL,
        [IncomeTo] decimal(18,2) NULL,
        [FixedTax] decimal(18,2) NOT NULL,
        [RatePercent] decimal(5,2) NOT NULL,
        [IsActive] bit NOT NULL,
        [createdby] int NOT NULL,
        [createat] datetime2 NOT NULL,
        [updatedby] int NOT NULL,
        [updateat] datetime2 NOT NULL,
        CONSTRAINT [PK_TaxSlabMaster] PRIMARY KEY ([id]),
        CONSTRAINT [FK_TaxSlabMaster_FiscalYear_FiscalFK] FOREIGN KEY ([FiscalFK]) REFERENCES [FiscalYear] ([id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE TABLE [TaxSurcharge] (
        [id] int NOT NULL IDENTITY,
        [FiscalFK] int NOT NULL,
        [IncomeThreshold] decimal(18,2) NOT NULL,
        [RatePercent] decimal(5,2) NOT NULL,
        [IncomeFrom] decimal(18,2) NOT NULL,
        [IncomeTo] decimal(18,2) NULL,
        [IsActive] bit NOT NULL,
        [createdby] int NOT NULL,
        [createat] datetime2 NOT NULL,
        [updatedby] int NOT NULL,
        [updateat] datetime2 NOT NULL,
        CONSTRAINT [PK_TaxSurcharge] PRIMARY KEY ([id]),
        CONSTRAINT [FK_TaxSurcharge_FiscalYear_FiscalFK] FOREIGN KEY ([FiscalFK]) REFERENCES [FiscalYear] ([id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE TABLE [LeaveConfigurationDetails] (
        [Id] numeric(18,0) NOT NULL IDENTITY,
        [LeaveConfigurationId] numeric(18,0) NOT NULL,
        [ReferenceId] int NOT NULL,
        [ReferenceName] nvarchar(200) NULL,
        [ApproverEmpNo] nvarchar(50) NOT NULL,
        [ApproverName] nvarchar(200) NULL,
        CONSTRAINT [PK_LeaveConfigurationDetails] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_LeaveConfigurationDetails_LeaveConfigurations_LeaveConfigurationId] FOREIGN KEY ([LeaveConfigurationId]) REFERENCES [LeaveConfigurations] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE TABLE [LeavePolicyRules] (
        [Id] numeric(18,0) NOT NULL IDENTITY,
        [LeavePolicyId] numeric(18,0) NOT NULL,
        [LeaveTypeId] numeric(18,0) NOT NULL,
        [AnnualQuota] decimal(5,2) NOT NULL,
        [MonthlyAccrual] decimal(5,2) NOT NULL,
        [CarryForwardLimit] decimal(5,2) NOT NULL,
        [IsEncashable] bit NOT NULL,
        [AllowNegativeBalance] bit NOT NULL,
        [AllowHalfDay] bit NOT NULL,
        [AllowDuringProbation] bit NOT NULL,
        [MaxConsecutiveDays] decimal(5,2) NOT NULL,
        [MinNoticeDays] decimal(5,2) NOT NULL,
        [ApplySandwichRule] bit NOT NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] nvarchar(50) NULL,
        [UpdatedBy] nvarchar(50) NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_LeavePolicyRules] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_LeavePolicyRules_LeavePolicy_LeavePolicyId] FOREIGN KEY ([LeavePolicyId]) REFERENCES [LeavePolicy] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_LeavePolicyRules_LeaveTypes_LeaveTypeId] FOREIGN KEY ([LeaveTypeId]) REFERENCES [LeaveTypes] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE TABLE [FunctionRoles] (
        [Id] int NOT NULL IDENTITY,
        [FunctionId] int NOT NULL,
        [RoleId] int NOT NULL,
        [createdby] int NOT NULL,
        [createat] datetime2 NOT NULL,
        CONSTRAINT [PK_FunctionRoles] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_FunctionRoles_functions_FunctionId] FOREIGN KEY ([FunctionId]) REFERENCES [functions] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_FunctionRoles_roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [roles] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE TABLE [emp] (
        [Id] int NOT NULL IDENTITY,
        [Code] nvarchar(450) NOT NULL,
        [FirstName] nvarchar(max) NOT NULL,
        [LastName] nvarchar(max) NOT NULL,
        [ProfileImage] nvarchar(max) NULL,
        [DesiginationId] int NULL,
        [Dateofjoin] date NOT NULL,
        [createdby] int NOT NULL,
        [createat] datetime2 NOT NULL,
        [updatedby] int NOT NULL,
        [updateat] datetime2 NOT NULL,
        [EmploymentTypeFk] int NULL,
        [EmploymentStatusFk] int NULL,
        [DepartmentFk] int NOT NULL,
        [LocationFk] int NOT NULL,
        [BankHolderName] nvarchar(max) NULL,
        [BankNameFk] int NULL,
        [AccountNumber] nvarchar(max) NULL,
        [NTN] nvarchar(50) NOT NULL,
        [IsFiler] bit NOT NULL,
        [TaxExempted] bit NOT NULL,
        [ZakatDeduction] bit NOT NULL,
        [IsServiceEnded] bit NOT NULL,
        [ServiceEndDate] datetime2 NULL,
        [IsSalaryStopped] bit NOT NULL,
        [SalaryStatus] nvarchar(max) NOT NULL,
        [EobiNumber] int NULL,
        [isEobi] bit NOT NULL,
        [SocailSecuirty] int NULL,
        [isSocailSecuirty] bit NOT NULL,
        [isOverTime] bit NOT NULL,
        [OvertimePolicyId] int NULL,
        [isPfMember] bit NOT NULL,
        [pfmembernumber] int NULL,
        [PFAmount] decimal(18,2) NULL,
        [IsFixedPFAmount] bit NOT NULL,
        [LineManagerEmpNo] nvarchar(50) NULL,
        [LeaveApproverEmpNo] nvarchar(50) NULL,
        [SubDepartmentFk] int NULL,
        [GradeFk] int NULL,
        [DivisionFk] int NULL,
        CONSTRAINT [PK_emp] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_emp_Overtimes_OvertimePolicyId] FOREIGN KEY ([OvertimePolicyId]) REFERENCES [Overtimes] ([OvertimeId]) ON DELETE NO ACTION,
        CONSTRAINT [FK_emp_department_DepartmentFk] FOREIGN KEY ([DepartmentFk]) REFERENCES [department] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_emp_location_LocationFk] FOREIGN KEY ([LocationFk]) REFERENCES [location] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_emp_valuesets_BankNameFk] FOREIGN KEY ([BankNameFk]) REFERENCES [valuesets] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_emp_valuesets_DesiginationId] FOREIGN KEY ([DesiginationId]) REFERENCES [valuesets] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_emp_valuesets_DivisionFk] FOREIGN KEY ([DivisionFk]) REFERENCES [valuesets] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_emp_valuesets_EmploymentStatusFk] FOREIGN KEY ([EmploymentStatusFk]) REFERENCES [valuesets] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_emp_valuesets_EmploymentTypeFk] FOREIGN KEY ([EmploymentTypeFk]) REFERENCES [valuesets] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_emp_valuesets_GradeFk] FOREIGN KEY ([GradeFk]) REFERENCES [valuesets] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_emp_valuesets_SubDepartmentFk] FOREIGN KEY ([SubDepartmentFk]) REFERENCES [valuesets] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE TABLE [PayRollDefinationFile] (
        [Id] int NOT NULL IDENTITY,
        [DefinitionName] nvarchar(max) NULL,
        [PayRollComFK] int NOT NULL,
        [label] nvarchar(max) NULL,
        [Field] nvarchar(max) NULL,
        [Percentage] nvarchar(max) NULL,
        [createdby] int NOT NULL,
        [createat] datetime2 NOT NULL,
        [updatedby] int NOT NULL,
        [updateat] datetime2 NOT NULL,
        CONSTRAINT [PK_PayRollDefinationFile] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_PayRollDefinationFile_valuesets_PayRollComFK] FOREIGN KEY ([PayRollComFK]) REFERENCES [valuesets] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE TABLE [AttendenceException] (
        [id] int NOT NULL IDENTITY,
        [AttendeceDate] date NOT NULL,
        [Remarks] nvarchar(max) NOT NULL,
        [Status] nvarchar(max) NOT NULL,
        [EmployeeId] int NOT NULL,
        [ExpectionTypeFk] int NULL,
        [ExpectionStatusFk] int NULL,
        [createdby] int NOT NULL,
        [createat] datetime2 NOT NULL,
        [updatedby] int NOT NULL,
        [updateat] datetime2 NOT NULL,
        CONSTRAINT [PK_AttendenceException] PRIMARY KEY ([id]),
        CONSTRAINT [FK_AttendenceException_emp_EmployeeId] FOREIGN KEY ([EmployeeId]) REFERENCES [emp] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_AttendenceException_valuesets_ExpectionStatusFk] FOREIGN KEY ([ExpectionStatusFk]) REFERENCES [valuesets] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_AttendenceException_valuesets_ExpectionTypeFk] FOREIGN KEY ([ExpectionTypeFk]) REFERENCES [valuesets] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE TABLE [AttendenceLogs] (
        [id] int NOT NULL IDENTITY,
        [EmployeeID] int NOT NULL,
        [AttendenceDate] date NOT NULL,
        [PunchTime] datetime2 NOT NULL,
        [PunchType] int NULL,
        [Source] nvarchar(max) NOT NULL,
        [Devicesd] nvarchar(max) NULL,
        [latitude] decimal(18,10) NOT NULL,
        [longitude] decimal(18,10) NOT NULL,
        [shiftidFk] int NULL,
        [createdby] int NOT NULL,
        [createat] datetime2 NOT NULL,
        [updatedby] int NOT NULL,
        [updateat] datetime2 NOT NULL,
        CONSTRAINT [PK_AttendenceLogs] PRIMARY KEY ([id]),
        CONSTRAINT [FK_AttendenceLogs_emp_EmployeeID] FOREIGN KEY ([EmployeeID]) REFERENCES [emp] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_AttendenceLogs_shifts_shiftidFk] FOREIGN KEY ([shiftidFk]) REFERENCES [shifts] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE TABLE [Bonuses] (
        [Id] int NOT NULL IDENTITY,
        [EmployeeID] int NOT NULL,
        [SalaryPeriodID] int NOT NULL,
        [BonusTypeID] int NOT NULL,
        [BonusPercentage] decimal(18,2) NOT NULL,
        [IsPercentage] bit NOT NULL,
        [BonusAmount] decimal(18,2) NOT NULL,
        [TaxDeduction] decimal(18,2) NOT NULL,
        [NetBonus] decimal(18,2) NOT NULL,
        [PaymentDate] datetime2 NOT NULL,
        [ApprovalStatus] nvarchar(max) NOT NULL,
        [Remarks] nvarchar(max) NULL,
        [createdby] int NOT NULL,
        [createat] datetime2 NOT NULL,
        [updatedby] int NULL,
        [updateat] datetime2 NULL,
        CONSTRAINT [PK_Bonuses] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Bonuses_SalaryPeriods_SalaryPeriodID] FOREIGN KEY ([SalaryPeriodID]) REFERENCES [SalaryPeriods] ([SalaryPeriodID]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Bonuses_emp_EmployeeID] FOREIGN KEY ([EmployeeID]) REFERENCES [emp] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Bonuses_valuesets_BonusTypeID] FOREIGN KEY ([BonusTypeID]) REFERENCES [valuesets] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE TABLE [EmployeeLeavePolicies] (
        [Id] numeric(18,0) NOT NULL IDENTITY,
        [EmployeeId] int NOT NULL,
        [LeavePolicyId] numeric(18,0) NOT NULL,
        [AssignedAt] datetime2 NOT NULL,
        [IsActive] bit NOT NULL,
        CONSTRAINT [PK_EmployeeLeavePolicies] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_EmployeeLeavePolicies_LeavePolicy_LeavePolicyId] FOREIGN KEY ([LeavePolicyId]) REFERENCES [LeavePolicy] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_EmployeeLeavePolicies_emp_EmployeeId] FOREIGN KEY ([EmployeeId]) REFERENCES [emp] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE TABLE [EmployeeShifts] (
        [id] int NOT NULL IDENTITY,
        [EffiectiveForm] date NOT NULL,
        [EffiectiveTo] date NULL,
        [RestDay] int NOT NULL,
        [createdby] int NOT NULL,
        [createat] datetime2 NOT NULL,
        [updatedby] int NOT NULL,
        [updateat] datetime2 NOT NULL,
        [EmployeeId] int NOT NULL,
        [ShiftId] int NOT NULL,
        CONSTRAINT [PK_EmployeeShifts] PRIMARY KEY ([id]),
        CONSTRAINT [FK_EmployeeShifts_emp_EmployeeId] FOREIGN KEY ([EmployeeId]) REFERENCES [emp] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_EmployeeShifts_shifts_ShiftId] FOREIGN KEY ([ShiftId]) REFERENCES [shifts] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE TABLE [EOBIDetuction] (
        [id] int NOT NULL IDENTITY,
        [PeriodSalery] int NOT NULL,
        [EmployeeId] int NOT NULL,
        [Amount] decimal(18,2) NULL,
        [createdby] int NOT NULL,
        [createat] datetime2 NOT NULL,
        [updatedby] int NOT NULL,
        [updateat] datetime2 NOT NULL,
        CONSTRAINT [PK_EOBIDetuction] PRIMARY KEY ([id]),
        CONSTRAINT [FK_EOBIDetuction_SalaryPeriods_PeriodSalery] FOREIGN KEY ([PeriodSalery]) REFERENCES [SalaryPeriods] ([SalaryPeriodID]) ON DELETE NO ACTION,
        CONSTRAINT [FK_EOBIDetuction_emp_EmployeeId] FOREIGN KEY ([EmployeeId]) REFERENCES [emp] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE TABLE [IncomeTaxDetuction] (
        [id] int NOT NULL IDENTITY,
        [incometaxdetection] decimal(18,2) NOT NULL,
        [PeriodSalery] int NOT NULL,
        [EmployeeId] int NOT NULL,
        [createdby] int NOT NULL,
        [createat] datetime2 NOT NULL,
        [updatedby] int NOT NULL,
        [updateat] datetime2 NOT NULL,
        CONSTRAINT [PK_IncomeTaxDetuction] PRIMARY KEY ([id]),
        CONSTRAINT [FK_IncomeTaxDetuction_SalaryPeriods_PeriodSalery] FOREIGN KEY ([PeriodSalery]) REFERENCES [SalaryPeriods] ([SalaryPeriodID]) ON DELETE NO ACTION,
        CONSTRAINT [FK_IncomeTaxDetuction_emp_EmployeeId] FOREIGN KEY ([EmployeeId]) REFERENCES [emp] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE TABLE [LeaveBalances] (
        [Id] numeric(18,0) NOT NULL IDENTITY,
        [EmployeeId] int NOT NULL,
        [LeaveTypeId] numeric(18,0) NOT NULL,
        [Year] int NOT NULL,
        [Allocated] decimal(5,2) NOT NULL,
        [Used] decimal(5,2) NOT NULL,
        [Pending] decimal(5,2) NOT NULL,
        [CarriedForward] decimal(5,2) NOT NULL,
        [Available] decimal(5,2) NOT NULL,
        [UpdatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_LeaveBalances] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_LeaveBalances_LeaveTypes_LeaveTypeId] FOREIGN KEY ([LeaveTypeId]) REFERENCES [LeaveTypes] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_LeaveBalances_emp_EmployeeId] FOREIGN KEY ([EmployeeId]) REFERENCES [emp] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE TABLE [LeaveOpenings] (
        [Id] numeric(18,0) NOT NULL IDENTITY,
        [EmployeeFk] int NOT NULL,
        [LeaveTypeFk] numeric(18,0) NOT NULL,
        [Year] int NOT NULL,
        [OpeningBalance] decimal(5,2) NOT NULL,
        [Narration] nvarchar(500) NULL,
        [CreatedBy] int NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_LeaveOpenings] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_LeaveOpenings_LeaveTypes_LeaveTypeFk] FOREIGN KEY ([LeaveTypeFk]) REFERENCES [LeaveTypes] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_LeaveOpenings_emp_EmployeeFk] FOREIGN KEY ([EmployeeFk]) REFERENCES [emp] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE TABLE [LeaveRequests] (
        [Id] numeric(18,0) NOT NULL IDENTITY,
        [EmployeeId] int NOT NULL,
        [LeaveTypeId] numeric(18,0) NOT NULL,
        [FromDate] date NOT NULL,
        [ToDate] date NOT NULL,
        [DaysCount] decimal(5,2) NOT NULL,
        [IsHalfDay] bit NOT NULL,
        [HalfDaySession] nvarchar(20) NULL,
        [Reason] nvarchar(500) NULL,
        [AttachmentUrl] nvarchar(max) NULL,
        [Status] nvarchar(20) NOT NULL,
        [CurrentApproverId] int NULL,
        [CurrentStage] int NOT NULL,
        [ApprovalLevel] int NOT NULL,
        [AppliedAt] datetime2 NULL,
        [ApprovedAt] datetime2 NULL,
        [ApprovedByEmpNo] nvarchar(50) NULL,
        [RejectedAt] datetime2 NULL,
        [RejectionReason] nvarchar(500) NULL,
        [RowVersion] rowversion NULL,
        [CreatedAt] datetime2 NULL,
        [CreatedBy] nvarchar(max) NULL,
        CONSTRAINT [PK_LeaveRequests] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_LeaveRequests_LeaveTypes_LeaveTypeId] FOREIGN KEY ([LeaveTypeId]) REFERENCES [LeaveTypes] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_LeaveRequests_emp_CurrentApproverId] FOREIGN KEY ([CurrentApproverId]) REFERENCES [emp] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_LeaveRequests_emp_EmployeeId] FOREIGN KEY ([EmployeeId]) REFERENCES [emp] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE TABLE [MasterLoanAdvance] (
        [id] int NOT NULL IDENTITY,
        [EmployeeID] int NOT NULL,
        [TransactionType] int NOT NULL,
        [RequestDate] datetime2 NOT NULL,
        [TotalAmount] decimal(18,2) NOT NULL,
        [ApprovedAmount] decimal(18,2) NOT NULL,
        [Reason] nvarchar(max) NULL,
        [Status] int NOT NULL,
        [TenureMonths] int NOT NULL,
        [MonthlyInstallment] decimal(18,2) NOT NULL,
        [InterestRate] decimal(18,2) NOT NULL,
        [LoanDetuction] bit NOT NULL,
        [createdby] int NOT NULL,
        [createat] datetime2 NOT NULL,
        [updatedby] int NOT NULL,
        [updateat] datetime2 NOT NULL,
        CONSTRAINT [PK_MasterLoanAdvance] PRIMARY KEY ([id]),
        CONSTRAINT [FK_MasterLoanAdvance_emp_EmployeeID] FOREIGN KEY ([EmployeeID]) REFERENCES [emp] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE TABLE [OverTimeHours] (
        [id] int NOT NULL IDENTITY,
        [EmployeeId] int NOT NULL,
        [SalaryPeriodFK] int NOT NULL,
        [OTHours] nvarchar(max) NOT NULL,
        [Amount] int NOT NULL,
        [createdby] int NOT NULL,
        [createat] datetime2 NOT NULL,
        [updatedby] int NOT NULL,
        [updateat] datetime2 NOT NULL,
        CONSTRAINT [PK_OverTimeHours] PRIMARY KEY ([id]),
        CONSTRAINT [FK_OverTimeHours_SalaryPeriods_SalaryPeriodFK] FOREIGN KEY ([SalaryPeriodFK]) REFERENCES [SalaryPeriods] ([SalaryPeriodID]) ON DELETE NO ACTION,
        CONSTRAINT [FK_OverTimeHours_emp_EmployeeId] FOREIGN KEY ([EmployeeId]) REFERENCES [emp] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE TABLE [PFMembers] (
        [id] int NOT NULL IDENTITY,
        [EmployeeId] int NOT NULL,
        [SalaryPeriodFK] int NOT NULL,
        [Amount] decimal(18,2) NULL,
        [createdby] int NOT NULL,
        [createat] datetime2 NOT NULL,
        [updatedby] int NOT NULL,
        [updateat] datetime2 NOT NULL,
        CONSTRAINT [PK_PFMembers] PRIMARY KEY ([id]),
        CONSTRAINT [FK_PFMembers_SalaryPeriods_SalaryPeriodFK] FOREIGN KEY ([SalaryPeriodFK]) REFERENCES [SalaryPeriods] ([SalaryPeriodID]) ON DELETE NO ACTION,
        CONSTRAINT [FK_PFMembers_emp_EmployeeId] FOREIGN KEY ([EmployeeId]) REFERENCES [emp] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE TABLE [SalaryAdjustments] (
        [Id] int NOT NULL IDENTITY,
        [EmployeeId] int NOT NULL,
        [SalaryPeriodId] int NOT NULL,
        [Amount] decimal(18,2) NOT NULL,
        [Type] int NOT NULL,
        [AdjustmentCategory] nvarchar(max) NOT NULL,
        [Reason] nvarchar(max) NOT NULL,
        [IsApproved] bit NOT NULL,
        [IsAppliedInPayroll] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] int NOT NULL,
        [ApprovedAt] datetime2 NULL,
        [ApprovedBy] int NULL,
        CONSTRAINT [PK_SalaryAdjustments] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_SalaryAdjustments_SalaryPeriods_SalaryPeriodId] FOREIGN KEY ([SalaryPeriodId]) REFERENCES [SalaryPeriods] ([SalaryPeriodID]) ON DELETE NO ACTION,
        CONSTRAINT [FK_SalaryAdjustments_emp_EmployeeId] FOREIGN KEY ([EmployeeId]) REFERENCES [emp] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE TABLE [users] (
        [Id] int NOT NULL IDENTITY,
        [FirstName] nvarchar(150) NOT NULL,
        [LastName] nvarchar(150) NOT NULL,
        [Email] nvarchar(max) NOT NULL,
        [Password] nvarchar(max) NOT NULL,
        [UsrIsActive] nvarchar(max) NULL,
        [createdby] int NOT NULL,
        [createat] datetime2 NOT NULL,
        [updatedby] int NOT NULL,
        [updateat] datetime2 NOT NULL,
        [EmployeeId] int NULL,
        CONSTRAINT [PK_users] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_users_emp_EmployeeId] FOREIGN KEY ([EmployeeId]) REFERENCES [emp] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE TABLE [WithoutPayDays] (
        [id] int NOT NULL IDENTITY,
        [withoutpaydays] decimal(5,2) NOT NULL,
        [PeriodSalery] int NOT NULL,
        [EmployeeId] int NOT NULL,
        [createdby] int NOT NULL,
        [createat] datetime2 NOT NULL,
        [updatedby] int NOT NULL,
        [updateat] datetime2 NOT NULL,
        CONSTRAINT [PK_WithoutPayDays] PRIMARY KEY ([id]),
        CONSTRAINT [FK_WithoutPayDays_SalaryPeriods_PeriodSalery] FOREIGN KEY ([PeriodSalery]) REFERENCES [SalaryPeriods] ([SalaryPeriodID]) ON DELETE NO ACTION,
        CONSTRAINT [FK_WithoutPayDays_emp_EmployeeId] FOREIGN KEY ([EmployeeId]) REFERENCES [emp] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE TABLE [PayRollGenrate] (
        [id] int NOT NULL IDENTITY,
        [EmployeeFK] int NOT NULL,
        [PayRollDefinationFK] int NOT NULL,
        [salery] int NOT NULL,
        [createdby] int NOT NULL,
        [createat] datetime2 NOT NULL,
        [updatedby] int NOT NULL,
        [updateat] datetime2 NOT NULL,
        CONSTRAINT [PK_PayRollGenrate] PRIMARY KEY ([id]),
        CONSTRAINT [FK_PayRollGenrate_PayRollDefinationFile_PayRollDefinationFK] FOREIGN KEY ([PayRollDefinationFK]) REFERENCES [PayRollDefinationFile] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_PayRollGenrate_emp_EmployeeFK] FOREIGN KEY ([EmployeeFK]) REFERENCES [emp] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE TABLE [LeaveApprovals] (
        [Id] numeric(18,0) NOT NULL IDENTITY,
        [LeaveRequestId] numeric(18,0) NOT NULL,
        [ApproverEmployeeId] int NOT NULL,
        [ApprovalLevel] int NOT NULL,
        [Action] nvarchar(20) NOT NULL,
        [Remarks] nvarchar(500) NULL,
        [ActionAt] datetime2 NOT NULL,
        CONSTRAINT [PK_LeaveApprovals] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_LeaveApprovals_LeaveRequests_LeaveRequestId] FOREIGN KEY ([LeaveRequestId]) REFERENCES [LeaveRequests] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_LeaveApprovals_emp_ApproverEmployeeId] FOREIGN KEY ([ApproverEmployeeId]) REFERENCES [emp] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE TABLE [LeaveTransactions] (
        [Id] numeric(18,0) NOT NULL IDENTITY,
        [LeaveRequestFk] numeric(18,0) NULL,
        [EmployeeFk] int NOT NULL,
        [LeaveTypeFk] numeric(18,0) NOT NULL,
        [TransactionDate] date NOT NULL,
        [TransactionType] int NOT NULL,
        [Year] int NOT NULL,
        [Credit] decimal(5,2) NOT NULL,
        [Debit] decimal(5,2) NOT NULL,
        [Balance] decimal(5,2) NOT NULL,
        [Narration] nvarchar(1000) NULL,
        [ReferenceNo] nvarchar(100) NULL,
        [TransactionSource] nvarchar(50) NULL,
        [ReversalOfTransactionId] numeric(18,0) NULL,
        [CreatedBy] int NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_LeaveTransactions] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_LeaveTransactions_LeaveRequests_LeaveRequestFk] FOREIGN KEY ([LeaveRequestFk]) REFERENCES [LeaveRequests] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_LeaveTransactions_LeaveTypes_LeaveTypeFk] FOREIGN KEY ([LeaveTypeFk]) REFERENCES [LeaveTypes] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_LeaveTransactions_emp_EmployeeFk] FOREIGN KEY ([EmployeeFk]) REFERENCES [emp] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE TABLE [MasterLoanAdvanceDetail] (
        [id] int NOT NULL IDENTITY,
        [MasterId] int NOT NULL,
        [InstallmentNo] int NOT NULL,
        [DeductionMonth] nvarchar(50) NOT NULL,
        [TotalDeductionAmount] decimal(18,2) NOT NULL,
        [DeductedAmount] decimal(18,2) NOT NULL,
        [RemainingAmount] decimal(18,2) NOT NULL,
        [InterestAmount] decimal(18,2) NOT NULL,
        [Status] int NOT NULL,
        [createdby] int NOT NULL,
        [createat] datetime2 NOT NULL,
        [updatedby] int NOT NULL,
        [updateat] datetime2 NOT NULL,
        CONSTRAINT [PK_MasterLoanAdvanceDetail] PRIMARY KEY ([id]),
        CONSTRAINT [FK_MasterLoanAdvanceDetail_MasterLoanAdvance_MasterId] FOREIGN KEY ([MasterId]) REFERENCES [MasterLoanAdvance] ([id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE TABLE [UserRoles] (
        [Id] int NOT NULL IDENTITY,
        [UserId] int NOT NULL,
        [RoleId] int NOT NULL,
        [createdby] int NOT NULL,
        [createat] datetime2 NOT NULL,
        CONSTRAINT [PK_UserRoles] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_UserRoles_roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [roles] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_UserRoles_users_UserId] FOREIGN KEY ([UserId]) REFERENCES [users] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE TABLE [SalarySlipMasters] (
        [Id] int NOT NULL IDENTITY,
        [EmployeeID] int NOT NULL,
        [SalaryPeriodID] int NOT NULL,
        [EmployeeNameSnapshot] nvarchar(max) NOT NULL,
        [DepartmentSnapshot] nvarchar(max) NOT NULL,
        [DesignationSnapshot] nvarchar(max) NOT NULL,
        [LocationSnapshot] nvarchar(max) NOT NULL,
        [BankNameSnapshot] nvarchar(max) NOT NULL,
        [AccountNumberSnapshot] nvarchar(max) NOT NULL,
        [TotalDaysInMonth] int NOT NULL,
        [WithoutPayDaysFK] int NULL,
        [UnpaidDaysSnapshot] decimal(18,2) NOT NULL,
        [NetPaidDays] decimal(18,2) NOT NULL,
        [GrossSalaryFK] int NULL,
        [GrossSalarySnapshot] decimal(18,2) NOT NULL,
        [TotalAllowances] decimal(18,2) NOT NULL,
        [TotalDeductions] decimal(18,2) NOT NULL,
        [NetSalary] decimal(18,2) NOT NULL,
        [ArrearAmount] decimal(18,2) NOT NULL,
        [BonusAmount] decimal(18,2) NOT NULL,
        [PreviousCarryForward] decimal(18,2) NOT NULL,
        [AdjustedNetSalary] decimal(18,2) NOT NULL,
        [PayableAmount] decimal(18,2) NOT NULL,
        [NewCarryForward] decimal(18,2) NOT NULL,
        [Status] nvarchar(max) NOT NULL,
        [Remarks] nvarchar(max) NULL,
        [createdby] int NOT NULL,
        [createat] datetime2 NOT NULL,
        [updatedby] int NULL,
        [updateat] datetime2 NULL,
        CONSTRAINT [PK_SalarySlipMasters] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_SalarySlipMasters_PayRollGenrate_GrossSalaryFK] FOREIGN KEY ([GrossSalaryFK]) REFERENCES [PayRollGenrate] ([id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_SalarySlipMasters_SalaryPeriods_SalaryPeriodID] FOREIGN KEY ([SalaryPeriodID]) REFERENCES [SalaryPeriods] ([SalaryPeriodID]) ON DELETE NO ACTION,
        CONSTRAINT [FK_SalarySlipMasters_WithoutPayDays_WithoutPayDaysFK] FOREIGN KEY ([WithoutPayDaysFK]) REFERENCES [WithoutPayDays] ([id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_SalarySlipMasters_emp_EmployeeID] FOREIGN KEY ([EmployeeID]) REFERENCES [emp] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE TABLE [SalarySlipDetails] (
        [Id] int NOT NULL IDENTITY,
        [SalarySlipMasterID] int NOT NULL,
        [ComponentName] nvarchar(max) NOT NULL,
        [Amount] decimal(18,2) NOT NULL,
        [Type] nvarchar(max) NOT NULL,
        [PayRollDefinationFK] int NULL,
        [createdby] int NOT NULL,
        [createat] datetime2 NOT NULL,
        [updatedby] int NULL,
        [updateat] datetime2 NULL,
        CONSTRAINT [PK_SalarySlipDetails] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_SalarySlipDetails_PayRollDefinationFile_PayRollDefinationFK] FOREIGN KEY ([PayRollDefinationFK]) REFERENCES [PayRollDefinationFile] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_SalarySlipDetails_SalarySlipMasters_SalarySlipMasterID] FOREIGN KEY ([SalarySlipMasterID]) REFERENCES [SalarySlipMasters] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE INDEX [IX_AttendenceException_EmployeeId] ON [AttendenceException] ([EmployeeId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE INDEX [IX_AttendenceException_ExpectionStatusFk] ON [AttendenceException] ([ExpectionStatusFk]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE INDEX [IX_AttendenceException_ExpectionTypeFk] ON [AttendenceException] ([ExpectionTypeFk]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE INDEX [IX_AttendenceLogs_EmployeeID] ON [AttendenceLogs] ([EmployeeID]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE INDEX [IX_AttendenceLogs_shiftidFk] ON [AttendenceLogs] ([shiftidFk]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE INDEX [IX_Bonuses_BonusTypeID] ON [Bonuses] ([BonusTypeID]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE INDEX [IX_Bonuses_EmployeeID] ON [Bonuses] ([EmployeeID]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE INDEX [IX_Bonuses_SalaryPeriodID] ON [Bonuses] ([SalaryPeriodID]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE INDEX [IX_emp_BankNameFk] ON [emp] ([BankNameFk]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE INDEX [IX_emp_DepartmentFk] ON [emp] ([DepartmentFk]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE INDEX [IX_emp_DesiginationId] ON [emp] ([DesiginationId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE INDEX [IX_emp_DivisionFk] ON [emp] ([DivisionFk]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE INDEX [IX_emp_EmploymentStatusFk] ON [emp] ([EmploymentStatusFk]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE INDEX [IX_emp_EmploymentTypeFk] ON [emp] ([EmploymentTypeFk]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE INDEX [IX_emp_GradeFk] ON [emp] ([GradeFk]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE INDEX [IX_emp_LocationFk] ON [emp] ([LocationFk]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE INDEX [IX_emp_OvertimePolicyId] ON [emp] ([OvertimePolicyId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE INDEX [IX_emp_SubDepartmentFk] ON [emp] ([SubDepartmentFk]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Employees_Code_Unique] ON [emp] ([Code]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE INDEX [IX_EmployeeLeavePolicies_EmployeeId] ON [EmployeeLeavePolicies] ([EmployeeId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE INDEX [IX_EmployeeLeavePolicies_LeavePolicyId] ON [EmployeeLeavePolicies] ([LeavePolicyId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE INDEX [IX_EmployeeShifts_EmployeeId] ON [EmployeeShifts] ([EmployeeId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE INDEX [IX_EmployeeShifts_ShiftId] ON [EmployeeShifts] ([ShiftId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE INDEX [IX_EOBIDetuction_EmployeeId] ON [EOBIDetuction] ([EmployeeId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE INDEX [IX_EOBIDetuction_PeriodSalery] ON [EOBIDetuction] ([PeriodSalery]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE INDEX [IX_FunctionRoles_FunctionId] ON [FunctionRoles] ([FunctionId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE INDEX [IX_FunctionRoles_RoleId] ON [FunctionRoles] ([RoleId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE INDEX [IX_IncomeTaxDetuction_EmployeeId] ON [IncomeTaxDetuction] ([EmployeeId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE INDEX [IX_IncomeTaxDetuction_PeriodSalery] ON [IncomeTaxDetuction] ([PeriodSalery]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE INDEX [IX_LeaveApprovals_ApproverEmployeeId] ON [LeaveApprovals] ([ApproverEmployeeId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE INDEX [IX_LeaveApprovals_LeaveRequestId] ON [LeaveApprovals] ([LeaveRequestId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE INDEX [IX_LeaveBalances_EmployeeId] ON [LeaveBalances] ([EmployeeId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE INDEX [IX_LeaveBalances_LeaveTypeId] ON [LeaveBalances] ([LeaveTypeId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE INDEX [IX_LeaveConfigDetail_ApproverEmpNo] ON [LeaveConfigurationDetails] ([ApproverEmpNo]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE UNIQUE INDEX [IX_LeaveConfigDetail_Config_Ref_Unique] ON [LeaveConfigurationDetails] ([LeaveConfigurationId], [ReferenceId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE UNIQUE INDEX [IX_LeaveOpening_Employee_LeaveType_Year] ON [LeaveOpenings] ([EmployeeFk], [LeaveTypeFk], [Year]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE INDEX [IX_LeaveOpenings_LeaveTypeFk] ON [LeaveOpenings] ([LeaveTypeFk]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE UNIQUE INDEX [IX_LeavePolicyRule_Policy_LeaveType_Unique] ON [LeavePolicyRules] ([LeavePolicyId], [LeaveTypeId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE INDEX [IX_LeavePolicyRules_LeaveTypeId] ON [LeavePolicyRules] ([LeaveTypeId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE INDEX [IX_LeaveRequests_CurrentApproverId] ON [LeaveRequests] ([CurrentApproverId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE INDEX [IX_LeaveRequests_EmployeeId] ON [LeaveRequests] ([EmployeeId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE INDEX [IX_LeaveRequests_LeaveTypeId] ON [LeaveRequests] ([LeaveTypeId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE INDEX [IX_LeaveTransaction_Employee_LeaveType_Year] ON [LeaveTransactions] ([EmployeeFk], [LeaveTypeFk], [Year]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE INDEX [IX_LeaveTransaction_ReferenceNo] ON [LeaveTransactions] ([ReferenceNo]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE INDEX [IX_LeaveTransactions_LeaveRequestFk] ON [LeaveTransactions] ([LeaveRequestFk]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE INDEX [IX_LeaveTransactions_LeaveTypeFk] ON [LeaveTransactions] ([LeaveTypeFk]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE INDEX [IX_MasterLoanAdvance_EmployeeID] ON [MasterLoanAdvance] ([EmployeeID]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE INDEX [IX_MasterLoanAdvanceDetail_MasterId] ON [MasterLoanAdvanceDetail] ([MasterId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE INDEX [IX_OverTimeHours_EmployeeId] ON [OverTimeHours] ([EmployeeId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE INDEX [IX_OverTimeHours_SalaryPeriodFK] ON [OverTimeHours] ([SalaryPeriodFK]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE INDEX [IX_PayRollDefinationFile_PayRollComFK] ON [PayRollDefinationFile] ([PayRollComFK]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE INDEX [IX_PayRollGenrate_EmployeeFK] ON [PayRollGenrate] ([EmployeeFK]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE INDEX [IX_PayRollGenrate_PayRollDefinationFK] ON [PayRollGenrate] ([PayRollDefinationFK]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE INDEX [IX_PFMembers_EmployeeId] ON [PFMembers] ([EmployeeId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE INDEX [IX_PFMembers_SalaryPeriodFK] ON [PFMembers] ([SalaryPeriodFK]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE INDEX [IX_SalaryAdjustments_EmployeeId] ON [SalaryAdjustments] ([EmployeeId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE INDEX [IX_SalaryAdjustments_SalaryPeriodId] ON [SalaryAdjustments] ([SalaryPeriodId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE INDEX [IX_SalarySlipDetails_PayRollDefinationFK] ON [SalarySlipDetails] ([PayRollDefinationFK]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE INDEX [IX_SalarySlipDetails_SalarySlipMasterID] ON [SalarySlipDetails] ([SalarySlipMasterID]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE INDEX [IX_SalarySlipMasters_EmployeeID] ON [SalarySlipMasters] ([EmployeeID]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE INDEX [IX_SalarySlipMasters_GrossSalaryFK] ON [SalarySlipMasters] ([GrossSalaryFK]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE INDEX [IX_SalarySlipMasters_SalaryPeriodID] ON [SalarySlipMasters] ([SalaryPeriodID]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE INDEX [IX_SalarySlipMasters_WithoutPayDaysFK] ON [SalarySlipMasters] ([WithoutPayDaysFK]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE INDEX [IX_TaxSlabMaster_FiscalFK] ON [TaxSlabMaster] ([FiscalFK]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE INDEX [IX_TaxSurcharge_FiscalFK] ON [TaxSurcharge] ([FiscalFK]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE INDEX [IX_UserRoles_RoleId] ON [UserRoles] ([RoleId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE INDEX [IX_UserRoles_UserId] ON [UserRoles] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE INDEX [IX_users_EmployeeId] ON [users] ([EmployeeId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE INDEX [IX_WithoutPayDays_EmployeeId] ON [WithoutPayDays] ([EmployeeId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    CREATE INDEX [IX_WithoutPayDays_PeriodSalery] ON [WithoutPayDays] ([PeriodSalery]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512033214_intial'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260512033214_intial', N'8.0.24');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260513150918_AddUpdatedAtToLeaveOpening'
)
BEGIN
    ALTER TABLE [LeaveOpenings] ADD [UpdatedAt] datetime2 NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260513150918_AddUpdatedAtToLeaveOpening'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260513150918_AddUpdatedAtToLeaveOpening', N'8.0.24');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260513165500_AddCanApplyForOthersToEmployee'
)
BEGIN
    ALTER TABLE [emp] ADD [CanApplyForOthers] bit NOT NULL DEFAULT CAST(0 AS bit);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260513165500_AddCanApplyForOthersToEmployee'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260513165500_AddCanApplyForOthersToEmployee', N'8.0.24');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260525034732_AddExecutionsTable'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260525034732_AddExecutionsTable', N'8.0.24');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260525042237_UpdateExecutionSteps'
)
BEGIN
    ALTER TABLE [Executions] ADD [BonusFetched] bit NOT NULL DEFAULT CAST(0 AS bit);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260525042237_UpdateExecutionSteps'
)
BEGIN
    ALTER TABLE [Executions] ADD [EOBIExecuted] bit NOT NULL DEFAULT CAST(0 AS bit);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260525042237_UpdateExecutionSteps'
)
BEGIN
    ALTER TABLE [Executions] ADD [IncomeTaxExecuted] bit NOT NULL DEFAULT CAST(0 AS bit);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260525042237_UpdateExecutionSteps'
)
BEGIN
    ALTER TABLE [Executions] ADD [LoanDeductionExecuted] bit NOT NULL DEFAULT CAST(0 AS bit);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260525042237_UpdateExecutionSteps'
)
BEGIN
    ALTER TABLE [Executions] ADD [PFExecuted] bit NOT NULL DEFAULT CAST(0 AS bit);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260525042237_UpdateExecutionSteps'
)
BEGIN
    ALTER TABLE [Executions] ADD [SalaryAdjustmentFetched] bit NOT NULL DEFAULT CAST(0 AS bit);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260525042237_UpdateExecutionSteps'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260525042237_UpdateExecutionSteps', N'8.0.24');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260603080120_AddFiscalYearIsLocked'
)
BEGIN
    ALTER TABLE [FiscalYear] ADD [IsLocked] bit NOT NULL DEFAULT CAST(0 AS bit);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260603080120_AddFiscalYearIsLocked'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260603080120_AddFiscalYearIsLocked', N'8.0.24');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260603081558_RevertFiscalYearIsLocked'
)
BEGIN
    DECLARE @var0 sysname;
    SELECT @var0 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[FiscalYear]') AND [c].[name] = N'IsLocked');
    IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [FiscalYear] DROP CONSTRAINT [' + @var0 + '];');
    ALTER TABLE [FiscalYear] DROP COLUMN [IsLocked];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260603081558_RevertFiscalYearIsLocked'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260603081558_RevertFiscalYearIsLocked', N'8.0.24');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260603082125_DropFiscalYearIsLocked'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260603082125_DropFiscalYearIsLocked', N'8.0.24');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260603083747_AddLeaveYearLock'
)
BEGIN
    CREATE TABLE [dbo].[LeaveYearLocks] (
        [Year] int NOT NULL IDENTITY,
        [LockedAt] datetime2 NOT NULL,
        [LockedBy] int NOT NULL,
        CONSTRAINT [PK_LeaveYearLocks] PRIMARY KEY ([Year])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260603083747_AddLeaveYearLock'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260603083747_AddLeaveYearLock', N'8.0.24');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260603092023_MoveLeaveYearLocksToDbo'
)
BEGIN
    ALTER SCHEMA dbo TRANSFER [ahrmaster].[LeaveYearLocks];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260603092023_MoveLeaveYearLocksToDbo'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260603092023_MoveLeaveYearLocksToDbo', N'8.0.24');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260603092143_TransferLeaveYearLockSchema'
)
BEGIN

                    IF OBJECT_ID('[ahrmaster].[LeaveYearLocks]') IS NOT NULL
                        ALTER SCHEMA dbo TRANSFER [ahrmaster].[LeaveYearLocks];
                
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260603092143_TransferLeaveYearLockSchema'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260603092143_TransferLeaveYearLockSchema', N'8.0.24');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260603095754_FixLeaveYearLockIdentity'
)
BEGIN

                    IF OBJECT_ID('[dbo].[LeaveYearLocks]') IS NOT NULL DROP TABLE [dbo].[LeaveYearLocks];
                    IF OBJECT_ID('[ahrmaster].[LeaveYearLocks]') IS NOT NULL DROP TABLE [ahrmaster].[LeaveYearLocks];
                
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260603095754_FixLeaveYearLockIdentity'
)
BEGIN
    CREATE TABLE [dbo].[LeaveYearLocks] (
        [Year] int NOT NULL,
        [LockedAt] datetime2 NOT NULL,
        [LockedBy] int NOT NULL,
        CONSTRAINT [PK_LeaveYearLocks] PRIMARY KEY ([Year])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260603095754_FixLeaveYearLockIdentity'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260603095754_FixLeaveYearLockIdentity', N'8.0.24');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260603144449_FixEmployeeLeavePolicyRelationship'
)
BEGIN

                    DELETE FROM EmployeeLeavePolicies
                    WHERE LeavePolicyId NOT IN (SELECT Id FROM LeavePolicy);
                
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260603144449_FixEmployeeLeavePolicyRelationship'
)
BEGIN
    CREATE UNIQUE INDEX [IX_EmployeeLeavePolicies_Employee_Policy_Unique] ON [EmployeeLeavePolicies] ([EmployeeId], [LeavePolicyId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260603144449_FixEmployeeLeavePolicyRelationship'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260603144449_FixEmployeeLeavePolicyRelationship', N'8.0.24');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260605000000_AddHrRequisition'
)
BEGIN

    IF OBJECT_ID(N'[HR_Requisition]', N'U') IS NULL
    BEGIN
        CREATE TABLE [HR_Requisition]
        (
            [Id] NUMERIC(18,0) IDENTITY(1,1) NOT NULL CONSTRAINT [PK_HR_Requisition] PRIMARY KEY,
            [RequisitionNo] NVARCHAR(30) NULL,
            [EmployeeFK] NUMERIC(18,0) NULL,
            [DepartmentFK] NUMERIC(18,0) NULL,
            [PositionTitle] NVARCHAR(200) NULL,
            [VacancyCount] NUMERIC(18,0) NULL,
            [Reason] NVARCHAR(1000) NULL,
            [Status] NVARCHAR(30) NULL,
            [RequiredByDate] DATE NULL,
            [CreatedBy] NVARCHAR(50) NULL,
            [CreatedOn] DATETIME2 NULL,
            [UpdatedBy] NVARCHAR(50) NULL,
            [UpdatedOn] DATETIME2 NULL
        );
    END;

    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [name] = N'IX_HR_Requisition_RequisitionNo_Unique' AND [object_id] = OBJECT_ID(N'[HR_Requisition]'))
    BEGIN
        CREATE UNIQUE INDEX [IX_HR_Requisition_RequisitionNo_Unique]
        ON [HR_Requisition] ([RequisitionNo])
        WHERE [RequisitionNo] IS NOT NULL;
    END;

END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260605000000_AddHrRequisition'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260605000000_AddHrRequisition', N'8.0.24');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260605010000_AddHrRequisitionWorkflow'
)
BEGIN

    IF COL_LENGTH(N'HR_Requisition', N'WorkflowInstanceFK') IS NULL
    BEGIN
        ALTER TABLE [HR_Requisition]
        ADD [WorkflowInstanceFK] NUMERIC(18,0) NULL;
    END;

    IF OBJECT_ID(N'[HR_WorkflowDefinition]', N'U') IS NULL
    BEGIN
        CREATE TABLE [HR_WorkflowDefinition]
        (
            [Id] NUMERIC(18,0) IDENTITY(1,1) NOT NULL CONSTRAINT [PK_HR_WorkflowDefinition] PRIMARY KEY,
            [WorkflowName] NVARCHAR(100) NOT NULL,
            [IsActive] BIT NOT NULL CONSTRAINT [DF_HR_WorkflowDefinition_IsActive] DEFAULT(1),
            [CreatedBy] NVARCHAR(50) NULL,
            [CreatedOn] DATETIME2 NULL,
            [UpdatedBy] NVARCHAR(50) NULL,
            [UpdatedOn] DATETIME2 NULL
        );
    END;

    IF OBJECT_ID(N'[HR_WorkflowStep]', N'U') IS NULL
    BEGIN
        CREATE TABLE [HR_WorkflowStep]
        (
            [Id] NUMERIC(18,0) IDENTITY(1,1) NOT NULL CONSTRAINT [PK_HR_WorkflowStep] PRIMARY KEY,
            [WorkflowDefinitionId] NUMERIC(18,0) NOT NULL,
            [StepOrder] INT NOT NULL,
            [StepName] NVARCHAR(100) NOT NULL,
            [RoleId] NUMERIC(18,0) NOT NULL,
            [IsFinalStep] BIT NOT NULL CONSTRAINT [DF_HR_WorkflowStep_IsFinalStep] DEFAULT(0),
            [CreatedBy] NVARCHAR(50) NULL,
            [CreatedOn] DATETIME2 NULL,
            [UpdatedBy] NVARCHAR(50) NULL,
            [UpdatedOn] DATETIME2 NULL,
            CONSTRAINT [FK_HR_WorkflowStep_HR_WorkflowDefinition_WorkflowDefinitionId]
                FOREIGN KEY ([WorkflowDefinitionId]) REFERENCES [HR_WorkflowDefinition]([Id])
        );
    END;

    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [name] = N'IX_HR_WorkflowStep_Definition_Order' AND [object_id] = OBJECT_ID(N'[HR_WorkflowStep]'))
    BEGIN
        CREATE UNIQUE INDEX [IX_HR_WorkflowStep_Definition_Order]
        ON [HR_WorkflowStep] ([WorkflowDefinitionId], [StepOrder]);
    END;

    IF OBJECT_ID(N'[HR_WorkflowInstance]', N'U') IS NULL
    BEGIN
        CREATE TABLE [HR_WorkflowInstance]
        (
            [Id] NUMERIC(18,0) IDENTITY(1,1) NOT NULL CONSTRAINT [PK_HR_WorkflowInstance] PRIMARY KEY,
            [WorkflowDefinitionId] NUMERIC(18,0) NOT NULL,
            [EntityType] NVARCHAR(50) NOT NULL,
            [EntityId] NUMERIC(18,0) NOT NULL,
            [CurrentStepId] NUMERIC(18,0) NOT NULL,
            [Status] NVARCHAR(50) NOT NULL,
            [StartedDate] DATETIME2 NOT NULL,
            [CompletedDate] DATETIME2 NULL,
            CONSTRAINT [FK_HR_WorkflowInstance_HR_WorkflowDefinition_WorkflowDefinitionId]
                FOREIGN KEY ([WorkflowDefinitionId]) REFERENCES [HR_WorkflowDefinition]([Id]),
            CONSTRAINT [FK_HR_WorkflowInstance_HR_WorkflowStep_CurrentStepId]
                FOREIGN KEY ([CurrentStepId]) REFERENCES [HR_WorkflowStep]([Id])
        );
    END;

    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [name] = N'IX_HR_WorkflowInstance_Entity' AND [object_id] = OBJECT_ID(N'[HR_WorkflowInstance]'))
    BEGIN
        CREATE INDEX [IX_HR_WorkflowInstance_Entity]
        ON [HR_WorkflowInstance] ([EntityType], [EntityId]);
    END;

    IF OBJECT_ID(N'[HR_WorkflowHistory]', N'U') IS NULL
    BEGIN
        CREATE TABLE [HR_WorkflowHistory]
        (
            [Id] NUMERIC(18,0) IDENTITY(1,1) NOT NULL CONSTRAINT [PK_HR_WorkflowHistory] PRIMARY KEY,
            [WorkflowInstanceFK] NUMERIC(18,0) NOT NULL,
            [WorkflowStepFK] NUMERIC(18,0) NOT NULL,
            [ActionBy] NUMERIC(18,0) NOT NULL,
            [ActionType] NVARCHAR(50) NOT NULL,
            [Comments] NVARCHAR(1000) NULL,
            [ActionDate] DATETIME2 NOT NULL,
            CONSTRAINT [FK_HR_WorkflowHistory_HR_WorkflowInstance_WorkflowInstanceFK]
                FOREIGN KEY ([WorkflowInstanceFK]) REFERENCES [HR_WorkflowInstance]([Id]),
            CONSTRAINT [FK_HR_WorkflowHistory_HR_WorkflowStep_WorkflowStepFK]
                FOREIGN KEY ([WorkflowStepFK]) REFERENCES [HR_WorkflowStep]([Id])
        );
    END;

    DECLARE @WorkflowDefinitionId NUMERIC(18,0);

    SELECT @WorkflowDefinitionId = [Id]
    FROM [HR_WorkflowDefinition]
    WHERE [WorkflowName] = N'Recruitment Requisition Approval';

    IF @WorkflowDefinitionId IS NULL
    BEGIN
        INSERT INTO [HR_WorkflowDefinition] ([WorkflowName], [IsActive], [CreatedBy], [CreatedOn])
        VALUES (N'Recruitment Requisition Approval', 1, N'Migration', SYSDATETIME());

        SET @WorkflowDefinitionId = SCOPE_IDENTITY();
    END;

    IF EXISTS (SELECT 1 FROM [roles] WHERE [Name] = N'HRBP')
       AND NOT EXISTS (SELECT 1 FROM [HR_WorkflowStep] WHERE [WorkflowDefinitionId] = @WorkflowDefinitionId AND [StepOrder] = 1)
    BEGIN
        INSERT INTO [HR_WorkflowStep] ([WorkflowDefinitionId], [StepOrder], [StepName], [RoleId], [IsFinalStep], [CreatedBy], [CreatedOn])
        SELECT @WorkflowDefinitionId, 1, N'HRBP Review', CAST([Id] AS NUMERIC(18,0)), 0, N'Migration', SYSDATETIME()
        FROM [roles]
        WHERE [Name] = N'HRBP';
    END;

    IF EXISTS (SELECT 1 FROM [roles] WHERE [Name] = N'HOD')
       AND NOT EXISTS (SELECT 1 FROM [HR_WorkflowStep] WHERE [WorkflowDefinitionId] = @WorkflowDefinitionId AND [StepOrder] = 2)
    BEGIN
        INSERT INTO [HR_WorkflowStep] ([WorkflowDefinitionId], [StepOrder], [StepName], [RoleId], [IsFinalStep], [CreatedBy], [CreatedOn])
        SELECT @WorkflowDefinitionId, 2, N'HOD Approval', CAST([Id] AS NUMERIC(18,0)), 0, N'Migration', SYSDATETIME()
        FROM [roles]
        WHERE [Name] = N'HOD';
    END;

    IF EXISTS (SELECT 1 FROM [roles] WHERE [Name] = N'Recruiter')
       AND NOT EXISTS (SELECT 1 FROM [HR_WorkflowStep] WHERE [WorkflowDefinitionId] = @WorkflowDefinitionId AND [StepOrder] = 3)
    BEGIN
        INSERT INTO [HR_WorkflowStep] ([WorkflowDefinitionId], [StepOrder], [StepName], [RoleId], [IsFinalStep], [CreatedBy], [CreatedOn])
        SELECT @WorkflowDefinitionId, 3, N'Recruiter Assignment', CAST([Id] AS NUMERIC(18,0)), 1, N'Migration', SYSDATETIME()
        FROM [roles]
        WHERE [Name] = N'Recruiter';
    END;

END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260605010000_AddHrRequisitionWorkflow'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260605010000_AddHrRequisitionWorkflow', N'8.0.24');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260605020000_AddHrRecruitmentSprint3'
)
BEGIN

    -- ── HR_RequisitionAssignment ────────────────────────────────
    IF OBJECT_ID(N'[HR_RequisitionAssignment]', N'U') IS NULL
    BEGIN
        CREATE TABLE [HR_RequisitionAssignment]
        (
            [Id]                  NUMERIC(18,0) IDENTITY(1,1) NOT NULL CONSTRAINT [PK_HR_RequisitionAssignment] PRIMARY KEY,
            [RequisitionFk]       NUMERIC(18,0) NULL,
            [RecruiterEmployeeFK] NUMERIC(18,0) NULL,
            [AssignedDate]        DATETIME2     NULL,
            [Notes]               NVARCHAR(1000) NULL,
            [CreatedBy]           NVARCHAR(50)  NULL,
            [CreatedOn]           DATETIME2     NULL,
            [UpdatedBy]           NVARCHAR(50)  NULL,
            [UpdatedOn]           DATETIME2     NULL
        );
    END;

    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_HR_RequisitionAssignment_Requisition_Unique' AND object_id = OBJECT_ID(N'[HR_RequisitionAssignment]'))
        CREATE UNIQUE INDEX [IX_HR_RequisitionAssignment_Requisition_Unique]
        ON [HR_RequisitionAssignment] ([RequisitionFk])
        WHERE [RequisitionFk] IS NOT NULL;

    -- ── HR_PostingChannel ────────────────────────────────────────
    IF OBJECT_ID(N'[HR_PostingChannel]', N'U') IS NULL
    BEGIN
        CREATE TABLE [HR_PostingChannel]
        (
            [Id]          NUMERIC(18,0) IDENTITY(1,1) NOT NULL CONSTRAINT [PK_HR_PostingChannel] PRIMARY KEY,
            [ChannelName] NVARCHAR(100) NOT NULL,
            [IsInternal]  BIT NOT NULL,
            [IsActive]    BIT NOT NULL CONSTRAINT [DF_HR_PostingChannel_IsActive] DEFAULT(1)
        );
    END;

    -- ── HR_JobPosting ────────────────────────────────────────────
    IF OBJECT_ID(N'[HR_JobPosting]', N'U') IS NULL
    BEGIN
        CREATE TABLE [HR_JobPosting]
        (
            [Id]             NUMERIC(18,0) IDENTITY(1,1) NOT NULL CONSTRAINT [PK_HR_JobPosting] PRIMARY KEY,
            [RequisitionFK]  NUMERIC(18,0)  NULL,
            [JobCode]        NVARCHAR(30)   NULL,
            [JobTitle]       NVARCHAR(200)  NULL,
            [JobDescription] NVARCHAR(1000) NULL,
            [EmploymentType] NVARCHAR(50)   NULL,
            [Location]       NVARCHAR(200)  NULL,
            [PostingStatus]  NVARCHAR(30)   NOT NULL,
            [OpenDate]       DATE           NOT NULL,
            [CloseDate]      DATE           NULL,
            [IsDeleted]      BIT            NOT NULL CONSTRAINT [DF_HR_JobPosting_IsDeleted] DEFAULT(0),
            [CreatedBy]      NVARCHAR(50)   NULL,
            [CreatedOn]      DATETIME2      NULL,
            [UpdatedBy]      NVARCHAR(50)   NULL,
            [UpdatedOn]      DATETIME2      NULL
        );
    END;

    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_HR_JobPosting_JobCode_Unique' AND object_id = OBJECT_ID(N'[HR_JobPosting]'))
        CREATE UNIQUE INDEX [IX_HR_JobPosting_JobCode_Unique]
        ON [HR_JobPosting] ([JobCode])
        WHERE [JobCode] IS NOT NULL;

    -- ── HR_JobPostingChannel ─────────────────────────────────────
    IF OBJECT_ID(N'[HR_JobPostingChannel]', N'U') IS NULL
    BEGIN
        CREATE TABLE [HR_JobPostingChannel]
        (
            [Id]                NUMERIC(18,0) IDENTITY(1,1) NOT NULL CONSTRAINT [PK_HR_JobPostingChannel] PRIMARY KEY,
            [JobPostingFK]      NUMERIC(18,0) NOT NULL,
            [PostingChannelFK]  NUMERIC(18,0) NOT NULL,
            [PublishedDate]     DATETIME2     NULL,
            [ExternalReference] NVARCHAR(500) NULL,
            CONSTRAINT [FK_HR_JobPostingChannel_HR_JobPosting]      FOREIGN KEY ([JobPostingFK])     REFERENCES [HR_JobPosting]([Id]),
            CONSTRAINT [FK_HR_JobPostingChannel_HR_PostingChannel]  FOREIGN KEY ([PostingChannelFK]) REFERENCES [HR_PostingChannel]([Id])
        );
    END;

    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_HR_JobPostingChannel_Posting_Channel_Unique' AND object_id = OBJECT_ID(N'[HR_JobPostingChannel]'))
        CREATE UNIQUE INDEX [IX_HR_JobPostingChannel_Posting_Channel_Unique]
        ON [HR_JobPostingChannel] ([JobPostingFK], [PostingChannelFK]);

    -- ── Seed posting channels ────────────────────────────────────
    IF NOT EXISTS (SELECT 1 FROM [HR_PostingChannel] WHERE [ChannelName] = N'Internal Portal')
        INSERT INTO [HR_PostingChannel] ([ChannelName],[IsInternal],[IsActive]) VALUES (N'Internal Portal', 1, 1);
    IF NOT EXISTS (SELECT 1 FROM [HR_PostingChannel] WHERE [ChannelName] = N'LinkedIn')
        INSERT INTO [HR_PostingChannel] ([ChannelName],[IsInternal],[IsActive]) VALUES (N'LinkedIn', 0, 1);
    IF NOT EXISTS (SELECT 1 FROM [HR_PostingChannel] WHERE [ChannelName] = N'Indeed')
        INSERT INTO [HR_PostingChannel] ([ChannelName],[IsInternal],[IsActive]) VALUES (N'Indeed', 0, 1);
    IF NOT EXISTS (SELECT 1 FROM [HR_PostingChannel] WHERE [ChannelName] = N'Naukri')
        INSERT INTO [HR_PostingChannel] ([ChannelName],[IsInternal],[IsActive]) VALUES (N'Naukri', 0, 1);
    IF NOT EXISTS (SELECT 1 FROM [HR_PostingChannel] WHERE [ChannelName] = N'Company Website')
        INSERT INTO [HR_PostingChannel] ([ChannelName],[IsInternal],[IsActive]) VALUES (N'Company Website', 0, 1);

END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260605020000_AddHrRecruitmentSprint3'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260605020000_AddHrRecruitmentSprint3', N'8.0.24');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260605030000_AddHrRecruitmentSprint4'
)
BEGIN

    -- ── HR_Candidate ─────────────────────────────────────────────
    IF OBJECT_ID(N'[HR_Candidate]', N'U') IS NULL
    BEGIN
        CREATE TABLE [HR_Candidate]
        (
            [Id]                   NUMERIC(18,0) IDENTITY(1,1) NOT NULL CONSTRAINT [PK_HR_Candidate] PRIMARY KEY,
            [FirstName]            NVARCHAR(100) NOT NULL,
            [LastName]             NVARCHAR(100) NULL,
            [Email]                NVARCHAR(200) NOT NULL,
            [Phone]                NVARCHAR(50)  NULL,
            [CurrentLocation]      NVARCHAR(200) NULL,
            [TotalExperienceYears] DECIMAL(5,2)  NULL,
            [CurrentEmployer]      NVARCHAR(200) NULL,
            [CurrentDesignation]   NVARCHAR(200) NULL,
            [LinkedInProfile]      NVARCHAR(500) NULL,
            [CreatedDate]          DATETIME2     NULL,
            [ModifiedDate]         DATETIME2     NULL,
            [IsDeleted]            BIT NOT NULL  CONSTRAINT [DF_HR_Candidate_IsDeleted] DEFAULT(0)
        );
    END;

    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_HR_Candidate_Email_Unique' AND object_id = OBJECT_ID(N'[HR_Candidate]'))
        CREATE UNIQUE INDEX [IX_HR_Candidate_Email_Unique]
        ON [HR_Candidate] ([Email])
        WHERE [IsDeleted] = 0;

    -- ── HR_CandidateDocument ──────────────────────────────────────
    IF OBJECT_ID(N'[HR_CandidateDocument]', N'U') IS NULL
    BEGIN
        CREATE TABLE [HR_CandidateDocument]
        (
            [Id]           NUMERIC(18,0)  IDENTITY(1,1) NOT NULL CONSTRAINT [PK_HR_CandidateDocument] PRIMARY KEY,
            [CandidateFk]  NUMERIC(18,0)  NULL,
            [DocumentType] NVARCHAR(50)   NULL,
            [FileName]     NVARCHAR(500)  NULL,
            [FilePath]     NVARCHAR(1000) NULL,
            [UploadedDate] DATETIME2      NULL,
            CONSTRAINT [FK_HR_CandidateDocument_HR_Candidate]
                FOREIGN KEY ([CandidateFk]) REFERENCES [HR_Candidate]([Id])
        );
    END;

    -- ── HR_ApplicationStage ───────────────────────────────────────
    IF OBJECT_ID(N'[HR_ApplicationStage]', N'U') IS NULL
    BEGIN
        CREATE TABLE [HR_ApplicationStage]
        (
            [Id]         NUMERIC(18,0) IDENTITY(1,1) NOT NULL CONSTRAINT [PK_HR_ApplicationStage] PRIMARY KEY,
            [StageName]  NVARCHAR(100) NOT NULL,
            [StageOrder] INT           NULL,
            [IsActive]   BIT NOT NULL  CONSTRAINT [DF_HR_ApplicationStage_IsActive] DEFAULT(1)
        );
    END;

    -- Seed stages
    IF NOT EXISTS (SELECT 1 FROM [HR_ApplicationStage] WHERE [StageName] = N'Applied')
        INSERT INTO [HR_ApplicationStage] ([StageName],[StageOrder],[IsActive]) VALUES (N'Applied',    1, 1);
    IF NOT EXISTS (SELECT 1 FROM [HR_ApplicationStage] WHERE [StageName] = N'Screening')
        INSERT INTO [HR_ApplicationStage] ([StageName],[StageOrder],[IsActive]) VALUES (N'Screening',  2, 1);
    IF NOT EXISTS (SELECT 1 FROM [HR_ApplicationStage] WHERE [StageName] = N'Shortlisted')
        INSERT INTO [HR_ApplicationStage] ([StageName],[StageOrder],[IsActive]) VALUES (N'Shortlisted',3, 1);
    IF NOT EXISTS (SELECT 1 FROM [HR_ApplicationStage] WHERE [StageName] = N'Interview')
        INSERT INTO [HR_ApplicationStage] ([StageName],[StageOrder],[IsActive]) VALUES (N'Interview',  4, 1);
    IF NOT EXISTS (SELECT 1 FROM [HR_ApplicationStage] WHERE [StageName] = N'Final Review')
        INSERT INTO [HR_ApplicationStage] ([StageName],[StageOrder],[IsActive]) VALUES (N'Final Review',5,1);
    IF NOT EXISTS (SELECT 1 FROM [HR_ApplicationStage] WHERE [StageName] = N'Selected')
        INSERT INTO [HR_ApplicationStage] ([StageName],[StageOrder],[IsActive]) VALUES (N'Selected',   6, 1);
    IF NOT EXISTS (SELECT 1 FROM [HR_ApplicationStage] WHERE [StageName] = N'Rejected')
        INSERT INTO [HR_ApplicationStage] ([StageName],[StageOrder],[IsActive]) VALUES (N'Rejected',   7, 1);
    IF NOT EXISTS (SELECT 1 FROM [HR_ApplicationStage] WHERE [StageName] = N'Withdrawn')
        INSERT INTO [HR_ApplicationStage] ([StageName],[StageOrder],[IsActive]) VALUES (N'Withdrawn',  8, 1);

    -- ── HR_JobApplication ─────────────────────────────────────────
    IF OBJECT_ID(N'[HR_JobApplication]', N'U') IS NULL
    BEGIN
        CREATE TABLE [HR_JobApplication]
        (
            [Id]              NUMERIC(18,0)  IDENTITY(1,1) NOT NULL CONSTRAINT [PK_HR_JobApplication] PRIMARY KEY,
            [CandidateFK]     NUMERIC(18,0)  NULL,
            [JobPostingFK]    NUMERIC(18,0)  NULL,
            [CurrentStageFK]  NUMERIC(18,0)  NULL,
            [AppliedDate]     DATETIME2      NULL,
            [RecruiterNotes]  NVARCHAR(MAX)  NULL,
            [IsActive]        BIT NOT NULL   CONSTRAINT [DF_HR_JobApplication_IsActive] DEFAULT(1),
            CONSTRAINT [FK_HR_JobApplication_HR_Candidate]
                FOREIGN KEY ([CandidateFK])    REFERENCES [HR_Candidate]([Id]),
            CONSTRAINT [FK_HR_JobApplication_HR_JobPosting]
                FOREIGN KEY ([JobPostingFK])   REFERENCES [HR_JobPosting]([Id]),
            CONSTRAINT [FK_HR_JobApplication_HR_ApplicationStage]
                FOREIGN KEY ([CurrentStageFK]) REFERENCES [HR_ApplicationStage]([Id])
        );
    END;

    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_HR_JobApplication_Candidate_Posting_Active' AND object_id = OBJECT_ID(N'[HR_JobApplication]'))
        CREATE INDEX [IX_HR_JobApplication_Candidate_Posting_Active]
        ON [HR_JobApplication] ([CandidateFK], [JobPostingFK])
        WHERE [IsActive] = 1;

    -- ── HR_ApplicationStageHistory ────────────────────────────────
    IF OBJECT_ID(N'[HR_ApplicationStageHistory]', N'U') IS NULL
    BEGIN
        CREATE TABLE [HR_ApplicationStageHistory]
        (
            [Id]               NUMERIC(18,0) IDENTITY(1,1) NOT NULL CONSTRAINT [PK_HR_ApplicationStageHistory] PRIMARY KEY,
            [JobApplicationFk] NUMERIC(18,0) NULL,
            [FromStageId]      NUMERIC(18,0) NULL,
            [ToStageFk]        NUMERIC(18,0) NULL,
            [ChangedBy]        NUMERIC(18,0) NULL,
            [Comments]         NVARCHAR(1000) NULL,
            [ChangedDate]      DATETIME2     NULL,
            CONSTRAINT [FK_HR_StageHistory_HR_JobApplication]
                FOREIGN KEY ([JobApplicationFk]) REFERENCES [HR_JobApplication]([Id]),
            CONSTRAINT [FK_HR_StageHistory_FromStage]
                FOREIGN KEY ([FromStageId]) REFERENCES [HR_ApplicationStage]([Id]),
            CONSTRAINT [FK_HR_StageHistory_ToStage]
                FOREIGN KEY ([ToStageFk])   REFERENCES [HR_ApplicationStage]([Id])
        );
    END;

END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260605030000_AddHrRecruitmentSprint4'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260605030000_AddHrRecruitmentSprint4', N'8.0.24');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260605040000_AddHrRecruitmentSprint5'
)
BEGIN

    -- ── HR_InterviewRound ─────────────────────────────────────────
    IF OBJECT_ID(N'[HR_InterviewRound]', N'U') IS NULL
    BEGIN
        CREATE TABLE [HR_InterviewRound]
        (
            [Id]          NUMERIC(18,0) IDENTITY(1,1) NOT NULL CONSTRAINT [PK_HR_InterviewRound] PRIMARY KEY,
            [JobPostingId] NUMERIC(18,0) NOT NULL,
            [RoundName]   NVARCHAR(100) NOT NULL,
            [RoundOrder]  INT NOT NULL,
            [IsMandatory] BIT NOT NULL CONSTRAINT [DF_HR_InterviewRound_IsMandatory] DEFAULT(1),
            [IsActive]    BIT NOT NULL CONSTRAINT [DF_HR_InterviewRound_IsActive]    DEFAULT(1)
        );
    END;

    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_HR_InterviewRound_Posting_Order' AND object_id=OBJECT_ID(N'[HR_InterviewRound]'))
        CREATE UNIQUE INDEX [IX_HR_InterviewRound_Posting_Order]
        ON [HR_InterviewRound] ([JobPostingId], [RoundOrder])
        WHERE [IsActive] = 1;

    -- ── HR_InterviewSchedule ──────────────────────────────────────
    IF OBJECT_ID(N'[HR_InterviewSchedule]', N'U') IS NULL
    BEGIN
        CREATE TABLE [HR_InterviewSchedule]
        (
            [Id]                  NUMERIC(18,0) IDENTITY(1,1) NOT NULL CONSTRAINT [PK_HR_InterviewSchedule] PRIMARY KEY,
            [JobApplicationId]    NUMERIC(18,0) NOT NULL,
            [InterviewRoundId]    NUMERIC(18,0) NOT NULL,
            [ScheduledDateTime]   DATETIME2     NOT NULL,
            [MeetingLink]         NVARCHAR(1000) NULL,
            [Location]            NVARCHAR(500)  NULL,
            [Status]              NVARCHAR(50)  NOT NULL,
            [CreatedBy]           NUMERIC(18,0) NOT NULL,
            [CreatedDate]         DATETIME2     NOT NULL,
            CONSTRAINT [FK_HR_InterviewSchedule_HR_JobApplication]
                FOREIGN KEY ([JobApplicationId]) REFERENCES [HR_JobApplication]([Id]),
            CONSTRAINT [FK_HR_InterviewSchedule_HR_InterviewRound]
                FOREIGN KEY ([InterviewRoundId]) REFERENCES [HR_InterviewRound]([Id])
        );
    END;

    -- ── HR_InterviewPanel ─────────────────────────────────────────
    IF OBJECT_ID(N'[HR_InterviewPanel]', N'U') IS NULL
    BEGIN
        CREATE TABLE [HR_InterviewPanel]
        (
            [Id]                  NUMERIC(18,0) IDENTITY(1,1) NOT NULL CONSTRAINT [PK_HR_InterviewPanel] PRIMARY KEY,
            [InterviewScheduleId] NUMERIC(18,0) NOT NULL,
            [EmployeeFk]          NUMERIC(18,0) NOT NULL,
            CONSTRAINT [FK_HR_InterviewPanel_HR_InterviewSchedule]
                FOREIGN KEY ([InterviewScheduleId]) REFERENCES [HR_InterviewSchedule]([Id])
        );
    END;

    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_HR_InterviewPanel_Schedule_Employee' AND object_id=OBJECT_ID(N'[HR_InterviewPanel]'))
        CREATE UNIQUE INDEX [IX_HR_InterviewPanel_Schedule_Employee]
        ON [HR_InterviewPanel] ([InterviewScheduleId], [EmployeeFk]);

    -- ── HR_InterviewFeedback ──────────────────────────────────────
    IF OBJECT_ID(N'[HR_InterviewFeedback]', N'U') IS NULL
    BEGIN
        CREATE TABLE [HR_InterviewFeedback]
        (
            [Id]                     NUMERIC(18,0) IDENTITY(1,1) NOT NULL CONSTRAINT [PK_HR_InterviewFeedback] PRIMARY KEY,
            [InterviewScheduleFk]    NUMERIC(18,0) NOT NULL,
            [InterviewerEmployeeFk]  NUMERIC(18,0) NOT NULL,
            [OverallScore]           DECIMAL(5,2)  NULL,
            [Recommendation]         NVARCHAR(50)  NOT NULL,
            [Strengths]              NVARCHAR(MAX) NULL,
            [Concerns]               NVARCHAR(MAX) NULL,
            [Comments]               NVARCHAR(MAX) NULL,
            [SubmittedDate]          DATETIME2     NOT NULL,
            CONSTRAINT [FK_HR_InterviewFeedback_HR_InterviewSchedule]
                FOREIGN KEY ([InterviewScheduleFk]) REFERENCES [HR_InterviewSchedule]([Id])
        );
    END;

    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_HR_InterviewFeedback_Schedule_Interviewer' AND object_id=OBJECT_ID(N'[HR_InterviewFeedback]'))
        CREATE UNIQUE INDEX [IX_HR_InterviewFeedback_Schedule_Interviewer]
        ON [HR_InterviewFeedback] ([InterviewScheduleFk], [InterviewerEmployeeFk]);

    -- ── HR_EvaluationCriteria ─────────────────────────────────────
    IF OBJECT_ID(N'[HR_EvaluationCriteria]', N'U') IS NULL
    BEGIN
        CREATE TABLE [HR_EvaluationCriteria]
        (
            [Id]            NUMERIC(18,0) IDENTITY(1,1) NOT NULL CONSTRAINT [PK_HR_EvaluationCriteria] PRIMARY KEY,
            [JobPostingId]  NUMERIC(18,0) NOT NULL,
            [CriteriaName]  NVARCHAR(200) NOT NULL,
            [MaxScore]      DECIMAL(5,2)  NOT NULL
        );
    END;

    -- ── HR_EvaluationScore ────────────────────────────────────────
    IF OBJECT_ID(N'[HR_EvaluationScore]', N'U') IS NULL
    BEGIN
        CREATE TABLE [HR_EvaluationScore]
        (
            [Id]                   NUMERIC(18,0) IDENTITY(1,1) NOT NULL CONSTRAINT [PK_HR_EvaluationScore] PRIMARY KEY,
            [InterviewFeedbackFk]  NUMERIC(18,0) NOT NULL,
            [EvaluationCriteriaFk] NUMERIC(18,0) NOT NULL,
            [Score]                DECIMAL(5,2)  NOT NULL,
            CONSTRAINT [FK_HR_EvaluationScore_HR_InterviewFeedback]
                FOREIGN KEY ([InterviewFeedbackFk])  REFERENCES [HR_InterviewFeedback]([Id]),
            CONSTRAINT [FK_HR_EvaluationScore_HR_EvaluationCriteria]
                FOREIGN KEY ([EvaluationCriteriaFk]) REFERENCES [HR_EvaluationCriteria]([Id])
        );
    END;

    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_HR_EvaluationScore_Feedback_Criteria' AND object_id=OBJECT_ID(N'[HR_EvaluationScore]'))
        CREATE UNIQUE INDEX [IX_HR_EvaluationScore_Feedback_Criteria]
        ON [HR_EvaluationScore] ([InterviewFeedbackFk], [EvaluationCriteriaFk]);

END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260605040000_AddHrRecruitmentSprint5'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260605040000_AddHrRecruitmentSprint5', N'8.0.24');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260605050000_AddHrRecruitmentSprint6'
)
BEGIN

    -- ── HR_Offer ─────────────────────────────────────────────────
    IF OBJECT_ID(N'[HR_Offer]', N'U') IS NULL
    BEGIN
        CREATE TABLE [HR_Offer]
        (
            [Id]                 NUMERIC(18,0) IDENTITY(1,1) NOT NULL CONSTRAINT [PK_HR_Offer] PRIMARY KEY,
            [JobApplicationFk]   NUMERIC(18,0) NULL,
            [OfferNumber]        NVARCHAR(50)  NULL,
            [ProposedSalary]     DECIMAL(18,2) NOT NULL,
            [ProposedJoiningDate] DATE         NULL,
            [ExpiryDate]         DATE          NULL,
            [Status]             NVARCHAR(50)  NULL CONSTRAINT [DF_HR_Offer_Status] DEFAULT(N'Draft'),
            [CreatedBy]          NVARCHAR(50)  NULL,
            [CreatedOn]          DATETIME2     NULL,
            [UpdatedBy]          NVARCHAR(50)  NULL,
            [UpdatedOn]          DATETIME2     NULL,
            CONSTRAINT [FK_HR_Offer_HR_JobApplication]
                FOREIGN KEY ([JobApplicationFk]) REFERENCES [HR_JobApplication]([Id])
        );
    END;

    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_HR_Offer_OfferNumber_Unique' AND object_id=OBJECT_ID(N'[HR_Offer]'))
        CREATE UNIQUE INDEX [IX_HR_Offer_OfferNumber_Unique]
        ON [HR_Offer] ([OfferNumber])
        WHERE [OfferNumber] IS NOT NULL;

    -- ── HR_OfferVersion ──────────────────────────────────────────
    IF OBJECT_ID(N'[HR_OfferVersion]', N'U') IS NULL
    BEGIN
        CREATE TABLE [HR_OfferVersion]
        (
            [Id]          NUMERIC(18,0) IDENTITY(1,1) NOT NULL CONSTRAINT [PK_HR_OfferVersion] PRIMARY KEY,
            [OfferFk]     NUMERIC(18,0) NULL,
            [VersionNo]   INT           NULL,
            [Salary]      DECIMAL(18,2) NULL,
            [JoiningDate] DATE          NULL,
            [Remarks]     NVARCHAR(1000) NULL,
            [CreatedBy]   NUMERIC(18,0) NULL,
            [CreatedDate] DATETIME2     NULL,
            CONSTRAINT [FK_HR_OfferVersion_HR_Offer]
                FOREIGN KEY ([OfferFk]) REFERENCES [HR_Offer]([Id])
        );
    END;

    -- ── HR_OfferApproval ─────────────────────────────────────────
    IF OBJECT_ID(N'[HR_OfferApproval]', N'U') IS NULL
    BEGIN
        CREATE TABLE [HR_OfferApproval]
        (
            [Id]                   NUMERIC(18,0) IDENTITY(1,1) NOT NULL CONSTRAINT [PK_HR_OfferApproval] PRIMARY KEY,
            [OfferFk]              NUMERIC(18,0) NULL,
            [ApproverEmployeeFk]   NUMERIC(18,0) NULL,
            [ApprovalLevel]        INT           NULL,
            [Status]               NVARCHAR(50)  NULL CONSTRAINT [DF_HR_OfferApproval_Status] DEFAULT(N'Pending'),
            [Comments]             NVARCHAR(1000) NULL,
            [ActionDate]           DATETIME2     NULL,
            CONSTRAINT [FK_HR_OfferApproval_HR_Offer]
                FOREIGN KEY ([OfferFk]) REFERENCES [HR_Offer]([Id])
        );
    END;

    -- ── HR_OfferDocument ─────────────────────────────────────────
    IF OBJECT_ID(N'[HR_OfferDocument]', N'U') IS NULL
    BEGIN
        CREATE TABLE [HR_OfferDocument]
        (
            [Id]            NUMERIC(18,0) IDENTITY(1,1) NOT NULL CONSTRAINT [PK_HR_OfferDocument] PRIMARY KEY,
            [OfferFk]       NUMERIC(18,0) NULL,
            [FileName]      NVARCHAR(500)  NULL,
            [FilePath]      NVARCHAR(1000) NOT NULL,
            [GeneratedDate] DATETIME2      NULL,
            CONSTRAINT [FK_HR_OfferDocument_HR_Offer]
                FOREIGN KEY ([OfferFk]) REFERENCES [HR_Offer]([Id])
        );
    END;

    -- ── HR_OfferResponse ─────────────────────────────────────────
    IF OBJECT_ID(N'[HR_OfferResponse]', N'U') IS NULL
    BEGIN
        CREATE TABLE [HR_OfferResponse]
        (
            [Id]           NUMERIC(18,0) IDENTITY(1,1) NOT NULL CONSTRAINT [PK_HR_OfferResponse] PRIMARY KEY,
            [OfferFk]      NUMERIC(18,0) NULL,
            [ResponseType] NVARCHAR(50)  NOT NULL,
            [ResponseDate] DATETIME2     NOT NULL,
            [Comments]     NVARCHAR(1000) NULL,
            CONSTRAINT [FK_HR_OfferResponse_HR_Offer]
                FOREIGN KEY ([OfferFk]) REFERENCES [HR_Offer]([Id])
        );
    END;

    -- ── HR_HiringDecision ────────────────────────────────────────
    IF OBJECT_ID(N'[HR_HiringDecision]', N'U') IS NULL
    BEGIN
        CREATE TABLE [HR_HiringDecision]
        (
            [Id]               NUMERIC(18,0) IDENTITY(1,1) NOT NULL CONSTRAINT [PK_HR_HiringDecision] PRIMARY KEY,
            [JobApplicationFk] NUMERIC(18,0) NULL,
            [Decision]         NVARCHAR(50)  NOT NULL,
            [DecisionBy]       NUMERIC(18,0) NULL,
            [DecisionDate]     DATETIME2     NULL,
            [Remarks]          NVARCHAR(1000) NULL,
            CONSTRAINT [FK_HR_HiringDecision_HR_JobApplication]
                FOREIGN KEY ([JobApplicationFk]) REFERENCES [HR_JobApplication]([Id])
        );
    END;

    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_HR_HiringDecision_Application_Unique' AND object_id=OBJECT_ID(N'[HR_HiringDecision]'))
        CREATE UNIQUE INDEX [IX_HR_HiringDecision_Application_Unique]
        ON [HR_HiringDecision] ([JobApplicationFk])
        WHERE [JobApplicationFk] IS NOT NULL;

END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260605050000_AddHrRecruitmentSprint6'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260605050000_AddHrRecruitmentSprint6', N'8.0.24');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260605060000_AddHrRecruitmentSprint7'
)
BEGIN

    -- ── HR_OnboardingTaskTemplate ─────────────────────────────────
    IF OBJECT_ID(N'[HR_OnboardingTaskTemplate]', N'U') IS NULL
    BEGIN
        CREATE TABLE [HR_OnboardingTaskTemplate]
        (
            [Id]                    NUMERIC(18,0) IDENTITY(1,1) NOT NULL CONSTRAINT [PK_HR_OnboardingTaskTemplate] PRIMARY KEY,
            [TaskName]              NVARCHAR(200) NOT NULL,
            [ResponsibleDepartment] NVARCHAR(100) NOT NULL,
            [IsMandatory]           BIT NOT NULL CONSTRAINT [DF_HR_OnboardingTaskTemplate_IsMandatory] DEFAULT(1),
            [IsActive]              BIT NOT NULL CONSTRAINT [DF_HR_OnboardingTaskTemplate_IsActive]    DEFAULT(1)
        );
    END;

    -- Seed default task templates
    IF NOT EXISTS (SELECT 1 FROM [HR_OnboardingTaskTemplate] WHERE [TaskName] = N'Create Email Account')
    BEGIN
        INSERT INTO [HR_OnboardingTaskTemplate] ([TaskName], [ResponsibleDepartment], [IsMandatory], [IsActive]) VALUES
        (N'Create Email Account',   N'IT',       1, 1),
        (N'Allocate Laptop',        N'IT',       1, 1),
        (N'Issue Access Card',      N'Admin',    1, 1),
        (N'Assign Buddy',           N'HR',       0, 1),
        (N'Prepare Workstation',    N'Admin',    1, 1),
        (N'Payroll Registration',   N'Payroll',  1, 1),
        (N'Add to Leave System',    N'HR',       1, 1),
        (N'Safety Induction',       N'HR',       1, 1);
    END;

    -- ── HR_Onboarding ─────────────────────────────────────────────
    IF OBJECT_ID(N'[HR_Onboarding]', N'U') IS NULL
    BEGIN
        CREATE TABLE [HR_Onboarding]
        (
            [Id]                 NUMERIC(18,0) IDENTITY(1,1) NOT NULL CONSTRAINT [PK_HR_Onboarding] PRIMARY KEY,
            [CandidateId]        NUMERIC(18,0) NOT NULL,
            [OfferId]            NUMERIC(18,0) NOT NULL,
            [PlannedJoiningDate] DATE          NOT NULL,
            [ActualJoiningDate]  DATE          NULL,
            [Status]             NVARCHAR(50)  NOT NULL CONSTRAINT [DF_HR_Onboarding_Status] DEFAULT(N'Initiated'),
            [CreatedBy]          NVARCHAR(50)  NULL,
            [CreatedOn]          DATETIME2     NULL,
            [UpdatedBy]          NVARCHAR(50)  NULL,
            [UpdatedOn]          DATETIME2     NULL,
            CONSTRAINT [FK_HR_Onboarding_HR_Candidate]
                FOREIGN KEY ([CandidateId]) REFERENCES [HR_Candidate]([Id]),
            CONSTRAINT [FK_HR_Onboarding_HR_Offer]
                FOREIGN KEY ([OfferId]) REFERENCES [HR_Offer]([Id])
        );
    END;

    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_HR_Onboarding_Offer_Unique' AND object_id=OBJECT_ID(N'[HR_Onboarding]'))
        CREATE UNIQUE INDEX [IX_HR_Onboarding_Offer_Unique]
        ON [HR_Onboarding] ([OfferId]);

    -- ── HR_OnboardingTask ─────────────────────────────────────────
    IF OBJECT_ID(N'[HR_OnboardingTask]', N'U') IS NULL
    BEGIN
        CREATE TABLE [HR_OnboardingTask]
        (
            [Id]                   NUMERIC(18,0) IDENTITY(1,1) NOT NULL CONSTRAINT [PK_HR_OnboardingTask] PRIMARY KEY,
            [OnboardingFk]         NUMERIC(18,0) NOT NULL,
            [TaskTemplateFk]       NUMERIC(18,0) NOT NULL,
            [AssignedToEmployeeFk] NUMERIC(18,0) NULL,
            [DueDate]              DATE          NULL,
            [Status]               NVARCHAR(50)  NOT NULL CONSTRAINT [DF_HR_OnboardingTask_Status] DEFAULT(N'Pending'),
            [CompletedDate]        DATETIME2     NULL,
            [Remarks]              NVARCHAR(1000) NULL,
            CONSTRAINT [FK_HR_OnboardingTask_HR_Onboarding]
                FOREIGN KEY ([OnboardingFk]) REFERENCES [HR_Onboarding]([Id]),
            CONSTRAINT [FK_HR_OnboardingTask_HR_OnboardingTaskTemplate]
                FOREIGN KEY ([TaskTemplateFk]) REFERENCES [HR_OnboardingTaskTemplate]([Id])
        );
    END;

    -- ── HR_OnboardingDocument ─────────────────────────────────────
    IF OBJECT_ID(N'[HR_OnboardingDocument]', N'U') IS NULL
    BEGIN
        CREATE TABLE [HR_OnboardingDocument]
        (
            [Id]            NUMERIC(18,0) IDENTITY(1,1) NOT NULL CONSTRAINT [PK_HR_OnboardingDocument] PRIMARY KEY,
            [OnboardingFk]  NUMERIC(18,0) NOT NULL,
            [DocumentType]  NVARCHAR(100) NOT NULL,
            [FileName]      NVARCHAR(500) NOT NULL,
            [FilePath]      NVARCHAR(1000) NOT NULL,
            [UploadedDate]  DATETIME2     NOT NULL,
            CONSTRAINT [FK_HR_OnboardingDocument_HR_Onboarding]
                FOREIGN KEY ([OnboardingFk]) REFERENCES [HR_Onboarding]([Id])
        );
    END;

    -- ── HR_JoiningConfirmation ────────────────────────────────────
    IF OBJECT_ID(N'[HR_JoiningConfirmation]', N'U') IS NULL
    BEGIN
        CREATE TABLE [HR_JoiningConfirmation]
        (
            [Id]                    NUMERIC(18,0) IDENTITY(1,1) NOT NULL CONSTRAINT [PK_HR_JoiningConfirmation] PRIMARY KEY,
            [OnboardingId]          NUMERIC(18,0) NOT NULL,
            [JoinedDate]            DATE          NOT NULL,
            [ConfirmedByEmployeeFk] NUMERIC(18,0) NOT NULL,
            [Remarks]               NVARCHAR(1000) NULL,
            [ConfirmedDate]         DATETIME2     NOT NULL,
            CONSTRAINT [FK_HR_JoiningConfirmation_HR_Onboarding]
                FOREIGN KEY ([OnboardingId]) REFERENCES [HR_Onboarding]([Id])
        );
    END;

    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_HR_JoiningConfirmation_Onboarding_Unique' AND object_id=OBJECT_ID(N'[HR_JoiningConfirmation]'))
        CREATE UNIQUE INDEX [IX_HR_JoiningConfirmation_Onboarding_Unique]
        ON [HR_JoiningConfirmation] ([OnboardingId]);

END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260605060000_AddHrRecruitmentSprint7'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260605060000_AddHrRecruitmentSprint7', N'8.0.24');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260606000000_RefactorHrRequisitionWorkflowApprovers'
)
BEGIN

    IF OBJECT_ID(N'[HR_DepartmentApprover]', N'U') IS NULL
    BEGIN
        CREATE TABLE [HR_DepartmentApprover]
        (
            [Id] NUMERIC(18,0) IDENTITY(1,1) NOT NULL CONSTRAINT [PK_HR_DepartmentApprover] PRIMARY KEY,
            [DepartmentId] NUMERIC(18,0) NOT NULL,
            [ApproverType] NVARCHAR(50) NOT NULL,
            [EmployeeId] NUMERIC(18,0) NOT NULL,
            [IsActive] BIT NOT NULL CONSTRAINT [DF_HR_DepartmentApprover_IsActive] DEFAULT(1)
        );
    END;

    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [name] = N'IX_HR_DepartmentApprover_Department_Type_Active' AND [object_id] = OBJECT_ID(N'[HR_DepartmentApprover]'))
    BEGIN
        CREATE INDEX [IX_HR_DepartmentApprover_Department_Type_Active]
        ON [HR_DepartmentApprover] ([DepartmentId], [ApproverType], [IsActive]);
    END;

    IF OBJECT_ID(N'[HR_WorkflowStep]', N'U') IS NOT NULL AND COL_LENGTH(N'HR_WorkflowStep', N'ApproverType') IS NULL
    BEGIN
        ALTER TABLE [HR_WorkflowStep] ADD [ApproverType] NVARCHAR(50) NULL;
    END;

END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260606000000_RefactorHrRequisitionWorkflowApprovers'
)
BEGIN

    IF OBJECT_ID(N'[HR_WorkflowStep]', N'U') IS NOT NULL
    BEGIN
        UPDATE [HR_WorkflowStep]
        SET [ApproverType] = CASE
            WHEN [StepOrder] = 1 THEN N'HRBP'
            WHEN [StepOrder] = 2 THEN N'HOD'
            WHEN [StepOrder] = 3 THEN N'Recruiter'
            ELSE REPLACE(REPLACE([StepName], N' ', N''), N'Approval', N'Approver')
        END
        WHERE [ApproverType] IS NULL OR LTRIM(RTRIM([ApproverType])) = N'';

        IF EXISTS (SELECT 1 FROM sys.columns WHERE [object_id] = OBJECT_ID(N'[HR_WorkflowStep]') AND [name] = N'ApproverType' AND [is_nullable] = 1)
        BEGIN
            ALTER TABLE [HR_WorkflowStep] ALTER COLUMN [ApproverType] NVARCHAR(50) NOT NULL;
        END;
    END;

END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260606000000_RefactorHrRequisitionWorkflowApprovers'
)
BEGIN

    IF OBJECT_ID(N'[HR_WorkflowStep]', N'U') IS NOT NULL
    BEGIN
        IF COL_LENGTH(N'HR_WorkflowStep', N'RoleId') IS NOT NULL
        BEGIN
            ALTER TABLE [HR_WorkflowStep] DROP COLUMN [RoleId];
        END;
    END;

END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260606000000_RefactorHrRequisitionWorkflowApprovers'
)
BEGIN

    IF OBJECT_ID(N'[HR_WorkflowInstanceStep]', N'U') IS NULL
    BEGIN
        CREATE TABLE [HR_WorkflowInstanceStep]
        (
            [Id] NUMERIC(18,0) IDENTITY(1,1) NOT NULL CONSTRAINT [PK_HR_WorkflowInstanceStep] PRIMARY KEY,
            [WorkflowInstanceId] NUMERIC(18,0) NOT NULL,
            [WorkflowStepId] NUMERIC(18,0) NOT NULL,
            [StepOrder] INT NOT NULL,
            [ApproverType] NVARCHAR(50) NOT NULL,
            [EmployeeId] NUMERIC(18,0) NOT NULL,
            [Status] NVARCHAR(50) NOT NULL,
            [AssignedDate] DATETIME2 NOT NULL,
            [ActionDate] DATETIME2 NULL,
            [Comments] NVARCHAR(1000) NULL,
            CONSTRAINT [FK_HR_WorkflowInstanceStep_HR_WorkflowInstance_WorkflowInstanceId]
                FOREIGN KEY ([WorkflowInstanceId]) REFERENCES [HR_WorkflowInstance]([Id]),
            CONSTRAINT [FK_HR_WorkflowInstanceStep_HR_WorkflowStep_WorkflowStepId]
                FOREIGN KEY ([WorkflowStepId]) REFERENCES [HR_WorkflowStep]([Id])
        );
    END;

    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [name] = N'IX_HR_WorkflowInstanceStep_Instance_Order' AND [object_id] = OBJECT_ID(N'[HR_WorkflowInstanceStep]'))
    BEGIN
        CREATE UNIQUE INDEX [IX_HR_WorkflowInstanceStep_Instance_Order]
        ON [HR_WorkflowInstanceStep] ([WorkflowInstanceId], [StepOrder]);
    END;

    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [name] = N'IX_HR_WorkflowInstanceStep_Employee_Status' AND [object_id] = OBJECT_ID(N'[HR_WorkflowInstanceStep]'))
    BEGIN
        CREATE INDEX [IX_HR_WorkflowInstanceStep_Employee_Status]
        ON [HR_WorkflowInstanceStep] ([EmployeeId], [Status]);
    END;

    IF OBJECT_ID(N'[HR_WorkflowAction]', N'U') IS NULL
    BEGIN
        CREATE TABLE [HR_WorkflowAction]
        (
            [Id] NUMERIC(18,0) IDENTITY(1,1) NOT NULL CONSTRAINT [PK_HR_WorkflowAction] PRIMARY KEY,
            [WorkflowInstanceStepId] NUMERIC(18,0) NOT NULL,
            [ActionByEmployeeId] NUMERIC(18,0) NOT NULL,
            [ActionType] NVARCHAR(50) NOT NULL,
            [Comments] NVARCHAR(1000) NULL,
            [ActionDate] DATETIME2 NOT NULL,
            CONSTRAINT [FK_HR_WorkflowAction_HR_WorkflowInstanceStep_WorkflowInstanceStepId]
                FOREIGN KEY ([WorkflowInstanceStepId]) REFERENCES [HR_WorkflowInstanceStep]([Id])
        );
    END;

END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260606000000_RefactorHrRequisitionWorkflowApprovers'
)
BEGIN

    DECLARE @WorkflowDefinitionId NUMERIC(18,0);

    SELECT @WorkflowDefinitionId = [Id]
    FROM [HR_WorkflowDefinition]
    WHERE [WorkflowName] = N'Recruitment Requisition Approval';

    IF @WorkflowDefinitionId IS NULL
    BEGIN
        INSERT INTO [HR_WorkflowDefinition] ([WorkflowName], [IsActive], [CreatedBy], [CreatedOn])
        VALUES (N'Recruitment Requisition Approval', 1, N'Migration', SYSDATETIME());

        SET @WorkflowDefinitionId = SCOPE_IDENTITY();
    END;

END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260606000000_RefactorHrRequisitionWorkflowApprovers'
)
BEGIN

    DECLARE @WorkflowDefinitionId NUMERIC(18,0);

    SELECT @WorkflowDefinitionId = [Id]
    FROM [HR_WorkflowDefinition]
    WHERE [WorkflowName] = N'Recruitment Requisition Approval';

    IF NOT EXISTS (SELECT 1 FROM [HR_WorkflowStep] WHERE [WorkflowDefinitionId] = @WorkflowDefinitionId AND [StepOrder] = 1)
    BEGIN
        INSERT INTO [HR_WorkflowStep] ([WorkflowDefinitionId], [StepOrder], [StepName], [ApproverType], [IsFinalStep], [CreatedBy], [CreatedOn])
        VALUES (@WorkflowDefinitionId, 1, N'HRBP Review', N'HRBP', 0, N'Migration', SYSDATETIME());
    END;

    IF NOT EXISTS (SELECT 1 FROM [HR_WorkflowStep] WHERE [WorkflowDefinitionId] = @WorkflowDefinitionId AND [StepOrder] = 2)
    BEGIN
        INSERT INTO [HR_WorkflowStep] ([WorkflowDefinitionId], [StepOrder], [StepName], [ApproverType], [IsFinalStep], [CreatedBy], [CreatedOn])
        VALUES (@WorkflowDefinitionId, 2, N'HOD Approval', N'HOD', 0, N'Migration', SYSDATETIME());
    END;

    IF NOT EXISTS (SELECT 1 FROM [HR_WorkflowStep] WHERE [WorkflowDefinitionId] = @WorkflowDefinitionId AND [StepOrder] = 3)
    BEGIN
        INSERT INTO [HR_WorkflowStep] ([WorkflowDefinitionId], [StepOrder], [StepName], [ApproverType], [IsFinalStep], [CreatedBy], [CreatedOn])
        VALUES (@WorkflowDefinitionId, 3, N'Recruiter Assignment', N'Recruiter', 1, N'Migration', SYSDATETIME());
    END;

END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260606000000_RefactorHrRequisitionWorkflowApprovers'
)
BEGIN

    IF OBJECT_ID(N'[HR_WorkflowHistory]', N'U') IS NOT NULL
    BEGIN
        DROP TABLE [HR_WorkflowHistory];
    END;

END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260606000000_RefactorHrRequisitionWorkflowApprovers'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260606000000_RefactorHrRequisitionWorkflowApprovers', N'8.0.24');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260606010000_AddHrDepartmentApproverFunctions'
)
BEGIN

    IF NOT EXISTS (SELECT 1 FROM [functions] WHERE [route] = N'/HrDepartmentApprovers/Index')
        INSERT INTO [functions] ([Name], [Description], [route], [createdby], [createat], [updatedby], [updateat])
        VALUES (N'Dept. Approvers - List', N'View department approver configuration', N'/HrDepartmentApprovers/Index', 1, SYSDATETIME(), 1, SYSDATETIME());

    IF NOT EXISTS (SELECT 1 FROM [functions] WHERE [route] = N'/HrDepartmentApprovers/Create')
        INSERT INTO [functions] ([Name], [Description], [route], [createdby], [createat], [updatedby], [updateat])
        VALUES (N'Dept. Approvers - Create', N'Add a new department approver', N'/HrDepartmentApprovers/Create', 1, SYSDATETIME(), 1, SYSDATETIME());

    IF NOT EXISTS (SELECT 1 FROM [functions] WHERE [route] = N'/HrDepartmentApprovers/Edit')
        INSERT INTO [functions] ([Name], [Description], [route], [createdby], [createat], [updatedby], [updateat])
        VALUES (N'Dept. Approvers - Edit', N'Edit an existing department approver', N'/HrDepartmentApprovers/Edit', 1, SYSDATETIME(), 1, SYSDATETIME());

END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260606010000_AddHrDepartmentApproverFunctions'
)
BEGIN

    INSERT INTO [FunctionRoles] ([FunctionId], [RoleId], [createdby], [createat])
    SELECT f.[Id], r.[Id], 1, SYSDATETIME()
    FROM [functions] f
    CROSS JOIN [roles] r
    WHERE f.[route] IN (
        N'/HrDepartmentApprovers/Index',
        N'/HrDepartmentApprovers/Create',
        N'/HrDepartmentApprovers/Edit'
    )
    AND r.[Name] = N'Super Admin'
    AND NOT EXISTS (
        SELECT 1 FROM [FunctionRoles] fr
        WHERE fr.[FunctionId] = f.[Id] AND fr.[RoleId] = r.[Id]
    );

END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260606010000_AddHrDepartmentApproverFunctions'
)
BEGIN

    INSERT INTO [FunctionRoles] ([FunctionId], [RoleId], [createdby], [createat])
    SELECT f.[Id], r.[Id], 1, SYSDATETIME()
    FROM [functions] f
    CROSS JOIN [roles] r
    WHERE f.[route] = N'/HrDepartmentApprovers/Index'
    AND r.[Name] IN (N'HRBP', N'HOD')
    AND NOT EXISTS (
        SELECT 1 FROM [FunctionRoles] fr
        WHERE fr.[FunctionId] = f.[Id] AND fr.[RoleId] = r.[Id]
    );

END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260606010000_AddHrDepartmentApproverFunctions'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260606010000_AddHrDepartmentApproverFunctions', N'8.0.24');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260606020000_AddHrRequisitionWorkflowFunctions'
)
BEGIN

    IF NOT EXISTS (SELECT 1 FROM [functions] WHERE [route] = N'/HrRequisitions/Index')
        INSERT INTO [functions] ([Name], [Description], [route], [createdby], [createat], [updatedby], [updateat])
        VALUES (N'Requisitions - List', N'View all HR requisitions', N'/HrRequisitions/Index', 1, SYSDATETIME(), 1, SYSDATETIME());

    IF NOT EXISTS (SELECT 1 FROM [functions] WHERE [route] = N'/HrRequisitions/Create')
        INSERT INTO [functions] ([Name], [Description], [route], [createdby], [createat], [updatedby], [updateat])
        VALUES (N'Requisitions - Create', N'Create a new HR requisition', N'/HrRequisitions/Create', 1, SYSDATETIME(), 1, SYSDATETIME());

    IF NOT EXISTS (SELECT 1 FROM [functions] WHERE [route] = N'/HrRequisitions/Edit')
        INSERT INTO [functions] ([Name], [Description], [route], [createdby], [createat], [updatedby], [updateat])
        VALUES (N'Requisitions - Edit', N'Edit a draft HR requisition', N'/HrRequisitions/Edit', 1, SYSDATETIME(), 1, SYSDATETIME());

    IF NOT EXISTS (SELECT 1 FROM [functions] WHERE [route] = N'/HrRequisitions/Details')
        INSERT INTO [functions] ([Name], [Description], [route], [createdby], [createat], [updatedby], [updateat])
        VALUES (N'Requisitions - Details', N'View requisition details', N'/HrRequisitions/Details', 1, SYSDATETIME(), 1, SYSDATETIME());

    IF NOT EXISTS (SELECT 1 FROM [functions] WHERE [route] = N'/HrRequisitions/Inbox')
        INSERT INTO [functions] ([Name], [Description], [route], [createdby], [createat], [updatedby], [updateat])
        VALUES (N'Requisitions - Approval Inbox', N'View pending requisition approvals assigned to me', N'/HrRequisitions/Inbox', 1, SYSDATETIME(), 1, SYSDATETIME());

    IF NOT EXISTS (SELECT 1 FROM [functions] WHERE [route] = N'/HrRequisitions/Action')
        INSERT INTO [functions] ([Name], [Description], [route], [createdby], [createat], [updatedby], [updateat])
        VALUES (N'Requisitions - Approve/Reject', N'Approve, reject or send back a requisition', N'/HrRequisitions/Action', 1, SYSDATETIME(), 1, SYSDATETIME());

END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260606020000_AddHrRequisitionWorkflowFunctions'
)
BEGIN

    INSERT INTO [FunctionRoles] ([FunctionId], [RoleId], [createdby], [createat])
    SELECT f.[Id], r.[Id], 1, SYSDATETIME()
    FROM [functions] f
    CROSS JOIN [roles] r
    WHERE f.[route] IN (
        N'/HrRequisitions/Index',
        N'/HrRequisitions/Create',
        N'/HrRequisitions/Edit',
        N'/HrRequisitions/Details',
        N'/HrRequisitions/Inbox',
        N'/HrRequisitions/Action'
    )
    AND r.[Name] = N'Super Admin'
    AND NOT EXISTS (
        SELECT 1 FROM [FunctionRoles] fr
        WHERE fr.[FunctionId] = f.[Id] AND fr.[RoleId] = r.[Id]
    );

END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260606020000_AddHrRequisitionWorkflowFunctions'
)
BEGIN

    INSERT INTO [FunctionRoles] ([FunctionId], [RoleId], [createdby], [createat])
    SELECT f.[Id], r.[Id], 1, SYSDATETIME()
    FROM [functions] f
    CROSS JOIN [roles] r
    WHERE f.[route] IN (
        N'/HrRequisitions/Index',
        N'/HrRequisitions/Details',
        N'/HrRequisitions/Inbox',
        N'/HrRequisitions/Action'
    )
    AND r.[Name] IN (N'HRBP', N'HOD', N'Recruiter')
    AND NOT EXISTS (
        SELECT 1 FROM [FunctionRoles] fr
        WHERE fr.[FunctionId] = f.[Id] AND fr.[RoleId] = r.[Id]
    );

END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260606020000_AddHrRequisitionWorkflowFunctions'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260606020000_AddHrRequisitionWorkflowFunctions', N'8.0.24');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609000000_AddHrRequisitionRefactorSprint2Rev'
)
BEGIN

    IF OBJECT_ID(N'[HR_Requisition]', N'U') IS NOT NULL
    BEGIN
        IF COL_LENGTH(N'HR_Requisition', N'InitialDate') IS NULL
            ALTER TABLE [HR_Requisition] ADD [InitialDate] DATE NULL;

        IF COL_LENGTH(N'HR_Requisition', N'PromisedDate') IS NULL
            ALTER TABLE [HR_Requisition] ADD [PromisedDate] DATE NULL;

        IF COL_LENGTH(N'HR_Requisition', N'RequisitionType') IS NULL
            ALTER TABLE [HR_Requisition] ADD [RequisitionType] NVARCHAR(50) NULL;

        IF COL_LENGTH(N'HR_Requisition', N'Nature') IS NULL
            ALTER TABLE [HR_Requisition] ADD [Nature] NVARCHAR(50) NULL;

        IF COL_LENGTH(N'HR_Requisition', N'BudgetAmountPerMonth') IS NULL
            ALTER TABLE [HR_Requisition] ADD [BudgetAmountPerMonth] DECIMAL(18,2) NULL;

        IF COL_LENGTH(N'HR_Requisition', N'ReplacementEmployeeId') IS NULL
            ALTER TABLE [HR_Requisition] ADD [ReplacementEmployeeId] NUMERIC(18,0) NULL;

        IF COL_LENGTH(N'HR_Requisition', N'TransferFromDepartmentId') IS NULL
            ALTER TABLE [HR_Requisition] ADD [TransferFromDepartmentId] NUMERIC(18,0) NULL;

        IF COL_LENGTH(N'HR_Requisition', N'TransferToDepartmentId') IS NULL
            ALTER TABLE [HR_Requisition] ADD [TransferToDepartmentId] NUMERIC(18,0) NULL;
    END;

END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609000000_AddHrRequisitionRefactorSprint2Rev'
)
BEGIN

    IF OBJECT_ID(N'[HR_RequisitionType]', N'U') IS NULL
    BEGIN
        CREATE TABLE [HR_RequisitionType]
        (
            [Id]   NUMERIC(18,0) IDENTITY(1,1) NOT NULL CONSTRAINT [PK_HR_RequisitionType] PRIMARY KEY,
            [Code] NVARCHAR(50)  NOT NULL,
            [Name] NVARCHAR(100) NOT NULL
        );
    END;

    IF NOT EXISTS (SELECT 1 FROM [HR_RequisitionType] WHERE [Code] = N'Replacement')
        INSERT INTO [HR_RequisitionType] ([Code], [Name]) VALUES (N'Replacement', N'Replacement');

    IF NOT EXISTS (SELECT 1 FROM [HR_RequisitionType] WHERE [Code] = N'Transfer')
        INSERT INTO [HR_RequisitionType] ([Code], [Name]) VALUES (N'Transfer', N'Transfer');

    IF NOT EXISTS (SELECT 1 FROM [HR_RequisitionType] WHERE [Code] = N'NewVacancy')
        INSERT INTO [HR_RequisitionType] ([Code], [Name]) VALUES (N'NewVacancy', N'New Vacancy');

END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609000000_AddHrRequisitionRefactorSprint2Rev'
)
BEGIN

    IF OBJECT_ID(N'[HR_RequisitionNature]', N'U') IS NULL
    BEGIN
        CREATE TABLE [HR_RequisitionNature]
        (
            [Id]   NUMERIC(18,0) IDENTITY(1,1) NOT NULL CONSTRAINT [PK_HR_RequisitionNature] PRIMARY KEY,
            [Code] NVARCHAR(50)  NOT NULL,
            [Name] NVARCHAR(100) NOT NULL
        );
    END;

    IF NOT EXISTS (SELECT 1 FROM [HR_RequisitionNature] WHERE [Code] = N'Budgeted')
        INSERT INTO [HR_RequisitionNature] ([Code], [Name]) VALUES (N'Budgeted', N'Budgeted');

    IF NOT EXISTS (SELECT 1 FROM [HR_RequisitionNature] WHERE [Code] = N'NonBudgeted')
        INSERT INTO [HR_RequisitionNature] ([Code], [Name]) VALUES (N'NonBudgeted', N'Non-Budgeted');

END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609000000_AddHrRequisitionRefactorSprint2Rev'
)
BEGIN

    IF OBJECT_ID(N'[HR_RequisitionSkill]', N'U') IS NULL
    BEGIN
        CREATE TABLE [HR_RequisitionSkill]
        (
            [Id]              NUMERIC(18,0) IDENTITY(1,1) NOT NULL CONSTRAINT [PK_HR_RequisitionSkill] PRIMARY KEY,
            [RequisitionId]   NUMERIC(18,0) NOT NULL,
            [SkillName]       NVARCHAR(200) NOT NULL,
            [YearsExperience] DECIMAL(5,2)  NULL,
            [IsMandatory]     BIT           NOT NULL CONSTRAINT [DF_HR_RequisitionSkill_IsMandatory] DEFAULT(1),
            CONSTRAINT [FK_HR_RequisitionSkill_HR_Requisition]
                FOREIGN KEY ([RequisitionId]) REFERENCES [HR_Requisition]([Id])
        );
    END;

END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609000000_AddHrRequisitionRefactorSprint2Rev'
)
BEGIN

    IF OBJECT_ID(N'[HR_RequisitionOffering]', N'U') IS NULL
    BEGIN
        CREATE TABLE [HR_RequisitionOffering]
        (
            [Id]            NUMERIC(18,0) IDENTITY(1,1) NOT NULL CONSTRAINT [PK_HR_RequisitionOffering] PRIMARY KEY,
            [RequisitionId] NUMERIC(18,0) NOT NULL,
            [OfferingName]  NVARCHAR(200) NOT NULL,
            [Description]   NVARCHAR(500) NULL,
            CONSTRAINT [FK_HR_RequisitionOffering_HR_Requisition]
                FOREIGN KEY ([RequisitionId]) REFERENCES [HR_Requisition]([Id])
        );
    END;

END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609000000_AddHrRequisitionRefactorSprint2Rev'
)
BEGIN

    IF OBJECT_ID(N'[HR_RecruitmentUnit]', N'U') IS NULL
    BEGIN
        CREATE TABLE [HR_RecruitmentUnit]
        (
            [Id]                         NUMERIC(18,0) IDENTITY(1,1) NOT NULL CONSTRAINT [PK_HR_RecruitmentUnit] PRIMARY KEY,
            [UnitName]                   NVARCHAR(200) NOT NULL,
            [RecruitmentHeadEmployeeId]  NUMERIC(18,0) NOT NULL,
            [IsActive]                   BIT           NOT NULL CONSTRAINT [DF_HR_RecruitmentUnit_IsActive] DEFAULT(1)
        );
    END;

END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609000000_AddHrRequisitionRefactorSprint2Rev'
)
BEGIN

    IF OBJECT_ID(N'[HR_RecruitmentUnitDepartment]', N'U') IS NULL
    BEGIN
        CREATE TABLE [HR_RecruitmentUnitDepartment]
        (
            [Id]                NUMERIC(18,0) IDENTITY(1,1) NOT NULL CONSTRAINT [PK_HR_RecruitmentUnitDepartment] PRIMARY KEY,
            [RecruitmentUnitId] NUMERIC(18,0) NOT NULL,
            [DepartmentId]      NUMERIC(18,0) NOT NULL,
            CONSTRAINT [FK_HR_RecruitmentUnitDepartment_HR_RecruitmentUnit]
                FOREIGN KEY ([RecruitmentUnitId]) REFERENCES [HR_RecruitmentUnit]([Id])
        );
    END;

    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [name] = N'IX_HR_RecruitmentUnitDepartment_Unit_Dept'
                   AND [object_id] = OBJECT_ID(N'[HR_RecruitmentUnitDepartment]'))
    BEGIN
        CREATE UNIQUE INDEX [IX_HR_RecruitmentUnitDepartment_Unit_Dept]
        ON [HR_RecruitmentUnitDepartment] ([RecruitmentUnitId], [DepartmentId]);
    END;

END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609000000_AddHrRequisitionRefactorSprint2Rev'
)
BEGIN

    IF OBJECT_ID(N'[HR_RecruitmentTeam]', N'U') IS NULL
    BEGIN
        CREATE TABLE [HR_RecruitmentTeam]
        (
            [Id]                   NUMERIC(18,0) IDENTITY(1,1) NOT NULL CONSTRAINT [PK_HR_RecruitmentTeam] PRIMARY KEY,
            [RecruitmentUnitId]    NUMERIC(18,0) NOT NULL,
            [RecruiterEmployeeId]  NUMERIC(18,0) NOT NULL,
            [IsActive]             BIT           NOT NULL CONSTRAINT [DF_HR_RecruitmentTeam_IsActive] DEFAULT(1),
            CONSTRAINT [FK_HR_RecruitmentTeam_HR_RecruitmentUnit]
                FOREIGN KEY ([RecruitmentUnitId]) REFERENCES [HR_RecruitmentUnit]([Id])
        );
    END;

END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609000000_AddHrRequisitionRefactorSprint2Rev'
)
BEGIN

    IF OBJECT_ID(N'[HR_RequisitionAssignment]', N'U') IS NOT NULL
        AND COL_LENGTH(N'HR_RequisitionAssignment', N'AssignedByEmployeeId') IS NULL
    BEGIN
        ALTER TABLE [HR_RequisitionAssignment] ADD [AssignedByEmployeeId] NUMERIC(18,0) NULL;
    END;

END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609000000_AddHrRequisitionRefactorSprint2Rev'
)
BEGIN

    DECLARE @DefId NUMERIC(18,0);
    SELECT @DefId = [Id] FROM [HR_WorkflowDefinition]
    WHERE [WorkflowName] = N'Recruitment Requisition Approval';

    IF @DefId IS NOT NULL
    BEGIN
        -- Step 1: ensure HRBP
        IF NOT EXISTS (SELECT 1 FROM [HR_WorkflowStep] WHERE [WorkflowDefinitionId] = @DefId AND [StepOrder] = 1)
            INSERT INTO [HR_WorkflowStep] ([WorkflowDefinitionId],[StepOrder],[StepName],[ApproverType],[IsFinalStep],[CreatedBy],[CreatedOn])
            VALUES (@DefId, 1, N'HRBP Review', N'HRBP', 0, N'Migration', SYSDATETIME());
        ELSE
            UPDATE [HR_WorkflowStep]
            SET [StepName] = N'HRBP Review', [ApproverType] = N'HRBP', [IsFinalStep] = 0
            WHERE [WorkflowDefinitionId] = @DefId AND [StepOrder] = 1;

        -- Step 2: ensure HOD
        IF NOT EXISTS (SELECT 1 FROM [HR_WorkflowStep] WHERE [WorkflowDefinitionId] = @DefId AND [StepOrder] = 2)
            INSERT INTO [HR_WorkflowStep] ([WorkflowDefinitionId],[StepOrder],[StepName],[ApproverType],[IsFinalStep],[CreatedBy],[CreatedOn])
            VALUES (@DefId, 2, N'HOD Approval', N'HOD', 0, N'Migration', SYSDATETIME());
        ELSE
            UPDATE [HR_WorkflowStep]
            SET [StepName] = N'HOD Approval', [ApproverType] = N'HOD', [IsFinalStep] = 0
            WHERE [WorkflowDefinitionId] = @DefId AND [StepOrder] = 2;

        -- Step 3: UPDATE old Recruiter step → RecruitmentHead (keeps FK refs intact)
        IF NOT EXISTS (SELECT 1 FROM [HR_WorkflowStep] WHERE [WorkflowDefinitionId] = @DefId AND [StepOrder] = 3)
            INSERT INTO [HR_WorkflowStep] ([WorkflowDefinitionId],[StepOrder],[StepName],[ApproverType],[IsFinalStep],[CreatedBy],[CreatedOn])
            VALUES (@DefId, 3, N'Recruitment Head Review', N'RecruitmentHead', 1, N'Migration', SYSDATETIME());
        ELSE
            UPDATE [HR_WorkflowStep]
            SET [StepName] = N'Recruitment Head Review', [ApproverType] = N'RecruitmentHead', [IsFinalStep] = 1
            WHERE [WorkflowDefinitionId] = @DefId AND [StepOrder] = 3;

        -- Patch any existing WorkflowInstanceStep rows that still carry the old ApproverType
        UPDATE [HR_WorkflowInstanceStep]
        SET [ApproverType] = N'RecruitmentHead'
        WHERE [ApproverType] = N'Recruiter'
          AND [WorkflowInstanceId] IN (
              SELECT [Id] FROM [HR_WorkflowInstance] WHERE [EntityType] = N'Requisition'
          );
    END;

END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609000000_AddHrRequisitionRefactorSprint2Rev'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260609000000_AddHrRequisitionRefactorSprint2Rev', N'8.0.24');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609010000_AddHrRecruitmentUnitFunctions'
)
BEGIN

    IF NOT EXISTS (SELECT 1 FROM [functions] WHERE [route] = N'/HrRecruitmentUnits/Index')
        INSERT INTO [functions] ([Name], [Description], [route], [createdby], [createat], [updatedby], [updateat])
        VALUES (N'Recruitment Units - List', N'View recruitment units', N'/HrRecruitmentUnits/Index', 1, SYSDATETIME(), 1, SYSDATETIME());

    IF NOT EXISTS (SELECT 1 FROM [functions] WHERE [route] = N'/HrRecruitmentUnits/Create')
        INSERT INTO [functions] ([Name], [Description], [route], [createdby], [createat], [updatedby], [updateat])
        VALUES (N'Recruitment Units - Create', N'Create a new recruitment unit', N'/HrRecruitmentUnits/Create', 1, SYSDATETIME(), 1, SYSDATETIME());

    IF NOT EXISTS (SELECT 1 FROM [functions] WHERE [route] = N'/HrRecruitmentUnits/Edit')
        INSERT INTO [functions] ([Name], [Description], [route], [createdby], [createat], [updatedby], [updateat])
        VALUES (N'Recruitment Units - Edit', N'Edit a recruitment unit', N'/HrRecruitmentUnits/Edit', 1, SYSDATETIME(), 1, SYSDATETIME());

    IF NOT EXISTS (SELECT 1 FROM [functions] WHERE [route] = N'/HrRecruitmentUnits/Details')
        INSERT INTO [functions] ([Name], [Description], [route], [createdby], [createat], [updatedby], [updateat])
        VALUES (N'Recruitment Units - Details', N'View recruitment unit details', N'/HrRecruitmentUnits/Details', 1, SYSDATETIME(), 1, SYSDATETIME());

END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609010000_AddHrRecruitmentUnitFunctions'
)
BEGIN

    INSERT INTO [FunctionRoles] ([FunctionId], [RoleId], [createdby], [createat])
    SELECT f.[Id], r.[Id], 1, SYSDATETIME()
    FROM [functions] f
    CROSS JOIN [roles] r
    WHERE f.[route] IN (
        N'/HrRecruitmentUnits/Index',
        N'/HrRecruitmentUnits/Create',
        N'/HrRecruitmentUnits/Edit',
        N'/HrRecruitmentUnits/Details'
    )
    AND r.[Name] = N'Super Admin'
    AND NOT EXISTS (
        SELECT 1 FROM [FunctionRoles] fr
        WHERE fr.[FunctionId] = f.[Id] AND fr.[RoleId] = r.[Id]
    );

END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609010000_AddHrRecruitmentUnitFunctions'
)
BEGIN

    INSERT INTO [FunctionRoles] ([FunctionId], [RoleId], [createdby], [createat])
    SELECT f.[Id], r.[Id], 1, SYSDATETIME()
    FROM [functions] f
    CROSS JOIN [roles] r
    WHERE f.[route] IN (N'/HrRecruitmentUnits/Index', N'/HrRecruitmentUnits/Details')
    AND r.[Name] IN (N'HRBP', N'HOD', N'Recruiter')
    AND NOT EXISTS (
        SELECT 1 FROM [FunctionRoles] fr
        WHERE fr.[FunctionId] = f.[Id] AND fr.[RoleId] = r.[Id]
    );

END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609010000_AddHrRecruitmentUnitFunctions'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260609010000_AddHrRecruitmentUnitFunctions', N'8.0.24');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260610000000_AddRecruitmentDepartmentToUnit'
)
BEGIN

    IF NOT EXISTS (
        SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
        WHERE TABLE_NAME = 'HR_RecruitmentUnit' AND COLUMN_NAME = 'RecruitmentDepartmentId'
    )
    BEGIN
        ALTER TABLE [HR_RecruitmentUnit] ADD [RecruitmentDepartmentId] INT NULL;
    END

END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260610000000_AddRecruitmentDepartmentToUnit'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260610000000_AddRecruitmentDepartmentToUnit', N'8.0.24');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260610010000_SimplifyHrRecruitmentUnit'
)
BEGIN

    IF NOT EXISTS (
        SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
        WHERE TABLE_NAME = 'HR_RecruitmentUnit' AND COLUMN_NAME = 'BusinessDepartmentId'
    )
    BEGIN
        ALTER TABLE [HR_RecruitmentUnit] ADD [BusinessDepartmentId] INT NOT NULL DEFAULT 0;
    END

END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260610010000_SimplifyHrRecruitmentUnit'
)
BEGIN

    IF NOT EXISTS (
        SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
        WHERE TABLE_NAME = 'HR_RecruitmentUnit' AND COLUMN_NAME = 'RecruitmentDepartmentId'
    )
    BEGIN
        ALTER TABLE [HR_RecruitmentUnit] ADD [RecruitmentDepartmentId] INT NOT NULL DEFAULT 0;
    END
    ELSE
    BEGIN
        -- Update any NULLs before making non-nullable
        UPDATE [HR_RecruitmentUnit] SET [RecruitmentDepartmentId] = 0 WHERE [RecruitmentDepartmentId] IS NULL;
        ALTER TABLE [HR_RecruitmentUnit] ALTER COLUMN [RecruitmentDepartmentId] INT NOT NULL;
    END

END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260610010000_SimplifyHrRecruitmentUnit'
)
BEGIN

    IF EXISTS (
        SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
        WHERE TABLE_NAME = 'HR_RecruitmentUnit' AND COLUMN_NAME = 'UnitName'
    )
    BEGIN
        -- Drop the NOT NULL constraint by making the column nullable
        ALTER TABLE [HR_RecruitmentUnit] ALTER COLUMN [UnitName] NVARCHAR(200) NULL;
    END

    IF EXISTS (
        SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
        WHERE TABLE_NAME = 'HR_RecruitmentUnit' AND COLUMN_NAME = 'RecruitmentHeadEmployeeId'
    )
    BEGIN
        ALTER TABLE [HR_RecruitmentUnit] ALTER COLUMN [RecruitmentHeadEmployeeId] NUMERIC(18,0) NULL;
    END

END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260610010000_SimplifyHrRecruitmentUnit'
)
BEGIN

    IF NOT EXISTS (SELECT 1 FROM [functions] WHERE [route] = N'/HrDepartmentApprovers/RecruitmentHead')
    BEGIN
        -- No separate route needed; RecruitmentHead is handled by existing DeptApprovers routes
        -- Just ensure existing DeptApprovers functions cover Create/Edit for this type (they do)
        SELECT 1; -- no-op placeholder
    END

END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260610010000_SimplifyHrRecruitmentUnit'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260610010000_SimplifyHrRecruitmentUnit', N'8.0.24');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260611095509_Documents'
)
BEGIN
    DROP INDEX [IX_EmployeeLeavePolicies_EmployeeId] ON [EmployeeLeavePolicies];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260611095509_Documents'
)
BEGIN
    CREATE TABLE [DocumentCategories] (
        [CategoryID] int NOT NULL IDENTITY,
        [CategoryName] nvarchar(200) NOT NULL,
        [Description] nvarchar(500) NULL,
        [IsActive] bit NOT NULL,
        [createdby] int NOT NULL,
        [createat] datetime2 NOT NULL,
        [updatedby] int NOT NULL,
        [updateat] datetime2 NOT NULL,
        CONSTRAINT [PK_DocumentCategories] PRIMARY KEY ([CategoryID])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260611095509_Documents'
)
BEGIN
    CREATE TABLE [HR_ApplicationStage] (
        [Id] numeric(18,0) NOT NULL IDENTITY,
        [StageName] nvarchar(100) NOT NULL,
        [StageOrder] int NULL,
        [IsActive] bit NOT NULL,
        CONSTRAINT [PK_HR_ApplicationStage] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260611095509_Documents'
)
BEGIN
    CREATE TABLE [HR_Candidate] (
        [Id] numeric(18,0) NOT NULL IDENTITY,
        [FirstName] nvarchar(100) NOT NULL,
        [LastName] nvarchar(100) NULL,
        [Email] nvarchar(200) NOT NULL,
        [Phone] nvarchar(50) NULL,
        [CurrentLocation] nvarchar(200) NULL,
        [TotalExperienceYears] decimal(5,2) NULL,
        [CurrentEmployer] nvarchar(200) NULL,
        [CurrentDesignation] nvarchar(200) NULL,
        [LinkedInProfile] nvarchar(500) NULL,
        [CreatedDate] datetime2 NULL,
        [ModifiedDate] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_HR_Candidate] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260611095509_Documents'
)
BEGIN
    CREATE TABLE [HR_DepartmentApprover] (
        [Id] numeric(18,0) NOT NULL IDENTITY,
        [DepartmentId] numeric(18,0) NOT NULL,
        [ApproverType] nvarchar(50) NOT NULL,
        [EmployeeId] numeric(18,0) NOT NULL,
        [IsActive] bit NOT NULL,
        CONSTRAINT [PK_HR_DepartmentApprover] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260611095509_Documents'
)
BEGIN
    CREATE TABLE [HR_EvaluationCriteria] (
        [Id] numeric(18,0) NOT NULL IDENTITY,
        [JobPostingId] numeric(18,0) NOT NULL,
        [CriteriaName] nvarchar(200) NOT NULL,
        [MaxScore] decimal(5,2) NOT NULL,
        CONSTRAINT [PK_HR_EvaluationCriteria] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260611095509_Documents'
)
BEGIN
    CREATE TABLE [HR_InterviewRound] (
        [Id] numeric(18,0) NOT NULL IDENTITY,
        [JobPostingId] numeric(18,0) NOT NULL,
        [RoundName] nvarchar(100) NOT NULL,
        [RoundOrder] int NOT NULL,
        [IsMandatory] bit NOT NULL,
        [IsActive] bit NOT NULL,
        CONSTRAINT [PK_HR_InterviewRound] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260611095509_Documents'
)
BEGIN
    CREATE TABLE [HR_JobPosting] (
        [Id] numeric(18,0) NOT NULL IDENTITY,
        [RequisitionFK] numeric(18,0) NULL,
        [JobCode] nvarchar(30) NULL,
        [JobTitle] nvarchar(200) NULL,
        [JobDescription] nvarchar(1000) NULL,
        [EmploymentType] nvarchar(50) NULL,
        [Location] nvarchar(200) NULL,
        [PostingStatus] nvarchar(30) NOT NULL,
        [OpenDate] date NOT NULL,
        [CloseDate] date NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedBy] nvarchar(50) NULL,
        [CreatedOn] datetime2 NULL,
        [UpdatedBy] nvarchar(50) NULL,
        [UpdatedOn] datetime2 NULL,
        CONSTRAINT [PK_HR_JobPosting] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260611095509_Documents'
)
BEGIN
    CREATE TABLE [HR_OnboardingTaskTemplate] (
        [Id] numeric(18,0) NOT NULL IDENTITY,
        [TaskName] nvarchar(200) NOT NULL,
        [ResponsibleDepartment] nvarchar(100) NOT NULL,
        [IsMandatory] bit NOT NULL,
        [IsActive] bit NOT NULL,
        CONSTRAINT [PK_HR_OnboardingTaskTemplate] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260611095509_Documents'
)
BEGIN
    CREATE TABLE [HR_PostingChannel] (
        [Id] numeric(18,0) NOT NULL IDENTITY,
        [ChannelName] nvarchar(100) NOT NULL,
        [IsInternal] bit NOT NULL,
        [IsActive] bit NOT NULL,
        CONSTRAINT [PK_HR_PostingChannel] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260611095509_Documents'
)
BEGIN
    CREATE TABLE [HR_Requisition] (
        [Id] numeric(18,0) NOT NULL IDENTITY,
        [RequisitionNo] nvarchar(30) NULL,
        [EmployeeFK] numeric(18,0) NULL,
        [DepartmentFK] numeric(18,0) NULL,
        [PositionTitle] nvarchar(200) NULL,
        [VacancyCount] numeric(18,0) NULL,
        [Reason] nvarchar(1000) NULL,
        [Status] nvarchar(30) NOT NULL,
        [RequiredByDate] date NULL,
        [WorkflowInstanceFK] numeric(18,0) NULL,
        [CreatedBy] nvarchar(50) NULL,
        [CreatedOn] datetime2 NULL,
        [UpdatedBy] nvarchar(50) NULL,
        [UpdatedOn] datetime2 NULL,
        CONSTRAINT [PK_HR_Requisition] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260611095509_Documents'
)
BEGIN
    CREATE TABLE [HR_RequisitionAssignment] (
        [Id] numeric(18,0) NOT NULL IDENTITY,
        [RequisitionFk] numeric(18,0) NULL,
        [RecruiterEmployeeFK] numeric(18,0) NULL,
        [AssignedDate] datetime2 NULL,
        [Notes] nvarchar(1000) NULL,
        [CreatedBy] nvarchar(50) NULL,
        [CreatedOn] datetime2 NULL,
        [UpdatedBy] nvarchar(50) NULL,
        [UpdatedOn] datetime2 NULL,
        CONSTRAINT [PK_HR_RequisitionAssignment] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260611095509_Documents'
)
BEGIN
    CREATE TABLE [HR_WorkflowDefinition] (
        [Id] numeric(18,0) NOT NULL IDENTITY,
        [WorkflowName] nvarchar(100) NOT NULL,
        [IsActive] bit NOT NULL,
        [CreatedBy] nvarchar(50) NULL,
        [CreatedOn] datetime2 NULL,
        [UpdatedBy] nvarchar(50) NULL,
        [UpdatedOn] datetime2 NULL,
        CONSTRAINT [PK_HR_WorkflowDefinition] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260611095509_Documents'
)
BEGIN
    CREATE TABLE [DocumentTypes] (
        [DocumentTypeID] int NOT NULL IDENTITY,
        [CategoryID] int NOT NULL,
        [TypeName] nvarchar(200) NOT NULL,
        [RequiresApproval] bit NOT NULL,
        [HasExpiry] bit NOT NULL,
        [IsActive] bit NOT NULL,
        [createdby] int NOT NULL,
        [createat] datetime2 NOT NULL,
        [updatedby] int NOT NULL,
        [updateat] datetime2 NOT NULL,
        CONSTRAINT [PK_DocumentTypes] PRIMARY KEY ([DocumentTypeID]),
        CONSTRAINT [FK_DocumentTypes_DocumentCategories_CategoryID] FOREIGN KEY ([CategoryID]) REFERENCES [DocumentCategories] ([CategoryID]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260611095509_Documents'
)
BEGIN
    CREATE TABLE [HR_CandidateDocument] (
        [Id] numeric(18,0) NOT NULL IDENTITY,
        [CandidateFk] numeric(18,0) NULL,
        [DocumentType] nvarchar(50) NULL,
        [FileName] nvarchar(500) NULL,
        [FilePath] nvarchar(1000) NULL,
        [UploadedDate] datetime2 NULL,
        CONSTRAINT [PK_HR_CandidateDocument] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_HR_CandidateDocument_HR_Candidate_CandidateFk] FOREIGN KEY ([CandidateFk]) REFERENCES [HR_Candidate] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260611095509_Documents'
)
BEGIN
    CREATE TABLE [HR_JobApplication] (
        [Id] numeric(18,0) NOT NULL IDENTITY,
        [CandidateFK] numeric(18,0) NULL,
        [JobPostingFK] numeric(18,0) NULL,
        [CurrentStageFK] numeric(18,0) NULL,
        [AppliedDate] datetime2 NULL,
        [RecruiterNotes] nvarchar(max) NULL,
        [IsActive] bit NOT NULL,
        CONSTRAINT [PK_HR_JobApplication] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_HR_JobApplication_HR_ApplicationStage_CurrentStageFK] FOREIGN KEY ([CurrentStageFK]) REFERENCES [HR_ApplicationStage] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_HR_JobApplication_HR_Candidate_CandidateFK] FOREIGN KEY ([CandidateFK]) REFERENCES [HR_Candidate] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_HR_JobApplication_HR_JobPosting_JobPostingFK] FOREIGN KEY ([JobPostingFK]) REFERENCES [HR_JobPosting] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260611095509_Documents'
)
BEGIN
    CREATE TABLE [HR_JobPostingChannel] (
        [Id] numeric(18,0) NOT NULL IDENTITY,
        [JobPostingFK] numeric(18,0) NOT NULL,
        [PostingChannelFK] numeric(18,0) NOT NULL,
        [PublishedDate] datetime2 NULL,
        [ExternalReference] nvarchar(500) NULL,
        CONSTRAINT [PK_HR_JobPostingChannel] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_HR_JobPostingChannel_HR_JobPosting_JobPostingFK] FOREIGN KEY ([JobPostingFK]) REFERENCES [HR_JobPosting] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_HR_JobPostingChannel_HR_PostingChannel_PostingChannelFK] FOREIGN KEY ([PostingChannelFK]) REFERENCES [HR_PostingChannel] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260611095509_Documents'
)
BEGIN
    CREATE TABLE [HR_WorkflowStep] (
        [Id] numeric(18,0) NOT NULL IDENTITY,
        [WorkflowDefinitionId] numeric(18,0) NOT NULL,
        [StepOrder] int NOT NULL,
        [StepName] nvarchar(100) NOT NULL,
        [ApproverType] nvarchar(50) NOT NULL,
        [IsFinalStep] bit NOT NULL,
        [CreatedBy] nvarchar(50) NULL,
        [CreatedOn] datetime2 NULL,
        [UpdatedBy] nvarchar(50) NULL,
        [UpdatedOn] datetime2 NULL,
        CONSTRAINT [PK_HR_WorkflowStep] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_HR_WorkflowStep_HR_WorkflowDefinition_WorkflowDefinitionId] FOREIGN KEY ([WorkflowDefinitionId]) REFERENCES [HR_WorkflowDefinition] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260611095509_Documents'
)
BEGIN
    CREATE TABLE [Documents] (
        [DocumentID] bigint NOT NULL IDENTITY,
        [DocumentNo] nvarchar(50) NOT NULL,
        [EmployeeID] int NOT NULL,
        [DocumentTypeID] int NOT NULL,
        [CategoryID] int NOT NULL,
        [Title] nvarchar(500) NOT NULL,
        [Description] nvarchar(max) NULL,
        [FileName] nvarchar(500) NOT NULL,
        [FilePath] nvarchar(1000) NOT NULL,
        [FileExtension] nvarchar(20) NULL,
        [FileSize] bigint NOT NULL,
        [MimeType] nvarchar(100) NULL,
        [UploadDate] datetime2 NOT NULL,
        [UploadedBy] int NOT NULL,
        [Status] nvarchar(50) NULL,
        [EffectiveDate] datetime2 NULL,
        [ExpiryDate] datetime2 NULL,
        [IsConfidential] bit NOT NULL,
        [Remarks] nvarchar(1000) NULL,
        CONSTRAINT [PK_Documents] PRIMARY KEY ([DocumentID]),
        CONSTRAINT [FK_Documents_DocumentCategories_CategoryID] FOREIGN KEY ([CategoryID]) REFERENCES [DocumentCategories] ([CategoryID]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Documents_DocumentTypes_DocumentTypeID] FOREIGN KEY ([DocumentTypeID]) REFERENCES [DocumentTypes] ([DocumentTypeID]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Documents_emp_EmployeeID] FOREIGN KEY ([EmployeeID]) REFERENCES [emp] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Documents_users_UploadedBy] FOREIGN KEY ([UploadedBy]) REFERENCES [users] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260611095509_Documents'
)
BEGIN
    CREATE TABLE [HR_ApplicationStageHistory] (
        [Id] numeric(18,0) NOT NULL IDENTITY,
        [JobApplicationFk] numeric(18,0) NULL,
        [FromStageId] numeric(18,0) NULL,
        [ToStageFk] numeric(18,0) NULL,
        [ChangedBy] numeric(18,0) NULL,
        [Comments] nvarchar(1000) NULL,
        [ChangedDate] datetime2 NULL,
        CONSTRAINT [PK_HR_ApplicationStageHistory] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_HR_ApplicationStageHistory_HR_ApplicationStage_FromStageId] FOREIGN KEY ([FromStageId]) REFERENCES [HR_ApplicationStage] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_HR_ApplicationStageHistory_HR_ApplicationStage_ToStageFk] FOREIGN KEY ([ToStageFk]) REFERENCES [HR_ApplicationStage] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_HR_ApplicationStageHistory_HR_JobApplication_JobApplicationFk] FOREIGN KEY ([JobApplicationFk]) REFERENCES [HR_JobApplication] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260611095509_Documents'
)
BEGIN
    CREATE TABLE [HR_HiringDecision] (
        [Id] numeric(18,0) NOT NULL IDENTITY,
        [JobApplicationFk] numeric(18,0) NULL,
        [Decision] nvarchar(50) NOT NULL,
        [DecisionBy] numeric(18,0) NULL,
        [DecisionDate] datetime2 NULL,
        [Remarks] nvarchar(1000) NULL,
        CONSTRAINT [PK_HR_HiringDecision] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_HR_HiringDecision_HR_JobApplication_JobApplicationFk] FOREIGN KEY ([JobApplicationFk]) REFERENCES [HR_JobApplication] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260611095509_Documents'
)
BEGIN
    CREATE TABLE [HR_InterviewSchedule] (
        [Id] numeric(18,0) NOT NULL IDENTITY,
        [JobApplicationId] numeric(18,0) NOT NULL,
        [InterviewRoundId] numeric(18,0) NOT NULL,
        [ScheduledDateTime] datetime2 NOT NULL,
        [MeetingLink] nvarchar(1000) NULL,
        [Location] nvarchar(500) NULL,
        [Status] nvarchar(50) NOT NULL,
        [CreatedBy] numeric(18,0) NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        CONSTRAINT [PK_HR_InterviewSchedule] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_HR_InterviewSchedule_HR_InterviewRound_InterviewRoundId] FOREIGN KEY ([InterviewRoundId]) REFERENCES [HR_InterviewRound] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_HR_InterviewSchedule_HR_JobApplication_JobApplicationId] FOREIGN KEY ([JobApplicationId]) REFERENCES [HR_JobApplication] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260611095509_Documents'
)
BEGIN
    CREATE TABLE [HR_Offer] (
        [Id] numeric(18,0) NOT NULL IDENTITY,
        [JobApplicationFk] numeric(18,0) NULL,
        [OfferNumber] nvarchar(50) NULL,
        [ProposedSalary] decimal(18,2) NOT NULL,
        [ProposedJoiningDate] date NULL,
        [ExpiryDate] date NULL,
        [Status] nvarchar(50) NULL,
        [CreatedBy] nvarchar(50) NULL,
        [CreatedOn] datetime2 NULL,
        [UpdatedBy] nvarchar(50) NULL,
        [UpdatedOn] datetime2 NULL,
        CONSTRAINT [PK_HR_Offer] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_HR_Offer_HR_JobApplication_JobApplicationFk] FOREIGN KEY ([JobApplicationFk]) REFERENCES [HR_JobApplication] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260611095509_Documents'
)
BEGIN
    CREATE TABLE [HR_WorkflowInstance] (
        [Id] numeric(18,0) NOT NULL IDENTITY,
        [WorkflowDefinitionId] numeric(18,0) NOT NULL,
        [EntityType] nvarchar(50) NOT NULL,
        [EntityId] numeric(18,0) NOT NULL,
        [CurrentStepId] numeric(18,0) NOT NULL,
        [Status] nvarchar(50) NOT NULL,
        [StartedDate] datetime2 NOT NULL,
        [CompletedDate] datetime2 NULL,
        CONSTRAINT [PK_HR_WorkflowInstance] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_HR_WorkflowInstance_HR_WorkflowDefinition_WorkflowDefinitionId] FOREIGN KEY ([WorkflowDefinitionId]) REFERENCES [HR_WorkflowDefinition] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_HR_WorkflowInstance_HR_WorkflowStep_CurrentStepId] FOREIGN KEY ([CurrentStepId]) REFERENCES [HR_WorkflowStep] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260611095509_Documents'
)
BEGIN
    CREATE TABLE [HR_InterviewFeedback] (
        [Id] numeric(18,0) NOT NULL IDENTITY,
        [InterviewScheduleFk] numeric(18,0) NOT NULL,
        [InterviewerEmployeeFk] numeric(18,0) NOT NULL,
        [OverallScore] decimal(5,2) NULL,
        [Recommendation] nvarchar(50) NOT NULL,
        [Strengths] nvarchar(max) NULL,
        [Concerns] nvarchar(max) NULL,
        [Comments] nvarchar(max) NULL,
        [SubmittedDate] datetime2 NOT NULL,
        CONSTRAINT [PK_HR_InterviewFeedback] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_HR_InterviewFeedback_HR_InterviewSchedule_InterviewScheduleFk] FOREIGN KEY ([InterviewScheduleFk]) REFERENCES [HR_InterviewSchedule] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260611095509_Documents'
)
BEGIN
    CREATE TABLE [HR_InterviewPanel] (
        [Id] numeric(18,0) NOT NULL IDENTITY,
        [InterviewScheduleId] numeric(18,0) NOT NULL,
        [EmployeeFk] numeric(18,0) NOT NULL,
        CONSTRAINT [PK_HR_InterviewPanel] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_HR_InterviewPanel_HR_InterviewSchedule_InterviewScheduleId] FOREIGN KEY ([InterviewScheduleId]) REFERENCES [HR_InterviewSchedule] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260611095509_Documents'
)
BEGIN
    CREATE TABLE [HR_OfferApproval] (
        [Id] numeric(18,0) NOT NULL IDENTITY,
        [OfferFk] numeric(18,0) NULL,
        [ApproverEmployeeFk] numeric(18,0) NULL,
        [ApprovalLevel] int NULL,
        [Status] nvarchar(50) NULL,
        [Comments] nvarchar(1000) NULL,
        [ActionDate] datetime2 NULL,
        CONSTRAINT [PK_HR_OfferApproval] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_HR_OfferApproval_HR_Offer_OfferFk] FOREIGN KEY ([OfferFk]) REFERENCES [HR_Offer] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260611095509_Documents'
)
BEGIN
    CREATE TABLE [HR_OfferDocument] (
        [Id] numeric(18,0) NOT NULL IDENTITY,
        [OfferFk] numeric(18,0) NULL,
        [FileName] nvarchar(500) NULL,
        [FilePath] nvarchar(1000) NOT NULL,
        [GeneratedDate] datetime2 NULL,
        CONSTRAINT [PK_HR_OfferDocument] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_HR_OfferDocument_HR_Offer_OfferFk] FOREIGN KEY ([OfferFk]) REFERENCES [HR_Offer] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260611095509_Documents'
)
BEGIN
    CREATE TABLE [HR_OfferResponse] (
        [Id] numeric(18,0) NOT NULL IDENTITY,
        [OfferFk] numeric(18,0) NULL,
        [ResponseType] nvarchar(50) NOT NULL,
        [ResponseDate] datetime2 NOT NULL,
        [Comments] nvarchar(1000) NULL,
        CONSTRAINT [PK_HR_OfferResponse] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_HR_OfferResponse_HR_Offer_OfferFk] FOREIGN KEY ([OfferFk]) REFERENCES [HR_Offer] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260611095509_Documents'
)
BEGIN
    CREATE TABLE [HR_OfferVersion] (
        [Id] numeric(18,0) NOT NULL IDENTITY,
        [OfferFk] numeric(18,0) NULL,
        [VersionNo] int NULL,
        [Salary] decimal(18,2) NULL,
        [JoiningDate] date NULL,
        [Remarks] nvarchar(1000) NULL,
        [CreatedBy] numeric(18,0) NULL,
        [CreatedDate] datetime2 NULL,
        CONSTRAINT [PK_HR_OfferVersion] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_HR_OfferVersion_HR_Offer_OfferFk] FOREIGN KEY ([OfferFk]) REFERENCES [HR_Offer] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260611095509_Documents'
)
BEGIN
    CREATE TABLE [HR_Onboarding] (
        [Id] numeric(18,0) NOT NULL IDENTITY,
        [CandidateId] numeric(18,0) NOT NULL,
        [OfferId] numeric(18,0) NOT NULL,
        [PlannedJoiningDate] date NOT NULL,
        [ActualJoiningDate] date NULL,
        [Status] nvarchar(50) NULL,
        [CreatedBy] nvarchar(50) NULL,
        [CreatedOn] datetime2 NULL,
        [UpdatedBy] nvarchar(50) NULL,
        [UpdatedOn] datetime2 NULL,
        CONSTRAINT [PK_HR_Onboarding] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_HR_Onboarding_HR_Candidate_CandidateId] FOREIGN KEY ([CandidateId]) REFERENCES [HR_Candidate] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_HR_Onboarding_HR_Offer_OfferId] FOREIGN KEY ([OfferId]) REFERENCES [HR_Offer] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260611095509_Documents'
)
BEGIN
    CREATE TABLE [HR_WorkflowInstanceStep] (
        [Id] numeric(18,0) NOT NULL IDENTITY,
        [WorkflowInstanceId] numeric(18,0) NOT NULL,
        [WorkflowStepId] numeric(18,0) NOT NULL,
        [StepOrder] int NOT NULL,
        [ApproverType] nvarchar(50) NOT NULL,
        [EmployeeId] numeric(18,0) NOT NULL,
        [Status] nvarchar(50) NOT NULL,
        [AssignedDate] datetime2 NOT NULL,
        [ActionDate] datetime2 NULL,
        [Comments] nvarchar(1000) NULL,
        CONSTRAINT [PK_HR_WorkflowInstanceStep] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_HR_WorkflowInstanceStep_HR_WorkflowInstance_WorkflowInstanceId] FOREIGN KEY ([WorkflowInstanceId]) REFERENCES [HR_WorkflowInstance] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_HR_WorkflowInstanceStep_HR_WorkflowStep_WorkflowStepId] FOREIGN KEY ([WorkflowStepId]) REFERENCES [HR_WorkflowStep] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260611095509_Documents'
)
BEGIN
    CREATE TABLE [HR_EvaluationScore] (
        [Id] numeric(18,0) NOT NULL IDENTITY,
        [InterviewFeedbackFk] numeric(18,0) NOT NULL,
        [EvaluationCriteriaFk] numeric(18,0) NOT NULL,
        [Score] decimal(5,2) NOT NULL,
        CONSTRAINT [PK_HR_EvaluationScore] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_HR_EvaluationScore_HR_EvaluationCriteria_EvaluationCriteriaFk] FOREIGN KEY ([EvaluationCriteriaFk]) REFERENCES [HR_EvaluationCriteria] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_HR_EvaluationScore_HR_InterviewFeedback_InterviewFeedbackFk] FOREIGN KEY ([InterviewFeedbackFk]) REFERENCES [HR_InterviewFeedback] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260611095509_Documents'
)
BEGIN
    CREATE TABLE [HR_JoiningConfirmation] (
        [Id] numeric(18,0) NOT NULL IDENTITY,
        [OnboardingId] numeric(18,0) NOT NULL,
        [JoinedDate] date NOT NULL,
        [ConfirmedByEmployeeFk] numeric(18,0) NOT NULL,
        [Remarks] nvarchar(1000) NULL,
        [ConfirmedDate] datetime2 NOT NULL,
        CONSTRAINT [PK_HR_JoiningConfirmation] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_HR_JoiningConfirmation_HR_Onboarding_OnboardingId] FOREIGN KEY ([OnboardingId]) REFERENCES [HR_Onboarding] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260611095509_Documents'
)
BEGIN
    CREATE TABLE [HR_OnboardingDocument] (
        [Id] numeric(18,0) NOT NULL IDENTITY,
        [OnboardingFk] numeric(18,0) NOT NULL,
        [DocumentType] nvarchar(100) NOT NULL,
        [FileName] nvarchar(500) NOT NULL,
        [FilePath] nvarchar(1000) NOT NULL,
        [UploadedDate] datetime2 NOT NULL,
        CONSTRAINT [PK_HR_OnboardingDocument] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_HR_OnboardingDocument_HR_Onboarding_OnboardingFk] FOREIGN KEY ([OnboardingFk]) REFERENCES [HR_Onboarding] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260611095509_Documents'
)
BEGIN
    CREATE TABLE [HR_OnboardingTask] (
        [Id] numeric(18,0) NOT NULL IDENTITY,
        [OnboardingFk] numeric(18,0) NOT NULL,
        [TaskTemplateFk] numeric(18,0) NOT NULL,
        [AssignedToEmployeeFk] numeric(18,0) NULL,
        [DueDate] date NULL,
        [Status] nvarchar(50) NOT NULL,
        [CompletedDate] datetime2 NULL,
        [Remarks] nvarchar(1000) NULL,
        CONSTRAINT [PK_HR_OnboardingTask] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_HR_OnboardingTask_HR_OnboardingTaskTemplate_TaskTemplateFk] FOREIGN KEY ([TaskTemplateFk]) REFERENCES [HR_OnboardingTaskTemplate] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_HR_OnboardingTask_HR_Onboarding_OnboardingFk] FOREIGN KEY ([OnboardingFk]) REFERENCES [HR_Onboarding] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260611095509_Documents'
)
BEGIN
    CREATE TABLE [HR_WorkflowAction] (
        [Id] numeric(18,0) NOT NULL IDENTITY,
        [WorkflowInstanceStepId] numeric(18,0) NOT NULL,
        [ActionByEmployeeId] numeric(18,0) NOT NULL,
        [ActionType] nvarchar(50) NOT NULL,
        [Comments] nvarchar(1000) NULL,
        [ActionDate] datetime2 NOT NULL,
        CONSTRAINT [PK_HR_WorkflowAction] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_HR_WorkflowAction_HR_WorkflowInstanceStep_WorkflowInstanceStepId] FOREIGN KEY ([WorkflowInstanceStepId]) REFERENCES [HR_WorkflowInstanceStep] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260611095509_Documents'
)
BEGIN
    CREATE UNIQUE INDEX [IX_EmployeeLeavePolicies_Employee_Policy_Unique] ON [EmployeeLeavePolicies] ([EmployeeId], [LeavePolicyId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260611095509_Documents'
)
BEGIN
    CREATE INDEX [IX_Documents_CategoryID] ON [Documents] ([CategoryID]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260611095509_Documents'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Documents_DocumentNo_Unique] ON [Documents] ([DocumentNo]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260611095509_Documents'
)
BEGIN
    CREATE INDEX [IX_Documents_DocumentTypeID] ON [Documents] ([DocumentTypeID]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260611095509_Documents'
)
BEGIN
    CREATE INDEX [IX_Documents_EmployeeID] ON [Documents] ([EmployeeID]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260611095509_Documents'
)
BEGIN
    CREATE INDEX [IX_Documents_UploadedBy] ON [Documents] ([UploadedBy]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260611095509_Documents'
)
BEGIN
    CREATE INDEX [IX_DocumentTypes_CategoryID] ON [DocumentTypes] ([CategoryID]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260611095509_Documents'
)
BEGIN
    CREATE INDEX [IX_HR_ApplicationStageHistory_FromStageId] ON [HR_ApplicationStageHistory] ([FromStageId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260611095509_Documents'
)
BEGIN
    CREATE INDEX [IX_HR_ApplicationStageHistory_JobApplicationFk] ON [HR_ApplicationStageHistory] ([JobApplicationFk]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260611095509_Documents'
)
BEGIN
    CREATE INDEX [IX_HR_ApplicationStageHistory_ToStageFk] ON [HR_ApplicationStageHistory] ([ToStageFk]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260611095509_Documents'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_HR_Candidate_Email_Unique] ON [HR_Candidate] ([Email]) WHERE [IsDeleted] = 0');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260611095509_Documents'
)
BEGIN
    CREATE INDEX [IX_HR_CandidateDocument_CandidateFk] ON [HR_CandidateDocument] ([CandidateFk]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260611095509_Documents'
)
BEGIN
    CREATE INDEX [IX_HR_DepartmentApprover_Department_Type_Active] ON [HR_DepartmentApprover] ([DepartmentId], [ApproverType], [IsActive]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260611095509_Documents'
)
BEGIN
    CREATE INDEX [IX_HR_EvaluationScore_EvaluationCriteriaFk] ON [HR_EvaluationScore] ([EvaluationCriteriaFk]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260611095509_Documents'
)
BEGIN
    CREATE UNIQUE INDEX [IX_HR_EvaluationScore_Feedback_Criteria] ON [HR_EvaluationScore] ([InterviewFeedbackFk], [EvaluationCriteriaFk]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260611095509_Documents'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_HR_HiringDecision_Application_Unique] ON [HR_HiringDecision] ([JobApplicationFk]) WHERE [JobApplicationFk] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260611095509_Documents'
)
BEGIN
    CREATE UNIQUE INDEX [IX_HR_InterviewFeedback_Schedule_Interviewer] ON [HR_InterviewFeedback] ([InterviewScheduleFk], [InterviewerEmployeeFk]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260611095509_Documents'
)
BEGIN
    CREATE UNIQUE INDEX [IX_HR_InterviewPanel_Schedule_Employee] ON [HR_InterviewPanel] ([InterviewScheduleId], [EmployeeFk]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260611095509_Documents'
)
BEGIN
    CREATE UNIQUE INDEX [IX_HR_InterviewRound_Posting_Order] ON [HR_InterviewRound] ([JobPostingId], [RoundOrder]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260611095509_Documents'
)
BEGIN
    CREATE INDEX [IX_HR_InterviewSchedule_InterviewRoundId] ON [HR_InterviewSchedule] ([InterviewRoundId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260611095509_Documents'
)
BEGIN
    CREATE INDEX [IX_HR_InterviewSchedule_JobApplicationId] ON [HR_InterviewSchedule] ([JobApplicationId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260611095509_Documents'
)
BEGIN
    EXEC(N'CREATE INDEX [IX_HR_JobApplication_Candidate_Posting_Active] ON [HR_JobApplication] ([CandidateFK], [JobPostingFK]) WHERE [IsActive] = 1');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260611095509_Documents'
)
BEGIN
    CREATE INDEX [IX_HR_JobApplication_CurrentStageFK] ON [HR_JobApplication] ([CurrentStageFK]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260611095509_Documents'
)
BEGIN
    CREATE INDEX [IX_HR_JobApplication_JobPostingFK] ON [HR_JobApplication] ([JobPostingFK]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260611095509_Documents'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_HR_JobPosting_JobCode_Unique] ON [HR_JobPosting] ([JobCode]) WHERE [JobCode] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260611095509_Documents'
)
BEGIN
    CREATE UNIQUE INDEX [IX_HR_JobPostingChannel_Posting_Channel_Unique] ON [HR_JobPostingChannel] ([JobPostingFK], [PostingChannelFK]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260611095509_Documents'
)
BEGIN
    CREATE INDEX [IX_HR_JobPostingChannel_PostingChannelFK] ON [HR_JobPostingChannel] ([PostingChannelFK]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260611095509_Documents'
)
BEGIN
    CREATE UNIQUE INDEX [IX_HR_JoiningConfirmation_Onboarding_Unique] ON [HR_JoiningConfirmation] ([OnboardingId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260611095509_Documents'
)
BEGIN
    CREATE INDEX [IX_HR_Offer_JobApplicationFk] ON [HR_Offer] ([JobApplicationFk]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260611095509_Documents'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_HR_Offer_OfferNumber_Unique] ON [HR_Offer] ([OfferNumber]) WHERE [OfferNumber] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260611095509_Documents'
)
BEGIN
    CREATE INDEX [IX_HR_OfferApproval_OfferFk] ON [HR_OfferApproval] ([OfferFk]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260611095509_Documents'
)
BEGIN
    CREATE INDEX [IX_HR_OfferDocument_OfferFk] ON [HR_OfferDocument] ([OfferFk]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260611095509_Documents'
)
BEGIN
    CREATE INDEX [IX_HR_OfferResponse_OfferFk] ON [HR_OfferResponse] ([OfferFk]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260611095509_Documents'
)
BEGIN
    CREATE INDEX [IX_HR_OfferVersion_OfferFk] ON [HR_OfferVersion] ([OfferFk]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260611095509_Documents'
)
BEGIN
    CREATE INDEX [IX_HR_Onboarding_CandidateId] ON [HR_Onboarding] ([CandidateId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260611095509_Documents'
)
BEGIN
    CREATE UNIQUE INDEX [IX_HR_Onboarding_Offer_Unique] ON [HR_Onboarding] ([OfferId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260611095509_Documents'
)
BEGIN
    CREATE INDEX [IX_HR_OnboardingDocument_OnboardingFk] ON [HR_OnboardingDocument] ([OnboardingFk]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260611095509_Documents'
)
BEGIN
    CREATE INDEX [IX_HR_OnboardingTask_OnboardingFk] ON [HR_OnboardingTask] ([OnboardingFk]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260611095509_Documents'
)
BEGIN
    CREATE INDEX [IX_HR_OnboardingTask_TaskTemplateFk] ON [HR_OnboardingTask] ([TaskTemplateFk]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260611095509_Documents'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_HR_Requisition_RequisitionNo_Unique] ON [HR_Requisition] ([RequisitionNo]) WHERE [RequisitionNo] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260611095509_Documents'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_HR_RequisitionAssignment_Requisition_Unique] ON [HR_RequisitionAssignment] ([RequisitionFk]) WHERE [RequisitionFk] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260611095509_Documents'
)
BEGIN
    CREATE INDEX [IX_HR_WorkflowAction_WorkflowInstanceStepId] ON [HR_WorkflowAction] ([WorkflowInstanceStepId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260611095509_Documents'
)
BEGIN
    CREATE INDEX [IX_HR_WorkflowInstance_CurrentStepId] ON [HR_WorkflowInstance] ([CurrentStepId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260611095509_Documents'
)
BEGIN
    CREATE INDEX [IX_HR_WorkflowInstance_Entity] ON [HR_WorkflowInstance] ([EntityType], [EntityId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260611095509_Documents'
)
BEGIN
    CREATE INDEX [IX_HR_WorkflowInstance_WorkflowDefinitionId] ON [HR_WorkflowInstance] ([WorkflowDefinitionId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260611095509_Documents'
)
BEGIN
    CREATE INDEX [IX_HR_WorkflowInstanceStep_Employee_Status] ON [HR_WorkflowInstanceStep] ([EmployeeId], [Status]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260611095509_Documents'
)
BEGIN
    CREATE UNIQUE INDEX [IX_HR_WorkflowInstanceStep_Instance_Order] ON [HR_WorkflowInstanceStep] ([WorkflowInstanceId], [StepOrder]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260611095509_Documents'
)
BEGIN
    CREATE INDEX [IX_HR_WorkflowInstanceStep_WorkflowStepId] ON [HR_WorkflowInstanceStep] ([WorkflowStepId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260611095509_Documents'
)
BEGIN
    CREATE UNIQUE INDEX [IX_HR_WorkflowStep_Definition_Order] ON [HR_WorkflowStep] ([WorkflowDefinitionId], [StepOrder]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260611095509_Documents'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260611095509_Documents', N'8.0.24');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260615000000_AddHrAtsCoreFunctions'
)
BEGIN

    -- HrJobPostings
    IF NOT EXISTS (SELECT 1 FROM [functions] WHERE [route] = N'/HrJobPostings/Index')
        INSERT INTO [functions] ([Name],[Description],[route],[createdby],[createat],[updatedby],[updateat])
        VALUES (N'Job Postings - List', N'View all job postings', N'/HrJobPostings/Index', 1, SYSDATETIME(), 1, SYSDATETIME());

    IF NOT EXISTS (SELECT 1 FROM [functions] WHERE [route] = N'/HrJobPostings/Create')
        INSERT INTO [functions] ([Name],[Description],[route],[createdby],[createat],[updatedby],[updateat])
        VALUES (N'Job Postings - Create', N'Create a new job posting', N'/HrJobPostings/Create', 1, SYSDATETIME(), 1, SYSDATETIME());

    IF NOT EXISTS (SELECT 1 FROM [functions] WHERE [route] = N'/HrJobPostings/Edit')
        INSERT INTO [functions] ([Name],[Description],[route],[createdby],[createat],[updatedby],[updateat])
        VALUES (N'Job Postings - Edit', N'Edit an existing job posting', N'/HrJobPostings/Edit', 1, SYSDATETIME(), 1, SYSDATETIME());

    IF NOT EXISTS (SELECT 1 FROM [functions] WHERE [route] = N'/HrJobPostings/Details')
        INSERT INTO [functions] ([Name],[Description],[route],[createdby],[createat],[updatedby],[updateat])
        VALUES (N'Job Postings - Details', N'View job posting details', N'/HrJobPostings/Details', 1, SYSDATETIME(), 1, SYSDATETIME());

    -- HrCandidates
    IF NOT EXISTS (SELECT 1 FROM [functions] WHERE [route] = N'/HrCandidates/Index')
        INSERT INTO [functions] ([Name],[Description],[route],[createdby],[createat],[updatedby],[updateat])
        VALUES (N'Candidates - List', N'View all candidates', N'/HrCandidates/Index', 1, SYSDATETIME(), 1, SYSDATETIME());


    IF NOT EXISTS (SELECT 1 FROM [functions] WHERE [route] = N'/HrCandidates/Edit')
        INSERT INTO [functions] ([Name],[Description],[route],[createdby],[createat],[updatedby],[updateat])
        VALUES (N'Candidates - Edit', N'Edit a candidate profile', N'/HrCandidates/Edit', 1, SYSDATETIME(), 1, SYSDATETIME());

    IF NOT EXISTS (SELECT 1 FROM [functions] WHERE [route] = N'/HrCandidates/Details')
        INSERT INTO [functions] ([Name],[Description],[route],[createdby],[createat],[updatedby],[updateat])
        VALUES (N'Candidates - Details', N'View candidate profile', N'/HrCandidates/Details', 1, SYSDATETIME(), 1, SYSDATETIME());

    -- HrApplications
    IF NOT EXISTS (SELECT 1 FROM [functions] WHERE [route] = N'/HrApplications/Index')
        INSERT INTO [functions] ([Name],[Description],[route],[createdby],[createat],[updatedby],[updateat])
        VALUES (N'Applications - List', N'View applications for a job posting', N'/HrApplications/Index', 1, SYSDATETIME(), 1, SYSDATETIME());


    IF NOT EXISTS (SELECT 1 FROM [functions] WHERE [route] = N'/HrApplications/Details')
        INSERT INTO [functions] ([Name],[Description],[route],[createdby],[createat],[updatedby],[updateat])
        VALUES (N'Applications - Details', N'View application (candidate card)', N'/HrApplications/Details', 1, SYSDATETIME(), 1, SYSDATETIME());

    IF NOT EXISTS (SELECT 1 FROM [functions] WHERE [route] = N'/HrApplications/Pipeline')
        INSERT INTO [functions] ([Name],[Description],[route],[createdby],[createat],[updatedby],[updateat])
        VALUES (N'Applications - Pipeline', N'View Kanban pipeline for a job posting', N'/HrApplications/Pipeline', 1, SYSDATETIME(), 1, SYSDATETIME());

    IF NOT EXISTS (SELECT 1 FROM [functions] WHERE [route] = N'/HrApplications/MoveStage')
        INSERT INTO [functions] ([Name],[Description],[route],[createdby],[createat],[updatedby],[updateat])
        VALUES (N'Applications - Move Stage', N'Move candidate to another pipeline stage', N'/HrApplications/MoveStage', 1, SYSDATETIME(), 1, SYSDATETIME());

    -- HrInterviews
    IF NOT EXISTS (SELECT 1 FROM [functions] WHERE [route] = N'/HrInterviews/ApplicationInterviews')
        INSERT INTO [functions] ([Name],[Description],[route],[createdby],[createat],[updatedby],[updateat])
        VALUES (N'Interviews - Index', N'View all interviews for an application', N'/HrInterviews/ApplicationInterviews', 1, SYSDATETIME(), 1, SYSDATETIME());

    IF NOT EXISTS (SELECT 1 FROM [functions] WHERE [route] = N'/HrInterviews/Schedule')
        INSERT INTO [functions] ([Name],[Description],[route],[createdby],[createat],[updatedby],[updateat])
        VALUES (N'Interviews - Schedule', N'Schedule an interview for a candidate', N'/HrInterviews/Schedule', 1, SYSDATETIME(), 1, SYSDATETIME());

    IF NOT EXISTS (SELECT 1 FROM [functions] WHERE [route] = N'/HrInterviews/Details')
        INSERT INTO [functions] ([Name],[Description],[route],[createdby],[createat],[updatedby],[updateat])
        VALUES (N'Interviews - Details', N'View interview schedule details', N'/HrInterviews/Details', 1, SYSDATETIME(), 1, SYSDATETIME());

    IF NOT EXISTS (SELECT 1 FROM [functions] WHERE [route] = N'/HrInterviews/SubmitFeedback')
        INSERT INTO [functions] ([Name],[Description],[route],[createdby],[createat],[updatedby],[updateat])
        VALUES (N'Interviews - Submit Feedback', N'Submit interview feedback / scorecard', N'/HrInterviews/SubmitFeedback', 1, SYSDATETIME(), 1, SYSDATETIME());

    IF NOT EXISTS (SELECT 1 FROM [functions] WHERE [route] = N'/HrInterviews/Edit')
        INSERT INTO [functions] ([Name],[Description],[route],[createdby],[createat],[updatedby],[updateat])
        VALUES (N'Interviews - Edit', N'Edit an existing interview schedule', N'/HrInterviews/Edit', 1, SYSDATETIME(), 1, SYSDATETIME());

    IF NOT EXISTS (SELECT 1 FROM [functions] WHERE [route] = N'/HrInterviews/FeedbackDetails')
        INSERT INTO [functions] ([Name],[Description],[route],[createdby],[createat],[updatedby],[updateat])
        VALUES (N'Interviews - Feedback Details', N'View submitted interview feedback', N'/HrInterviews/FeedbackDetails', 1, SYSDATETIME(), 1, SYSDATETIME());

END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260615000000_AddHrAtsCoreFunctions'
)
BEGIN

    INSERT INTO [FunctionRoles] ([FunctionId], [RoleId], [createdby], [createat])
    SELECT f.[Id], r.[Id], 1, SYSDATETIME()
    FROM [functions] f
    CROSS JOIN [roles] r
    WHERE f.[route] IN (
        N'/HrJobPostings/Index',   N'/HrJobPostings/Create',  N'/HrJobPostings/Edit',   N'/HrJobPostings/Details',
        N'/HrCandidates/Index',    N'/HrCandidates/Edit',    N'/HrCandidates/Details',
        N'/HrApplications/Index',  N'/HrApplications/Details',
        N'/HrApplications/Pipeline', N'/HrApplications/MoveStage',
        N'/HrInterviews/ApplicationInterviews', N'/HrInterviews/Schedule', N'/HrInterviews/Edit', N'/HrInterviews/Details',
        N'/HrInterviews/SubmitFeedback', N'/HrInterviews/FeedbackDetails'
    )
    AND r.[Name] = N'Super Admin'
    AND NOT EXISTS (
        SELECT 1 FROM [FunctionRoles] fr WHERE fr.[FunctionId] = f.[Id] AND fr.[RoleId] = r.[Id]
    );

END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260615000000_AddHrAtsCoreFunctions'
)
BEGIN

    INSERT INTO [FunctionRoles] ([FunctionId], [RoleId], [createdby], [createat])
    SELECT f.[Id], r.[Id], 1, SYSDATETIME()
    FROM [functions] f
    CROSS JOIN [roles] r
    WHERE f.[route] IN (
        N'/HrJobPostings/Index',   N'/HrJobPostings/Create',  N'/HrJobPostings/Edit',   N'/HrJobPostings/Details',
        N'/HrCandidates/Index',    N'/HrCandidates/Edit',    N'/HrCandidates/Details',
        N'/HrApplications/Index',  N'/HrApplications/Details',
        N'/HrApplications/Pipeline', N'/HrApplications/MoveStage',
        N'/HrInterviews/ApplicationInterviews', N'/HrInterviews/Schedule', N'/HrInterviews/Edit', N'/HrInterviews/Details',
        N'/HrInterviews/SubmitFeedback', N'/HrInterviews/FeedbackDetails'
    )
    AND r.[Name] = N'HRBP'
    AND NOT EXISTS (
        SELECT 1 FROM [FunctionRoles] fr WHERE fr.[FunctionId] = f.[Id] AND fr.[RoleId] = r.[Id]
    );

END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260615000000_AddHrAtsCoreFunctions'
)
BEGIN

    INSERT INTO [FunctionRoles] ([FunctionId], [RoleId], [createdby], [createat])
    SELECT f.[Id], r.[Id], 1, SYSDATETIME()
    FROM [functions] f
    CROSS JOIN [roles] r
    WHERE f.[route] IN (
        N'/HrJobPostings/Index',    N'/HrJobPostings/Details',
        N'/HrCandidates/Index',    N'/HrCandidates/Edit',    N'/HrCandidates/Details',
        N'/HrApplications/Index',  N'/HrApplications/Details',
        N'/HrApplications/Pipeline', N'/HrApplications/MoveStage',
        N'/HrInterviews/ApplicationInterviews', N'/HrInterviews/Schedule', N'/HrInterviews/Edit', N'/HrInterviews/Details',
        N'/HrInterviews/SubmitFeedback', N'/HrInterviews/FeedbackDetails'
    )
    AND r.[Name] = N'Recruiter'
    AND NOT EXISTS (
        SELECT 1 FROM [FunctionRoles] fr WHERE fr.[FunctionId] = f.[Id] AND fr.[RoleId] = r.[Id]
    );

END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260615000000_AddHrAtsCoreFunctions'
)
BEGIN

    INSERT INTO [FunctionRoles] ([FunctionId], [RoleId], [createdby], [createat])
    SELECT f.[Id], r.[Id], 1, SYSDATETIME()
    FROM [functions] f
    CROSS JOIN [roles] r
    WHERE f.[route] IN (
        N'/HrJobPostings/Index',    N'/HrJobPostings/Details',
        N'/HrCandidates/Index',     N'/HrCandidates/Details',
        N'/HrApplications/Index',   N'/HrApplications/Details', N'/HrApplications/Pipeline',
        N'/HrInterviews/ApplicationInterviews', N'/HrInterviews/Details',   N'/HrInterviews/SubmitFeedback', N'/HrInterviews/FeedbackDetails'
    )
    AND r.[Name] = N'HOD'
    AND NOT EXISTS (
        SELECT 1 FROM [FunctionRoles] fr WHERE fr.[FunctionId] = f.[Id] AND fr.[RoleId] = r.[Id]
    );

END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260615000000_AddHrAtsCoreFunctions'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260615000000_AddHrAtsCoreFunctions', N'8.0.24');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260616000000_AddHrInterviewsApplicationInterviewsFunction'
)
BEGIN

    IF NOT EXISTS (SELECT 1 FROM [functions] WHERE [route] = N'/HrInterviews/ApplicationInterviews')
        INSERT INTO [functions] ([Name],[Description],[route],[createdby],[createat],[updatedby],[updateat])
        VALUES (N'Interviews - Index', N'View all interviews for an application', N'/HrInterviews/ApplicationInterviews', 1, SYSDATETIME(), 1, SYSDATETIME());

    INSERT INTO [FunctionRoles] ([FunctionId], [RoleId], [createdby], [createat])
    SELECT f.[Id], r.[Id], 1, SYSDATETIME()
    FROM [functions] f
    CROSS JOIN [roles] r
    WHERE f.[route] = N'/HrInterviews/ApplicationInterviews'
    AND r.[Name] IN (N'Super Admin', N'HRBP', N'Recruiter', N'HOD')
    AND NOT EXISTS (
        SELECT 1 FROM [FunctionRoles] fr WHERE fr.[FunctionId] = f.[Id] AND fr.[RoleId] = r.[Id]
    );

END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260616000000_AddHrInterviewsApplicationInterviewsFunction'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260616000000_AddHrInterviewsApplicationInterviewsFunction', N'8.0.24');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260616020000_AddLineManagerFeedbackFunction'
)
BEGIN

    IF NOT EXISTS (SELECT 1 FROM [functions] WHERE [route] = N'/HrRequisitions/LineManagerFeedback')
        INSERT INTO [functions] ([Name],[Description],[route],[createdby],[createat],[updatedby],[updateat])
        VALUES (N'Requisitions - Interview Feedback', N'Line manager gives interview feedback for their requisition', N'/HrRequisitions/LineManagerFeedback', 1, SYSDATETIME(), 1, SYSDATETIME());

    INSERT INTO [FunctionRoles] ([FunctionId], [RoleId], [createdby], [createat])
    SELECT f.[Id], r.[Id], 1, SYSDATETIME()
    FROM [functions] f
    CROSS JOIN [roles] r
    WHERE f.[route] = N'/HrRequisitions/LineManagerFeedback'
    AND r.[Name] IN (N'Super Admin', N'HRBP', N'Recruiter', N'HOD', N'Employee')
    AND NOT EXISTS (
        SELECT 1 FROM [FunctionRoles] fr WHERE fr.[FunctionId] = f.[Id] AND fr.[RoleId] = r.[Id]
    );

END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260616020000_AddLineManagerFeedbackFunction'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260616020000_AddLineManagerFeedbackFunction', N'8.0.24');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260616030000_AddLineManagerInterviewFeedbackFunction'
)
BEGIN

    IF NOT EXISTS (SELECT 1 FROM [functions] WHERE [route] = N'/HrInterviews/LineManagerFeedback')
        INSERT INTO [functions] ([Name],[Description],[route],[createdby],[createat],[updatedby],[updateat])
        VALUES (N'Interviews - LM Feedback', N'Line manager gives feedback on an interview', N'/HrInterviews/LineManagerFeedback', 1, SYSDATETIME(), 1, SYSDATETIME());

    INSERT INTO [FunctionRoles] ([FunctionId], [RoleId], [createdby], [createat])
    SELECT f.[Id], r.[Id], 1, SYSDATETIME()
    FROM [functions] f
    CROSS JOIN [roles] r
    WHERE f.[route] = N'/HrInterviews/LineManagerFeedback'
    AND r.[Name] IN (N'Super Admin', N'HRBP', N'Recruiter', N'HOD', N'Employee')
    AND NOT EXISTS (
        SELECT 1 FROM [FunctionRoles] fr WHERE fr.[FunctionId] = f.[Id] AND fr.[RoleId] = r.[Id]
    );

END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260616030000_AddLineManagerInterviewFeedbackFunction'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260616030000_AddLineManagerInterviewFeedbackFunction', N'8.0.24');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260616040000_AddMyInterviewsFunction'
)
BEGIN

    IF NOT EXISTS (SELECT 1 FROM [functions] WHERE [route] = N'/HrRequisitions/MyInterviews')
        INSERT INTO [functions] ([Name],[Description],[route],[createdby],[createat],[updatedby],[updateat])
        VALUES (N'Requisitions - My Interviews', N'Line manager views interviews for their initiated requisitions', N'/HrRequisitions/MyInterviews', 1, SYSDATETIME(), 1, SYSDATETIME());

    INSERT INTO [FunctionRoles] ([FunctionId], [RoleId], [createdby], [createat])
    SELECT f.[Id], r.[Id], 1, SYSDATETIME()
    FROM [functions] f
    CROSS JOIN [roles] r
    WHERE f.[route] = N'/HrRequisitions/MyInterviews'
    AND r.[Name] IN (N'Super Admin', N'HRBP', N'Recruiter', N'HOD', N'Employee')
    AND NOT EXISTS (
        SELECT 1 FROM [FunctionRoles] fr WHERE fr.[FunctionId] = f.[Id] AND fr.[RoleId] = r.[Id]
    );

END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260616040000_AddMyInterviewsFunction'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260616040000_AddMyInterviewsFunction', N'8.0.24');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260617000000_AddFeedbackAuditFields'
)
BEGIN
    ALTER TABLE [HR_InterviewFeedback] ADD [IsSubmitted] bit NOT NULL DEFAULT CAST(0 AS bit);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260617000000_AddFeedbackAuditFields'
)
BEGIN
    ALTER TABLE [HR_InterviewFeedback] ADD [UpdatedBy] numeric(18,0) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260617000000_AddFeedbackAuditFields'
)
BEGIN
    ALTER TABLE [HR_InterviewFeedback] ADD [UpdatedAt] datetime2 NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260617000000_AddFeedbackAuditFields'
)
BEGIN
    ALTER TABLE [HR_InterviewFeedback] ADD [SubmittedBy] numeric(18,0) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260617000000_AddFeedbackAuditFields'
)
BEGIN
    ALTER TABLE [HR_InterviewFeedback] ADD [SubmittedAt] datetime2 NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260617000000_AddFeedbackAuditFields'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260617000000_AddFeedbackAuditFields', N'8.0.24');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260617010000_MoveCriteriaToRound'
)
BEGIN

    IF EXISTS (
        SELECT 1 FROM sys.foreign_keys
        WHERE name = 'FK_HR_EvaluationCriteria_HR_JobPosting'
          AND parent_object_id = OBJECT_ID('HR_EvaluationCriteria'))
        ALTER TABLE [HR_EvaluationCriteria] DROP CONSTRAINT [FK_HR_EvaluationCriteria_HR_JobPosting];

END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260617010000_MoveCriteriaToRound'
)
BEGIN

    ALTER TABLE [HR_EvaluationCriteria] ADD [InterviewRoundId] NUMERIC(18,0) NULL;

END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260617010000_MoveCriteriaToRound'
)
BEGIN

    UPDATE ec
    SET ec.[InterviewRoundId] = r.[Id]
    FROM [HR_EvaluationCriteria] ec
    INNER JOIN (
        SELECT [JobPostingId], MIN([Id]) AS [Id]
        FROM [HR_InterviewRound]
        WHERE [IsActive] = 1
        GROUP BY [JobPostingId]
    ) r ON r.[JobPostingId] = ec.[JobPostingId];

END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260617010000_MoveCriteriaToRound'
)
BEGIN

    DELETE FROM [HR_EvaluationCriteria] WHERE [InterviewRoundId] IS NULL;

END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260617010000_MoveCriteriaToRound'
)
BEGIN

    ALTER TABLE [HR_EvaluationCriteria] ALTER COLUMN [InterviewRoundId] NUMERIC(18,0) NOT NULL;

END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260617010000_MoveCriteriaToRound'
)
BEGIN

    ALTER TABLE [HR_EvaluationCriteria] DROP COLUMN [JobPostingId];

END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260617010000_MoveCriteriaToRound'
)
BEGIN

    ALTER TABLE [HR_EvaluationCriteria]
        ADD CONSTRAINT [FK_HR_EvaluationCriteria_HR_InterviewRound]
        FOREIGN KEY ([InterviewRoundId]) REFERENCES [HR_InterviewRound]([Id]);

END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260617010000_MoveCriteriaToRound'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260617010000_MoveCriteriaToRound', N'8.0.24');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260618113400_AddTrnEmailTable'
)
BEGIN

    IF NOT EXISTS (
        SELECT 1 FROM INFORMATION_SCHEMA.TABLES
        WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'TRN_EML_EMAILS'
    )
    BEGIN
        CREATE TABLE [dbo].[TRN_EML_EMAILS] (
            [EML_ID]        numeric(18,0)  IDENTITY(1,1) NOT NULL,
            [EML_FROM]      nvarchar(max)  NULL,
            [EML_TO]        nvarchar(max)  NULL,
            [EML_CC]        nvarchar(max)  NULL,
            [EML_SUBJECT]   nvarchar(max)  NULL,
            [EML_BODY]      nvarchar(max)  NULL,
            [EML_IS_SENT]   bit            NULL,
            [EML_SENT_ON]   datetime2      NULL,
            [EML_ADD_TAG]   nvarchar(50)   NULL,
            [EML_ADD_STAMP] datetime2      NULL,
            CONSTRAINT [PK_TRN_EML_EMAILS] PRIMARY KEY ([EML_ID])
        );
    END

END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260618113400_AddTrnEmailTable'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260618113400_AddTrnEmailTable', N'8.0.24');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260619082146_AddOfferResponseToken'
)
BEGIN
    ALTER TABLE [HR_Offer] ADD [ResponseToken] nvarchar(64) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260619082146_AddOfferResponseToken'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260619082146_AddOfferResponseToken', N'8.0.24');
END;
GO

COMMIT;
GO

