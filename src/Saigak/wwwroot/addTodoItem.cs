if (!Context.Request.Cookies.TryGetValue("username", out var username))
{
	throw new Exception("no username");
}

if (!Context.Request.Cookies.TryGetValue("password", out var password))
{
	throw new Exception("no username");
}

class User
{
	public string Username { get; set; }
	public string Password { get; set; }
}

var path = Path.Combine("data", "users.json");
var content = await File.ReadAllTextAsync(path);
var users = JsonConvert.DeserializeObject<List<User>>(content);

if (!users.Any(u => u.Username == username && u.Password == password))
{
	throw new Exception("bad login");
}

public class TodoItem
{
	public Guid Id { get; set; }
	public string Title { get; set; }
	public DateTime CreatedAt { get; set; }
	public string Priority { get; set; }
}

path = Path.Combine("data", username, "todos.json");
content = await File.ReadAllTextAsync(path);
var items = JsonConvert.DeserializeObject<List<TodoItem>>(content);

items.Add(new TodoItem()
{
	Id = Guid.NewGuid(),
	Title = Context.Request.Form["title"].ToString(),
	CreatedAt = DateTime.Now,
	Priority = Context.Request.Form["priority"].ToString()
});

content = JsonConvert.SerializeObject(items);
await File.WriteAllTextAsync(path, content);