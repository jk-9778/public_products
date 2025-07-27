from Card import *

#手札
class Hand:
    #コンストラクタ
    def __init__(self):
        self.hand = []

    #カードを追加
    def add(self, card):
        self.hand.append(card)

    #全カードを描画
    def draw_all(self):
        for i in range(len(self.hand)):
            self.hand[i].draw()

    #全カードを表にする
    def face_up_all(self):
        for i in range(len(self.hand)):
            self.hand[i].face = True

    #スコアの計算
    def score(self):
        #エースは最後に都合のいい方で加算する
        ace = 0
        sum = 0
        for i in range(len(self.hand)):
            if self.hand[i].rank == 1:
                ace += 1
                continue
            elif self.hand[i].rank >= 10:
                sum += 10
            else:
                sum += self.hand[i].rank
        #合計が10を超えていなければエースを11として加算する
        for i in range(ace):
            if sum <= 10:
                sum += 11
            else:
                sum += 1
        #合計スコアを返す
        return sum

    #ブラックジャックか？
    def is_blackjack(self):
        if len(self.hand) == 2 and self.score() == 21:
            return True
        else:
            return False

    #カードをクリア
    def clear(self):
        self.hand.clear()

