/** @jsxImportSource @emotion/react */
import { Box, css } from '@mui/material'
import React from 'react'
import { useDispatch, useSelector } from 'react-redux'
import { changeGameState, GameState } from '../../features/GameStateSlice'
import { RootState } from '../../features/ReduxStore'
import GameResult from './result/GameResult'
import GameStage from './stage/GameStage'

const GameScene = () => {
  const gameStyle = css({
    width: '100%',
    height: '100vh',
    display: 'flex',
    flexDirection: 'column',
    justifyContent: 'center',
    alignItems: 'center',
  });

  const dispatch = useDispatch();
  const state = useSelector((state: RootState) => state.game);
  if (state.open >= (state.size ** 2 - state.mine) && state.state === GameState.gaming) {
    console.log("GameClear");
    dispatch(changeGameState(GameState.clear));
  }

  return (
    <Box sx={{ width: '100%', height: '100vh' }}>
      <Box css={gameStyle}>
        {/* <div>{state.size}</div>
        <div>{state.mine}</div>
        <div>{state.open}</div> */}
        <GameStage />
        {state.state !== GameState.gaming && (<GameResult />)}
      </Box>
    </Box>
  )
}

export default GameScene