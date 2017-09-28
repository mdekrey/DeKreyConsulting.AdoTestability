CREATE TABLE People
(
    Id SERIAL NOT NULL PRIMARY KEY,
    FullName TEXT NOT NULL,
    Email VARCHAR(254) NOT NULL,
    OptOut BIT NOT NULL
);


CREATE INDEX IX_People_Email ON People (Email);


CREATE INDEX IX_People_OptOut ON People (OptOut);
