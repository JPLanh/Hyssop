const models = require('../Model/gameModel'),
instantiator = require('../Controller/instantiateController'),
dataController = require('../Controller/dataController'),
controller = require('../Controller/controller'),
serverController = require('../Controller/serverController'),
packetController = require('../Controller/packetController'),
mongoose = require('mongoose'),
item = mongoose.model('Item'),
areaItem = mongoose.model('AreaItem'),
area = mongoose.model('Area'),
itemMarket = mongoose.model('ItemMarket'),
itemDatabase = mongoose.model('ItemDatabase'),
plantDatabase = mongoose.model('PlantDatabase'),
areaIndex = mongoose.model('AreaIndex'),
areaPlant = mongoose.model('AreaPlant'),
storage = mongoose.model('Storage'),
entityExistance = mongoose.model("EntityExistance"),
itemExistance = mongoose.model('ItemExistance'),
user = mongoose.model('User'),
character = mongoose.model('Character'),
world = mongoose.model('World'),
currentTime = Date.now();


module.exports = async function(socket){
	socket.on('error', async () => {
		console.log("There was an error");
	})

	socket.on('connect', async (getSocket) => {
		var serverPing;

		await getSocket.on('Login', async (getPayload) => {
			let param = JSON.parse(getPayload);
			let payload = {};
			console.log(param);

			switch(param["Action"]){
				case "Login":
					user.findOne({"username": param["Username"], "password": param["Password"]}).exec()
					.then(async (found_user) => {
						if (found_user != null){
							found_user["active"] = true;
							getSocket.username = param["Username"];
							found_user.save();

							serverPing = setInterval(async () =>{
								await character.find({}).select('-backpack -holding -maxStamina -stamina').exec()
								.then(async (get_characters_list) => 
								{
									return await sendCharacterData(getSocket, 5, get_characters_list);	
								})
							}, 33);
							payload["action"] = "Welcome";
						} else {
							payload["action"] = "Denied";
						}
							sendPacket(getSocket, "Acknowledge", payload);
					})
					break;
				// case "Create user":
				// 	user.findOne({"username": param["Username"]}).exec()
				// 	.then(async (found_characters) => {
				// 		if (found_characters != null){
				// 			payload["action"] = "Failed user creation";
				// 		} else {
				// 			console.log("User created");
				// 			new user({"username": param["Username"], "password": param["Password"]}).save();
				// 			payload["action"] = "Created User";
				// 		}
				// 			sendPacket(getSocket, "Acknowledge", payload);
				// 	});
				// 	break;
				// case "Create character":					
				// 	let parsedJson = JSON.parse(param["Character"]);
				// 	parsedJson["account"] =  param["Username"];
				// 	characterAccount.findOne({"entityObj.entityName": parsedJson["entityName"]}).exec()
				// 	.then(async (find_character) => {
				// 		if (find_character != null){
				// 			payload["Action"] = "Duplicate character";
				// 			sendPacket(getSocket, "Acknowledge", payload);
				// 		} else {
				// 			return new Promise(async (resolve, reject) => {
				// 				resolve(await instantiator.createNewCharacter(parsedJson, param["Username"]));
				// 			})
				// 		}
				// 	})
				// 	.then(async () => {
				// 		return await character.find({"account": param["Username"]}).exec()	
				// 	})
				// 	.then(async (found_characters) => {
				// 		sendPacket(getSocket, "Character list", found_characters);
				// 	})
				// 	break;
				// case "Delete":
				// await character.deleteOne({"account": param["Username"], "entityName": param["EntityName"]}).exec()
				// .then(async () => {
				// 	await area.deleteMany({"areaName": param["EntityName"] + "_farm"}).exec()
				// })
				// .then(async () => {
				// 	await areaIndex.deleteMany({"areaName": param["EntityName"] + "_farm"}).exec()
				// })
				// .then(async () => {
				// 	return await itemExistance.find({"binder.entityName": param["EntityName"]}).exec()
				// })
				// .then(async (itemList) => {
				// 	itemList.forEach(async (it_itemList) => {
				// 		item.findByIdAndRemove(it_itemList.itemObj._id).exec();
				// 	})
				// })
				// .then(async () => {
				// 	return await itemExistance.find({"binder.entityName": param["EntityName"] + "_farm"}).exec()
				// })
				// .then(async (itemList) => {
				// 	itemList.forEach(async (it_itemList) => {
				// 		item.findByIdAndRemove(it_itemList.itemObj._id).exec();
				// 	})
				// })
				// .then(async () => {
				// 	await itemExistance.deleteMany({"binder.entityName": param["EntityName"]}).exec()
				// })
				// .then(async () => {
				// 	await areaItem.deleteMany({"areaObj.areaName": param["EntityName"] + "_farm"}).exec()
				// })
				// .then(async () => {
				// 	await areaPlant.deleteMany({"index.areaName": param["EntityName"] + "_farm"}).exec()
				// })
				// .then(async () => {						
				// 	return await character.find({"account": param["Username"]}).exec()	
				// })
				// .then(async (found_characters) => {
				// 	sendPacket(getSocket, "Character list", found_characters);
				// })
				// break;
			}	
		})


		await getSocket.on('Packet', async (getPayload) => {
			let param = JSON.parse(getPayload);
			packetController.processPacket(socket, getSocket, param);
		})

		getSocket.on('disconnecting', async () => 
		{
			user.findOne({"username": getSocket.username}).exec()
			.then(async (found_user) => {
				found_user["active"] = false;
				clearInterval(serverPing);
				found_user.save();
			});
		});

		getSocket.on('disconnect', async () => {
			console.log("User Disconnected");
		});
	})
}

async function sendCharacterData(in_socket, in_pagination, in_list)
{
	let newParam = {};
	let counter = 0;

	for (let index = 0; index < in_list.length; index+=in_pagination){
		await new Promise(async (resolve, reject) => {
			if (index > in_list.length){
				newParam["characterList"] = in_list.slice(index, in_list.length)																
			} else {
				newParam["characterList"] = in_list.slice(index, index+in_pagination)																
			}
			newParam["Action"] = "Update characters";
			newParam["index"] = (index + in_pagination > in_list.length ? in_list.length : index + in_pagination);
			newParam["total"] = in_list.length
			newParam["time"] = Date.now()%1000000;
			counter = (index + in_pagination > in_list.length ? in_list.length : index + in_pagination);
			await resolve(await in_socket.emit("Character updates", JSON.stringify(newParam).replace(/"/g, "`")));
		})
	}
	if (counter < in_list.length){								
		new Promise(async (resolve, reject) => {
			newParam["characterList"] = in_list.slice(counter, in_list.length)
			newParam["Action"] = "Update characters";
			newParam["index"] = in_list.length;
			newParam["total"] = in_list.length
			newParam["time"] = Date.now()%1000000;
			resolve(await in_socket.emit("Character updates", JSON.stringify(newParam).replace(/"/g, "`")));
		})
	}
	return new Promise(async (resolve, reject) =>{
		newParam["characterList"] = null;
		newParam["Action"] = "Complete";
		newParam["time"] = Date.now()%1000000;
		resolve(await in_socket.emit("Character updates", newParam));									
	})
}

async function sendPacket(in_socket, in_type, in_data)
{
	return await new Promise(async (resolve, reject) => {
		let newParam = {};
		newParam["type"] = in_type;
		newParam["data"] = JSON.stringify(in_data).replace(/"/g, "`");
		in_socket.emit("Get Data", newParam);
		resolve("Done");
	})
}