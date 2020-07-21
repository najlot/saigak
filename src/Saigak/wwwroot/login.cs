
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

var path = Path.Combine("data", "users.json");

if (!File.Exists(path))
{
	throw new Exception("bad login");
}

var content = await File.ReadAllTextAsync(path);
var users = JsonConvert.DeserializeObject<List<User>>(content);

if (!users.Any(u => u.Username == user.Username && u.Password == user.Password))
{
	throw new Exception("bad login");
}

Context.Response.Cookies.Append("username", user.Username);
Context.Response.Cookies.Append("password", user.Password);
