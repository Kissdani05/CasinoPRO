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
  `Role` ENUM ('User', 'Organizer', 'Admin') NOT NULL DEFAULT 'User'
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


--AutoIncrement resets

DELIMITER //

CREATE PROCEDURE delete_bet_and_reset_ai(IN bet_id INT)
BEGIN
  DELETE FROM Bets WHERE BetsID = bet_id;
  SET @new_auto_increment = (SELECT IFNULL(MAX(BetsID), 0) + 1 FROM Bets);
  SET @query = CONCAT('ALTER TABLE Bets AUTO_INCREMENT = ', @new_auto_increment);
  PREPARE stmt FROM @query;
  EXECUTE stmt;
  DEALLOCATE PREPARE stmt;
END //

DELIMITER ;

DELIMITER //

CREATE PROCEDURE reset_auto_increment_bettors()
BEGIN
    DECLARE max_id INT;
    SELECT IFNULL(MAX(BettorsID), 0) INTO max_id FROM Bettors;
    IF max_id > 0 THEN
        SET @query = CONCAT('ALTER TABLE Bettors AUTO_INCREMENT = ', max_id + 1);
    ELSE
        SET @query = 'ALTER TABLE Bettors AUTO_INCREMENT = 1';
    END IF;
    PREPARE stmt FROM @query;
    EXECUTE stmt;
    DEALLOCATE PREPARE stmt;
END //

DELIMITER ;

DELIMITER //

CREATE PROCEDURE reset_auto_increment_events()
BEGIN
    DECLARE max_id INT;
    SELECT IFNULL(MAX(EventID), 0) INTO max_id FROM Events;
    
    IF max_id > 0 THEN
        SET @query = CONCAT('ALTER TABLE Events AUTO_INCREMENT = ', max_id + 1);
    ELSE
        SET @query = 'ALTER TABLE Events AUTO_INCREMENT = 1';
    END IF;

    PREPARE stmt FROM @query;
    EXECUTE stmt;
    DEALLOCATE PREPARE stmt;
END //

DELIMITER ;

--Test Data
INSERT INTO `bettors` (`BettorsID`, `Username`, `Password`, `Balance`, `Email`, `JoinDate`, `IsActive`, `Role`) 
VALUES
(1, 'user', '$2a$11$I.Gnk1dp4kAbKOo/dGQmNOGjYRztXlsucCUfOEGj8bO3HPMJTNPlW', 1000, 'user@example.com', '2024-10-11', 1, 'User'),
(2, 'admin', '$2a$11$c4tTxbYTHA32sFdD10sXROi7kzm8RxdrWR7rklW/DDPPn8fHwoonS', 0, 'admin@example.com', '2024-10-11', 1, 'Admin'),
(3, 'organizer', '$2a$11$SRtmBVKRj77nSkskpEh7DORdiznx2BtWG3dlMN5S3oykS2PDocvv.', 0, 'organizer@example.com', '2021-12-01', 1, 'Organizer');

INSERT INTO `Events` (`EventName`, `EventDate`, `Category`, `Location`) VALUES
('Football Match', '2023-05-10', 'Sports', 'Stadium A'),
('Basketball Game', '2023-06-15', 'Sports', 'Arena B'),
('Concert', '2023-07-20', 'Entertainment', 'Concert Hall C');

INSERT INTO `Bets` (`BetDate`, `Odds`, `Amount`, `BettorsID`, `EventID`, `Status`) VALUES
('2023-04-01', 2.5, 200, 1, 1, 1),
('2023-04-02', 1.8, 150, 2, 2, 0),
('2023-04-03', 3.0, 300, 1, 3, 1);