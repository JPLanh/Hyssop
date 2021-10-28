const models = require('../Model/gameModel'),
	mongoose = require('mongoose'),
    instantiator = require('./instantiateController'),
	serverController = require('./serverController'),
	item = mongoose.model('Item'),
	areaItem = mongoose.model('AreaItem'),
	area = mongoose.model('Area'),
	itemMarket = mongoose.model('ItemMarket'),
	plantDatabase = mongoose.model('PlantDatabase'),
	itemDatabase = mongoose.model('ItemDatabase'),
	areaIndex = mongoose.model('AreaIndex'),
	areaPlant = mongoose.model('AreaPlant'),
	position = mongoose.model('Position'),
	rotation = mongoose.model('Rotation'),
	storage = mongoose.model('Storage'),
	itemExistance = mongoose.model('ItemExistance'),
	user = mongoose.model('User'),
	cart = mongoose.model('Cart'),
    entity = mongoose.model("Entity"),
    entityExistance = mongoose.model("EntityExistance"),
    characterAccount = mongoose.model("CharacterAccount"),
	character = mongoose.model('Character'),
    world = mongoose.model('World'),
	currentTime = Date.now();

exports.createNewAreaItem = async function(in_socket, in_entityName, in_itemName, in_quantity, in_area, in_xPos, in_yPos, in_zPos, in_xRot, in_yRot, in_zRot, in_wRot, in_state)
{
	return await itemDatabase.findOne({"itemName": in_itemName}).exec()
	.then(async (res) =>
	{
		return await new item({"binder": in_entityName + "_farm", "itemName": res["itemName"], "itemType": res["itemType"], maxDurability: res["maxDurability"], maxCapacity: res["maxCapacity"], quantity: in_quantity, "state": in_state});
	})			
	.then(async (res) =>
	{
		res["quantity"] = in_quantity;
		return await new areaItem({"entityObj": res, "areaObj": in_area, "position": new position({"x": in_xPos, "y": in_yPos, "z": in_zPos}), "rotation": new rotation({"x": in_xRot, "y": in_yRot, "z": in_zRot, "w": in_wRot})}).save();
	})
	.then(async (getNewAreaItem) =>
	{		
		in_socket.emit("Load new item", getNewAreaItem);
		return await getNewAreaItem;							
	})
}

exports.createNewNPC = async function(in_param, in_area, in_pos){
    return await new entity(in_param).save()
    .then(async (new_entity) => {
        return await new entityExistance({"entityObj": new_entity, "position": in_pos["position"], "rotation": in_pos["rotation"], "areaObj": in_area}).save();
    })
}

exports.storage_getItem = async function(in_storageName, in_itemName, in_quantity){
    await itemExistance.findOne({"storageObj.itemName": in_storageName, "entityObj.itemName": in_itemName}).exec()
    .then(async (item_found) => {
        if (item_found) {
            item_found["entityObj"]["quantity"] += parseInt(in_quantity);
            if (item_found["entityObj"]["quantity"] <= 0){
                item_found.remove();
            } else {
                item_found.save();
            }
        } else {
            let getStorage = areaItem.findOne({"entityObj.itemName": in_storageName}).exec()
            let getItem = itemDatabase.findOne({"itemName": in_itemName}).exec()
            await Promise.all([getStorage, getItem])
            .then(async (res) => {
                return await new item({"itemName": res[1]["itemName"], "itemType": res[1]["itemType"], maxDurability: res[1]["maxDurability"], durability: res[1]["maxDurability"], capacity: 0, maxCapacity: res[1]["maxCapacity"], quantity: in_quantity}).save()
                .then(async (newItem) => {
                    return await new itemExistance({"storageObj": res[0]["entityObj"], "entityObj": newItem}).save()
                })
            })
        }
    })
}

exports.createNewCharacter = async function(in_character, in_position, in_rotation, in_account)
{
    in_character["_id"] = mongoose.Types.ObjectId();
    let new_entity = await new entity(in_character);
	return await new characterAccount({"entityObj": new_entity, "position": in_position, "rotation": in_rotation, "account": in_account}).save()
}

exports.createNewGrid = async function(in_length, in_width, in_height, in_name, in_buildable, in_world){
	return await new area({"areaName": in_name, "length": in_length+1, "width": in_width+1, "height": in_height, "buildable": in_buildable, "worldObj": in_world}).save();
}

async function generateFarmParameters(in_area)
{
	console.log("Generating parameters");
    return new Promise(async (resolve, reject) => {

			for( let length = 0; length < in_area["length"]; length++)
			{
				if (length == 0 || length == in_area["length"]-1)
				{
					for (let width = 0; width < in_area["width"]; width++)
					{					
						if (length == 0 && (width == 10 || width == 11))
						{
								new areaIndex({"areaObj": in_area, "x": length, "y": width, "z": 0, "objectName": "Area Connector", "destructable": false, "pickable": false, "state": "Teleport 49 " + (width - 5) + " 0 Central Hub"}).save();
						} else {
								new areaIndex({"areaObj": in_area, "x": length, "y": width, "z": 0, "objectName": "Wooden Fence", "destructable": false, "pickable": false, "state": ""}).save();
								new areaIndex({"areaObj": in_area, "x": length, "y": width, "z": 1, "objectName": "Wooden Fence", "destructable": false, "pickable": false, "state": ""}).save();
						}				
					}					
				} else {
								new areaIndex({"areaObj": in_area, "x": length, "y": 0, "z": 0, "objectName": "Wooden Fence", "destructable": false, "pickable": false, "state": ""}).save();
								new areaIndex({"areaObj": in_area, "x": length, "y": 0, "z": 1, "objectName": "Wooden Fence", "destructable": false, "pickable": false, "state": ""}).save();					

								new areaIndex({"areaObj": in_area, "x": length, "y": in_area["width"]-1, "z": 0, "objectName": "Wooden Fence", "destructable": false, "pickable": false, "state": ""}).save();
								new areaIndex({"areaObj": in_area, "x": length, "y": in_area["width"]-1, "z": 1, "objectName": "Wooden Fence", "destructable": false, "pickable": false, "state": ""}).save();					
				}
			}
		await resolve(in_area);
    })
}

async function generateFarmItems(in_socket, in_name, in_area)
{
    return new Promise(async (resolve, reject) => {

		instantiator.createNewAreaItem(in_socket, in_name, "Torch", 1, in_area, 18, 5.1, 7, 90, 0, 0, 0, null);	
		await instantiator.createNewAreaItem(in_socket, in_name, "Basic Well", 2, in_area, 7, 0, 6, 0, 0, 0, 0, null)
		.then(async (instWell) => {
			instWell["entityObj.capacity"] = 50;
			instWell["entityObj.maxCapacity"] = 100;
			instWell.save();
			serverController.refills.push(instWell);			
		})
		let instStorage = await instantiator.createNewAreaItem(in_socket, in_name, "Basic Shipping Bin", 0, in_area, 3, 0, 13, 0, 0, 0, 0, null)
		let instMail = await instantiator.createNewAreaItem(in_socket, in_name, "Basic Mailbox", 0, in_area, 3, 0, 8, 0, 0, 0, 0, null)
		instantiator.createNewAreaItem(in_socket, in_name, "Basic Bed", 0, in_area, 22, 0, 5, 0, 0, 0, 0, null);
		instantiator.createNewAreaItem(in_socket, in_name, "Wooden Door", 0, in_area, 13, 1, 11, 0, 0, 0, 0, "Open");



		new cart({"fromObj": instStorage, "toObj": instMail}).save();
		serverController.storages[instStorage] = instMail;

        resolve(await in_area);
	})

}

async function generateFarmHouse(in_area, x_start, y_start, z_start, x_size, y_size, z_size)
    {
        return await new Promise(async (resolve, reject) => {            
            for (let x = x_start; x <= x_start + x_size; x++)
            {
                for (let y = y_start; y <= y_start + y_size; y++)
                {
                    for (let z = z_start; z <= z_start + z_size; z++)
                    {
                        let notAWall = false;
                        if (x == x_start || x == x_start + x_size || y == y_start || y == y_start + y_size)
                        {
                        	if (x == x_start && (y == y_start + y_size - 2 || y == y_start + y_size - 3 ) && z < 3){
                        		//create item
                        		notAWall = true;
                        	}                    	
                        	if (!notAWall){                    		
    								new areaIndex({"areaObj": in_area, "x": x, "y": y, "z": z, "objectName": "Wooden Wall", "destructable": false, "pickable": false, "state": ""}).save();
                        	}
                        }
                        else
                        {

                            //floor
                            if (z == z_start)
                            	new areaIndex({"areaObj": in_area, "x": x, "y": y, "z": 0, "objectName": "Wooden Floor", "destructable": false, "pickable": false, "state": ""}).save();
                            if (z == z_start + z_size)
                            	new areaIndex({"areaObj": in_area, "x": x, "y": y, "z": z, "objectName": "Wooden Wall", "destructable": false, "pickable": false, "state": ""}).save();

                        }
                    }
                }
            }
            resolve(await in_area);
        })

    }

//async function generateCentralHubNPC()
exports.generateCentralHubNPC = async function(in_area)
{
    trevik = {};
    pos = {};
    trevik['entityName'] = "Trevik";
    trevik["stamina"] = 100;
    trevik["maxStamina"] = 100;
    trevik["state"] = "";
    pos["position"] = new position({"x": 5, "y": 0, "z": 13});
    pos["rotation"] = new rotation({"x": 90, "y": 0, "z": 0, "w": 0});
    trevik["currentAnimal"] = "Fox";
    trevik["primary_currentBlue"] = 32;
    trevik["primary_currentRed"] = 84;
    trevik["primary_currentGreen"] = 114;
    trevik["secondary_currentBlue"] = 52;
    trevik["secondary_currentRed"] = 125;
    trevik["secondary_currentGreen"] = 25;
    return await instantiator.createNewNPC(trevik, in_area, pos)
    .then(async () => {
        trevik = {};
        await instantiator.npc_getItem("Trevik", "Strawberry seed", 25, 10, 10, 30, 250);
        await instantiator.npc_getItem("Trevik", "Grape seed", 25, 10, 10, 30, 250);
        await instantiator.npc_getItem("Trevik", "Coffee bean", 25, 10, 10, 30, 250);
        await instantiator.npc_getItem("Trevik", "Corn seed", 25, 10, 10, 30, 250);
        await instantiator.npc_getItem("Trevik", "Wheat seed", 25, 10, 10, 30, 250);
        await instantiator.npc_getItem("Trevik", "Carrot seed", 25, 10, 10, 30, 250);
        await instantiator.npc_getItem("Trevik", "Potato tuber", 25, 10, 10, 30, 250);
        await instantiator.npc_getItem("Trevik", "Lettuce seed", 25, 10, 10, 30, 250);
        await instantiator.npc_getItem("Trevik", "Cabbage seed", 25, 10, 10, 30, 250);
        await instantiator.npc_getItem("Trevik", "Tomato seed", 25, 10, 10, 30, 250);
        await instantiator.npc_getItem("Trevik", "Green onion bulb", 25, 10, 10, 30, 250);
        await instantiator.npc_getItem("Trevik", "Garlic clove", 25, 10, 10, 30, 250);
        await instantiator.npc_getItem("Trevik", "Rice seedling", 25, 10, 10, 30, 250);
    })
    .then(async () => {
        Izak = {};
        pos = {};
        Izak['entityName'] = "Izak";
        Izak["stamina"] = 100;
        Izak["maxStamina"] = 100;
        Izak["state"] = "";
        pos["position"] = new position({"x": 42, "y": 0, "z": 17});
        pos["rotation"] = new rotation({"x": 180, "y": 0, "z": 0, "w": 0});
        Izak["currentAnimal"] = "Cat";
        Izak["primary_currentBlue"] = 122;
        Izak["primary_currentRed"] = 122;
        Izak["primary_currentGreen"] = 28;
        Izak["secondary_currentBlue"] = 0;
        Izak["secondary_currentRed"] = 75;
        Izak["secondary_currentGreen"] = 90;
        return await instantiator.createNewNPC(Izak, in_area, pos)
    })
}

async function generateCentralHubParameter (in_area, in_object)
{
    console.log("Generating Central Hub");
    for( let length = 0; length < in_area["length"]; length++)
    {
        if (length == 0 || length == in_area["length"]-1)
        {
            for (let width = 0; width < in_area["width"]; width++)
            {                   
                if (length == 50 && (width == 5 || width == 6))
                {
                    new areaIndex({"areaObj": in_area, "x": length, "y": width, "z": 0, "objectName": "Area Connector", "destructable": false, "pickable": false, "state": "Teleport 1 " + (width + 5) + " 0 (Player Farm)"}).save();
                } else {
                    new areaIndex({"areaObj": in_area, "x": length, "y": width, "z": 0, "objectName": in_object, "destructable": false, "pickable": false, "state": ""}).save();
                    new areaIndex({"areaObj": in_area, "x": length, "y": width, "z": 1, "objectName": in_object, "destructable": false, "pickable": false, "state": ""}).save();
                }               
            }                   
        } else {
            new areaIndex({"areaObj": in_area, "x": length, "y": 0, "z": 0, "objectName": in_object, "destructable": false, "pickable": false, "state": ""}).save();
            new areaIndex({"areaObj": in_area, "x": length, "y": 0, "z": 1, "objectName": in_object, "destructable": false, "pickable": false, "state": ""}).save();                  

            new areaIndex({"areaObj": in_area, "x": length, "y": in_area["width"]-1, "z": 0, "objectName": in_object, "destructable": false, "pickable": false, "state": ""}).save();
            new areaIndex({"areaObj": in_area, "x": length, "y": in_area["width"]-1, "z": 1, "objectName": in_object, "destructable": false, "pickable": false, "state": ""}).save();                 
        }
    }
    return await in_area;
}

async function generateCentralHubPath(in_area, in_object)
{
    return new Promise(async (resolve, reject) => 
    {

        for (let x = 49; x >= 28; x--)
        {                               
            new areaIndex({"areaObj": in_area, "x": x, "y": 5, "z": 0, "objectName": in_object, "destructable": false, "pickable": false, "state": ""}).save();
            new areaIndex({"areaObj": in_area, "x": x, "y": 6, "z": 0, "objectName": in_object, "destructable": false, "pickable": false, "state": ""}).save();
        }
        for (let y = 24; y >= 11; y--)
        {
            new areaIndex({"areaObj": in_area, "x": 28, "y": y, "z": 0, "objectName": in_object, "destructable": false, "pickable": false, "state": ""}).save();
            new areaIndex({"areaObj": in_area, "x": 29, "y": y, "z": 0, "objectName": in_object, "destructable": false, "pickable": false, "state": ""}).save();
        }
        for (let y = 10; y >= 2; y--)
        {
            new areaIndex({"areaObj": in_area, "x": 28, "y": y, "z": 0, "objectName": in_object, "destructable": false, "pickable": false, "state": ""}).save();
            new areaIndex({"areaObj": in_area, "x": 29, "y": y, "z": 0, "objectName": in_object, "destructable": false, "pickable": false, "state": ""}).save();
        }
        for (let x = 22; x <= 27; x++)
        {
            new areaIndex({"areaObj": in_area, "x": x, "y": 13, "z": 0, "objectName": in_object, "destructable": false, "pickable": false, "state": ""}).save();
            new areaIndex({"areaObj": in_area, "x": x, "y": 14, "z": 0, "objectName": in_object, "destructable": false, "pickable": false, "state": ""}).save();
        }
        await resolve(in_area);
    })
}

async function generateBasicShop(in_socket, in_area, x_start, y_start, z_start, x_size, y_size, z_size, direction)
{
    return new Promise(async (resolve, reject) => {        
        for (let x = x_start; x <= x_start + x_size; x++)
        {
            for (let y = y_start; y <= y_start + y_size; y++)
            {
                for (let z = z_start; z <= z_start + z_size; z++)
                {
                    let notAWall = false;
                    if (x == x_start || x == x_start + x_size || y == y_start || y == y_start + y_size)
                    {
                        switch(direction){
                            case "North":
                                if ((x == x_start + x_size && y == y_start + parseInt(y_size/2) && z < z_start + 3) ||
                                    (x == x_start + x_size && y == y_start + parseInt(y_size/2)+1 && z < z_start + 3))
                                {    
                                    notAWall = true;
                                    if (x == x_start + x_size && y == y_start + parseInt(y_size/2)+1 && z < z_start + 1){
                                        await instantiator.createNewAreaItem(in_socket, "Central Hub", "Wooden Door", 0, in_area, 22, 1, 14, 0, 0, 0, 0, "Open")
                                    }
                                }
                                if (x == x_start + x_size && (y > y_start + 2 && y < y_start + 9) && (z == z_start + 1 || z == z_start + 2) ||
                                    (x == x_start + x_size && (y < y_start + y_size - 2 && y > y_start + y_size - 8) && (z == z_start + 1 || z == z_start + 2)))
                                {
                                    notAWall = true;
                                    new areaIndex({"areaObj": in_area, "x": x, "y": y, "z": z, "objectName": "Glass", "destructable": false, "pickable": false, "state": ""}).save();                                
                                }
//                                instantiator.createNewAreaItem(in_socket, "Central Hub", "Torch", 1, in_area, (x_start + x_size)/2, z_size + .1, (y_start + y_size)/2, 90, 0, 0, 0, null);    
                                break;
                            case "East":                            
                                if ((x == x_start + parseInt(x_size/2) && y == y_start  && z < z_start + 3) ||
                                    (x == x_start + parseInt(x_size/2) + 1 && y == y_start && z < z_start + 3))
                                {    
                                    notAWall = true;
                                    if (x == x_start + parseInt(x_size/2) && y == y_start && z < z_start + 1){
                                        await instantiator.createNewAreaItem(in_socket, "Central Hub", "Wooden Door", 0, in_area, 42, 1, 9, 0, 0, 0, 0, "Open")
                                    }
                                }
                                if (x == x_start && (y > y_start + 2 && y < y_start + 9) && (z == z_start + 1 || z == z_start + 2) ||
                                    (x == x_start && (y < y_start + y_size - 2 && y > y_start + y_size - 8) && (z == z_start + 1 || z == z_start + 2)))
                                {
                                    notAWall = true;
                                    new areaIndex({"areaObj": in_area, "x": x, "y": y, "z": z, "objectName": "Glass", "destructable": false, "pickable": false, "state": ""}).save();                                
                                }
                                break;

                            }
                            if (!notAWall)
                                new areaIndex({"areaObj": in_area, "x": x, "y": y, "z": z, "objectName": "Wooden Wall", "destructable": false, "pickable": false, "state": ""}).save();

                    }
                    else
                    {

                        //floor
                        if (z == z_start)
                            new areaIndex({"areaObj": in_area, "x": x, "y": y, "z": 0, "objectName": "Wooden Floor", "destructable": false, "pickable": false, "state": ""}).save();
                        if (z == z_start + z_size)
                            new areaIndex({"areaObj": in_area, "x": x, "y": y, "z": z, "objectName": "Wooden Wall", "destructable": false, "pickable": false, "state": ""}).save();

                    }
                }
            }
        }

                instantiator.createNewAreaItem(in_socket, "Central Hub", "Torch", 1, in_area, (x_start + x_size/2), z_size + .1, (y_start + y_size/2), 90, 0, 0, 0, null);    
        // console.log("Torch: " + x_start + " , "+ x_size + " / " + y_start + " , " + y_size + " / " + (x_start + x_size)/2 + " / " + (y_start + y_size)/2)
        // instantiator.createNewAreaItem(in_socket, "Central Hub", "Torch", 1, in_area, (x_start + x_size)/2, z_size + .1, (y_start + y_size)/2, 90, 0, 0, 0, null);    

        await resolve(in_area);
    })
}

async function generateBasicShopInterior(in_area)
{
        for (let x = 5; x <= 19; x++)
        {
            new areaIndex({"areaObj": in_area, "x": x, "y": 3, "z": 0, "objectName": "Wooden Wall", "destructable": false, "pickable": false, "state": ""}).save();
            new areaIndex({"areaObj": in_area, "x": x, "y": 23, "z": 0, "objectName": "Wooden Wall", "destructable": false, "pickable": false, "state": ""}).save();
        }

        for (let y = 5; y <= 10; y++)
        {
            new areaIndex({"areaObj": in_area, "x": 21, "y": y, "z": 0, "objectName": "Wooden Wall", "destructable": false, "pickable": false, "state": ""}).save();
            new areaIndex({"areaObj": in_area, "x": 3, "y": y, "z": 0, "objectName": "Wooden Wall", "destructable": false, "pickable": false, "state": ""}).save();
        }

        for (let y = 17; y <= 21; y++)
        {
            new areaIndex({"areaObj": in_area, "x": 21, "y": y, "z": 0, "objectName": "Wooden Wall", "destructable": false, "pickable": false, "state": ""}).save();
            new areaIndex({"areaObj": in_area, "x": 3, "y": y, "z": 0, "objectName": "Wooden Wall", "destructable": false, "pickable": false, "state": ""}).save();
        }

        for (let y = 7; y <= 11; y++)
        {
            new areaIndex({"areaObj": in_area, "x": 16, "y": y, "z": 0, "objectName": "Wooden Wall", "destructable": false, "pickable": false, "state": ""}).save();
            new areaIndex({"areaObj": in_area, "x": 11, "y": y, "z": 0, "objectName": "Wooden Wall", "destructable": false, "pickable": false, "state": ""}).save();
        }

        for (let y = 16; y <= 19; y++)
        {
            new areaIndex({"areaObj": in_area, "x": 16, "y": y, "z": 0, "objectName": "Wooden Wall", "destructable": false, "pickable": false, "state": ""}).save();
            new areaIndex({"areaObj": in_area, "x": 11, "y": y, "z": 0, "objectName": "Wooden Wall", "destructable": false, "pickable": false, "state": ""}).save();
        }

        for (let x = 3; x <= 6; x++)
        {
            new areaIndex({"areaObj": in_area, "x": x, "y": 11, "z": 0, "objectName": "Wooden Wall", "destructable": false, "pickable": false, "state": ""}).save();
            new areaIndex({"areaObj": in_area, "x": x, "y": 16, "z": 0, "objectName": "Wooden Wall", "destructable": false, "pickable": false, "state": ""}).save();
        }

        for (let y = 12; y <= 15; y++)
        {
            new areaIndex({"areaObj": in_area, "x": 6, "y": y, "z": 0, "objectName": "Wooden Wall", "destructable": false, "pickable": false, "state": ""}).save();
        }
        return in_area;

}

async function generateCentralHub(in_socket, in_world){
    return await instantiator.createNewGrid(50, 25, 10, "Central Hub", false, in_world)
    .then(async (getArea) => {
        return await generateCentralHubParameter(getArea, "Wooden Fence");
    })
    .then(async (getArea) => {
        return await generateCentralHubPath(getArea, "Stone Path");
    })
    .then(async (getArea) => {
        return await generateBasicShop(in_socket, getArea, 2, 2, 0, 20, 22, 4, "North");
    })
    .then(async (getArea) => {
        return await generateBasicShop(in_socket, getArea, 33, 9, 0, 16, 15, 4, "East");
    })
    .then(async (getArea) => {
        return await generateBasicShopInterior(getArea);
    })
    .then(async (getArea) => {
        return await instantiator.generateCentralHubNPC(getArea);
    })
}

exports.generatePlayerFarm = async function(in_socket, in_world, in_name){

    return await instantiator.createNewGrid(25, 50, 10, in_name + "_farm", true, in_world)
    .then(async (getArea) => {
        return await generateFarmParameters(getArea);
    })
    .then(async (getArea) => {
        return await generateFarmHouse(getArea, 13, 2, 0, 11, 11, 5);
    })
    .then(async (getArea) => {
        return await generateFarmItems(in_socket, in_name, getArea);
    })
    .then(async (getArea) => {
        await characterAccount.findOneAndUpdate({"entityObj.entityName": in_name}, {"areaObj": getArea}).exec()
        .then(async (get_character) => {        
            let newParam = {};
            newParam["type"] = "Character";
            newParam["data"] = JSON.stringify(get_character).replace(/"/g, "`");
            in_socket.emit("Get Data", newParam);
        })
        await areaItem.findOneAndUpdate({"entityObj.itemName": "Basic Well", "entityObj.binder": in_name + "_farm"}, {"entityObj.capacity": 50, "entityObj.maxCapacity": 100}, {upsert:true}).exec();
        return getArea;
    })
}

exports.newWorld = async function(in_socket, in_world){
    generateCentralHub(in_socket, in_world);
}


exports.entity_getItem = async function(in_socket, in_entityName, in_itemName, in_quantity){
    let newParam = {};

    await itemExistance.findOne({"binder.entityName": in_entityName, "entityObj.itemName": in_itemName}).exec()
    .then(async (item_found) => {
        if (item_found) {
            item_found["entityObj"]["quantity"] += parseInt(in_quantity);
            if (item_found["entityObj"]["quantity"] <= 0){
                item_found.remove();
            } else {
                item_found.save();
            }
        } else {
            let getCharacter = characterAccount.findOne({"entityName": in_entityName}).exec()
            let getItem = itemDatabase.findOne({"itemName": in_itemName}).exec()
            await Promise.all([getCharacter, getItem])
            .then(async (res) => {
                newParam["Action"] = "New item";
                return await new item({"itemName": res[1]["itemName"], "itemType": res[1]["itemType"], maxDurability: res[1]["maxDurability"], durability: res[1]["maxDurability"], capacity: 0, maxCapacity: res[1]["maxCapacity"], quantity: in_quantity}).save()
                .then(async (newItem) => {
                    newParam["Item"] = newItem;             
                    return await new itemExistance({"binder": res[0], "entityObj": newItem}).save()
                })
                .then(async (newItem) =>{
                    in_socket.emit("Get Item", newItem);
                })
            })
        }
    })
}

exports.npc_getItem = async function(in_entityName, in_itemName, in_quantity, in_buyPrice, in_sellPrice, in_priceRoof, in_itemRoof){
    let newParam = {};

    return await itemExistance.findOne({"binder.entityName": in_entityName, "entityObj.itemName": in_itemName}).exec()
    .then(async (item_found) => {
        if (item_found) {
            item_found["entityObj"]["quantity"] += parseInt(in_quantity);
            if (item_found["entityObj"]["quantity"] <= 0){
                item_found.remove();
            } else {
                item_found.save();
            }
        } else {
            let getCharacter = entityExistance.findOne({"entityObj.entityName": in_entityName}).exec()
            let getItem = itemDatabase.findOne({"itemName": in_itemName}).exec()
            let newMarketItem = new itemMarket({"buyPrice": in_buyPrice, "sellPrice": in_sellPrice, "priceRoof": in_priceRoof, "itemRoof": in_itemRoof});
            return await Promise.all([getCharacter, getItem, newMarketItem])
            .then(async (res) => {
                newParam["Action"] = "New item";
                return await new item({"itemName": res[1]["itemName"], "itemType": res[1]["itemType"], maxDurability: res[1]["maxDurability"], durability: res[1]["maxDurability"], capacity: 0, maxCapacity: res[1]["maxCapacity"], quantity: in_quantity}).save()
                .then(async (newItem) => {
                    newParam["Item"] = newItem;
//                    in_socket.emit("Item", JSON.stringify(newParam).replace(/"/g, "`"));                
                    return await new itemExistance({"binder": res[0]["entityObj"], "itemObj": newItem, "itemMarketObj": res[2]}).save()
                })
            })
        }
    })
}

exports.instantiateDatabase = async function()
{
    return await new itemDatabase({"itemName": "Strawberry seed","itemType": "Seed","maxDurability": 100,"maxCapacity": 0}).save()
    .then(async () => {
        return await new itemDatabase({"itemName": "Grape seed","itemType": "Seed","maxDurability": 100,"maxCapacity": 0}).save()
    })
    .then(async () => {
        return await new itemDatabase({"itemName": "Basic Shovel","itemType": "Shovel","maxDurability": 100,"maxCapacity": 0}).save()
    })
    .then(async () => {
        return await new itemDatabase({"itemName": "Strawberry","itemType": "Ingrediants","maxDurability": 100,"maxCapacity": 0}).save()
    })
    .then(async () => {
        return await new itemDatabase({"itemName": "Grape","itemType": "Ingrediants","maxDurability": 100,"maxCapacity": 0}).save()
    })
    .then(async () => {
        return await new itemDatabase({"itemName": "Silver","itemType": "Money","maxDurability": 100,"maxCapacity": 0}).save()
    })
    .then(async () => {
        return await new itemDatabase({"itemName": "Small Watering Can","itemType": "Watering Can","maxDurability": 100,"maxCapacity": 10}).save()
    })
    .then(async () => {
        return await new itemDatabase({"itemName": "Basic Axe","itemType": "Axe","maxDurability": 100,"maxCapacity": 0}).save()
    })
    .then(async () => {
        return await new itemDatabase({"itemName": "Basic Pickaxe","itemType": "Pickaxe","maxDurability": 100,"maxCapacity": 0}).save()
    })
    .then(async () => {
        return await new itemDatabase({"itemName": "Wooden Fence","itemType": "Fence","maxDurability": 100,"maxCapacity": 0}).save()
    })
    .then(async () => {
        return await new itemDatabase({"itemName": "Area Connector","itemType": "Trigger","maxDurability": 100,"maxCapacity": 0}).save()
    })
    .then(async () => {
        return await new itemDatabase({"itemName": "Action Modifier","itemType": "Action Modifier Tool","maxDurability": 100,"maxCapacity": 0}).save()
    })
    .then(async () => {
        return await new itemDatabase({"itemName": "Placement Manipulator","itemType": "Placement Modifier Tool","maxDurability": 100,"maxCapacity": 0}).save()
    })
    .then(async () => {
        return await new itemDatabase({"itemName": "Wooden Wall","itemType": "Wall","maxDurability": 100,"maxCapacity": 0}).save()
    })
    .then(async () => {
        return await new itemDatabase({"itemName": "Wooden Slab","itemType": "Slab","maxDurability": 100,"maxCapacity": 0}).save()
    })
    .then(async () => {
        return await new itemDatabase({"itemName": "Wooden Stair","itemType": "Stair","maxDurability": 100,"maxCapacity": 0}).save()
    })
    .then(async () => {
        return await new itemDatabase({"itemName": "Wooden Floor","itemType": "Floor","maxDurability": 100,"maxCapacity": 0}).save()
    })
    .then(async () => {
        return await new itemDatabase({"itemName": "Wooden Door","itemType": "Door","maxDurability": 100,"maxCapacity": 0}).save()
    })
    .then(async () => {
        return await new itemDatabase({"itemName": "Stone Path","itemType": "Path","maxDurability": 100,"maxCapacity": 0}).save()
    })
    .then(async () => {
        return await new itemDatabase({"itemName": "Glass","itemType": "Wall","maxDurability": 100,"maxCapacity": 0}).save()
    })
    .then(async () => {
        return await new itemDatabase({"itemName": "NPC Spawner","itemType": "Spawner","maxDurability": 100,"maxCapacity": 0}).save()
    })
    .then(async () => {
        return await new itemDatabase({"itemName": "Basic Bed","itemType": "Bed","maxDurability": 100,"maxCapacity": 0}).save()
    })
    .then(async () => {
        return await new itemDatabase({"itemName": "Basic Well","itemType": "Well","maxDurability": 100,"maxCapacity": 0}).save()
    })
    .then(async () => {
        return await new itemDatabase({"itemName": "Torch","itemType": "Light","maxDurability": 100,"maxCapacity": 0}).save()
    })
    .then(async () => {
        return await new itemDatabase({"itemName": "Basic Mailbox","itemType": "Mailbox","maxDurability": 0,"maxCapacity": 0}).save()
    })
    .then(async () => {
        return await new itemDatabase({"itemName": "Basic Shipping Bin","itemType": "Shipping Bin","maxDurability": 0,"maxCapacity": 0}).save()
    })
    .then(async () => {
        return await new itemDatabase({"itemName": "Coffee cherry","itemType": "Produce","maxDurability": 0,"maxCapacity": 0}).save()
    })
    .then(async () => {
        return await new itemDatabase({"itemName": "Corn","itemType": "Produce","maxDurability": 0,"maxCapacity": 0}).save()
    })
    .then(async () => {
        return await new itemDatabase({"itemName": "Wheat","itemType": "Produce","maxDurability": 0,"maxCapacity": 0}).save()
    })
    .then(async () => {
        return await new itemDatabase({"itemName": "Carrot","itemType": "Produce","maxDurability": 0,"maxCapacity": 0}).save()
    })
    .then(async () => {
        return await new itemDatabase({"itemName": "Potato","itemType": "Produce","maxDurability": 0,"maxCapacity": 0}).save()
    })
    .then(async () => {
        return await new itemDatabase({"itemName": "Lettuce","itemType": "Produce","maxDurability": 0,"maxCapacity": 0}).save()
    })
    .then(async () => {
        return await new itemDatabase({"itemName": "Cabbage","itemType": "Produce","maxDurability": 0,"maxCapacity": 0}).save()
    })
    .then(async () => {
        return await new itemDatabase({"itemName": "Tomato","itemType": "Produce","maxDurability": 0,"maxCapacity": 0}).save()
    })
    .then(async () => {
        return await new itemDatabase({"itemName": "Onion","itemType": "Produce","maxDurability": 0,"maxCapacity": 0}).save()
    })
    .then(async () => {
        return await new itemDatabase({"itemName": "Green Onion","itemType": "Produce","maxDurability": 0,"maxCapacity": 0}).save()
    })
    .then(async () => {
        return await new itemDatabase({"itemName": "Garlic","itemType": "Produce","maxDurability": 0,"maxCapacity": 0}).save()
    })
    .then(async () => {
        return await new itemDatabase({"itemName": "Rice","itemType": "Produce","maxDurability": 0,"maxCapacity": 0}).save()
    })
    .then(async () => {
        return await new itemDatabase({"itemName": "Coffee bean","itemType": "Seed","maxDurability": 0,"maxCapacity": 0}).save()
    })
    .then(async () => {
        return await new itemDatabase({"itemName": "Corn seed","itemType": "Seed","maxDurability": 0,"maxCapacity": 0}).save()
    })
    .then(async () => {
        return await new itemDatabase({"itemName": "Wheat seed","itemType": "Seed","maxDurability": 0,"maxCapacity": 0}).save()
    })
    .then(async () => {
        return await new itemDatabase({"itemName": "Carrot seed","itemType": "Seed","maxDurability": 0,"maxCapacity": 0}).save()
    })
    .then(async () => {
        return await new itemDatabase({"itemName": "Potato tuber","itemType": "Seed","maxDurability": 0,"maxCapacity": 0}).save()
    })
    .then(async () => {
        return await new itemDatabase({"itemName": "Lettuce seed","itemType": "Seed","maxDurability": 0,"maxCapacity": 0}).save()
    })
    .then(async () => {
        return await new itemDatabase({"itemName": "Cabbage seed","itemType": "Seed","maxDurability": 0,"maxCapacity": 0}).save()
    })
    .then(async () => {
        return await new itemDatabase({"itemName": "Tomato seed","itemType": "Seed","maxDurability": 0,"maxCapacity": 0}).save()
    })
    .then(async () => {
        return await new itemDatabase({"itemName": "Onion seed","itemType": "Seed","maxDurability": 0,"maxCapacity": 0}).save()
    })
    .then(async () => {
        return await new itemDatabase({"itemName": "Green onion bulb","itemType": "Seed","maxDurability": 0,"maxCapacity": 0}).save()
    })
    .then(async () => {
        return await new itemDatabase({"itemName": "Garlic clove","itemType": "Seed","maxDurability": 0,"maxCapacity": 0}).save()
    })
    .then(async () => {
        return await new itemDatabase({"itemName": "Rice seedling","itemType": "Seed","maxDurability": 0,"maxCapacity": 0}).save()
    })
    .then(async () => {
        return await new plantDatabase({"plantName": "Strawberry","seedName": "Strawberry seed","dayRequired": 5,"deathDayRequired": 2}).save()
    })
    .then(async () => {
        return await new plantDatabase({"plantName": "Grape","seedName": "Grape seed","dayRequired": 8,"deathDayRequired": 5}).save()
    })
    .then(async () => {
        return await new plantDatabase({"plantName": "Coffee cherry","seedName": "Coffee bean","dayRequired": 5,"deathDayRequired": 6}).save()
    })
    .then(async () => {
        return await new plantDatabase({"plantName": "Corn","seedName": "Corn seed","dayRequired": 5,"deathDayRequired": 6}).save()
    })
    .then(async () => {
        return await new plantDatabase({"plantName": "Wheat","seedName": "Wheat seed","dayRequired": 5,"deathDayRequired": 6}).save()
    })
    .then(async () => {
        return await new plantDatabase({"plantName": "Carrot","seedName": "Carrot seed","dayRequired": 5,"deathDayRequired": 6}).save()
    })
    .then(async () => {
        return await new plantDatabase({"plantName": "Potato","seedName": "Potato tuber","dayRequired": 5,"deathDayRequired": 6}).save()
    })
    .then(async () => {
        return await new plantDatabase({"plantName": "Lettuce","seedName": "Lettuce seed","dayRequired": 5,"deathDayRequired": 6}).save()
    })
    .then(async () => {
        return await new plantDatabase({"plantName": "Cabbage","seedName": "Cabbage seed","dayRequired": 5,"deathDayRequired": 6}).save()
    })
    .then(async () => {
        return await new plantDatabase({"plantName": "Tomato","seedName": "Tomato seed","dayRequired": 5,"deathDayRequired": 6}).save()
    })
    .then(async () => {
        return await new plantDatabase({"plantName": "Green onion","seedName": "Green onion bulb","dayRequired": 5,"deathDayRequired": 6}).save()
    })
    .then(async () => {
        return await new plantDatabase({"plantName": "Garlic","seedName": "Garlic clove","dayRequired": 5,"deathDayRequired": 6}).save()
    })
    .then(async () => {
        return await new plantDatabase({"plantName": "Rice","seedName": "Rice seedling","dayRequired": 5,"deathDayRequired": 6}).save()
    })
}