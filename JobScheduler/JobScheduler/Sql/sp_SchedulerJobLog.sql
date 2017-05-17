SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Michael Varey
-- Create date: 2016
-- Comments:	SchedulerJobLog Maintenance
-- =============================================
ALTER PROCEDURE [dbo].[sp_SchedulerJobLog]
@Command					varchar(10),
@SchedulerJobLogID			int = null,
@SchedulerJobId				int = null,
@SchedulerFolderID			int = null,
@ExecutionScheduled			datetime = null,
@SchedulerUserId			int = null,
@ExecutionStart				datetime2(7) = null,
@ExecutionEnd				datetime2(7) = null
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	Declare @Start datetime
	Declare @End datetime
	Set @Start = convert(datetime, convert(varchar(20), getdate(), 101))
	Set @End = Dateadd(day, 1, @Start)

	IF @Command = 'Add'
	BEGIN
		Insert Into SchedulerJobLog (SchedulerJobId, ExecutionScheduled, ExecutionStart, ExecutionEnd, SchedulerUserId)
		Values (@SchedulerJobId, @ExecutionScheduled, @ExecutionStart, @ExecutionEnd, @SchedulerUserId)

		Select @@IDENTITY as SchedulerJobLogID
	END

	IF @Command = 'GetLogID'
	BEGIN
		Select	@SchedulerJobLogID = SchedulerJobLogId
		From	SchedulerJobLog
		Where	SchedulerJobId = @SchedulerJobId
		And		ExecutionScheduled = @ExecutionScheduled

		IF @SchedulerJobLogID is null
		BEGIN
			Insert Into SchedulerJobLog (SchedulerJobId, ExecutionScheduled, ExecutionStart, ExecutionEnd, SchedulerUserId)
			Values (@SchedulerJobId, @ExecutionScheduled, @ExecutionStart, @ExecutionEnd, @SchedulerUserId)

			Select @@IDENTITY as SchedulerJobLogID
		END
		ELSE
		BEGIN
			Select	@SchedulerJobLogID as SchedulerJobLogID
		END
	END

	IF @Command = 'Update'
	BEGIN
		Update	SchedulerJobLog
		Set		ExecutionStart  = ISNULL(@ExecutionStart, ExecutionStart),
				ExecutionEnd = isnull(@ExecutionEnd, ExecutionEnd)
		Where	SchedulerJobLogID = @SchedulerJobLogID
	END

	IF @Command = 'UpdateEx'
	BEGIN
		Update	SchedulerJobLog
		Set		ExecutionStart = ISNULL(ExecutionStart, @ExecutionStart),
				ExecutionEnd = @ExecutionEnd
		Where	SchedulerJobLogId = @SchedulerJobLogID
	END

	IF @Command = 'List'
	BEGIN
		Select	j.*
		From	SchedulerJobLog j
		Where	ExecutionScheduled between @ExecutionStart and @ExecutionEnd
		Order by j.SchedulerUserId
	END

	IF @Command = 'Delete'
	BEGIN
		Delete From SchedulerJobLog
		Where	SchedulerJobLogID = @SchedulerJobLogID
	END

	IF @Command = 'Get'
	BEGIN
		Select	*
		From	SchedulerJobLog
		Where	SchedulerJobLogID = @SchedulerJobLogID
	END

	IF @Command = 'Purge'
	BEGIN
		Select	SchedulerJobLogId
		Into	#Jobs
		From	SchedulerJobLog
		Where	ExecutionScheduled < @ExecutionScheduled

		Delete From SchedulerTaskLog
		Where	SchedulerJobLogId in (Select SchedulerJobLogId from #Jobs)

		Delete From SchedulerJobLog
		Where	SchedulerJobLogId in (Select SchedulerJobLogId from #Jobs)

		Drop Table #Jobs
	END

	IF @Command = 'Active'
	BEGIN
		Select	j.*, jl.ExecutionStart, jl.ExecutionScheduled
		From	SchedulerJobLog jl
		Join	SchedulerJob j on jl.SchedulerJobId = j.SchedulerJobID
		Where	jl.ExecutionEnd is null 
		and		jl.ExecutionStart between @Start and @End
		And		jl.SchedulerJobLogId not in (Select SchedulerJobLogId from SchedulerTaskLog Where StandardError <> '' and ExecutionStart between @Start and @End)
		Order by jl.ExecutionStart
	END

	IF @Command = 'History'
	BEGIN
		Select	top 10 j.*, jl.ExecutionStart, jl.ExecutionScheduled, jl.ExecutionEnd
		From	SchedulerJobLog jl
		Join	SchedulerJob j on jl.SchedulerJobId = j.SchedulerJobID
		Where	jl.ExecutionEnd is not null
		And		jl.ExecutionStart between @Start and @End
		Order by jl.ExecutionStart desc
	END

	IF @Command = 'Error'
	BEGIN
		Select	top 10 j.*, jl.ExecutionStart, jl.ExecutionScheduled, jl.ExecutionEnd
		From	SchedulerJobLog jl
		Join	SchedulerJob j on jl.SchedulerJobId = j.SchedulerJobID
		Where	jl.ExecutionEnd is not null
		And		jl.ExecutionStart between @Start and @End
		And		jl.SchedulerJobLogId in (Select SchedulerJobLogId from SchedulerTaskLog Where StandardError <> '' and ExecutionStart between @Start and @End)
		Order by jl.ExecutionStart desc
	END

	IF @Command = 'JobHistory'
	BEGIN
		Select	*
		From	SchedulerJobLog
		Where	SchedulerJobID = @SchedulerJobID
		Order by ExecutionStart desc
	END
END
GO
