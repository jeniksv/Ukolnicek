using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Communication;

public struct CustomFile{ // TODO change name
	public string Name { get; set; }
	public byte[] Content { get; set; }

	public CustomFile(string name, byte[] content){
		Name = name;
		Content = content;
	}

	public void Save(){
		File.WriteAllBytes(Name ,Content);	
	}
}

public interface IObjectTransfer : IDisposable{
	void Send<T>(T obj);
	T Receive<T>();
	Task<T?> ReceiveAsync<T>();
}

public class JsonTcpTransfer : IObjectTransfer{
	private readonly TcpClient client;
	private readonly NetworkStream stream;
	
	public JsonTcpTransfer(TcpClient client){
		this.client = client;
		stream = client.GetStream();
	}

	public void Send<T>(T item){
		if( stream == null ) throw new InvalidOperationException("stream is null");

		var json = JsonSerializer.Serialize<T>(item);
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
	
		T json = JsonSerializer.Deserialize<T>( Encoding.UTF8.GetString(data.ToArray()) );	
		return json;
	}

	public async Task<T?> ReceiveAsync<T>(){
		if( stream == null ) throw new InvalidOperationException("stream is null");

		var buffer = new byte[1024];
		var data = new MemoryStream();

		do{	
			var task  = await stream.ReadAsync(buffer, 0, buffer.Length);
			await data.WriteAsync(buffer, 0, task);
		} while(stream.DataAvailable);
		
		data.Position = 0;

		return JsonSerializer.Deserialize<T>( Encoding.UTF8.GetString(data.ToArray()) );
	}

	public void Dispose(){
		client.Dispose();
		stream.Dispose();
	}
}

