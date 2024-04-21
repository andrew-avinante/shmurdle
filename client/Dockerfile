FROM node:14-alpine

# Create app directory
WORKDIR /usr/src/app

# Install app dependencies
# A wildcard is used to ensure both package.json AND package-lock.json are copied
# where available (npm@5+)
COPY package*.json ./

# Download the build artifact
COPY ./public ./public
COPY ./src ./src
COPY ./*.js ./

RUN npm install
RUN CI=false npm run build

# Serve the built app with Nginx
FROM nginx:1.19-alpine
COPY --from=0 /usr/src/app/dist /usr/share/nginx/html
RUN rm /etc/nginx/conf.d/default.conf
COPY nginx/nginx.conf /etc/nginx/conf.d
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
