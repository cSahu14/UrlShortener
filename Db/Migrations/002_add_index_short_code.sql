-- 002_add_index_short_code.sql
CREATE INDEX idx_urls_short_code ON urls(short_code);