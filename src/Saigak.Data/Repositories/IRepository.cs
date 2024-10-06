using System.Threading.Tasks;

namespace Saigak.Data.Repositories
{
	public interface IRepository
	{
		Task<dynamic> GetAsync(string subPath);
		Task CreateAsync(string subPath, dynamic content);
		Task UpdateAsync(string subPath, dynamic content);
		Task DeleteAsync(string subPath);
	}
}
