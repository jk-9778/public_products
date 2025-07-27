import pygame
from pygame.locals import *
from Vector2 import *
from Info import *
from Block import *

class BlockGroup:
    #コンストラクタ
    def __init__(self, pos, tex, shape):
        #ブロックの座標
        self.position = pos
        #ブロックの画像
        self.texture = tex
        #ブロックの構造配列
        self.shape = shape
        #ブロックの集合体配列
        self.blocks = [[0 for i in range(len(self.shape))] for j in range(len(self.shape[0]))]
        for y in range(len(self.shape)):
            for x in range(len(self.shape[0])):
                if self.shape[y][x] == 1:
                    self.blocks[y][x] = Block(Vector2(self.position.x + Info().BlockSize * x, self.position.y + Info().BlockSize * y), self.texture)

    #描画
    def draw(self, screen):
        for y in range(len(self.blocks)):
            for x in range(len(self.blocks[0])):
                if self.blocks[y][x] != 0:
                    self.blocks[y][x].draw(screen)

    #左回転
    def rotate_left(self):
        #引数の2次元配列 g を時計回りに回転させたものを返す
        t = [[None for i in range(len(self.blocks[0]))] for j in range(len(self.blocks))]
        for y in range(len(self.blocks)):
            for x in range(len(self.blocks[0])):
                t[len(self.blocks[0]) - x - 1][y] = self.blocks[y][x];
        self.blocks = t
        self.set_all_position()

    #右回転
    def rotate_right(self):
        #引数の2次元配列 g を時計回りに回転させたものを返す
        t = [[None for i in range(len(self.blocks[0]))] for j in range(len(self.blocks))]
        for y in range(len(self.blocks)):
            for x in range(len(self.blocks[0])):
                t[x][len(self.blocks) - y - 1] = self.blocks[y][x];
        self.blocks = t
        self.set_all_position()

    #左移動
    def move_left_all(self):
        if self.check_all_in_area_left() == False:
            return
        for y in range(len(self.blocks)):
            for x in range(len(self.blocks[0])):
                if self.blocks[y][x] != 0:
                    self.blocks[y][x].move_left()
        self.position.x -= Info().BlockSize

    #右移動
    def move_right_all(self):
        if self.check_all_in_area_right() == False:
            return
        for y in range(len(self.blocks)):
            for x in range(len(self.blocks[0])):
                if self.blocks[y][x] != 0:
                    self.blocks[y][x].move_right()
        self.position.x += Info().BlockSize

    #落下
    def fall_all(self):
        if self.check_all_in_area_height() == False:
            return
        for y in range(len(self.blocks)):
            for x in range(len(self.blocks[0])):
                if self.blocks[y][x] != 0:
                    self.blocks[y][x].fall()
        self.position.y += Info().BlockSize

    #ブロックが全てエリア左端に収まっているか？
    def check_all_in_area_left(self):
        for y in range(len(self.blocks)):
            for x in range(len(self.blocks[0])):
                if self.blocks[y][x] != 0:
                    if self.blocks[y][x].check_in_area_left() == False:
                        return False
                    else:
                        continue
                else:
                    continue
        return True

    #ブロックが全てエリア右端に収まっているか？
    def check_all_in_area_right(self):
        for y in range(len(self.blocks)):
            for x in range(len(self.blocks[0])):
                if self.blocks[y][x] != 0:
                    if self.blocks[y][x].check_in_area_right() == False:
                        return False
                    else:
                        continue
                else:
                    continue
        return True

    #ブロックが全てエリア下端に収まっているか？
    def check_all_in_area_height(self):
        for y in range(len(self.blocks)):
            for x in range(len(self.blocks[0])):
                if self.blocks[y][x] != 0:
                    if self.blocks[y][x].check_in_area_height() == False:
                        return False
                    else:
                        continue
                else:
                    continue
        return True

    #いずれかのブロックが他のブロックに乗ったか？
    def check_all_on_other_block(self, area):
        for y in range(len(self.blocks)):
            for x in range(len(self.blocks[0])):
                if self.blocks[y][x] != 0:
                    if self.blocks[y][x].check_on_other_block(area) == True:
                        return True
                    else:
                        continue
                else:
                    continue
        return False

    #現在の座標をブロック単位に変換して返す
    def get_block_unit_position(self):
        return Vector2(self.position.x / Info().BlockSize, self.position.y / Info().BlockSize)

    #使っているテクスチャを返す
    def get_texture(self):
        return self.texture

    #自分のブロック型を返す
    def get_blocks(self):
        return self.blocks

    #各ブロックの座標を更新(回転用)
    def set_all_position(self):
        for y in range(len(self.blocks)):
            for x in range(len(self.blocks[0])):
                if self.blocks[y][x] != 0:
                    self.blocks[y][x].set_position(Vector2(x * Info().BlockSize + self.position.x, y * Info().BlockSize + self.position.y))

