import './App.css';
import Form from './cpmponents/Form';
import Results from './cpmponents/Results';
import Title from './cpmponents/Title';
import React, { useState } from "react"
import Loading from './cpmponents/loading';

//Resultの型を指定
type ResultsStateType = {
  country: string,
  cityName: string,
  temperature: string,
  conditionText: string,
  icon: string
}

function App() {
  const [loading, setLoading] = useState<boolean>(false);
  const [city, setCity] = useState<string>("");
  const [results, setResults] = useState<ResultsStateType>({
    country: "",
    cityName: "",
    temperature: "",
    conditionText: "",
    icon: ""
  });

  //WeatherAPI
  const url = `https://api.weatherapi.com/v1/current.json?key=906bd1a8505645e1a7345339221306&q=${city}&aqi=no`;
  //API実行
  const getWeather = (e: React.FormEvent<HTMLFormElement>) => {
    //画面の更新をキャンセル
    e.preventDefault();
    setLoading(true);

    //指導黄処理にてAPIを実行し、データを取得する
    fetch(url)
      //フェッチ成功処理
      .then((res) => {
        return res.json();
      })
      .then((data) => {
        setResults({
          country: data.location.country,
          cityName: data.location.name,
          temperature: data.current.temp_c,
          conditionText: data.current.condition.text,
          icon: data.current.condition.icon
        });
        setCity("");
        setLoading(false);
      })
      //フェッチ失敗処理
      .catch((err) => {
        alert("エラーが発生しました。ページをリロードしてください")
      });
  }

  return (
    <div className='wrapper'>
      <div className='container'>
        <Title />
        <Form city={city} setCity={setCity} getWeather={getWeather} />
        {loading ? <Loading /> : <Results results={results} />}
      </div>
    </div>
  );
}

export default App;
