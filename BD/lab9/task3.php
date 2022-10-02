<?php
require('db_connect.php');

$result = mysqli_query($conn, 'SELECT * FROM notebook_br1');
?>

<style>
    html {
        font-family: "Verdana";
    }

    table {
        border: 2px solid green;
        border-radius: 8px;
    }

    th, td {
        padding: 0.5rem;
    }

    .wrapper {
        display: flex;
        justify-content: center;
    }
</style>

<div class="wrapper">
    <table>
        <thead>
            <tr>
                <th>ID</th>
                <th>Name</th>
                <th>City</th>
                <th>Address</th>
                <th>Birthday</th>
                <th>Mail</th>
            </tr>
        </thead>
        <?foreach ($result as $row) {?>
            <tr>
                <?foreach ($row as $el) {?>
                    <td>
                        <?echo $el?>
                    </td>
                <?}?>
            </tr>
        <?}?>
    </table>
<div>