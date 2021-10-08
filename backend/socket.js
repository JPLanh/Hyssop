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
		var message = {}

		await getSocket.on('Login', async (getPayload) => {
			let param = JSON.parse(getPayload);
			let newParam = {};

			switch(param["Action"]){
				case "Login":
				user.findOne({"username": param["Username"], "password": param["Password"]}).exec()
				.then(async (found_user) => {
					if (found_user != null){
						// newParam["Action"] = "Welcome";	
						// newParam["Username"] = param["Username"];
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

						sendPacket(getSocket, "Acknowledge", "Welcome");
					} else {
						sendPacket(getSocket, "Acknowledge", "Denied");
					}

//					getSocket.emit("Action", JSON.stringify(newParam).replace(/"/g, "`"));
				})
				break;
				case "Create user":
				console.log(param);

				user.findOne({"username": param["Username"]}).exec()
				.then(async (found_characters) => {
					if (found_characters != null){
						newParam["Action"] = "Failed user creation";
					} else {
						console.log("User created");
						new user({"username": param["Username"], "password": param["Password"]}).save();
					}
						sendPacket(getSocket, "Acknowledge", "Created User");
				});
				break;
				case "Create character":					
				let parsedJson = JSON.parse(param["Character"]);
				parsedJson["account"] =  param["Username"];
				character.findOne({"entityName": parsedJson["entityName"]}).exec()
				.then(async (find_character) => {
					if (find_character != null){
						newParam["Action"] = "Duplicate character";
						getSocket.emit("Action", JSON.stringify(newParam).replace(/"/g, "`"));
					} else {
						return new Promise(async (resolve, reject) => {
							resolve(await instantiator.createNewCharacter(parsedJson));
						})
					}
				})
				.then(async () => {
					return await character.find({"account": param["Username"]}).exec()	
				})
				.then(async (found_characters) => {
					sendPacket(getSocket, "Character list", found_characters);
				})
				break;
				case "Delete":
				await character.deleteOne({"account": param["Username"], "entityName": param["EntityName"]}).exec()
				.then(async () => {
					await area.deleteMany({"areaName": param["EntityName"] + "_farm"}).exec()
				})
				.then(async () => {
					await areaIndex.deleteMany({"areaName": param["EntityName"] + "_farm"}).exec()
				})
				.then(async () => {
					return await itemExistance.find({"binder.entityName": param["EntityName"]}).exec()
				})
				.then(async (itemList) => {
					itemList.forEach(async (it_itemList) => {
						item.findByIdAndRemove(it_itemList.itemObj._id).exec();
					})
				})
				.then(async () => {
					return await itemExistance.find({"binder.entityName": param["EntityName"] + "_farm"}).exec()
				})
				.then(async (itemList) => {
					itemList.forEach(async (it_itemList) => {
						item.findByIdAndRemove(it_itemList.itemObj._id).exec();
					})
				})
				.then(async () => {
					await itemExistance.deleteMany({"binder.entityName": param["EntityName"]}).exec()
				})
				.then(async () => {
					await areaItem.deleteMany({"areaObj.areaName": param["EntityName"] + "_farm"}).exec()
				})
				.then(async () => {
					await areaPlant.deleteMany({"index.areaName": param["EntityName"] + "_farm"}).exec()
				})
				.then(async () => {						
					return await character.find({"account": param["Username"]}).exec()	
				})
				.then(async (found_characters) => {
					sendPacket(in_socket, "Character list", found_characters);
				})
				break;
			}	
		})

		await getSocket.on('Packet', async (getPayload) => {
			let param = JSON.parse(getPayload);
			packetController.processPacket(socket, getSocket, param);
		})

		await getSocket.on('Player', async (getPayload) => {
			let param = JSON.parse(getPayload);

			switch(param["input"]){
				case "Items":
					return await itemExistance.find({"binder.entityName": param["Username"]}).select('-binder').exec()	
					.then(async (found_items) => {
						sendPacket(getSocket, "Item list", found_items);
					})
					break;
			}
		})

		await getSocket.on('Loading', async (getPayload) => {
			let param = JSON.parse(getPayload);
			let newParam = {};

			switch(param["Action"]){
				case "Get plant database":				
					await plantDatabase.find({}).exec()
					.then(async (get_plantdatabase) => {
						await dataController.sendListData(getSocket, 5, get_plantdatabase, "plantList", "Load plant database", "Load all plants", "All plants complete");	
					})
					break;
				case "Get item database":				
					await itemDatabase.find({}).exec()
					.then(async (get_itemdatabase) => {
						await dataController.sendListData(getSocket, 5, get_itemdatabase, "itemList", "Load item database", "Load all Items", "All character items complete");	
					})
					break;
				case "Get Inventory":
					return await itemExistance.find({"binder.entityName": param["Username"]}).select('-binder').exec()	
					.then(async (found_items) => {

//						await dataController.sendListData(getSocket, 3, found_items, "itemList", "Load Items", "Load all Items", "All character items complete");	
					})	
					break;	
				case "Check Farm":
//				console.log(param);
					area.findOne({"areaName": param["Username"] + "_farm", "worldObj.worldName": param["World"]})
					.then(async (getFarm) => {
						if (getFarm == null){
							console.log("Farm generated");
							world.findOne({"worldName": param["World"]}).exec()
							.then(async (get_world) =>{
								return await instantiator.generatePlayerFarm(getSocket, get_world, param["Username"]);
							})
						}
					})
					.then(async () =>{
						newParam["Action"] = "Farm generated";
						getSocket.emit("Action", newParam);
					})
					break;
			}
		});

		getSocket.on('Item', async (getPayload) => {
			let param = JSON.parse(getPayload);
			switch(param["Action"]){
				case "Entity pickup":
					instantiator.entity_getItem(getSocket, param["EntityName"], param["Item"], param["Quantity"], "pickup")
					break;
				case "NPC pickup":
					instantiator.npc_getItem(getSocket, param["EntityName"], param["Item"], param["Quantity"], "pickup", param)
					break;
				case "Update":

				}
			})

			// await itemExistance.findOne({"binder.entityName": param["EntityName"], "itemObj.itemName": param["Item"]}).exec()
			// .then(async (item_found) => {
			// 	if (item_found) {
			// 		item_found["itemObj"]["quantity"] += parseInt(param["Quantity"]);
			// 		if (item_found["itemObj"]["quantity"] <= 0){
			// 			item_found.remove();
			// 		} else {
			// 			item_found.save();
			// 		}
			// 	} else {
			// 		let getCharacter = character.findOne({"entityName": param["EntityName"]}).exec()
			// 		let getItem = itemDatabase.findOne({"itemName": param["Item"]}).exec()

			// 		await Promise.all([getCharacter, getItem])
			// 		.then(async (res) => {
			// 			switch(param["Action"]){
			// 				case "New":
			// 				newParam["Action"] = "New item";
			// 				return await new item({"itemName": res[1]["itemName"], "itemType": res[1]["itemType"], maxDurability: res[1]["maxDurability"], durability: res[1]["maxDurability"], state: "", capacity: 0, maxCapacity: res[1]["maxCapacity"], quantity: param["Quantity"]}).save()
			// 				.then(async (newItem) => {
			// 					newParam["Item"] = newItem;
			// 					getSocket.emit("Item", JSON.stringify(newParam).replace(/"/g, "`"));				
			// 					return await new itemExistance({"binder": res[0], "itemObj": newItem}).save()
			// 				})
			// 				break;
			// 			}

			// 		})
			// 	}
			// })
			//})
			getSocket.on("Load Database", async(in_payload) =>{
				let param = JSON.parse(in_payload);
				switch(in_data["action"]){
					case "Items":
						await itemDatabase.find({}).exec()
						.then(async (get_itemdatabase) => {
							await dataController.sendListData(getSocket, 5, get_itemdatabase, "items", "Load database", "Load all Items", "All character items complete");	
						})
					break;
					case "Plants":
						await plantDatabase.find({}).exec()
						.then(async (get_plantdatabase) => {
							await dataController.sendListData(getSocket, 5, get_plantdatabase, "plants", "Load database", "Load all plants", "All plants complete");	
						})
					break;
				}
			})

		getSocket.on("Action Index", async(in_payload) =>{
			let param = JSON.parse(in_payload);
			let in_data = JSON.parse(param["Data"]);
			let new_param = {};

			switch(in_data["action"]){
				case "Pick vegetation":
//					new_param = in_data;
					return await areaPlant.findOne({"index.areaObj.areaName": in_data["areaName"], "index.areaObj.worldObj.worldName": param["worldName"], "index.x": in_data["index"]["x"], "index.y": in_data["index"]["y"], "index.z": in_data["index"]["z"]}).exec()
					.then(async (get_areaPlant) => {
						return await new Promise(async (resolve, reject) => {						
							if (get_areaPlant["deathDayPassed"] >= get_areaPlant["deathDayRequired"])
							{
								new_param["action"] = "Removing " + get_areaPlant["plantName"];
							} else {
								if (get_areaPlant["dayPassed"] >= get_areaPlant["dayRequired"]){
									new_param["action"] = "Harvesting " + get_areaPlant["plantName"];
								}
							}
							await resolve("Done");
						})

					})
					.then(async () =>{
						await getSocket.emit("Action Index", JSON.stringify(new_param).replace(/"/g, "`"));
					})
	
					break;
			}

		});

		getSocket.on("Trade", async(in_payload) => {
			let param = JSON.parse(in_payload);
			let newParam = {}

			let get_trade;
			switch(param["fromType"]){
				case "Storage":
					get_trade = itemExistance.findOne({"itemObj._id": mongoose.Types.ObjectId(param["item"]), "storageObj._id": mongoose.Types.ObjectId(param["fromName"])})
					break;
				case "Entity":
				case "NPC":
					get_trade = itemExistance.findOne({"itemObj._id": mongoose.Types.ObjectId(param["item"]), "binder.entityName": param["fromName"]})
					break;
			}
			let to_entity;
			switch(param["toType"]){
				case "Storage":
					to_entity = areaItem.findOne({"itemObj._id": mongoose.Types.ObjectId(param["toName"])});
					break;
				case "Entity":
					to_entity = character.findOne({"entityName": param["toName"]});
					break;
				case "NPC":
					to_entity = entityExistance.findOne({"entityObj.entityName": param["toName"]});
					break;
			}

			Promise.all([get_trade, to_entity])
			.then(async (res) =>{			
				switch(param["toType"]){
					case "Storage":
						res.push(await itemExistance.findOne({"itemObj.itemName": res[0]["itemObj"]["itemName"], "storageObj._id": mongoose.Types.ObjectId(param["toName"])}).exec());
						break;
					case "Entity":
					case "NPC":
						res.push(await itemExistance.findOne({"itemObj.itemName": res[0]["itemObj"]["itemName"], "binder.entityName": param["toName"]}).exec());
						break;
				}
				return res;
			})
			.then(async (res) => {
				res[0]["itemObj"]["quantity"] -= parseInt(param["quantity"]);
				if (res[2] != null){
					res[2]["itemObj"]["quantity"] += parseInt(param["quantity"]);
					await res[2].save();
				} else {
					await res.pop();
					let temp_item = await new item(res[0].itemObj);
					temp_item.quantity = param["quantity"];
					switch(param["toType"]){
						case "Storage":
						res.push(await new itemExistance({"itemObj": temp_item, "storageObj": res[1]["itemObj"]}).save());
						break;
						case "Entity":
							res.push(await new itemExistance({"itemObj": temp_item, "binder": res[1]}).save());
							break;
						case "NPC":
							res.push(await new itemExistance({"itemObj": temp_item, "binder": res[1]["entityObj"]}).save());
							break;
					}
				}
				if (res[0]["itemObj"]["quantity"] <= 0){
					res[0].remove();
				} else {
					res[0].save();
				}
				return res;
			})
			.then(async (res) => {
				await getSocket.emit("Transfer Item", JSON.stringify(res[2]).replace(/"/g, "`"));
				await getSocket.emit("Transfer Item", JSON.stringify(res[0]).replace(/"/g, "`"));				
			})
		})


		// getSocket.on("Trade", async(in_payload) => {
		// 	let param = JSON.parse(in_payload);
		// 	let newParam = {}

		// 	let get_trade = itemExistance.findOne({"itemObj._id": mongoose.Types.ObjectId(param["item"])})
		// 	let to_entity = areaItem.findOne({"itemObj._id": mongoose.Types.ObjectId(param["toName"])});

		// 	Promise.all([get_trade, to_entity])
		// 	.then(async (res) =>{			
		// 		res.push(await itemExistance.findOne({"itemObj.itemName": res[0]["itemObj"]["itemName"], "storageObj._id": mongoose.Types.ObjectId(param["toName"])}).exec());
		// 		return res;
		// 	})
		// 	.then(async (res) => {
		// 		res[0]["itemObj"]["quantity"] -= 1;
		// 		if (res[2] != null){
		// 			res[2]["itemObj"]["quantity"] += parseInt(param["quantity"]);
		// 			await res[2].save();
		// 		} else {
		// 			await res.pop();
		// 			let temp_item = await new item(res[0].itemObj);
		// 			temp_item.quantity = param["quantity"];
		// 			res.push(await new itemExistance({"itemObj": temp_item, "storageObj": res[1]["itemObj"]}).save());
		// 		}
		// 		res[0].save();
		// 		return res;
		// 	})
		// 	.then(async (res) => {
		// 		await getSocket.emit("Transfer Item", JSON.stringify(res[2]).replace(/"/g, "`"));
		// 		await getSocket.emit("Transfer Item", JSON.stringify(res[0]).replace(/"/g, "`"));				
		// 	})
		// })

		getSocket.on('Save Entity', async (in_payload) =>{
			let param = JSON.parse(in_payload);
			let in_data = JSON.parse(param["Data"]);
//			console.log(in_data);

			let currentRoom = Object.keys(getSocket.rooms).filter(function(item) {
			    return item !== getSocket.id;
			});

			if (currentRoom != in_data["areaName"]){
				currentRoom.forEach(async (room) => {
					getSocket.leave(room);
				});
//				console.log(getSocket);
//				console.log(currentRoom + " , " + in_data["areaName"]);
				getSocket.join(in_data["areaName"]);
//				console.log(getSocket);

			}

			character.findOne({"entityName": in_data["entityName"]}).exec()
			.then(async (foundEntity) => {
				foundEntity["position"] = in_data["position"];
				foundEntity["rotation"] = in_data["rotation"];
				foundEntity["areaName"] = in_data["areaName"];
				foundEntity["stamina"] = in_data["stamina"];
				foundEntity["maxStamina"] = in_data["maxStamina"];
				foundEntity["state"] = in_data["state"];
				foundEntity["holding"] = in_data["holding"];
				foundEntity["backpack"] = in_data["backpack"];
				foundEntity.save();
			})
		})

		getSocket.on('Save Item', async (in_payload) =>{
			let param = JSON.parse(in_payload);
			let in_data = JSON.parse(param["Data"]);

			itemExistance.findOneAndUpdate({"itemObj._id": mongoose.Types.ObjectId(in_data["Item"]["_id"])}, {"itemObj": in_data["Item"]}).exec();
		})

		getSocket.on('Save World', async (in_payload) =>{
			let param = JSON.parse(in_payload);
			let in_data = JSON.parse(param["Data"]);

			switch(in_data["Action"]){
				case "New World":
					await new world(in_data).save()
					.then(async (new_world) => {
						return await instantiator.newWorld(getSocket, new_world);
					})
					.then(async (save_world) => {
						return await world.find({}).exec()
					})
					.then(async (getWorlds) => {
						serverController.init();
						sendPacket(getSocket, "World list", getWorlds);
//						await dataController.sendListData(getSocket, 10, getWorlds, "worldList", "Load worlds", "Load all World", "All world loaded complete");	
					})				
					break;
				case "Save World":
					serverController.worlds.map(it_world => {
						if (it_world["worldName"] === in_data["worldName"]){
							it_world["time"] = in_data["time"];
							var index = serverController.worlds.indexOf(it_world);
							  if (index > -1) {
								    serverController.worlds.splice(index, 1);
							  }
							  in_data["Action"] = "Update world";
							serverController.worlds.push(it_world);
							sendPacket(getSocket, "World", in_data);
	//						socket.emit("Load world", JSON.stringify(in_data).replace(/"/g, "`"));
						}
					});
					break;
			}

		})

		getSocket.on('Load Area', async (in_payload) => {
			let param = JSON.parse(in_payload);
			let in_data = JSON.parse(param["Data"]);

			return await area.findOne({"areaName": in_data["areaName"]}).exec()
			.then(async (getArea) => {
				let newParam = {};
				newParam["length"] = getArea['length'];
				newParam["width"] = getArea['width'];
				newParam["height"] = getArea["height"];
				newParam["areaName"] = getArea["areaName"];
				newParam["Action"] = "Preload grid";
				getSocket.emit("Area", JSON.stringify(newParam).replace(/"/g, "`"));
			})
		})

		getSocket.on('Access Storage', async (in_payload) =>{
			let param = JSON.parse(in_payload);

			return await itemExistance.find({"storageObj._id": mongoose.Types.ObjectId(param["Data"])}).exec()
			.then(async (getStorage) => {
				await dataController.sendListData(getSocket, 2, getStorage, "storageList", "Load storage access", "Load all storage item", "All world loaded complete");	
			})
		})



		getSocket.on('Save Index', async (in_payload) =>
		{
			let newParam = {};


			let param = JSON.parse(in_payload);
			let wrapper = JSON.parse(param["Data"]);
			let parse = wrapper["state"].split(' ');
			let gridIndex = wrapper["Data"];
			// console.log(param);
			// console.log(parse);
			// console.log(gridIndex);
			switch(parse[0])
			{
				case "Plow":
					areaIndex.findOneAndUpdate({"areaObj.areaName": wrapper["areaName"], "areaObj.worldObj.worldName": param["worldName"], "x": gridIndex["x"], "y": gridIndex["y"], "z": gridIndex["z"]}, {"objectName": gridIndex["objectName"], "destructable": gridIndex["destructable"], "pickable": gridIndex["pickable"], "state": gridIndex["state"]}, {upsert:true}).exec()
					socket.emit("Area Update", JSON.stringify(gridIndex).replace(/"/g, "`"));
				break;
				case "Plant":
					let seed = wrapper["state"].replace(parse[0] + " ", "");
					let foundIndex = areaIndex.findOne({"areaObj.areaName": wrapper["areaName"], "areaObj.worldObj.worldName": param["worldName"], "x": gridIndex["x"], "y": gridIndex["y"], "z": gridIndex["z"]});
					let foundPlant = plantDatabase.findOne({"seedName": seed});

					Promise.all([foundIndex, foundPlant])
					.then(async (res) => {
						new areaPlant({"index": res[0], "plantName": res[1]["seedName"], "state": "Growing", "dayPassed": 0, "dayRequired": res[1]["dayRequired"], "deathDayPassed": 0, "deathDayRequired": res[1]["deathDayRequired"], isWatered: false}).save()
						.then(async (newPlant) => {
							sendPacket(getSocket, "Area Plant", newPlant);
							// newParam["index"] = res[0];
							// newParam["plant"] = newPlant;
							// getSocket.emit("Load Plant", JSON.stringify(newParam).replace(/"/g, "`"));
						})
					})
					break;
				case "Harvest":
					await areaIndex.deleteOne({"areaObj.areaName": wrapper["areaName"], "areaObj.worldObj.worldName": param["worldName"], "x": gridIndex["x"], "y": gridIndex["y"], "z": gridIndex["z"]}).exec()
					.then(async () => {
						return await areaPlant.deleteOne({"index.areaObj.areaName": wrapper["areaName"], "index.areaObj.worldObj.worldName": param["worldName"], "index.x": gridIndex["x"], "index.y": gridIndex["y"], "index.z": gridIndex["z"]}).exec();
					})
					newParam["x"] = gridIndex["x"];
					newParam["y"] = gridIndex["y"];
					newParam["z"] = gridIndex["z"];
					newParam["state"] = "Clear";					
					socket.emit("Area Update", JSON.stringify(newParam).replace(/"/g, "`"));	
					break;				
				case "Water":
				areaPlant.findOneAndUpdate({"index.areaObj.areaName": wrapper["areaName"], "index.areaObj.worldObj.worldName": param["worldName"], "index.x": gridIndex["x"], "index.y": gridIndex["y"], "index.z": gridIndex["z"]}, {"isWatered": true}, {upsert:true, new:true}).exec()
				.then(async (getPlant) => {
					sendPacket(getSocket, "Area Plant", getPlant);
					// newParam["index"] = getPlant["index"];
					// newParam["plant"] = getPlant;
					// getSocket.emit("Load Plant", JSON.stringify(newParam).replace(/"/g, "`"));
				})
				break;
			}
		})

		getSocket.on('Action', async (in_payload) => {
			let param = JSON.parse(in_payload);
			let newParam = {};
			switch(param["Action"]){
				case "New day":
					return serverController.newDay(socket, param["worldName"]);
				case "Testing NPC":
						instantiator.generateCentralHubNPC("Central Hub");
				default:
					break;
			}
		})

		getSocket.on('Shop', async (in_payload) =>{
			let param = JSON.parse(in_payload);
			let newParam = {};

			switch(param["Action"]){
				case "Get item database":
					return await itemExistance.find({"binder.entityName": param["Target"]}).select('-binder').exec()	
					.then(async (found_items) => {
						await dataController.sendListData(getSocket, 3, found_items, "itemList", "Load Items", "Load all Items", "All character items complete");	
					})	
					break;
			}

		})

		getSocket.on('Area', async (in_payload) => {
			let param = JSON.parse(in_payload);
			let newParam = {};

			switch (param["state"]){
			}
		})

		getSocket.on('Update area item', async (in_payload) => {
			let param = JSON.parse(in_payload);
			let newParam = {};

			areaItem.findByIdAndUpdate(param["_id"], {"itemObj": param["itemObj"], "position": param["position"], "rotation": param["rotation"]}).exec()
		});


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