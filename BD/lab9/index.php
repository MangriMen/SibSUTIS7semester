<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Document</title>
</head>
<body>
    <?for ($i = 1; $i <= 3; $i++) {?>
        <a href="task<?echo $i?>.php" style="grid-column: <?echo $i?>;">Task <?echo $i?></a>
    <?}?>

    <a href="task4.php" style="grid-column: 4;">Task 4</a>
</body>
</html>
