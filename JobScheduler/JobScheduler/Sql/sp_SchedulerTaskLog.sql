SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Michael Varey
-- Create date: 2016
-- Comments:	SchedulerTaskLog Maintenance
-- =============================================
ALTER PROCEDURE [dbo].[sp_SchedulerTaskLog]
@Command					varchar(10),
@SchedulerTaskLogId			int = null,
@SchedulerTaskID			int = null,
@ExecutionScheduled			datetime = null,
@SchedulerJobLogID			int = null,
@StandardOutput				varchar(max) = null,
@StandardError				varchar(max) = null,
@SchedulerServerID			int = null,
@ExecutionStart				datetime2(7) = null,
@ExecutionEnd				datetime2(7) = null,
@ExecutionCommandLine		varchar(max) = null,
@ScheduledStart				datetime = null,
@ShowErrors					bit = null
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	IF @Command = 'Add'
	BEGIN
		Insert Into SchedulerTaskLog (SchedulerTaskID, ExecutionScheduled, SchedulerJobLogID, SchedulerServerID, StandardOutput, StandardError, ExecutionStart, ExecutionEnd,
				ExecutionCommandLine, ScheduledStart)
		Values (@SchedulerTaskID, @ExecutionScheduled, @SchedulerJobLogID, @SchedulerServerID, @StandardOutput, @StandardError, @ExecutionStart, @ExecutionEnd,
				@ExecutionCommandLine, @ScheduledStart)

		Select @@Identity as SchedulerTaskLogID
	END

	IF @Command = 'Update'
	BEGIN
		Select	@SchedulerTaskLogID = max(SchedulerTaskLogID)
		From	SchedulerTaskLog
		Where	SchedulerTaskID = @SchedulerTaskID
		And		ExecutionScheduled = @ExecutionScheduled

		Update	SchedulerTaskLog
		Set		StandardOutput	= isnull(@StandardOutput, StandardOutput),
				StandardError   = isnull(@StandardError, StandardError),
				ExecutionStart = isnull(@ExecutionStart, ExecutionStart),
				ExecutionEnd = isnull(@ExecutionEnd, ExecutionEnd),
				ExecutionCommandLine = isnull(@ExecutionCommandLine, ExecutionCommandLine),
				ScheduledStart = isnull(@ScheduledStart, ScheduledStart)
		Where	SchedulerTaskLogID = @SchedulerTaskLogID
	END

	IF @Command = 'List'
	BEGIN
		IF @ShowErrors is null or @ShowErrors = 0
		BEGIN
			Select	*
			From	SchedulerTaskLog t
			Where	ExecutionScheduled between @ExecutionStart and @ExecutionEnd
			And		SchedulerTaskID = isnull(@SchedulerTaskID, SchedulerTaskID)
			And		SchedulerServerID = isnull(@SchedulerServerID, SchedulerServerID)
			Order by ExecutionScheduled
		END
		ELSE
		BEGIN
			Select	*
			From	SchedulerTaskLog t
			Where	ExecutionScheduled between @ExecutionStart and @ExecutionEnd
			And		SchedulerTaskID = isnull(@SchedulerTaskID, SchedulerTaskID)
			And		SchedulerServerID = isnull(@SchedulerServerID, SchedulerServerID)
			And		StandardError > ''
			Order by ExecutionScheduled

		END
	END


	IF @Command = 'Delete'
	BEGIN
		Delete From SchedulerTaskLog
		Where	SchedulerTaskLogID = @SchedulerTaskLogID
	END

	IF @Command = 'Get'
	BEGIN
		Select	tl.*, t.TaskName, tt.TaskTypeName, s.ServerName
		From	SchedulerTaskLog tl
		Join	SchedulerTask t on tl.SchedulerTaskID = t.SchedulerTaskID
		Join	SchedulerTaskType tt on t.SchedulerTaskTypeID = tt.SchedulerTaskTypeID
		Join	SchedulerServer s on tl.SchedulerServerID = s.SchedulerServerID
		Where	SchedulerTaskLogID = @SchedulerTaskLogID
	END

	IF @Command = 'GetByJob'
	BEGIN
		Select	tl.*, t.TaskName, tt.TaskTypeName, s.ServerName
		From	SchedulerTaskLog tl
		Join	SchedulerTask t on tl.SchedulerTaskID = t.SchedulerTaskID
		Join	SchedulerTaskType tt on t.SchedulerTaskTypeID = tt.SchedulerTaskTypeID
		Join	SchedulerServer s on tl.SchedulerServerID = s.SchedulerServerID
		Where	tl.SchedulerJobLogId = @SchedulerJobLogID
	END
END
GO
