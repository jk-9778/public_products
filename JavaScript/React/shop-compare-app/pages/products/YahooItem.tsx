import Link from "next/link";
import { Shops } from "../features/SearchStateSlice";
import styles from "../../styles/Item.module.css";

const YahooItem = (props: { item: any }) => {
  return (
    <Link href={{
      pathname: "/products/" + (props.item.name).replaceAll("/", "・").replaceAll(".", "・"), query: {
        market: Shops.Yahoo,
        name: props.item.name,
        originURL: props.item.review.url,
        price: props.item.price,
        description: props.item.description,
        images: [props.item.image.medium],
        reviewCount: props.item.review.count,
        reviewRate: props.item.review.rate,
        shopName: props.item.seller.name,
        shopURL: props.item.seller.url,
      }
    }}>
      <a className={styles.item}>
        <div>{props.item.name}</div>
        <div className={styles.price}>{props.item.price}</div>
        <div className={styles.mediumImages}>
          <img src={props.item.image.medium} alt="mediumImage" />
        </div>
      </a>
    </Link>
  );
}

export default YahooItem;