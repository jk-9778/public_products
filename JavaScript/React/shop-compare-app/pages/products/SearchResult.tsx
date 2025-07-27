import styles from '../../styles/SearchResult.module.css'
import { useSelector } from 'react-redux';
import { RootState } from '../features/ReduxStore';
import { Shops } from '../features/SearchStateSlice';
import { useEffect, useState } from 'react';
import id from "../../public/appid.json";
import Link from 'next/link';
import RakutenItem from './RakutenItem';
import YahooItem from './YahooItem';

const SearchResult = () => {
  const keyword = useSelector((state: RootState) => state.search.keyword);
  const shops = useSelector((state: RootState) => state.search.shops);
  const [rakuten, setRakuten] = useState([]);
  const [yahoo, setYahoo] = useState([]);

  useEffect(() => {
    if (keyword !== "") {
      //楽天API
      (async () => {
        const rakutenID = id.rakuten;
        const rakutenURL = `https://app.rakuten.co.jp/services/api/IchibaItem/Search/20170706?format=json&keyword=${keyword}&applicationId=${rakutenID}`;
        const rakutenReq = await fetch(rakutenURL);
        const rakutenData = await rakutenReq.json();
        setRakuten(rakutenData.Items);
        // console.log(rakutenData.Items);

        //YhooAPI
        const yahooID = id.yahoo;
        const yahooURL = `https://shopping.yahooapis.jp/ShoppingWebService/V3/itemSearch?appid=${yahooID}&query=${keyword}&results=30`;
        const yahooReq = await fetch(yahooURL);
        const yahooData = await yahooReq.json();
        setYahoo(yahooData.hits);
        // console.log(yahooData.hits);
      })();
    }
  }, [keyword]);

  const handleClick = (e: React.MouseEvent<HTMLDivElement, MouseEvent>) => {
    console.log("click");
  }

  return (
    <div className={styles.container}>
      {shops.map((shop, i) => (
        <div className={styles.shop} key={i}>
          <h2>{shop}</h2>
          <div>
            {shop === Shops.Amazon && (
              <div>検索不可</div>
            )}
            {shop === Shops.Rakuten &&
              rakuten.map((item: any[any], i: number) => (
                <RakutenItem key={i} item={item} />
              ))}
            {shop === Shops.Yahoo &&
              yahoo.map((item: any[any], i: number) => (
                <YahooItem key={i} item={item} />
              ))}
          </div>
        </div>
      ))}
    </div>
  );
}

export default SearchResult;