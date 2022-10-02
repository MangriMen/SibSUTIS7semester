<?php
$server = "localhost";
$username = "user_bd";
$password = "user_password";
$db = "sample";

$conn = new mysqli($server, $username, $password, $db);

if ($conn->connect_error) {
    die("Connection failed: " . $conn->connect_error);
}
?>