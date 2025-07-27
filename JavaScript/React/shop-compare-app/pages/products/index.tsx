import { NextPage } from "next";
import { RootState } from "../features/ReduxStore";
import Search from "./Search";
import styles from "../../styles/index.module.css";

const Index: NextPage = () => {
  return (
    <div className={styles.container}>
      <Search />
    </div>
  );
}

export default Index;