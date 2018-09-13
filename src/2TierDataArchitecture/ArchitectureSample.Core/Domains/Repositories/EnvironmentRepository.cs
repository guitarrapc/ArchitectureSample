using ArchitectureSample.Core.Datas.DataStores;
using System.Threading.Tasks;

namespace ArchitectureSample.Core.Repositories
{
    public class EnvironmentRepository
    {
        public static async Task<bool> IsEc2Instance()
        {
            var result = await Ec2MetaDataStore.IsEc2Instance;
            return result;
        }
    }
}
