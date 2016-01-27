CREATE OR REPLACE TRIGGER trg_--table--
  BEFORE UPDATE ON --table--
  FOR EACH ROW
BEGIN
  :new.time_updated := sysdate;
END;