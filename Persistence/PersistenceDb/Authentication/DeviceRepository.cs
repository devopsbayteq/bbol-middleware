using Microsoft.Extensions.Logging;
using PersistenceDb.Models.Authentication;
using PersistenceDb.Repository.Interfaces.Authentication;
namespace PersistenceDb.Authentication;
public class DeviceRepository(PersistenceContext dbContext, ILogger<DeviceRepository> logger) : GenericRepository<Device>(dbContext, logger), IDeviceRepository
{
}