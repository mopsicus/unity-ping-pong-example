const net = require('net')

let players = []

const server = net.createServer((socket) => {
  socket.on('data', (chunk) => {
    if (typeof chunk === 'string') {
      chunk = Buffer.from(chunk, 'utf8')
    }
    players.forEach((item) => {
      item.write(chunk)
    })
    return true
  })
  players.push(socket)
  players.forEach((item) => {
    let data = {}
    data.c = players.length
    data.cmd = 0
    let json = Buffer.from(JSON.stringify(data), 'utf8')
    item.write(json)
  })
})

server.listen(3001, '192.168.0.111')
