/** @jsxImportSource @emotion/react */
import { Box, css } from '@mui/material'
import React from 'react'
import GameRetry from './GameRetry'

const GameClear = () => {
  const gameClearStyle = css({
    width: '100%',
    display: 'flex',
    justifyContent: 'center',
    marginTop: '5%',
  });
  return (
    <Box css={gameClearStyle}>
      <GameRetry message='ゲームクリア' />
    </Box>
  )
}

export default GameClear