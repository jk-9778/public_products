import { useRouter } from "next/router";
import styles from "../../styles/More.module.css";
import id from "../../public/appid.json";
import { useSelector } from "react-redux";
import { RootState } from "../features/ReduxStore";
import { Shops } from "../features/SearchStateSlice";
import RakutenItem from "./RakutenItem";
import YahooItem from "./YahooItem";
import itemStyles from '../../styles/SearchResult.module.css'
import HeaderContent from "./HeaderContent";
import React from "react";

type itemType = {
  id: string,
  market: string,
  name: string,
  originURL: string,
  price: string,
  description: string,
  images: string[],
  reviewCount: string,
  reviewRate: string,
  shopName: string,
  shopURL: string,
}

//サーバサイドレンダリング
export async function getServerSideProps({ params, query }: any) {
  //商品名をスペースで区切って検索ワードにする
  const words = query.name.split(" ");
  //検索ワードの単語数
  const len = 3;
  let keyword = "";
  if (words.length >= len) {
    for (let i = 0; i < len; i++) {
      //指定した単語数までをつなげる
      keyword += words[i];
      if (i < 2) keyword += " ";
    }
  } else {
    for (let i = 0; i < words.length; i++) {
      //指定した単語数ない場合は、ある分をつなげる
      keyword += words[i];
      if (i < words.length - 1) keyword += " ";
    }
  }

  //楽天API
  const rakutenID = id.rakuten;
  const rakutenURL = `https://app.rakuten.co.jp/services/api/IchibaItem/Search/20170706?format=json&keyword=${keyword}&applicationId=${rakutenID}`;
  const rakutenReq = await fetch(rakutenURL);
  const rakutenData = await rakutenReq.json();
  const rakutenItem = rakutenData.Items;
  //YhooAPI
  const yahooID = id.yahoo;
  const yahooURL = `https://shopping.yahooapis.jp/ShoppingWebService/V3/itemSearch?appid=${yahooID}&query=${keyword}&results=30`;
  const yahooReq = await fetch(yahooURL);
  const yahooData = await yahooReq.json();
  const yahooItem = yahooData.hits;

  const data = { rakutenItem, yahooItem };
  return {
    //↓↓↓ここはpropsと決まっているらしい
    props: {
      market: query.market,
      keyword: keyword,
      items: data,
    },
  };
}

const Product = (props: any) => {
  const router = useRouter();
  const item = router.query;
  const shops = useSelector((state: RootState) => state.search.shops);

  const handleItemPageClick = () => {
    const url = Array.isArray(item.originURL) ? "" : item.originURL;
    window.open(url, '_blank');
  }

  return (
    <div className={styles.container}>
      <HeaderContent />
      <div className={styles.content}>
        <div className={styles.market}>{item.market}</div>
        <div className={styles.name}>{item.name}</div>
        <div className={styles.images}>
          {/* Next.jsの使用？で渡す配列の長さが１だと配列から取り出されてしまうため、配列であるかの判定が必要 */}
          {Array.isArray(item.images) ?
            item.images!.map((img: string, i: number) => (
              <img key={i} src={img} alt="image" />
            )) :
            (<img src={item.images} alt="image" />)
          }
        </div>
        <div className={styles.price}>￥{item.price}</div>
        <div className={styles.review}>レビュー数：{item.reviewCount}</div>
        <div className={styles.review}>スコア：{item.reviewRate}</div>
        <div className={styles.itemPageButton} onClick={handleItemPageClick}>商品ページ</div>
        <div className={styles.favoriteButton}>お気に入りに追加</div>
        <div className={styles.description}>{item.description}</div>
        <div className={styles.otherSearchs}>
          <div>その他の検索結果</div>
          <div>検索ワード：{props.keyword}</div>
          <div className={itemStyles.container}>
            {shops.map((shop, i) => (
              <div className={itemStyles.shop} key={i}>
                <h2>{shop}</h2>
                <div>
                  {shop === Shops.Amazon && (
                    <div>検索不可</div>
                  )}
                  {shop === Shops.Rakuten &&
                    props.items.rakutenItem.map((item: any[any], i: number) => (
                      <RakutenItem key={i} item={item} />
                    ))}
                  {shop === Shops.Yahoo &&
                    props.items.yahooItem.map((item: any[any], i: number) => (
                      <YahooItem key={i} item={item} />
                    ))}
                </div>
              </div>
            ))}
          </div>
        </div>
      </div>
    </div>
  );
}

export default Product;