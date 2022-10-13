import code
from dataclasses import dataclass
from enum import Enum, EnumMeta
import json
import secrets
from textwrap import indent
from tkinter.messagebox import NO
from typing import Dict, Tuple
import crypto

DECK_NOMINAL = ['2', '3', '4', '5', '6', '7', '8',
                '9', '10', 'jack', 'queen', 'king', 'ace']


class Suit(Enum):
    Diamonds = 1
    Hearts = 2
    Clubs = 3
    Spades = 4


@dataclass
class Card:
    nominal: str
    suit: Suit

    def __hash__(self) -> int:
        return hash((self.nominal, self.suit))

    def __repr__(self) -> str:
        return f'{self.nominal.capitalize()} of {self.suit.name}'

    def __str__(self) -> str:
        return self.__repr__()


class Player:
    _c: int
    _d: int
    p: int

    deck: dict

    decoded_cards: list
    encoded_cards: list

    def __init__(self, p) -> None:
        self.encoded_cards = []
        self.decoded_cards = self.encoded_cards

        self.p = p
        self._c, self._d = self.__generate_c_d(p)

    @staticmethod
    def __generate_c_d(p) -> Tuple[int, int]:
        c = 0
        gcd = ()
        while True:
            c = secrets.randbelow(int(1e9))
            gcd = crypto.extendedGCD(c, p - 1)
            if crypto.isPrime(c) and gcd[0] == 1:
                break
        return c, gcd[1]

    def calculate_codes_for_deck(self, deck: list) -> Dict[int, str]:
        codes_for_deck = set()
        while len(codes_for_deck) < len(deck):
            codes_for_deck.add(secrets.randbelow(self.p - 1) + 1)
        return {code: card for card, code in zip(deck, codes_for_deck)}

    def code_deck(self, cards: list) -> list:
        coded_cards = [pow(card, self._c, self.p) for card in cards]
        secrets.SystemRandom().shuffle(coded_cards)
        return coded_cards

    def deal_cards(self, players: list, coded_deck: list, cards_in_hand: int):
        card_index = 0
        for player in players:
            for _ in range(cards_in_hand):
                player.add_card(coded_deck[card_index])
                card_index += 1

        table_cards = [coded_deck[card_index]
                       for card_index in range(card_index, len(coded_deck))]

        return table_cards

    def set_decoded_cards(self, cards: list) -> None:
        self.decoded_cards = cards.copy()

    def decode_cards_at_other_players(self, players, cards: list) -> list:
        table_cards = cards.copy()
        for other_player in players:
            if self != other_player:
                table_cards = other_player.decode_cards(
                    table_cards)
        return table_cards

    def decode_cards(self, cards: list) -> list:
        return [pow(card, self._d, self.p) for card in cards]

    def add_card(self, card: Card) -> None:
        self.encoded_cards.append(card)


def generate_deck(deck_nominals: list):
    return [Card(nominal, suit) for suit in Suit for nominal in deck_nominals]


def main() -> None:
    deck = generate_deck(DECK_NOMINAL)

    players_count: int = int(input("Enter players count: ") or "2")
    cards_in_hand: int = int(input("Enter number of cards in hand: ") or "2")
    cards_on_table: int = int(
        input("Enter number of cards on the table: ") or "5")

    print()

    cards_count: int = (players_count * cards_in_hand) + cards_on_table

    p: int = crypto.getPrimeInBounds(10000, 1e9)
    players = [Player(p) for _ in range(players_count)]

    # Send deck to players

    random_cards = secrets.SystemRandom().sample(deck, cards_count)
    players[0].deck = players[0].calculate_codes_for_deck(random_cards)
    for player in players:
        if player != players[0]:
            player.deck = players[0].deck

    for card in players[0].deck.items():
        print(f'{card[0]}: {card[1]}')

    print()

    # Encoding deck

    encoded_deck = players[0].code_deck(list(players[0].deck.keys()))
    for i in range(1, len(players)):
        encoded_deck = players[i].code_deck(encoded_deck)

    table_cards = players[-1].deal_cards(players, encoded_deck, cards_in_hand)

    # Encoded cards print

    for player_index, player in enumerate(players):
        print(f'{player_index}: {player.encoded_cards}')
    print(f'Table: {table_cards}')

    print()

    # Decoding cards

    for player in players:
        player.set_decoded_cards(
            player.decode_cards_at_other_players(players, player.decoded_cards))
        player.set_decoded_cards(
            player.decode_cards(player.decoded_cards))

        table_cards = player.decode_cards_at_other_players(
            players, table_cards)

    # Decoded cards print

    for player_index, player in enumerate(players):
        print(
            f'{player_index}: {[player.deck[code] for code in player.decoded_cards]}')
    print(f'Table: {[players[0].deck[code] for code in table_cards]}')


main()
