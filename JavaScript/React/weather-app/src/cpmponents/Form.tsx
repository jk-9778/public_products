
//propsの型を指定
type FormPropsType = {
  city: string;
  setCity: React.Dispatch<React.SetStateAction<string>>;
  getWeather: (e: React.FormEvent<HTMLFormElement>) => void;
}

const Form = (props: FormPropsType) => {
  return (
    <div>
      <form onSubmit={props.getWeather}>
        <input
          type="text"
          name="city"
          placeholder="都市名"
          onChange={(e) => props.setCity(e.target.value)}
          value={props.city}
        />
        <button type='submit'>Get Weather</button>
      </form>
    </div>
  )
}

export default Form