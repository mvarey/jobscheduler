SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Michael Varey
-- Create date: 2016
-- Comments:	SchedulerJob Maintenance
-- =============================================
ALTER PROCEDURE [dbo].[sp_SchedulerJob]
@Command					varchar(10),
@SchedulerJobID				int = null,
@JobName					varchar(100) = null,
@SchedulerFolderID			int = null,
@IsActive					bit = null,
@SortOrder					int = null,
@CreateDate					datetime = null,
@CreatedBy					int = null,
@SchedulerIntervalID		int = null,
@SchedulerAgentID			int = null,
@SchedulerOperatorID		int = null,
@LastRunTime				datetime = null,
@NextRunTime				datetime = null,
@MaxInstances				int = null,
@ServerName					varchar(50) = null,
@IsScheduled				bit = null
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	IF @Command = 'Add'
	BEGIN
		Insert Into SchedulerJob (SchedulerFolderID, JobName, IsActive, CreateDate, CreatedBy, SortOrder, 
				SchedulerIntervalID, SchedulerAgentID, SchedulerOperatorId, MaxInstances, IsScheduled)
		Values (@SchedulerFolderID, @JobName, @IsActive, @CreateDate, @CreatedBy, @SortOrder, 
				@SchedulerIntervalID, @SchedulerAgentID, @SchedulerOperatorID, @MaxInstances, @IsScheduled)

		Select @@IDENTITY as SchedulerJobID
	END

	IF @Command = 'Update'
	BEGIN
		Declare @LastIsActive bit
		Select	@LastIsActive = IsActive
		From	SchedulerJob
		Where	SchedulerJobID = @SchedulerJobID

		Update	SchedulerJob
		Set		JobName = isnull(@JobName, JobName),
				IsActive	= isnull(@IsActive, IsActive),
				IsScheduled = isnull(@IsScheduled, IsScheduled),
				SortOrder = isnull(@SortOrder, SortOrder),
				SchedulerIntervalID  = ISNULL(@SchedulerIntervalID, SchedulerIntervalID),
				SchedulerAgentID = isnull(@SchedulerAgentID, SchedulerAgentID),
				SchedulerOperatorId = isnull(@SchedulerOperatorID, SchedulerOperatorId),
				MaxInstances = isnull(@MaxInstances, MaxInstances)
		Where	SchedulerJobID = @SchedulerJobID

		IF @LastIsActive = 1 and @IsActive = 0
		BEGIN
			Update	SchedulerJob
			Set		IsScheduled = 0
			Where	SchedulerJobId = @SchedulerJobID
		END

		IF @LastIsActive = 0 and @IsActive = 1
		BEGIN
			-- Set NextRunTime
			Set @NextRunTime = null

			Select	@SchedulerIntervalID = SchedulerIntervalID
			From	SchedulerJob
			Where	SchedulerJobID = @SchedulerJobID

			Declare @Interval int
			Declare @IntervalType varchar(10)
			Declare @StartTime time(7)
			Declare @EndTime time(7)
			Declare @ExclusionStart time(7)
			Declare @ExclusionEnd time(7)

			Select	@Interval = Interval,
					@IntervalType = IntervalType,
					@StartTime = StartTime,
					@EndTime = EndTime,
					@ExclusionStart = ExclusionStart,
					@ExclusionEnd = ExclusionEnd
			From	SchedulerInterval
			Where	SchedulerIntervalID = @SchedulerIntervalID

			IF convert(time, GETDATE()) between @ExclusionStart and @ExclusionEnd
			BEGIN
				Set @NextRunTime = Convert(DateTime, Convert(varchar(10), getdate(), 101) + ' ' + convert(varchar(20), @ExclusionEnd))
			END

			IF @EndTime is not null
			BEGIN
				IF Convert(time, GETDATE()) > @EndTime
				BEGIN
					-- Tomorrow at StartTime
					Set @NextRunTime = DATEADD(day, 1, convert(varchar(10), getdate(), 101))
					IF @StartTime is not null
					BEGIN
						Set @NextRunTime = DATEADD(hour, datepart(hour, @StartTime), @NextRunTime)
						Set @NextRunTime = DATEADD(minute, datepart(minute, @StartTime), @NextRunTime)
						Set @NextRunTime = DATEADD(second, datepart(second, @StartTime), @NextRunTime)
					END
				END
			END

			IF @StartTime is not null
			BEGIN
				IF Convert(time, GETDATE()) < @StartTime
				BEGIN
					Set @NextRunTime = Convert(DateTime, Convert(varchar(10), getdate(), 101) + ' ' + convert(varchar(20), @StartTime))
				END
			END

			IF @NextRunTime is not null
			BEGIN
				Update	SchedulerJob
				Set		NextRunTime = @NextRunTime,
						IsScheduled = 1
				Where	SchedulerJobID = @SchedulerJobID
			END
			-- Will not set job to scheduled if no next run time can be calculated

		END
	END

	IF @Command = 'Trigger'
	BEGIN
		Update	SchedulerJob
		Set		NextRunTime = GETDATE(),
				IsScheduled = 1
		Where	SchedulerJobID = @SchedulerJobID
		And		IsActive = 1

		Select	*
		From	SchedulerJob
		Where	SchedulerJobId = @SchedulerJobID
	END

	IF @Command = 'List'
	BEGIN
		Select	j.*, i.IntervalName, s.ServerName, o.SchedulerOperatorName
		From	SchedulerJob j
		Left Join SchedulerInterval i on j.SchedulerIntervalID = i.SchedulerIntervalID
		Left Join SchedulerAgent a on j.SchedulerAgentID = a.SchedulerAgentID
		Left Join SchedulerServer s on a.SchedulerServerID = s.SchedulerServerID
		Left Join SchedulerOperator o on j.SchedulerOperatorId = o.SchedulerOperatorId
		Where	SchedulerFolderID = @SchedulerFolderID
		Order by j.SortOrder
	END

	IF @Command = 'DropDown'
	BEGIN
		Select	*
		From	SchedulerJob
		Where	SchedulerFolderID = @SchedulerFolderID
		And		IsActive = 1
		Order by JobName
	END

	IF @Command = 'Delete'
	BEGIN
		Delete From SchedulerJob
		Where	SchedulerJobID = @SchedulerJobID
	END

	IF @Command = 'Get'
	BEGIN
		Select	*
		From	SchedulerJob
		Where	SchedulerJobID = @SchedulerJobID
	END

	IF @Command = 'GetTime'
	BEGIN
		Select	i.*, j.NextRunTime
		From	SchedulerJob j
		Join	SchedulerInterval i on j.SchedulerIntervalID = i.SchedulerIntervalID
		Where	j.SchedulerJobID = @SchedulerJobID

		Select	x.*
		From	SchedulerJob j
		Join	SchedulerFolder f on j.SchedulerFolderID = f.SchedulerFolderID
		Join	SchedulerQueue q on f.SchedulerQueueID = q.SchedulerQueueID
		Join	SchedulerAgent a on q.SchedulerQueueID = a.SchedulerQueueID
		Join	SchedulerExclusion x on a.SchedulerAgentID = x.SchedulerAgentID
		Join	SchedulerServer s on a.SchedulerServerID = s.SchedulerServerID
		Where	j.SchedulerJobID = @SchedulerJobID
		And		s.ServerName = @ServerName
	END

	IF @Command = 'LastRun'
	BEGIN
		Update	SchedulerJob
		Set		LastRunTime = @LastRunTime
		Where	SchedulerJobID = @SchedulerJobID
	END

	IF @Command = 'NextRun'
	BEGIN
		Update	SchedulerJob
		Set		NextRunTime = @NextRunTime
		Where	SchedulerJobID = @SchedulerJobID
	END

	IF @Command = 'GetRun'
	BEGIN
		Select	t.*, tt.TaskTypeCode, tt.TaskTypeName, s.SchedulerServerID, o.SchedulerOperatorId, o.SchedulerOperatorName, 
				j.SchedulerJobID, j.NextRunTime, j.MaxInstances, q.SchedulerQueueID, j.SchedulerIntervalID,
				q.MaxMinutes, q.MaxThreads, j.MaxInstances
		From	SchedulerTask t
		Join	SchedulerTaskType tt on t.SchedulerTaskTypeID = tt.SchedulerTaskTypeID and tt.IsActive = 1
		Join	SchedulerJob j on t.SchedulerJobID = j.SchedulerJobID and j.IsActive = 1
		Join	SchedulerFolder f on j.SchedulerFolderID = f.SchedulerFolderID and f.IsActive = 1
		Join	SchedulerQueue q on f.SchedulerQueueID = q.SchedulerQueueID and q.IsActive = 1 and q.IsRunning = 1
		Join	SchedulerAgent a on q.SchedulerQueueID = a.SchedulerQueueID and a.IsActive = 1
		Join	SchedulerServer s on a.SchedulerServerID = s.SchedulerServerID and s.IsActive = 1 and s.AgentEnabled = 1 and s.ServerName = @ServerName
		Left Join SchedulerOperator o on j.SchedulerOperatorId = o.SchedulerOperatorId and o.IsActive = 1
		Where	t.IsActive = 1
		And		j.NextRunTime < getdate()
		And		a.SchedulerAgentID = isnull(j.SchedulerAgentID, a.SchedulerAgentID)
		Order by j.NextRunTime, j.SchedulerJobID, t.SortOrder

		-- Grab Variable List as Well
		Select	v.*
		From	SchedulerVariable v
		Join	SchedulerFolder f on v.SchedulerFolderID = f.SchedulerFolderID and f.IsActive = 1
		Join	SchedulerQueue q on f.SchedulerQueueID = q.SchedulerQueueID and q.IsActive = 1
		Join	SchedulerAgent a on q.SchedulerQueueID = a.SchedulerQueueID and a.IsActive = 1
		Join	SchedulerServer s on a.SchedulerServerID = s.SchedulerServerID and s.IsActive = 1 and s.AgentEnabled = 1
		Where	s.ServerName = @ServerName
		Order by q.SchedulerQueueID, f.SchedulerFolderID

		Select	s.*
		From	SchedulerServer s
		Where	s.ServerName = @ServerName
		and s.IsActive = 1 and s.AgentEnabled = 1
	END

	IF @Command = 'GetSched'
	BEGIN
		Set		@LastRunTime = GETDATE();

		Select	top 10 j.*
		From	SchedulerJob j
		Where	j.IsScheduled = 1
		And		j.NextRunTime is not null
		And		j.NextRunTime < Convert(datetime, convert(varchar(4), datepart(year, @LastRunTime)) + '-' + convert(varchar(2), datepart(month, @LastRunTime)) + '-' +
										convert(varchar(2), datepart(day, @LastRunTime)) + ' 23:59:59')
		Order by j.NextRunTime
	END


END
GO
