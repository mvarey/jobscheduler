SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Michael Varey
-- Create date: 2016
-- Comments:	SchedulerOperatorUser Maintenance
-- =============================================
CREATE PROCEDURE [dbo].[sp_SchedulerOperatorUser]
@Command					varchar(10),
@SchedulerOperatorUserID	int = null,
@SchedulerOperatorId		int = null,
@SchedulerUserId			int = null,
@IsActive					bit = null
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	IF @Command = 'Add'
	BEGIN
		Insert Into SchedulerOperatorUser (SchedulerOperatorId, SchedulerUserId, IsActive)
		Values (@SchedulerOperatorId, @SchedulerUserId, @IsActive)

		Select @@IDENTITY as SchedulerOperatorUserID
	END

	IF @Command = 'Update'
	BEGIN
		Update	SchedulerOperatorUser
		Set		SchedulerOperatorId = isnull(@SchedulerOperatorId, SchedulerOperatorId),
				SchedulerUserId = isnull(@SchedulerUserId, SchedulerUserId),
				IsActive	= isnull(@IsActive, IsActive)
		Where	SchedulerOperatorUserID = @SchedulerOperatorUserID
	END

	IF @Command = 'List'
	BEGIN
		Select	ou.*, u.UserFirstName, u.UserLastName
		From	SchedulerOperatorUser ou
		Join	SchedulerUser u on ou.SchedulerUserId = u.SchedulerUserId
		Order by u.UserFirstName, u.UserLastName
	END

	IF @Command = 'DropDown'
	BEGIN
		Select	ou.*, u.UserFirstName, u.UserLastName
		From	SchedulerOperatorUser ou
		Join	SchedulerUser u on ou.SchedulerUserId = u.SchedulerUserId
		Where	ou.IsActive = 1
		And		u.IsActive = 1
		Order by u.UserFirstName, u.UserLastName
	END

	IF @Command = 'Delete'
	BEGIN
		Delete From SchedulerOperatorUser
		Where	SchedulerOperatorUserID = @SchedulerOperatorUserID
	END

	IF @Command = 'Get'
	BEGIN
		Select	*
		From	SchedulerOperatorUser
		Where	SchedulerOperatorUserID = @SchedulerOperatorUserID
	END
END
GO
