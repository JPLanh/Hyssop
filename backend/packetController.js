const models = require('../Model/gameModel'),
instantiator = require('../Controller/instantiateController'),
dataController = require('../Controller/dataController'),
controller = require('../Controller/controller'),
serverController = require('../Controller/serverController'),
mongoose = require('mongoose'),
item = mongoose.model('Item'),
areaItem = mongoose.model('AreaItem'),
area = mongoose.model('Area'),
itemMarket = mongoose.model('ItemMarket'),
itemDatabase = mongoose.model('ItemDatabase'),
plantDatabase = mongoose.model('PlantDatabase'),
characterAccount = mongoose.model('CharacterAccount'),
areaIndex = mongoose.model('AreaIndex'),
areaPlant = mongoose.model('AreaPlant'),
storage = mongoose.model('Storage'),
entityExistance = mongoose.model("EntityExistance"),
itemExistance = mongoose.model('ItemExistance'),
user = mongoose.model('User'),
character = mongoose.model('Character'),
world = mongoose.model('World'),
currentTime = Date.now();

exports.processPacket = async function(in_broadcast_socket, in_socket, in_param)
{
	switch(in_param["command"]){
		case "User":
			processUser(in_socket, in_param);
			break;	
		case "Character":
			processCharacter(in_socket, in_param);
			break;	
		case "Database":
			processDatabase(in_socket, in_param);
			break;
		case "Update":
			processUpdate(in_socket, in_param);
			break;
		case "Preload":
			processPreload(in_socket, in_param);
			break;
		case "Player":
			processPlayer(in_socket, in_param);
			break;
		case "Load":
			processLoad(in_socket, in_param);
			break;		
		case "Action":
			processAction(in_broadcast_socket, in_socket, in_param);
			break;
		case "Player Item":
			processPlayerItem(in_socket, in_param);
			break;
		case "Item":
			processItem(in_socket, in_param);
			break;
		case "Area":
			processArea(in_socket, in_param);
			break;
		case "Storage":
			processStorage(in_socket, in_param);
			break;
		case "World":
			processWorld(in_broadcast_socket, in_socket, in_param);
			break;
		case "Index":
			processIndex(in_broadcast_socket, in_socket, in_param);
			break;
		case "Entity":
			processEntity(in_socket, in_param);
			break;
	}
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
async function processUser(in_socket, in_param)
{
	let newParam = {};
    let param = JSON.parse(in_param["data"]);
	switch(in_param["input"]){		
		case "Create user":
			user.findOne({"username": param["Username"]}).exec()
			.then(async (found_characters) => {
				if (found_characters != null){
					payload["action"] = "Failed user creation";
				} else {
					console.log("User created");
					new user({"username": param["Username"], "password": param["Password"]}).save();
					payload["action"] = "Created User";
				}
					sendPacket(getSocket, "Acknowledge", payload);
			});
			break;
	}
}

async function processCharacter(in_socket, in_param)
{
//	console.log(in_param);
	let newParam = {};
    let param = JSON.parse(in_param["data"]);
	switch(in_param["input"]){		
		case "All Characters":
			return await characterAccount.find({"account": in_param["username"]}).exec()	
			.then(async (found_characters) => {
				sendPacket(in_socket, "Character list", found_characters);
				//await dataController.sendListData(getSocket, 1, found_characters, "characterList", "Load characters", "Load all Characters", "All characters complete");	
			})
			break;
		case "Create":					
			let entityExistance = JSON.parse(param["Character"])
			let charData = entityExistance["entityObj"];
			//console.log(charData);
			characterAccount.findOne({"entityObj.entityName": charData["entityName"]}).exec()
			.then(async (find_character) => {
				if (find_character != null){
					payload["Action"] = "Duplicate character";
					sendPacket(in_socket, "Acknowledge", payload);
				} else {
					return new Promise(async (resolve, reject) => {
						resolve(await instantiator.createNewCharacter(charData, entityExistance["position"], entityExistance["rotation"], in_param["username"]));
					})
				}
			})
			.then(async () => {
				return await characterAccount.find({"account": in_param["username"]}).exec()	
			})
			.then(async (found_characters) => {
				sendPacket(in_socket, "Character list", found_characters);
			})
			break;
		case "Delete":
			await character.deleteOne({"account": param["Username"], "entityName": param["EntityName"]}).exec()
			.then(async () => {
				await area.deleteMany({"areaName": param["EntityName"] + "_" + param["Occupation"]}).exec()
			})
			.then(async () => {
				await areaIndex.deleteMany({"areaName": param["EntityName"] + "_" + param["Occupation"]})
			})
			.then(async () => {
				return await itemExistance.find({"binder.entityName": param["EntityName"]}).exec()
			})
			.then(async (itemList) => {
				itemList.forEach(async (it_itemList) => {
					item.findByIdAndRemove(it_itemList.entityObj._id).exec();
				})
			})
			.then(async () => {
				return await itemExistance.find({"binder.entityName": param["EntityName"] + "_" + param["Occupation"]}).exec()
			})
			.then(async (itemList) => {
				itemList.forEach(async (it_itemList) => {
					item.findByIdAndRemove(it_itemList.entityObj._id).exec();
				})
			})
			.then(async () => {
				await itemExistance.deleteMany({"binder.entityName": param["EntityName"]}).exec()
			})
			.then(async () => {
				await areaItem.deleteMany({"areaObj.areaName": param["EntityName"] + "_" + param["Occupation"]}).exec()
			})
			.then(async () => {
				await areaPlant.deleteMany({"index.areaName": param["EntityName"] + "_" + param["Occupation"]}).exec()
			})
			.then(async () => {						
				return await character.find({"account": param["Username"]}).exec()	
			})
			.then(async (found_characters) => {
				sendPacket(getSocket, "Character list", found_characters);
			})
			break;
	}
}

async function processDatabase(in_socket, in_param){
	switch(in_param["input"]){				
		case "Plants":				
			await plantDatabase.find({}).exec()
			.then(async (found_plants) => {
				sendPacket(in_socket, "Plant database", found_plants);
//				await dataController.sendListData(in_socket, 5, get_plantdatabase, "plantList", "Load plant database", "Load all plants", "All plants complete");	
			})
			break;
		case "Items":				
			await itemDatabase.find({}).exec()
			.then(async (found_items) => {
				sendPacket(in_socket, "Item database", found_items);
//				await dataController.sendListData(in_socket, 5, get_itemdatabase, "itemList", "Load item database", "Load all Items", "All character items complete");	
			})
			break;
	}
}

async function processUpdate(in_socket, in_param)
{
	let data = JSON.parse(in_param["data"]);
	let get_entity = data["entityObj"];
	let get_position = data["position"];
	let get_rotation = data["rotation"];
	entityExistance.findOneAndUpdate({"entityObj._id": mongoose.Types.ObjectId(get_entity["_id"])}, data, {upsert:true, new:true}).exec();
	//mongoose.Types.ObjectId(param["storageID"])
}

async function processPreload(in_socket, in_param){
	newParam = {};
	let data = JSON.parse(in_param["data"]);
//	console.log(in_param);
	switch(in_param["input"])
	{
		case "Generate farm":
		//Need to fix this
			area.findOne({"areaName": in_param["entity"] + "_" + data["occupation"], "worldObj.worldName": in_param["worldName"]})
			.then(async (getFarm) => {
				if (getFarm == null){
					console.log(data["occupation"] + " generated");
					return await world.findOne({"worldName": in_param["worldName"]}).exec()
					.then(async (get_world) =>{
						return await instantiator.generatePlayerFarm(in_socket, get_world, in_param["entity"], data["occupation"]);
					})
				} else return getFarm;
			})
			.then(async (getArea) =>{

							in_socket.username = data["entityID"];
//				newParam["action"] = "Farm generated";
//				newParam["data"] = getArea; 
				sendPacket(in_socket, "Area", getArea);
			})
			break;		
		case "Preload Area":
			// console.log(in_param);
			param = in_param["data"];
			// console.log(param);
			return await area.findOne({"areaName": param["areaName"]}).exec()
			.then(async (getArea) => {
				// console.log(getArea);
				let newParam = {};
				newParam["length"] = getArea['length'];
				newParam["width"] = getArea['width'];
				newParam["height"] = getArea["height"];
				newParam["areaName"] = getArea["areaName"];
				newParam["Action"] = "Preload grid";
				sendPacket(in_socket, "Acknowledge", newParam);
//				getSocket.emit("Area", JSON.stringify(newParam).replace(/"/g, "`"));
			})
			break;
	}
}

async function processPlayer(in_socket, in_param){
    let param = JSON.parse(in_param["data"]);
	switch(in_param["input"]){				
		case "Items":
			return await itemExistance.find({"binder.entityName": param["entity"]}).select('-binder').exec()	
			.then(async (found_items) => {
				sendPacket(in_socket, "Item list", found_items);
			})	
		break;	
	}
}

async function processLoad(in_socket, in_param)
{
//	console.log(in_param);
	switch(in_param["input"])
	{
		case "World":
			return await world.findOne({"worldName": in_param["worldName"]}).exec()
			.then(async (found_world) => {
					sendPacket(in_socket, "World", found_world);
			})
			break;
		case "Config":
			area.findOne({"areaName": in_param["areaName"]}).exec()
			.then(async (found_area_config) => {
				sendPacket(in_socket, "Area", found_area_config);
			})
			break;
		case "Area Indexes":
			areaIndex.find({"areaObj.areaName": in_param["areaName"], "areaObj.worldObj.worldName": in_param["worldName"]}).select('-areaObj').exec()
			.then(async (found_areas) => {
				sendPacket(in_socket, "Area Indexes", found_areas);
//				await dataController.sendListData(in_socket, 5, found_areas, "areaIndices", "Area", "Load area grid", "All area grid loaded complete");	
			})
			break;
		case "Area Items":
			areaItem.find({"areaObj.areaName": in_param["areaName"], "areaObj.worldObj.worldName": in_param["worldName"]}).select('-areaObj').exec()
			.then(async (found_area_item) => {
				sendPacket(in_socket, "Area Items", found_area_item);
//				await dataController.sendListData(in_socket, 5, found_items, "Items", "Load area item", "Load all item in area", "All area item loaded complete");	
			})
			break;
		case "Area NPCs":
			entityExistance.find({"areaObj.areaName": in_param["areaName"], "areaObj.worldObj.worldName": in_param["worldName"]}).select('-areaObj').exec()
			.then(async (found_area_npc) => {
				sendPacket(in_socket, "Area NPCs", found_area_npc);
//				await dataController.sendListData(in_socket, 3, found_items, "objectList", "Load area NPC", "Load all npc in area", "All area npc loaded complete");	
			})
			break;
		case "Area Plants":
			areaPlant.find({"index.areaObj.areaName": in_param["areaName"], "index.areaObj.worldObj.worldName": in_param["worldName"]}).exec()
			.then(async (found_area_plant) => {
				sendPacket(in_socket, "Area Plants", found_area_plant);
//				await dataController.sendListData(in_socket, 2, found_plants, "listOfPlants", "Load area plant", "Load all plants in area", "All area plants loaded complete");	
			})
		break;
	}
}

async function processAction(in_broadcast_socket, in_socket, in_param)
{
	let newParam = {};
    let param = JSON.parse(in_param["data"]);
	switch(in_param["input"])
	{
		case "New day":
			return serverController.newDay(in_broadcast_socket, in_param["worldName"]);
		case "refill water":
			return areaItem.findByIdAndUpdate(in_param["item"]["_id"], {"entityObj": in_param["item"]["entityObj"], "position": in_param["item"]["position"], "rotation": in_param["item"]["rotation"]}).exec();
		case "Pick vegetation":
			return await areaPlant.findOne({"index.areaObj.areaName": param["areaName"], "index.areaObj.worldObj.worldName": in_param["worldName"], "index.x": param["index"]["x"], "index.y": param["index"]["y"], "index.z": param["index"]["z"]}).exec()
			.then(async (get_areaPlant) => {
				return await new Promise(async (resolve, reject) => {						
					newParam["action"] = "Action event";
					if (get_areaPlant["deathDayPassed"] >= get_areaPlant["deathDayRequired"])
					{
						newParam["message"] = "Removing " + get_areaPlant["plantName"];
					} else {
						if (get_areaPlant["dayPassed"] >= get_areaPlant["dayRequired"]){
							newParam["message"] = "Harvesting " + get_areaPlant["plantName"];
						}
					}
					await resolve("Done");
				})

			})
			.then(async () =>{
				sendPacket(in_socket, "Acknowledge", newParam);
//				await getSocket.emit("Action Index", JSON.stringify(new_param).replace(/"/g, "`"));
			})

			break;
		case "Trade":
			return trade(in_socket, in_param);
			break;
		case "Teleport":
			return await area.findOne({"areaName": param["areaName"]}).exec()
			.then(async (getArea) => {
				if (getArea != null) {				
					if (param["x"] > 0 && param["x"] < getArea['length'] &&
						param["y"] > 0 && param["y"] < getArea['width'] && 
						param["z"] >= 0 && param["z"] < getArea['height'])
					{
 						entityExistance.findByIdAndUpdate(param["_id"], {"areaObj": getArea, "position.x": param["x"], "position.y": param["y"], "position.z": param["z"]}).exec();
						sendPacket(in_socket, "Area", getArea);
					} else {
						newParam["message"] = "Teleportation spot out of area field.";
						newParam["action"] = "Error";
						sendPacket(in_socket, "Acknowledge", newParam);
					}
				} else {
						newParam["message"] = "Area does not exist.";
						newParam["action"] = "Error";
						sendPacket(in_socket, "Acknowledge", newParam);					
				}
			})
			break;	
	}
}

async function processItem(in_socket, in_param)
{
	let param = JSON.parse(in_param["data"]);
//	 console.log(param);
	switch(in_param["input"])
	{
		case "Save":		
 			itemExistance.findByIdAndUpdate(param["_id"], {"itemObj": param["ItemObj"]}).exec();
 			break;
		case "Remove":
 			itemExistance.findByIdAndRemove(param["_id"]).exec();
 			break;
	}
}
async function processPlayerItem(in_socket, in_param)
{
	switch(in_param["input"])
	{
		case "Pickup Item":
		    let newParam = {};
		
		    await itemExistance.findOne({"binder.entityName": in_param["entity"], "itemObj.itemName": in_param["item"]}).exec()
		    .then(async (item_found) => {
		        if (item_found) {
		            item_found["itemObj"]["quantity"] += parseInt(in_param["quantity"]);
					sendPacket(in_socket, "Item", item_found);
		            if (item_found["itemObj"]["quantity"] <= 0){
		                item_found.remove();
		            } else {
		                item_found.save();
		            }
		        } else {
		            let getCharacter = characterAccount.findOne({"entityObj.entityName": in_param["entity"]}).exec()
		            let getItem = serverController.itemsDB[in_param["item"]]//itemDatabase.findOne({"itemName": in_param["item"]}).exec()
		            await Promise.all([getCharacter, getItem])
		            .then(async (res) => {
		                newParam["Action"] = "New item";
		                return await new item({"itemName": res[1]["itemName"], "itemType": res[1]["itemType"], maxDurability: res[1]["maxDurability"], durability: res[1]["maxDurability"], capacity: 0, maxCapacity: res[1]["maxCapacity"], quantity: in_param["quantity"]}).save()
		                .then(async (newItem) => {
		                    newParam["Item"] = newItem;             
		                    return await new itemExistance({"binder": res[0]["entityObj"], "itemObj": newItem}).save()
		                })
		                .then(async (newItem) =>{
							sendPacket(in_socket, "Item", newItem);
		                })
		            })
		        }
		    })
			break;
		case "NPC pickup":
			instantiator.npc_getItem(getSocket, in_param["EntityName"], in_param["Item"], in_param["Quantity"], param)
			break;
	}
}

async function processArea(in_socket, in_param)
{

}
async function processStorage(in_socket, in_param)
{
    let data = JSON.parse(in_param["data"]);
    console.log(data);
	switch(in_param["input"])
	{
		case "Access":
			return await itemExistance.find({"storageObj._id": mongoose.Types.ObjectId(data["storageID"])}).exec()
			.then(async (getStorage) => {
				sendPacket(in_socket, "Storage", getStorage);
//				await dataController.sendListData(in_socket, 2, getStorage, "storageList", "Load storage access", "Load all storage item", "All world loaded complete");	
			})
		break;
		case "Pickup":
		    let newParam = {};		
		    await itemExistance.findOne({"storageObj._id": mongoose.Types.ObjectId(data["storage"]), "itemObj.itemName": data["item"]}).exec()
		    .then(async (item_found) => {
		        if (item_found) {
		            item_found["itemObj"]["quantity"] += parseInt(data["quantity"]);
					sendPacket(in_socket, "Item", item_found);
		            if (item_found["itemObj"]["quantity"] <= 0){
		                item_found.remove();
		            } else {
		                item_found.save();
		            }
		        } else {
//		            let getCharacter = characterAccount.findOne({"entityObj.entityName": in_param["entity"]}).exec()
		            let getAreaItem = areaItem.findOne({"entityObj._id": mongoose.Types.ObjectId(data["storage"])}).exec()
		            let getItem = serverController.itemsDB[data["item"]]//itemDatabase.findOne({"itemName": in_param["item"]}).exec()
		            await Promise.all([getAreaItem, getItem])
		            .then(async (res) => {
		                newParam["Action"] = "New item";
		                return await new item({"itemName": res[1]["itemName"], "itemType": res[1]["itemType"], maxDurability: res[1]["maxDurability"], durability: res[1]["maxDurability"], capacity: 0, maxCapacity: res[1]["maxCapacity"], quantity: data["quantity"]}).save()
		                .then(async (newItem) => {
		                    newParam["Item"] = newItem;             
		                    return await new itemExistance({"storageObj": res[0]["entityObj"], "itemObj": newItem}).save()
		                })
		                .then(async (newItem) =>{
							sendPacket(in_socket, "Item", newItem);
		                })
		            })
		        }
		    })
			break;
	}
}

async function trade(in_socket, in_param)
{
	let param = JSON.parse(in_param["data"]);
//	console.log(param);
			let get_trade;
			switch(param["fromType"]){
				case "Storage":
					get_trade = itemExistance.findOne({"_id": mongoose.Types.ObjectId(param["item"]), "storageObj._id": mongoose.Types.ObjectId(param["fromName"])})
					break;
				case "Entity":
					// get_trade = itemExistance.findOne({"_id": mongoose.Types.ObjectId(param["item"]), "binder.entityName": param["fromName"]})
					// break;
				case "NPC":
					get_trade = itemExistance.findOne({"_id": mongoose.Types.ObjectId(param["item"]), "binder._id": mongoose.Types.ObjectId(param["fromName"])})
					break;
			}
			let to_entity;
			switch(param["toType"]){
				case "Storage":
					to_entity = areaItem.findOne({"entityObj._id": mongoose.Types.ObjectId(param["toName"])});
					break;
				case "Entity":
					// to_entity = character.findOne({"entityName": param["toName"]});
					// break;
				case "NPC":
					to_entity = entityExistance.findOne({"entityObj._id": mongoose.Types.ObjectId(param["toName"])});
					break;
			}

			Promise.all([get_trade, to_entity])
			.then(async (res) =>{		
				switch(param["toType"]){
					case "Storage":
						res.push(await itemExistance.findOne({"itemObj.itemName": res[0]["itemObj"]["itemName"], "storageObj._id": mongoose.Types.ObjectId(res[1]["entityObj"]["_id"])}).exec());
						break;
					case "Entity":
					case "NPC":
						res.push(await itemExistance.findOne({"itemObj.itemName": res[0]["itemObj"]["itemName"], "binder._id": mongoose.Types.ObjectId(res[1]["entityObj"]["_id"])}).exec());
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
							res.push(await new itemExistance({"itemObj": temp_item, "storageObj": res[1]["entityObj"]}).save());
						break;
						case "Entity":
							// res.push(await new itemExistance({"entityObj": temp_item, "binder": res[1]}).save());
							// break;
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
				// console.log(res[2]);
				// console.log(res[0]);
				sendPacket(in_socket, "Item", res[2]);
				sendPacket(in_socket, "Item", res[0]);
				// await getSocket.emit("Transfer Item", JSON.stringify(res[2]).replace(/"/g, "`"));
				// await getSocket.emit("Transfer Item", JSON.stringify(res[0]).replace(/"/g, "`"));				
			})
}

async function processWorld(in_broadcast_socket, in_socket, in_param)
{
    let param = JSON.parse(in_param["data"]);
	switch(in_param["input"])
	{
		case "All worlds":
			return await world.find({}).exec()
			.then(async (found_worlds) => {
				sendPacket(in_socket, "World list", found_worlds);
//					await dataController.sendListData(getSocket, 10, getWorlds, "worldList", "Load worlds", "Load all World", "All world loaded complete");	
			})
		case "Create":
			await new world(param).save()
			.then(async (new_world) => {
				return await instantiator.newWorld(in_socket, new_world);
			})
			.then(async (save_world) => {
				return await world.find({}).exec()
			})
			.then(async (getWorlds) => {
				serverController.init();
				sendPacket(in_socket, "World list", getWorlds);
//						await dataController.sendListData(getSocket, 10, getWorlds, "worldList", "Load worlds", "Load all World", "All world loaded complete");	
			})				
			break;
		case "Save":
			serverController.worlds.map(it_world => {
				if (it_world["worldName"] === param["worldName"]){
					it_world["time"] = param["time"];
					var index = serverController.worlds.indexOf(it_world);
					  if (index > -1) {
						    serverController.worlds.splice(index, 1);
					  }
					serverController.worlds.push(it_world);
					sendPacket(in_broadcast_socket, "World", param);
//						socket.emit("Load world", JSON.stringify(in_data).replace(/"/g, "`"));
				}
			});
			break;
	}

}

async function processIndex(in_broadcast_socket, in_socket, in_param)
{
    let param = JSON.parse(in_param["data"]);
	switch(in_param["input"])
	{
		case "Plow":
			areaIndex.findOneAndUpdate({"areaObj.areaName": param["areaObj"]["areaName"], "areaObj.worldObj.worldName": in_param["worldName"], "x": param["x"], "y": param["y"], "z": param["z"]}, {"objectName": param["objectName"], "destructable": param["destructable"], "pickable": param["pickable"], "state": param["state"]}, {upsert:true}).exec()
			sendPacket(in_broadcast_socket, "Index", param);
//			in_broadcast_socket.emit("Area Update", JSON.stringify(param).replace(/"/g, "`"));
		break;
		case "Plant":
			let seed = param["state"].replace("Plant ", "");
			let foundIndex = areaIndex.findOne({"areaObj.areaName": param["areaObj"]["areaName"], "areaObj.worldObj.worldName": in_param["worldName"], "x": param["x"], "y": param["y"], "z": param["z"]});
			let foundPlant = serverController.plantDB[seed]//plantDatabase.findOne({"seedName": seed});

			Promise.all([foundIndex, foundPlant])
			.then(async (res) => {
				new areaPlant({"index": res[0], "plantName": res[1]["seedName"], "state": "Growing", "dayPassed": 0, "dayRequired": res[1]["dayRequired"], "deathDayPassed": 0, "deathDayRequired": res[1]["deathDayRequired"], isWatered: false}).save()
				.then(async (newPlant) => {
					sendPacket(in_socket, "Area Plant", newPlant);
					// newParam["index"] = res[0];
					// newParam["plant"] = newPlant;
					// getSocket.emit("Load Plant", JSON.stringify(newParam).replace(/"/g, "`"));
				})
			})
			break;
		case "Harvest":
			await areaIndex.deleteOne({"areaObj.areaName": param["areaObj"]["areaName"], "areaObj.worldObj.worldName": in_param["worldName"], "x": param["x"], "y": param["y"], "z": param["z"]}).exec()
			.then(async () => {
				return await areaPlant.deleteOne({"index.areaObj.areaName": param["areaObj"]["areaName"], "index.areaObj.worldObj.worldName": in_param["worldName"], "index.x": param["x"], "index.y": param["y"], "index.z": param["z"]}).exec();
			})
			newParam["x"] = param["x"];
			newParam["y"] = param["y"];
			newParam["z"] = param["z"];
			newParam["state"] = "Clear";					
			in_broadcast_socket.emit("Area Update", JSON.stringify(newParam).replace(/"/g, "`"));	
			break;				
		case "Water":
		areaPlant.findOneAndUpdate({"index.areaObj.areaName": param["areaObj"]["areaName"], "index.areaObj.worldObj.worldName": in_param["worldName"], "index.x": param["x"], "index.y": param["y"], "index.z": param["z"]}, {"isWatered": true}, {upsert:true, new:true}).exec()
		.then(async (getPlant) => {
			sendPacket(in_socket, "Area Plant", getPlant);
			// newParam["index"] = getPlant["index"];
			// newParam["plant"] = getPlant;
			// getSocket.emit("Load Plant", JSON.stringify(newParam).replace(/"/g, "`"));
		})
		break;
	}
}

async function processEntity(in_socket, in_param)
{
	let param = JSON.parse(in_param["data"]);
	let get_entity = param["entityObj"];
	switch(in_param["input"])
	{
		case "Player":
			let currentRoom = Object.keys(in_socket.rooms).filter(function(item) {
			    return item !== in_socket.id;
			});

			if (currentRoom != param["areaName"]){
				currentRoom.forEach(async (room) => {
					in_socket.leave(room);
				});
				in_socket.join(param["areaName"]);

			}
			characterAccount.findOneAndUpdate({"entityObj._id": mongoose.Types.ObjectId(get_entity["_id"])}, param).exec()
			break;
	}	
}