using Frgo.Dohod.DbModel.ModelCrm.Core;
using Frgo.Dohod.DbModel.Extension;
using NUnit.Framework;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Frgo.Dohod.DbModel.DataAdapter.Import;

namespace Module.Testing.Nunit.AdapterTest
{
    public class ImportXlFileTest : BaseServiceTest<IImportAdapter>
    {
        ImportParam _importParam = new ImportParam()
        {
            FileName = "",
            FormatGroup = "MFXL",   
            Format = "VP",         
            TypeImp = ImportType.None,
            CodeAte = "004",     // ГО Волоколамский
            CodePos = "00",
            DaysToSkip = 1
        }; 
        byte[] _fileData;
        public ImportXlFileTest() : base()
        {
            var files = Directory.GetFiles(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ImportFiles"), _importParam.ImportFileMask());
            if (files.Length == 0)
                return;
            _importParam.FileName = Path.GetFileName(files[0]);
            _fileData = File.ReadAllBytes(files[0]);
        }

        [Test]
        public async Task ImportFile_Normal()
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var resPreview = await Service.PreviewFile(_fileData, _importParam);
            var mspreview = stopwatch.Elapsed.TotalMilliseconds;

            stopwatch.Restart();
            var resImport = await Service.ImportFile(_fileData, _importParam);
            var msimport = stopwatch.Elapsed.TotalMilliseconds;

            Assert.IsTrue(true);
        }
 
    }
}
