SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Michael Varey
-- Create date: 2016
-- Comments:	SchedulerServer Maintenance
-- =============================================
CREATE PROCEDURE [dbo].[sp_SchedulerServer]
@Command			varchar(10),
@SchedulerServerID	int = null,
@ServerName			varchar(50) = null,
@AgentEnabled		bit = null,
@IsActive			bit = null,
@MaxThreads			int = null,
@MailServer			varchar(50) = null,
@MailID				varchar(100) = null,
@MailPassword		varchar(100) = null,
@MachineIP4Address	varchar(15) = null,
@MachineIP6Address	varchar(50) = null
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	IF @Command = 'Add'
	BEGIN
		Insert Into SchedulerServer (ServerName, AgentEnabled, IsActive, MaxThreads, MailServer, MailID, MailPassword, MachineIP4Address, MachineIP6Address)
		Values (@ServerName, @AgentEnabled, @IsActive, @MaxThreads, @MailServer, @MailID, @MailPassword, @MachineIP4Address, @MachineIP6Address)

		Select @@IDENTITY as SchedulerServerID
	END

	IF @Command = 'Update'
	BEGIN
		Update	SchedulerServer
		Set		ServerName = isnull(@ServerName, ServerName),
				IsActive	= isnull(@IsActive, IsActive),
				AgentEnabled = isnull(@AgentEnabled, AgentEnabled),
				MaxThreads = @MaxThreads,
				MailServer = @MailServer,
				MailID = @MailID,
				MailPassword = @MailPassword,
				MachineIP4Address = @MachineIP4Address,
				MachineIP6Address = @MachineIP6Address
		Where	SchedulerServerID = @SchedulerServerID
	END

	IF @Command = 'List'
	BEGIN
		Select	*
		From	SchedulerServer
	END

	IF @Command = 'DropDown'
	BEGIN
		Select	*
		From	SchedulerServer
		Where	IsActive = 1
		Order by ServerName
	END

	IF @Command = 'Delete'
	BEGIN
		Delete From SchedulerServer
		Where	SchedulerServerID = @SchedulerServerID
	END

	IF @Command = 'Get'
	BEGIN
		Select	*
		From	SchedulerServer
		Where	SchedulerServerID = @SchedulerServerID
	END
END
GO
