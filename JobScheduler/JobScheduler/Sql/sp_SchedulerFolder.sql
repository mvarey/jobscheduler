SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Michael Varey
-- Create date: 2016
-- Comments:	SchedulerFolder Maintenance
-- =============================================
ALTER PROCEDURE [dbo].[sp_SchedulerFolder]
@Command					varchar(10),
@SchedulerFolderID			int = null,
@SchedulerFolderName		varchar(100) = null,
@SchedulerQueueID			int = null,
@ParentSchedulerFolderID	int = null,
@IsActive					bit = null,
@SortOrder					int = null,
@CreateDate					datetime = null,
@CreatedBy					int = null
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	IF @Command = 'Add'
	BEGIN
		Insert Into SchedulerFolder (SchedulerFolderName, SchedulerQueueID, ParentSchedulerFolderID, IsActive, SortOrder, CreateDate, CreatedBy)
		Values (@SchedulerFolderName, @SchedulerQueueID, @ParentSchedulerFolderID, @IsActive, @SortOrder, @CreateDate, @CreatedBy)

		Select @@IDENTITY as SchedulerFolderID
	END

	IF @Command = 'Update'
	BEGIN
		Update	SchedulerFolder
		Set		SchedulerFolderName = isnull(@SchedulerFolderName, SchedulerFolderName),
				ParentSchedulerFolderID = isnull(@ParentSchedulerFolderID, ParentSchedulerFolderID),
				IsActive	= isnull(@IsActive, IsActive),
				SortOrder   = isnull(@SortOrder, SortOrder)
		Where	SchedulerFolderID = @SchedulerFolderID
	END

	IF @Command = 'List'
	BEGIN
		Select	f.*, p.SchedulerFolderName as ParentFolderName
		From	SchedulerFolder f
		Left Join SchedulerFolder p on f.ParentSchedulerFolderID = p.SchedulerFolderID and p.IsActive = 1
		Where	f.SchedulerQueueID = @SchedulerQueueID
		Order by SchedulerFolderName
	END

	IF @Command = 'DropDown'
	BEGIN
		Select	*
		From	SchedulerFolder
		Where	IsActive = 1
		Order by SchedulerFolderName
	END

	IF @Command = 'Delete'
	BEGIN
		Delete From SchedulerFolder
		Where	SchedulerFolderID = @SchedulerFolderID
	END

	IF @Command = 'Get'
	BEGIN
		Select	*
		From	SchedulerFolder
		Where	SchedulerFolderID = @SchedulerFolderID
	END
END
GO
