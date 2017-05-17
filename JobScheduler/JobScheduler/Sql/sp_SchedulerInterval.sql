SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Michael Varey
-- Create date: 2016
-- Comments:	SchedulerInterval Maintenance
-- =============================================
CREATE PROCEDURE [dbo].[sp_SchedulerInterval]
@Command					varchar(10),
@SchedulerIntervalID		int = null,
@IntervalName				varchar(50) = null,
@IsActive					bit = null,
@Interval					int = null,
@Occurrences				varchar(12) = null,
@IntervalType				varchar(10) = null,
@StartTime					time(7) = null,
@EndTime					time(7) = null,
@ExclusionStart				time(7) = null,
@ExclusionEnd				time(7) = null,
@RepeatMinutes				int = null,
@SchedulerIntervalDetails	varchar(max) = null
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	IF @Command = 'Add'
	BEGIN
		Insert Into SchedulerInterval (IntervalName, IsActive, Interval, Occurrences, IntervalType, StartTime, EndTime, ExclusionStart, ExclusionEnd, RepeatMinutes, SchedulerIntervalDetails)
		Values (@IntervalName, @IsActive, @Interval, @Occurrences, @IntervalType, @StartTime, @EndTime, @ExclusionStart, @ExclusionEnd, @RepeatMinutes, @SchedulerIntervalDetails)

		Select @@IDENTITY as SchedulerIntervalID
	END

	IF @Command = 'Update'
	BEGIN
		Update	SchedulerInterval
		Set		IntervalName = isnull(@IntervalName, IntervalName),
				IsActive	= isnull(@IsActive, IsActive),
				Interval = @Interval,
				Occurrences = @Occurrences,
				IntervalType = @IntervalType,
				StartTime = @StartTime,
				EndTime = @EndTime,
				ExclusionStart = @ExclusionStart,
				ExclusionEnd = @ExclusionEnd,
				RepeatMinutes = @RepeatMinutes,
				SchedulerIntervalDetails = @SchedulerIntervalDetails
		Where	SchedulerIntervalID = @SchedulerIntervalID
	END

	IF @Command = 'List'
	BEGIN
		Select	*
		From	SchedulerInterval
	END

	IF @Command = 'DropDown'
	BEGIN
		Select	*
		From	SchedulerInterval
		Where	IsActive = 1
		Order by IntervalName
	END

	IF @Command = 'Delete'
	BEGIN
		Delete From SchedulerInterval
		Where	SchedulerIntervalID = @SchedulerIntervalID
	END

	IF @Command = 'Get'
	BEGIN
		Select	*
		From	SchedulerInterval
		Where	SchedulerIntervalID = @SchedulerIntervalID
	END
END
GO
