import pygame
from pygame.locals import *
import random
from Vector2 import *
from Block import *
import enum

#マインスイーパー
class MineSweeper:
    #ゲームの状態種別
    Playing = 0
    Cleared = 1
    Failed = 2


    #ウィンドウサイズ
    WindowWidth = 320
    WindowHeight = 320

    #盤面の原点
    BoradOrigin = Vector2(80, 80)
    #マスの数(横)
    BoardWidth = 10
    #マスの数(縦)
    BoardHeight = 10
    #マスの大きさ
    GridSize = 16
    #地雷の個数
    TotalMineNum = 12

    #コンストラクタ
    def __init__(self):
        #盤面の情報を格納するための2次元配列
        self.board = [[None for i in range(MineSweeper.BoardWidth)] for j in range(MineSweeper.BoardHeight)]
        #1フレーム前のマウスの状態を取得
        self.prev_mouse = [False,  False]
        #現在のゲームの状態
        self.state = MineSweeper.Playing

    #ゲームの実行
    def run(self):
        #pygameの初期化
        pygame.init()
        #スクリーンの初期化
        self.screen = pygame.display.set_mode((MineSweeper.WindowWidth, MineSweeper.WindowHeight))
        #ウェイトタイマの作成
        clock = pygame.time.Clock()
        #開始
        self.start()
        #ゲームループ
        is_end = False
        while is_end == False:
            #更新
            self.update(1.0)
            #画面消去
            self.screen.fill((0, 128, 255))
            #描画
            self.draw()
            #画面の更新
            pygame.display.update()
            #タイマウェイト
            clock.tick(60)
            #終了チェック
            for event in pygame.event.get():
                if event.type == QUIT or (event.type == KEYDOWN and event.key == K_ESCAPE):
                    is_end = True
        #pygameの終了
        pygame.quit()

    #リソースの読み込み
    def load_content(self):
        self.texture_block = pygame.image.load("assets/block.png")              #ブロックの画像
        self.texture_flag = pygame.image.load("assets/flag.png")                #フラッグの画像
        self.texture_grid = pygame.image.load("assets/grid.png")                #グリッドの画像
        self.texture_mine = pygame.image.load("assets/mine.png")                #爆弾の画像
        self.texture_face_normal = pygame.image.load("assets/face_normal.png")  #顔(普通)の画像
        self.texture_face_smile = pygame.image.load("assets/face_smile.png")    #顔(笑顔)の画像
        self.texture_face_dying = pygame.image.load("assets/face_dying.png")    #顔(ゲッソリ)の画像
        #0～9までの画像
        self.texture_num = [pygame.image.load("assets/num_" + str(i) + ".png") for i in range(10)]

    #開始
    def start(self):
        #リソースの読み込み
        self.load_content()
        #盤面の初期化
        self.init_board()

    #更新
    def update(self, game_time):
        #マウスのボタン入力の情報を取得
        mouse_button = pygame.mouse.get_pressed()
        #マウス座標を取得
        mouse_position = pygame.mouse.get_pos()
        #マウスカーソルの座標をマス目の番号に変換する
        x = int((mouse_position[0] - MineSweeper.BoradOrigin.x) / MineSweeper.GridSize)
        y = int((mouse_position[1] - MineSweeper.BoradOrigin.y) / MineSweeper.GridSize)

        #左クリックされたか？
        if mouse_button[0] == False and self.prev_mouse[0] == True:
            #盤面の外側がクリックされた時は何もしない
            if self.is_inside_board(x, y) == False:
                return
            self.board[y][x].on_left_click()

            #クリックした場所が地雷ならゲームオーバー
            if self.board[y][x].is_mine == True:
                self.state = MineSweeper.Failed
            elif self.is_cleared() == True:
                self.state = MineSweeper.Cleared

        #右クリックされたか？
        if mouse_button[1] == False and self.prev_mouse[1] == True:
            #盤面の外側がクリックされた時は何もしない
            if self.is_inside_board(x, y) == False:
                return
            self.board[y][x].on_right_click()


        #マウスの状態を保存
        self.prev_mouse[0] = mouse_button[0]
        self.prev_mouse[1] = mouse_button[1]

    #描画
    def draw(self):
        #背景色
        bgColor = pygame.Color(0, 0, 0)
        #顔画像
        faceTexture = self.texture_face_normal

        #ゲームの状態によって、背景色と顔画像を変更する
        if self.state == MineSweeper.Playing:
            bgColor = pygame.Color(100, 149, 237)
            faceTexture = self.texture_face_normal
        elif self.state == MineSweeper.Cleared:
            bgColor = pygame.Color(255, 255, 0)
            faceTexture = self.texture_face_smile
        else:
            bgColor = pygame.Color(255, 0, 0)
            faceTexture = self.texture_face_smile

        #背景色で画面全体を塗りつぶす
        self.screen.fill((bgColor))

        #顔画像を描画
        self.screen.blit(faceTexture, (self.WindowWidth / 2 - 8, 32))

        #盤面の描画
        for y in range(MineSweeper.BoardHeight):
            for x in range(MineSweeper.BoardWidth):
                self.board[y][x].draw(self.screen)

    #地中の情報を初期化する
    def init_board(self):
        #二次元配列の全要素にBlockインスタンスを格納する
        for y in range(MineSweeper.BoardHeight):
            for x in range(MineSweeper.BoardWidth):
                #マスのスクリーン座標を計算
                position = Vector2(MineSweeper.GridSize * x, MineSweeper.GridSize * y) + MineSweeper.BoradOrigin
                #ブロックのインスタンスを生成して二次元配列に格納する
                self.board[y][x] = Block(position, self.texture_block, self.texture_grid, self.texture_flag, self.texture_mine, self.texture_num)

        #ランダムな場所に地雷を埋める
        mineNum = 0
        while(mineNum < MineSweeper.TotalMineNum):
            x = random.randint(0, MineSweeper.BoardWidth - 1)
            y = random.randint(0, MineSweeper.BoardHeight - 1)
            #適当に選んだ場所にまだ地雷が無ければ、そこに地雷を埋める
            if self.board[y][x].is_mine == False:
                self.board[y][x].is_mine = True
                mineNum += 1

        #全てのマスについて、周囲の地雷の数を数え、格納する
        for y in range(MineSweeper.BoardHeight):
            for x in range(MineSweeper.BoardWidth):
                #そのマスが地雷の場合は何もしない
                if self.board[y][x].is_mine == True:
                    continue
                #地雷でない場合、周囲の地雷の数を数え、格納する
                self.board[y][x].neighbor_mine_count = self.count_neighbor_mines(x, y)

    #指定されたマスの周囲に地雷が何個あるかを亜k添える
    def count_neighbor_mines(self, x, y):
        mineNum = 0
        if self.is_mine(x - 1, y - 1) == True: mineNum += 1 #左上
        if self.is_mine(x + 0, y - 1) == True: mineNum += 1 #上
        if self.is_mine(x + 1, y - 1) == True: mineNum += 1 #右上
        if self.is_mine(x - 1, y + 0) == True: mineNum += 1 #左
        if self.is_mine(x + 1, y + 0) == True: mineNum += 1 #右
        if self.is_mine(x - 1, y + 1) == True: mineNum += 1 #左下
        if self.is_mine(x + 0, y + 1) == True: mineNum += 1 #下
        if self.is_mine(x + 1, y + 1) == True: mineNum += 1 #右下
        return mineNum

    #指定されたマスが地雷か？
    def is_mine(self, x, y):
        return self.is_inside_board(x, y) == True and self.board[y][x].is_mine

    #指定されたマスが盤面の内側か？
    def is_inside_board(self, x, y):
        return x >= 0 and x < MineSweeper.BoardWidth and y >= 0 and y < MineSweeper.BoardHeight

    #クリアか？
    def is_cleared(self):
        for y in range(MineSweeper.BoardHeight):
            for x in range(MineSweeper.BoardWidth):
                #地雷ではないのに開かれていないということは未クリア
                if self.board[y][x].is_mine == False and self.board[y][x].opened == False:
                    return False
        #ここまで来たら、地雷でないマスは全て開かれているということなので、クリア
        return True


#ゲームの実行
MineSweeper().run()

