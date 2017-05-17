SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Michael Varey
-- Create date: 2016
-- Comments:	SchedulerQueue Maintenance
-- =============================================
CREATE PROCEDURE [dbo].[sp_SchedulerQueue]
@Command				varchar(10),
@SchedulerQueueID		int = null,
@QueueName				varchar(50) = null,
@IsActive				bit = null,
@IsRunning				bit = null,
@MaxThreads				int = null,
@MaxMinutes				int = null
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	IF @Command = 'Add'
	BEGIN
		Insert Into SchedulerQueue (QueueName, IsActive, IsRunning, MaxThreads, MaxMinutes)
		Values (@QueueName, @IsActive, @IsRunning, @MaxThreads, @MaxMinutes)

		Select @@IDENTITY as SchedulerQueueID
	END

	IF @Command = 'Update'
	BEGIN
		Update	SchedulerQueue
		Set		QueueName = isnull(@QueueName, QueueName),
				IsActive	= isnull(@IsActive, IsActive),
				IsRunning = isnull(@IsRunning, IsRunning),
				MaxThreads = isnull(@MaxThreads, MaxThreads),
				MaxMinutes = isnull(@MaxMinutes, MaxMinutes)
		Where	SchedulerQueueID = @SchedulerQueueID
	END

	IF @Command = 'List'
	BEGIN
		Select	*
		From	SchedulerQueue
		Order by QueueName
	END

	IF @Command = 'DropDown'
	BEGIN
		Select	*
		From	SchedulerQueue
		Where	IsActive = 1
		Order by QueueName
	END

	IF @Command = 'Delete'
	BEGIN
		Delete From SchedulerQueue
		Where	SchedulerQueueID = @SchedulerQueueID
	END

	IF @Command = 'Get'
	BEGIN
		Select	*
		From	SchedulerQueue
		Where	SchedulerQueueID = @SchedulerQueueID
	END
END
GO
