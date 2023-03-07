const express = require('express')
const mongoose = require('mongoose')
const config = require('./config/config')

const postRouter = require('./routes/postRoutes')

//init
const app = express()
const port = process.env.PORT || 3000

//connect
const mongoUrl = `mongodb://${config.MONGO_USER}:${config.MONGO_PASSWORD}@${config.MONGO_IP}:${config.MONGO_PORT}?authSource=admin`
console.log(mongoUrl)
mongoose
  .connect(mongoUrl)
  .then(() => console.log('connected to db'))
  .catch((e) => console.log(e))

//middleware
app.use(express.json())

//routes
app.get('/', (req, res) => {
  res.send('<h1>Hello there3!!!</h1>')
})
app.use('/api/v1/posts', postRouter)

app.listen(port, () => console.log(`Listening on port ${port}`))
