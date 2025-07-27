import { configureStore } from "@reduxjs/toolkit";
import SceneStateReducer from "./SceneStateSlice";
import GameStateReducer from "./GameStateSlice";

//ストア作成
export const store = configureStore({
  reducer: {
    scene: SceneStateReducer,
    game: GameStateReducer,
  },
});

export type RootState = ReturnType<typeof store.getState>;
