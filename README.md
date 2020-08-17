# bitkub-official-api-docs
Official Documentation for Bitkub APIs

* The documentation described here is official. This means the documentation is officially supported and maintained by Bitkub's own development team.
* The use of any other projects is **not supported**; please make sure you are visiting **bitkub/bitkub-official-api-docs**.


Name | Description
------------ | ------------ 
[restful-api.md](./restful-api.md) | Details on the RESTful API (/api)
[websocket-api.md](./websocket-api.md) | Details on the Websocket API (/websocket-api)

<br />

# Run docker for sample call RESTful API 

* change your environment in .env file
```
API_HOST=https://api.bitkub.com
API_KEY=YOUR API KEY
API_SECRET=YOUR API SECRET
```
* run this command in terminal
```
docker build -t samples:test .
```
```
docker run samples:test
```
* follow your result

