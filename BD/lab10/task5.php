<?php
function vid_content($table_name, $db_conn) {
    $rus_name = array();
    $rus_name['cnum'] = 'номер покупателя';
    $rus_name['cname'] = 'имя покупателя';
    $rus_name['city'] = 'город';
    $rus_name['rating'] = 'рейтинг покупателя';
    $rus_name['snum'] = 'номер продавца';
    
    $rus_name['snum'] = 'номер продавца';
    $rus_name['sname'] = 'имя продавца';
    $rus_name['comm'] = 'комиссия';
    
    $rus_name['onum'] = 'номер заказа';
    $rus_name['amt'] = 'сумма заказа';
    $rus_name['odate'] = 'дата';
    

    $res = $db_conn->query('SELECT * FROM ' . $table_name . ';');

    print "<table>\n";
        print "<thead>\n";
            print "<tr>\n";
                for ($x = 0; $x < mysqli_num_fields($res); $x++) {
                    $col_name = $res->fetch_field_direct($x)->name;
                    $translation = $rus_name[$col_name];
                    print "<th>" . "$translation<br>($col_name)" . "</th>\n";
                }
            print "</tr>\n";
        print "</thead>\n";
        print "<tbody>\n";
            foreach ($res as $row) {
                print "<tr>\n";
                    foreach($row as $cell) {
                        print "<td>$cell</td>\n";
                    }
                print "</tr>\n";
            }
        print "</tbody>\n";
    print "</table>\n";
}

function getFlagDescription($col) {
    $out = "";

    if ($col->flags & MYSQLI_NOT_NULL_FLAG) {
        $out .= ' NOT NULL';
    }

    if ($col->flags & MYSQLI_PRI_KEY_FLAG) {
        $out .= ' NOT NULL';
    }

    if ($col->flags & MYSQLI_UNIQUE_KEY_FLAG) {
        $out .= ' UNIQUE INDEX';
    }

    if ($col->flags & MYSQLI_MULTIPLE_KEY_FLAG) {
        $out .= ' UNIQUE INDEX';
    }

    if ($col->flags & MYSQLI_BLOB_FLAG) {
        $out .= ' UNIQUE INDEX';
    }

    if ($col->flags & MYSQLI_UNSIGNED_FLAG) {
        $out .= ' UNIQUE INDEX';
    }

    if ($col->flags & MYSQLI_ZEROFILL_FLAG) {
        $out .= ' UNIQUE INDEX';
    }

    if ($col->flags & MYSQLI_AUTO_INCREMENT_FLAG) {
        $out .= ' AUTO_INCREMENT';
    }

    if ($col->flags & MYSQLI_TIMESTAMP_FLAG) {
        $out .= ' NUMERIC';
    }

    if ($col->flags & MYSQLI_SET_FLAG) {
        $out .= ' UNIQUE INDEX';
    }

    if ($col->flags & MYSQLI_BINARY_FLAG) {
        $out .= ' UNIQUE INDEX';
    }

    if ($col->flags & MYSQLI_ENUM_FLAG) {
        $out .= ' UNIQUE INDEX';
    }

    if ($col->flags & MYSQLI_NUM_FLAG) {
        $out .= ' NUMERIC';
    }

    return $out;
}

function vid_structure($table_name, $db_conn) {
    $res = $db_conn->query('SELECT * FROM ' . $table_name . ';');

    $mysql_data_type_hash = array(
        1=>'tinyint',
        2=>'smallint',
        3=>'int',
        4=>'float',
        5=>'double',
        7=>'timestamp',
        8=>'bigint',
        9=>'mediumint',
        10=>'date',
        11=>'time',
        12=>'datetime',
        13=>'year',
        16=>'bit',
        //252 is currently mapped to all text and blob types (MySQL 5.0.51a)
        253=>'varchar',
        254=>'char',
        246=>'decimal'
    );

    print "<div>";
    for ($x = 0; $x < mysqli_num_fields($res); $x++) {
        $column = $res->fetch_field_direct($x);
        $type = $mysql_data_type_hash[$column->type];
        $len = $column->length;
        $name = $column->name;
        $flags = getFlagDescription($res->fetch_field_direct($x));
        
        print "<span style='display: block;'>$type $len $name $flags</span>\n";
    }
    print "</div>";
}
?>

<div class='content-wrapper'>
    <? if (array_key_exists('structure', $_GET)) {
            foreach ($_GET['structure'] as $value) {
                print "<h3>Структура таблицы $value</h3>\n";
                vid_structure($value, $conn);
            }
        }

        if (array_key_exists('content', $_GET)) {
            foreach ($_GET['content'] as $value) {
                print "<h3>Содержимое таблицы $value</h3>\n";
                vid_content($value, $conn);
            }
        }
    ?>
</div>

<a href="task1.html">Возврат к выбору таблицы</a>