import { NextPage } from 'next';
import styles from '../../styles/Search.module.css'
import SearchBox from './SearchBox';
import SearchResult from './SearchResult';

const Search: NextPage = () => {
  return (
    <div className={styles.container}>
      <SearchBox />
      <SearchResult />
    </div>
  );
}

export default Search;