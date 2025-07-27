/** @jsxImportSource @emotion/react */
import { Box, Grid } from '@mui/material'
import React from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { setGrids } from '../../../features/GameStateSlice';
import { RootState } from '../../../features/ReduxStore';
import { GridState, GridType } from '../../manager/GameManager';
import StageGrid from './StageGrid'

const GameStage = React.memo(() => {
  const dispatch = useDispatch();
  //ステージサイズ
  const stageSize = useSelector((state: RootState) => state.game.size);
  //爆弾の数
  const mineCount = useSelector((state: RootState) => state.game.mine);;
  const test = useSelector((state: RootState) => state.game.reBuild);;
  //爆弾ランダム配置用配列
  let mines: number[] = [];
  for (let i = 0; i < stageSize ** 2; i++) {
    if (i < stageSize ** 2 - mineCount) {
      mines[mines.length] = GridType.none;
    } else {
      mines[mines.length] = GridType.mine;
    }
  }
  //フィッシャーイェーツ方式でシャッフル
  for (let i = 0; i < mines.length; i++) {
    const r = Math.floor(Math.random() * (i + 1));
    [mines[i], mines[r]] = [mines[r], mines[i]];
  }

  //ステージ用配列
  let grids: number[][] = [];
  for (let i = 0; i < stageSize; i++) {
    grids[i] = [];
    for (let j = 0; j < stageSize; j++) {
      grids[i][j] = mines[i + j * stageSize];
    }
  }
  //gridsの中身を全て置き換えて渡す
  dispatch(setGrids(grids.map(grid => grid.map(g => g = GridState.close))));

  //そのマスがステージ内かを返す
  const isInStage = (x: number, y: number): Boolean => {
    return !(x < 0 || y < 0 || x >= stageSize || y >= stageSize);
  }

  //そのマスが爆弾かを判定して返す
  const isMine = (x: number, y: number): Boolean => {
    if (!isInStage(x, y)) {
      //ステージ外ならfalseを返す
      return false;
    } else {
      if (grids[y][x] === GridType.mine) {
        return true;
      } else {
        return false;
      }
    }
  }

  //周囲の爆弾の数を数える
  const checkSurround = (x: number, y: number): number => {
    let count = 0;
    isMine(x, y - 1) && count++;       //上
    isMine(x + 1, y - 1) && count++;   //右上
    isMine(x + 1, y) && count++;       //右
    isMine(x + 1, y + 1) && count++;   //右下
    isMine(x, y + 1) && count++;       //下
    isMine(x - 1, y + 1) && count++;   //左下
    isMine(x - 1, y) && count++;       //左
    isMine(x - 1, y - 1) && count++;   //左上
    return count;
  }

  //周囲の地雷以外のマスを配列で返す
  const getSurrounds = (x: number, y: number): { mineCount: number, nones: { x: number, y: number, mine: number }[] } => {
    let nones: { x: number, y: number, mine: number }[] = [];
    let mine = 0;
    if (isInStage(x, y - 1)) isMine(x, y - 1) ? mine++ : nones[nones.length] = { x: x, y: y - 1, mine: checkSurround(x, y - 1) };                   //上
    if (isInStage(x + 1, y - 1)) isMine(x + 1, y - 1) ? mine++ : nones[nones.length] = { x: x + 1, y: y - 1, mine: checkSurround(x + 1, y - 1) };   //右上
    if (isInStage(x + 1, y)) isMine(x + 1, y) ? mine++ : nones[nones.length] = { x: x + 1, y: y, mine: checkSurround(x + 1, y) };                   //右
    if (isInStage(x + 1, y + 1)) isMine(x + 1, y + 1) ? mine++ : nones[nones.length] = { x: x + 1, y: y + 1, mine: checkSurround(x + 1, y + 1) };   //右下
    if (isInStage(x, y + 1)) isMine(x, y + 1) ? mine++ : nones[nones.length] = { x: x, y: y + 1, mine: checkSurround(x, y + 1) };                   //下
    if (isInStage(x - 1, y + 1)) isMine(x - 1, y + 1) ? mine++ : nones[nones.length] = { x: x - 1, y: y + 1, mine: checkSurround(x - 1, y + 1) };   //左下
    if (isInStage(x - 1, y)) isMine(x - 1, y) ? mine++ : nones[nones.length] = { x: x - 1, y: y, mine: checkSurround(x - 1, y) };                   //左
    if (isInStage(x - 1, y - 1)) isMine(x - 1, y - 1) ? mine++ : nones[nones.length] = { x: x - 1, y: y - 1, mine: checkSurround(x - 1, y - 1) };   //左上
    return { mineCount: mine, nones: nones };
  }

  return (
    <Box>
      {grids.map((col, y) => (
        <Grid key={y} container spacing={0} flexWrap='nowrap'>
          {col.map((low, x) => (
            <StageGrid
              key={y + x}
              x={x}
              y={y}
              type={grids[y][x]}
              surrounds={getSurrounds(x, y)}
            />
          ))}
        </Grid>
      ))}
    </Box>
  )
})

export default GameStage