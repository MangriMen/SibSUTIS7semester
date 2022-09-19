<?php
$server = "localhost";
$username = "user_bd";
$password = "romastepanov";
$db = "sample";

$conn = new mysqli($server, $username, $password, $db);

if ($conn->connection_error) {
    die("Connection failed: " . $conn->connect_error);
}
?>