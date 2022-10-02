<?php

$treug = array();
for ($i = 1; $i <= 10; $i++) {
    $treug[$i] = $i*($i+1)/2;
}

$kvd = array();
for ($i = 1; $i <= 10; $i++) {
    $kvd[$i] = $i * $i;
}

$rez = array_merge($treug, $kvd);
?>

<div style="display: flex; flex-direction: column;">
        <span>
            <?foreach ($treug as &$value) {
                echo $value . "&nbsp&nbsp";
            }?>
            :$treug
        </span>

        <span>
            <?foreach ($kvd as &$value) {
                echo $value . "&nbsp&nbsp";
            }?>
            :$kvd
        </span>

        <span>
            <?foreach ($rez as &$value) {
                echo $value . "&nbsp&nbsp";
            }?>
            :$rez
        </span>

        <?sort($rez);?>

        <span>
            <?foreach ($rez as &$value) {
                echo $value . "&nbsp&nbsp";
            }?>
            :sorted $rez
        </span>
        
        <?unset($rez[1]);?>

        <span> 
            <?foreach ($rez as &$value) {
                echo $value . "&nbsp&nbsp";
            }?>
            :removed 1 el
        </span>

        <?$rez1 = array_unique($rez);?>

        <span>
            <?foreach ($rez1 as &$value) {
                echo $value . "&nbsp&nbsp";
            }?>
             :removed unique
        </span>

        <??>
    </div>