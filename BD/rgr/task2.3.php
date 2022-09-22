<?php
require('db_connect.php');

$column = array_key_exists('column', $_GET) ? $_GET['column'] : '';

function display_column($db_connect, $column) {
    $result = $db_connect->query('SELECT ' . $column . ' FROM T3;');

    print "<table>\n";
        print "<thead>\n";
            print "<th>$column</th>\n";
        print "</thead>\n";
        foreach ($result as $row) {
            print "<tr>\n";
            foreach ($row as $cell) {
                print "<td>$cell</td>";
            }
            print "</tr>\n";
        }
    print "</table>\n";
}

display_column($conn, $column);
?>

<style>
    table {
        border-collapse: collapse;
        text-align: center;
    }

    table, th, td {
        border: 4px inset green;
    }
</style>