<?php
$key = ''; //Fill in your key
$secret_key = ''; //Fill in your secret key
$API_HOST = 'https://api.bitkub.com';
// Step 1 : Request server time API
$ch1 = curl_init();
curl_setopt($ch1, CURLOPT_URL, "$API_HOST/api/servertime");
curl_setopt($ch1, CURLOPT_RETURNTRANSFER, TRUE);
$responsetime = curl_exec($ch1);
$jsonArrayResponse = json_decode($responsetime);
curl_close($ch1);
$data =json_encode(array(
    'sym' => 'THB_ADA',
    'ts' => $jsonArrayResponse
),JSON_NUMERIC_CHECK);
// Step 2 : Hash Secret key with share256 with data
$signature = hash_hmac('sha256', $data, $secret_key);
echo (Int)$jsonArrayResponse;
//Prepare array data for POST with your cryptocurrency name
$postRequest =json_encode(array(
    "sig" => $signature,
    'sym' => 'THB_ADA', //change your cryptocurrency
    'ts' => $jsonArrayResponse
),JSON_NUMERIC_CHECK);

var_dump($postRequest);

// Step 3 : Request API my-order-history with postRequest
$ch = curl_init();
curl_setopt($ch, CURLOPT_URL, "$API_HOST/api/market/my-order-history");
curl_setopt($ch, CURLOPT_RETURNTRANSFER, TRUE);
curl_setopt($ch, CURLOPT_HEADER, FALSE);
curl_setopt($ch, CURLOPT_POST, TRUE);
curl_setopt($ch, CURLOPT_POSTFIELDS, $postRequest);
curl_setopt($ch, CURLOPT_HTTPHEADER, array("Content-Type: application/json","Accept: application/json","X-BTK-APIKEY: ".$key));
$response = curl_exec($ch);
curl_close($ch);

// Here is result
echo $response;

?>
