<?
require('db_connect.php');

$result = $conn->query('DROP TABLE IF EXISTS T3;');

if (!$result) {
    die("Error when removing table");
}

$result = $conn->query('CREATE TABLE IF NOT EXISTS T3 (
    ID INT AUTO_INCREMENT PRIMARY KEY,
    Название CHAR(20),
    Тип CHAR(20),
    Фирма CHAR(20)
    );');

if (!$result) {
    die("Error when creating table");
}

$result = $conn->query('INSERT INTO T3 (
        ID, Название, Тип, Фирма
    )
VALUES
(1, "Pascal", "Процед", "Borland"),
(2, "C", "Процед", "Borland"),
(3, "Java", "Процед", "Java inc"),
(4, "C++", "Объект", "Java inc"),
(5, "Visual C", "Объект", "Microsoft"),
(6, "Visual Basic", "Объект", "Microsoft"),
(7, "Delphi", "Объект", "Borland"),
(8, "Lisp", "Сценарн", "IBM"),
(9, "Prolog", "Сценарн", "IBM"),
(10, "XML", "Сценарн", "Borland");');

if (!$result) {
    die("Error when filling table");
}

?>