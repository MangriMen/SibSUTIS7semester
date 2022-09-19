<?php
require('db_connect.php');


mysqli_query($conn, "DROP TABLE IF EXISTS notebook_br1;");
mysqli_query($conn, "CREATE TABLE notebook_br1 (
    id INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
    name CHAR(50),
    city CHAR(50),
    address CHAR(50),
    birthday DATE,
    mail CHAR(20)
    );");

if(mysqli_error($conn)) {
    die("Нельзя создать таблицу notebook_br1");
}

?>