const http = require("http");
require("dotenv").config();

const host = process.env.HOST;
const port = process.env.PORT;

const server = http.createServer((req, res) => {
  res.statusCode = 200;
  res.setHeader("Content-Type", "text/plain");
  res.end("api.mg-dashboard");
});

server.listen(port, host, () => {
  console.log(`MG-Dashboard API running at http://${host}:${port}.`);
});
