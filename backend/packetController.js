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
		case "Main Menu":
			processMainMenu(in_socket, in_param);
			break;
		case "Database":
			processDatabase(in_socket, in_param);
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
			processAction(in_broadcast_socket, in_param);
			break;
		case "Player Item":
			processItem(in_socket, in_param);
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

async function processMainMenu(in_socket, in_param){
	switch(in_param["input"]){
		case "Characters":
			return await character.find({"account": in_param["Username"]}).exec()	
			.then(async (found_characters) => {
				sendPacket(in_socket, "Character list", found_characters);
				//await dataController.sendListData(getSocket, 1, found_characters, "characterList", "Load characters", "Load all Characters", "All characters complete");	
			})
			break;
		case "Worlds":
			return await world.find({}).exec()
			.then(async (found_worlds) => {
				sendPacket(in_socket, "World list", found_worlds);
//					await dataController.sendListData(getSocket, 10, getWorlds, "worldList", "Load worlds", "Load all World", "All world loaded complete");	
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

async function processPreload(in_socket, in_param){
	newParam = {};
	area.findOne({"areaName": in_param["entity"] + "_farm", "worldObj.worldName": in_param["worldName"]})
	.then(async (getFarm) => {
		if (getFarm == null){
			console.log("Farm generated");
			world.findOne({"worldName": in_param["worldName"]}).exec()
			.then(async (get_world) =>{
				return await instantiator.generatePlayerFarm(in_socket, get_world, in_param["entity"]);
			})
		}
	})
	.then(async () =>{
		sendPacket(in_socket, "Acknowledge", "Farm generated");
	})
}

async function processPlayer(in_socket, in_param){
	switch(in_param["input"]){				
		case "Items":
			return await itemExistance.find({"binder.entityName": in_param["entity"]}).select('-binder').exec()	
			.then(async (found_items) => {
				sendPacket(in_socket, "Item list", found_items);
			})	
		break;	
	}
}

async function processLoad(in_socket, in_param)
{
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

async function processAction(in_broadcast_socket, in_param)
{
	switch(in_param["input"])
	{
		case "New day":
			return serverController.newDay(in_broadcast_socket, in_param["worldName"]);
		case "refill water":
			return areaItem.findByIdAndUpdate(in_param["item"]["_id"], {"itemObj": in_param["item"]["itemObj"], "position": in_param["item"]["position"], "rotation": in_param["item"]["rotation"]}).exec();
	}
}

async function processItem(in_socket, in_param)
{
	switch(in_param["input"])
	{
		case "Pickup Item":
		    let newParam = {};
		
		    await itemExistance.findOne({"binder.entityName": in_param["entity"], "itemObj.itemName": in_param["item"]}).exec()
		    .then(async (item_found) => {
		        if (item_found) {
		            item_found["itemObj"]["quantity"] += parseInt(in_param["quantity"]);
		            if (item_found["itemObj"]["quantity"] <= 0){
		                item_found.remove();
		            } else {
		                item_found.save();
		            }
		        } else {
		            let getCharacter = character.findOne({"entityName": in_param["entity"]}).exec()
		            let getItem = itemDatabase.findOne({"itemName": in_param["item"]}).exec()
		            await Promise.all([getCharacter, getItem])
		            .then(async (res) => {
		                newParam["Action"] = "New item";
		                return await new item({"itemName": res[1]["itemName"], "itemType": res[1]["itemType"], maxDurability: res[1]["maxDurability"], durability: res[1]["maxDurability"], capacity: 0, maxCapacity: res[1]["maxCapacity"], quantity: in_param["quantity"]}).save()
		                .then(async (newItem) => {
		                    newParam["Item"] = newItem;             
		                    return await new itemExistance({"binder": res[0], "itemObj": newItem}).save()
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