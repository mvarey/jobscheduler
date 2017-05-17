SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Michael Varey
-- Create date: 2016
-- Comments:	SchedulerVariable Maintenance
-- =============================================
CREATE PROCEDURE [dbo].[sp_SchedulerVariable]
@Command				varchar(10),
@SchedulerVariableID	int = null,
@SchedulerFolderID		int = null,
@VariableName			varchar(100) = null,
@VariableValue			varchar(1000) = null,
@VariableDescription	varchar(1000) = null,
@IsActive				bit = null,
@SortOrder				int = null
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	IF @Command = 'Add'
	BEGIN
		Insert Into SchedulerVariable (SchedulerFolderID, VariableName, VariableValue, VariableDescription, IsActive, SortOrder)
		Values (@SchedulerFolderID, @VariableName, @VariableValue, @VariableDescription, @IsActive, @SortOrder)

		Select @@IDENTITY as SchedulerVariableID
	END

	IF @Command = 'Update'
	BEGIN
		Update	SchedulerVariable
		Set		VariableName = isnull(@VariableName, VariableName),
				VariableValue = isnull(@VariableValue, VariableValue),
				VariableDescription = isnull(@VariableDescription, VariableDescription),
				IsActive	= isnull(@IsActive, IsActive),
				SortOrder = isnull(@SortOrder, SortOrder)
		Where	SchedulerVariableID = @SchedulerVariableID
	END

	IF @Command = 'List'
	BEGIN
		Select	*
		From	SchedulerVariable
		Where	SchedulerFolderID = @SchedulerFolderID
		Order by SortOrder
	END

	IF @Command = 'DropDown'
	BEGIN
		Select	*
		From	SchedulerVariable
		Where	IsActive = 1
		And		SchedulerFolderID = @SchedulerFolderID
		Order by SortOrder
	END

	IF @Command = 'Delete'
	BEGIN
		Delete From SchedulerVariable
		Where	SchedulerVariableID = @SchedulerVariableID
	END

	IF @Command = 'Get'
	BEGIN
		Select	*
		From	SchedulerVariable
		Where	SchedulerVariableID = @SchedulerVariableID
	END
END
GO
