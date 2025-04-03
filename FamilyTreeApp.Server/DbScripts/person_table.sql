CREATE TABLE IF NOT EXISTS Person (
    personid INTEGER PRIMARY KEY,
    firstname TEXT NOT NULL,
    lastname TEXT NOT NULL,
    gender TEXT NOT NULL,
    dateofbirth TEXT NOT NULL,
    dateofdeath TEXT NULL,
    placeofbirth TEXT NULL
);