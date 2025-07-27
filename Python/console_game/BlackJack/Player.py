from BasePlayer import *

#プレイヤー
class Player(BasePlayer):
    #コンストラクタ
    def __init__(self, name, chip):
        return super().__init__(name, chip)

    #行動する
    def action(self, deck, bet):
        #行動選択
        while(self.get_score() <= 21):
            key = input("\nHit(h key) or Stand(s key) -> ")
            #入力チェック
            if key == "h" or key == "H":
                self.add(deck.get_card())
                self.draw(bet)
            elif key == "s" or key == "S":
                #ターン終了
                self.draw(bet)
                break
            else:
                print("もう一度入力してください！")

