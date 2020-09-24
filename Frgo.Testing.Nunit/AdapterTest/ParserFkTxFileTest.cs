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
    public class ImportFileTest : BaseServiceTest<IImportAdapter>
    {
        ImportParam _importParam = new ImportParam()
        {
            FileName = "",
            FormatGroup = "FKTX",  // "MFXL"
            Format = "", //"BD", "VT", ...     
            TypeImp = ImportType.None,
            CodeAte = "004",     // ГО Волоколамский
            CodePos = "00",
            DaysToSkip = 1
        }; 
        byte[] _fileData;
        public ImportFileTest() : base()
        {
            //_importParam.Format = "BD";  
            //_importParam.Format = "VT";  
            //_importParam.Format = "SF";  
            //_importParam.Format = "ZF";  
            //_importParam.Format = "NP";  
            //_importParam.Format = "IP";  
            _importParam.Format = "SE";  
            //_importParam.Format = "CE";  
           var files = Directory.GetFiles(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ImportFiles"), _importParam.ImportFileMask());
            if (files.Length == 0)
                return;
            _importParam.FileName = Path.GetFileName(files[0]);
            var enc = CodePagesEncodingProvider.Instance.GetEncoding(1251);
            using (StreamReader filereader = new StreamReader(files[0], enc))
            {
                // var str64 = Convert.ToBase64String(bytes);
                _fileData = Encoding.UTF8.GetBytes(filereader.ReadToEnd());
            }
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
