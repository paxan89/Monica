using Frgo.Dohod.DbModel.ModelCrm.Core;
using Frgo.Dohod.DbModel.Extension;
using NUnit.Framework;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Frgo.Dohod.DbModel.DataAdapter.Import;
using Frgo.Dohod.DbModel.ModelCrm.Fk;

namespace Module.Testing.Nunit.AdapterTest
{
    public class LoadFkDocxFormatTest : BaseServiceTest<IImportAdapter>
    {

        ImportParam _importParam;
        byte[] _fileData;

        public LoadFkDocxFormatTest() : base()
        {
            //string filename = "TXCE170101";
            //string filename = "TXBD180101";
            string filename = "TXBD200601";
            //string filename = "TXIP170101";
            //string filename = "TXNP170101";
            //string filename = "TXSE170101";
            //string filename = "TXSF170101";
            //string filename = "TXVT170101";
            //string filename = "TXZF180903";
            _importParam = new ImportParam()
            {
                FileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"FkTxFormatFiles", $"{filename}.docx"),
                FormatGroup = "FKTX",
                Format = filename.Substring(2, 2)  // "BD"            
            };
         }

        [Test]
        public async Task LoadFkTxFormat_Normal()
        {
           if (!File.Exists(_importParam.FileName))
                return;

            _fileData = File.ReadAllBytes(_importParam.FileName);
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var resPreview = await Service.LoadFkDocxFormat(_fileData, _importParam);
            var mspreview = stopwatch.Elapsed.TotalMilliseconds;

            Assert.IsTrue(true);
        }
 
    }
}
