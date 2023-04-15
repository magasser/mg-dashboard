FROM node:19-alpine as build

WORKDIR /home/build

COPY ./package.json /home/build/

RUN npm install

COPY ./ /home/build/

RUN npm run build-prod

FROM nginx:stable-alpine

COPY ./nginx/nginx.conf /etc/nginx/nginx.conf

COPY --from=build /home/build/dist/mg-dashboard /usr/share/nginx/html

EXPOSE 80