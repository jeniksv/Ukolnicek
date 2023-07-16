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
	
	private string lastJson = "";
	private string prevJson = "";

	public JsonTcpTransfer(TcpClient client){
		this.client = client;
		stream = client.GetStream();
	}

	public void Send<T>(T item){
		if( stream == null ) throw new InvalidOperationException("stream is null");

		// TODO handle if object is FileInfo 
		var json = JsonSerializer.Serialize<T>(item);
		lastJson = json;

		var jsonBytes = Encoding.UTF8.GetBytes(json);
		stream.Write(jsonBytes, 0, jsonBytes.Length);
	}

	public T Recieve<T>(){
		// TODO implement	
	}

	public void Dispose(){
		client.Dispose();
		stream.Dispose();
	}
}

