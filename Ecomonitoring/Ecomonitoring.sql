create database ecomonitoring;
use ecomonitoring;

create table pollutants(
	pollutant_Id INT NOT NULL AUTO_INCREMENT,
    pollutant_name varchar(500) NOT NULL,
    activity varchar(500),
    ownership varchar(500),
    address varchar(500),
    PRIMARY KEY (pollutant_Id)
);

alter table pollutants
add column Kf decimal(6,3),
add column Knas decimal(6,3);

select * from pollutants;

create table substances(
	substance_Id INT NOT NULL AUTO_INCREMENT,
    substance_name varchar(500) NOT NULL,
    TLK DECIMAL(20,6),
    RfC DECIMAL(20,6),
    SF DECIMAL(20,6),
    cancerogenic int,
    danger_class varchar(5),
    PRIMARY KEY (substance_Id)
);

alter table substances
add column roNorm decimal (20,6),
add column qv decimal (20,6);

alter table substances
add column taxType varchar(500),
add column taxAmmount decimal (20,6);

select * from substances;

create table pollution(
	pollution_Id INT NOT NULL AUTO_INCREMENT,
	pollutant_Id INT NOT NULL,
    substance_Id INT NOT NULL,
    ammount DECIMAL(20,6),
    avrage_concentration DECIMAL(20,6),
    report_year year,
    PRIMARY KEY (pollution_Id),
    FOREIGN KEY (pollutant_Id) REFERENCES pollutants(pollutant_Id)
    on update cascade
    on delete cascade,
    FOREIGN KEY (substance_Id) REFERENCES substances(substance_Id)
    on update cascade
    on delete cascade
);

ALTER TABLE pollution
add COLUMN T DECIMAL(20,6);

ALTER TABLE pollution
add column excessive_mass decimal (20,6),
add column compensation_ammount decimal (20,6);

ALTER TABLE pollution
add column taxYearAmmount decimal (20,6);

ALTER TABLE pollution
add column roi decimal (20,6);

select * from pollution;

create table NonCarcinogenicRisks(
	NCR_Id INT NOT NULL AUTO_INCREMENT,
    pollution_Id INT NOT NULL,
    HQ DECIMAL(20,6),
    PRIMARY KEY (NCR_Id),
	FOREIGN KEY (pollution_Id) REFERENCES pollution(pollution_Id)
    on update cascade
    on delete cascade
);

select * from NonCarcinogenicRisks;

create table CarcinogenicRisks(
	CR_Id INT NOT NULL AUTO_INCREMENT,
    pollution_Id INT NOT NULL,
    CR_TimeOut DECIMAL(20,6),
    CR_TimeIn DECIMAL(20,6),
    Vout DECIMAL(20,6),
    Vin DECIMAL(20,6),
    EF DECIMAL(20,6),
    ED DECIMAL(20,6),
    BW DECIMAL(20,6),
    LADD DECIMAL(20,6),
    CR DECIMAL(20,6),
    PRIMARY KEY (CR_Id),
	FOREIGN KEY (pollution_Id) REFERENCES pollution(pollution_Id)
    on update cascade
    on delete cascade
);

select * from CarcinogenicRisks;
