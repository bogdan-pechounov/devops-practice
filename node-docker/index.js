const express = require('express')
const mongoose = require('mongoose')
const session = require('express-session')
const redis = require('redis')
const RedisStore = require('connect-redis').default
const config = require('./config/config')
const cors = require('cors')

const postRouter = require('./routes/postRoutes')
const userRouter = require('./routes/userRoutes')

//init
const app = express()
const port = process.env.PORT || 3000
// store
const redisClient = redis.createClient({
  url: 'redis://redis:6379',
  // host: config.REDIS_URL,
  // port: config.REDIS_PORT,
})
console.log(config.REDIS_URL)
redisClient
  .connect()
  .then(() => console.log('connected to redis'))
  .catch(console.error)
const redisStore = new RedisStore({ client: redisClient })

//connect
const mongoUrl = `mongodb://${config.MONGO_USER}:${config.MONGO_PASSWORD}@${config.MONGO_IP}:${config.MONGO_PORT}?authSource=admin`
console.log(mongoUrl)
mongoose
  .connect(mongoUrl)
  .then(() => console.log('connected to db'))
  .catch((e) => console.log(e))

//middleware
app.use(express.json())
app.enable('trust proxy')
app.use(cors())
app.use(
  session({
    store: redisStore,
    secret: config.SESSION_SECRET,
    cookie: {
      secure: false,
      resave: false,
      saveUninitialized: false,
      httpOnly: true,
      maxAge: 30_000,
    },
  })
)

//routes
app.get('/api/v1', (req, res) => {
  res.send('<h1>Hello there3!!!</h1>')
})
app.use('/api/v1/posts', postRouter)
app.use('/api/v1/users', userRouter)

app.listen(port, () => console.log(`Listening on port ${port}`))
