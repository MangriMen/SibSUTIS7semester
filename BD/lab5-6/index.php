<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Lab 5-6</title>
    <style>
        a {
            font-size: 1.5rem;
            color: #004D40;
            text-decoration: inherit;
        }

        a:hover {
            color: #00BFA5;
        }
    </style>
</head>
<body>
    <div style="display: grid; align-items: flex-start; justify-content: space-around;">
        <?for ($i = 1; $i <= 3; $i++) {?>
        <a href="task<?echo $i?>.php" style="grid-column: <?echo $i?>;">Task <?echo $i?></a>
        <?}?>

        <div style="display: flex; flex-direction: column; grid-column: 4;">
            <a href="task4.php?lang=ru">Task 4 ru</a>
            <a href="task4.php?lang=en">Task 4 en</a>
            <a href="task4.php?lang=fr">Task 4 fr</a>
            <a href="task4.php?lang=de">Task 4 de</a>
            <a href="task4.php?lang=kz">Task 4 kz</a>
        </div>

        <div style="display: flex; flex-direction: column; grid-column: 5;">
            <a href="task5.php?lang=ru&color=red">Task 5 ru red</a>
            <a href="task5.php?lang=en&color=green">Task 5 en green</a>
            <a href="task5.php?lang=fr&color=blue">Task 5 fr blue</a>
            <a href="task5.php?lang=de&color=magenta">Task 5 de magenta</a>
        </div>
    </div>
</body>
</html>

