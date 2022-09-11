<?php
if (array_key_exists("lang", $_GET)){
    $lang=$_GET["lang"];
} else {
    $lang="";
}
$langFullName="";

if ($lang == "ru") {
    $langFullName="русский";
} elseif ($lang == "en") {
    $langFullName="английский";
} elseif ($lang == "fr") {
    $langFullName="французский";
} elseif ($lang == "de") {
    $langFullName="немецкий";
} else {
    $langFullName="неизвестный";
}
?>

<p><?echo $langFullName?></p>