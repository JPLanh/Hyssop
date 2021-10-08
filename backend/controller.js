const models = require('../Model/gameModel'),
	mongoose = require('mongoose'),
	area = mongoose.model("Area"),
	character = mongoose.model("Character"),
	areaIndex = mongoose.model("AreaIndex");

const dataController = require('../Controller/dataController')

exports.loadCharacters = async function (in_socket, in_username)
{
	return await character.find({"account": in_username}).exec()	
	.then(async (found_characters) => {
		let counter = 0;
		let newParam = {};
		if (found_characters.length == 0){
				newParam["characters"] = null;
				newParam["total"] = found_characters.length;
				newParam["index"] = counter;
				return await in_socket.emit("Load characters", JSON.stringify(newParam).replace(/"/g, "`"));
		} else {
//				found_characters.forEach(async (it_character) => {
				for await (var it_character of found_characters)
				{
					let temp = new Promise(async (resolve, reject) => {
						counter++;
						newParam["characters"] = await it_character;
						newParam["total"] = found_characters.length;
						newParam["index"] = counter;
						 resolve(await in_socket.emit("Load characters", JSON.stringify(newParam).replace(/"/g, "`")));					
//					console.log(newParam);
//					let result = await dataController.segmentData(in_socket, "Character List", it_character);
					})

			}						
		}
	})
}