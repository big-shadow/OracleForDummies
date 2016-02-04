CREATE OR REPLACE PROCEDURE --procedure--
(
	p_id number default 0
)
AS
	l_curs SYS_REFCURSOR;
	l_num NUMBER(10);
BEGIN

	l_num := p_id;

	IF l_num = 0 THEN
		OPEN l_curs FOR
			SELECT * FROM --table-- WHERE rownum < 1000;
	ELSE
		OPEN l_curs FOR
			SELECT * FROM --table-- WHERE ID = p_id;
	END IF;

	DBMS_SQL.RETURN_RESULT(l_curs);
END;