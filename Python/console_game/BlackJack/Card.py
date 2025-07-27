from Trump import *

#カード
class Card:
    #コンストラクタ
    def __init__(self, suit, rank, face):
        self.suit = suit
        self.rank = rank
        self.face = face

    #描画
    def draw(self):
        if self.face == True:
            if self.suit == Trump.SPADE:
                if self.rank == 1:
                    print("スペードのエース")
                elif self.rank == 11:
                    print("スペードのジャック")
                elif self.rank == 12:
                    print("スペードのクイーン")
                elif self.rank == 13:
                    print("スペードのキング")
                else:
                    print("スペードの" + str(self.rank))
            elif self.suit == Trump.CLUB:
                if self.rank == 1:
                    print("クラブのエース")
                elif self.rank == 11:
                    print("クラブのジャック")
                elif self.rank == 12:
                    print("クラブのクイーン")
                elif self.rank == 13:
                    print("クラブのキング")
                else:
                    print("クラブの" + str(self.rank))
            elif self.suit == Trump.DIAMOND:
                if self.rank == 1:
                    print("ダイヤのエース")
                elif self.rank == 11:
                    print("ダイヤのジャック")
                elif self.rank == 12:
                    print("ダイヤのクイーン")
                elif self.rank == 13:
                    print("ダイヤのキング")
                else:
                    print("ダイヤの" + str(self.rank))
            elif self.suit == Trump.HEART:
                if self.rank == 1:
                    print("ハートのエース")
                elif self.rank == 11:
                    print("ハートのジャック")
                elif self.rank == 12:
                    print("ハートのクイーン")
                elif self.rank == 13:
                    print("ハートのキング")
                else:
                    print("ハートの" + str(self.rank))
        else:
            print("???")

    #表にする
    def face_up(self):
        self.face = True

    #裏にする
    def face_down(self):
        self.face = False
