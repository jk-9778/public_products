import Link from "next/link";
import { Shops } from "../features/SearchStateSlice";
import styles from "../../styles/Item.module.css";

const RakutenItem = (props: { item: { Item: any } }) => {
  return (
    <Link href={{
      pathname: "/products/" + (props.item.Item.itemName).replaceAll("/", "・").replaceAll(".", "・"), query: {
        market: Shops.Rakuten,
        name: props.item.Item.itemName,
        originURL: props.item.Item.itemUrl,
        price: props.item.Item.itemPrice,
        description: props.item.Item.itemCaption,
        images: props.item.Item.mediumImageUrls.map((image: { imageUrl: string }) => image.imageUrl),
        reviewCount: props.item.Item.reviewCount,
        reviewRate: props.item.Item.reviewAverage,
        shopName: props.item.Item.shopName,
        shopURL: props.item.Item.shopUrl,
      }
    }}>
      <a className={styles.item}>
        <div>{props.item.Item.itemName}</div>
        <div className={styles.price}>￥{props.item.Item.itemPrice}</div>
        <div className={styles.mediumImages}>
          {props.item.Item.mediumImageUrls.map((img: { imageUrl: string }, i: number) => (
            <img key={i} src={img.imageUrl} alt="mediumImage" />
          ))}
        </div>
      </a>
    </Link>
  );
}

export default RakutenItem;