import MathHelper from "./MathHelper";

//2次元ベクトルクラス
export default class Vector2 {
  x: number;
  y: number;

  constructor(x: number = 0, y: number = 0) {
    this.x = x;
    this.y = y;
  }

  //内積を求める
  dot(other: Vector2): number {
    return (this.x * other.x) + (this.y * other.y);
  }

  //外積を求める
  cross(other: Vector2): number {
    return (this.x * other.x) - (this.y * other.y);
  }

  //長さを求める
  length(): number {
    return Math.sqrt(this.dot(this));
  }

  //正規化する
  normalize(): Vector2 {
    if (this.length() >= 0.0000001) {
      return new Vector2(this.x / this.length(), this.y / this.length());
    }
    return new Vector2();
  }

  //角度からベクトルを作成
  fromAngle(degree: number): Vector2 {
    const rad = MathHelper.toRadian(degree); //弧度法に変換
    return new Vector2(Math.cos(rad), Math.sin(rad));
  }

  //ベクトルが向いている角度を求める
  toAngle(): number {

    if (this.isZero()) return 0.0; //ゼロベクトルは角度を求められない
    return MathHelper.toDegree(Math.atan2(this.x, this.y));
  }

  //回転
  rotate(degree: number): void {
    const rad = MathHelper.toRadian(degree);
    this.x = this.x * Math.cos(rad) - this.y * Math.sin(rad);
    this.y = this.x * Math.sin(rad) + this.y * Math.cos(rad);
  }

  //クランプ
  clamp(min: Vector2, max: Vector2): void {
    this.x = MathHelper.clamp(this.x, min.x, max.x);
    this.y = MathHelper.clamp(this.y, min.y, max.y);
  }

  //二つのベクトルのなす角を求める
  innerAngle(other: Vector2): number {
    const n1 = this.normalize();
    const n2 = other.normalize();
    const cos = MathHelper.clamp(n1.dot(n2), -1.0, 1.0);
    return MathHelper.toDegree(Math.acos(cos));
  }

  //ターゲットとの距離を求める
  distance(target: Vector2): number {
    return new Vector2(this.x - target.x, this.y - target.y).length();
  }

  //ターゲット方向のベクトルを求める
  toTarget(target: Vector2): Vector2 {
    return new Vector2(target.x - this.x, target.y - this.y).normalize();
  }

  //ターゲット方向の角度を求める
  toTargetAngle(target: Vector2): number {
    return this.toTarget(target).toAngle();
  }

  //ゼロベクトルか？
  isZero(): boolean {
    return this.x === 0 && this.y === 0;
  }
}
