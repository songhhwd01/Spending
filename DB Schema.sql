SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;


CREATE TABLE `accounts` (
  `Id` int(10) NOT NULL AUTO_INCREMENT,
  `UserId` int(11) NOT NULL,
  `Name` varchar(50) NOT NULL,
  `Balance` decimal(10,2) NOT NULL,
  `Owned` tinyint(3) unsigned NOT NULL,
  `Order` tinyint(3) unsigned NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB  DEFAULT CHARSET=latin1;

CREATE TABLE `budgets` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `CategoryId` int(11) NOT NULL,
  `Amount` decimal(10,2) NOT NULL,
  `StartingMonth` date NOT NULL,
  `Frequency` tinyint(3) unsigned NOT NULL,
  `Times` tinyint(3) unsigned NOT NULL,
  `Notes` varchar(50) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `CategoryId` (`CategoryId`)
) ENGINE=InnoDB  DEFAULT CHARSET=latin1;

CREATE TABLE `categories` (
  `Id` int(10) NOT NULL AUTO_INCREMENT,
  `GroupId` int(10) NOT NULL,
  `Name` varchar(50) NOT NULL,
  `StartingBalance` decimal(10,2) NOT NULL,
  `Added` decimal(10,2) NOT NULL,
  `Budget` decimal(10,2) NOT NULL,
  `EstimateBudget` tinyint(1) unsigned NOT NULL,
  `Income` tinyint(1) unsigned NOT NULL,
  `System` tinyint(1) unsigned NOT NULL,
  `Order` tinyint(3) unsigned NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `GroupId` (`GroupId`)
) ENGINE=InnoDB  DEFAULT CHARSET=latin1;

CREATE TABLE `categorygroups` (
  `Id` int(10) NOT NULL AUTO_INCREMENT,
  `UserId` int(10) NOT NULL,
  `Name` varchar(50) NOT NULL,
  `Order` tinyint(3) unsigned NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `GroupId` (`UserId`)
) ENGINE=InnoDB  DEFAULT CHARSET=latin1;

CREATE TABLE `my_aspnet_applications` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(256) DEFAULT NULL,
  `description` varchar(256) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB  DEFAULT CHARSET=latin1;

CREATE TABLE `my_aspnet_membership` (
  `userId` int(11) NOT NULL DEFAULT '0',
  `Email` varchar(128) DEFAULT NULL,
  `Comment` varchar(255) DEFAULT NULL,
  `Password` varchar(128) NOT NULL,
  `PasswordKey` char(32) DEFAULT NULL,
  `PasswordFormat` tinyint(4) DEFAULT NULL,
  `PasswordQuestion` varchar(255) DEFAULT NULL,
  `PasswordAnswer` varchar(255) DEFAULT NULL,
  `IsApproved` tinyint(1) DEFAULT NULL,
  `LastActivityDate` datetime DEFAULT NULL,
  `LastLoginDate` datetime DEFAULT NULL,
  `LastPasswordChangedDate` datetime DEFAULT NULL,
  `CreationDate` datetime DEFAULT NULL,
  `IsLockedOut` tinyint(1) DEFAULT NULL,
  `LastLockedOutDate` datetime DEFAULT NULL,
  `FailedPasswordAttemptCount` int(10) unsigned DEFAULT NULL,
  `FailedPasswordAttemptWindowStart` datetime DEFAULT NULL,
  `FailedPasswordAnswerAttemptCount` int(10) unsigned DEFAULT NULL,
  `FailedPasswordAnswerAttemptWindowStart` datetime DEFAULT NULL,
  PRIMARY KEY (`userId`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1 COMMENT='2';

CREATE TABLE `my_aspnet_profiles` (
  `userId` int(11) NOT NULL,
  `valueindex` longtext,
  `stringdata` longtext,
  `binarydata` longblob,
  `lastUpdatedDate` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`userId`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

CREATE TABLE `my_aspnet_roles` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `applicationId` int(11) NOT NULL,
  `name` varchar(255) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1 ROW_FORMAT=DYNAMIC;

CREATE TABLE `my_aspnet_schemaversion` (
  `version` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

CREATE TABLE `my_aspnet_sessioncleanup` (
  `LastRun` datetime NOT NULL,
  `IntervalMinutes` int(11) NOT NULL,
  `ApplicationId` int(11) NOT NULL,
  PRIMARY KEY (`ApplicationId`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

CREATE TABLE `my_aspnet_sessions` (
  `SessionId` varchar(191) NOT NULL,
  `ApplicationId` int(11) NOT NULL,
  `Created` datetime NOT NULL,
  `Expires` datetime NOT NULL,
  `LockDate` datetime NOT NULL,
  `LockId` int(11) NOT NULL,
  `Timeout` int(11) NOT NULL,
  `Locked` tinyint(1) NOT NULL,
  `SessionItems` longblob,
  `Flags` int(11) NOT NULL,
  PRIMARY KEY (`SessionId`,`ApplicationId`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

CREATE TABLE `my_aspnet_users` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `applicationId` int(11) NOT NULL,
  `name` varchar(256) NOT NULL,
  `isAnonymous` tinyint(1) NOT NULL DEFAULT '1',
  `lastActivityDate` datetime DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB  DEFAULT CHARSET=latin1;

CREATE TABLE `my_aspnet_usersinroles` (
  `userId` int(11) NOT NULL DEFAULT '0',
  `roleId` int(11) NOT NULL DEFAULT '0',
  PRIMARY KEY (`userId`,`roleId`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1 ROW_FORMAT=DYNAMIC;

CREATE TABLE `settings` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `UserId` int(11) NOT NULL,
  `Month` date NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB  DEFAULT CHARSET=latin1;

CREATE TABLE `splits` (
  `Id` int(10) NOT NULL AUTO_INCREMENT,
  `TransactionId` int(10) NOT NULL,
  `CategoryId` int(10) NOT NULL,
  `Amount` decimal(10,2) NOT NULL,
  `Notes` varchar(50) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `TransactionId` (`TransactionId`),
  KEY `CategoryId` (`CategoryId`)
) ENGINE=InnoDB  DEFAULT CHARSET=latin1;

CREATE TABLE `transactions` (
  `Id` int(10) NOT NULL AUTO_INCREMENT,
  `DayOrder` int(11) NOT NULL,
  `AccountId` int(10) DEFAULT NULL,
  `Date` date NOT NULL,
  `OriginalDescription` varchar(100) NOT NULL,
  `Description` varchar(100) NOT NULL,
  `Pending` tinyint(1) NOT NULL,
  `ImportState` int(11) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `AccountId` (`AccountId`)
) ENGINE=InnoDB  DEFAULT CHARSET=latin1;


ALTER TABLE `budgets`
  ADD CONSTRAINT `budgets_ibfk_2` FOREIGN KEY (`CategoryId`) REFERENCES `categories` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE;

ALTER TABLE `categories`
  ADD CONSTRAINT `categories_ibfk_1` FOREIGN KEY (`GroupId`) REFERENCES `categorygroups` (`Id`);

ALTER TABLE `categorygroups`
  ADD CONSTRAINT `categorygroups_ibfk_1` FOREIGN KEY (`UserId`) REFERENCES `my_aspnet_users` (`id`);

ALTER TABLE `splits`
  ADD CONSTRAINT `splits_ibfk_2` FOREIGN KEY (`CategoryId`) REFERENCES `categories` (`Id`),
  ADD CONSTRAINT `splits_ibfk_4` FOREIGN KEY (`TransactionId`) REFERENCES `transactions` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
