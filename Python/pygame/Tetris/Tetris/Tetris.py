import pygame
from pygame.locals import *
import random
from Vector2 import *
import math
from BlockGroup import *
from Info import *

#ブロック崩し
class Tetris:
    #ゲームの状態種別
    Ready = 0
    Play = 1
    GameOver = 2
    Clear = 3

    #ブロックの形
    Shape0 = [[0,0,0],[0,0,1],[1,1,1]]
    Shape1 = [[1,1],[1,1]]
    Shape2 = [[0,0,0],[0,1,1],[1,1,0]]
    Shape3 = [[0,0,0],[1,1,1],[0,1,0]]
    Shape4 = [[0,0,0],[1,1,1],[0,0,1]]
    Shape5 = [[0,0,0,0],[0,0,0,0],[1,1,1,1],[0,0,0,0]]
    Shape6 = [[0,0,0],[1,1,0],[0,1,1]]

    #コンストラクタ
    def __init__(self):
        #1フレーム前のキー入力の状態を保存(A・D・左・右・スペース)
        self.prev_key = [False,  False, False, False, False]
        #現在のゲームの状態
        self.state = Tetris.Ready
        #ブロックの形を数字で扱えるように配列として持つ
        self.shapes = [Tetris.Shape0, Tetris.Shape1, Tetris.Shape2, Tetris.Shape3, Tetris.Shape4, Tetris.Shape5, Tetris.Shape6]
        #ブロックが落下する間隔(ミリ秒)
        self.fall_span = 500
        #経過時間
        self.elapse_time = 0
        #プレイエリア
        self.area = [[0 for i in range(Info().AreaWidth)] for j in range(Info().AreaHeight)]
        #落下ブロック
        self.fall_blocks = None

    #ゲームの実行
    def run(self):
        #pygameの初期化
        pygame.init()
        #スクリーンの初期化
        self.screen = pygame.display.set_mode((Info().WindowWidth, Info().WindowHeight))
        #ウェイトタイマの作成
        clock = pygame.time.Clock()
        #開始
        self.start()
        #ゲームループ
        is_end = False
        while is_end == False:
            #更新
            self.update(clock.get_time())
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
        self.texture_bg = pygame.image.load("assets/bg.png")      #背景の画像
        self.texture_num = pygame.image.load("assets/num.png")    #数字の画像
        #0～6までのブロック画像(色別)
        self.texture_blocks = [pygame.image.load("assets/block_" + str(i) + ".png") for i in range(7)]

    #開始
    def start(self):
        #リソースの読み込み
        self.load_content()

        #最初のブロックを生成
        self.genaratel_block()

    #更新
    def update(self, game_time):
        if self.state == Tetris.Ready:
            self.update_ready(game_time)
        elif self.state == Tetris.Play:
            self.update_play(game_time)
        elif self.state == Tetris.Clear:
            pass
        elif self.state == Tetris.GameOver:
            pass

    #Ready状態の処理
    def update_ready(self, game_time):
        # キーの入力
        input_key = pygame.key.get_pressed()

        #スペースキーが押されたか？
        if input_key[K_SPACE] == False and self.prev_key[4] == True:
            self.state = Tetris.Play

        #スペースキーの状態を保存
        self.prev_key[4] = input_key[K_SPACE]


    #Play状態の処理
    def update_play(self, game_time):
        #一定間隔ごとにブロックを落下させていく
        self.elapse_time += game_time
        if self.elapse_time >= self.fall_span:
            self.fall_blocks.fall_all()
            self.elapse_time = 0;

        # キーの入力
        input_key = pygame.key.get_pressed()

        #Aキーが押されたら
        if input_key[K_a] == False and self.prev_key[0] == True:
            self.fall_blocks.move_left_all()
        #Dキーが押されたら
        if input_key[K_d] == False and self.prev_key[1] == True:
            self.fall_blocks.move_right_all()
        #左キーが押されたら
        if input_key[K_LEFT] == False and self.prev_key[2] == True:
            self.fall_blocks.rotate_left()
        #右キーが押されたら
        if input_key[K_RIGHT] == False and self.prev_key[3] == True:
            self.fall_blocks.rotate_right()
        #スペースキーが押されている間は落下速度アップ
        if input_key[K_SPACE] == True:
            self.fall_span = 100
        else:
            self.fall_span = 500

        #キーの状態を保存
        self.prev_key[0] = input_key[K_a]
        self.prev_key[1] = input_key[K_d]
        self.prev_key[2] = input_key[K_LEFT]
        self.prev_key[3] = input_key[K_RIGHT]

        #ブロックがプレイエリアの下端に達したら
        if self.fall_blocks.check_all_in_area_height() == False:
            #落下ブロックをプレイエリアに反映させる
            self.reflects_fall_blocks()
            self.check_line()
        #ブロックが他のブロックに乗ったら
        elif self.fall_blocks.check_all_on_other_block(self.area) == True:
            #落下ブロックをプレイエリアに反映させる
            self.reflects_fall_blocks()
            self.check_line()

    #描画
    def draw(self):
        #背景色で画面全体を塗りつぶす(一応)
        bgColor = pygame.Color(0, 0, 135)
        self.screen.fill((bgColor))

        ##### ※背景を塗りつぶした後に画像を描画しないと背景に隠れるので注意※ #####
        #背景の描画
        self.screen.blit(self.texture_bg, (0, 0))

        #とりあえず落下ブロックを描画
        self.fall_blocks.draw(self.screen)

        #プレイエリアのブロックを描画
        for y in range(len(self.area)):
            for x in range(len(self.area[0])):
                if self.area[y][x] != 0:
                    self.area[y][x].draw(self.screen)

    #ブロックを生成する
    def genaratel_block(self):
        #現在のブロックを削除
        self.fall_blocks = None
        #ランダムで次のブロックを決定
        n = random.randint(0, len(self.shapes) - 1)
        self.fall_blocks = BlockGroup(Vector2(Info().BlockSize * 3, -Info().BlockSize * len(self.shapes[n])), self.texture_blocks[n], self.shapes[n])

    #落下ブロックをプレイエリアに反映させる
    def reflects_fall_blocks(self):
        blocks = self.fall_blocks.get_blocks()
        for y in range(len(blocks)):
            for x in range(len(blocks[0])):
                if blocks[y][x] != 0:
                    self.area[blocks[y][x].get_block_unit_position().y][blocks[y][x].get_block_unit_position().x] = blocks[y][x]
        #新たなブロックを生成
        self.genaratel_block()

    #横一列揃ったかを確認する
    def check_line(self):
        for y in range(len(self.area)):
            for x in range(len(self.area[0])):
                #下から順にチェックする
                if self.area[len(self.area) - y - 1][x] == 0:
                    return
                else:
                    continue
            #揃っていたら、その行を消して、その行より上を一段ずつ下げる
            if y != len(self.area - 1):
                self.area[len(self.area) - y - 1] = self.area[len(self.area) - y - 2]
            else:
                #一番上の段には0を詰める
                for x in range(len(self.area[0])):
                    self.area[0][x] = 0

#ゲームの実行
Tetris().run()

