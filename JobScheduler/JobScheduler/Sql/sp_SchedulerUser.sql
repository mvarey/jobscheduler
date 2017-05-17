SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Michael Varey
-- Create date: 2016
-- Comments:	SchedulerUser Maintenance
-- =============================================
CREATE PROCEDURE [dbo].[sp_SchedulerUser]
@Command			varchar(10),
@SchedulerUserID	int = null,
@UserLogin			varchar(50) = null,
@UserEmail			varchar(50) = null,
@IsActive			bit = null,
@UserPassword		varchar(100) = null,
@UserFirstName		varchar(20) = null,
@UserLastName		varchar(20) = null,
@AdminAccess		bit = null,
@ViewAccess			bit = null,
@ReportAccess		bit = null,
@OperatorAccess		bit = null
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	IF @Command = 'Add'
	BEGIN
		Insert Into SchedulerUser (UserLogin, UserEmail, IsActive, UserPassword, UserFirstName, UserLastName, AdminAccess, ViewAccess, ReportAccess, OperatorAccess)
		Values (@UserLogin, @UserEmail, @IsActive, @UserPassword, @UserFirstName, @UserLastName, @AdminAccess, @ViewAccess, @ReportAccess, @OperatorAccess)

		Select @@IDENTITY as SchedulerUserID
	END

	IF @Command = 'Update'
	BEGIN
		Update	SchedulerUser
		Set		UserLogin = isnull(@UserLogin, UserLogin),
				IsActive	= isnull(@IsActive, IsActive),
				UserEmail = isnull(@UserEmail, UserEmail),
				UserPassword = @UserPassword,
				UserFirstName = @UserFirstName,
				UserLastName = @UserLastName,
				AdminAccess = @AdminAccess,
				ViewAccess = @ViewAccess,
				ReportAccess = @ReportAccess,
				OperatorAccess = @OperatorAccess
		Where	SchedulerUserID = @SchedulerUserID
	END

	IF @Command = 'List'
	BEGIN
		Select	*
		From	SchedulerUser
	END

	IF @Command = 'DropDown'
	BEGIN
		Select	*
		From	SchedulerUser
		Where	IsActive = 1
		Order by UserLogin
	END

	IF @Command = 'Delete'
	BEGIN
		Delete From SchedulerUser
		Where	SchedulerUserID = @SchedulerUserID
	END

	IF @Command = 'Get'
	BEGIN
		Select	*
		From	SchedulerUser
		Where	SchedulerUserID = @SchedulerUserID
	END

	IF @Command = 'Login'
	BEGIN
		Select	*
		From	SchedulerUser
		Where	UserLogin = @UserLogin
		And		IsActive = 1
	END

	IF @Command = 'Forms'
	BEGIN
		Select	*
		From	SchedulerUser
		Where	UserLogin = @UserLogin
		And		UserPassword = @UserPassword
		And		IsActive = 1
	END

	IF @Command = 'Find'
	BEGIN
		Select	*
		From	SchedulerUser
		Where	UserFirstName like '%' + @UserFirstName + '%'
		And		UserLastName like '%' + @UserLastName + '%'
		Order by UserFirstName, UserLastName
	END
END
GO
