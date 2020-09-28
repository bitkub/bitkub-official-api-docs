import axios from "axios";
import { createHmac } from "crypto";
import { config } from "dotenv";

config({ path: "../../.env" });

const samplegetkey = async () => {
  // get server time
  const { data: servertime } = await axios.get(
    `${process.env.API_HOST}/api/servertime`
  );
  const body = {
    ts: servertime,
    sym: "THB_BTC",
    amt: 0.000088,
    rat: 0,
    typ: "market",
  };
  const str = JSON.stringify(body);
  console.log(" parseInt,", parseFloat("0.000001"));
  // generate sig with servertime
  const hash = createHmac("sha256", process.env.API_SECRET)
    .update(str)
    .digest("hex");

  console.log("hash", hash);

  // get ws token
  const { data: wstoken } = await axios({
    method: "post",
    url: `${process.env.API_HOST}${process.env.API_ENDPOINT}`,
    headers: {
      Accept: "application/json",
      "Content-Type": "application/json",
      "X-BTK-APIKEY": process.env.API_KEY,
    },
    data: {
      sig: hash,
      ...body,
    },
  });
  console.log("get wstoken", wstoken);
  return wstoken;
};

(async () => {
  try {
    await samplegetkey();
  } catch (e) {
    console.log(e.response.data);
  }
})();

export default samplegetkey;
