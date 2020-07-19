
public class TodoItem
{
	public Guid Id { get; set; }
	public string Title { get; set; }
	public DateTime CreatedAt { get; set; }
	public string Priority { get; set; }
}

var path = Path.Combine("data", "todos.json");
var content = await File.ReadAllTextAsync(path);
var items = JsonConvert.DeserializeObject<List<TodoItem>>(content);

items.Add(new TodoItem()
{
	Id = Guid.NewGuid(),
	Title = Context.Request.Form["title"].ToString(),
	CreatedAt = DateTime.Now,
	Priority = ""
});

content = JsonConvert.SerializeObject(items);
await File.WriteAllTextAsync(path, content);