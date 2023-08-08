## Application Architecture Documentation
### Table of Contents

- Introduction
- Overall Architecture
- Client-Side Architecture
- User Interface Layer
- User Management Layer
- Communication Layer
- Server-Side Architecture
- Server Core Layer
- Communication Layer
- User Management Layer
- Data Storage Layer
- Conclusion

### Introduction
The Application Architecture Documentation provides an overview of the design and structure of the application. It describes how the client and server components are organized, how they interact, and the overall flow of data and operations within the system.

### Overall Architecture
The application follows a client-server architecture, where clients interact with a central server to perform various tasks. The server manages user authentication, assignment handling, and communication between users.

### Client-Side Architecture
#### User Interface Layer
The User Interface (UI) layer is responsible for presenting information to users and capturing user inputs. It consists of a console-based interface that allows users to sign in, create accounts, and interact with assignments and solutions.

#### User Management Layer
The User Management layer handles user authentication and account creation. Users can either sign in with existing accounts or create new accounts. This layer communicates with the server to verify credentials and manage user sessions.

#### Communication Layer
The Communication layer facilitates communication between the client and the server. It handles the serialization and deserialization of data using JSON and manages the TCP connection for data transfer.

### Server-Side Architecture
Server Core Layer
The Server Core layer forms the heart of the server-side architecture. It listens for incoming client connections, manages client sessions, and dispatches requests to appropriate handlers. The Server Core ensures smooth communication between clients and other server components.

### Communication Layer
Similar to the client-side, the Communication layer on the server handles data serialization, deserialization, and TCP communication. It ensures reliable data exchange between the server and clients.

### User Management Layer
The User Management layer on the server is responsible for user authentication, account creation, and user session management. It interacts with the data storage layer to verify user credentials and manage user-related data.

### Data Storage Layer
The Data Storage layer handles the persistent storage of user accounts, assignments, solutions, and other relevant data. It communicates with the User Management layer to authenticate users and retrieve user-related information.

### Conclusion
The Application Architecture Documentation provides a high-level overview of the client-server architecture, outlining the key components and their interactions. The separation of concerns into various layers ensures modularity, maintainability, and scalability of the application. By following this architecture, the application provides a robust platform for managing user accounts, assignments, and solutions in an efficient and user-friendly manner.



## Documentation for JsonTcpTransfer and Communication Interfaces
### JsonTcpTransfer Class
The JsonTcpTransfer class provides functionality for sending and receiving JSON-serialized objects over a TCP connection. It implements the IObjectTransfer interface and is designed to facilitate communication between a client and a server.

### Constructors
JsonTcpTransfer(TcpClient client): Initializes an instance of the JsonTcpTransfer class using an existing TcpClient instance.
JsonTcpTransfer(string ip, int port): Initializes an instance of the JsonTcpTransfer class by establishing a new TCP connection to the specified IP address and port.
Methods
void Send<T>(T item): Serializes the provided object item to JSON using Newtonsoft.Json, encodes it as UTF-8 bytes, and sends it over the established TCP connection.
T Receive<T>(): Receives JSON data from the TCP connection, accumulates the received data until a complete JSON object is formed, deserializes it back into an object of type T, and returns the deserialized object.
Fields and Properties
private readonly JsonSerializerSettings settings: A set of JSON serialization settings, including type information handling for objects.
Communication Interfaces
IObjectTransfer Interface
The IObjectTransfer interface defines a contract for transferring objects over a communication channel.

void Send<T>(T item): Sends an object of type T over the communication channel.
T Receive<T>(): Receives an object of type T from the communication channel.
IRequest Interface
The IRequest<T> interface represents a generic user request for communication with a server.

RequestEnum Type: Gets the type of the request.
T? Data: Gets the optional data associated with the request.
RequestEnum Enumeration
The RequestEnum enumeration lists possible user actions or requests that can be communicated to the server. Some of the supported actions include:

Login: Initiates a user login.
CreateAccount: Requests the creation of a new user account.
Exit: Signals the intention to exit the application.
ShowAssignments: Requests a list of assignments.
... (and various other actions)
Response<T> Class
The Response<T> class implements the IResponse<T> interface and represents a response from the server to a client's request.

T? Data: Gets or sets the data included in the response.
Request Factory (Request Class)
The Request class provides factory methods for creating requests:

static Request<object> Create(RequestEnum type): Creates a generic request of the specified type with no associated data.
static Request<T> Create<T>(RequestEnum type, T data): Creates a generic request of the specified type with associated data of type T.
Summary
The provided code includes a JsonTcpTransfer class for sending and receiving JSON-serialized objects over a TCP connection, along with a set of communication interfaces and classes to define user requests and server responses. The communication architecture is designed to facilitate interactions between a client and a server application. It utilizes JSON serialization for data interchange and supports various user actions through the RequestEnum enumeration. The code demonstrates a structured approach to communication and provides a foundation for building a client-server communication system.



## Documentation for Server Components and TcpUser Class
### Progam Class
The Progam class serves as the entry point for the server application. It establishes a UDP socket to obtain the local IP address, initializes a Server instance, and starts the server's main loop.

### Methods
static async Task Main(string[] args): The main entry point of the server application. It initializes a UDP socket to get the local IP address, creates an instance of the Server class, and starts the server's main loop asynchronously.
Server Class
The Server class represents the core of the server application. It listens for incoming TCP client connections and manages the communication with connected clients.

### Constructors
public Server(int port = 8888): Initializes an instance of the Server class by starting a TCP listener on the specified port.
Methods
public async Task MainLoop(): Enters a loop that continuously listens for incoming TCP client connections using AcceptTcpClientAsync. When a client connects, a new TcpUser instance is created to handle communication with the client.

public void Dispose(): Disposes of the server resources, stopping the listener and cleaning up client connections.

Private Methods
private void ReleaseUnmanagedResources(): Disposes of client connections and stops the server.

~Server(): The finalizer, ensuring proper resource disposal when the object is garbage-collected.

### TcpUser Class
The TcpUser class represents a connected client and handles communication with that client. It implements the server-side logic for processing various requests from the client.

### Properties
public string? Name { get; set; }: The name of the user associated with the client.
Constructors
public TcpUser(TcpClient client): Initializes an instance of the TcpUser class by associating it with a TcpClient instance for communication.
### Methods
public void ClientLoop(): Enters a loop that listens for requests from the client. It processes each request by calling the appropriate handler method.

public void HandleRequest(IRequest<object> request): Dispatches requests to corresponding methods based on the RequestEnum provided in the request.

Various private methods: These methods handle specific request types such as login, creating an account, adding solutions, assigning tasks, displaying assignments, and more.

### Summary
The provided code includes components for the server application, including the entry point in the Progam class and the core server logic in the Server and TcpUser classes. The Server class manages incoming client connections and delegates request handling to individual TcpUser instances. The TcpUser class handles specific request types and communicates with clients using JSON-serialized objects over TCP. This architecture provides the foundation for a multi-client server application that can process various user requests and respond accordingly. It supports user authentication, assignment management, and other functionality defined by the communication protocol.

## Documentation for Client Components and User Classes

### Progam Class
The Progam class serves as the entry point for the client application. It provides a simple command-line interface for creating accounts or signing in users, using the Client class. Once a user is signed in, the main loop of the user interface is started.

### Methods
static void Main(string[] args): The main entry point of the client application. It interacts with the user interface to either create an account or sign in a user. After successful sign-in, it starts the main loop of the user interface.
User Class (Abstract)
The User class is an abstract base class representing a user of the system. It handles common user operations and communication with the server.

### Properties
public string Name: The name of the user.
Constructors
public User(string name, IObjectTransfer t): Initializes a User instance with a name and an IObjectTransfer implementation for communication.
### Methods
public virtual object HandleCommand(RequestEnum command, string[] args): Handles user commands, which can include actions like showing assignments, solutions, task descriptions, and more.

protected void AddSolution(string[] args): Adds a solution to the server based on provided arguments.

protected string[] ShowAssignments(): Requests and retrieves a list of assignments from the server.

Various other methods for handling different types of requests and responses.

Student Class
The Student class extends the User class and represents a student user of the system.

Constructors
public Student(string name, IObjectTransfer transfer): Initializes a Student instance with a name and an IObjectTransfer implementation for communication.
Admin Class
The Admin class extends the User class and represents an administrative user of the system. It provides additional functionality specific to administrators.

Constructors
public Admin(string name, IObjectTransfer transfer): Initializes an Admin instance with a name and an IObjectTransfer implementation for communication.
Methods
public override object HandleCommand(RequestEnum command, string[] args): Overrides the base method to include administrative commands such as managing groups, users, and assignments.
Client Class
The Client class provides methods for creating different types of users (students, admins) and managing user authentication and communication.

Methods
public static User? SignIn(IUserInterface ui): Attempts user sign-in by sending login credentials to the server and creating an appropriate user instance if successful.

public static User? CreateAccount(IUserInterface ui): Attempts to create a new user account by sending registration data to the server.

public static User CreateAdmin(): Creates an administrative user instance.

public static User CreateStudent(): Creates a student user instance.

Summary
The provided code includes components for the client application, including the Progam class as the entry point, abstract User class for user management, Student and Admin classes for specific user types, and the Client class for user creation and authentication. The code demonstrates a client-side structure for interacting with the server application, sending requests, and receiving responses using JSON-serialized objects over TCP. The abstract User class provides a basis for different user types to handle various commands and responses from the server. This architecture provides the building blocks for a client application with user authentication, command handling, and communication with a server.
