<?
require('dataBaseConnect.php');

$result = $connection->query('DROP TABLE IF EXISTS T4;');

if (!$result) die("Table removing error");

$result = $connection->query('CREATE TABLE T4 (
    NUM INT AUTO_INCREMENT PRIMARY KEY,
    OS_NAME CHAR(32),
    OS_TYPE CHAR(32),
    MANUFACTURER CHAR(32)
);');

if (!$result) die("Table creating error");

$result = $connection->query('INSERT INTO T4 (NUM, OS_NAME, OS_TYPE, MANUFACTURER)
VALUES
    (1, "Win95", "Win", "Microsoft"),
    (2, "Win98", "Win", "Microsoft"),
    (3, "WinNT", "Win", "UnixF"),
    (4, "WinXP", "Win", "Apple"),
    (5, "Unix", "Unix", "UnixF"),
    (6, "FreeBSD", "Unix", "Jobbs"),
    (7, "Linux", "Unix", "UnixF"),
    (8, "MacOS1", "Mac", "Apple"),
    (9, "MacOS2", "Mac", "Apple"),
    (10, "MacOS3", "Mac", "Jobbs");');

if (!$result) die("Table filling error");
