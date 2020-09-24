using System.Linq;
using System.Threading.Tasks;
using Monica.Core.Abstraction.ReportEngine;
using Monica.Core.DbModel.ModelDto.Report;
using Monica.Core.Service.ReportEngine;
using NUnit.Framework;

namespace Module.Testing.Nunit.AdapterTest
{
    public class ReportManagerTest : BaseServiceTest<IReportManager>
    {

        [Test]
        public async Task AddFormReportAsync()
        {
            var result = await Service.AddOrEditFormReportAsync(new FormModelDto()
            {
                Id = 0,
                Caption = "Продукты",
                NameClassDataEngine = nameof(ReportEngineDefaultData),
                Order = 10,
                TableName = "product",
                TypeFormId = 1
            });
            Assert.IsTrue(result.Succeeded);
        }
        
        [Test]
        public async Task GetFieldsFormAsync_Normal()
        {
            var fields = await Service.GetFieldsFormAsync("Administrator", 2, true);
            Assert.IsTrue(fields.Any());
        }


        [Test]
        public async Task GetFieldsFormWithProfileAsync_Normal()
        {
            var fields = await Service.GetFieldsFormWithProfileAsync("Administrator", 2, true);
            Assert.IsTrue(fields.Any());
        }
        
        [Test]
        public async Task GetFormsAsunc_Normal()
        {
            var forms = await Service.GetFormsAsunc("Administrator",  true);
            Assert.IsTrue(forms.Any());
        }

        [Test]
        public async Task GetButtonsAsync_Normal()
        {
            var forms = await Service.GetButtonsAsync("Administrator",2, true);
            Assert.IsTrue(forms.Any());
        }


        [Test]
        public async Task GetTypeForms_Normal()
        {
            var forms = await Service.GetTypeForms();
            Assert.IsTrue(forms.Any());
        }
    }
}
