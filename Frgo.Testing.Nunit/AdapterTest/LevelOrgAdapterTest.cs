using Frgo.Dohod.DbModel.Interfaces.Adapters.Settings;
using Module.Testing.Nunit;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Frgo.Testing.Nunit.AdapterTest
{
    public class LevelOrgAdapterTest : BaseServiceTest<ILevelOrgAdapter>
    {
        /// <summary>
        /// Тест удаление записи из t_levelorg
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task RemoveTest()
        {
           // await Service.RemoveAsync(4);
        }
        /// <summary>
        /// Тест добавление записи в t_levelorg 
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task AddTest()
        {
            //await Service.Add(3, 2);
        }
        /// <summary>
        /// Тест получение всех строк в t_levelorg 
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task GetAllTest()
        {
            await Service.GetAll();
        }
    }
}
