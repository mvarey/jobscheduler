SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Michael Varey
-- Create date: 2017
-- Comments:	SchedulerTaskType Maintenance
-- =============================================
CREATE PROCEDURE [dbo].[sp_SchedulerTaskType]
@Command				varchar(10),
@SchedulerTaskTypeID	int = null,
@TaskTypeName			varchar(100) = null,
@IsActive				bit = null,
@TaskTypeCode			varchar(20) = null
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	IF @Command = 'Add'
	BEGIN
		Insert Into SchedulerTaskType (TaskTypeName, IsActive, TaskTypeCode)
		Values (@TaskTypeName, @IsActive, @TaskTypeCode)

		Select @@IDENTITY as SchedulerTaskTypeID
	END

	IF @Command = 'Update'
	BEGIN
		Update	SchedulerTaskType
		Set		TaskTypeName = isnull(@TaskTypeName, TaskTypeName),
				IsActive	= isnull(@IsActive, IsActive),
				TaskTypeCode = isnull(@TaskTypeCode, TaskTypeCode)
		Where	SchedulerTaskTypeID = @SchedulerTaskTypeID
	END

	IF @Command = 'List'
	BEGIN
		Select	*
		From	SchedulerTaskType
	END

	IF @Command = 'DropDown'
	BEGIN
		Select	*
		From	SchedulerTaskType
		Where	IsActive = 1
		Order by TaskTypeName
	END

	IF @Command = 'Delete'
	BEGIN
		Delete From SchedulerTaskType
		Where	SchedulerTaskTypeID = @SchedulerTaskTypeID
	END

	IF @Command = 'Get'
	BEGIN
		Select	*
		From	SchedulerTaskType
		Where	SchedulerTaskTypeID = @SchedulerTaskTypeID
	END
END
GO
