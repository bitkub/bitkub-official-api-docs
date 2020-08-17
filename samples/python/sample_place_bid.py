import hashlib
import hmac
import json
import os
import requests

from dotenv import load_dotenv
load_dotenv('../../.env')

# API info
API_HOST = os.environ['API_HOST']
API_KEY = os.environ['API_KEY']
API_SECRET = bytes(os.environ['API_SECRET'], encoding='utf-8')

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

# place bid
header = {
	'Accept': 'application/json',
	'Content-Type': 'application/json',
	'X-BTK-APIKEY': API_KEY,
}
data = {
	'sym': 'THB_BTC',
	'amt': 10, # THB amount you want to spend
	'rat': 260000,
	'typ': 'limit',
	'ts': ts,
}
signature = sign(data)
data['sig'] = signature

print('Payload with signature: ' + json_encode(data))
response = requests.post(API_HOST + '/api/market/place-bid', headers=header, data=json_encode(data))

print('Response: ' + response.text)
