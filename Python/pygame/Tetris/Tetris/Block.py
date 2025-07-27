import pygame
from pygame.locals import *
from Vector2 import *
from Info import *

class Block:
    #コンストラクタ
    def __init__(self, pos, tex):
        #座標
        self.position = pos
        #ブロックの画像
        self.texture = tex

    #描画
    def draw(self, screen):
        screen.blit(self.texture, (self.position.x + Info().Offset.x, self.position.y + Info().Offset.y))

    #左移動
    def move_left(self):
        self.position.x -= Info().BlockSize

    #右移動
    def move_right(self):
        self.position.x += Info().BlockSize

    #落下
    def fall(self):
        self.position.y += Info().BlockSize

    #エリア左端に収まっているか？
    def check_in_area_left(self):
        if self.get_block_unit_position().x <= 0:
            return False
        else:
            return True

    #エリア右端に収まっているか？
    def check_in_area_right(self):
        if self.get_block_unit_position().x >= Info().AreaWidth- 1:
            return False
        else:
            return True

    #エリア下端に収まっているか？
    def check_in_area_height(self):
        if self.get_block_unit_position().y >= Info().AreaHeight - 1:
            return False
        else:
            return True

    #他のブロックに乗ったか？
    def check_on_other_block(self, area):
        if area[self.get_block_unit_position().y + 1][self.get_block_unit_position().x] != 0:
            return True
        else:
            return False

    #現在の座標を返す
    def get_position(self):
        return self.position

    #現在の座標をブロック単位に変換して返す
    def get_block_unit_position(self):
        return Vector2(int(self.position.x / Info().BlockSize), int(self.position.y / Info().BlockSize))

    #使っているテクスチャを返す
    def get_texture(self):
        return self.texture

    #座標を設定
    def set_position(self, pos):
        self.position = pos

