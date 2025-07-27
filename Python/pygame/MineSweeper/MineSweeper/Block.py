import pygame
from pygame.locals import *
from Vector2 import *

class Block:
    #コンストラクタ
    def __init__(self, pos, t_block, t_grid, t_flag, t_mine, t_num):
        #このマスは地雷か？
        self.is_mine = False
        #周囲の地雷の個数
        self.neighbor_mine_count = 0
        #旗が立っているか
        self.flag = False
        #開けられたか
        self.opened = False

        self.texture_block = t_block    #ブロックの画像
        self.texture_flag = t_flag      #フラッグの画像
        self.texture_grid = t_grid      #グリッドの画像
        self.texture_mine = t_mine      #爆弾の画像
        self.texture_num = t_num        #0～9までの画像

        #マスの位置
        self.position = pos

    #描画
    def draw(self, screen):
        #開いている
        if self.opened == True:
            #マス目を描画
            screen.blit(self.texture_grid, (self.position.x, self.position.y))
            #このマスが地雷であれば、地雷を描画
            if self.is_mine == True:
                screen.blit(self.texture_mine, (self.position.x, self.position.y))
            #地雷でなければ、周囲の地雷の数を描画
            else:
                screen.blit(self.texture_num[self.neighbor_mine_count], (self.position.x, self.position.y))
        #開いていない
        else:
            #ブロックを描画
            screen.blit(self.texture_block, (self.position.x, self.position.y))
            #旗が立っていれば、旗を描画
            if self.flag == True:
                screen.blit(self.texture_flag, (self.position.x, self.position.y))


    #左クリックされた時の処理
    def on_left_click(self):
        #もうすでに開いている場合は何もしない
        if self.opened == True:
            return
        #開く
        self.opened = True

    #右クリックされた時の処理
    def on_right_click(self):
        #既に開いてる場合、右クリックされれも、何もしない
        if self.opened == True:
            return
        #旗のON/OFFを切り替える
        if self.flag == True: self.flag = False
        else: self.flag = True

