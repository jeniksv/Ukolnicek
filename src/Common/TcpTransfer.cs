using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Communication;

public interface IObjectTransfer : IDisposable{
	void Send<T>(T obj);
	T Recieve<T>();
}

public class JsonTcpTransfer : IObjectTransfer{
	private readonly TcpClient client;
	private readonly NetworkStream stream;
	
	public JsonTcpTransfer(TcpClient client){
		this.client = client;
		stream = client.GetStream();
	}


	// TODO special methods for T == FileInfo
	public void Send<T>(T item){
		if( stream == null ) throw new InvalidOperationException("stream is null");

		var json = JsonSerializer.Serialize<T>(item);

		var jsonBytes = Encoding.UTF8.GetBytes(json);
		stream.Write(jsonBytes, 0, jsonBytes.Length);
	}

	public T Recieve<T>(){
		if( stream == null ) throw new InvalidOperationException("stream is null");
		
		var buffer = new byte[1024];
		var data = new Stream();

		while(stream.DataAvailable){
			var temp = stream.Read(buffer, 0, buffer.Length);
			data.Write(buffer, 0, );
		}
		
		return JsonSerializer.Deserialize<T>( Encoding.UTF8.GetString(data.ToArray()) );
	}

	public void Dispose(){
		client.Dispose();
		stream.Dispose();
	}
}

