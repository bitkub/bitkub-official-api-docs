import hashlib
import hmac
import json
import os
import requests
from requests.adapters import HTTPAdapter
from requests.packages.urllib3.util.retry import Retry
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
while True:
    session = requests.Session()
    retry = Retry(connect=3, backoff_factor=0.5)
    adapter = HTTPAdapter(max_retries=retry)
    session.mount('http://', adapter)
    session.mount('https://', adapter)
    response = session.get(API_HOST + '/api/servertime')
    print('Server time: ' + response.text)
# ts = int(response.text)


# # check balances
# header = {
# 	'Accept': 'application/json',
# 	'Content-Type': 'application/json',
# 	'X-BTK-APIKEY': API_KEY,
# }
# data = {
# 	'ts': ts,
# }
# signature = sign(data)
# data['sig'] = signature

# print('Payload with signature: ' + json_encode(data))
# response = requests.post(API_HOST + '/api/market/balances', headers=header, data=json_encode(data))

# print('Balances: ' + response.text)
