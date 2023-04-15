const loadEnv = () => {
  const fs = require("fs");
  const writeFile = fs.writeFile;
  const targetPath = "./src/environments/environment.ts";
  const appVersion = require("../../package.json").version;
  const env = process.argv[2];

  require("dotenv").config({
    path: `src/environments/.env.${env}`,
  });

  console.log(`Generating 'environment.ts' file for environment '${env}'.`);

  const envConfigFile = `export const environment = {
    apiUrl: '${process.env.API_URL}',
    mqttConnectionUri: '${process.env.MQTT_CONNECTION_URI}',
    appVersion: '${appVersion}'
  };
  `;
  console.log(
    `The file 'environment.ts' will be written with the following content: \n${envConfigFile}`
  );
  writeFile(targetPath, envConfigFile, (err) => {
    if (err) {
      console.error(err);
      throw err;
    } else {
      console.log(
        `Angular 'environment.ts' file generated correctly at ${targetPath} \n`
      );
    }
  });
};

loadEnv();
