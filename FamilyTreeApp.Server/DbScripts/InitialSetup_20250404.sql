/* OBJECTS */
/* TABLES */
DROP TABLE IF EXISTS Relationship;

DROP TABLE IF EXISTS Person;

DROP VIEW IF EXISTS v_PersonRelationhip;

CREATE TABLE IF NOT EXISTS Person (
    personid  INTEGER PRIMARY KEY,
    firstname TEXT    NOT NULL,
    lastname  TEXT    NOT NULL,
    gender    TEXT    NOT NULL,
    img       BLOB    NULL
);

CREATE TABLE IF NOT EXISTS Relationship (
    relationshipid   INTEGER PRIMARY KEY,
    personid1        INTEGER NOT NULL,
    personid2        INTEGER NOT NULL,
    relationshiptype TEXT    NOT NULL,
    FOREIGN KEY (
        personid1
    )
    REFERENCES person (personid),
    FOREIGN KEY (
        personid2
    )
    REFERENCES person (personid) 
);
/* END TABLES */

/* VIEWS */
CREATE VIEW IF NOT EXISTS v_PersonRelationhip AS
    SELECT p.personid,
       GROUP_CONCAT(DISTINCT partner.personid1) spousepersonids,
       MIN(CASE WHEN parents.relationshiptype = 'mother-child' THEN parents.personid2 ELSE NULL END) motherpersonid,
       MIN(CASE WHEN parents.relationshiptype = 'father-child' THEN parents.personid2 ELSE NULL END) fatherpersonid,
       p.firstname,
       p.lastname,
       p.gender,
       p.img
    FROM person p
    LEFT JOIN 
        relationship parents ON parents.personid1 = p.personid
    LEFT JOIN 
        relationship partner ON partner.personid2 = p.personid AND partner.relationshiptype = 'spouse'
    GROUP BY p.personid;
/* END VIEWS */
/* END OBJECTS */

/* DATA */
insert into person(firstname, lastname, gender)
values ('Gomez', 'Addams', 'male');

insert into person(firstname, lastname, gender)
values ('Morticia', 'Addams', 'female');

insert into person(firstname, lastname, gender)
values ('Pugsley', 'Addams', 'male');

insert into person(firstname, lastname, gender)
values ('Wednesday', 'Addams', 'female');

insert into relationship(personid1, personid2, relationshiptype)
values (1,2, 'spouse');

insert into relationship(personid1, personid2, relationshiptype)
values (2,1, 'spouse');

insert into relationship(personid1, personid2, relationshiptype)
values (3,1, 'father-child');

insert into relationship(personid1, personid2, relationshiptype)
values (3,2, 'mother-child');

insert into relationship(personid1, personid2, relationshiptype)
values (4,1, 'father-child');

insert into relationship(personid1, personid2, relationshiptype)
values (4,2, 'mother-child');

/* END DATA*/