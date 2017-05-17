SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Michael Varey
-- Create date: 2016
-- Comments:	SchedulerOperator Maintenance
-- =============================================
CREATE PROCEDURE [dbo].[sp_SchedulerOperator]
@Command				varchar(10),
@SchedulerOperatorID	int = null,
@SchedulerOperatorName	varchar(50) = null,
@IsActive				bit = null
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	IF @Command = 'Add'
	BEGIN
		Insert Into SchedulerOperator (SchedulerOperatorName, IsActive)
		Values (@SchedulerOperatorName, @IsActive)

		Select @@IDENTITY as SchedulerOperatorID
	END

	IF @Command = 'Update'
	BEGIN
		Update	SchedulerOperator
		Set		SchedulerOperatorName = isnull(@SchedulerOperatorName, SchedulerOperatorName),
				IsActive	= isnull(@IsActive, IsActive)
		Where	SchedulerOperatorID = @SchedulerOperatorID
	END

	IF @Command = 'List'
	BEGIN
		Select	*
		From	SchedulerOperator
	END

	IF @Command = 'DropDown'
	BEGIN
		Select	*
		From	SchedulerOperator
		Where	IsActive = 1
		Order by SchedulerOperatorName
	END

	IF @Command = 'Delete'
	BEGIN
		Delete From SchedulerOperator
		Where	SchedulerOperatorID = @SchedulerOperatorID
	END

	IF @Command = 'Get'
	BEGIN
		Select	*
		From	SchedulerOperator
		Where	SchedulerOperatorID = @SchedulerOperatorID
	END
END
GO
