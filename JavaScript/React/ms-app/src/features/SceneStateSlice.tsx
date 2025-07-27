import { createSlice } from "@reduxjs/toolkit";

//シーン一覧
export const SceneState = {
  Game: "Game",
  Title: "Title",
  Result: "Result",
  Menu: "Menu",
}

export const sceneStateSlice = createSlice({
  name: "SceneState",
  initialState: {
    value: SceneState.Menu,
  },
  reducers: {
    changeScene: (state, action) => {
      state.value = action.payload;
    },
  },
});

export const { changeScene } = sceneStateSlice.actions;

export default sceneStateSlice.reducer;