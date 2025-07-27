import React, { useEffect, useState } from 'react';
import SignOut from './SignOut';
import { auth, db } from "../firebase.js";
import { collection, getDocs, where, query, orderBy, limit, onSnapshot } from 'firebase/firestore';
import { async } from '@firebase/util';
import SendMessage from './SendMessage';

function Line() {
  const [messages, setMessages] = useState([]);
  useEffect(() => {
    // (async () => {
    //を参照するためのクエリ。WHEREなどもインポートすればある
    const q = query(collection(db, "messages"), orderBy("createdAt"), limit(50));
    onSnapshot(q, (querySnapshot) => {
      // setMessages(querySnapshot.docs.map((doc) =>
      //   doc.data()
      // ));
      setMessages(querySnapshot.docs.map((doc) => {
        // doc.data();
        const obj = doc.data();
        obj.id = doc.id;
        return obj
      }
      ));
    });
    // })();

  }, []); //第２引数に空の配列を渡すことでマウント時の1回だけ実行する

  return (
    <div>
      <SignOut />
      {/* messagesに格納したデータをmap関数で展開して表示する */}
      <div className='msgs'>
        {messages.map(({ id, text, photoURL, uid }) => (
          <div key={id}>
            <div
              //データのユーザーIDがログイン中のものかそうでないかでクラスタグを分ける
              className={`msg ${uid === auth.currentUser.uid ? "sent" : "received"}`}>
              <img src={photoURL} alt="" />
              <p>{text}</p>
            </div>
          </div>
        ))}
      </div>
      <SendMessage />
    </div>
  )
}

export default Line