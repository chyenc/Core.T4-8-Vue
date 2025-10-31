using Microsoft.AspNetCore.Cors.Infrastructure;
using MyChy.Core.T4.Common;
using MyChy.Frame.Core.Common.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MyChy.Core.T4.Template
{
    public class Service
    {
        private const string IPath = "/MyChy.Service.Database";
        private const string IMappings = "/Mappings";
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

            await CreatMappings(file, list);


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
                sb.AppendFormat("services.AddTransient<I{0}Service, {0}Service>();", i.Namespace);
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
                string files = Path + $"/I{i.Namespace}Service.cs";
                var _sw = new StreamWriter(new FileStream(files, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.Read), Encoding.UTF8);

                sb.AppendLine("using System.Threading.Tasks;");
                sb.AppendLine("using System.Collections.Generic;");
                sb.AppendLine($"using MyChy.Core.Domains;");
                sb.AppendLine($"using MyChy.Core.Domains.{i.Namespace};");
                sb.AppendLine("using MyChy.Frame.Core.Common.Model;");
                sb.AppendLine("using MyChy.Frame.Core.Services;");
                sb.AppendLine("using MyChy.Web.ViewModels;");
                sb.AppendLine($"using MyChy.Web.ViewModels.{i.Namespace};");
                sb.AppendLine("using MyChy.Service.Plugin.Caches;");
                sb.AppendLine("using MyChy.Service.Plugin.Config;");

                sb.AppendLine("namespace MyChy.Service.Database");

                sb.AppendLine("{");
                sb.AppendLine($"public interface I{i.Namespace}Service : IServiceBase");
                sb.AppendLine("{");
                foreach (var x in i.FileName)
                {
                    sb.AppendLine(" ");

                    sb.AppendLine(" ");

                    sb.AppendLine($"#region {x.Description}管理");
                    sb.AppendLine("/// <summary>");
                    sb.AppendLine($"/// 显示分页{x.Description}");
                    sb.AppendLine("/// </summary>");
                    sb.AppendLine("/// <returns></returns>");
                    sb.AppendLine($"Task<IPagedList<{x.Name}>> Show{x.Name}PageAsync({x.Name}SearchModel Search, int pageSize = 15);");
                    sb.AppendLine(" ");

                    if (!x.IsViewEntity)
                    {

                        sb.AppendLine("/// <summary>");
                        sb.AppendLine($"/// 显示List{x.Description}");
                        sb.AppendLine("/// </summary>");
                        sb.AppendLine("/// <returns></returns>");
                        sb.AppendLine($"Task<IList<{x.Name}>> Show{x.Name}ListAsync({x.Name}SearchModel Search);");
                        sb.AppendLine(" ");

                        sb.AppendLine("/// <summary>");
                        sb.AppendLine($"/// 显示{x.Description}List信息带缓存");
                        sb.AppendLine("/// </summary>");
                        sb.AppendLine("/// <param name=\"\"></param>");
                        sb.AppendLine("/// <returns></returns>");
                        sb.AppendLine($"Task<IList<{x.Name}>> Show{x.Name}ListCacheAsync({x.Name}SearchModel Search,int Second = 360);");
                        sb.AppendLine(" ");

                        sb.AppendLine("/// <summary>");
                        sb.AppendLine($"/// 显示{x.Description}信息");
                        sb.AppendLine("/// </summary>");
                        sb.AppendLine("/// <param name=\"\"></param>");
                        sb.AppendLine("/// <returns></returns>");
                        sb.AppendLine($"Task<{x.Name}> Show{x.Name}Async({x.Name}SearchModel Search );");
                        sb.AppendLine(" ");



                        sb.AppendLine("/// <summary>");
                        sb.AppendLine($"/// 显示{x.Description}信息带缓存");
                        sb.AppendLine("/// </summary>");
                        sb.AppendLine("/// <param name=\"\"></param>");
                        sb.AppendLine("/// <returns></returns>");
                        sb.AppendLine($"Task<{x.Name}> Show{x.Name}CacheAsync({x.Name}SearchModel Search,int Second = 360);");
                        sb.AppendLine(" ");

                    }

                    sb.AppendLine("/// <summary>");
                    sb.AppendLine($"/// 转化分页{x.Description}List");
                    sb.AppendLine("/// </summary>");
                    sb.AppendLine("/// <returns></returns>");
                    sb.Append($"IPagedList<{x.Name}PostModel> Conversion{x.Name}(IPagedList<{x.Name}> list");
                    if (x.IsThumbnail)
                    {
                        sb.Append($", ThumbnailModel thumbnailModel=null");
                    }
                    sb.AppendLine($");");


                    sb.AppendLine(" ");

                    if (!x.IsViewEntity)
                    {
                        sb.AppendLine("/// <summary>");
                        sb.AppendLine($"/// 转化{x.Description}List");
                        sb.AppendLine("/// </summary>");
                        sb.AppendLine("/// <returns></returns>");
                        sb.Append($"IList<{x.Name}PostModel> Conversion{x.Name}(IList<{x.Name}> list");
                        if (x.IsThumbnail)
                        {
                            sb.Append($",ThumbnailModel thumbnailModel=null");
                        }
                        sb.AppendLine($");");

                        sb.AppendLine(" ");

                        sb.AppendLine("/// <summary>");
                        sb.AppendLine($"/// 转化{x.Description}");
                        sb.AppendLine("/// </summary>");
                        sb.AppendLine("/// <returns></returns>");
                        sb.Append($"{x.Name}PostModel Conversion{x.Name}({x.Name} Model");
                        if (x.IsThumbnail)
                        {
                            sb.Append($", ThumbnailModel thumbnailModel=null");
                        }
                        sb.AppendLine($");");
                        sb.AppendLine(" ");
                    }

                    if (!x.IsViewEntity)
                    {
                        sb.AppendLine("/// <summary>");
                        sb.AppendLine($"/// 保存{x.Description}{x.Description}");
                        sb.AppendLine("/// </summary>");
                        sb.AppendLine("/// <returns></returns>");
                        sb.AppendLine($"Task<ResultBaseModel> Save{x.Name}PostAsync({x.Name}PostModel PostModel, string DefUserName = \"SyStem\");");
                        sb.AppendLine(" ");

                        sb.AppendLine(" ");
                        sb.AppendLine("/// <summary>");
                        sb.AppendLine($"/// 批量保存{x.Description}");
                        sb.AppendLine("/// </summary>");
                        sb.AppendLine("/// <returns></returns>");
                        sb.AppendLine($"Task<ResultBaseModel> Save{x.Name}ListAsync(IList<{x.Name}> List);");

                        sb.AppendLine("/// <summary>");
                        sb.AppendLine($"/// 删除{x.Description}");
                        sb.AppendLine("/// </summary>");
                        sb.AppendLine("/// <param name=\"Search\"></param>");
                        sb.AppendLine("/// <returns></returns>");
                        sb.AppendLine($"Task<ResultBaseModel> Delete{x.Name}PostAsync({x.Name}SearchModel Search);");
                        sb.AppendLine(" ");



                        sb.AppendLine(" ");
                        sb.AppendLine("/// <summary>");
                        sb.AppendLine($"/// 移除{x.Description}缓存信息");
                        sb.AppendLine("/// </summary>");
                        sb.AppendLine("/// <param name=\"\"></param>");
                        sb.AppendLine("/// <returns></returns>");
                        sb.AppendLine($"void Remove{x.Name}CacheAsync({x.Name}SearchModel Search,CacheKeyType cacheKeyType);");
                        sb.AppendLine(" ");
                    }

                    //sb.AppendLine(" ");
                    //sb.AppendLine("/// <summary>");
                    //sb.AppendLine($"/// 移除{x.Description}List缓存信息");
                    //sb.AppendLine("/// </summary>");
                    //sb.AppendLine("/// <param name=\"\"></param>");
                    //sb.AppendLine("/// <returns></returns>");
                    //sb.AppendLine($"void Remove{x.Name}ListCacheAsync({x.Name}SearchModel Search);");
                    //sb.AppendLine(" ");

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
            var newservicelist = new List<string>();
            foreach (var i in list)
            {
                sb = new StringBuilder();
                string files = Path + $"/{i.Namespace}Service.cs";
                var _sw = new StreamWriter(new FileStream(files, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.Read), Encoding.UTF8);

                var isPicture = false;
                newservicelist = new List<string>();
                foreach (var y1 in i.FileName)
                {
                    foreach (var y2 in y1.Attributes)
                    {
                        if (y2.Name == "Picture")
                        {
                            isPicture = true;
                            break;
                        }
                    }

                    foreach (var y3 in y1.ServiceList)
                    {
                        if (!newservicelist.Contains(y3))
                        {
                            newservicelist.Add(y3);
                        }

                    }

                }


                sb.AppendLine("using AutoMapper;");
                sb.AppendLine("using LinqKit;");
                sb.AppendLine("using System;");
                sb.AppendLine("using System.Collections.Generic;");
                sb.AppendLine("using System.Threading.Tasks;");
                sb.AppendLine("using System.Linq;");
                sb.AppendLine("using Microsoft.Extensions.Logging;");
                sb.AppendLine("using Microsoft.EntityFrameworkCore;");
                //sb.AppendLine("using Microsoft.Data.SqlClient;");
                sb.AppendLine("using MyChy.Service.Database;");
                sb.AppendLine("using MyChy.Frame.Core.Common.Model;");
                sb.AppendLine("using MyChy.Frame.Core.EFCore.Entitys.Pages;");
                sb.AppendLine("using MyChy.Frame.Core.Services;");
                sb.AppendLine("using MyChy.Core.Domains;");
                sb.AppendLine("using MyChy.Web.ViewModels;");
                sb.AppendLine($"using MyChy.Web.ViewModels.{i.Namespace};");
                sb.AppendLine($"using MyChy.Core.Domains.{i.Namespace};");
                sb.AppendLine($"using MyChy.Core.Data.{i.Namespace};");
                sb.AppendLine("using MyChy.Service.Plugin.Caches;");
                sb.AppendLine("using MyChy.Service.Database.Common;");
                sb.AppendLine("using MyChy.Frame.Core.Common.Extensions;");
                sb.AppendLine("using MyChy.Service.Plugin.Config;");
                sb.AppendLine("using MyChy.Web.ViewModels.Logs;");
                
                foreach (var y in newservicelist)
                {
                    sb.AppendLine($"using MyChy.Web.ViewModels.{y};");
                }


                if (isPicture)
                {
                    //sb.AppendLine("using MyChy.Web.ViewModels.CommonUse;");
                    sb.AppendLine("using MyChy.Plugin.Models.Config;");

                }

                sb.AppendLine("namespace MyChy.Service.Database.Implementation");
                sb.AppendLine("{");

                sb.AppendLine($"public class {i.Namespace}Service : ServiceBase, I{i.Namespace}Service");
                sb.AppendLine("{");
                sb.AppendLine($"private readonly I{i.Namespace}WorkArea _{i.Namespace}Work;");
                sb.AppendLine("private readonly IGeneralCacheServer generalCacheServer;");
                sb.AppendLine($"private readonly ILogger _logger;");
                sb.AppendLine($"private readonly IMapper mapper;");
                sb.AppendLine($"private readonly ILogsService logsService;");

                foreach (var y in newservicelist)
                {
                    sb.AppendLine($"private readonly I{y}Service {y.ToLower()}Service;");
                }

                sb.AppendLine($"private readonly string CacheKey = \"{i.Namespace}S_\";");
                if (isPicture)
                {
                    sb.AppendLine("private readonly IConfigServerice configServerice;");
                    sb.AppendLine("private readonly UploadConfig uploadConfig;");
                }
                sb.AppendLine($"public {i.Namespace}Service(I{i.Namespace}WorkArea {i.Namespace}Work");
                sb.AppendLine(", IMapper _mapper");
                foreach (var y in newservicelist)
                {
                    sb.AppendLine($",I{y}Service _{y.ToLower()}Service");
                }


                sb.AppendLine(", ILogsService _logsService");
                sb.AppendLine(", ILoggerFactory loggerFactory");
                if (isPicture)
                {
                    sb.AppendLine(", IConfigServerice _configServerice");
                }
                sb.AppendLine(", IGeneralCacheServer _generalCacheServer)");

                sb.AppendLine("{");
                sb.AppendLine($"_{i.Namespace}Work = {i.Namespace}Work;");
                sb.AppendLine("generalCacheServer = _generalCacheServer;");
                sb.AppendLine("mapper = _mapper;");
                foreach (var y in newservicelist)
                {
                    sb.AppendLine($"{y.ToLower()}Service = _{y.ToLower()}Service;");
                }
                sb.AppendLine($"_logger = loggerFactory.CreateLogger<{i.Namespace}Service>();");
                sb.AppendLine($"logsService = _logsService;");
                if (isPicture)
                {
                    sb.AppendLine("configServerice = _configServerice;");
                    sb.AppendLine("uploadConfig = configServerice.GetUploadConfig();");
                }
                sb.AppendLine("}");
                foreach (var x in i.FileName)
                {
                    isPicture = false;
                    foreach (var y2 in x.Attributes)
                    {
                        if (y2.Name == "Picture")
                        {
                            isPicture = true;
                        }
                    }

                    sb.AppendLine(" ");

                    sb.AppendLine($"#region {x.Description}管理");
                    sb.AppendLine("/// <summary>");
                    sb.AppendLine($"/// 显示{x.Description}");
                    sb.AppendLine("/// </summary>");
                    sb.AppendLine("/// <returns></returns>");
                    sb.AppendLine($"public async Task<IPagedList<{x.Name}>> Show{x.Name}PageAsync({x.Name}SearchModel Search, int pageSize = 10)");
                    sb.AppendLine("{");
                    sb.AppendLine("if (Search.PageSize > 0) { pageSize = Search.PageSize; }");
                    sb.AppendLine("if (Search.Pageid <= 0) { Search.Pageid = 1; }");
                    sb.AppendLine($"var predicate = PredicateBuilder.New<{x.Name}>();");
                    sb.AppendLine($"if (!string.IsNullOrEmpty(Search.Keyword))");
                    sb.AppendLine("{");
                    sb.AppendLine("//predicate.And(x => x.Title.Contains(Search.Keyword) || x.Title.Contains(Search.Keyword));");
                    sb.AppendLine("}");
                    sb.AppendLine($"Func<IQueryable<{x.Name}>, IOrderedQueryable<{x.Name}>> IdItemFunc = source => source.OrderByDescending(x => x.Id);");
                    sb.AppendLine($"var list = await _{i.Namespace}Work.{x.Name}R.QueryPageAsync(predicate,IdItemFunc, page: Search.Pageid, pageSize: pageSize);");
                    sb.AppendLine("return list;");
                    sb.AppendLine("// var list =");
                    sb.AppendLine("}");

                    if (!x.IsViewEntity)
                    {
                        sb.AppendLine("/// <summary>");
                        sb.AppendLine($"/// 显示List{x.Description}");
                        sb.AppendLine("/// </summary>");
                        sb.AppendLine("/// <returns></returns>");
                        sb.AppendLine($"public async Task<IList<{x.Name}>> Show{x.Name}ListAsync({x.Name}SearchModel Search)");
                        sb.AppendLine("{");
                        sb.AppendLine($"var req = _{i.Namespace}Work.{x.Name}R.QueryNoTracking();");
                        sb.AppendLine($"if (!string.IsNullOrEmpty(Search.Keyword))");
                        sb.AppendLine("{");
                        sb.AppendLine("// req=req.Where(x => x.Keyword == Search.Keyword);");
                        sb.AppendLine("}");
                        foreach (var y in x.Attributes)
                        {
                            if (y.Name == "State")
                            {
                                sb.AppendLine("if (Search.State == true)");
                                sb.AppendLine("{");
                                sb.AppendLine("req = req.Where(x => x.State == true);");
                                sb.AppendLine("}");
                            }
                        }
                        sb.AppendLine("var list = await req.ToListAsync();");
                        sb.AppendLine("return list;");
                        sb.AppendLine("}");


                        sb.AppendLine(" ");
                        sb.AppendLine("/// <summary>");
                        sb.AppendLine($"/// 显示{x.Description}信息带缓存");
                        sb.AppendLine("/// </summary>");
                        sb.AppendLine("/// <param name=\"\"></param>");
                        sb.AppendLine("/// <returns></returns>");
                        sb.AppendLine($"public async Task<IList<{x.Name}>> Show{x.Name}ListCacheAsync({x.Name}SearchModel Search,int Second = 360)");
                        sb.AppendLine("{");
                        sb.AppendLine($"var key =Show{x.Name}CacheKey(Search,CacheKeyType.List);");
                        sb.AppendLine($"var result = generalCacheServer.Get<List<{x.Name}>>(key);");
                        sb.AppendLine("if (result == null)");
                        sb.AppendLine("{");
                        sb.AppendLine($"result = (await Show{x.Name}ListAsync(Search)).ToList();");
                        sb.AppendLine("if (result.Count > 0)");
                        sb.AppendLine("{");
                        sb.AppendLine("generalCacheServer.Set(key, result,Second);");
                        sb.AppendLine("}");
                        sb.AppendLine("}");
                        sb.AppendLine("return result;");
                        sb.AppendLine("}");


                        sb.AppendLine(" ");
                        sb.AppendLine("/// <summary>");
                        sb.AppendLine($"/// 显示{x.Description}信息");
                        sb.AppendLine("/// </summary>");
                        sb.AppendLine("/// <param name=\"\"></param>");
                        sb.AppendLine("/// <returns></returns>");
                        sb.AppendLine($"public async Task<{x.Name}> Show{x.Name}Async({x.Name}SearchModel Search)");
                        sb.AppendLine("{");
                        sb.AppendLine($"var model = new {x.Name}();");
                        sb.AppendLine("if (Search.Id > 0)");
                        sb.AppendLine("{");
                        sb.AppendLine($"model = await _{i.Namespace}Work.{x.Name}R.GetByIdAsync(Search.Id, true);");
                        sb.AppendLine("}");
                        sb.AppendLine("else");
                        sb.AppendLine("{");
                        if (x.AttributeName.Contains("State"))
                        {
                            sb.AppendLine("model.State = true;");
                        }
                        sb.AppendLine("}");
                        sb.AppendLine("return model;");
                        sb.AppendLine("}");

                        sb.AppendLine(" ");
                        sb.AppendLine("/// <summary>");
                        sb.AppendLine($"/// 显示{x.Description}信息带缓存");
                        sb.AppendLine("/// </summary>");
                        sb.AppendLine("/// <param name=\"\"></param>");
                        sb.AppendLine("/// <returns></returns>");
                        sb.AppendLine($"public async Task<{x.Name}> Show{x.Name}CacheAsync({x.Name}SearchModel Search,int Second = 360)");
                        sb.AppendLine("{");
                        sb.AppendLine($"var key =Show{x.Name}CacheKey(Search,CacheKeyType.Model);");
                        sb.AppendLine($"var result = generalCacheServer.Get<{x.Name}>(key);");
                        sb.AppendLine("if (result == null)");
                        sb.AppendLine("{");
                        sb.AppendLine($"//result = await _{i.Namespace}Work.{x.Name}R.QueryNoTracking().Where(x => x.Id == Search.Id).FirstOrDefaultAsync();");
                        sb.AppendLine($"result = await Show{x.Name}Async(Search);");
                        sb.AppendLine("if (result?.Id > 0)");
                        sb.AppendLine("{");
                        sb.AppendLine("generalCacheServer.Set(key, result, Second);");
                        sb.AppendLine("}");
                        sb.AppendLine("}");
                        sb.AppendLine("return result;");
                        sb.AppendLine("}");


                    }

                    sb.AppendLine(" ");
                    sb.AppendLine("/// <summary>");
                    sb.AppendLine($"/// 转化{x.Description}List");
                    sb.AppendLine("/// </summary>");
                    sb.AppendLine("/// <returns></returns>");

                    //sb.AppendLine($"public IPagedList<{x.Name}PostModel> Conversion{x.Name}(IPagedList<{x.Name}> list,ThumbnailModel thumbnailModel=null)");
                    sb.Append($"public IPagedList<{x.Name}PostModel> Conversion{x.Name}(IPagedList<{x.Name}> list");
                    if (x.IsThumbnail)
                    {
                        sb.Append(",ThumbnailModel thumbnailModel=null");
                    }
                    sb.AppendLine(")");

                    sb.AppendLine("{");
                    if (x.IsThumbnail)
                    {
                        sb.AppendLine("if (thumbnailModel == null) thumbnailModel = new ThumbnailModel() { Width = 50, Height = 100 };");
                    }
                    sb.Append($"var result = Conversion{x.Name}(list.ToList()");
                    if (x.IsThumbnail)
                    {
                        sb.Append(", thumbnailModel");
                    }
                    sb.AppendLine(");");

                    sb.AppendLine($"return new PagedList<{x.Name}PostModel>(result, list.PageIndex, list.PageSize, list.TotalCount);");
                    sb.AppendLine("}");

                    sb.AppendLine(" ");
                    sb.AppendLine("/// <summary>");
                    sb.AppendLine($"/// 转化{x.Description}List");
                    sb.AppendLine("/// </summary>");
                    sb.AppendLine("/// <returns></returns>");
                    sb.Append($"public IList<{x.Name}PostModel> Conversion{x.Name}(IList<{x.Name}> list");
                    if (x.IsThumbnail)
                    {
                        sb.Append(",ThumbnailModel thumbnailModel=null");
                    }
                    sb.AppendLine(")");

                    sb.AppendLine("{");
                    //sb.AppendLine($"var result = mapper.Map<IEnumerable<{x.Name}>, IEnumerable<{x.Name}PostModel>>(list);");
                    //sb.AppendLine("foreach (var i in result)");
                    //sb.AppendLine("{");
                    //sb.Append($" Conversion{x.Name}(i");
                    //if (x.IsThumbnail)
                    //{
                    //    sb.Append($", thumbnailModel");
                    //}
                    //sb.AppendLine($" );");

                    //sb.AppendLine("}");
                    sb.AppendLine($"            var result = new List<{x.Name}PostModel>();");
                    sb.AppendLine("            foreach (var i in list)");
                    sb.AppendLine("            {");
                    sb.Append($"                result.Add(Conversion{x.Name}(i");
                    if (x.IsThumbnail)
                    {
                        sb.Append($", thumbnailModel");
                    }
                    sb.AppendLine($" ));");

                    sb.AppendLine("            }");

                    sb.AppendLine($"return result.ToList();");
                    sb.AppendLine("}");

                    if (!x.IsViewEntity)
                    {
                        sb.AppendLine(" ");
                        sb.AppendLine("/// <summary>");
                        sb.AppendLine($"/// 转化{x.Description}");
                        sb.AppendLine("/// </summary>");
                        sb.AppendLine("/// <returns></returns>");
                        sb.Append($"public {x.Name}PostModel Conversion{x.Name}({x.Name} Model");
                        if (x.IsThumbnail)
                        {
                            sb.Append(",ThumbnailModel thumbnailModel=null");
                        }
                        sb.Append(")");
                        sb.AppendLine("{");

                        if (x.IsThumbnail)
                        {
                            sb.AppendLine("if (thumbnailModel == null) { thumbnailModel = new ThumbnailModel() { Width = 200, Height = 400 }; }");
                        }
                        sb.AppendLine($"var result = new {x.Name}PostModel();");
                        sb.AppendLine("if (Model?.Id > 0)");
                        sb.AppendLine("{");
                        sb.AppendLine($"result = _mapper.Map<{x.Name}, {x.Name}PostModel>(Model);");
                        if (isPicture)
                        {
                            sb.AppendLine("if (!string.IsNullOrEmpty(result.Picture))");
                            sb.AppendLine("{");
                            if (x.IsThumbnail)
                            {
                                sb.AppendLine("result.PictureShow = CommonServer.PictureShow(result.Picture, uploadConfig.PictureService, thumbnailModel);");
                            }
                            else
                            {
                                sb.AppendLine("result.PictureShow =CommonServer.PictureHref(Model.Picture, uploadConfig.PictureService);");
                            }
                            sb.AppendLine("result.PictureHref = CommonServer.PictureHref(Model.Picture, uploadConfig.PictureService);");
                            sb.AppendLine("}");
                        }
                        foreach (var y in x.Attributes)
                        {
                            if (y.Types0f == "Enum")
                            {
                                sb.AppendLine($"result.{y.Name}Show =  Model.{y.Name}.ToDescription();;");
                                //sb.AppendLine($"model.{y.Name} = ({y.EnumName})PostModel.{y.Name};");
                            }
                            else if (y.Types0f == "Attributes")
                            {
                                switch (y.AttributeName)
                                {
                                    case "EnumListStringAttribute":
                                        sb.Append($"result.{y.Name}Show = ");
                                        sb.Append($"expandsService.ShowEnumListVualeByCodingCacheAsync");
                                        sb.AppendLine($"(\"{y.AttributeCode}\",Model.{y.Name}).Result;");
                                        break;
                                    case "TableToAttribute":
                                        sb.AppendLine($"if (Model.{y.Name}>0)");
                                        sb.AppendLine("{");
                                        if (y.AttributeTwo == "BaseArea" || string.IsNullOrEmpty(y.AttributeTwo))
                                        {
                                            sb.Append($"var info = Show{y.AttributeOne}CacheAsync (new {y.AttributeOne}SearchModel() ");
                                            sb.Append("{ Id =");
                                            sb.Append($" Model.{y.Name}");
                                            sb.AppendLine("}).Result;");
                                        }
                                        else
                                        {
                                            sb.Append($"var info = {y.AttributeTwo.ToLower()}Service.Show{y.AttributeOne}CacheAsync (new {y.AttributeOne}SearchModel() ");
                                            sb.Append("{ Id =");
                                            sb.Append($" Model.{y.Name}");
                                            sb.AppendLine("}).Result;");

                                        }
                                        sb.Append($"if (info?.Id > 0) result.{y.Name}Show = info.{y.AttributeThree}; ");

                                        sb.AppendLine("}");

                                        break;
                                }
                            }
                            else if (y.Types0f == "DateTime")
                            {
                                sb.AppendLine($"result.{y.Name}Show = Model.{y.Name}.ToString(\"yyyy-MM-dd\");");
                            }

                        }

                        sb.AppendLine("}");
                        sb.AppendLine("return result;");

                        sb.AppendLine("}");

                        if (!x.IsViewEntity)
                        {
                            sb.AppendLine(" ");
                            sb.AppendLine("/// <summary>");
                            sb.AppendLine($"/// 保存{x.Description}{x.Description}");
                            sb.AppendLine("/// </summary>");
                            sb.AppendLine("/// <returns></returns>");
                            sb.AppendLine($"public async Task<ResultBaseModel> Save{x.Name}PostAsync({x.Name}PostModel PostModel, string DefUserName = \"SyStem\")");
                            sb.AppendLine("{");
                            sb.AppendLine("var result = new ResultBaseModel();");
                            sb.AppendLine("try");
                            sb.AppendLine("{");


                            sb.AppendLine($"//var counts = _{i.Namespace}Work.{x.Name}R.QueryNoTracking().Where(x => x.Title == PostModel.Title && x.Id != PostModel.Id).Count();");
                            sb.AppendLine("//if (counts > 0) { result.Msg = \"名称相同，请修改名称\"; return result; }");

                            sb.AppendLine("var userinfo =  AdminIdentityServer.AccountUserid();");
                            sb.AppendLine("if (userinfo?.UserId > 0) DefUserName = $\"{userinfo.UserNick}-{userinfo.UserName}\";");

                            sb.AppendLine($"var model = new {x.Name}();");
                            sb.AppendLine("if (PostModel.Id > 0)");
                            sb.AppendLine("{");
                            sb.AppendLine($"model = await _{i.Namespace}Work.{x.Name}R.GetByIdAsync(PostModel.Id);");
                            if (x.IsBaseWithAllEntity)
                            {
                                sb.AppendLine("model.UpdatedBy = DefUserName;");
                                sb.AppendLine("model.UpdatedOn = DateTime.Now;");
                            }
                            foreach (var y in x.Attributes)
                            {
                                if (y.Types0f == "Enum")
                                {
                                    sb.AppendLine($"model.{y.Name} = ({y.EnumName})PostModel.{y.Name};");
                                }
                                else
                                {
                                    sb.AppendLine($"model.{y.Name} = PostModel.{y.Name};");
                                }
                            }

                            sb.AppendLine("}");
                            sb.AppendLine("else");
                            sb.AppendLine("{");
                            sb.AppendLine($"model = mapper.Map<{x.Name}PostModel, {x.Name}>(PostModel);");
                            if (x.IsBaseWithAllEntity)
                            {
                                sb.AppendLine("model.CreatedOn = DateTime.Now;");
                                sb.AppendLine("model.CreatedBy = DefUserName;");
                            }

                            sb.AppendLine("}");
                            sb.AppendLine("if (model.Id > 0)");
                            sb.AppendLine("{");
                            sb.AppendLine($"_{i.Namespace}Work.{x.Name}R.Update(model);");

                            if (x.CustomAttributeList.Contains("AuditIncludeAttribute"))
                            {
                                sb.AppendLine($"await _{i.Namespace}Work.{x.Name}R.CommitAutoHistoryAsync(DefUserName);");
                            }
                            else
                            {
                                sb.AppendLine($"await _{i.Namespace}Work.{x.Name}R.CommitAsync();");
                            }

                            sb.Append($"var Search=new {x.Name}SearchModel()");
                            sb.AppendLine("{ Id=model.Id };");
                            sb.AppendLine($"Remove{x.Name}CacheAsync(Search, CacheKeyType.Model);");
                            sb.AppendLine("}");
                            sb.AppendLine("else");
                            sb.AppendLine("{");
                            sb.AppendLine($"await _{i.Namespace}Work.{x.Name}R.AddAsync(model);");
                            sb.AppendLine($"await _{i.Namespace}Work.{x.Name}R.CommitAsync();");
                            sb.AppendLine("}");

                            sb.AppendLine("result.Id = model.Id;");
                            sb.AppendLine("result.Success = true;");
                            sb.Append($"Remove{x.Name}CacheAsync(new {x.Name}SearchModel() ");
                            sb.AppendLine("{ State = true }, CacheKeyType.List);");

                            sb.AppendLine("}");
                            sb.AppendLine("catch (Exception ex)");
                            sb.AppendLine("{");

                            sb.AppendLine(" var LogError = new LogErrorObjectModel()");
                            sb.AppendLine("{");
                            sb.AppendLine("   EnterParameters = PostModel,");
                            sb.AppendLine("   ExceptionResults = ex,");
                            sb.AppendLine($"  Name = \"{i.Namespace}Service\",");
                            sb.AppendLine("   Operator = DefUserName,");
                            sb.AppendLine($"   Method = \"Save{x.Name}PostAsync\",");
                            sb.AppendLine(" };");

                            sb.AppendLine(" await logsService.SaveLogErrorAsync(LogError);");

                            sb.AppendLine(" result.Success = false;");
                            sb.AppendLine(" result.Msg = ex.Message;");




                            sb.AppendLine("}");

                            sb.AppendLine("return result;");
                            sb.AppendLine("}");


                            sb.AppendLine(" ");
                            sb.AppendLine("/// <summary>");
                            sb.AppendLine($"/// 批量保存{x.Description}");
                            sb.AppendLine("/// </summary>");
                            sb.AppendLine("/// <returns></returns>");
                            sb.AppendLine($"public async Task<ResultBaseModel> Save{x.Name}ListAsync(IList<{x.Name}> List)");
                            sb.AppendLine("{");
                            sb.AppendLine("var result = new ResultBaseModel();");
                            sb.AppendLine($"result = await _{i.Namespace}Work.{x.Name}R.AddBulkCopyAsync(List);");
                            //sb.AppendLine($"result.Id = List.Count;");

                            sb.AppendLine("//foreach (var i in List)");
                            sb.AppendLine("//{");
                            sb.AppendLine($"//    await _{i.Namespace}Work.{x.Name}R.AddAsync(i);");
                            sb.AppendLine("//}");
                            sb.AppendLine($"//await _{i.Namespace}Work.{x.Name}R.CommitAsync();");
                            sb.AppendLine("//result.Success = true;");
                            sb.AppendLine("return result;");
                            sb.AppendLine("}");
                            sb.AppendLine(" ");


                            sb.AppendLine(" ");
                            sb.AppendLine("/// <summary>");
                            sb.AppendLine($"/// 删除{x.Description}");
                            sb.AppendLine("/// </summary>");
                            sb.AppendLine("/// <param name=\"Search\"></param>");
                            sb.AppendLine("/// <returns></returns>");
                            sb.AppendLine($"public async Task<ResultBaseModel> Delete{x.Name}PostAsync({x.Name}SearchModel Search)");
                            sb.AppendLine("{");
                            sb.AppendLine("var result = new ResultBaseModel();");
                            sb.AppendLine("var userinfo = AdminIdentityServer.AccountUserid();");

                            sb.AppendLine($"var Model = await _{i.Namespace}Work.{x.Name}R.GetByIdAsync(Search.Id);");

                            if (x.IsBaseWithAllEntity)
                            {
                                sb.AppendLine("Model.IsDeleted = true;");
                                sb.AppendLine("Model.DeletedBy = $\"{userinfo.UserNick}-{userinfo.UserName}\";");
                                sb.AppendLine("Model.DeletedOn = DateTime.Now;");
                            }

                            sb.AppendLine($"_{i.Namespace}Work.{x.Name}R.Update(Model);");
                            sb.AppendLine($"await _{i.Namespace}Work.{x.Name}R.CommitAsync();");
                            sb.AppendLine("result.Id = Search.Id;");
                            sb.AppendLine("result.Success = true;");
                            sb.AppendLine($"Remove{x.Name}CacheAsync(Search, CacheKeyType.Model);");
                            sb.AppendLine("return result;");
                            sb.AppendLine("}");
                            sb.AppendLine(" ");


                            sb.AppendLine(" ");
                            sb.AppendLine("/// <summary>");
                            sb.AppendLine($"/// 移除{x.Description}缓存信息");
                            sb.AppendLine("/// </summary>");
                            sb.AppendLine("/// <param name=\"\"></param>");
                            sb.AppendLine("/// <returns></returns>");
                            sb.AppendLine($"public void Remove{x.Name}CacheAsync({x.Name}SearchModel Search,CacheKeyType cacheKeyType)");
                            sb.AppendLine("{");
                            sb.AppendLine($"var key =Show{x.Name}CacheKey(Search,cacheKeyType);");
                            sb.AppendLine("generalCacheServer.Remove(key);");
                            sb.AppendLine($"//Remove{x.Name}ListCacheAsync(Search);");
                            sb.AppendLine("}");
                            sb.AppendLine(" ");




                            //sb.AppendLine(" ");
                            //sb.AppendLine("/// <summary>");
                            //sb.AppendLine($"/// 移除{x.Description}缓存信息");
                            //sb.AppendLine("/// </summary>");
                            //sb.AppendLine("/// <param name=\"\"></param>");
                            //sb.AppendLine("/// <returns></returns>");
                            //sb.AppendLine($"public void Remove{x.Name}ListCacheAsync({x.Name}SearchModel Search)");
                            //sb.AppendLine("{");
                            //sb.AppendLine($"var key =Show{x.Name}CacheKey(Search,CacheKeyType.List);");
                            //sb.AppendLine("generalCacheServer.Remove(key);");
                            //sb.AppendLine("}");
                            //sb.AppendLine(" ");

                            sb.AppendLine(" ");
                            sb.AppendLine("/// <summary> ");
                            sb.AppendLine("/// 显示缓存Key ");
                            sb.AppendLine("/// </summary> ");
                            sb.AppendLine("/// <param name=\"Search\"></param> ");
                            sb.AppendLine("/// <param name=\"cache\"></param> ");
                            sb.AppendLine("/// <returns></returns> ");
                            sb.AppendLine($"private string Show{x.Name}CacheKey({x.Name}SearchModel Search, CacheKeyType cache) ");
                            sb.AppendLine("{ ");
                            sb.AppendLine("var key = string.Empty; ");
                            sb.AppendLine("switch (cache) ");
                            sb.AppendLine("{ ");
                            sb.AppendLine("case CacheKeyType.Model: ");
                            sb.Append(" key =$\"{CacheKey}");
                            sb.Append($"Show{x.Name}ByCacheAsync");
                            sb.AppendLine("_{Search.Id}\";");
                            sb.AppendLine("break; ");
                            sb.AppendLine("case CacheKeyType.List: ");
                            sb.Append(" key =$\"{CacheKey}");
                            sb.AppendLine($"Show{x.Name}ListCacheAsync\";");
                            sb.AppendLine(" break; ");
                            sb.AppendLine("case CacheKeyType.Page: ");
                            sb.Append(" key =$\"{CacheKey}");
                            sb.AppendLine($"Show{x.Name}PageCacheAsync\";");
                            sb.AppendLine("break; ");
                            sb.AppendLine("} ");
                            sb.AppendLine("return key; ");
                            sb.AppendLine("} ");

                        }
                        sb.AppendLine("#endregion");
                    }

                }
                sb.AppendLine("}");

                sb.AppendLine("}");

                await _sw.WriteAsync(sb.ToString());
                _sw.Close();

            }


        }


        private async Task CreatMappings(string Path, IList<MyChyEntityNamespace> list)
        {
            string file = Path + IMappings;
            FileHelper.CreatedFolder(file);

            file = file + "/AutoMapperConfiguration.txt";

            var _sw = new StreamWriter(new FileStream(file, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.Read), Encoding.UTF8);

            StringBuilder sb = new StringBuilder();

            foreach (var i in list)
            {

                sb.AppendLine($"using MyChy.Core.Domains.{i.Namespace};");
                sb.AppendLine($"using MyChy.Web.ViewModels.{i.Namespace};");
            }

            sb.AppendLine("");

            foreach (var i in list)
            {



                foreach (var x in i.FileName)
                {
                    sb.AppendLine($"cfg.CreateMap<{x.Name}PostModel, {x.Name}>().ReverseMap();");
                }
            }
            await _sw.WriteAsync(sb.ToString());

            _sw.Close();


        }
    }
}
