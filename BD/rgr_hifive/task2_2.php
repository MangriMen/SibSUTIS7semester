<?php
require('dataBaseConnect.php');

function display_form($connection)
{
    $result = $connection->query('SELECT * FROM T4;');

    echo "<form action='task2_3.php'>\n";
    echo "<div class='columnNamesWrapper'>\n";
    for ($x = 0; $x < mysqli_num_fields($result); $x++) {
        $col_name = $result->fetch_field_direct($x)->name;
        echo "<label>" . $col_name . "<input value=$col_name type='checkbox' name='column[]'></label>\n";
    }
    echo "</div>\n";
    echo "<button type='submit'>Вывести!</button>\n";
    echo "</form>\n";
}

display_form($connection);

?>

<style>
    .columnNamesWrapper {
        display: flex;
        flex-direction: column;
        margin-bottom: 1rem;
    }

    body {
        color: #EEEEEE;
        background-color: #272727;
        display: flex;
        align-items: center;
        justify-content: center;
    }
</style>