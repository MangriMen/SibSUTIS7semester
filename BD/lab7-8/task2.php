<?php
$treug = array();
for ($i = 1; $i <= 30; $i++) {
    $treug[$i] = $i*($i+1)/2;
}

$kvd = array();
for ($i = 1; $i <= 30; $i++) {
    $kvd[$i] = $i * $i;
}
?>

<style>
    table {
        border: 4px outset gray;
    }

    td {
        border: 4px outset gray;
        width: 1.8rem;
        height: 1.8rem;
        text-align: center;
    }

    td.kvd {
        background-color: blue;
    }
    
    td.treug {
        background-color: green;
    }

    td.kvd-treug {
        background-color: red;
    }
</style>

<table cellspacing="0">
    <?for ($i = 1; $i <= 30; $i++) {?>
        <tr>
            <?for ($j = 1; $j <= 30; $j++) {?>
                <?
                    $tmp = $i * $j;
                    $class = "";
                    if (in_array($tmp, $kvd) && in_array($tmp, $treug)) {
                        $class = "kvd-treug";
                    }
                    elseif (in_array($tmp, $kvd)) {
                        $class = "kvd";
                    }
                    elseif (in_array($tmp, $treug)) {
                        $class = "treug";
                    }
                ?>
                <td class="<?echo $class?>"></td>
            <?}?>
        </tr>
    <?}?>
</table>