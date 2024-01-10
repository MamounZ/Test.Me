CREATE TABLE MC_Answers (
    MCid int NOT NULL,
    Quid int NOT NULL,
    PRIMARY KEY (MCid),
    FOREIGN KEY (Quid) REFERENCES Question(Quid)
);