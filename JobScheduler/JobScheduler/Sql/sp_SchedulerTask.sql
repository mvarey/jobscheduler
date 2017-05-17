SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Michael Varey
-- Create date: 2016
-- Comments:	SchedulerTask Maintenance
-- =============================================
ALTER PROCEDURE [dbo].[sp_SchedulerTask]
@Command					varchar(10),
@SchedulerTaskID			int = null,
@TaskName					varchar(100) = null,
@SchedulerJobID				int = null,
@IsActive					bit = null,
@SortOrder					int = null,
@SchedulerTaskTypeID		int = null,
@TaskInformation			varchar(max) = null,
@ServerName					varchar(50) = null
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	IF @Command = 'Add'
	BEGIN
		Insert Into SchedulerTask (TaskName, SchedulerJobID, SchedulerTaskTypeID, IsActive, SortOrder, TaskInformation)
		Values (@TaskName, @SchedulerJobID, @SchedulerTaskTypeID, @IsActive, @SortOrder, @TaskInformation)

		Select @@IDENTITY as SchedulerTaskID
	END

	IF @Command = 'Update'
	BEGIN
		Update	SchedulerTask
		Set		TaskName = isnull(@TaskName, TaskName),
				SchedulerTaskTypeID = isnull(@SchedulerTaskTypeID, SchedulerTaskTypeID),
				IsActive	= isnull(@IsActive, IsActive),
				SortOrder   = isnull(@SortOrder, SortOrder),
				TaskInformation = isnull(@TaskInformation, TaskInformation)
		Where	SchedulerTaskID = @SchedulerTaskID
	END

	IF @Command = 'List'
	BEGIN
		Select	t.*, tt.TaskTypeName, tt.TaskTypeCode
		From	SchedulerTask t
		Join	SchedulerTaskType tt on t.SchedulerTaskTypeID = tt.SchedulerTaskTypeID
		Where	SchedulerJobID = @SchedulerJobID
		Order by SortOrder
	END

	IF @Command = 'DropDown'
	BEGIN
		Select	*
		From	SchedulerTask
		Where	IsActive = 1
		And		SchedulerJobID = @SchedulerJobID
		Order by TaskName
	END

	IF @Command = 'Delete'
	BEGIN
		Delete From SchedulerTask
		Where	SchedulerTaskID = @SchedulerTaskID
	END

	IF @Command = 'Get'
	BEGIN
		Select	t.*, tt.TaskTypeCode
		From	SchedulerTask t
		Join	SchedulerTaskType tt on t.SchedulerTaskTypeID = tt.SchedulerTaskTypeID
		Where	SchedulerTaskID = @SchedulerTaskID
	END
END
GO
