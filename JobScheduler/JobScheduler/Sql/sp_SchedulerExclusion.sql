SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Michael Varey
-- Create date: 2016
-- Comments:	SchedulerExclusion Maintenance
-- =============================================
ALTER PROCEDURE [dbo].[sp_SchedulerExclusion]
@Command				varchar(10),
@SchedulerExclusionID	int = null,
@SchedulerAgentID		int = null,
@IsActive				bit = null,
@Sunday					bit = null,
@Monday					bit = null,
@Tuesday				bit = null,
@Wednesday				bit = null,
@Thursday				bit = null,
@Friday					bit = null,
@Saturday				bit = null,
@StartTime				time(7) = null,
@EndTime				time(7) = null,
@SpecificDate			date = null
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	IF @Command = 'Add'
	BEGIN
		Insert Into SchedulerExclusion (SchedulerAgentID, IsActive, Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, StartTime, EndTime, SpecificDate)
		Values (@SchedulerAgentID, @IsActive, @Sunday, @Monday, @Tuesday, @Wednesday, @Thursday, @Friday, @Saturday, @StartTime, @EndTime, @SpecificDate)

		Select @@IDENTITY as SchedulerExclusionID
	END

	IF @Command = 'Update'
	BEGIN
		Update	SchedulerExclusion
		Set		IsActive	= isnull(@IsActive, IsActive),
				Sunday = @Sunday, 
				Monday = @Monday,
				Tuesday = @Tuesday,
				Wednesday = @Wednesday,
				Thursday = @Thursday,
				Friday = @Friday,
				Saturday = @Saturday,
				StartTime = @StartTime,
				EndTime = @EndTime,
				SpecificDate = @SpecificDate
		Where	SchedulerExclusionID = @SchedulerExclusionID
	END

	IF @Command = 'List'
	BEGIN
		Select	*
		From	SchedulerExclusion
		Where	SchedulerAgentID = @SchedulerAgentID
		Or		SchedulerAgentID is null
		Order by StartTime
	END

	IF @Command = 'DropDown'
	BEGIN
		Select	*
		From	SchedulerExclusion
		Where	IsActive = 1
		Order by SchedulerAgentID
	END

	IF @Command = 'Delete'
	BEGIN
		Delete From SchedulerExclusion
		Where	SchedulerExclusionID = @SchedulerExclusionID
	END

	IF @Command = 'Get'
	BEGIN
		Select	*
		From	SchedulerExclusion
		Where	SchedulerExclusionID = @SchedulerExclusionID
	END
END
GO
