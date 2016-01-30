CREATE OR REPLACE PROCEDURE --procedure--
(
	p_id number
)
AS
	l_curs SYS_REFCURSOR
BEGIN

OPEN l_curs FOR
	SELECT * FROM --table-- WHERE ID = p_id;

	DBMS_SQL.RETURN_RESULT(l_curs);
END;