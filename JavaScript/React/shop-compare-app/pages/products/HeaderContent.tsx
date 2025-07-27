import styles from '../../styles/HeaderContent.module.css'
import ToppageButton from './ToppageButton';

const HeaderContent = () => {
  return (
    <div className={styles.container}>
      <ToppageButton />
    </div>
  );
}

export default HeaderContent;