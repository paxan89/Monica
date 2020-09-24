using System.Reflection;
using System.Threading.Tasks;
using Monica.Core.Abstraction.ReportEngine;
using Monica.Core.DbModel.ModelDto.Core;
using NUnit.Framework;

namespace Module.Testing.Nunit.AdapterTest
{
    public class ReportDataTest : BaseServiceTest<IReportData>
    {
        [Test]
        public async Task GetListData_Program()
        {
            var result = await Service.GetDataList(new BaseModelReportParam
            {
                FormId = 2,
                PageCount = 100,
                PageSize = 1,
                ModelId = "0",
                RowCount = 100
            });

            Assert.IsTrue(result.Data != null);
        }
    }
}
