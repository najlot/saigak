# Saigak
## About
Saigak is an open source web framework based on ASP.NET written in C#. Saigak makes it possible to create websites with C#, Python, JavaScript, Lua or PHP (maybe more to come) in an DotNet environment.<br/>
The main goal is to combine the performance of DotNet and the speed of development with scripting languages (no recompilation etc).

## Status
This application is under development. There maybe be some bugs and breaking changes as the development continues.

## Getting started
- Open your favorite terminal in the folder <b>src/Saigak</b>
- Start the application with <b>dotnet run</b>
- Open the <b>wwwroot</b> folder. Files in this folder will be run when someone requests a website depending on the ending:
    - .cs = C# code
    - .py = Python code
    - .js = JavaScript code
    - .lua = Lua code
    - .php = PHP code
    - .sai = HTML file with embedded source code (like PHP, but with multiple languages) executed on the server. 
- Files in the <b>wwwroot/static</b> subfolder are served 1:1 to the client.

## Ideas
This list may change as the development continues. The order is not the priority.
- Example projects
- Middleware
- Hosted services
- Extensions / packages
- NuGet packages
- Additional languages
- This application as microservices
- C-Style "#include" statement
- Development in the web browser
- Debugging support
- Provide precompiled binaries and Docker container
- Share data between sections (Something like "<?cs (var) num = 42;?> some HTML here <?cs Console.WriteLine(num);?>")
- Routing (routing config file / routing stript)
- Send and handle events
- 