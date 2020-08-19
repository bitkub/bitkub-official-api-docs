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
  console.log(data);
});
