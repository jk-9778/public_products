import { configureStore } from "@reduxjs/toolkit";
import SceneStateReducer from "./SceneStateSlice";
import SearchStateReducer from "./SearchStateSlice";

//ストア作成
export const store = configureStore({
  reducer: {
    scene: SceneStateReducer,
    search: SearchStateReducer,
  },
});

export type RootState = ReturnType<typeof store.getState>;
