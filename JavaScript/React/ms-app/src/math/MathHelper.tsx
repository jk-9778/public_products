import Vector2 from "./Vector2";

//計算お助け用クラス
export default class MathHelper {

  //2点間の距離を返す
  static distance(p1: Vector2, p2: Vector2): number {
    return Math.sqrt((p1.x - p2.x) ** 2 + (p1.y - p2.y) ** 2);
  }

  //2点間のベクトルの正規化した値を返す
  static normalize(vec: Vector2): Vector2 {
    const len = Math.sqrt(vec.x ** 2 + vec.y ** 2);
    if (len > 0.0000001) {
      vec = new Vector2(vec.x / len, vec.y / len)
    }
    return new Vector2();
  }

  //ラジアンを角度に変換
  static toDegree(radian: number): number {
    return radian * (180.0 / Math.PI);
  }

  //角度をラジアンに変換
  static toRadian(degree: number): number {
    return degree * (Math.PI / 180.0);
  }

  //クランプ
  static clamp(val: number, low: number, hi: number): number {
    return (val < low) ? low : (val > hi) ? hi : val;
  }

  //線形補間
  static lerp(start: number, end: number, t: number): number {
    return start * (1.0 - t) + end * t;
  }
}