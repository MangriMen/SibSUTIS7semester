<?php
$SERVER_NAME = "localhost";
$USERNAME = "nikitapanin";
$PASSWORD = "nikitapanin";
$DB_NAME = "rgr";

$connection = new mysqli($SERVER_NAME, $USERNAME, $PASSWORD, $DB_NAME);

if ($connection->connect_error) {
    die("Connection failed: " . $connection->connect_error);
}
