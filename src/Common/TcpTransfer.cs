using System.Net;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;

namespace Communication;

public interface IObjectTransfer : IDisposable{
	void Send<T>(T obj);
	T Receive<T>();
}

public class JsonTcpTransfer : IObjectTransfer{
	private readonly TcpClient client;
	private readonly NetworkStream stream;
	private readonly JsonSerializerSettings settings = new(){
		TypeNameHandling = TypeNameHandling.All,
		TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Full
	};
	private string prevJson = "";

	public JsonTcpTransfer(TcpClient client){
		this.client = client;
		stream = client.GetStream();
	}

	public JsonTcpTransfer(string ip, int port){
		client = new TcpClient();
		client.Connect(ip, port);
		stream = client.GetStream();
	}



	public void Send<T>(T item){
		if( stream == null ) throw new InvalidOperationException("stream is null");

		var json = JsonConvert.SerializeObject(item, settings);
		var jsonBytes = Encoding.UTF8.GetBytes(json);
		stream.Write(jsonBytes, 0, jsonBytes.Length);
	}

	public T Receive<T>(){
		if( stream == null ) throw new InvalidOperationException("stream is null");
	
		string json;

		if( prevJson.Length == 0 ){	
			var buffer = new byte[1024];
			var data = new MemoryStream();

			do{
				var temp = stream.Read(buffer, 0, buffer.Length);
				data.Write(buffer, 0, temp);
			} while(stream.DataAvailable);
		
			var jsonBytes = data.ToArray();
			json = prevJson + Encoding.UTF8.GetString(jsonBytes);
			prevJson = "";
			//var item = JsonConvert.DeserializeObject(json, settings);
		
			//if( item is T t ) return t;
			//throw new InvalidCastException($"Cannot cast {item.GetType()} to {typeof(T)}");
		} else {
			json = prevJson;
			prevJson = "";
		}

		var jsons = json.Split("}{");

		if(jsons.Length > 1) json = jsons[0] + "}";
		
		for(int i = 1; i < jsons.Length; i++){
			prevJson += "{" + jsons[i] + "}";
		}

		if (prevJson.Length > 0) prevJson = prevJson[..^1];

		var item = JsonConvert.DeserializeObject(json, settings);
		if( item is T t ) return t;
		throw new InvalidCastException($"Cannot cast {item.GetType()} to {typeof(T)}");
	}

	public void Dispose(){
		client.Dispose();
		stream.Dispose();
	}
}
