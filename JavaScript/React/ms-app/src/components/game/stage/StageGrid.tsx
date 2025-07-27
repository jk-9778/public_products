/** @jsxImportSource @emotion/react */
import { Grid } from "@mui/material"
import { Box, css } from "@mui/system"
import React from "react";
import { useDispatch, useSelector } from "react-redux";
import { changeGameState, GameState, setGridState } from "../../../features/GameStateSlice";
import { RootState } from "../../../features/ReduxStore";
import { GridState, GridType } from "../../manager/GameManager";

const StageGrid = React.memo((props: {
  x: number,
  y: number,
  type: number,
  surrounds: { mineCount: number, nones: { x: number, y: number, mine: number }[] },
}) => {
  //周囲の爆弾の数によって数字の色を変える
  const fontColor = [
    '#4c52ff',
    '#00cc44',
    '#ff3232',
    '#11327a',
    '#996600',
    '#00f2f2',
    '#434747',
    '#a2adad'
  ];

  //グリッドCSS
  const closeStyle = css({
    width: '32px',
    height: '32px',
    backgroundColor: '#cccccc',
    borderRadius: '5px',
    border: '1px solid #aaaaaa',
    display: 'flex',
    justifyContent: 'center',
    alignItems: 'center',
    ":hover": {
      cursor: 'default',
      backgroundColor: '#bbbbbb'
    },
  });

  const openStyle = css(closeStyle, {
    backgroundColor: '#eeeeee',
    ":hover": {
      backgroundColor: '#eeeeee'
    },
  });

  //空白CSS
  const noneStyle = css({
    color: fontColor[props.surrounds.mineCount - 1],
    fontWeight: '800',
  });

  //爆弾CSS
  const mineStyle = css({
    width: '24px',
    height: '24px',
    backgroundColor: '#777777',
    borderRadius: '50%',
    border: '1px solid #222222',
  });

  //フラグCSS
  const flagStyle = css({
    width: '24px',
    height: '24px',
    backgroundColor: 'yellow',
    borderRadius: '50%',
    border: '1px solid #ccc73d',
  });

  const dispatch = useDispatch();
  const state = useSelector((state: RootState) => state.game.grids[props.y][props.x]);
  // const size = useSelector((state: RootState) => state.game.size);
  // const mine = useSelector((state: RootState) => state.game.mine);
  // const open = useSelector((state: RootState) => state.game.open);
  // const gameState = useSelector((state: RootState) => state.game.state);

  //周囲の爆弾の数が0のマスとその周辺のマスをオープンにする
  if (props.type === GridType.none && state === GridState.open && props.surrounds.mineCount === 0) {
    for (const none of props.surrounds.nones) {
      dispatch(setGridState({ x: none.x, y: none.y, state: GridState.open }));
    }
  }
  // if (open >= (size ** 2 - mine) && gameState === GameState.gaming) {
  //   console.log("GameClear");
  //   dispatch(changeGameState(GameState.clear));
  // }

  //左クリック処理
  const leftClick = () => {
    //フラグがtっている時は押せない
    if (state !== GridState.flag) {
      dispatch(setGridState({ x: props.x, y: props.y, state: GridState.open }))
      if (props.type === GridType.mine) {
        //地雷をクリックしたらゲームオーバー
        console.log("GameOver");
        dispatch(changeGameState(GameState.gameOver));
      }
    }
  }

  //右クリック処理
  const rightClick = () => {
    //フラグのオンオフ
    if (state === GridState.flag) {
      dispatch(setGridState({ x: props.x, y: props.y, state: GridState.close }))
    } else {
      dispatch(setGridState({ x: props.x, y: props.y, state: GridState.flag }))
    }
  }

  //いずれかのマウスボタンが押されたら
  const handleMouseDown = (e: React.MouseEvent<HTMLDivElement, MouseEvent>) => {
    if (e.button === 0) {
      //左クリック
      leftClick();
    } else if (e.button === 2) {
      //右クリック
      //右クリックメニューをキャンセル
      e.currentTarget.oncontextmenu = () => false;
      rightClick();
    }
  }



  return (
    <Grid item className="Grid" onMouseDown={handleMouseDown}>
      {state === GridState.open ? (
        <Box css={openStyle}>
          <Box css={props.type === GridType.none ? noneStyle : mineStyle}>
            {props.type === GridType.none && props.surrounds.mineCount !== 0 && props.surrounds.mineCount}
          </Box>
        </Box>
      ) : (
        <Box css={closeStyle}>
          {state === GridState.flag && (
            <Box css={flagStyle}>
            </Box>
          )}
        </Box>
      )}
    </Grid>
  )
})

export default StageGrid