using MyChy.Core.T4.Common;
using MyChy.Frame.Core.Common.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyChy.Core.T4.Template
{

    public class ServiceWebInterface
    {
        private const string IPath = "/MyChy.Service.WebInterface";
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
                sb.AppendFormat("services.AddTransient<I{0}WebInterfaceService, {0}WebInterfaceService>();", i.Namespace);
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
                string files = Path + $"/I{i.Namespace}WebInterfaceService.cs";
                var _sw = new StreamWriter(new FileStream(files, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.Read), Encoding.UTF8);

                sb.AppendLine("using System.Threading.Tasks;");
                sb.AppendLine("using System.Collections.Generic;");
                sb.AppendLine("using MyChy.Frame.Core.Common.Model;");
                sb.AppendLine($"using MyChy.Web.ViewModels.{i.Namespace};");
                sb.AppendLine($"using MyChy.Frame.Core.Services;");


                sb.AppendLine("namespace MyChy.Service.WebInterface");
                sb.AppendLine("{");
                sb.AppendLine($"public interface I{i.Namespace}WebInterfaceService : IServiceBase");
                sb.AppendLine("{");

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
                string files = Path + $"/{i.Namespace}WebInterfaceService.cs";
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
                sb.AppendLine("using MyChy.Service.WebInterface;");
                sb.AppendLine("using MyChy.Service.WebAdmin;");
                sb.AppendLine("");


                sb.AppendLine("namespace MyChy.Service.WebInterface.Implementation");
                sb.AppendLine("{");

                sb.AppendLine($"public class {i.Namespace}WebInterfaceService : ServiceBase, I{i.Namespace}WebInterfaceService");
                sb.AppendLine("{");
                sb.AppendLine($"private readonly I{i.Namespace}Service _{i.Namespace}Service;");
                sb.AppendLine($"private readonly I{i.Namespace}AdminService _{i.Namespace}AdminService;");
                sb.AppendLine($"private readonly ILogger _logger;");
                sb.AppendLine("//private readonly IGeneralCacheServer generalCacheServer;");
                sb.AppendLine("private readonly IRedisCachesService redisCachesService;");
                sb.AppendLine($"private readonly string CacheKey = \"{i.Namespace}SWI_\";");

                sb.AppendLine($"public {i.Namespace}WebInterfaceService(I{i.Namespace}Service {i.Namespace}Service");
                sb.AppendLine(", ILoggerFactory loggerFactory");
                sb.AppendLine($", I{i.Namespace}AdminService {i.Namespace}AdminService");
                sb.AppendLine("//, IGeneralCacheServer _generalCacheServer");
                sb.AppendLine(", IRedisCachesService _redisCachesService)");
                sb.AppendLine("{");
                sb.AppendLine($"_{i.Namespace}Service = {i.Namespace}Service;");
                sb.AppendLine($"_{i.Namespace}AdminService = {i.Namespace}AdminService;");
                sb.AppendLine("//generalCacheServer = _generalCacheServer;");
                sb.AppendLine("redisCachesService = _redisCachesService;");
                sb.AppendLine($"_logger = loggerFactory.CreateLogger<{i.Namespace}WebInterfaceService>();");

                sb.AppendLine("}");

                sb.AppendLine("}");


                sb.AppendLine("}");

                await _sw.WriteAsync(sb.ToString());
                _sw.Close();

            }


        }


    }
}
