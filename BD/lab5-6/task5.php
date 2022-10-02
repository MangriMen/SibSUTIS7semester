<?function DisplayGreetings($text, $style) {?>
    <p style="font-size: 1.2rem; <?echo $style?>"><?echo $text?></p>
<?}?>

<?php
function Ru($color) {
    DisplayGreetings("Здравствуйте!", "color: $color;");
}

function En($color) {
    DisplayGreetings("Hello!", "color: $color;");
}

function Fr($color) {
    DisplayGreetings("Bonjour!", "color: $color;");
}

function De($color) {
    DisplayGreetings("Guten Tag!", "color: $color;");
}

$lang = array_key_exists("lang", $_GET) ? $_GET["lang"] : "";
$color = array_key_exists("color", $_GET) ? $_GET["color"] : "";

$lang($color);
?>
