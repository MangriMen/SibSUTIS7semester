<?php
$align = $_GET['align'];
$valign = array_key_exists('valign', $_GET) ? $_GET['valign'] : '';
?>

<style>
    .cell {
        border: 1px solid black;
        width: 300px;
        height: 300px;
    }
</style>

<table>
    <tr>
        <td align="<?echo $align?>" valign="<?echo $valign?>" class="cell">
            HELLO
        </td>
    </tr>
</table>
<a href="task4a.html">task4a.html</a>