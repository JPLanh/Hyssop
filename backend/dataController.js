const MAX_DATA_SIZE = 1200,
	BUFFER_PAYLOAD = 7;

exports.segmentData = async function segmentData(in_socket, in_header, in_data)
{
	let MAX_TRANSFER_SIZE = MAX_DATA_SIZE - BUFFER_PAYLOAD - in_header.length
	let data = JSON.stringify(in_data);
	if (data.length < MAX_TRANSFER_SIZE){
		in_socket.emit(in_header, in_data);
	} else {
		let segParam = {};
		let total = Math.ceil(data.length / MAX_TRANSFER_SIZE);
		console.log(data.length + " , " + MAX_TRANSFER_SIZE);
		console.log(total);
		for ( let index = 0; index < total; index ++){
			new Promise(async (resolve, reject) => {
				let toSend = "";
				if ((index+1) * MAX_TRANSFER_SIZE > data.length){
					toSend = data.substring(index * MAX_TRANSFER_SIZE, data.length);
				} else {
					toSend = data.substring(index * MAX_TRANSFER_SIZE, (index+1) * MAX_TRANSFER_SIZE)				
				}
				console.log(toSend);
				segParam["Data"] = toSend.replace(/"/g, "|");
				segParam["GUID"] = "Test";
				segParam["Count"] = index;
				segParam["Total"] = total-1;
				segParam["Header"] = in_header;

				console.log(segParam);
				await resolve(in_socket.emit("Data Parser", JSON.stringify(segParam).replace(/"/g, "`")));				
			})
		}
	}

}

exports.sendListData = async function(in_socket, in_pagination, in_list, in_indiceName, in_network_command, in_action, in_complete_action)
{
	let newParam = {};
	let counter = 0;

	for (let index = 0; index < in_list.length; index+=in_pagination){
		await new Promise(async (resolve, reject) => {
			if (index > in_list.length){
				newParam[in_indiceName] = in_list.slice(index, in_list.length)																
			} else {
				newParam[in_indiceName] = in_list.slice(index, index+in_pagination)																
			}
			newParam["Action"] = in_action;
			newParam["index"] = (index + in_pagination > in_list.length ? in_list.length : index + in_pagination);
			newParam["total"] = in_list.length
			counter = (index + in_pagination > in_list.length ? in_list.length : index + in_pagination);
//			await resolve(await this.segmentData(in_socket, in_network_command, JSON.stringify(newParam).replace(/"/g, "~")));
			await resolve(await in_socket.emit(in_network_command, JSON.stringify(newParam).replace(/"/g, "`")));
		})
	}
	if (counter < in_list.length){								
		new Promise(async (resolve, reject) => {
			newParam[in_indiceName] = in_list.slice(counter, in_list.length)
			newParam["Action"] = in_action;
			newParam["index"] = in_list.length;
			newParam["total"] = in_list.length
			resolve(await in_socket.emit(in_network_command, JSON.stringify(newParam).replace(/"/g, "`")));
		})
	}
	return new Promise(async (resolve, reject) =>{
		newParam[in_indiceName] = null;
		newParam["Action"] = "Complete";
		resolve(await in_socket.emit(in_network_command, newParam));									
	})
}