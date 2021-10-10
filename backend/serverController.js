const models = require('../Model/gameModel'),
	serverController = require('./serverController'),
	mongoose = require('mongoose'),
	character = mongoose.model('Character'),
	areaItem = mongoose.model('AreaItem'),
	plantDatabase = mongoose.model('PlantDatabase'),
	itemExistance = mongoose.model('ItemExistance'),
	instantiator = require('../Controller/instantiateController'),
	areaPlant = mongoose.model('AreaPlant'),
	world = mongoose.model('World');

worlds = []
refills = []
storages = {}

exports.worlds = worlds;
exports.refills = refills;
exports.storages = storages;



setInterval(function(){
	if (worlds.length != 0){
		worlds.forEach(async (it_world) =>{
			try{
				it_world["time"] += (1/it_world["fullDayLength"]);
				if (it_world["time"] > 1) it_world["time"] = 0;

				if (it_world["time"] >= ((it_world["dayEndHour"]*60)/it_world["fullDayLength"])){
					dayEnd(it_world);
				}

				if (it_world["time"] >= ((it_world["dayBeginHour"]*60)/it_world["fullDayLength"]) &&
					it_world["time"] < ((it_world["dayEndHour"]*60)/it_world["fullDayLength"])){
					dayBegin(it_world);
				}

				it_world.save();		
			} catch (error) {
				var index = worlds.indexOf(it_world);
				  if (index > -1) {
					    serverController.worlds.splice(index, 1);
				  }
			}
		})
	}
}, 1000);

exports.init = async function(){
	await world.find({}).exec()
	.then(async (found_world) => {
		found_world.forEach(async (it_world) => {
			worlds.push(it_world);
		})
	})
}

async function dayEnd(it_world){
	let plantUpdates = {};
	if (!it_world["isDayEnd"]){
		it_world["isDayEnd"] = true;
		console.log("Day End has begun");
		return await areaPlant.find({"index.areaObj.worldObj.worldName": it_world["worldName"]}).exec()
		.then(async (found_plant) => {
			return new Promise(async (resolve, reject) => {
				found_plant.forEach(async (it_plant) => {
					if (it_plant["isWatered"]){
						it_plant["dayPassed"] += 1;
					} else {
						it_plant["deathDayPassed"] += 1;
					}
					it_plant["isWatered"] = false;
					it_plant.save();
				})
				await resolve("Done");				
			})
		})
		 .then(async () => {
		 	return await areaItem.find({"itemObj.itemType": "Shipping Bin"}).exec()
		 })
		 .then(async (listOfStorages) =>{
		 	return new Promise(async (resolve, reject) => {		 		
			 	listOfStorages.forEach(async (it_storage) => {
			 		let silver_made = 0;
			 		await itemExistance.find({"storageObj._id": mongoose.Types.ObjectId(it_storage["itemObj"]["_id"])}).exec()
			 		.then(async (listOfItems) => {
			 			return new Promise(async (resolve, reject) => {
			 				listOfItems.forEach(async (it_item) => {
				 				//Implement market database lookup here
				 				silver_made += it_item['itemObj']['quantity'] * 10;
				 				it_item.remove();
			 				})
			 				await resolve("Done");
			 			})
			 			.then(async () => {
			 				return await areaItem.findOne({"itemObj.itemType": "Mailbox", "areaObj._id": mongoose.Types.ObjectId(it_storage["areaObj"]["_id"])}).exec()
			 			})
			 		})
			 		.then(async (getMail) => {
			 			if (silver_made > 0 ){	
				 			instantiator.storage_getItem(getMail["itemObj"]["itemName"], "Silver", silver_made);		 				
			 			}
			 		})
			 	})
			 	await resolve("done");
		 	})
		 })
		 .then(async () => {
		 	return await areaItem.find({"areaObj.areaName": "Central Hub", "areaObj.worldObj.worldName": it_world["worldName"], "itemObj.itemType": "Door"}).exec()
		 })
		 .then(async (listOfDoors) => {
		 	listOfDoors.forEach(async (it_door) => {
		 		it_door["itemObj"]["state"] = "Closed";
		 		it_door.save();
		 	})
		 })

		// 	let getRecepient = itemExistance.find({"areaObj.worldObj.worldName": it_world["worldName"], storageObj: {$ne: null}, "itemobj.itemName": "Silver"});

		// 	return Promise.all([getStorage, getRecepient]).exec();
		// })
		// .then(async (found_data) => {
		// 	if (found_data[0].length > 0){		
		// 		if (found_data[1] == null) {
		// 			instantiator.createNewStorageItem("Silver", 0, res[0])
		// 		}
		// 		for (let found_data[0] = 0; found_data[0] < found_storage.length ; found_data[0]++){
		// 			found_storage[found_data[0]] 
		// 		}
		// 	}
		// })
	}
}

async function dayBegin(it_world){
	if (it_world["isDayEnd"]){
		it_world["isDayEnd"] = false;

	 	return await areaItem.find({"areaObj.areaName": "Central Hub", "areaObj.worldObj.worldName": it_world["worldName"]}).exec()
		 .then(async (listOfDoors) => {
		 	listOfDoors.forEach(async (it_door) => {
		 		it_door["itemObj"]["state"] = "Open";
		 		it_door.save();
		 	})
		 })
		console.log("Day Begin has begun");
	}	
}

exports.newDay = async function(in_socket, in_world){
	payload = {}
		worlds.forEach(async (it_world) =>{
			if (it_world["worldName"] === in_world){
				payload["action"] = "Old day end";
				await dayEnd(it_world);
				sendPacket(in_socket, "Acknowledge", payload);
				setTimeout(async () => { 
					payload["action"] = "New day begin";
					await dayBegin(it_world);
				sendPacket(in_socket, "Acknowledge", payload);
				}, 1000);
			}
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