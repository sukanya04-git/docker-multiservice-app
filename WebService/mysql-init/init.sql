CREATE DATABASE IF NOT EXISTS testdb;

USE testdb;

CREATE TABLE IF NOT EXISTS users (
    id INT PRIMARY KEY,
    name VARCHAR(100)
);

INSERT INTO users (id, name) VALUES 
    (1, 'Sukanya'),
    (2, 'Samarth'),
    (3, 'Samayra')
ON DUPLICATE KEY UPDATE name=VALUES(name);
