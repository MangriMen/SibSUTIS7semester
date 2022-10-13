from dataclasses import dataclass
from enum import Enum


class Suit(Enum):
    Diamonds = 1
    Hearts = 2
    Clubs = 3
    Spades = 4


@dataclass
class Card:
    nominal: str
    suit: str

    def __repr__(self):
        return f'{self.nominal.capitalize()} of {self.suit.name}'


deck_nominal = ['2', '3', '4', '5', '6', '7', '8',
                '9', '10', 'jack', 'queen', 'king', 'ace']
deck = [Card(nominal, suit) for suit in Suit for nominal in deck_nominal]
