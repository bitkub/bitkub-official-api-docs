import hashlib
import hmac
import json
import time
import requests

def gen_sign(api_secret, payload_string=None):
    return hmac.new(api_secret.encode('utf-8'), payload_string.encode('utf-8'), hashlib.sha256).hexdigest()

def gen_query_param(url, query_param):
    req = requests.PreparedRequest()
    req.prepare_url(url, query_param)
    return req.url.replace(url,"")

if __name__ == '__main__':
    
    host = 'https://api.bitkub.com'
    path = '/api/v3/market/order-info'
    api_key = 'your API key'
    api_secret = 'your API SECRET'

    ts = str(round(time.time() * 1000))
    param = {
        'sym':"", # symbol in quote_base format: e.g. btc_thb
        'id': "", # order id
        "sd": "", # side buy or sell
        # "hash":"", # order hash (optional)
    }
    query_param = gen_query_param(host+path, param)

    payload = []
    payload.append(ts)
    payload.append('GET')
    payload.append(path)
    payload.append(query_param)

    sig = gen_sign(api_secret, ''.join(payload))
    headers = {
        'Accept': 'application/json',
        'Content-Type': 'application/json',
        'X-BTK-TIMESTAMP': ts,
        'X-BTK-SIGN': sig,
        'X-BTK-APIKEY': api_key
    }

    response = requests.request('GET', f'{host}{path}{query_param}', headers=headers, data={}, verify=False)
    print(response.text)
