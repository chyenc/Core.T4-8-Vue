using MyChy.Core.T4.Common;
using MyChy.Frame.Core.Common.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MyChy.Core.T4.Template
{
    public class ServiceWebVue
    {
        private const string IPath = "/MyChy.Service.WebAdminVue";
       // private const string IMappings = "/Mappings";
        private const string Implementation = "/Implementation";

        /// <summary>
        /// 输出模板
        /// </summary>
        /// <param name="Path"></param>
        /// <param name="EntityNamespace"></param>
        public async Task Write(string Path, IList<MyChyEntityNamespace> list)
        {

            var file = Path + IPath;
            //FileHelper.DeleteFolder(file);

            FileHelper.CreatedFolder(file);

            await CreatModuleInitializer(file, list);

            await CreatIService(file, list);

            //await CreatMappings(file, list);


             file = file + Implementation;
             FileHelper.CreatedFolder(file);

             await CreatService(file, list);


        }

        private async Task CreatModuleInitializer(string Path, IList<MyChyEntityNamespace> list)
        {
            string files = Path + "/ModuleInitializer.txt";

            var _sw = new StreamWriter(new FileStream(files, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.Read), Encoding.UTF8);

            StringBuilder sb = new StringBuilder();
            foreach (var i in list)
            {
                sb.AppendFormat("services.AddTransient<I{0}AdminVueService, {0}AdminVueService>();", i.Namespace);
                sb.AppendLine("");
            }
            await _sw.WriteAsync(sb.ToString());
            _sw.Close();


        }

        private async Task CreatIService(string Path, IList<MyChyEntityNamespace> list)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var i in list)
            {
                sb = new StringBuilder();
                string files = Path + $"/I{i.Namespace}AdminVueService.cs";
                var _sw = new StreamWriter(new FileStream(files, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.Read), Encoding.UTF8);

                sb.AppendLine("using System.Threading.Tasks;");
                sb.AppendLine("using System.Collections.Generic;");
                sb.AppendLine("using MyChy.Frame.Core.Common.Model;");
                sb.AppendLine($"using MyChy.Web.ViewModels.{i.Namespace};");
                sb.AppendLine($"using MyChy.Frame.Core.Services;");
                sb.AppendLine("using MyChy.Web.ViewModels.AdminVue;");

                sb.AppendLine("namespace MyChy.Service.WebAdminVue");
                sb.AppendLine("{");
                sb.AppendLine($"public interface I{i.Namespace}AdminVueService : IServiceBase");
                sb.AppendLine("{");

                foreach (var x in i.FileName)
                {
                    sb.AppendLine(" ");

                    sb.AppendLine(" ");

                    sb.AppendLine($"#region {x.Description}管理");
                    sb.AppendLine(" ");
                    sb.AppendLine($"/// <summary>");
                    sb.AppendLine($"/// {x.Description}保存");
                    sb.AppendLine($"/// </summary>");
                    sb.AppendLine($"/// <param name=\"Search\"></param>");
                    sb.AppendLine($"/// <returns></returns>");
                    sb.AppendLine($"Task<ResultBaseModel> Save{x.Name}PostAsync({x.Name}PostModel PostModel);");

                    sb.AppendLine(" ");
                    sb.AppendLine($"/// <summary>");
                    sb.AppendLine($"/// {x.Description}导入");
                    sb.AppendLine($"/// </summary>");
                    sb.AppendLine($"/// <returns></returns>");
                    sb.AppendLine($"Task<ResultBaseModel> Import{x.Name}PostAsync(ImportPostModel PostModel);");

                    sb.AppendLine(" ");
                    sb.AppendLine($"/// <summary>");
                    sb.AppendLine($"/// {x.Description}导出");
                    sb.AppendLine($"/// </summary>");
                    sb.AppendLine($"/// <returns></returns>");
                    sb.AppendLine($"Task<ResultBaseModel> Export{x.Name}PostAsync({x.Name}SearchModel PostModel);");



                    sb.AppendLine("#endregion");
                }

                sb.AppendLine("}");

                sb.AppendLine("}");

                await _sw.WriteAsync(sb.ToString());
                _sw.Close();

            }


        }

        private async Task CreatService(string Path, IList<MyChyEntityNamespace> list)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var i in list)
            {
                sb = new StringBuilder();
                string files = Path + $"/{i.Namespace}AdminVueService.cs";
                var _sw = new StreamWriter(new FileStream(files, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.Read), Encoding.UTF8);

                sb.AppendLine("using System;");
                sb.AppendLine("using System.Collections.Generic;");
                sb.AppendLine("using System.Threading.Tasks;");
                sb.AppendLine("using System.Linq;");
                sb.AppendLine("using AutoMapper;");
                sb.AppendLine("using Microsoft.Extensions.Logging;");
                sb.AppendLine("using MyChy.Core.Domains;");
                sb.AppendLine("using MyChy.Frame.Core.Common.Model;");
                sb.AppendLine("using MyChy.Frame.Core.Services;");
                sb.AppendLine($"using MyChy.Web.ViewModels.{i.Namespace};");
                sb.AppendLine("using MyChy.Service.Plugin.Caches;");
                sb.AppendLine("using MyChy.Frame.Core.Common.Extensions;");
                sb.AppendLine("using MyChy.Service.Database;");
                sb.AppendLine("using MyChy.Service.WebAdminVue;");
                sb.AppendLine("using MyChy.Web.ViewModels.AdminVue;");

                sb.AppendLine("");

                sb.AppendLine("namespace MyChy.Service.WebAdmin.Implementation");
                sb.AppendLine("{");

                sb.AppendLine($"public class {i.Namespace}AdminVueService : ServiceBase, I{i.Namespace}AdminVueService");
                sb.AppendLine("{");
                sb.AppendLine($"private readonly I{i.Namespace}Service {i.Namespace.ToLower()}Service;");
                sb.AppendLine($"private readonly ILogger _logger;");
                sb.AppendLine("//private readonly IGeneralCacheServer generalCacheServer;");
                sb.AppendLine("private readonly IRedisCachesService redisCachesService;");
                sb.AppendLine($"private readonly ICommonUseAdminVueService  commonUseAdminVueService;");
                sb.AppendLine($"//private readonly IExcelService excelService;");  
                sb.AppendLine($"private readonly string CacheKey = \"{i.Namespace}WS_\";");

                sb.AppendLine($"public {i.Namespace}AdminVueService(I{i.Namespace}Service _{i.Namespace.ToLower()}Service");
                sb.AppendLine(", ILoggerFactory loggerFactory");
                sb.AppendLine("//, IGeneralCacheServer _generalCacheServer");
                sb.AppendLine("//, IExcelService _excelService");
                sb.AppendLine(", IRedisCachesService _redisCachesService");
                sb.AppendLine(", ICommonUseAdminVueService _commonUseAdminVueService)");
                sb.AppendLine("{");
                sb.AppendLine($"{i.Namespace.ToLower()}Service = _{i.Namespace.ToLower()}Service;");
                sb.AppendLine("//generalCacheServer = _generalCacheServer;");
                sb.AppendLine("redisCachesService = _redisCachesService;");
                sb.AppendLine("//excelService = _excelService;");
                sb.AppendLine($"_logger = loggerFactory.CreateLogger<{i.Namespace}AdminService>();");
                sb.AppendLine($"commonUseAdminVueService = _commonUseAdminVueService;");
                sb.AppendLine("}");


                foreach (var x in i.FileName)
                {
                    sb.AppendLine(" ");

                    sb.AppendLine(" ");


                    sb.AppendLine($"#region {x.Description}管理");

                    sb.AppendLine(" ");
                    sb.AppendLine($"/// <summary>");
                    sb.AppendLine($"/// {x.Description}保存");
                    sb.AppendLine($"/// </summary>");
                    sb.AppendLine($"/// <param name=\"Search\"></param>");
                    sb.AppendLine($"/// <returns></returns>");
                    sb.AppendLine($"public async Task<ResultBaseModel> Save{x.Name}PostAsync({x.Name}PostModel PostModel)");
                    sb.AppendLine("{");
                    sb.AppendLine($"var result = new ResultBaseModel();");
                    sb.AppendLine($"result = await {i.Namespace.ToLower()}Service.Save{x.Name}PostAsync(PostModel);");
                    sb.AppendLine($"");
                    sb.AppendLine($"return result;");
                    sb.AppendLine("}");


                    sb.AppendLine(" ");
                    sb.AppendLine($"/// <summary>");
                    sb.AppendLine($"/// {x.Description}导入");
                    sb.AppendLine($"/// </summary>");
                    sb.AppendLine($"/// <returns></returns>");
                    sb.AppendLine($"public async Task<ResultBaseModel> Import{x.Name}PostAsync(ImportPostModel PostModel)");
                    sb.AppendLine("{");
                    sb.AppendLine($"");
                    sb.AppendLine($"var result = new ResultBaseModel();");
                    sb.AppendLine($"");
                    sb.AppendLine($"if (string.IsNullOrEmpty(PostModel.Content))");
                    sb.AppendLine("{");
                    sb.AppendLine($"result.Msg = \"Excel 文件必须上传\"; return result;");
                    sb.AppendLine("}");
                    sb.AppendLine($"");
                    sb.AppendLine($"var fileinfo = await commonUseAdminVueService.ShowUploadFiles(PostModel.Content);");
                    sb.AppendLine($"var filepath = commonUseAdminVueService.ShowConfigFilePath(fileinfo.Code);");
                    sb.AppendLine($"");
                    sb.AppendLine($"//var resultBaseT = excelService.Import<{x.Name}ExcelModel>(filepath.Msg);");
                    sb.AppendLine($"//if (resultBaseT.Success)");
                    sb.AppendLine("//{");
                    sb.AppendLine("//    result.Msg = $\"导入客户{result.Id}人\";");
                    sb.AppendLine($"//    result.Success = true;");
                    sb.AppendLine("//}");
                    sb.AppendLine("//else { result.Msg = resultBaseT.Msg; }");
                    sb.AppendLine($"");
                    sb.AppendLine($"");
                    sb.AppendLine($"return result;");
                    sb.AppendLine($"");
                    sb.AppendLine("}");

                    sb.AppendLine(" ");
                    sb.AppendLine($"/// <summary>");
                    sb.AppendLine($"/// {x.Description}导出");
                    sb.AppendLine($"/// </summary>");
                    sb.AppendLine($"/// <returns></returns>");
                    sb.AppendLine($"public async Task<ResultBaseModel> Export{x.Name}PostAsync({x.Name}SearchModel PostModel)");
                    sb.AppendLine("{");
                    sb.AppendLine($"");
                    sb.AppendLine($"var result = new ResultBaseModel();");
                    sb.AppendLine($"var list = await {i.Namespace.ToLower()}Service.Show{x.Name}ListAsync(PostModel);");
                    sb.AppendLine($"if (list.Count > 0)");
                    sb.AppendLine("{");
                    sb.AppendLine($"//  result = excelService.Export(\"批次操作统计\", list);");
                    sb.AppendLine("}");
                    sb.AppendLine($"");
                    sb.AppendLine($"return result;");
                    sb.AppendLine($"");
                    sb.AppendLine("}");
                    sb.AppendLine("#endregion");

                }

                sb.AppendLine("}");

                sb.AppendLine("}");


                await _sw.WriteAsync(sb.ToString());
                _sw.Close();
            }
        }
    }
}
