namespace Ukolnicek.Communication;

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

public struct InvalidOperation{
	public string Message { get; set; }
}
