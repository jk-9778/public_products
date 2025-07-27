/** @jsxImportSource @emotion/react */
import { Box, Button, css } from '@mui/material'
import React from 'react'
import { useDispatch } from 'react-redux';
import { setMineCount, setRetry } from '../../../features/GameStateSlice';
import { changeScene, SceneState } from '../../../features/SceneStateSlice';
import { GameParameter } from '../../manager/GameManager';

const GameRetry = (props: {
  message: string,
}) => {
  const retryStyle = css({
    width: '50%',
    maxWidth: '300px',
    display: 'flex',
    flexDirection: 'column',
    border: '2px solid #777777',
    borderRadius: '10px',
    justifyContent: 'center',
    alignItems: 'center',
    padding: '30px',
    backgroundColor: 'azure',
  });
  const buutonContainerStyle = css({
    width: '100%',
    display: 'flex',
    flexDirection: 'column',
  });
  const buttonStyle = css({
    marginTop: '10px',
    backgroundColor: '#4176e0',
    color: 'white',
    ":hover": {
      backgroundColor: '#3b53cc',
    }
  });

  const dispatch = useDispatch();

  const handleRetryClick = (e: React.MouseEvent<HTMLButtonElement, MouseEvent>): void => {
    dispatch(setRetry());
  }

  const handleConfigClick = (e: React.MouseEvent<HTMLButtonElement, MouseEvent>): void => {
    dispatch(setRetry());
    dispatch(setMineCount(GameParameter.initMine));
    dispatch(changeScene(SceneState.Menu));
  }

  return (
    <Box css={retryStyle}>
      <h1>{props.message}</h1>
      <Box css={buutonContainerStyle}>
        <Button css={buttonStyle} onClick={handleRetryClick}>もう一度プレイ</Button>
        <Button css={buttonStyle} onClick={handleConfigClick}>ステージ設定</Button>
      </Box>
    </Box>
  )
}

export default GameRetry