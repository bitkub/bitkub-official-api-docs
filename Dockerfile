FROM node:14.0.0

COPY . .
RUN npm install --prefix samples/node

CMD ["npm", "start", "--prefix", "samples/node"]