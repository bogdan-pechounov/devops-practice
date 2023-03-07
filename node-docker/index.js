const express = require('express')
const mongoose = require('mongoose')
const config = require('./config/config')

const app = express()

const mongoUrl = `mongodb://${config.MONGO_USER}:${config.MONGO_PASSWORD}@${config.MONGO_IP}:${config.MONGO_PORT}?authSource=admin`
console.log(mongoUrl)
mongoose
  .connect(mongoUrl)
  .then(() => console.log('connected to db'))
  .catch((e) => console.log(e))

const port = process.env.PORT || 3000

app.get('/', (req, res) => {
  res.send('<h1>Hello there3!!!</h1>')
})

app.listen(port, () => console.log(`Listening on port ${port}`))
