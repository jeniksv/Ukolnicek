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

public struct AssignmentCreateData{
	public string Name { get; set; }
	public byte[] TaskDescription { get; set; }
}
