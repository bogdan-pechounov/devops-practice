const express = require('express')
const path = require('path')
const fs = require('fs')

const app = express()

//mongo
const { MongoClient } = require('mongodb')

const url = 'mongodb://admin:password@localhost:27017'
const dbName = 'docker-demo'

const client = new MongoClient(url)

//middleware
app.use(express.urlencoded({ extended: true }))
app.use(express.json())

//routes
app.get('/', (req, res) => {
  res.sendFile(path.join(__dirname, 'index.html'))
})

app.get('/profile-picture', (req, res) => {
  const img = fs.readFileSync(path.join(__dirname, 'images', 'profile-1.jpg'))
  res.writeHead(200, { 'Content-Type': 'image/jpg' })
  res.end(img, 'binary')
})

app.get('/get-profile', async (req, res) => {
  await client.connect()
  const user = await client
    .db(dbName)
    .collection('users')
    .findOne({ userid: 1 })
  console.log(user)
  await client.close()
  res.send(user || {})
})

app.post('/update-profile', async (req, res) => {
  const user = req.body
  await client.connect()
  user.userid = 1
  const ack = await client
    .db(dbName)
    .collection('users')
    .updateOne({ userid: user.userid }, { $set: user }, { upsert: true })
  console.log(user, ack)
  await client.close()
  res.send(user)
})

//listen
app.listen(3000, () => {
  console.log('Listening on port 3000...')
})
