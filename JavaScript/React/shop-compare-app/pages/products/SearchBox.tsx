import styles from '../../styles/SearchBox.module.css'
import { useDispatch } from 'react-redux';
import { setSearchKeyword } from '../features/SearchStateSlice';

const SearchBox = () => {
  const dispatch = useDispatch();
  let keyword = "";
  const handleChange = (e: React.ChangeEvent<HTMLInputElement>): void => {
    keyword = e.target.value;
  }

  const handleSubmit = (e: React.FormEvent<HTMLFormElement>): void => {
    e.preventDefault();
    dispatch(setSearchKeyword(keyword));
  }

  return (
    <form className={styles.container} onSubmit={handleSubmit}>
      <input className={styles.searchBox} type="text" placeholder='キーワード検索' onChange={handleChange} />
      <button className={styles.searchButton}>検索</button>
    </form>
  );
}

export default SearchBox;