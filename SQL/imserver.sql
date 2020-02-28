# Host: 127.0.0.1  (Version 5.6.36)
# Date: 2020-02-28 13:13:58
# Generator: MySQL-Front 6.0  (Build 2.20)


#
# Structure for table "user"
#

DROP TABLE IF EXISTS `user`;
CREATE TABLE `user` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `username` varchar(50) NOT NULL DEFAULT '',
  `password` varchar(100) NOT NULL DEFAULT '',
  `salt` varchar(40) NOT NULL DEFAULT '',
  `nickname` varchar(20) NOT NULL DEFAULT '',
  `signuptime` datetime(2) NOT NULL DEFAULT '0000-00-00 00:00:00.00',
  PRIMARY KEY (`id`),
  UNIQUE KEY `username` (`username`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8 COMMENT='用户信息';

#
# Structure for table "chat"
#

DROP TABLE IF EXISTS `chat`;
CREATE TABLE `chat` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `sendusername` varchar(50) NOT NULL DEFAULT '',
  `receiveusername` varchar(50) NOT NULL DEFAULT '',
  `messagetype` int(11) NOT NULL DEFAULT '0',
  `message` varchar(1024) NOT NULL DEFAULT '',
  `time` datetime(2) NOT NULL DEFAULT '0000-00-00 00:00:00.00',
  PRIMARY KEY (`id`),
  KEY `sendusername` (`sendusername`),
  KEY `receiveusername` (`receiveusername`),
  CONSTRAINT `chat_ibfk_1` FOREIGN KEY (`sendusername`) REFERENCES `user` (`username`),
  CONSTRAINT `chat_ibfk_2` FOREIGN KEY (`receiveusername`) REFERENCES `user` (`username`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='聊天内容';
