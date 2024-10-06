using System.IO;
using System.Threading.Tasks;

namespace Saigak.Blobs.Repositories
{
	public interface IRepository
	{
		Task<Stream> GetAsync(string subPath);
		Task PostAsync(string subPath, Stream stream);
		Task PutAsync(string subPath, Stream stream);
		Task DeleteAsync(string subPath);
	}
}
