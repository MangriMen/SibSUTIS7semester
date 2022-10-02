<?php
require('db_connect.php');

function display_form($db_conn) {
    $result = $db_conn->query('SELECT * FROM T3;');
    
    print "<form action='task2.3.php'>\n";
        print "<div style='display: flex; flex-direction: column; margin-bottom: 1rem;'>\n";
        for ($x = 0; $x < mysqli_num_fields($result); $x++) {
            $col_name = $result->fetch_field_direct($x)->name;
            print "<label>" . $col_name . "<input value=$col_name type='radio' name='column'></label>\n";
        }
        print "</div>\n";
        print "<button type='submit'>Вывести!</button>\n";
    print "</form>\n";
}

display_form($conn);

?>