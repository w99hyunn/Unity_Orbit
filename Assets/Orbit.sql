-- --------------------------------------------------------
-- 호스트:                          localhost
-- 서버 버전:                        10.5.10-MariaDB - mariadb.org binary distribution
-- 서버 OS:                        Win64
-- HeidiSQL 버전:                  11.2.0.6213
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!50503 SET NAMES utf8mb4 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;


-- orbit 데이터베이스 구조 내보내기
CREATE DATABASE IF NOT EXISTS `orbit` /*!40100 DEFAULT CHARACTER SET utf8 */;
USE `orbit`;

-- 테이블 orbit.game_data 구조 내보내기
CREATE TABLE IF NOT EXISTS `game_data` (
  `game_data_id` int(11) NOT NULL AUTO_INCREMENT,
  `user_id` int(11) NOT NULL,
  `game_time` float DEFAULT NULL,
  `max_health` int(11) DEFAULT NULL,
  `max_mana` int(11) DEFAULT NULL,
  `max_experience` int(11) DEFAULT NULL,
  `current_health` int(11) DEFAULT NULL,
  `current_mana` int(11) DEFAULT NULL,
  `current_experience` int(11) DEFAULT NULL,
  `level` int(11) DEFAULT NULL,
  `player_position_x` float DEFAULT NULL,
  `player_position_y` float DEFAULT NULL,
  `player_position_z` float DEFAULT NULL,
  `chip` int(11) DEFAULT NULL,
  PRIMARY KEY (`game_data_id`),
  KEY `user_id` (`user_id`),
  CONSTRAINT `game_data_ibfk_1` FOREIGN KEY (`user_id`) REFERENCES `users` (`user_id`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8;

-- 테이블 데이터 orbit.game_data:~6 rows (대략적) 내보내기
/*!40000 ALTER TABLE `game_data` DISABLE KEYS */;
REPLACE INTO `game_data` (`game_data_id`, `user_id`, `game_time`, `max_health`, `max_mana`, `max_experience`, `current_health`, `current_mana`, `current_experience`, `level`, `player_position_x`, `player_position_y`, `player_position_z`, `chip`) VALUES
	(1, 1, 0, 400, 400, 3100, 400, 400, 1197, 11, 2.44452, 0.08, 4.47975, 0),
	(2, 18, 0, 450, 450, 3600, 450, 450, 740, 32, 28.2685, 0.0799999, -5.1803, 0),
	(3, 23, 0, 250, 250, 1600, 250, 250, 50, 4, -1.57, 9.385, 13.13, 0),
	(4, 24, 0, 300, 300, 2100, 290, 300, 550, 5, -1.57, 9.385, 13.13, 0),
	(5, 26, 0, 200, 200, 1100, 200, 200, 126, 3, -1.57, 9.385, 13.13, 0),
	(6, 6, 0, 0, 0, 0, 0, 0, 0, -1, -1.57, 9.385, 13.13, 0);
/*!40000 ALTER TABLE `game_data` ENABLE KEYS */;

-- 테이블 orbit.users 구조 내보내기
CREATE TABLE IF NOT EXISTS `users` (
  `user_id` int(11) NOT NULL AUTO_INCREMENT,
  `username` varchar(50) NOT NULL,
  `password_hash` varchar(255) NOT NULL,
  `password_salt` varchar(255) NOT NULL,
  PRIMARY KEY (`user_id`),
  UNIQUE KEY `username` (`username`)
) ENGINE=InnoDB AUTO_INCREMENT=28 DEFAULT CHARSET=utf8;

-- 테이블 데이터 orbit.users:~10 rows (대략적) 내보내기
/*!40000 ALTER TABLE `users` DISABLE KEYS */;
REPLACE INTO `users` (`user_id`, `username`, `password_hash`, `password_salt`) VALUES
	(1, 'admin', 'fmQUZm6dak8tsz6tswwvZbQRpmw=', 'tmpBTpa23nBnROat1fz7mw=='),
	(3, 'ㅇㅇ', '7z+ie4FtcW5CmRV7njT7XvFOMwc=', '7wnTr+YLZbM5aYYcSyBV8w=='),
	(5, 'ss', 'oNgTnV8aFYzQIGspmQVRw7DXHmU=', 'ylC1IkBhmhFrk41X4QteLw=='),
	(6, '', 'P0IC93I8jtvck0YS7QNBlCtZC+8=', 'LNw7wyiMwR9uVCVE0sAZkQ=='),
	(7, 'zz', 'KCEuUiBsANPy8IB7gH/abfy7z8Y=', 'asBZpNSRfYgyvkI19/hMiA=='),
	(14, 'ㅋㅋ', 'awDDGblrxIwUH9qjHOrtUnwESXw=', 'kx6Kcb6cmRvybtdADLeinA=='),
	(18, 'admin2', 'R7weEVq/n7u3JMVA6TkepDBARgk=', 'yYBhmVPOk0jjTcUllw6FgA=='),
	(19, '아이디를 입력하세요.', 'dwsugfA1q7rS/3C90tu+KuwzXJk=', 'KYmahivpIO+z3NjVsfETNQ=='),
	(22, 'tlrmsjtm77', 'FDEeeskY6FYTZr/Vx417zzEmI88=', 'Am5ejjpNEDwttFMJdHVjqg=='),
	(23, 'admin4', 'pwCngnG8PYsEKO2J4CeRMO050d0=', 'ycWWg1/1q/eM8+ffB3e3jw=='),
	(24, 'admin77', 'M8RLQlUiGYOMbClWD27XeRy3ZEw=', 'h2MHbMrv56n1EXXuoQYCsQ=='),
	(26, 'admin3', '+Fh6eAVag8AXz/JtKPTvlAmww0U=', '+9y4P5/MC9SqQ92IO/eOjQ==');
/*!40000 ALTER TABLE `users` ENABLE KEYS */;

/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IFNULL(@OLD_FOREIGN_KEY_CHECKS, 1) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40111 SET SQL_NOTES=IFNULL(@OLD_SQL_NOTES, 1) */;
