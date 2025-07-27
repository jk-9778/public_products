import random
from Card import *
from Trump import *
import random

#デッキ
class Deck():

    #コンストラクタ
    def __init__(self):
        self.deck = []
        for i in range(4):
            for j in range(13):
                if i == Trump.SPADE:
                    self.deck.append(Card(Trump.SPADE, j + 1, True))
                if i == Trump.CLUB:
                    self.deck.append(Card(Trump.CLUB, j + 1, True))
                if i == Trump.DIAMOND:
                    self.deck.append(Card(Trump.DIAMOND, j + 1, True))
                if i == Trump.HEART:
                    self.deck.append(Card(Trump.HEART, j + 1, True))

    #カードを追加
    def add(self, card):
        self.deck.append(card)

    #カード取得
    def get_card(self):
        return self.deck.pop(0)

    #カードをシャッフル
    def shuffle(self):
        random.shuffle(self.deck)
        random.shuffle(self.deck)

    #全カードの描画
    def draw_all(self):
        for i in range(len(self.deck)):
            self.deck[i].draw()

    #カードをクリア
    def clear(self):
        self.deck.clear()
        for i in range(4):
            for j in range(13):
                if i == Trump.SPADE:
                    self.deck.append(Card(Trump.SPADE, j + 1, True))
                if i == Trump.CLUB:
                    self.deck.append(Card(Trump.CLUB, j + 1, True))
                if i == Trump.DIAMOND:
                    self.deck.append(Card(Trump.DIAMOND, j + 1, True))
                if i == Trump.HEART:
                    self.deck.append(Card(Trump.HEART, j + 1, True))


