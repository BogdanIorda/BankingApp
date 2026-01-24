-- COPY THIS INTO DbSetup.sql
CREATE DATABASE BankingDb;
GO
USE BankingDb;
GO

CREATE TABLE BankAccounts (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Owner NVARCHAR(50) NOT NULL,
    Balance DECIMAL(18, 2) NOT NULL
);

INSERT INTO BankAccounts (Owner, Balance) VALUES ('Bob Builder', 1200.50);
INSERT INTO BankAccounts (Owner, Balance) VALUES ('Alice Wonderland', 500.00);
INSERT INTO BankAccounts (Owner, Balance) VALUES ('Dan The Man', -20.00);

SELECT * FROM BankAccounts;