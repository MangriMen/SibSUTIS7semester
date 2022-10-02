<?php
require('dataBaseConnect.php');

$column = array_key_exists('column', $_GET) ? $_GET['column'] : [];

function display_column($connection, $column)
{;
    $result = $connection->query('SELECT ' .  implode(', ', $column) . ' FROM T4;');

    if (!$result) return;

    print "<table>\n";
    print "<thead>\n";
    foreach ($column as $col) {
        print "<th>$col</th>";
    }
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

display_column($connection, $column);
?>

<style>
    body {
        color: #EEEEEE;
        background-color: #272727;
        display: flex;
        align-items: center;
        justify-content: center;
    }

    table {
        border: 6px solid #7f8585;
        border-radius: 6px;
        text-align: center;
    }

    td,
    th {
        border-left: 3px solid #7f8585;
    }

    td:nth-child(1),
    th:nth-child(1) {
        border-left: none;
    }
</style>