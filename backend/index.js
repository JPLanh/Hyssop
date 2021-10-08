const
	express = require('express'),
  app = express(),
//	server = require("http").Server(app),
  server = require("http").createServer(app),
  options = {maxHttpBufferSize: 5e8},
	io = require('socket.io')(server, options),//(server, {cords: {origin: "*" }}),
	socket = require('./API/Socket/socket'),
  serverController = require('./API/Controller/serverController'),
  port = process.env.PORT || 26843,
//      cors = require('cors'),
  mongoose = require('mongoose');
//	routes = require('./API/Route/gameRoute');
//      bodyParser = require('body-parser');

socket(io);

server.listen(port, function(){
	console.log("Starting up service");
});

serverController.init();

mongoose.Promise = global.Promise;
mongoose.connect('mongodb://localhost/hyssop');

//instantiator.instantiateDatabase()


/*
app.use(function(req, res, next){
//  res.header("Access-Control-Allow-Origin", //"https://jplanh.tk");
  res.header("Access-Control-Allow-Credentials", true);
  res.header("Access-Control-Allow-Methods", "DELETE, POST, GET, OPTIONS");
  res.header("Access-Control-Allow-Headers", "x-refresh-token, x-access-token, Origin, X-Requested-With, Content-Type, Accept");
  next();
});

app.use(bodyParser.urlencoded({ extended: true }));
app.use(bodyParser.json());
app.use(cors());

app.use(require('express-session')({
    secret: "Super secret thingamajid",
    resave: false,
    saveUninitialized: false
}));

routes(app);

app.use(function(req, res)
{
        res.status(404).send({url: req.originalUrl + ' not found'})
});
*/
