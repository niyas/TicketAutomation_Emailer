CREATE PROCEDURE usp_getConfigDetails
(
@type varchar(100)
)
BEGIN
	SELECT [key],value from App_Config
	WHERE Type = @type
END