const
	express = require('express'),
  app = express(),
  server = require("http").createServer(app),
  options = {maxHttpBufferSize: 5e8},
	io = require('socket.io')(server, options),
	socket = require('./API/Socket/socket'),
  port = process.env.PORT || 26843,
  mongoose = require('mongoose');

socket(io);

server.listen(port, function(){
	console.log("Starting up service");
});

serverController.init();

mongoose.Promise = global.Promise;
mongoose.connect('mongodb://localhost/hyssop');