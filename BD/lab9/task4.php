<?php
require('db_connect.php');

$email_regex = "/[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}/";
$date_regex = "/[0-9]{4}-[0-9]{2}-[0-9]{2}/";

$result = $conn->query('SELECT * FROM notebook_br1');

$id = array_key_exists('id', $_GET) ? $_GET['id'] : -1;

if (!empty($_GET)) {
    // foreach ($_GET as $key=>$rw) {
    //     echo $key . " " . $rw . "\n";
    // }

    if (array_key_exists("field_name", $_GET)) {
        if ($_GET['field_name'] == 'mail' && !preg_match($email_regex, $_GET['field_value'])) {
            die("Email entered incorrectly\n&nbsp&nbsp<a href=task4.php>Go back</a>");
        }

        if ($_GET['field_name'] == 'birthday' && !preg_match($date_regex, $_GET['field_value'])) {
            die("Birthday entered incorrectly\n&nbsp&nbsp<a href=task4.php>Go back</a>");
        }

        $res = $conn->query('UPDATE notebook_br1 SET ' . $_GET['field_name'] . '=' . '\'' . $_GET['field_value'] . '\'' . ' WHERE id = ' . $id . ';');

        echo $conn->error;

        if ($res) {
            header("Location: ".$_SERVER['PHP_SELF']);
            exit;
        }
    }
}

$current_row = $conn->query('SELECT * FROM notebook_br1 WHERE id = ' . $id . ';')->fetch_array();
?>

<style>
    html {
        font-family: "Verdana";
    }

    table {
        border: 2px solid green;
        border-radius: 8px;
        text-align: center;
    }

    th, td {
        padding: 0.5rem;
        border-radius: 4px;
    }

    .wrapper {
        display: flex;
        flex-direction: column;
        justify-content: center;
        align-items: center;
    }

    .wrapper > *:nth-child(odd) {
        margin-bottom: 1rem;
    }

    .change-wrapper {
        display: flex;
        flex-direction: row;
        justify-content: center;
        align-items: flex-start;
        width: 100%;
    }

    .replace-wrapper {
        display: flex;
        flex-direction: column;
        margin-right: 1rem;
    }

    .replace-wrapper > *:nth-child(odd) {
        margin-bottom: 1rem;
    }
</style>

<form action="<?php echo htmlspecialchars($_SERVER["PHP_SELF"]);?>">
    <div class="wrapper">
        <table>
            <thead>
                <tr>
                    <th></th>
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
                    <td><input type="radio" name="id" onChange="handleRowChange(this.value);" value="<?echo $row['id']?>" <?echo $row['id'] == $id ? "checked" : ""?>></td>
                    <?foreach ($row as $el) {?>
                        <td>
                            <?echo $el?>
                        </td>
                    <?}?>
                </tr>
            <?}?>
        </table>
        <?if ($id == -1) {?>
            <button type="submit">Выбрать</button>
        <?}?>
        <?if ($id != -1) {?>
        <div class="change-wrapper">
            <div class="replace-wrapper">
                <select name="field_name">
                    <option value="name"><?echo $current_row['name']?></option>
                    <option value="city"><?echo $current_row['city']?></option>
                    <option value="address"><?echo $current_row['address']?></option>
                    <option value="birthday"><?echo $current_row['birthday']?></option>
                    <option value="mail"><?echo $current_row['mail']?></option>
                </select>
                <button type="submit" onClick=>Заменить</button>
            </div>
            <input name="field_value" type="text"/>
        </div>
        <?}?>
    </div>
</form>
<a href="./task3.php">task3.php</a>