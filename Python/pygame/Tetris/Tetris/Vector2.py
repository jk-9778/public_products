import math

#2Dベクトル
class Vector2:
    #コンストラクタ
    def __init__(self, x = 0.0, y = 0.0):
        self.x = x
        self.y = y

    #指定角度の単位ベクトルを求める
    @staticmethod
    def from_angle(degree):
        rad = math.radians(degree)
        return Vector2(math.cos(rad), math.sin(rad))

    #内積
    def dot(self, other):
        return self.x * other.x + self.y * other.y

    #長さ
    def length(self):
        return math.sqrt(self.dot(self))

    #距離
    def distance(self, other):
        return (self - other).length()

    #正規化
    def normalized(self):
        result = Vector2(self.x, self.y)
        len = result.length()
        return (result / len) if (len != 0.0) else result

    #逆ベクトル
    def __neg__(self):
        return Vector2(-self.x, -self.y)

    #加算
    def __add__(self, other):
        return Vector2(self.x + other.x, self.y + other.y)

    #減算
    def __sub__(self, other):
        return Vector2(self.x - other.x, self.y - other.y)

    #乗算
    def __mul__(self, scalar):
        return Vector2(self.x * scalar, self.y * scalar)

    #除算
    def __truediv__(self, scalar):
        return Vector2(self.x / scalar, self.y / scalar)

    #加算
    def __iadd__(self, other):
        self.x += other.x
        self.y += other.y
        return self

    #減算
    def __isub__(self, other):
        self.x -= other.x
        self.y -= other.y
        return self

    #乗算
    def __imul__(self, scalar):
        self.x *= scalar
        self.y *= scalar
        return self

    #除算
    def __truediv(self, scalar):
        self.x /= scalar
        self.y /= scalar
        return self

    #文字列化
    def __str__(self):
        return '(%s %s)' % (self.x, self.y)
