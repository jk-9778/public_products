import { Button } from '@mui/material';
import React from 'react';
import { auth } from "../firebase.js";
import CallIcon from "@mui/icons-material/Call";

function SignOut() {
  return (
    <div className='header'>
      <Button style={{ color: "white", fontSize: "15px" }} onClick={() => auth.signOut()}>
        サインアウト
      </Button>
      {/* 現在ログインしているユーザー名を取得 */}
      <h3>{auth.currentUser.displayName}</h3>
      {/* 電話アイコン追加 */}
      <CallIcon />
    </div>
  )
}

export default SignOut