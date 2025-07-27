/** @jsxImportSource @emotion/react */
import { Box, Button, css, InputLabel, MenuItem, Select, SelectChangeEvent, TextField } from '@mui/material'
import { useState } from 'react';
import { useDispatch, useSelector } from 'react-redux'
import { setMineCount, setStageSize } from '../../features/GameStateSlice';
import { RootState } from '../../features/ReduxStore'
import { changeScene, SceneState } from '../../features/SceneStateSlice';

const MenuScene = () => {
  const menuSceneStyle = css({
    width: '100%',
    height: '100%',
    display: 'flex',
    justifyContent: 'center',
    alignItems: 'center',
  });
  const configStyle = css({
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
  const itemStyle = css({
    width: '100%',
    marginBottom: '20px',
  });
  const startStyle = css({
    marginTop: '10px',
    backgroundColor: '#4176e0',
    color: 'white',
    ":hover": {
      backgroundColor: '#3b53cc',
    }
  });

  const state = useSelector((state: RootState) => state.game);
  const dispatch = useDispatch();
  const handleSizwChange = (e: SelectChangeEvent<number>): void => {
    dispatch(setStageSize(e.target.value))
  }

  const [mine, setMine] = useState(state.mine.toString());
  const [err, setErr] = useState("");
  const regex = /^[0-9]*$/;
  const handleMineChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>): void => {
    setMine(e.target.value);
  }
  const handleStartClick = (e: React.MouseEvent<HTMLButtonElement, MouseEvent>): void => {
    if (!regex.test(mine)) {
      setErr("半角数字で入力してください");
    }
    else if (Number(mine) >= state.size ** 2) {
      setErr("地雷の数がステージサイズを超えています");
    } else {
      setErr("");
      dispatch(setMineCount(Number(mine)));
      dispatch(changeScene(SceneState.Game));
    }
  }

  return (
    <Box className='MenuScene' css={menuSceneStyle}>
      <Box css={configStyle}>
        <h2>ステージ設定</h2>
        <InputLabel id='size' sx={{ alignSelf: 'flex-start' }}>ステージサイズ</InputLabel>
        <Select labelId='size' css={itemStyle} value={state.size} onChange={handleSizwChange}>
          <MenuItem value={8}>8 × 8</MenuItem>
          <MenuItem value={16}>16 × 16</MenuItem>
          <MenuItem value={24}>24 × 24</MenuItem>
          <MenuItem value={32}>32 × 32</MenuItem>
          <MenuItem value={64}>64 × 64</MenuItem>
        </Select>
        <InputLabel id='mine' sx={{ alignSelf: 'flex-start' }}>地雷の数</InputLabel>
        <TextField css={itemStyle} placeholder='10' onChange={handleMineChange} />
        <Box sx={{ height: '1em', color: 'red' }}>{err}</Box>
        <Button css={startStyle} onClick={handleStartClick}>スタート</Button>
      </Box>
    </Box>
  )
}

export default MenuScene