CREATE DATABASE bets CHARACTER SET UTF8 COLLATE UTF8_HUNGARIAN_CI

CREATE TABLE `Bets` (
  `BetsID` INT PRIMARY KEY AUTO_INCREMENT,
  `BetDate` DATE NOT NULL,
  `Odds` FLOAT NOT NULL,
  `Amount` INT NOT NULL,
  `BettorsID` INT NOT NULL,
  `EventID` INT NOT NULL,
  `Status` BOOLEAN NOT NULL
);

CREATE TABLE `Bettors` (
  `BettorsID` INT PRIMARY KEY AUTO_INCREMENT,
  `Username` VARCHAR(50) NOT NULL,
  `Password` VARCHAR(255) NOT NULL,
  `Balance` INT NOT NULL,
  `Email` VARCHAR(100) NOT NULL,
  `JoinDate` DATE NOT NULL,
  `IsActive` BOOLEAN NOT NULL DEFAULT 1,
  `Role` ENUM ('User', 'Moderator', 'Admin') NOT NULL DEFAULT 'User'
);

CREATE TABLE `Events` (
  `EventID` INT PRIMARY KEY AUTO_INCREMENT,
  `EventName` VARCHAR(100) NOT NULL,
  `EventDate` DATE NOT NULL,
  `Category` VARCHAR(50) NOT NULL,
  `Location` VARCHAR(100) NOT NULL
);

ALTER TABLE `Bets` ADD FOREIGN KEY (`BettorsID`) REFERENCES `Bettors` (`BettorsID`);

ALTER TABLE `Bets` ADD FOREIGN KEY (`EventID`) REFERENCES `Events` (`EventID`);

INSERT INTO `bettors` (`BettorsID`, `Username`, `Password`, `Balance`, `Email`, `JoinDate`, `IsActive`, `Role`) VALUES
(1, 'john', 'b7fcc6e612145267d2ffea04be754a34128c1ed8133a09bfbbabd6afe6327688aa71d47343dd36e719f35f30fa79aec540e91b81c214fddfe0bedd53370df46d', 1000, 'john@example.com', '2022-01-15', 1, 'user'),
(2, 'jane', '1769722a9dc2fd3ae675264e61a51bd7359eb1346aa2f096e68513e78e86f4c68bee853ef7db764fff7ce707ef367a6644e71511ca3f31e52a4cbc02a1091e3c', 1500, 'jane@example.com', '2022-02-20', 1, 'organiser'),
(3, 'admin', 'c7ad44cbad762a5da0a452f9e854fdc1e0e7a52a38015f23f3eab1d80b931dd472634dfac71cd34ebc35d16ab7fb8a90c81f975113d6c7538dc69dd8de9077ec', 5000, 'admin@example.com', '2021-12-01', 1, 'admin'),

INSERT INTO `Events` (`EventName`, `EventDate`, `Category`, `Location`) VALUES
('Football Match', '2023-05-10', 'Sports', 'Stadium A'),
('Basketball Game', '2023-06-15', 'Sports', 'Arena B'),
('Concert', '2023-07-20', 'Entertainment', 'Concert Hall C');

INSERT INTO `Bets` (`BetDate`, `Odds`, `Amount`, `BettorsID`, `EventID`, `Status`) VALUES
('2023-04-01', 2.5, 200, 1, 1, 1),
('2023-04-02', 1.8, 150, 2, 2, 0),
('2023-04-03', 3.0, 300, 1, 3, 1);