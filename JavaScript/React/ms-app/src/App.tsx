/** @jsxImportSource @emotion/react */
import { css } from '@emotion/react';
import { useSelector } from 'react-redux';
import GameScene from './components/game/GameScene';
import MenuScene from './components/menu/MenuScene';
import { RootState } from './features/ReduxStore';
import { SceneState } from './features/SceneStateSlice';

function App() {
  const appStyle = css({
    width: '100%',
    height: '100vh',
    // backgroundColor: '#dbf3ff',
  });

  const scene = useSelector((state: RootState) => state.scene.value);
  console.log(scene);

  switch (scene) {
    case SceneState.Menu: return (<div className="App" css={appStyle}><MenuScene /></div>);
    case SceneState.Game: return (<div className="App" css={appStyle}><GameScene /></div>);
    default: return (<div>シーンを読み込めません</div>);
  }
}

export default App;
