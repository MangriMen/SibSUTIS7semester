<?php

?>

<style>
    td *, th * {
        width: 100%;
    }

    .wrapper {
        display: flex;
        flex-direction: column;
        align-items: center;
        justify-content: center;
    }
    
</style>

<div class="wrapper">
    <table>
        <tr>
            <th>Name</th>
            <th>City</th>
            <th>Address</th>
            <th>Birthday</th>
            <th>Mail</th>
        </tr>
        <tr>
            <td><input type="text" /></td>
            <td><input type="text" /></td>
            <td><input type="text" /></td>
            <td><input type="date" /></td>
            <td><input type="email" /></td>
        </tr>
    </table>
    <button class='sub-btn' type="submit">Добавить</button>
</div>