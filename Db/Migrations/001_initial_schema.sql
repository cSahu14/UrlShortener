CREATE TABLE urls (
    id BIGINT PRIMARY KEY,
    short_code VARCHAR(11) NOT NULL,
    original_url TEXT NOT NULL,
    created_at TIMESTAMP NOT NULL DEFAULT NOW(),
    click_count INT NOT NULL DEFAULT 0
);