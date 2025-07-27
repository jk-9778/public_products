import { createSlice } from "@reduxjs/toolkit";
import { GridState } from "../components/manager/GameManager";

export const GameState = {
  clear: 0,
  gameOver: 1,
  gaming: 2,
}

export const GameStateSlice = createSlice({
  name: "GameState",
  initialState: {
    grids: [[0]],
    size: 8,
    mine: 10,
    open: 0,
    state: GameState.gaming,
    reBuild: true,
  },
  reducers: {
    setGrids: (state, action) => {
      state.grids = action.payload;
    },
    setGridState: (state, action) => {
      if (state.grids[action.payload.y][action.payload.x] !== GridState.open) {
        state.grids[action.payload.y][action.payload.x] = action.payload.state;
        if (action.payload.state === GridState.open) {
          state.open++;
        }
      }
    },
    setStageSize: (state, action) => {
      state.size = action.payload;
    },
    setMineCount: (state, action) => {
      state.mine = action.payload;
    },
    changeGameState: (state, action) => {
      state.state = action.payload;
    },
    setRetry: (state) => {
      state.state = GameState.gaming;
      state.open = 0;
      state.reBuild = state.reBuild ? false : true;
    },
  },
});

export const { setGrids, setGridState, setStageSize, setMineCount, changeGameState, setRetry } = GameStateSlice.actions;

export default GameStateSlice.reducer;