# https://github.com/testcab/docker-cors-anywhere
ARG DOMAIN
FROM node:lts-alpine

WORKDIR /proxy

ARG DOMAIN
ENV NODE_ENV=production
ENV NODE_PATH=/usr/local/lib/node_modules
ENV CORSANYWHERE_WHITELIST=$DOMAIN
RUN npm install cors-anywhere
COPY server.js .
EXPOSE 8080

CMD ["node", "server.js"]
