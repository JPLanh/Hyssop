'use strict';
var mongoose = require('mongoose'),
	Schema = mongoose.Schema,
	passportLocalMongoose = require('passport-local-mongoose');

var user = new Schema({
	username: {type: String},
	active: {type: Boolean},
	key: 
	{
		type: String,
		select: false
	},
	iv: 
	{
		type: String,
		select: false
	},
	password: 
	{
		type: String, 
		select: false
	},
});

var world = new Schema({
	worldName: {type: String},
	time: {type: Number},
	timeStopped: {type: Boolean},
	fullDayLength: {type: Number},
	owner: {type: String},
	isDayEnd: {type: Boolean},
	dayEndHour: {type: Number},
	dayEndMinute: {type: Number},
	dayBeginHour: {type: Number}
})

var item = new Schema({
	binder: {type: String},
	itemName: {type: String},
	itemType: {type: String},
	state: {type: String},
	quality: {type: String},
	quantity: {type: Number},
	durability: {type: Number},
	maxDurability: {type: Number},
	capacity: {type: Number},
	maxCapacity: {type: Number},
	itemIndex: {type: Number},
});

var area = new Schema({
	worldObj: world,
	areaName:{type: String},
	length: {type: Number},
	width: {type: Number},
	height: {type: Number},	
	buildable: {type: Boolean}
});

var itemMarket = new Schema({
	buyPrice: {type: Number},
	sellPrice: {type: Number},
	priceRoof: {type: Number},
	itemRoof: {type: Number}
});

var plantDatabase = new Schema({
	plantName: {type: String},
	seedName: {type: String},
	dayRequired: {type: Number},
	deathDayRequired: {type: Number}
});

var itemDatabase = new Schema({
	itemName: {type: String},
	itemType: {type: String},
	maxDurability: {type: Number},
	maxCapacity: {type: Number},
});

var normalized = new Schema({
	x: {type: Number},
	y: {type: Number},
	z: {type: Number},
	magnitude: {type: Number},
	sqrMagnitude: {type: Number}
})
	
var position = new Schema({
	x: {type: Number},
	y: {type: Number},
	z: {type: Number}
//	magnitude: {type: Number},
//	sqrMagnitude: {type: Number},
//	normalized: normalized
})

var eulerAngles = new Schema({
	x: {type: Number},
	y: {type: Number},
	z: {type: Number}
//	magnitude: {type: Number},
//	sqrMagnitude: {type: Number},
//	normalized: normalized

})

var rotation = new Schema({
	x: {type: Number},
	y: {type: Number},
	z: {type: Number},	
	w: {type: Number}
	// magnitude: {type: Number},
	// sqrMagnitude: {type: Number},
	// eulerAngles: eulerAngles
})

var backpack = new Schema({
	size: {type: Number},
	indexSelected: {type: Number}
})

var character = new Schema({
	account: {type: String},
	entityName: {type: String},
	areaName: {type: String},
	stamina: {type: Number},
	world: {type: String},
	maxStamina: {type: Number},
	state: {type: String},	
	position: position,
	rotation: rotation,
	holding: {type: String},
	backpack: backpack,
	currentAnimal: {type: String},
	primary_currentBlue : {type: Number},
	primary_currentGreen : {type: Number},
	primary_currentRed : {type: Number},
	secondary_currentBlue : {type: Number},
	secondary_currentGreen : {type: Number},
	secondary_currentRed : {type: Number}
});

var entity = new Schema({
	entityName: {type: String},
	stamina: {type: Number},
	maxStamina: {type: Number},
	state: {type: String},	
	currentAnimal: {type: String},
	backpack: backpack,
	primary_currentBlue : {type: Number},
	primary_currentGreen : {type: Number},
	primary_currentRed : {type: Number},
	secondary_currentBlue : {type: Number},
	secondary_currentGreen : {type: Number},
	secondary_currentRed : {type: Number}	
});

var characterAccount = new Schema({
	entityObj : entity,
	account : {type: String},
	areaObj: area,	
	position: position,
	rotation: rotation
})

var entityExistance = new Schema({
	entityObj: entity,
	areaObj: area,	
	position: position,
	rotation: rotation
});

var areaItem = new Schema({
	entityObj: item,
	areaObj: area,
	position: position,
	rotation: rotation
});

var areaIndex = new Schema({
	areaObj: area,
	x: {type: Number},
	y: {type: Number},
	z: {type: Number},
	objectName: {type: String},
	destructable: {type: Boolean},
	pickable: {type: Boolean},
	state: {type: String}
});

var areaPlant = new Schema({
	index: areaIndex,
	plantName: {type: String},
	state: {type: String},
	dayPassed: {type: Number},
	dayRequired: {type: Number},
	deathDayPassed: {type: Number},
	deathDayRequired: {type: Number},	
	isWatered: {type: Boolean}
})

var storage = new Schema({
	storageName: {type: String},
	storageType: {type: String},
	state: {type: String},
	position: position,
	rotation: rotation
})

var itemExistance = new Schema({
	binder: entity,
	itemObj: item,
	itemMarketObj: itemMarket,
	storageObj: item,
	areaObj: area
});

var cart = new Schema({
	fromObj: item,
	toObj: item
})


user.plugin(passportLocalMongoose);

module.exports = mongoose.model("Item", item);
module.exports = mongoose.model("AreaItem", areaItem);
module.exports = mongoose.model("Position", position);
module.exports = mongoose.model("Rotation", rotation);
module.exports = mongoose.model("Area", area);
module.exports = mongoose.model("Entity", entity);
module.exports = mongoose.model("EntityExistance", entityExistance);
module.exports = mongoose.model("ItemMarket", itemMarket);
module.exports = mongoose.model("PlantDatabase", plantDatabase);
module.exports = mongoose.model("ItemDatabase", itemDatabase);
module.exports = mongoose.model("AreaIndex", areaIndex);
module.exports = mongoose.model("AreaPlant", areaPlant);
module.exports = mongoose.model("Storage", storage);
module.exports = mongoose.model("ItemExistance", itemExistance);
module.exports = mongoose.model("User", user);
module.exports = mongoose.model("World", world);
module.exports = mongoose.model("Character", character);
module.exports = mongoose.model("Cart", cart);
module.exports = mongoose.model("CharacterAccount", characterAccount);