/** @jsxImportSource @emotion/react */
import { Box, css } from '@mui/material'
import React from 'react'
import { useSelector } from 'react-redux'
import { GameState } from '../../../features/GameStateSlice'
import { RootState } from '../../../features/ReduxStore'
import GameClear from './GameClear'
import GameOver from './GameOver'

const GameResult = () => {
  const resultStyle = css({
    position: 'fixed',
    top: '0',
    left: '0',
    width: '100%',
    height: '100%',
  });
  const grayLayer = css({
    position: 'fixed',
    top: '0',
    left: '0',
    width: '100%',
    height: '100%',
    backgroundColor: 'gray',
    opacity: '0.5',
    zIndex:'-1',
  });

  const state = useSelector((state: RootState) => state.game.state);

  return (
    <Box css={resultStyle}>
      <Box css={grayLayer}></Box>
      {state === GameState.clear ? (<GameClear />) : (<GameOver />)}
    </Box>
  )
}

export default GameResult