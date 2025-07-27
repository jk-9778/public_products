from Deck import *
from Player import *
from Dealer import *

#ブラックジャック
class BlackJack:
    #コンストラクタ
    def __init__(self):
        self.deck = Deck()
        self.player = Player("Player", 100)
        self.dealer = Dealer("Dealer", 1000)
        self.isEnd = False

    #ゲームの実行
    def run(self):
        print("***** Black Jack *****")
        #賭け金
        bet = 0
        #ループ処理
        while(self.isEnd == False):
            #デッキをシャッフル
            self.deck.shuffle()
            print("\nゲームを開始します！　賭けるチップの枚数を入力してください！")
            #入力チェック
            while(True):
                n = input("(1 ～ " + str(self.player.chip) + ")" + " -> ")
                if n.isdigit() == False:
                    print("数字以外が含まれています！もう一度入力してください！")
                    continue
                elif int(n) > self.player.chip:
                    print("所持枚数を超えています！もう一度入力してください！")
                else:
                    bet = int(n)
                    break

            #最初の2枚のカードをそれぞれに配布
            #ディーラー
            self.dealer.add(self.deck.get_card())
            self.dealer.add(self.deck.get_card())
            #1枚目のカードは裏に
            self.dealer.hand.hand[0].face_down()
            #ディーラーのカードを描画
            self.dealer.draw()
            #プレイヤー
            self.player.add(self.deck.get_card())
            self.player.add(self.deck.get_card())
            #プレイヤーのカードを描画
            self.player.draw(bet)

            #プレイヤーの行動
            self.player.action(self.deck, bet)

            #ディーラーの行動
            self.dealer.action(self.deck)

            #勝敗判定
            print("\nゲームの勝敗")
            #点数表示
            self.draw_score(self.dealer.name, self.dealer.get_score(), self.dealer.is_blackjack())
            self.draw_score(self.player.name, self.player.get_score(), self.player.is_blackjack())
            #勝敗表示とチップ増減
            if self.player.get_score() > 21:
                self.draw_judgement(bet, False, False)
            elif self.player.get_score() == self.dealer.get_score():
                print("\n引き分けです、賭けたチップは返還されされます。")
                print("\nチップの枚数 -> " + self.dealer.name + ":" + str(self.dealer.chip) + "枚 " + self.player.name + ":"  + str(self.player.chip) + "枚")
            elif self.player.is_blackjack():
                self.draw_judgement(bet, True, True)
            elif self.dealer.get_score() > 21:
                self.draw_judgement(bet, True, False)
            elif self.player.get_score() > self.dealer.get_score():
                self.draw_judgement(bet, True, False)
            else:
                self.draw_judgement(bet, False, False)

            #破産判定
            if self.dealer.chip <= 0:
                print("\nディーラーが破産しました！ プレイヤーの勝ちです！")
                self.isEnd = True
            elif self.player.chip <= 0:
                print("\nプレイヤーが破産しました！ プレイヤーの負けです！")
                self.isEnd = True
            else:
                while(True):
                    key = input("\nゲームを続けますか？ (y or n) -> ")
                    if key == "y" or key == "y":
                        #デッキと手札をクリア
                        self.deck.clear()
                        self.dealer.clear()
                        self.player.clear()
                        break
                    elif key == "n" or key == "N":
                        self.isEnd = True
                        break
                    else:
                        print("\nもう一度入力してください！")

            #ゲーム終了
            if self.isEnd == True:
                print("\nゲームを終了します。")
                break

    #スコアを表示
    def draw_score(self, name, score, blackjack):
        if score >= 22:
            print(name + " : バースト")
        elif blackjack == True:
            print(name + " : " + str(score) + "点 Black Jack")
        else:
            print(name + " : " + str(score) + "点")

    #勝敗を表示
    def draw_judgement(self, bet, judgement, blackjack):
        if judgement == True and blackjack == True:
            reward = int(bet * 2.5)
            print("\nプレイヤーの勝ちです！")
            print("プレイヤーは" + str(reward) + "枚のチップを受け取りました！")
            self.player.increase(reward)
            self.dealer.decrease(reward)
        elif judgement == True:
            print("\nプレイヤーの勝ちです！")
            print("プレイヤーは" + str(bet) + "枚のチップを受け取りました！")
            self.player.increase(bet)
            self.dealer.decrease(bet)
        else:
            print("プレイヤーの負けです！")
            print("プレイヤーは" + str(bet) + "枚のチップを失いました…")
            self.player.decrease(bet)
            self.dealer.increase(bet)
        print("\nチップの枚数 -> " + self.dealer.name + ":" + str(self.dealer.chip) + "枚 " + self.player.name + ":"  + str(self.player.chip) + "枚")