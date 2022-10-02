<?php
$cust = array();
$cust["cnum"] = 2001;
$cust["cname"] = "Hoffman";
$cust["city"] = "London";
$cust["snum"] = 1001;
$cust["rating"] = 100;

printArray($cust);

asort($cust);
printArray($cust);

ksort($cust);
printArray($cust);

sort($cust);
printArray($cust);
?>

<?function printArray($cust) {?>
    <div>
        <?foreach ($cust as $key => $value) {?>
            <span style="display: block"><?echo "$key = $value"?></span>
        <?}?>
    </div>
    <br>
<?}?>
