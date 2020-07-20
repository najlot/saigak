public class TodoItem
{
	public Guid Id { get; set; }
	public string Title { get; set; }
	public DateTime CreatedAt { get; set; }
	public string Priority { get; set; }
}

var toDelete = Guid.Parse(Context.Request.Form["id"].ToString());

var path = Path.Combine("data", "todos.json");
var content = await File.ReadAllTextAsync(path);
var items = JsonConvert.DeserializeObject<List<TodoItem>>(content);

var goodEntries = items.Where(item => item.Id != toDelete).ToList();

content = JsonConvert.SerializeObject(goodEntries);
await File.WriteAllTextAsync(path, content);