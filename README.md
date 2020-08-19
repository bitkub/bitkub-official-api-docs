# bitkub-official-api-docs
Official Documentation for Bitkub APIs

* The documentation described here is official. This means the documentation is officially supported and maintained by Bitkub's own development team.
* The use of any other projects is **not supported**; please make sure you are visiting **bitkub/bitkub-official-api-docs**.


Name | Description
------------ | ------------ 
[restful-api.md](./restful-api.md) | Details on the RESTful API (/api)
[websocket-api.md](./websocket-api.md) | Details on the Websocket API (/websocket-api)

<br />

# docker for sample call API 

* change your environment in .env file https://www.bitkub.com/publicapi
```
API_HOST=https://api.bitkub.com
API_KEY=YOUR API KEY
API_SECRET=YOUR API SECRET

WS_HOST=wss://api.bitkub.com/websocket-api
STREAM_NAME=market.trade.thb_btc
```
* run this command in terminal
```
docker build -t samples:test .
```
```
docker run samples:test
```
* follow your result

