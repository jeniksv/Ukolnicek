using System.Net;
using System.Net.Sockets;
using System.Text;
//using System.Text.Json;
//using System.Text.Json.Serialization;
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

		// var json = JsonSerializer.Serialize<T>(item);
		var json = JsonConvert.SerializeObject(item, settings);
		var jsonBytes = Encoding.UTF8.GetBytes(json);
		stream.Write(jsonBytes, 0, jsonBytes.Length);
	}

	public T Receive<T>(){
		if( stream == null ) throw new InvalidOperationException("stream is null");
		
		var buffer = new byte[1024];
		var data = new MemoryStream();

		do{
			var temp = stream.Read(buffer, 0, buffer.Length);
			data.Write(buffer, 0, temp);
		} while(stream.DataAvailable);
		
		var jsonBytes = data.ToArray();
		var json = Encoding.UTF8.GetString(jsonBytes);
		// T json = JsonSerializer.Deserialize<T>( Encoding.UTF8.GetString(data.ToArray()) );	
		var item = JsonConvert.DeserializeObject(json, settings);
		
		if( item is T t ) return t;
		throw new InvalidCastException($"Cannot cast {item.GetType()} to {typeof(T)}");
	}

	public void Dispose(){
		client.Dispose();
		stream.Dispose();
	}
}

