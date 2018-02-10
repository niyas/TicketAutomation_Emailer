ALTER PROCEDURE usp_get_WeeklyReportData
AS
BEGIN
	select 
		PayrollDataId,
		IncidentId,
		NotificationText,
		SeverityNumber,
		Status,
		SuspendReason,
		Assignee,
		AssigneeGroup,
		StatusTracking,
		CASE 
			WHEN (ISNULL(TBD, '') = '')
				THEN ETR
			ELSE 'TBD'
		END AS ETR,
		Priority,
		CreatedDateTime,
		UpdatedDateTime,
		Finalized,
		PreviousStatusTracking,
		PreviousETR,
		PreviousPriority		
	from IncidentManagement_WeeklyData
END 