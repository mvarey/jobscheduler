SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Michael Varey
-- Create date: 2016
-- Comments:	SchedulerAgent Maintenance
-- =============================================
ALTER PROCEDURE [dbo].[sp_SchedulerAgent]
@Command				varchar(10),
@SchedulerAgentID		int = null,
@SchedulerServerID		int = null,
@SchedulerQueueID		int = null,
@IsActive				bit = null
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	IF @Command = 'Add'
	BEGIN
		Insert Into SchedulerAgent (SchedulerServerID, SchedulerQueueID, IsActive)
		Values (@SchedulerServerID, @SchedulerQueueID, @IsActive)

		Select @@IDENTITY as SchedulerAgentID
	END

	IF @Command = 'Update'
	BEGIN
		Update	SchedulerAgent
		Set		SchedulerServerID = isnull(@SchedulerServerID, SchedulerServerID),
				IsActive	= isnull(@IsActive, IsActive)
		Where	SchedulerAgentID = @SchedulerAgentID
	END

	IF @Command = 'List'
	BEGIN
		Select	a.*, s.ServerName, s.IsActive as ServerIsActive
		From	SchedulerAgent a
		Join	SchedulerServer s on a.SchedulerServerID = s.SchedulerServerID
		Where	SchedulerQueueID = @SchedulerQueueID
		Order by s.ServerName
	END

	IF @Command = 'DropDown'
	BEGIN
		Select	a.*, s.ServerName, s.IsActive as ServerIsActive
		From	SchedulerAgent a
		Join	SchedulerServer s on a.SchedulerServerID = s.SchedulerServerID
		Where	a.IsActive = 1
		And		SchedulerQueueID = @SchedulerQueueID
		Order by SchedulerServerID
	END

	IF @Command = 'Delete'
	BEGIN
		Delete From SchedulerAgent
		Where	SchedulerAgentID = @SchedulerAgentID
	END

	IF @Command = 'Get'
	BEGIN
		Select	*
		From	SchedulerAgent
		Where	SchedulerAgentID = @SchedulerAgentID
	END
END
GO
