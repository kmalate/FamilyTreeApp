CREATE TABLE IF NOT EXISTS Relationship (
    relationshipid INTEGER PRIMARY KEY,
    personid1 INTEGER NOT NULL,
    personid2 INTEGER NOT NULL,
    relationshiptype TEXT NOT NULL,
    FOREIGN KEY (personid1)
        REFERENCES person(personid),
    FOREIGN KEY (personid2)
        REFERENCES person(personid)
);