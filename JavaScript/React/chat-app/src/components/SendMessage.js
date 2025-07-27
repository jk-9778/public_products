import React, { useState } from 'react';
import { db, auth } from "../firebase.js";
import { collection, getDocs, where, query, addDoc, serverTimestamp } from 'firebase/firestore';
import { Button, Input } from '@mui/material';
import SendIcon from "@mui/icons-material/Send";

function SendMessage() {
  const [message, setMessage] = useState("");

  //フォームが送信された際の処理
  function sendMessage(e) {
    //入力後に再ロードされないようにする
    e.preventDefault();

    const { uid, photoURL } = auth.currentUser;

    const colRef = collection(db, "messages");
    const data = {
      text: message,
      photoURL,
      uid,
      createdAt: serverTimestamp()
    };

    addDoc(colRef, data);
    setMessage("");
  }
  return (
    <div>
      <form onSubmit={sendMessage}>
        <div className='sendMsg'>
          <Input
            style={{
              width: "78%",
              fontSize: "15px",
              fontWeight: "550",
              marginLeft: "5px",
              marginBottom: "-3px",
            }}
            placeholder='メッセージを入力してください'
            type="text"
            // 入力された内容をmessage変数に格納
            onChange={(e) => setMessage(e.target.value)}
            value={message}
          />
          <SendIcon onClick={sendMessage} style={{ color: "#7ac2ff", marginLeft: "20px" }} />
        </div>
      </form>
    </div>
  )
}

export default SendMessage