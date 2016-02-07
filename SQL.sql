-- Run this after turning the sniffer off.

CREATE INDEX index_post_posttext ON post (PostText) COMPUTE STATISTICS;
/

ALTER TABLE post ADD CONSTRAINT fk_category_id FOREIGN KEY (CategoryID) REFERENCES category(ID);
/