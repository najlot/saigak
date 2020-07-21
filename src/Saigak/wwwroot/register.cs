
Directory.CreateDirectory("data");

class User
{
	public string Username { get; set; }
	public string Password { get; set; }
}

var user = new User()
{
	Username = Context.Request.Form["username"].ToString(),
	Password = Context.Request.Form["password"].ToString()
};

if (string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Password))
{
	throw new Exception("invalid");
}

var path = Path.Combine("data", "users.json");
var users = new List<User>();

if (File.Exists(path))
{
	var content = await File.ReadAllTextAsync(path);
	users = JsonConvert.DeserializeObject<List<User>>(content);
}

if (users.Any(u => u.Username.ToLower() == user.Username.ToLower()))
{
	throw new Exception("invalid");
}

users.Add(user);
var content = JsonConvert.SerializeObject(users);
await File.WriteAllTextAsync(path, content);

Context.Response.Cookies.Append("username", user.Username);
Context.Response.Cookies.Append("password", user.Password);
