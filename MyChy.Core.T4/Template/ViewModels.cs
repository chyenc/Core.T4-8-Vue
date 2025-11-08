using MyChy.Core.T4.Common;
using MyChy.Frame.Core.Common.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MyChy.Core.T4.Template
{
    public class ViewModels
    {
        private const string IPath = "/MyChy.Web.ViewModels/Database";

        /// <summary>
        /// 输出模板
        /// </summary>
        /// <param name="Path"></param>
        /// <param name="EntityNamespace"></param>
        public async Task Write(string Path, IList<MyChyEntityNamespace> list)
        {

            var file = Path + IPath;
            //  FileHelper.DeleteFolder(file);

            FileHelper.CreatedFolder(file);
            await CreatViewModels(file, list);



        }

        private async Task CreatViewModels(string Path, IList<MyChyEntityNamespace> list)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var i in list)
            {
                var file = Path + "/" + i.Namespace;

                FileHelper.CreatedFolder(file);

                foreach (var x in i.FileName)
                {
                    sb = new StringBuilder();
                    string files = file + $"/{x.Name}ViewModel.cs";
                    var _sw = new StreamWriter(new FileStream(files, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.Read), Encoding.UTF8);

                    sb.AppendLine("using System;");
                    sb.AppendLine("using System.ComponentModel;");
                    sb.AppendLine("using System.Collections.Generic;");
                    sb.AppendLine("using Microsoft.AspNetCore.Mvc.Rendering;");
                    sb.AppendLine("using MyChy.Frame.Core.Common.Model;");
                    sb.AppendLine("using MyChy.Plugin.Models.Cache;");
                    sb.AppendLine($"using MyChy.Web.ViewModels.AdminVue;");
                    sb.AppendLine($"namespace MyChy.Web.ViewModels.{i.Namespace}");
                    sb.AppendLine("{");

                    sb.AppendLine($"    public class {x.Name}IndexViewModel : TableVueReqly");
                    sb.AppendLine("    {");
                    sb.AppendLine("");
                    sb.AppendLine("    }");

                    sb.AppendLine($"public class {x.Name}PostViewModel : BaseViewModel");
                    sb.AppendLine("{");
                    sb.Append($"public {x.Name}PostModel? PostModel ");
                    sb.AppendLine("{ get; set; }");

                    foreach (var y in x.Attributes)
                    {
                        if (y.Types0f == "Enum")
                        {

                            sb.Append($"public ICollection<HtmlSelectOptionInt>? {y.Name}Select ");
                            sb.AppendLine("{ get; set; }");
                        }

                        if (y.Types0f == "Attributes" || !string.IsNullOrEmpty(y.AttributeName))
                        {
                            switch (y.AttributeName)
                            {
                                case "EnumListStringAttribute":
                                    sb.Append($"public ICollection<HtmlSelectOptionInt>? {y.Name}Select ");
                                    sb.AppendLine("{ get; set; }");

                                    break;
                                case "EnumListCheckAttribute":
                                    sb.Append($"public ICollection<HtmlSelectOptionInt> {y.Name}Select ");
                                    sb.AppendLine("{ get; set; }");

                                    break;
                                case "TableToAttribute":
                                    sb.Append($"public ICollection<HtmlSelectOptionInt>? {y.Name}Select ");
                                    sb.AppendLine("{ get; set; }");

                                    break;

                            }


                        }
                    }

                    sb.AppendLine("}");


                    sb.AppendLine($"public class {x.Name}PostModel : BasePostModel");
                    sb.AppendLine("{");

                    foreach (var y in x.Attributes)
                    {
                        sb.AppendLine("");
                        sb.AppendLine("/// <summary>");
                        sb.AppendLine($"/// {y.Description}");
                        sb.AppendLine("/// </summary>");
                        sb.AppendLine($"[Description(\"{y.Description}\")]");
                        if (y.Types0f == "Enum" || y.AttributeName == "EnumListStringAttribute" 
                            || y.Name == "Picture" || y.Types0f == "DateTime"
                            || y.AttributeName == "TableToAttribute")
                        {
                            if (y.Name == "Picture" )
                            {
                                sb.Append($"public string? {y.Name} ");
                                sb.AppendLine("{ get; set; }");

                                sb.AppendLine("");
                                sb.AppendLine("/// <summary>");
                                sb.AppendLine($"/// {y.Description}原图显示");
                                sb.AppendLine("/// </summary>");
                                sb.AppendLine($"[Description(\"{y.Description}\")]");
                                sb.Append($"public string? {y.Name}Href ");
                                sb.AppendLine("{ get; set; }");

                            }
                            else if (y.Types0f == "DateTime")
                            {
                                sb.Append($"public {y.Types0f} {y.Name} ");
                                sb.AppendLine("{ get; set; }");
                            }
                            else if (y.Types0f == "Enum" || y.AttributeName == "EnumListStringAttribute"
                                || y.AttributeName == "TableToAttribute" || y.AttributeName == "TableToAttribute")
                            {

                                sb.Append($"public  IList<string>? {y.Name}s ");
                                sb.AppendLine("{ get; set; }");

                                sb.AppendLine("");
                                sb.AppendLine("/// <summary>");
                                sb.AppendLine($"/// {y.Description} 单选");
                                sb.AppendLine("/// </summary>");
                                sb.Append($"public int? {y.Name} ");
                                sb.AppendLine("{ get; set; }");


                            }
                            else
                            {
                                sb.Append($"public string? {y.Name} ");
                                sb.AppendLine("{ get; set; }");
                            }

                            sb.AppendLine("");
                            sb.AppendLine("/// <summary>");
                            sb.AppendLine($"/// {y.Description}显示");
                            sb.AppendLine("/// </summary>");
                            sb.AppendLine($"[Description(\"{y.Description}显示\")]");
                            sb.Append($"public string? {y.Name}Show ");
                            sb.AppendLine("{ get; set; }");

                        }
                        else if (y.AttributeName == "EnumListCheckAttribute")
                        {
                            sb.Append($"public string? {y.Name} ");
                            sb.AppendLine("{ get; set; }");

                            sb.AppendLine("");
                            sb.AppendLine("/// <summary>");
                            sb.AppendLine($"/// {y.Description}Post参数");
                            sb.AppendLine("/// </summary>");
                            sb.AppendLine($"[Description(\"{y.Description}\")]");
                            sb.Append($"public  IList<int> {y.Name}List  ");
                            sb.AppendLine("{ get; set; }");
                        }
                        else
                        {

                            sb.Append($"public {CheckTypes0f(y.Types0f)} {y.Name}");
                            sb.AppendLine("{ get; set; }");
                        }

                    }
                    sb.AppendLine("}");

                    sb.AppendLine($"public class {x.Name}SearchModel : BaseSearchRequest");
                    sb.AppendLine("{");

                    //foreach (var y in x.Attributes)
                    //{
                    //    if (y.Name == "Picture")
                    //    {
                    //        sb.AppendLine("");
                    //        sb.AppendLine("/// <summary>");
                    //        sb.AppendLine($"/// {y.Description}");
                    //        sb.AppendLine("/// </summary>");
                    //        sb.AppendLine($"[Description(\"{y.Description}\")]");
                    //        sb.Append($"public {y.Types0f} {y.Name}Show");
                    //        sb.AppendLine("{ get; set; }");

                    //    }
                    //}

                    sb.AppendLine("}");

                    sb.AppendLine("");
                    sb.AppendLine($"public class {x.Name}ExcelModel");
                    sb.AppendLine("{");
                    sb.AppendLine("}");

                    
                    sb.AppendLine("}");

                    await _sw.WriteAsync(sb.ToString());

                    _sw.Close();

                }

            }

        }

        private string CheckTypes0f(string Types0f)
        {
            if (Types0f == "string")
            {
                return "string?";
            }
            
            if (Types0f == "long")
            {
                return "long?"; 
            }

            return Types0f;


        }
    }
}
