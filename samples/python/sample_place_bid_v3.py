import hashlib
import hmac
import json
import time
import requests

def gen_sign(api_secret, payload_string=None):
    return hmac.new(api_secret.encode('utf-8'), payload_string.encode('utf-8'), hashlib.sha256).hexdigest()

if __name__ == '__main__':
    host = 'https://api.bitkub.com'
    path = '/api/v3/market/place-bid'
    api_key = 'your API key'
    api_secret = 'your API SECRET'

    ts = str(round(time.time() * 1000))
    reqBody = {
        'sym': 'btc_thb', # {quote}_{base}
        'amt': 10,
        'rat': 10,
        'typ': 'limit' # limit, market
    }
    payload = []
    payload.append(ts)
    payload.append('POST')
    payload.append(path)
    payload.append(json.dumps(reqBody))

    sig = gen_sign(api_secret, ''.join(payload))
    headers = {
        'Accept': 'application/json',
        'Content-Type': 'application/json',
        'X-BTK-TIMESTAMP': ts,
        'X-BTK-SIGN': sig,
        'X-BTK-APIKEY': api_key
    }

    response = requests.request('POST', host + path, headers=headers, data=json.dumps(reqBody), verify=False)
    print(response.text)