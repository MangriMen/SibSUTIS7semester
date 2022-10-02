<?php
$username = $_GET['username'];

$questions = array();
$answers = array();
$answers[1] = "СибГУТИ";
$answers[2] = "2147483647";
$answers[3] = "banana";
$answers[4] = "7";
$answers[5] = "Джон Бэкус";

for ($i = 1; $i <= 5; $i++) {
    $questions[$i] = $_GET["question_$i"];
}

$otv = array();

for ($i = 1; $i <= count($questions); $i++) {
    if ($questions[$i] == $answers[$i]) {
        $otv[] = $i;
    }
}

$right_answers = count($otv);

echo "$username, вы ответили правильно на $right_answers вопросов. ";
switch($right_answers) {
    case 0:
        echo "Ну такое.";
        break;
    case 1:
        echo "Неплохо.";
        break;
    case 2:
        echo "Хорошо.";
        break;
    case 3:
        echo "Лучше среднего.";
        break;
    case 4:
        echo "Класс!";
        break;
    case 5:
        echo "Отлично!";
        break;
}
?>
<br>
<a href="task5a.html">task5a.html</a>