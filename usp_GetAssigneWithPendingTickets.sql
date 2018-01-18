CREATE PROCEDURE usp_GetAssigneeWithPendingTickets
AS
BEGIN
	select DISTINCT Assignee, email 
	from IncidentManagement_weeklyData I
	INNER JOIN Users U 
	ON U.Assignee = I.Assignee
	where UpdatedDateTime is NULL
END