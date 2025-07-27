import { FirebaseOptions, getApps, getApp, initializeApp, FirebaseApp } from 'firebase/app';
import { getAuth } from 'firebase/auth';
import { Firestore, getFirestore, getDocs, collection, QueryDocumentSnapshot } from 'firebase/firestore';

// const firebaseConfig: FirebaseOptions = {/* config */ };
// const firebase: FirebaseApp = !getApps().length ? initializeApp(firebaseConfig) : getApp();
// export default firebase;

const firebaseConfig = {
  apiKey: "AIzaSyDciVQ_iJf5I7BlxiGDmx5iq8sWVYVBI2Q",
  authDomain: "chat-app-a4351.firebaseapp.com",
  databaseURL: "https://chat-app-a4351-default-rtdb.asia-southeast1.firebasedatabase.app",
  projectId: "chat-app-a4351",
  storageBucket: "chat-app-a4351.appspot.com",
  messagingSenderId: "404135976075",
  appId: "1:404135976075:web:96613e6d8efe191801888e",
  measurementId: "G-8HKY7L05TT"
};

//v8から存在するエラー「Firebase: Firebase App named '[DEFAULT]' already exists」対策としてgetApp().Lengthで条件分岐する
const firebaseApp = !getApps().length ? initializeApp(firebaseConfig) : getApp();
// const firebaseApp = initializeApp(firebaseConfig);

const db = getFirestore(firebaseApp);

const auth = getAuth();

export { db, auth };