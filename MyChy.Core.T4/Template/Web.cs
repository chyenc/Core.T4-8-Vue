using MyChy.Core.T4.Common;
using MyChy.Frame.Core.Common.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MyChy.Core.T4.Template
{
    public class Web
    {
        private const string IPath = "/MyChy.Web.WebAdmin";
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
            await CreatControllers(file, list);

        }

        private async Task CreatControllers(string Path, IList<MyChyEntityNamespace> list)
        {
            StringBuilder sb = new StringBuilder();
            var newservicelist = new List<string>();
            foreach (var i in list)
            {

                sb = new StringBuilder();
                string files = Path + $"/{i.Namespace}Controller.cs";
                var _sw = new StreamWriter(new FileStream(files, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.Read), Encoding.UTF8);

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
                sb.AppendLine("using System.Threading.Tasks;");

                sb.AppendLine("using System.Linq;");
                sb.AppendLine("using Microsoft.AspNetCore.Mvc.Rendering;");

                sb.AppendLine("using MyChy.Frame.Core.Common.Helper;");
                sb.AppendLine("using MyChy.Service.Database;");
                sb.AppendLine("using MyChy.Service.Plugin.Caches;");
                sb.AppendLine($"using MyChy.Web.ViewModels.{i.Namespace};");
                sb.AppendLine("using MyChy.Web.ViewModels.Admin;");
                sb.AppendLine(" using MyChy.Frame.Core.Common.Model;");
               

                foreach (var y in newservicelist)
                {
                    if (y != i.Namespace)
                    {
                        sb.AppendLine($"using MyChy.Web.ViewModels.{y};");
                    }
                }
                sb.AppendLine("using MyChy.Web.WebAdmin.ActionFilter;");
                sb.AppendLine("using MyChy.Service.WebAdmin;");
                sb.AppendLine("using MyChy.Core.Domains;");
                sb.AppendLine("using MyChy.Frame.Core.Common.Extensions;");
                sb.AppendLine("using MyChy.Frame.Core.Services;");
                sb.AppendLine("namespace MyChy.Web.WebAdmin.Controllers");
                sb.AppendLine("{");
                sb.AppendLine($"public class {i.Namespace}Controller : BaseLoginController");
                sb.AppendLine("{");
                sb.AppendLine($"private readonly I{i.Namespace}Service {i.Namespace.ToLower()}Service;");
                sb.AppendLine($"private readonly I{i.Namespace}AdminService {i.Namespace.ToLower()}AdminService;");

                foreach (var y in newservicelist)
                {
                    if (y != i.Namespace)
                    {
                        sb.AppendLine($"private readonly I{y}Service {y.ToLower()}Service;");
                        sb.AppendLine($"private readonly I{y}AdminService {y.ToLower()}AdminService;");
                    }
                }
                sb.AppendLine("private readonly IGeneralCacheServer generalCacheServer;");
                sb.AppendLine("private readonly ICompetencesAdminService competencesAdminService;");
                sb.AppendLine("private readonly ILogger _logger;");
                sb.AppendLine($"public {i.Namespace}Controller(");
                sb.AppendLine($"I{i.Namespace}Service _{i.Namespace.ToLower()}Service");
                sb.AppendLine($",I{i.Namespace}AdminService _{i.Namespace.ToLower()}AdminService");

                foreach (var y in newservicelist)
                {
                    if (y != i.Namespace)
                    {
                        sb.AppendLine($",I{y}Service _{y.ToLower()}Service");
                        sb.AppendLine($",I{y}AdminService _{y.ToLower()}AdminService");
                    }
                }
                sb.AppendLine(", IGeneralCacheServer _generalCacheServer");
                sb.AppendLine(", ICompetencesAdminService _competencesAdminService");
                sb.AppendLine(", ILoggerFactory loggerFactory)");
                sb.AppendLine("{");
                sb.AppendLine($"{i.Namespace.ToLower()}Service = _{i.Namespace.ToLower()}Service;");
                sb.AppendLine($"{i.Namespace.ToLower()}AdminService = _{i.Namespace.ToLower()}AdminService;");

                foreach (var y in newservicelist)
                {
                    if (y != i.Namespace)
                    {
                        sb.AppendLine($"{y.ToLower()}Service = _{y.ToLower()}Service;");
                        sb.AppendLine($"{y.ToLower()}AdminService = _{y.ToLower()}AdminService;");
                    }
                }
                sb.AppendLine("generalCacheServer = _generalCacheServer;");
                sb.AppendLine("competencesAdminService = _competencesAdminService;");
                sb.AppendLine($"_logger = loggerFactory.CreateLogger<{i.Namespace}Controller>();");

                sb.AppendLine("}");
                foreach (var x in i.FileName)
                {
                    sb.AppendLine(" ");

                    sb.AppendLine($"#region {x.Description}管理");
                    sb.AppendLine("/// <summary>");
                    sb.AppendLine("/// 列表");
                    sb.AppendLine("/// </summary>");
                    sb.AppendLine("/// <param name=\"Search\"></param>");
                    sb.AppendLine("/// <returns></returns>");
                    sb.AppendLine($"[AuthorityAction(\"{x.AuthorityCode}\", (int)CompetenceFeatures.List)]");
                    sb.AppendLine($"public async Task<IActionResult> {x.Name}Index({x.Name}SearchModel Search)");
                    sb.AppendLine("{");
                    sb.AppendLine($"Search = generalCacheServer.ShowSeacrhWeb(\"{i.Namespace}_{x.Name}Index\", Search);");
                    sb.AppendLine($"var Features = competencesAdminService.ShowUserAuthorityByCode(\"{x.AuthorityCode}\");");
                    sb.AppendLine($"var userinfo = AdminIdentityServer.AccountUserid();");
                    sb.AppendLine($"if (Features.Contains((int)CompetenceFeatures.AllData))");
                    sb.AppendLine("{");
                    sb.AppendLine("    Search.IsAllData = true;");
                   sb.AppendLine(" }");
                    sb.AppendLine("else");
                    sb.AppendLine("{");
                    sb.AppendLine("    Search.IsAllData = false;");
                    sb.AppendLine("}");

                    sb.AppendLine($"var list = await {i.Namespace.ToLower()}Service.Show{x.Name}PageAsync(Search);");
                    sb.AppendLine($"{x.Name}ViewModel viewModel = new {x.Name}ViewModel");
                    sb.AppendLine("{");
                    sb.AppendLine($"List = {i.Namespace.ToLower()}Service.Conversion{x.Name}(list),");
                    sb.AppendLine("Search = Search,");
                    sb.AppendLine($" Features = Features,");
                    sb.AppendLine("};");
                    sb.AppendLine("return View(viewModel);");
                    sb.AppendLine("}");
                    sb.AppendLine(" ");

                    if (!x.IsViewEntity)
                    {

                        sb.AppendLine("/// <summary>");
                        sb.AppendLine("/// 编辑-页面");
                        sb.AppendLine("/// </summary>");
                        sb.AppendLine("/// <param name=\"Search\"></param>");
                        sb.AppendLine("/// <returns></returns>");
                        sb.AppendLine($"[AuthorityAction(\"{x.AuthorityCode}\", (int)CompetenceFeatures.View)]");
                        sb.AppendLine($"public async Task<IActionResult> {x.Name}Edit({x.Name}SearchModel Search)");
                        sb.AppendLine("{");
                        sb.AppendLine($"var result =  new {x.Name}PostViewModel();");

                        sb.AppendLine($"var Features = competencesAdminService.ShowUserAuthorityByCode(\"{x.AuthorityCode}\");");
                        sb.AppendLine($"var userinfo = AdminIdentityServer.AccountUserid();");
                        sb.AppendLine($"if (Features.Contains((int)CompetenceFeatures.AllData))");
                        sb.AppendLine("{");
                        sb.AppendLine("    Search.IsAllData = true;");
                        sb.AppendLine(" }");
                        sb.AppendLine("else");
                        sb.AppendLine("{");
                        sb.AppendLine("    Search.IsAllData = false;");
                        sb.AppendLine("}");

                        sb.AppendLine($"var model = await {i.Namespace.ToLower()}Service.Show{x.Name}Async(Search);");
                        sb.AppendLine($"result.Post =  {i.Namespace.ToLower()}Service.Conversion{x.Name}(model);");
                        sb.AppendLine($"result.Features = Features;");
                        sb.AppendLine(" ");
                        foreach (var y in x.Attributes)
                        {
                            if (y.Types0f == "Attributes")
                            {
                                switch (y.AttributeName)
                                {
                                    case "EnumListStringAttribute":
                                        sb.Append($"result.{y.Name}Select=");
                                        sb.AppendLine(" await expandsService.ShowEnumListVualeSelectListCacheAsync(");
                                        sb.Append($"new ViewModels.Expands.EnumListVualeSearchModel()");
                                        sb.Append("{ EnumListCoding =");
                                        sb.Append($"\"{y.AttributeCode}\", State = true");
                                        sb.AppendLine(" }); ");
                                        sb.AppendLine(" ");
                                        break;
                                    case "EnumListCheckAttribute":
                                        sb.Append($"result.{y.Name}Select=");
                                        sb.AppendLine(" await expandsService.ShowEnumListCheckSelectListCacheAsync(");
                                        sb.Append($"new ViewModels.Expands.EnumListCheckSearchModel()");
                                        sb.Append("{ EnumListCoding =");
                                        sb.Append($"\"{y.AttributeCode}\", TableId= result.Post.Id, State = true");
                                        sb.AppendLine(" }); ");
                                        sb.AppendLine(" ");
                                        break;
                                    case "TableToAttribute":
                                        if (y.AttributeTwo == "BaseArea"|| string.IsNullOrEmpty(y.AttributeTwo))
                                        {
                                            sb.Append($"var {y.AttributeOne}List = await {i.Namespace.ToLower()}Service.Show{y.AttributeOne}ListCacheAsync(new {y.AttributeOne}SearchModel ");
                                        }
                                        else
                                        {
                                            sb.Append($"var {y.AttributeOne}List = await {y.AttributeTwo.ToLower()}Service.Show{y.AttributeOne}ListCacheAsync(new {y.AttributeOne}SearchModel ");
                                        }
                                        sb.AppendLine("{ State = true });");

                                        sb.Append($"result.{y.Name}Select = {y.AttributeOne}List.Select(x => new SelectListItem()");
                                        sb.AppendLine("{");
                                        sb.Append($" Text = x.{y.AttributeThree},");
                                        sb.Append($" Value = x.Id.ToString()");
                                        sb.AppendLine(" }).ToList();");
                                        break;
                                }
                            }
                        }

                        sb.AppendLine("return View(result);");
                        sb.AppendLine("}");
                        sb.AppendLine(" ");

                        sb.AppendLine("/// <summary>");
                        sb.AppendLine("/// 编辑Post");
                        sb.AppendLine("/// </summary>");
                        sb.AppendLine("/// <param name=\"PostModel\"></param>");
                        sb.AppendLine("/// <returns></returns>");
                        sb.AppendLine("[HttpPost]");
                        sb.AppendLine("[ValidateAntiForgeryToken]");
                        sb.AppendLine($"[AuthorityAction(\"{x.AuthorityCode}\", (int)CompetenceFeatures.Modify)]");
                        sb.AppendLine($"public async Task<IActionResult> {x.Name}Edit({x.Name}PostModel PostModel)");
                        sb.AppendLine("{");
                        foreach (var y in x.Attributes)
                        {
                            if (y.Types0f == "Attributes")
                            {
                                switch (y.AttributeName)
                                {
                                    case "EnumListCheckAttribute":
                                        sb.Append($"var {y.Name}String = await expandsService.ShowEnumListVualeByCodingCacheAsync(");
                                        sb.AppendLine($" \"{y.AttributeCode}\", PostModel.{y.Name}List);");
                                        sb.AppendLine($"if (!string.IsNullOrEmpty({y.Name}String) ) PostModel.{y.Name} = {y.Name}String;");
                                        sb.AppendLine(" ");
                                        break;
                                }
                            }
                        }

                        sb.AppendLine($"var model = await {i.Namespace.ToLower()}AdminService.Save{x.Name}PostAsync(PostModel);");
                        sb.AppendLine("if (model.Success) { ");
                        foreach (var y in x.Attributes)
                        {
                            if (y.Types0f == "Attributes")
                            {
                                switch (y.AttributeName)
                                {
                                    case "EnumListCheckAttribute":
                                        sb.Append($"await expandsService.SaveEnumListCheckByEnumListCodingPostAsync");
                                        sb.AppendLine($"(\"{y.AttributeCode}\", model.Id,PostModel.{y.Name}List);");
                                        sb.AppendLine(" ");
                                        break;
                                }
                            }
                        }
                        sb.AppendLine($" model.Code = \"/{i.Namespace}/{x.Name}Index\"; ");
                        sb.AppendLine(" }");
                        sb.AppendLine("return Json(model);");
                        sb.AppendLine("}");

                        sb.AppendLine(" ");
                        sb.AppendLine("/// <summary>");
                        sb.AppendLine("/// 删除");
                        sb.AppendLine("/// </summary>");
                        sb.AppendLine("/// <param name=\"PostModel\"></param>");
                        sb.AppendLine("/// <returns></returns>");
                        sb.AppendLine("//[HttpPost]");
                        sb.AppendLine("//[ValidateAntiForgeryToken]");
                        sb.AppendLine($"//[AuthorityAction(\"{x.AuthorityCode}\", (int)CompetenceFeatures.Delete)]");
                        sb.AppendLine($"//public async Task<IActionResult> {x.Name}Delete({x.Name}SearchModel PostModel)");
                        sb.AppendLine("//{");
                        sb.AppendLine($"//var model = await {i.Namespace.ToLower()}Service.Delete{x.Name}PostAsync(PostModel);");
                        sb.AppendLine("//return Json(model);");
                        sb.AppendLine("//}");


                        sb.AppendLine(" ");
                        sb.AppendLine("/// <summary>");
                        sb.AppendLine("/// 导入-页面");
                        sb.AppendLine("/// </summary>");
                        sb.AppendLine("/// <param name=\"Search\"></param>");
                        sb.AppendLine("/// <returns></returns>");
                        sb.AppendLine($"[AuthorityAction(\"{x.AuthorityCode}\", (int)CompetenceFeatures.Import)]");
                        sb.AppendLine($"public IActionResult {x.Name}Import(ImportSearchModel Search)");
                        sb.AppendLine("{");

                        sb.AppendLine($"var result = new ImportViewModel()");
                        sb.AppendLine("{");
                        sb.AppendLine($" Post = new ImportPostModel(),");
                        sb.AppendLine($" Features = competencesAdminService.ShowUserAuthorityByCode(\"{x.AuthorityCode}\")");
                        sb.AppendLine("};");
                        sb.AppendLine($"return View(result);");
                        sb.AppendLine("}");

                        sb.AppendLine(" ");
                        sb.AppendLine("/// <summary>");
                        sb.AppendLine("/// 导入-提交");
                        sb.AppendLine("/// </summary>");
                        sb.AppendLine("/// <param name=\"PostModel\"></param>");
                        sb.AppendLine("/// <returns></returns>");
                        sb.AppendLine("[HttpPost]");
                        sb.AppendLine("[ValidateAntiForgeryToken]");
                        sb.AppendLine($"[AuthorityAction(\"{x.AuthorityCode}\", (int)CompetenceFeatures.Import)]");
                        sb.AppendLine($"public async Task<IActionResult> {x.Name}Import(ImportPostModel PostModel)");
                        sb.AppendLine("{");

                        sb.AppendLine($"var model = new ResultBaseModel();");
                        sb.AppendLine($"model = await {i.Namespace.ToLower()}AdminService.Import{x.Name}PostAsync(PostModel);");
                        sb.AppendLine("if (model.Success) { ");
                        sb.AppendLine($" model.Code = \"/{i.Namespace}/{x.Name}Index\"; ");
                        sb.AppendLine(" }");
                        sb.AppendLine("return Json(model);");

                        sb.AppendLine("}");

                        sb.AppendLine(" ");
                        sb.AppendLine($"/// <summary>");
                        sb.AppendLine($"/// 导出");
                        sb.AppendLine($"/// </summary>");
                        sb.AppendLine($"/// <param name=\"Search\"></param>");
                        sb.AppendLine($"/// <returns></returns>");
                        sb.AppendLine($"[AuthorityAction(\"{x.AuthorityCode}\", (int)CompetenceFeatures.Export)]");
                        sb.AppendLine($"public async Task<IActionResult> {x.Name}Excel({x.Name}SearchModel Search)");
                        sb.AppendLine("{");
                        sb.AppendLine($"var model = new ResultBaseModel();");
                        sb.AppendLine($"model = await {i.Namespace.ToLower()}AdminService.Export{x.Name}PostAsync(Search);");
                        sb.AppendLine($"if (!model.Success)");
                        sb.AppendLine("{");
                        sb.AppendLine($"return new RedirectResult(\"/Error/\");");
                        sb.AppendLine("}");
                        sb.AppendLine($"else");
                        sb.AppendLine("{");
                        sb.AppendLine($"return File(model.Code, \"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet\", model.Msg);");
                        sb.AppendLine("}");
                        sb.AppendLine($"");
                        sb.AppendLine("}");



                    }
                    sb.AppendLine("#endregion");
                    sb.AppendLine(" ");
                }
                sb.AppendLine("}");
                sb.AppendLine("}");

                await _sw.WriteAsync(sb.ToString());
                _sw.Close();
            }

        }
    }
}
