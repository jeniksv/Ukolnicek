## Ukolnicek

Client-server application for testing student program solutions.



### Table of Contents

- Introduction
- Getting Started
- Available Commands
- Examples
- Troubleshooting
- Additional Resources



### Introduction

Welcome to the Ukolnicek Console Application! Ukolnicek is a command-line tool that allows you to manage assignments, solutions, and user information. This document provides an overview of the application's features, available commands, and usage instructions.



### Getting Started

Open a terminal or command prompt on your computer.
Navigate to the directory where the application is located.

##### Creating an Account

To create a new user account, use the --create option when running the program. This option allows you to create a new user account to access the application:

```
dotnet run --create
```

##### Already Have an Account

To run application use command:

```
dotnet run
```



### Available Commands

The Ukolnicek Console Application supports various commands for managing assignments, solutions, and user-related tasks. Below is a list of available commands:

- `exit`: Exit the application.
- `download-solution [assignment name] [solution name]`: Download a solution file for a specific assignment.
- `show-assignments`: Display a list of available assignments.
- `show-assignment [assignment name]`: Display details of a specific assignment.
- `show-solution [assignment name] [solution name]`: Display details of a solution for a specific assignment.
- `add-solution [assignment name] [file]`: Add a solution for a specific assignment.
- `show-users`: Display a list of users (Admin-only)
- `show-groups`: Display a list of groups (Admin-only).
- `show-group [group name]`: Display details of a specific group (Admin-only).
- `add-admin [student name]`: Add a user as an admin (Admin-only).
- `add-group [group name] [student name] ... `: Add a group with specified students (Admin-only).
- `assign-task [assignment name] [student name]`: Assign a task to a student (Admin-only).
- `unassign-task [assignment name] [student name]`: Unassign a task from a student (Admin-only).
- `remove-assignment [assignment name]`: Remove a specific assignment (Admin-only).
- `remove-test [assignment name] [test name]`: Remove a test for a specific assignment (Admin-only).
- `remove-task-description [assignment name]`: Remove the description of a specific assignment (Admin-only).
- `remove-group [group name]`: Remove a specific group (Admin-only).
- `help`: Display a list of available commands.



### Examples
Here are some examples of how to use the Ukolnicek Console Application:

Display available assignments:

```show-assignments
show-assignments
```

Add assignment as admin user:

```
add-assignment A
```

Add task description as admin user (file `f` contains string "some description"):

```
add-task-description A f
```

Assign a task to a user:

```
assign-task A Ann
```

Add an admin user:

``` 
add-admin Ann
```



On first start, there are prepared tasks with names Prime and Palindrome (and some test also) and user account Ann with password 123.

Add solution:

```
add-solution Prime prime.py
```

Show assignment:

``` 
show-assignment Prime
```

Show details of a solution:

```
show-solution Prime Solution01
```



### Troubleshooting

If you encounter any issues while using the Ukolnicek Console Application, consider the following tips:

- Double-check the command syntax and arguments.
- Ensure you have the necessary permissions (Admin-only commands).
- Verify that the file paths provided are correct.



### Additional Resources
For more information and assistance, you can refer to the following resources:

Documentation: [Full Documentation](src/README.md)
Contact Support: [Mail](honous.svojan@gmail.com)
