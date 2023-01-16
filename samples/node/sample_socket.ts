import * as WebSocket from "ws";
import { config } from "dotenv";

config({ path: "../../.env" });

const ws = new WebSocket(`${process.env.WS_HOST}/${process.env.STREAM_NAME}`);

ws.on("open", function open() {
  console.log("connected");
});

ws.on("close", function close() {
  console.log("disconnected");
});

ws.on("message", function incoming(data: any) {
  // test your logic here
  const datatest = [
    {
      data: [[37819.78, 328867.69, 0.115, 0, true, false]],
      event: "askschanged",
      pairing_id: 1,
    },
    {
      data: [[37819.78, 328867.69, 0.115, 0, true, false]],
      event: "askschanged",
      pairing_id: 1,
    },
  ];
  const str = JSON.stringify({
    data: [[37819.78, 328867.69, 0.115, 0, true, false]],
    event: "askschanged",
    pairing_id: 1,
  });
  const strTostr = str + "\n" + str;
  const restr = "[" + strTostr.replace(/\n/g, ",") + "]";
  console.log("str", restr);
  const json = JSON.parse(restr);
  console.log("json >>>>>", json);
  // console.log(data);
});
