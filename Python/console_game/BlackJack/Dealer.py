from BasePlayer import *

#ディーラー
class Dealer(BasePlayer):
    #コンストラクタ
    def __init__(self, name, chip):
        return super().__init__(name, chip)

    #行動する
    def action(self, deck):
        while(self.get_score() < 17):
            self.add(deck.get_card())
            self.draw()
