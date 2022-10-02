<?php
require('db_connect.php');

if (!empty($_POST)) {
    $result = $conn->query("INSERT INTO notebook_br1 VALUES (
        NULL,
        '$_POST[name]',
        '$_POST[city]',
        '$_POST[address]',
        '$_POST[birthday]',
        '$_POST[mail]'
        );");

    if ($result) {
        header("Location: ".$_SERVER['PHP_SELF']);
        exit;
    }
    
    echo $conn->error;
}
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

<form class="wrapper" method="POST" action="<?php echo htmlspecialchars($_SERVER["PHP_SELF"]); ?>">
    <table style="margin-bottom: 1rem;">
        <tr>
            <th>Name</th>
            <th>City</th>
            <th>Address</th>
            <th>Birthday</th>
            <th>Mail</th>
        </tr>
        <tr>
            <td><input name="name" required type="text" /></td>
            <td><input name="city" type="text" /></td>
            <td><input name="address" type="text" /></td>
            <td><input name="birthday" type="date" /></td>
            <td><input name="mail" required type="text" /></td>
        </tr>
    </table>
    <button class='sub-btn' type="submit">Добавить</button>
</form>