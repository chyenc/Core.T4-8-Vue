using MyChy.Core.T4.Common;
using MyChy.Frame.Core.Common.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MyChy.Core.T4.Template
{
    public class WebCore
    {
        private const string IPath = "/MyChy.WebAdmin";
        private const string IViews = "/Views";



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

            file = file + IViews;
            FileHelper.CreatedFolder(file);

            await CreatView(file, list);



        }

        private async Task CreatView(string Path, IList<MyChyEntityNamespace> list)
        {
            //StringBuilder sb = new StringBuilder();
            foreach (var i in list)
            {
                var file = Path + "/" + i.Namespace;

                FileHelper.CreatedFolder(file);

                foreach (var x in i.FileName)
                {
                    await CreatVideIndex(file, x, i);

                    if (!x.IsViewEntity)
                    {
                        await CreatVideEdit(file, x, i);

                        await CreatVideImport(file, x, i);

                    }
                }

            }

        }


        private async Task CreatVideIndex(string file, MyChyEntity x, MyChyEntityNamespace i)
        {
            var sb = new StringBuilder();

            string files = file + $"/{x.Name}Index.cshtml";
            var _sw = new StreamWriter(new FileStream(files, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.Read), Encoding.UTF8);

            sb.AppendLine("@{");
            sb.AppendLine($"ViewData[\"Title\"] = \"{x.Description}管理\";");
            sb.AppendLine("Layout = \"~/Views/Shared/_Layout.cshtml\";");
            sb.AppendLine("}");
            sb.AppendLine($"@model MyChy.Web.ViewModels.{i.Namespace}.{x.Name}ViewModel");
            sb.AppendLine("@section scripts");
            sb.AppendLine("{");
            sb.AppendLine("<!--begin::Page Snippets -->");
            sb.AppendLine("<script src=\"@MyChy.Web.WebAdmin.ReadConfig.Current().CdnServer/frame/script/Index.js\" type=\"text/javascript\"></script>");


            sb.AppendLine("<!--end::Page Snippets -->");
            sb.AppendLine("<script>");
            sb.AppendLine("@await Html.PartialAsync(\"_JqPage\", Model.List)");

            sb.AppendLine("layui.use(['jquery','FromCommon','layedit','form','element','laydate'],function(){");
            sb.AppendLine($"varelement=layui.element,");
            sb.AppendLine($"$=layui.jquery,");
            sb.AppendLine($"layedit=layui.layedit,");
            sb.AppendLine($"form=layui.form,");
            sb.AppendLine($"laydate=layui.laydate,");
            sb.AppendLine($"FromCommon=layui.FromCommon;");
            sb.AppendLine($"");
            sb.AppendLine("$(function(){");
            sb.AppendLine($"");
            sb.AppendLine("$(\"#excelfb\").click(function(){");
            sb.AppendLine($"$(\"#FormPost\").attr(\"target\",\"_blank\");");
            sb.AppendLine($"    var url=$(\"#FormPost\").attr(\"action\");");
            sb.AppendLine($"$(\"    #FormPost\").attr(\"action\",\"/{i.Namespace}/{x.Name}Excel\");");
            sb.AppendLine($"$(\"    #FormPost\").submit();");
            sb.AppendLine($"$(\"    #FormPost\").attr(\"action\",\"/{i.Namespace}/{x.Name}Index\");");
            sb.AppendLine($"$(\"    #FormPost\").attr(\"target\",\"\");");
            sb.AppendLine($"    return false;");
            sb.AppendLine("});");
            sb.AppendLine($"");
            sb.AppendLine("$(\"#Copy\").click(function(){");
            sb.AppendLine("    layer.confirm('是否拷贝修改？',{");
            sb.AppendLine($"        btn:['拷贝','取消']//按钮");
            sb.AppendLine("    },function(index){");
            sb.AppendLine($"        layer.close(index);");
            sb.AppendLine($"        return false;");
            sb.AppendLine("    },function(index){");
            sb.AppendLine($"        layer.close(index);");
            sb.AppendLine("    });");
            sb.AppendLine("});");
            sb.AppendLine($"");
            sb.AppendLine($"");
            sb.AppendLine("});");
            sb.AppendLine("});");


            sb.AppendLine("</script>");
            sb.AppendLine("}");
            sb.AppendLine("@section style");
            sb.AppendLine("{}");
            sb.AppendLine("<div class=\"weadmin-body\">");
            sb.AppendLine("<!--begin: Search Form -->");
            sb.AppendLine("<form method=\"post\" id=\"FormPost\">");
            sb.AppendLine("<div class=\"layui-row\">");
            sb.AppendLine("<div class=\"layui-form layui-form-item\">");

            sb.AppendLine("<div class=\"layui-inline\">");
            sb.AppendLine("<label class=\"layui-form-label\">搜索：</label>");
            sb.AppendLine("<div class=\"layui-input-block\">");
            sb.AppendLine("<input type=\"text\" name=\"Keyword\" id=\"Keyword\" placeholder=\"请输入名称\"");
            sb.AppendLine(" autocomplete=\"off\" value=\"@Model.Search.Keyword\" class=\"layui-input\">");
            sb.AppendLine("</div>");
            sb.AppendLine("</div>");
            sb.AppendLine("<button class=\"layui-btn\" lay-submit=\"\" lay-filter=\"sreach\"><i class=\"layui-icon\">&#xe615;</i></button>");
            sb.AppendLine("@*<button class=\"layui-btn\" type=\"reset\" lay-filter=\"重置\"><i class=\"layui-icon\">&#xe9aa;</i></button>*@");
            sb.AppendLine("</div>");
            sb.AppendLine("</div>");
            sb.AppendLine("<input hidden=\"hidden\" id=\"PageId\" name=\"Pageid\" value=\"1\" />");
            sb.AppendLine("<input hidden=\"hidden\" id=\"Pagexd\" name=\"Pagexd\" value=\"@Model.Search.Pageid\" />");
            sb.AppendLine("</form>");

            sb.AppendLine("<div class=\"weadmin-block\">");
            sb.AppendLine("@if (Model.Features.Contains((int)MyChy.Core.Domains.CompetenceFeatures.AddTo))");
            sb.AppendLine("{");
            sb.AppendLine("@*<button class=\"layui-btn layui-btn-danger\" onclick=\"delAll()\"><i class=\"layui-icon\"></i>批量删除</button>*@");
            sb.AppendLine($"<button class=\"layui-btn\" type=\"button\"  onclick=\"WeAdminShow('添加','/{i.Namespace}/{x.Name}Edit')\"><i class=\"layui-icon\"></i>添加</button>");
            sb.AppendLine("}");

            sb.AppendLine("@if (Model.Features.Contains((int)MyChy.Core.Domains.CompetenceFeatures.Import))");
            sb.AppendLine("{");
            sb.AppendLine($"<button class=\"layui-btn\" type=\"button\"  onclick=\"WeAdminShow('导入Excel','/{i.Namespace}/{x.Name}Import')\"><i class=\"layui-icon\">&#xe681;</i>导入Excel</button>");
            sb.AppendLine("}");

            sb.AppendLine($"@if (Model.Features.Contains((int)MyChy.Core.Domains.CompetenceFeatures.Export))");
            sb.AppendLine("{");
            sb.AppendLine($"<button id=\"excelfb\" class=\"layui-btn\">");
            sb.AppendLine($"<i class=\"layui-icon\">&#xe601;</i>导出EXCEL");
            sb.AppendLine($"</button>");
            sb.AppendLine("}");


            sb.AppendLine("<span class=\"weadmin-block-title\">@Model.Title</span>");
            sb.AppendLine("<span class=\"fr\" style=\"line-height:40px\">共有数据：@Model.List.TotalCount 条</span>");
            sb.AppendLine("<div style=\"clear: both\" ></div>");
            sb.AppendLine("</div>");

            sb.AppendLine("<div class=\"layui-table-body layui-table-main\">");
            sb.AppendLine("<table class=\"layui-table\" id=\"memberList\">");

            sb.AppendLine("<thead>");
            sb.AppendLine("<tr>");
            sb.AppendLine("@*<th>");
            sb.AppendLine("<div class=\"layui-unselect header layui-form-checkbox\"  data-xd=\"checkboxall\" lay-skin=\"primary\" data-id=\"0\" ><i class=\"layui-icon\">&#xe605;</i></div>");
            sb.AppendLine("<input type=\"hidden\" id=\"checkboxid\" name=\"checkboxid\" value=\"\" />");
            sb.AppendLine("<input type=\"hidden\" id=\"checkboxcount\" name=\"checkboxcount\" value=\"0\" />");
            sb.AppendLine("</th>*@");
            foreach (var y in x.Attributes)
            {
                if (y.Name == "Remark" || y.Name == "Remarks" || y.Name == "Introduction" || y.Name == "Content")
                {

                }
                else
                {
                    sb.AppendLine($"<th>{y.Description}</th>");
                }
            }
            sb.AppendLine("@*<th>管理</th>*@");
            sb.AppendLine("<th>查看</th>");

            sb.AppendLine("</tr>");
            sb.AppendLine("</thead>");
            sb.AppendLine("<tbody>");
            sb.AppendLine("@foreach (var i in Model.List)");
            sb.AppendLine("{");
            sb.AppendLine("<tr data-id=\"@i.Id\" id=\"tr_@i.Id\">");
            sb.AppendLine("@*<td>");
            sb.AppendLine("<div class=\"layui-unselect layui-form-checkbox\"  data-xd=\"checkbox\" lay-skin=\"primary\" data-id=\"@i.Id\">");
            sb.AppendLine("<i class=\"layui-icon\">&#xe605;</i></div>");
            sb.AppendLine("</td>*@");

            foreach (var y in x.Attributes)
            {
                if (y.Name == "State")
                {
                    sb.AppendLine("<td class=\"td-status\">");
                    sb.AppendLine("@if (i.State)");
                    sb.AppendLine("{");
                    sb.AppendLine("<span class=\"layui-btn layui-btn-normal layui-btn-xs\">已启用</span>");
                    sb.AppendLine("}");
                    sb.AppendLine("else");
                    sb.AppendLine("{");
                    sb.AppendLine("<span class=\"layui-btn layui-btn-danger layui-btn-xs\">");
                    sb.AppendLine("已停用");
                    sb.AppendLine("</span>");
                    sb.AppendLine("}");
                    sb.AppendLine("</td>");
                }
                else if (y.Name == "Remark" || y.Name == "Remarks" || y.Name == "Introduction" || y.Name == "Content")
                {

                }
                else if (y.Name == "Picture")
                {
                    sb.AppendLine("<td>");
                    sb.AppendLine("@if (!string.IsNullOrEmpty(i.PictureShow))");
                    sb.AppendLine("{");
                    sb.AppendLine("<img alt=\"@i.Title\" style=\"max-height:50px; max-width:100px;display: block;\" src=\"@i.PictureShow\" />");
                    sb.AppendLine("}");
                    sb.AppendLine("</td>");
                }
                else if (y.Types0f == "Enum" || y.Types0f == "DateTime")
                {
                    sb.AppendLine($"<td>@i.{y.Name}Show</td>");
                }
                else if (y.AttributesName.Contains("EnumListStringAttribute") || y.AttributesName.Contains("TableToAttribute"))
                {
                    sb.AppendLine($"<td>@i.{y.Name}Show</td>");
                }
                else
                {
                    sb.AppendLine($"<td>@i.{y.Name}</td>");
                }
            }
            sb.AppendLine("@*<td>管理</td>*@");

            sb.AppendLine("<td class=\"td-manage\">");
            sb.AppendLine("@if (Model.Features.Contains((int)MyChy.Core.Domains.CompetenceFeatures.View))");
            sb.AppendLine("{");
            sb.AppendLine($"<a title=\"查看\" onclick=\"WeAdminShow('查看','/{i.Namespace}/{x.Name}Edit/@i.Id')\">");
            sb.AppendLine("<i class=\"layui-icon\">&#xe642;查看</i>");
            sb.AppendLine("</a>");
            sb.AppendLine("}");
            sb.AppendLine("@if (Model.Features.Contains((int)MyChy.Core.Domains.CompetenceFeatures.Delete))");
            sb.AppendLine("{");
            sb.AppendLine("@*<a title=\"删除\" data-xd=\"del\" data-id=\"@i.Id\" href=\"javascript:void(0);\">");
            sb.AppendLine("<i class=\"layui-icon\">&#xe640;删除</i>");
            sb.AppendLine("</a>*@");
            sb.AppendLine("}");
            sb.AppendLine("</td>");
            sb.AppendLine("</tr>");
            sb.AppendLine("}");

            sb.AppendLine("</tbody>");
            sb.AppendLine("</table>");
            sb.AppendLine("</div>");
            sb.AppendLine("<div class=\"page\">");
            sb.AppendLine("<div id=\"pagination3\"></div>");
            sb.AppendLine("</div>");
            sb.AppendLine("@if (Model.Features.Contains((int)MyChy.Core.Domains.CompetenceFeatures.Delete))");
            sb.AppendLine("{");
            sb.AppendLine("@*<div style=\"display:none\">");
            sb.AppendLine("<input type=\"hidden\" id=\"FromaddValue\" name=\"FromaddValue\" value=\"0\" />");
            sb.AppendLine($"<form asp-action=\"{x.Name}Delete\" asp-controller=\"{i.Namespace}\" method=\"post\" id=\"DelPost\">");
            sb.AppendLine("<input type=\"hidden\" id=\"did\" name=\"id\" value=\"0\" />");
            sb.AppendLine("<input type=\"hidden\" id=\"FromDelValue\" name=\"FromDelValue\" value=\"0\" />");
            sb.AppendLine("</form>");
            sb.AppendLine("</div>*@");
            sb.AppendLine("}");
            sb.AppendLine("</div>");

            await _sw.WriteAsync(sb.ToString());

            _sw.Close();

        }

        private async Task CreatVideEdit(string file, MyChyEntity x, MyChyEntityNamespace i)
        {
            var sb = new StringBuilder();

            string files = file + $"/{x.Name}Edit.cshtml";
            var _sw = new StreamWriter(new FileStream(files, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.Read), Encoding.UTF8);


            sb.AppendLine("@{");
            sb.AppendLine($"ViewData[\"Title\"] = \"{x.Description}编辑\";");
            sb.AppendLine("Layout = \"~/Views/Shared/_Layout.cshtml\";");
            sb.AppendLine("}");
            sb.AppendLine($"@model MyChy.Web.ViewModels.{i.Namespace}.{x.Name}PostViewModel");
            sb.AppendLine("@section scripts");
            sb.AppendLine("{");
            sb.AppendLine("<!--begin::Page Snippets -->");

            if (x.AttributeName.Contains("Content"))
            {
                sb.AppendLine("<script src=\"@MyChy.Web.WebAdmin.ReadConfig.Current().CdnLibServer/ckeditor/ckeditor.js\" type=\"text/javascript\"></script>");
            }

            if (x.AttributeName.Contains("Picture"))
            {
                sb.AppendLine("<script src=\"@MyChy.Web.WebAdmin.ReadConfig.Current().CdnServer/frame/upload/Index.js\" type=\"text/javascript\"></script>");
            }

            sb.AppendLine("<script src=\"@MyChy.Web.WebAdmin.ReadConfig.Current().CdnServer/frame/script/PageIframe.js\" type=\"text/javascript\"></script>");

            if (x.ScriptList.Contains("TableToAttribute"))
            {
                sb.AppendLine("<script src=\"@MyChy.Web.WebAdmin.ReadConfig.Current().CdnServer/frame/lib/extend/formSelects.js\" type=\"text/javascript\"></script>");
            }

            sb.AppendLine("<!--end::Page Snippets -->");
            sb.AppendLine("}");
            sb.AppendLine("@section style");
            sb.AppendLine("{");
            sb.AppendLine("}");
            sb.AppendLine("<div class=\"weadmin-body\">");
            sb.AppendLine("<form class=\"layui-form\" action=\"\" method=\"post\"  id=\"SubPost\"> @*lay-verify=\"required\"*@");
            foreach (var y in x.Attributes)
            {
                if (y.Types0f == "Enum")
                {
                    sb.AppendLine("<div class=\"layui-form-item\">");
                    sb.AppendLine($"<label class=\"layui-form-label\">{y.Description}</label>");
                    sb.AppendLine("<div class=\"layui-input-block\">");

                    //sb.AppendLine($"<input asp-for=\"Post.{y.Name}\"  name=\"{y.Name}\" placeholder=\"{y.Description}\" type=\"text\" autocomplete=\"off\" class=\"layui-input\"> @*lay-verify=\"required\"*@ ");

                    sb.AppendLine($"<select name=\"{y.Name}\" asp-for=\"Post.{y.Name}\" asp-items=\"@Html.GetEnumSelectList(typeof(MyChy.Core.Domains.{y.EnumName}))\" lay-verify=\"required\">");
                    sb.AppendLine("@*<option value=\"0\">全部</option>*@");
                    sb.AppendLine("</select>");

                    sb.AppendLine("</div>");
                    sb.AppendLine("</div>");

                }
                else if (y.Types0f == "Attributes" && y.AttributesName.Contains("ThumbnailAttribute"))
                {
                    sb.AppendLine("<div class=\"layui-form-item\">");
                    sb.AppendLine($"<label class=\"layui-form-label\">{y.Description}</label>");
                    sb.AppendLine("<div class=\"layui-input-block\">");

                    foreach (var z in y.List)
                    {
                        switch (z.Name)
                        {
                            case "EnumListStringAttribute":
                                sb.Append($"<select name=\"{y.Name}\" asp-items=\"Model.{y.Name}Select\" ");
                                sb.AppendLine($"asp-for=\"Post.{y.Name}\" lay-verify=\"required\" ");
                                sb.AppendLine($"></select>");
                                break;
                            case "EnumListCheckAttribute":
                                sb.AppendLine($"@foreach (var i in Model.{y.Name}Select) ");
                                sb.AppendLine("{");
                                sb.AppendLine($" <input type=\"checkbox\"  name=\"{y.Name}List\" value=\"@i.Value\"  ");
                                sb.AppendLine("  title=\"@i.Text\" @if (i.Selected) { <text> checked=\"checked\" </text> } >");
                                sb.AppendLine("}");

                                break;
                            case "TableToAttribute":
                                sb.Append($"<select name=\"{y.Name}\" asp-items=\"Model.{y.Name}Select\" ");
                                sb.AppendLine($"asp-for=\"Post.{y.Name}\" lay-verify=\"required\" ");
                                sb.AppendLine($"xm-select=\"{y.Name}\" xm-select-search=\"\" xm-select-radio=\"true\" xm-select-skin=\"normal\">");
                                sb.AppendLine($"</select>");
                                break;
                        }
                    }
                    sb.AppendLine("</div>");
                    sb.AppendLine("</div>");
                }
                else if (y.Name == "State")
                {
                    sb.AppendLine("<div class=\"layui-form-item\">");
                    sb.AppendLine("<label class=\"layui-form-label\">状态</label>");
                    sb.AppendLine("<div class=\"layui-input-block\">");
                    sb.AppendLine("<input @if (Model.Post.State) { <text> checked=\"\" </text> } name=\"open\"  data-to=\"State\" lay-skin=\"switch\" lay-filter=\"switchTest\" lay-text=\"ON|OFF\" type=\"checkbox\">");
                    sb.AppendLine("</div>");
                    sb.AppendLine("</div>");
                    sb.AppendLine("<input type=\"hidden\" id=\"State\" name=\"State\" value=\"@Model.Post.State.ToString()\" />");
                }
                else
                {
                    if (y.Types0f == "bool")
                    {
                        sb.AppendLine("<div class=\"layui-form-item\">");
                        sb.AppendLine($"<label class=\"layui-form-label\">{y.Description}</label>");
                        sb.AppendLine("<div class=\"layui-input-block\">");
                        sb.Append($"<input @if (Model.Post.{y.Name})");
                        sb.Append("{ <text> checked=\"\" </text> }");
                        sb.AppendLine($" name=\"Check{y.Name}\"  data-to=\"{y.Name}\" lay-skin=\"switch\" lay-filter=\"switchTest\" lay-text=\"ON|OFF\" type=\"checkbox\">");

                        sb.AppendLine("</div>");
                        sb.AppendLine("</div>");
                        sb.AppendLine($"<input type=\"hidden\" id=\"{y.Name}\" name=\"{y.Name}\" value=\"@Model.Post.{y.Name}.ToString()\" />");
                    }
                    else if (y.Types0f == "DateTime")
                    {
                        sb.AppendLine("<div class=\"layui-form-item layui-form-text\">");
                        sb.AppendLine($"<label class=\"layui-form-label\">{y.Description}</label>");
                        sb.AppendLine("<div class=\"layui-input-block\">");
                        sb.AppendLine($"<input asp-for=\"Post.{y.Name}Show\"  name=\"{y.Name}\" placeholder=\"{y.Description}\" type=\"text\" autocomplete=\"off\" class=\"layui-input date-item\">  ");
                        sb.AppendLine("</div>");
                        sb.AppendLine("</div>");

                    }
                    else if (y.Name == "Content")
                    {
                        sb.AppendLine("<div class=\"layui-form-item layui-form-text\">");
                        sb.AppendLine($"<label class=\"layui-form-label\">{y.Description}</label>");
                        sb.AppendLine("<div class=\"layui-input-block\">");
                        sb.AppendLine($"<textarea id=\"editor\" data-to=\"{y.Name}\">@Model.Post.{y.Name}</textarea>");
                        sb.AppendLine($"<input type=\"hidden\" name=\"{y.Name}\" id=\"{y.Name}\" value=\"@Model.Post.{y.Name}\"  />");
                        sb.AppendLine("</div>");
                        sb.AppendLine("</div>");

                    }
                    else if (y.Name == "Remark" || y.Name == "Remarks" || y.Name == "Introduction")
                    {

                        sb.AppendLine("<div class=\"layui-form-item layui-form-text\">");
                        sb.AppendLine($"<label class=\"layui-form-label\">{y.Description}</label>");
                        sb.AppendLine("<div class=\"layui-input-block\">");
                        sb.AppendLine($"<textarea class=\"layui-textarea\" asp-for=\"Post.{y.Name}\"  name=\"{y.Name}\" rows=\"3\" placeholder=\"{y.Description}信息可以为空\"></textarea>");
                        sb.AppendLine("</div>");
                        sb.AppendLine("</div>");

                    }
                    else if (y.Name == "Picture")
                    {

                        //sb.AppendLine("@if (Model.Post.Id > 0)");
                        //sb.AppendLine("{");
                        sb.AppendLine($"<div class=\"layui-form-item\" id=\"{y.Name}Div\">");
                        sb.AppendLine($"<label class=\"layui-form-label\">{y.Description}预览</label>");
                        sb.AppendLine("<div class=\"layui-input-block\">");
                        sb.AppendLine("<a href =\"@Model.Post.PictureHref\" target =\"_blank\" >");

                        sb.AppendLine($"<img id=\"{y.Name}Images\" style=\"max-height:200px;max-width:400px; display: block;\" src=\"@Model.Post.{y.Name}Show\" />");

                        sb.AppendLine("</a>");
                        sb.AppendLine("</div>");
                        sb.AppendLine("</div>");
                        sb.AppendLine("@if (Model.Features.Contains((int)MyChy.Core.Domains.CompetenceFeatures.Modify) ||");
                        sb.AppendLine("(Model.Post.Id == 0 && Model.Features.Contains((int)MyChy.Core.Domains.CompetenceFeatures.AddTo)))");
                        sb.AppendLine("{");
                        sb.AppendLine("<div class=\"layui-form-item\">");
                        sb.AppendLine($"<label class=\"layui-form-label\">{y.Description}</label>");
                        sb.AppendLine("<div class=\"layui-input-block\">");
                        sb.AppendLine($"<button type=\"button\" class=\"layui-btn layui-btn-primary\" id=\"{y.Name}Upload\">");
                        sb.AppendLine($"<i class=\"layui-icon\"></i>上传{y.Description}");
                        sb.AppendLine("</button>");
                        sb.AppendLine("</div>");
                        sb.AppendLine("</div>");
                        //sb.AppendLine("}");
                        sb.AppendLine("<input type=\"hidden\" id=\"IsDefault\" value=\"0\" />");
                        sb.AppendLine($"<input type=\"hidden\" id=\"{y.Name}\" value=\"@Model.Post.{y.Name}\" name=\"{y.Name}\" />");
                        sb.AppendLine("<input type=\"hidden\" id=\"TypeId\" value=\"1\" />");
                        sb.AppendLine("<input type=\"hidden\" id=\"KeysId\" value=\"@Model.Post.Id\" />");
                        if (x.IsThumbnail)
                        {
                            foreach (var z in y.List)
                            {
                                if (z.Name == "ThumbnailAttribute")
                                {
                                    sb.AppendLine($"<input type=\"hidden\" id=\"ThumWith\" value=\"{z.One}\" />");
                                    sb.AppendLine($"<input type=\"hidden\" id=\"ThumHigth\" value=\"{z.Two}\" />");
                                }
                            }

                        }
                        sb.AppendLine("}");
                    }
                    else
                    {
                        sb.AppendLine("<div class=\"layui-form-item\">");
                        sb.AppendLine($"<label class=\"layui-form-label\">{y.Description}</label>");
                        sb.AppendLine("<div class=\"layui-input-block\">");
                        sb.AppendLine($"<input asp-for=\"Post.{y.Name}\"  name=\"{y.Name}\" placeholder=\"{y.Description}\" type=\"text\" autocomplete=\"off\" class=\"layui-input\">  ");
                        sb.AppendLine("</div>");
                        sb.AppendLine("</div>");

                    }
                }


            }

            sb.AppendLine("<div class=\"layui-form-item\">");
            sb.AppendLine("<div style=\"text-align:center;\">");
            sb.AppendLine("@if (Model.Features.Contains((int)MyChy.Core.Domains.CompetenceFeatures.Modify) ||");
            sb.AppendLine("(Model.Post.Id == 0 && Model.Features.Contains((int)MyChy.Core.Domains.CompetenceFeatures.AddTo)))");
            sb.AppendLine("{");
            sb.AppendLine("<button class=\"layui-btn\" lay-submit=\"\" lay-filter=\"Sub\">立即提交</button>");
            sb.AppendLine("<button type=\"reset\"  id=\"resetbat\"  class=\"layui-btn layui-btn-primary\">重置</button>");
            sb.AppendLine("}");
            sb.AppendLine($"<button class=\"layui-btn layui-btn-normal\" type=\"button\" data-url=\"/{i.Namespace}/{x.Name}Index\" id=\"Fh\">返回</button>");
            sb.AppendLine("</div>");
            sb.AppendLine("</div>");
            sb.AppendLine("<input type=\"hidden\" id=\"FromaddValue\" value=\"0\" />");
            sb.AppendLine("<input type=\"hidden\" asp-for=\"Post.Id\"  name=\"Id\" />");

            sb.AppendLine("</form>");
            sb.AppendLine("</div>");


            await _sw.WriteAsync(sb.ToString());

            _sw.Close();
        }


        private async Task CreatVideImport(string file, MyChyEntity x, MyChyEntityNamespace i)
        {
            var sb = new StringBuilder();

            string files = file + $"/{x.Name}Import.cshtml";
            var _sw = new StreamWriter(new FileStream(files, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.Read), Encoding.UTF8);


            sb.AppendLine("@{");
            sb.AppendLine($"ViewData[\"Title\"] = \"{x.Description}上传\";");
            sb.AppendLine($"Layout = \"~/Views/Shared/_Layout.cshtml\";");
            sb.AppendLine("}");
            sb.AppendLine($"@model MyChy.Web.ViewModels.Admin.ImportViewModel");
            sb.AppendLine($"@section scripts");
            sb.AppendLine("{");
            sb.AppendLine($"<!--begin::Page Snippets -->");
            sb.AppendLine($"<script src=\"@MyChy.Web.WebAdmin.ReadConfig.Current().CdnServer/frame/script/PageIframe.js\" type=\"text/javascript\"></script>");
            sb.AppendLine($"<script src=\"@MyChy.Web.WebAdmin.ReadConfig.Current().CdnServer/frame/upload/Excel.js\" type=\"text/javascript\"></script>");
            sb.AppendLine($"<!--end::Page Snippets -->");
            sb.AppendLine($"<!--end::Page Snippets -->");
            sb.AppendLine("}");
            sb.AppendLine($"@section style");
            sb.AppendLine("{");
            sb.AppendLine($"");
            sb.AppendLine("}");
            sb.AppendLine($"<div class=\"weadmin-body\">");
            sb.AppendLine($"<form class=\"layui-form\" action=\"\" method=\"post\">");
            sb.AppendLine($"");
            sb.AppendLine($"<div class=\"layui-form-item\">");
            sb.AppendLine($"<label class=\"layui-form-label\">Excel上传</label>");
            sb.AppendLine($"<div class=\"layui-input-block\">");
            sb.AppendLine($"<button type=\"button\" class=\"layui-btn layui-btn-primary\" id=\"ExcelUpload\">");
            sb.AppendLine($"<i class=\"layui-icon\"></i>上传Excel文件");
            sb.AppendLine($"</button>");
            sb.AppendLine($"<div class=\"layui-form-mtext\" id=\"ExcelTitle\"></div>");
            sb.AppendLine($"</div>");
            sb.AppendLine($"</div>");
            sb.AppendLine($"<input type=\"hidden\" id=\"IsDefault\" value=\"0\" />");
            sb.AppendLine($"<input type=\"hidden\" id=\"Excel\" value=\"@Model.Post.Content\" name=\"Content\" lay-verify=\"required\" />");
            sb.AppendLine($"<input type=\"hidden\" id=\"TypeId\" value=\"1\" />");
            sb.AppendLine($"<input type=\"hidden\" id=\"KeysId\" value=\"@Model.Post.Id\" />");
            sb.AppendLine($"");
            sb.AppendLine($"<div class=\"layui-form-item\">");
            sb.AppendLine($"<label class=\"layui-form-label\">模板下载</label>");
            sb.AppendLine($"<div class=\"layui-input-block\">");
            sb.AppendLine($"<div class=\"layui-form-mtext\">");
            sb.AppendLine($"<a target=\"_blank\" href=\"@MyChy.Web.WebAdmin.ReadConfig.Current().CdnServer/Files/TP/{x.Name}.xlsx\">下载</a>");
            sb.AppendLine($"</div>");
            sb.AppendLine($"</div>");
            sb.AppendLine($"</div>");
            sb.AppendLine($"");
            sb.AppendLine($"<div class=\"layui-form-item\">");
            sb.AppendLine($"<div style=\"text-align:center;\">");
            sb.AppendLine($"@if (Model.Features.Contains((int)MyChy.Core.Domains.CompetenceFeatures.Import))");
            sb.AppendLine("{");
            sb.AppendLine($"<button class=\"layui-btn\" lay-submit=\"\" lay-filter=\"Sub\">立即提交</button>");
            sb.AppendLine($"<button type=\"reset\" class=\"layui-btn layui-btn-primary\">重置</button>");
            sb.AppendLine("}");
            sb.AppendLine($"<button class=\"layui-btn layui-btn-normal\" type=\"button\" data-url=\"/{i.Namespace}/{x.Name}Index\" id=\"Fh\">返回</button>");
            sb.AppendLine($"</div>");
            sb.AppendLine($"</div>");
            sb.AppendLine($"<input type=\"hidden\" id=\"FromaddValue\" value=\"0\" />");
            sb.AppendLine($"<input type=\"hidden\" asp-for=\"Post.Id\" name=\"Id\" />");
            sb.AppendLine($"</form>");
            sb.AppendLine($"</div>");



            await _sw.WriteAsync(sb.ToString());

            _sw.Close();
        }
    }
}
