from Vector2 import *

#ゲーム全体の情報をまとめてここに記述する
class Info:
    #ウィンドウサイズ
    WindowWidth = 640
    WindowHeight = 480

    #ブロックサイズ
    BlockSize = 16

    #プレイエリアサイズ(ブロック単位)
    AreaWidth = 10
    AreaHeight = 20

    #座標補間(画面左上から盤面左上までの距離)
    Offset = Vector2(240, 112)
