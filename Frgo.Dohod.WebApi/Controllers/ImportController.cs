using System.Collections.Generic;
using System.Threading.Tasks;
using Frgo.Dohod.DbModel.DataAdapter.Import;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Monica.Core.Controllers;
using Monica.Core.Utils;
using System.Web;
using System;


using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Cors;
using Frgo.Dohod.DbModel.ModelCrm.Core;
using System.Text;
using Frgo.Dohod.DbModel.ModelCrm.ImportPreviw;

namespace Frgo.Dohod.WebApi.Controllers
{
    /// <summary>
    /// Основной контроллер для авторизации пользователей в системе
    /// </summary>
    [Route("[controller]/[action]")]
    [ApiController]
    public class ImportController : BaseController
    {
        private IImportAdapter _importAdapter;
        /// <summary>
        /// Наименование модуля
        /// </summary>
        public static string ModuleName => @"ImportController";

        private const string allowedExtensions = ".BD,.VT,.ZF,.IP,.NP,.SF,.SE,.CE,.XLSX" ;
           

        public ImportController(IImportAdapter importAdapter) : base(ModuleName)
        {
            _importAdapter = importAdapter;
        }


        /// <summary>
        /// тестовый пример выполнения
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(bool))]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> FilePreview(IFormFileCollection files) //System.Web HttpPostedFileBase file)
        {
            var resImport = new ResultCrmDb();
            if ((Request.Form?.Files?.Count ?? 0) != 1)
            {
                resImport.AddError(Constant.ErrCodeImport, "Файла для импорта не передан!");
                return Tools.CreateResult(true, "Файла не передан!", resImport);
            }

            var formfile = Request.Form.Files[0];
            if (formfile.Length > 500000)
            {
               resImport.AddError(Constant.ErrCodeImport, "Размер файла превышает 500кБ !");
               return Tools.CreateResult(false, "Файл слшком длинный !", resImport);
            }

            var filename = formfile.FileName;
            var fileextension = Path.GetExtension(filename).ToUpper();
            if (fileextension.Length < 4 || !allowedExtensions.Split(',').Any(x => fileextension.StartsWith(x.Trim())))
            {
                resImport.AddError(Constant.ErrCodeImport, "Файл с таким расширением не импортируется !");
                return Tools.CreateResult(true, "Файла не передан!", resImport);
            }
  

            using (var memoryStream = new MemoryStream())
            {
                byte[] bytesUtf = null;
                await formfile.CopyToAsync(memoryStream);
                if (fileextension == ".XLSX")
                {
                    // Уже Utf
                    bytesUtf = memoryStream.ToArray();
                }
                else
                {
                    var enc = CodePagesEncodingProvider.Instance.GetEncoding(1251);
                    var bytes1251 = memoryStream.ToArray();
                    //var txt1251 = enc.GetString(bytes1251, 0, bytes1251.Length);
                    bytesUtf = Encoding.Convert(enc, Encoding.UTF8, bytes1251);
                }
                //string txtUtf = Encoding.UTF8.GetString(bytesUtf);

                ImportParam importParam = new ImportParam()
                {
                    FileName = filename,
                    FormatGroup = fileextension == ".XLSX" ? "MFXL" : "FKTX",
                    Format = fileextension == ".XLSX" ? "VP" : fileextension.Substring(1, 2),
                    TypeImp = ImportType.Preview,
                    CodeAte = "004",     // ГО Волоколамский
                    CodePos = "00",
                    DaysToSkip = 1
                };


                resImport = await _importAdapter.PreviewFile(bytesUtf, importParam);
                return Tools.CreateResult(true, "", resImport);
            }
        }

       /// <summary>
        /// тестовый пример выполнения
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(bool))]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> FileImport(IFormFileCollection files)
        {
            var resImport = new ResultCrmDb();
            if ((Request.Form?.Files?.Count ?? 0) != 1)
            {
                resImport.AddError(Constant.ErrCodeImport, "Файла для импорта не передан!");
                return Tools.CreateResult(true, "Файла не передан!", resImport);
            }

            var formfile = Request.Form.Files[0];
            if (formfile.Length > 500000)
            {
                resImport.AddError(Constant.ErrCodeImport, "Размер файла превышает 500кБ !");
                return Tools.CreateResult(false, "Файл слшком длинный !", resImport);
            }

            var filename = formfile.FileName;
            var fileextension = Path.GetExtension(filename).ToUpper();
            if (fileextension.Length < 4 || !allowedExtensions.Split(',').Any(x => fileextension.StartsWith(x)))
            {
                resImport.AddError(Constant.ErrCodeImport, "Файл с таким расширением не импортируется !");
                return Tools.CreateResult(true, "Файла не передан!", resImport);
            }


            Microsoft.Extensions.Primitives.StringValues daysShiftVal;
            var okShift = Request.Form.TryGetValue("daysShift", out daysShiftVal); //.Keys.FirstOrDefault();
            var bb = daysShiftVal.ElementAt(0);
            var cc = Convert.ToInt32(bb);

            using (var memoryStream = new MemoryStream())
            {
                byte[] bytesUtf = null;
                await formfile.CopyToAsync(memoryStream);
                if (fileextension == ".XLSX")
                {
                    // Уже Utf
                    bytesUtf = memoryStream.ToArray();
                }
                else
                {
                    var enc = CodePagesEncodingProvider.Instance.GetEncoding(1251);
                    var bytes1251 = memoryStream.ToArray();
                    //var txt1251 = enc.GetString(bytes1251, 0, bytes1251.Length);
                    bytesUtf = Encoding.Convert(enc, Encoding.UTF8, bytes1251);
                }
                //string txtUtf = Encoding.UTF8.GetString(bytesUtf);
                ImportParam importParam = new ImportParam()
                {
                    FileName = filename,
                    FormatGroup = fileextension == ".XLSX" ? "MFXL" : "FKTX",
                    Format = fileextension == ".XLSX" ? "VP" : fileextension.Substring(1, 2),
                    TypeImp = ImportType.Preview,
                    CodeAte = "004",     // ГО Волоколамский
                    CodePos = "00",
                    DaysToSkip = okShift ? cc  : 1
                    };


               resImport = await _importAdapter.ImportFile(bytesUtf, importParam);
               return Tools.CreateResult(true, "", resImport);
          }
        }
    }
}
