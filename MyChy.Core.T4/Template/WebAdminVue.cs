using System.Text;
using MyChy.Core.T4.Common;
using MyChy.Frame.Core.Common.Helper;

namespace MyChy.Core.T4.Template;

public class WebAdminVue
{
    private const string IPath = "/MyChy.Web.WebAdminVue";
    private const string Ipath = "/Controllers";

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

        file = file + Ipath;
        FileHelper.CreatedFolder(file);

        foreach (var i in list)
        {
            //var filePath = file + "/" + i.Namespace;
            //FileHelper.CreatedFolder(filePath);
            await CreatControllers(file, i);
        }


    }
    private async Task CreatControllers(string Path, MyChyEntityNamespace i)
    {
        StringBuilder sb = new StringBuilder();
        sb = new StringBuilder();
        string files = Path + $"/{i.Namespace}Controller.cs";
        var _sw = new StreamWriter(new FileStream(files, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.Read), Encoding.UTF8);
        var newservicelist = new List<string>();

        foreach (var y1 in i.FileName)
        {
            foreach (var y3 in y1.ServiceList)
            {
                if (!newservicelist.Contains(y3))
                {
                    newservicelist.Add(y3);
                }

            }
        }


        sb.AppendLine("using Microsoft.AspNetCore.Mvc;");
        sb.AppendLine("using Microsoft.Extensions.Logging;");
        sb.AppendLine("using MyChy.Core.Domains;");
        sb.AppendLine("using MyChy.Frame.Core.Common.Extensions;");
        sb.AppendLine("using MyChy.Frame.Core.Common.Helper;");
        //sb.AppendLine("using MyChy.Plugin.Models.WeiXin;");
        sb.AppendLine("using MyChy.Service.Database;");
        sb.AppendLine("using MyChy.Service.WebAdmin;");
        sb.AppendLine("using MyChy.Service.WebAdminVue;");
        sb.AppendLine("using MyChy.Web.ViewModels.AdminVue;");
        sb.AppendLine($"using MyChy.Web.ViewModels.{i.Namespace};");
        foreach (var y in newservicelist)
        {
            if (y != i.Namespace)
            {
                sb.AppendLine($"using MyChy.Web.ViewModels.{y};");
            }
        }

        sb.AppendLine("using MyChy.Web.WebAdminVue.ActionFilter;");
        sb.AppendLine("");
        sb.AppendLine("namespace MyChy.Web.WebAdminVue.Controllers");
        sb.AppendLine("{");
        sb.AppendLine("    [ApiController]");
        sb.AppendLine("    [Route(\"api/[controller]/[Action]\")]");
        sb.AppendLine($"    public class {i.Namespace}Controller : BaseLoginApiController");
        sb.AppendLine("    {");
        sb.AppendLine($"        private readonly I{i.Namespace}AdminVueService {i.Namespace.ToLower()}AdminVueService;");
        sb.AppendLine($"        private readonly ICompetencesAdminVueService competencesAdminVueService;");
        sb.AppendLine($"        //private readonly I{i.Namespace}AdminService {i.Namespace.ToLower()}AdminService;");
        sb.AppendLine($"        private readonly I{i.Namespace}Service {i.Namespace.ToLower()}Service;");
        sb.AppendLine($"        private readonly ITableAdminVueService tableAdminVueService;");
        sb.AppendLine("        private readonly ILogger _logger;");
        foreach (var y in newservicelist)
        {
            if (y != i.Namespace)
            {
                sb.AppendLine($"private readonly I{y}Service {y.ToLower()}Service;");
                sb.AppendLine($"private readonly I{y}AdminService {y.ToLower()}AdminService;");
            }
        }

        sb.AppendLine($"        public {i.Namespace}Controller(");
        sb.AppendLine($"             ILoggerFactory loggerFactory,");
        sb.AppendLine($"             ICompetencesAdminVueService _competencesAdminVueService,");
        sb.AppendLine($"             I{i.Namespace}Service _{i.Namespace.ToLower()}Service,");
        sb.AppendLine($"             //I{i.Namespace}AdminService _{i.Namespace.ToLower()}AdminService,");
        sb.AppendLine($"             I{i.Namespace}AdminVueService _{i.Namespace.ToLower()}AdminVueService,");
        foreach (var y in newservicelist)
        {
            if (y != i.Namespace)
            {
                sb.AppendLine($"I{y}Service _{y.ToLower()}Service,");
                sb.AppendLine($"I{y}AdminService _{y.ToLower()}AdminService,");
            }
        }
        sb.AppendLine($"             ITableAdminVueService _tableAdminVueService)");
        sb.AppendLine("        {");
        sb.AppendLine("            competencesAdminVueService = _competencesAdminVueService;");
        sb.AppendLine($"            {i.Namespace.ToLower()}AdminVueService = _{i.Namespace.ToLower()}AdminVueService;");
        sb.AppendLine($"            {i.Namespace.ToLower()}Service = _{i.Namespace.ToLower()}Service;");
        sb.AppendLine($"            //{i.Namespace.ToLower()}AdminService = _{i.Namespace.ToLower()}AdminService;");
        sb.AppendLine($"            tableAdminVueService = _tableAdminVueService;");
        foreach (var y in newservicelist)
        {
            if (y != i.Namespace)
            {
                sb.AppendLine($"{y.ToLower()}Service = _{y.ToLower()}Service;");
                sb.AppendLine($"{y.ToLower()}AdminService = _{y.ToLower()}AdminService;");
            }
        }
        sb.AppendLine($"            _logger = loggerFactory.CreateLogger<{i.Namespace}Controller>();");
        sb.AppendLine("");
        sb.AppendLine("        }");

        foreach (var x in i.FileName)
        {
            sb.AppendLine(" ");

            sb.AppendLine($"        #region {x.Description}管理");
            sb.AppendLine("");
            sb.AppendLine("        /// <summary>");
            sb.AppendLine($"        /// {x.Description}列表    ");
            sb.AppendLine("        /// </summary>");
            sb.AppendLine("        /// <returns></returns>");
            sb.AppendLine($"        [AuthorityAction(\"{x.AuthorityCode}\", (int)CompetenceFeatures.List)]");
            sb.AppendLine($"        public async Task<IActionResult> {x.Alias}IndexInit([FromBody] {x.Name}SearchModel Search)");
            sb.AppendLine("        {");
            sb.AppendLine("");
            sb.AppendLine("             Search.IsAllData = false;");
            sb.AppendLine("             Search.BaseLoginName = GetUserCode(_logger);");
            sb.AppendLine($"            //var userinfo = competencesAdminVueService.ShowAdminUserInfo(Search.BaseLoginName);");
            sb.AppendLine($"            var Features = await competencesAdminVueService.ShowUserAuthorityByCodeAsync(Search.BaseLoginName, \"{x.AuthorityCode}\");");
            sb.AppendLine("             if (Features.Contains((int)CompetenceFeatures.AllData))");
            sb.AppendLine("             {");
            sb.AppendLine("                 Search.IsAllData = true;");
            sb.AppendLine("             }");
            sb.AppendLine($"            var list = await {i.Namespace.ToLower()}Service.Show{x.Name}PageAsync(Search);");
            sb.AppendLine($"            var ConvertedList = {i.Namespace.ToLower()}Service.Conversion{x.Name}(list);");
            sb.AppendLine($"            var tabledata = await tableAdminVueService.ShowTableListAsync");
            sb.AppendLine($"            (Search.BaseLoginName, \"{x.Name}\", ConvertedList);");



            sb.AppendLine("");
            sb.AppendLine($"            var result = new {x.Name}IndexViewModel");
            sb.AppendLine("            {");
            sb.AppendLine("                Columns = tabledata.Columns,");
            sb.AppendLine("                Data = tabledata.Data,");
            sb.AppendLine("                Pager = tabledata.Pager,");
            sb.AppendLine("                Permissions = Features,");
            sb.AppendLine("            };");
            sb.AppendLine("");
            sb.AppendLine($"            var resultJson = new BaseVueReqly<{x.Name}IndexViewModel>");
            sb.AppendLine("            {");
            sb.AppendLine("                Success = true,");
            sb.AppendLine("                Data = result");
            sb.AppendLine("            };");
            sb.AppendLine("            var okresult = new JsonResult(resultJson);");
            sb.AppendLine("            return okresult;");
            sb.AppendLine("        }");

            sb.AppendLine(" ");
            sb.AppendLine("        /// <summary>");
            sb.AppendLine($"        /// {x.Description}添加 初始化");
            sb.AppendLine("        /// </summary>");
            sb.AppendLine("        /// <returns></returns>");
            sb.AppendLine($"        [AuthorityAction(\"{x.AuthorityCode}\", (int)CompetenceFeatures.View)]");
            sb.AppendLine($"        public async Task<IActionResult> {x.Alias}AddInit([FromBody] {x.Name}SearchModel Search)");
            sb.AppendLine("        {");
            sb.AppendLine("                    Search.BaseLoginName = GetUserCode(_logger);");
            sb.AppendLine("            //var userinfo = competencesAdminVueService.ShowAdminUserInfo(Search.BaseLoginName);");
            sb.AppendLine($"            var result = new {x.Name}PostViewModel();");
            sb.AppendLine($"            var Features = await competencesAdminVueService.ShowUserAuthorityByCodeAsync(Search.BaseLoginName, \"{x.AuthorityCode}\");");
            sb.AppendLine($"           var model = await {i.Namespace.ToLower()}Service.Show{x.Name}CacheAsync(Search);");
            sb.AppendLine($"           result.PostModel = {i.Namespace.ToLower()}Service.Conversion{x.Name}(model);");
            sb.AppendLine("            result.Permissions = Features;");

            foreach (var y in x.Attributes)
            {
                if (y.Types0f == "Enum")
                {
                    sb.AppendLine($"            result.{y.Name}Select = [.. (typeof({y.EnumName}).GetDescriptionList(0)).Select(x => new HtmlSelectOptionInt()");
                    sb.AppendLine("            {");
                    sb.AppendLine("                Label = x.Title,");
                    sb.AppendLine("                Value = x.Id,");
                    sb.AppendLine("            })];");
                }

                foreach (var z in y.List)
                {
                    switch (z.Name)
                    {
                        case "EnumListStringAttribute":
                            sb.AppendLine($"            if (result.PostModel.{y.Name} == 0) result.PostModel.{y.Name} = null;");
                            sb.Append($"            result.{y.Name}Select=");
                            sb.AppendLine("             await expandsService.ShowEnumListVualeHtmlSelectOptionIntCacheAsync(");
                            sb.Append($"            new ViewModels.Expands.EnumListVualeSearchModel()");
                            sb.Append("{ EnumListCoding =");
                            sb.Append($"\"{z.Code}\", State = true");
                            sb.AppendLine("             }); ");
                            sb.AppendLine("             ");
                            break;
                        case "EnumListCheckAttribute":
                            sb.AppendLine($"            if (result.PostModel.{y.Name} == 0) result.PostModel.{y.Name} = null;");
                            sb.Append($"result.{y.Name}Select=");
                            sb.AppendLine("             await expandsService.ShowEnumListVualeHtmlSelectOptionIntCacheAsync(");
                            sb.Append($"new ViewModels.Expands.EnumListCheckSearchModel()");
                            sb.Append("{ EnumListCoding =");
                            sb.Append($"\"{z.Code}\", TableId= result.Post.Id, State = true");
                            sb.AppendLine("             }); ");
                            sb.AppendLine("             ");
                            break;
                        case "TableToAttribute":
                            sb.AppendLine($"            if (result.PostModel.{y.Name} == 0) result.PostModel.{y.Name} = null;");
                            if (z.Two == "BaseArea" || string.IsNullOrEmpty(z.Two))
                            {
                                sb.Append($"var {z.One}List = await {i.Namespace.ToLower()}Service.Show{z.One}ListCacheAsync(new {z.One}SearchModel ");
                            }
                            else
                            {
                                sb.Append($"var {z.One}List = await {z.Two.ToLower()}Service.Show{z.One}ListCacheAsync(new {z.One}SearchModel ");
                            }
                            sb.AppendLine("{ State = true });");

                            sb.Append($"            result.{y.Name}Select = {z.One}List.Select(x => new HtmlSelectOptionInt()");
                            sb.AppendLine("{");
                            sb.Append($" Label = x.{z.Three},");
                            sb.Append($" Value = x.Id");
                            sb.AppendLine("             }).ToList();");
                            break;
                    }
                }
            }

            sb.AppendLine("            if (!(model?.Id > 0))");
            sb.AppendLine("            {");
            foreach (var y in x.Attributes)
            {
                if (y.Types0f == "Enum")
                {
                    sb.AppendLine($"                result.PostModel.{y.Name} = null;");
                }
            }
            sb.AppendLine("            }");

            sb.AppendLine("");
            sb.AppendLine($"            var resultJson = new BaseVueReqly<{x.Name}PostViewModel>");
            sb.AppendLine("            {");
            sb.AppendLine("                Success = true,");
            sb.AppendLine("                Data = result");
            sb.AppendLine("            };");
            sb.AppendLine("            var okresult = new JsonResult(resultJson);");
            sb.AppendLine("");
            sb.AppendLine("            return okresult;");
            sb.AppendLine("        }");

            sb.AppendLine("        /// <summary>");
            sb.AppendLine($"        /// {x.Description}添加-提交 ");
            sb.AppendLine("        /// </summary>");
            sb.AppendLine("        /// <returns></returns>");
            sb.AppendLine($"        [AuthorityAction(\"{x.AuthorityCode}\", (int)CompetenceFeatures.Modify)]");
            sb.AppendLine("        [HttpPost]");
            sb.AppendLine($"        public async Task<IActionResult> {x.Alias}Add([FromBody] {x.Name}PostModel postModel)");
            sb.AppendLine("        {");
            sb.AppendLine("");
            sb.AppendLine("            postModel.BaseLoginName = GetUserCode(_logger);");
            sb.AppendLine("            //var userinfo = competencesAdminVueService.ShowAdminUserInfo(postModel.BaseLoginName);");
            sb.AppendLine($"            //var Features = await competencesAdminVueService.ShowUserAuthorityByCodeAsync(postModel.BaseLoginName, \"{x.AuthorityCode}\");");
            sb.AppendLine("");
            sb.AppendLine($"            var result = await {i.Namespace.ToLower()}AdminVueService.Save{x.Name}PostAsync(postModel);");
            sb.AppendLine("            var resultJson = new BaseVueReqly");
            sb.AppendLine("            {");
            sb.AppendLine("                Success = true,");
            sb.AppendLine("            };");
            sb.AppendLine("            if (!result.Success)");
            sb.AppendLine("            {");
            sb.AppendLine("                resultJson.Success = false;");
            sb.AppendLine("                resultJson.Msg = result.Msg;");
            sb.AppendLine("            }");
            sb.AppendLine("");
            sb.AppendLine("            var okresult = new JsonResult(resultJson);");
            sb.AppendLine("            return okresult;");
            sb.AppendLine("");
            sb.AppendLine("        }");

            sb.AppendLine("                /// <summary>");
            sb.AppendLine($"        /// {x.Description}导入-初始化");
            sb.AppendLine("        /// </summary>");
            sb.AppendLine("        /// <param name=\"Search\"></param>");
            sb.AppendLine("        /// <returns></returns>");
            sb.AppendLine($"        [AuthorityAction(\"{x.AuthorityCode}\", (int)CompetenceFeatures.Import)]");
            sb.AppendLine($"        public async Task<IActionResult>  {x.Alias}ImportInit([FromBody] ImportSearchModel Search)");
            sb.AppendLine("        {");
            sb.AppendLine("                    Search.BaseLoginName = GetUserCode(_logger);");
            sb.AppendLine("            //var userinfo = competencesAdminVueService.ShowAdminUserInfo(Search.BaseLoginName);");
            sb.AppendLine($"            var Features = await competencesAdminVueService.ShowUserAuthorityByCodeAsync(Search.BaseLoginName, \"{x.AuthorityCode}\");");
            sb.AppendLine("");
            sb.AppendLine("            var result = new ImportViewModel()");
            sb.AppendLine("            {");
            sb.AppendLine("                Post = new ImportPostModel(),");
            sb.AppendLine("                Permissions =Features");
            sb.AppendLine("            };");
            sb.AppendLine("            var resultJson = new BaseVueReqly<ImportViewModel>");
            sb.AppendLine("            {");
            sb.AppendLine("                Success = true,");
            sb.AppendLine("                Data = result");
            sb.AppendLine("            };");
            sb.AppendLine("            var okresult = new JsonResult(resultJson);");
            sb.AppendLine("");
            sb.AppendLine("            return okresult;");
            sb.AppendLine("        }");
            sb.AppendLine("");
            sb.AppendLine("        /// <summary>");
            sb.AppendLine($"        /// {x.Description}导入-提交");
            sb.AppendLine("        /// </summary>");
            sb.AppendLine("        /// <param name=\"PostModel\"></param>");
            sb.AppendLine("        /// <returns></returns>");
            sb.AppendLine("        [HttpPost]");
            sb.AppendLine($"        [AuthorityAction(\"{x.AuthorityCode}\", (int)CompetenceFeatures.Import)]");
            sb.AppendLine($"        public async Task<IActionResult> {x.Alias}Import([FromBody] ImportPostModel postModel)");
            sb.AppendLine("        {");
            sb.AppendLine($"            postModel.BaseLoginName = GetUserCode(_logger);");
            sb.AppendLine($"           var result = await {i.Namespace.ToLower()}AdminVueService.Import{x.Name}PostAsync(postModel);");
            sb.AppendLine("            var resultJson = new BaseVueReqly");
            sb.AppendLine("            {");
            sb.AppendLine("                Success = true,");
            sb.AppendLine("            };");
            sb.AppendLine("            if (!result.Success)");
            sb.AppendLine("            {");
            sb.AppendLine("                resultJson.Success = false;");
            sb.AppendLine("                resultJson.Msg = result.Msg;");
            sb.AppendLine("            }");
            sb.AppendLine("");
            sb.AppendLine("            var okresult = new JsonResult(resultJson);");
            sb.AppendLine("            return okresult;");
            sb.AppendLine("        }");
            sb.AppendLine("");
            sb.AppendLine("        /// <summary>");
            sb.AppendLine($"        /// {x.Description}导出");
            sb.AppendLine("        /// </summary>");
            sb.AppendLine("        /// <param name=\"Search\"></param>");
            sb.AppendLine("        /// <returns></returns>");
            sb.AppendLine($"        [AuthorityAction(\"{x.AuthorityCode}\", (int)CompetenceFeatures.Export)]");
            sb.AppendLine($"        public async Task<IActionResult> {x.Alias}Export([FromBody] {x.Name}SearchModel Search)");
            sb.AppendLine("        {");
            sb.AppendLine("                    Search.BaseLoginName = GetUserCode(_logger);");
            sb.AppendLine($"            var result = await {i.Namespace.ToLower()}AdminVueService.Export{x.Name}PostAsync(Search);");
            sb.AppendLine("            if (!result.Success)");
            sb.AppendLine("            {");
            sb.AppendLine("                 return ReturnErrorResult(\"报表导出失败！\"+ result.Msg);");
            sb.AppendLine("            }");
            sb.AppendLine("            else");
            sb.AppendLine("            {");
            sb.AppendLine("                return File(result.Code, \"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet\", result.Msg);");
            sb.AppendLine("            }");
            sb.AppendLine("");
            sb.AppendLine("            }");
            sb.AppendLine("  #endregion ");
        }

        sb.AppendLine("        }");
        sb.AppendLine("    }");

        await _sw.WriteAsync(sb.ToString());
        _sw.Close();

    }






}
