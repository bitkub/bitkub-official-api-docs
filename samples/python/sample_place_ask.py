import hashlib
import hmac
import json
import os
import requests

from dotenv import load_dotenv
load_dotenv('../../.env')

# API info
API_HOST = os.environ['API_HOST']
API_KEY = "aa17635ee581cac261da91ee4a5792cf"
API_SECRET = bytes("d61ca04669ea1eed94ab7f7a9bf1d5cb", encoding='utf-8')

def json_encode(data):
	return json.dumps(data, separators=(',', ':'), sort_keys=True)

def sign(data):
	j = json_encode(data)
	print('Signing payload: ' + j)
	h = hmac.new(API_SECRET, msg=j.encode(), digestmod=hashlib.sha256)
	return h.hexdigest()

# check server time
response = requests.get(API_HOST + '/api/servertime')
ts = int(response.text)
print('Server time: ' + response.text)

# place ask
header = {
	'Accept': 'application/json',
	'Content-Type': 'application/json',
	'X-BTK-APIKEY': API_KEY,
}
data = {
	'sym': 'THB_BTC',
	'amt': 0.01, # BTC amount you want to sell
	'rat': 260000,
	'typ': 'limit',
	'ts': ts,
}
signature = sign(data)
data['sig'] = signature

print('Payload with signature: ' + json_encode(data))
response = requests.post(API_HOST + '/api/market/place-ask', headers=header, data=json_encode(data))

print('Response: ' + response.text)