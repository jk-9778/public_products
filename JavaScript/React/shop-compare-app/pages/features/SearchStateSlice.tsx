import { createSlice } from "@reduxjs/toolkit";

export const Shops = {
  Amazon: "Amazon",
  Rakuten: "Rakuten",
  Yahoo: "Yahoo",
}

export const searchStateSlice = createSlice({
  name: "searchState",
  initialState: {
    keyword: "",
    shops: [Shops.Amazon, Shops.Rakuten, Shops.Yahoo],
  },
  reducers: {
    setSearchKeyword: (state, action) => {
      state.keyword = action.payload;
    },
    setSearchShops: (state, action) => {
      state.shops = action.payload;
    },
  },
});

export const { setSearchKeyword, setSearchShops } = searchStateSlice.actions;

export default searchStateSlice.reducer;