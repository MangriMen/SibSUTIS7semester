<?php
$var1="Alice";
$var2="My friend is $var1";
$var3='My friend is $var1';
$var4=&$var1;

?>

<h3>Before changed var1</h3>

<ol>
    <li><?echo $var1?></li>
    <li><?echo $var2?></li>
    <li><?echo $var3?></li>
    <li><?echo $var4?></li>
</ol>

<?php
$var1="Bob";
?>

<h3>After changed var1</h3>
<ol>
    <li><?echo $var1?></li>
    <li><?echo $var2?></li>
    <li><?echo $var3?></li>
    <li><?echo $var4?></li>
</ol>

<?php
$user="Michael";
$$user="Jackson";
?>