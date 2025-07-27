from Hand import *

#プレイヤー基底
class BasePlayer:
    #コンストラクタ
    def __init__(self, name, chip):
        self.name = name
        self.chip = chip
        self.hand = Hand()

    #行動する
    def action(self):
        pass

    #描画
    def draw(self, bet = 0):
        print("\n" + self.name + " Chip:" + str(self.chip) + "枚 Bet:" + str(bet) + "枚\n")
        self.hand.draw_all()

    #手札にカードを加える
    def add(self, card):
        self.hand.add(card)

    #全カードを表にする
    def open(self):
        self.hand.face_up_all()

    #チップを増やす
    def increase(self, chip):
        self.chip += chip

    #チップを減らす
    def decrease(self, chip):
        self.chip -= chip

    #手札の点数を返す
    def get_score(self):

        return self.hand.score()

    #手札の点数を表示
    def draw_point(self):
        print(self.name + " : " + str(get_score()) + "点")

    #ブラックジャックか？
    def is_blackjack(self):
        return self.hand.is_blackjack()

    #手札を消去する
    def clear(self):
        self.hand.clear()
