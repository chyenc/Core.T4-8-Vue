using Microsoft.AspNetCore.Routing;
using MyChy.Core.T4.Common;
using MyChy.Frame.Core.Common.Helper;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace MyChy.Core.T4.Template;

public class WebVue
{
    private const string IPath = "/src";
    private const string Ipath = "/api";

    private const string Ipath2 = "/pages";

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

        var filepath = file + Ipath;
        FileHelper.CreatedFolder(filepath);
        await CreatApi(filepath, list);

        filepath = file + Ipath2;
        await CreatPages(filepath, list);

    }

    private async Task CreatApi(string Path, IList<MyChyEntityNamespace> list)
    {
        //StringBuilder sb = new StringBuilder();
        foreach (var i in list)
        {
            await CreatApi(Path, i);
        }
    }



    private async Task CreatApi(string file, MyChyEntityNamespace i)
    {
        var sb = new StringBuilder();

        string files = file + $"/{FirstCharToLowerCase(i.Namespace)}.ts";
        var _sw = new StreamWriter(new FileStream(files, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.Read), Encoding.UTF8);

        var methodNames = new List<string>() { "IndexInit", "AddInit", "Add", "ImportInit", "Import", "Export" };

        sb.AppendLine("import { request } from '@/utils/request';");
        sb.AppendLine("import type { TableResult } from '@/api/model/baseModel';");
        sb.AppendLine("import type { UserInfo } from './model/userModel';"); // 假设导入了特定类型
        sb.AppendLine();

        // --- 定义 API 对象 ---
        sb.AppendLine("const Api = {");

        sb.AppendLine("//-------------------接口地址-------------------");
        foreach (var entity in i.FileName)
        {
            sb.AppendLine($"//-------------------接口「{entity.Alias}」-------------------");

            foreach (var methodName in methodNames)
            {
                sb.AppendLine($"{entity.Alias}{methodName}: '/{i.Namespace}/{entity.Alias}{methodName}',");
            }
            sb.AppendLine();

        }
        sb.AppendLine("};");

        sb.AppendLine();

        sb.AppendLine("//-------------------分割线-------------------");

        foreach (var entity in i.FileName)
        {
            sb.AppendLine($"//-------------------方法「{entity.Alias}」-------------------");

            foreach (var methodName in methodNames)
            {

                string? requestPostData;
                if (methodName == "IndexInit")
                {
                    requestPostData = "TableResult";
                }
                else
                {
                    requestPostData = "unknown";
                }

                sb.AppendLine($"export function {entity.Alias}{methodName}(data: any) {{");
                sb.AppendLine($"   return request.post<{requestPostData}>({{");
                sb.AppendLine($"     url: Api.{entity.Alias}{methodName},");
                sb.AppendLine($"     data");
                sb.AppendLine($"   }});");
                sb.AppendLine($"}}");

            }

            sb.AppendLine();
        }


        await _sw.WriteAsync(sb.ToString());

        _sw.Close();
    }

    private async Task CreatPages(string Path, IList<MyChyEntityNamespace> list)
    {
        var filepath = Path; //+ $"/page/";
        FileHelper.CreatedFolder(filepath);
        foreach (var i in list)
        {
            foreach (var entity in i.FileName)
            {
                var filepathzi = filepath + $"/{i.Namespace}/{entity.Alias}";
                FileHelper.CreatedFolder(filepathzi);
                await CreatPagesIndex(filepathzi, entity, i);

                filepathzi = filepath + $"/{i.Namespace}/{entity.Alias}/components";
                FileHelper.CreatedFolder(filepathzi);
                await CreatPagesComponentsAdd(filepathzi, entity, i);
                await CreatPagesComponentsImport(filepathzi, entity, i);
                await CreatPagesComponentsExport(filepathzi, entity, i);
                await CreatPagesComponentsSearchPanel(filepathzi, entity, i);
            }
            // var filepathzi = filepath + $"/{i.Namespace}/{i.FileName}";
            // FileHelper.CreatedFolder(filepathzi);
            // await CreatPagesIndex(filepathzi, i);

            // filepathzi = filepath + $"/{i.Namespace}/components";
            // FileHelper.CreatedFolder(filepathzi);
            // await CreatPagesComponents(filepathzi, i);
        }
    }

    private async Task CreatPagesIndex(string file, MyChyEntity i, MyChyEntityNamespace ns)
    {
        var sb = new StringBuilder();

        string files = file + $"/index.vue";
        var _sw = new StreamWriter(new FileStream(files, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.Read), Encoding.UTF8);
        //sb.AppendLine("//-------------------页面逻辑-------------------");
        //sb.AppendLine();
        sb.AppendLine("<template>");
        sb.AppendLine("  <t-space direction=\"vertical\">");
        sb.AppendLine("");
        sb.AppendLine("    ");
        sb.AppendLine("    <t-card class=\"list-card-container\"  :bordered=\"false\" :shadow=\"true\" >");
        sb.AppendLine("      ");
        sb.AppendLine("      <t-row v-if=\"searchMode === 'simple'\" align=\"middle\" :gutter=\"16\">");
        sb.AppendLine("        <t-col :flex=\"1\">");
        sb.AppendLine("          ");
        sb.AppendLine("          <operation-buttons ");
        sb.AppendLine("            :permissions=\"pagePermissions\" ");
        sb.AppendLine("            @add=\"handleClickAddVisible\"");
        sb.AppendLine("            @import=\"handleClickImportVisible\"");
        sb.AppendLine("            @export=\"handleClickExportVisible\"");
        sb.AppendLine("          />");
        sb.AppendLine("        </t-col>");
        sb.AppendLine("        ");
        sb.AppendLine("        <t-col :flex=\"'280px'\">");
        sb.AppendLine("          <t-input ");
        sb.AppendLine("            v-model=\"simpleSearchValue\" ");
        sb.AppendLine("            :placeholder=\"t('components.placeholder')\" ");
        sb.AppendLine("            clearable");
        sb.AppendLine("            @enter=\"handleSimpleSearch\"");
        sb.AppendLine("            @clear=\"handleSimpleSearch\"");
        sb.AppendLine("          >");
        sb.AppendLine("            <template #suffix-icon>");
        sb.AppendLine("              <t-icon v-if=\"simpleSearchValue === ''\" name=\"search\" size=\"16px\" />");
        sb.AppendLine("            </template>");
        sb.AppendLine("          </t-input>");
        sb.AppendLine("        </t-col>");
        sb.AppendLine("      </t-row>");
        sb.AppendLine("");
        sb.AppendLine("      ");
        sb.AppendLine("      <!-- <template v-else>");
        sb.AppendLine("        ");
        sb.AppendLine("        <t-row class=\"operation-row\">");
        sb.AppendLine("          <operation-buttons ");
        sb.AppendLine("            :permissions=\"pagePermissions\" ");
        sb.AppendLine("            @add=\"handleClickAddVisible\"");
        sb.AppendLine("            @import=\"handleClickImportVisible\"");
        sb.AppendLine("            @export=\"handleClickExportVisible\"");
        sb.AppendLine("          />");
        sb.AppendLine("        </t-row>");
        sb.AppendLine("        ");
        sb.AppendLine("        ");
        sb.AppendLine("        <t-row class=\"search-row\">");
        sb.AppendLine("          <search-panel @search=\"handleSearch\" @reset=\"handleSearchReset\" />");
        sb.AppendLine("        </t-row>");
        sb.AppendLine("      </template> -->");
        sb.AppendLine("");
        sb.AppendLine("");
        sb.AppendLine("    ");
        sb.AppendLine("      <t-table");
        sb.AppendLine("        class=\"data-table\"");
        sb.AppendLine("        row-key=\"Id\"");
        sb.AppendLine("        :data=\"data\"");
        sb.AppendLine("        :columns=\"COLUMNS\"");
        sb.AppendLine("        :stripe=\"stripe\"");
        sb.AppendLine("        :bordered=\"bordered\"");
        sb.AppendLine("        :hover=\"hover\"");
        sb.AppendLine("        :table-layout=\"tableLayout ? 'auto' : 'fixed'\"");
        sb.AppendLine("        :size=\"size\"");
        sb.AppendLine("        :pagination=\"pagination\"");
        sb.AppendLine("        :show-header=\"showHeader\"");
        sb.AppendLine("        cell-empty-content=\"-\"");
        sb.AppendLine("        :resizable=\"resizable\"");
        sb.AppendLine("        :loading=\"dataLoading\"");
        sb.AppendLine("        :disableDataPage=\"disableDataPage\"");
        sb.AppendLine("        lazy-load");
        sb.AppendLine("        @row-click=\"handleRowClick\"");
        sb.AppendLine("        @page-change=\"rehandlePageChange\"");
        sb.AppendLine("      >");
        sb.AppendLine("        <template #op=\"slotProps\">");
        sb.AppendLine("          <t-space :size=\"4\">");
        sb.AppendLine("            <t-link ");
        sb.AppendLine("              v-if=\"hasPermission(2)\" ");
        sb.AppendLine("              theme=\"primary\" ");
        sb.AppendLine("              hover=\"color\"");
        sb.AppendLine("              @click=\"handleClickDetail(slotProps)\"");
        sb.AppendLine("            >");
        sb.AppendLine("              {{ t('components.detail') }}");
        sb.AppendLine("            </t-link>");
        sb.AppendLine("          </t-space> ");
        sb.AppendLine("        </template>");
        sb.AppendLine("");
        foreach (var y in i.Attributes)
        {
            if (y.Name == "State")
            {
                sb.AppendLine("        <template #State=\"slotProps\">");
                sb.AppendLine("          <t-tag ");
                sb.AppendLine("            :theme=\"slotProps.row.State ? 'success' : 'default'\" ");
                sb.AppendLine("            variant=\"light\"");
                sb.AppendLine("          >");
                sb.AppendLine("            {{ slotProps.row.State ? t('components.isState.on') : t('components.isState.off') }}");
                sb.AppendLine("          </t-tag>");
                sb.AppendLine("        </template>");
            }

            else if (y.Name == "Picture")
            {
                sb.AppendLine($"<template #{y.Name}Show=\"slotProps\">");
                sb.AppendLine("    <t-space :size=\"4\">");
                sb.AppendLine("        <t-image");
                sb.AppendLine("            :src=\"slotProps.row.PictureShow\""); // 插入动态变量
                sb.AppendLine("            :style=\"{ maxWidth: '100%', maxHeight: '100%', width: 'auto', height: 'auto' }\"");
                sb.AppendLine("            fit=\"contain\"");
                sb.AppendLine("            shape=\"round\"");
                sb.AppendLine("        />");
                sb.AppendLine("    </t-space>");
                sb.AppendLine("</template>");
            }
            else
            {
                if (y.Types0f == "bool")
                {
                    sb.AppendLine($"<template #{y.Name}=\"slotProps\">");
                    sb.AppendLine("    <t-tag ");
                    sb.AppendLine($"        :theme=\"slotProps.row.{y.Name} ? 'success' : 'default'\" ");
                    sb.AppendLine("        variant=\"light\"");
                    sb.AppendLine("    >");
                    // 注意：这里的 {{ 和 }} 必须作为字面量添加到 StringBuilder 中
                    sb.AppendLine($"  {{ slotProps.row.{y.Name} ? t('components.isState.on') : t('components.isState.off') }}");
                    sb.AppendLine("    </t-tag>");
                    sb.AppendLine("</template>");
                }
            }

        }
        sb.AppendLine("      </t-table>");
        sb.AppendLine(" ");
        sb.AppendLine("    </t-card>");
        sb.AppendLine("    ");
        sb.AppendLine("    <dialog-add ");
        sb.AppendLine("      v-if=\"formDialogAddVisible\" ");
        sb.AppendLine("      v-model:visible=\"formDialogAddVisible\" ");
        sb.AppendLine("      :Id=\"fromId\" ");
        sb.AppendLine("      @success=\"handleFormSuccess\" ");
        sb.AppendLine("    />");
        sb.AppendLine("    ");
        sb.AppendLine("    <dialog-import ");
        sb.AppendLine("      v-if=\"formDialogImportVisible\" ");
        sb.AppendLine("      v-model:visible=\"formDialogImportVisible\" ");
        sb.AppendLine("      @success=\"handleFormSuccess\" ");
        sb.AppendLine("    />");
        sb.AppendLine("    ");
        sb.AppendLine("    <dialog-export ");
        sb.AppendLine("      v-if=\"formDialogExportVisible\" ");
        sb.AppendLine("      v-model:visible=\"formDialogExportVisible\" ");
        sb.AppendLine("      @success=\"handleFormSuccess\" ");
        sb.AppendLine("    />");
        sb.AppendLine("   </t-space>");
        sb.AppendLine("</template>");
        sb.AppendLine("");
        sb.AppendLine("");
        sb.AppendLine("<script setup>");
        sb.AppendLine("import { computed, onMounted, ref, defineAsyncComponent } from 'vue';");
        sb.AppendLine("import { useRouter,useRoute } from 'vue-router';");

        sb.AppendLine("import { t } from '@/locales';");
        sb.AppendLine($"import {{ {i.Alias}IndexInit }} from '@/api/{FirstCharToLowerCase(ns.Namespace)}';");
        sb.AppendLine("//import SearchPanel from './components/SearchPanel.vue';");
        sb.AppendLine("import OperationButtons from '@/pages/components/OperationButtons.vue';");
        sb.AppendLine("");
        sb.AppendLine("//const router = useRouter();");
        sb.AppendLine("//const route = useRoute();");
        sb.AppendLine("//const queryId = ref (0); ");
        sb.AppendLine("// 懒加载 Dialog 组件");
        sb.AppendLine("const DialogAdd = defineAsyncComponent(() => import('./components/DialogAdd.vue'));");
        sb.AppendLine("const DialogImport = defineAsyncComponent(() => import('./components/DialogImport.vue'));");
        sb.AppendLine("const DialogExport = defineAsyncComponent(() => import('./components/DialogExport.vue'));");
        sb.AppendLine("");
        sb.AppendLine("const formDialogAddVisible = ref(false);");
        sb.AppendLine("const formDialogImportVisible = ref(false);");
        sb.AppendLine("const formDialogExportVisible = ref(false); ");
        sb.AppendLine("");
        sb.AppendLine("// 搜索模式：'simple' 单行搜索，'complex' 多条件搜索");
        sb.AppendLine("const searchMode = ref('simple'); // 默认使用简单搜索模式");
        sb.AppendLine("");
        sb.AppendLine("// 简单搜索的关键词");
        sb.AppendLine("const simpleSearchValue = ref('');");
        sb.AppendLine("");
        sb.AppendLine("// 复杂搜索条件");
        sb.AppendLine("const searchParams = ref({");
        sb.AppendLine("  keyword: '',");
        sb.AppendLine("  state: '',");
        sb.AppendLine("  // type: '', // 第三个搜索条件示例");
        sb.AppendLine("});");
        sb.AppendLine("");
        sb.AppendLine("const stripe = ref(true); //是否显示斑马线");
        sb.AppendLine("const bordered = ref(true); //是否显示边框");
        sb.AppendLine("const hover = ref(true); //是否开启鼠标悬停效果 ");
        sb.AppendLine("const dataLoading = ref(false);");
        sb.AppendLine("const pagination = ref(null); //分页信息");
        sb.AppendLine("const showHeader = ref(true); //是否显示表头");
        sb.AppendLine("const tableLayout = ref(false); //表格布局，fixed 为 true 时，表格宽度不会被内容撑开");
        sb.AppendLine("const size = ref('medium'); //表格尺寸，可选值为 medium / small / mini");
        sb.AppendLine("const resizable = ref(false); //是否允许拖动列宽调整大小");
        sb.AppendLine("const disableDataPage = ref(true); //是否禁用本地分页");
        sb.AppendLine("const fromId = ref(0);");
        sb.AppendLine("");
        sb.AppendLine("const data = ref([]);");
        sb.AppendLine("const COLUMNS = ref([]);");
        sb.AppendLine("");
        sb.AppendLine("// 当前页面权限列表");
        sb.AppendLine("const pagePermissions = ref([]);");
        sb.AppendLine("");
        sb.AppendLine("onMounted(() => {");
        sb.AppendLine($"  console.log('{i.Alias}Index-onMounted');");
        sb.AppendLine($"  // queryId.value = parseInt(route.query.id) || 0; ");
        sb.AppendLine("  initInfo();");
        sb.AppendLine("});");
        sb.AppendLine("");
        sb.AppendLine("const handleRowClick = (row, index) => {");
        sb.AppendLine($"  console.log('{i.Alias}Index-handleRowClick', row, index);");
        sb.AppendLine("};");
        sb.AppendLine("const handleClickAddVisible = () => {");
        sb.AppendLine($"  console.log('{i.Alias}Index-handleClickAddVisible');");
        sb.AppendLine("  fromId.value = 0;");
        sb.AppendLine("  formDialogAddVisible.value = true;");
        sb.AppendLine("};");
        sb.AppendLine("const handleClickDetail = (row) => {");
        sb.AppendLine($"  console.log('{i.Alias}Index-handleClickDetail',row);");
        sb.AppendLine("  fromId.value = row.row.Id;");
        sb.AppendLine("  formDialogAddVisible.value = true;");
        sb.AppendLine("};");
        sb.AppendLine("const handleClickImportVisible = (row) => {");
        sb.AppendLine($"  console.log('{i.Alias}Index-handleClickImportVisible',row);");
        sb.AppendLine("  formDialogImportVisible.value = true;");
        sb.AppendLine("};");
        sb.AppendLine("const handleClickExportVisible = (row) => {");
        sb.AppendLine($"  console.log('{i.Alias}Index-handleClickExportVisible',row); ");
        sb.AppendLine("  formDialogExportVisible.value = true; ");
        sb.AppendLine("};");
        sb.AppendLine("");
        sb.AppendLine("const initInfo = async () => {");
        sb.AppendLine("    dataLoading.value = true;");
        sb.AppendLine($"    console.log('{i.Alias}Index-initInfo');");
        sb.AppendLine("    ");
        sb.AppendLine("    // 根据搜索模式使用不同的搜索参数");
        sb.AppendLine("    const keyword = searchMode.value === 'simple' ");
        sb.AppendLine("      ? simpleSearchValue.value ");
        sb.AppendLine("      : searchParams.value.keyword;");
        sb.AppendLine("    ");
        sb.AppendLine($"    console.log('{i.Alias}Index-searchParams', searchMode.value === 'simple' ? {{ keyword }} : searchParams.value);");
        sb.AppendLine("    ");
        sb.AppendLine($"    var res= await {i.Alias}IndexInit({{");
        sb.AppendLine("        Keyword: keyword,");
        sb.AppendLine("        //Id:queryId.value,");
        sb.AppendLine("        //State: searchParams.value.state, // 复杂搜索时可启用");
        sb.AppendLine("        // Type: searchParams.value.type, // 第三个搜索条件示例");
        sb.AppendLine("        Pageid: pagination.value?.current || 1,");
        sb.AppendLine("        PageSize: pagination.value?.pageSize || 0");
        sb.AppendLine("    });");
        sb.AppendLine("");
        sb.AppendLine($"    console.log('{i.Alias}Index-res', res);");
        sb.AppendLine("");
        sb.AppendLine($"    console.log('{i.Alias}Index-columns', JSON.parse(res.columns) );");
        sb.AppendLine("");
        sb.AppendLine($"    console.log('{i.Alias}Index-data', JSON.parse(res.data) );");
        sb.AppendLine("");
        sb.AppendLine("    data.value=JSON.parse(res.data);");
        sb.AppendLine("    COLUMNS.value=JSON.parse(res.columns);");
        sb.AppendLine("    pagination.value=res.pager;");
        sb.AppendLine("");
        sb.AppendLine("    // 获取页面权限，假设接口返回中包含 permissions 字段");
        sb.AppendLine("    pagePermissions.value = res.permissions || [];");
        sb.AppendLine("    console.log('当前页面权限:', pagePermissions.value);");
        sb.AppendLine("    ");
        sb.AppendLine("    dataLoading.value = false;  ");
        sb.AppendLine("        ");
        sb.AppendLine("};");
        sb.AppendLine("");
        sb.AppendLine("// 权限判断函数");
        sb.AppendLine("const hasPermission = (permissionCode) => {");
        sb.AppendLine("  return pagePermissions.value.includes(permissionCode);");
        sb.AppendLine("};");
        sb.AppendLine("");
        sb.AppendLine("// 简单搜索处理");
        sb.AppendLine("const handleSimpleSearch = () => {");
        sb.AppendLine($"  console.log('{i.Alias}Index-handleSimpleSearch', simpleSearchValue.value);");
        sb.AppendLine("  // 重置到第一页并搜索");
        sb.AppendLine("  if (pagination.value) {");
        sb.AppendLine("    pagination.value.current = 1;");
        sb.AppendLine("  }");
        sb.AppendLine("  initInfo();");
        sb.AppendLine("};");
        sb.AppendLine("");
        sb.AppendLine("// 搜索处理 - 接收搜索组件传递的搜索条件");
        sb.AppendLine("const handleSearch = (searchData) => {");
        sb.AppendLine($"  console.log('{i.Alias}Index-handleSearch', searchData);");
        sb.AppendLine("  // 更新搜索条件");
        sb.AppendLine("  searchParams.value = { ...searchData };");
        sb.AppendLine("  // 重置到第一页并搜索");
        sb.AppendLine("  if (pagination.value) {");
        sb.AppendLine("    pagination.value.current = 1;");
        sb.AppendLine("  }");
        sb.AppendLine("  initInfo();");
        sb.AppendLine("};");
        sb.AppendLine("");
        sb.AppendLine("// 重置搜索");
        sb.AppendLine("const handleSearchReset = () => {");
        sb.AppendLine($"  console.log('{i.Alias}Index-handleSearchReset');");
        sb.AppendLine("  searchParams.value = {");
        sb.AppendLine("    keyword: '',");
        sb.AppendLine("    state: '',");
        sb.AppendLine("    // type: '',");
        sb.AppendLine("  };");
        sb.AppendLine("  // 重置到第一页并搜索");
        sb.AppendLine("  if (pagination.value) {");
        sb.AppendLine("    pagination.value.current = 1;");
        sb.AppendLine("  }");
        sb.AppendLine("  initInfo();");
        sb.AppendLine("};");
        sb.AppendLine("");
        sb.AppendLine("const rehandlePageChange = (page) => {");
        sb.AppendLine($"  console.log('{i.Alias}Index-rehandlePageChange', page);");
        sb.AppendLine("  pagination.value = {");
        sb.AppendLine("    current: page.current,");
        sb.AppendLine("    pageSize: page.pageSize,");
        sb.AppendLine("  };");
        sb.AppendLine("  initInfo();");
        sb.AppendLine("};  ");
        sb.AppendLine("");
        sb.AppendLine("const handleFormSuccess = () => {");
        sb.AppendLine($"  console.log('{i.Alias}Index-handleFormSuccess - 重新加载表格数据');");
        sb.AppendLine("  initInfo(); // 重新加载表格数据");
        sb.AppendLine("};");
        sb.AppendLine("</script>");
        sb.AppendLine("");
        sb.AppendLine("<style scoped lang=\"less\">");
        sb.AppendLine("@import '@/style/table-page.less';");
        sb.AppendLine("</style>");

        await _sw.WriteAsync(sb.ToString());
        _sw.Close();
    }



    private async Task CreatPagesComponentsAdd(string file, MyChyEntity i, MyChyEntityNamespace ns)
    {
        var sb = new StringBuilder();

        string files = file + $"/DialogAdd.vue";
        var _sw = new StreamWriter(new FileStream(files, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.Read), Encoding.UTF8);

        sb.AppendLine("<template>");
        sb.AppendLine("  <t-dialog  v-model:visible=\"formVisible\" :header=\"Id > 0 ? t('components.edit') : t('components.create')\" ");
        sb.AppendLine("  width=\"80%\" :footer=\"false\" :confirm-btn=\"null\" :cancel-btn=\"null\">");
        sb.AppendLine("    <template #body>");
        sb.AppendLine("      <t-loading :loading=\"initLoading || submitLoading\" :text=\"initLoading ? '加载中...' : '提交中...'\">");
        sb.AppendLine("      ");
        sb.AppendLine("      <t-form ref=\"formRefAdd\" :data=\"formData\" :rules=\"rules\" :label-width=\"100\" @submit=\"onSubmit\" :show-error-message=\"showErrorMessage\">");
        sb.AppendLine("        ");
        foreach (var y in i.Attributes)
        {
            sb.AppendLine($"<t-form-item label=\"{y.Description}\" name=\"{FirstCharToLowerCase(y.Name)}\">        ");

            if (y.Types0f == "Enum")
            {
                sb.AppendLine("    <t-select");
                sb.AppendLine($"      v-model=\"formData.{FirstCharToLowerCase(y.Name)}\"");
                sb.AppendLine($"      :options=\"resData.{FirstCharToLowerCase(y.Name)}Select\"");
                sb.AppendLine($"      placeholder=\"{y.Description}\"");
                sb.AppendLine("      clearable");
                sb.AppendLine("    ></t-select>");

            }
            else if (y.Types0f == "Attributes")
            {
                var issb = false;
                foreach (var z in y.List)
                {
                    switch (z.Name)
                    {
                        case "EnumListStringAttribute":
                            sb.AppendLine("    <t-select");
                            sb.AppendLine($"      v-model=\"formData.{FirstCharToLowerCase(y.Name)}\"");
                            sb.AppendLine($"      :options=\"resData.{FirstCharToLowerCase(y.Name)}Select\"");
                            sb.AppendLine($"      placeholder=\"{y.Description}\"");
                            sb.AppendLine("      clearable");
                            sb.AppendLine("    ></t-select>");
                            issb = true;
                            break;
                        case "EnumListCheckAttribute":
                            sb.AppendLine("    <t-select");
                            sb.AppendLine($"      v-model=\"formData.{FirstCharToLowerCase(y.Name)}\"");
                            sb.AppendLine($"      :options=\"resData.{FirstCharToLowerCase(y.Name)}Select\"");
                            sb.AppendLine($"      placeholder=\"{y.Description}\"");
                            sb.AppendLine("      clearable");
                            sb.AppendLine("    ></t-select>");
                            issb = true;
                            break;
                        case "TableToAttribute":
                            sb.AppendLine("    <t-select");
                            sb.AppendLine($"      v-model=\"formData.{FirstCharToLowerCase(y.Name)}\"");
                            sb.AppendLine($"      :options=\"resData.{FirstCharToLowerCase(y.Name)}Select\"");
                            sb.AppendLine($"      placeholder=\"{y.Description}\"");
                            sb.AppendLine("      clearable");
                            sb.AppendLine("    ></t-select>");
                            issb = true;
                            break;
                    }
                }

                if (!issb)
                {
                    sb.AppendLine($" <t-input v-model=\"formData.{FirstCharToLowerCase(y.Name)}\" placeholder =\"{y.Description}\"/>");
                }
            }
            else if (y.Name == "State")
            {
                sb.AppendLine("<t-switch v-model=\"formData.state\" size=\"large\">");
                sb.AppendLine("<template #label=\"slotProps\">{{ slotProps.value ? t('components.isState.on') : t('components.isState.off') }}</template>");
                sb.AppendLine("</t-switch>");
            }
            else
            {
                if (y.Types0f == "bool")
                {
                    sb.AppendLine($"<t-switch v-model=\"formData.{FirstCharToLowerCase(y.Name)}\" size=\"large\">");
                    sb.AppendLine("<template #label=\"slotProps\">{{ slotProps.value ? t('components.isState.on') : t('components.isState.off') }}</template>");
                    sb.AppendLine("</t-switch>");

                }
                else if (y.Types0f == "DateTime")
                { }
                else if (y.Name == "Content")
                {
                }
                else if (y.Name == "Remark" || y.Name == "Remarks" || y.Name == "Introduction")
                {
                    sb.AppendLine("<t-textarea");
                    sb.AppendLine($"      v-model=\"formData.{FirstCharToLowerCase(y.Name)}\"");
                    sb.AppendLine("      placeholder=\"请输入备注，可以为空！\"");
                    sb.AppendLine("      :maxlength=\"200\"");
                    sb.AppendLine("    ></t-textarea>");
                }
                else if (y.Name == "Picture")
                {
                    // 1. 外部 <t-form-item>
                    // 注意：C# 字符串中的双引号需要转义 \"
                    sb.AppendLine("          <t-form-item label=\"图片\" name=\"picture\"> ");

                    // 2. 自定义 <image-upload> 组件
                    sb.AppendLine("            <image-upload");
                    sb.AppendLine("              v-model=\"fileList\"");
                    sb.AppendLine("              :upload-params=\"uploadParams\"");
                    sb.AppendLine("              :max-size=\"5\"");
                    sb.AppendLine("              :on-upload-success=\"handleUploadSuccess\"");
                    sb.AppendLine("            />");

                    // 3. 提示信息 <template #tips>
                    sb.AppendLine("            <template #tips>");
                    sb.AppendLine("              <div style=\"color: var(--td-text-color-placeholder); font-size: 12px; margin-top: 4px;\">");
                    // 注意：内部文本不需要转义
                    sb.AppendLine("                支持 jpg、png、gif 格式，文件大小不超过 5MB");
                    sb.AppendLine("              </div>");
                    sb.AppendLine("            </template>");

                    sb.AppendLine("<t-input v-model=\"formData.picture\" hidden />");
                    if (i.IsThumbnail)
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
                    // 4. 结束 <t-form-item>
                    sb.AppendLine("</t-form-item>");



                }
                else if (y.Name == "Code")
                {
                    sb.AppendLine($" <t-input v-model=\"formData.{FirstCharToLowerCase(y.Name)}\" :disabled=\"formData.id > 0\"  placeholder =\"{y.Description}\"/>");
                }
                else
                {
                    sb.AppendLine($" <t-input v-model=\"formData.{FirstCharToLowerCase(y.Name)}\" placeholder =\"{y.Description}\"/>");


                }
            }
            sb.AppendLine("        </t-form-item>");
        }
        sb.AppendLine("        <t-form-item style=\"float: right\">");
        sb.AppendLine("          <t-input v-model=\"formData.id\" hidden />  ");
        sb.AppendLine("          <t-button  variant=\"outline\" @click=\"onClickCloseBtn\" :disabled=\"initLoading || submitLoading\">取消</t-button>");
        sb.AppendLine("          <t-button  v-if=\"hasPermissionEdit()\"  theme=\"primary\" type=\"submit\" :loading=\"submitLoading\" :disabled=\"initLoading\">确定</t-button>");
        sb.AppendLine("        </t-form-item>");
        sb.AppendLine("      </t-form>");
        sb.AppendLine("      </t-loading>");
        sb.AppendLine("    </template>");
        sb.AppendLine("  </t-dialog>");
        sb.AppendLine("</template>");
        sb.AppendLine("<script setup>");
        sb.AppendLine("import { ref, watch, onMounted } from 'vue';");
        sb.AppendLine("import { MessagePlugin } from 'tdesign-vue-next';");
        sb.AppendLine("");
        sb.AppendLine("import { t } from '@/locales';");
        sb.AppendLine($"import {{ {i.Alias}AddInit,{i.Alias}Add }} from '@/api/{FirstCharToLowerCase(ns.Namespace)}';");
        sb.AppendLine("import { commonRules } from '@/utils/validators';");
        foreach (var y in i.Attributes)
        {
            if (y.Name == "Picture")
            {
                sb.AppendLine("import ImageUpload from '@/components/image-upload/index.vue'; ");

            }
        }

        sb.AppendLine("// 当前页面权限列表");
        sb.AppendLine("const pagePermissions = ref([]);");
        sb.AppendLine("");
        sb.AppendLine("const formRefAdd = ref(null);");
        sb.AppendLine("const formVisible = ref(false);");
        sb.AppendLine("const formData = ref({});");
        sb.AppendLine("const resData = ref({});");
        sb.AppendLine("const showErrorMessage = ref(false);");
        sb.AppendLine("const submitLoading = ref(false);");
        sb.AppendLine("const initLoading = ref(false); // 初始化加载状态");
        foreach (var y in i.Attributes)
        {
            if (y.Name == "Picture")
            {
                // 4. 文件列表 v-model 绑定
                // 使用 AppendFormat 插入动态或复杂的内容
                sb.AppendLine("const file = ref([]);");

            }
        }
        sb.AppendLine("");
        sb.AppendLine("");
        sb.AppendLine("const rules = ref({");
        sb.AppendLine(" //roles: [{ required: true, message: '请选择最少一个角色', type: 'error' }],");
        foreach (var y in i.Attributes)
        {
            sb.AppendLine($" //  {FirstCharToLowerCase(y.Name)}: commonRules.required,");
        }

        sb.AppendLine("});");
        sb.AppendLine("");
        sb.AppendLine("const emit = defineEmits(['update:visible', 'success']);");
        sb.AppendLine("const { visible,Id } = defineProps({");
        sb.AppendLine("  visible: Boolean,");
        sb.AppendLine("  Id: Number,");
        sb.AppendLine("});");
        sb.AppendLine("");
        //sb.AppendLine("// const props = defineProps({");
        //sb.AppendLine("//     Id: integer,");
        //sb.AppendLine("// })");
        //sb.AppendLine("");
        sb.AppendLine("onMounted(() => {");
        sb.AppendLine($"  console.log('{i.Alias}DialogAdd-onMounted');");
        sb.AppendLine("  // 不在这里执行 initInfo，等待弹窗打开时才执行");
        sb.AppendLine("});");
        sb.AppendLine("");
        sb.AppendLine("");
        sb.AppendLine(" const initInfo = async () => {");
        sb.AppendLine("      try {");
        sb.AppendLine("        initLoading.value = true; // 开启加载状态");
        sb.AppendLine($"        console.log('{i.Alias}DialogAdd-initInfo');");
        sb.AppendLine($"        console.log('{i.Alias}DialogAdd-Id:', Id);");
        sb.AppendLine($"        var res = await {i.Alias}AddInit({{ id: Id }}); // 传递对象格式的参数");
        sb.AppendLine($"        console.log('{i.Alias}DialogAdd-{i.Alias}AddInit-res', res);");
        sb.AppendLine("        formData.value = res.postModel;");
        sb.AppendLine("        resData.value = res;");

        foreach (var y in i.Attributes)
        {
            if (y.Name == "Picture")
            {
                // 1. If 语句块开始
                sb.AppendLine("    // 处理图片回显");
                sb.AppendLine("if (res.postModel.pictureShow != null) {");
                // 2. 赋值 file.value 
                sb.AppendLine("      file.value = [{");
                // 3. 设置 url 和 name 属性
                // 注意：JavaScript 对象中的属性值不需要 C# 的双引号转义，它们本身就是代码
                sb.AppendLine("        url: res.postModel.pictureShow,");
                sb.AppendLine("        name: res.postModel.title");
                // 4. 对象和数组结束
                sb.AppendLine("      }];");
                // 5. If 语句块结束
                sb.AppendLine("    }");
                // 6. Else 语句块开始
                sb.AppendLine("    else{");
                // 7. Else 赋值
                sb.AppendLine("      file.value = [];");
                // 8. Else 语句块结束
                sb.AppendLine("    }");
            }
        }

        // sb.AppendLine("        // 保存初始数据快照，用于重置表单");
        // sb.AppendLine("        //initialFormData.value = JSON.parse(JSON.stringify(res.postModel));");
        sb.AppendLine("        pagePermissions.value = res.permissions || [];");
        sb.AppendLine("     ");
        sb.AppendLine("      } catch (error) {");
        sb.AppendLine("        // 错误已经在请求拦截器中统一处理");
        sb.AppendLine("      } finally {");
        sb.AppendLine("        initLoading.value = false; // 关闭加载状态");
        sb.AppendLine("      }");
        sb.AppendLine("};");
        sb.AppendLine("   ");
        sb.AppendLine("const onClickCloseBtn = () => {");
        sb.AppendLine("  formVisible.value = false;");
        sb.AppendLine("  // 关闭时清空表单数据");
        sb.AppendLine("  formData.value = {};");
        sb.AppendLine("  showErrorMessage.value = false;");
        sb.AppendLine("  if (formRefAdd.value) {");
        sb.AppendLine("    formRefAdd.value.clearValidate();");
        sb.AppendLine("  }");
        sb.AppendLine("};");
        sb.AppendLine("");
        foreach (var y in i.Attributes)
        {
            if (y.Name == "Picture")
            {
                // 1. computed 属性开始
                sb.AppendLine("// 上传参数（computed 实现动态更新，从 DOM 元素中读取缩略图参数）");
                sb.AppendLine("const uploadParams = computed(() => {");
                sb.AppendLine("  const params = {");
                sb.AppendLine("    TypeId: 1,");
                sb.AppendLine("    KeysId: formData.value.id,");
                sb.AppendLine("  };");
                sb.AppendLine("  ");

                // 1. 读取缩略图宽度
                sb.AppendLine("  // 从 DOM 元素中读取缩略图宽度（直接传字符串）");
                sb.AppendLine("  const thumWidthElement = document.getElementById('ThumWidth');");
                sb.AppendLine("  if (thumWidthElement && thumWidthElement.value && thumWidthElement.value.trim()) {");
                sb.AppendLine("    params.ThumWidth = thumWidthElement.value.trim();");
                sb.AppendLine("  }");
                sb.AppendLine("  ");

                // 2. 读取缩略图高度
                sb.AppendLine("  // 从 DOM 元素中读取缩略图高度（直接传字符串）");
                sb.AppendLine("  const thumHeightElement = document.getElementById('ThumHeight');");
                sb.AppendLine("  if (thumHeightElement && thumHeightElement.value && thumHeightElement.value.trim()) {");
                sb.AppendLine("    params.ThumHeight = thumHeightElement.value.trim();");
                sb.AppendLine("  }");

                // 4. 返回 params
                sb.AppendLine("  console.log('上传参数:', params);");
                sb.AppendLine("  return params;");
                sb.AppendLine("});");
                sb.AppendLine("");

                // 5. 上传成功回调函数
                sb.AppendLine("// 上传成功回调");
                sb.AppendLine("const handleUploadSuccess = (response) => {");
                sb.AppendLine("  console.log('上传成功，响应:', response);");
                sb.AppendLine("  if (response.code != null) {");
                sb.AppendLine("    formData.value.picture = response.code;");
                sb.AppendLine("  }");
                sb.AppendLine("};");
            }
        }

        sb.AppendLine("// 权限判断函数");
        sb.AppendLine("const hasPermission = (permissionCode) => {");
        sb.AppendLine("  return pagePermissions.value.includes(permissionCode);");
        sb.AppendLine("};");
        sb.AppendLine("");

        sb.AppendLine("const hasPermissionEdit = () => {");
        sb.AppendLine("  var permissionCode=3;");
        sb.AppendLine("  if(Id>0)  {permissionCode=4};");
        sb.AppendLine("  return pagePermissions.value.includes(permissionCode);");
        sb.AppendLine("};");
        sb.AppendLine("");
        sb.AppendLine("");
        sb.AppendLine("const onSubmit = async ({ validateResult, firstError }) => {");
        sb.AppendLine("  showErrorMessage.value = true; // 提交时才显示错误信息");
        sb.AppendLine("  if (firstError) {");
        sb.AppendLine("    console.log('Errors: ', validateResult);");
        sb.AppendLine("    MessagePlugin.warning(firstError);");
        sb.AppendLine("    return;");
        sb.AppendLine("  }");
        sb.AppendLine("  ");
        sb.AppendLine("  try {");
        sb.AppendLine("    submitLoading.value = true; // 开启加载状态");
        sb.AppendLine("    console.log('formData:', formData.value);");
        sb.AppendLine($"    var res = await {i.Alias}Add(formData.value);");
        sb.AppendLine("");
        sb.AppendLine("    MessagePlugin.success('提交成功');");
        sb.AppendLine("    emit('success'); // 通知父组件刷新数据");
        sb.AppendLine("    onClickCloseBtn(); // 关闭对话框");
        sb.AppendLine("  } catch (error) {");
        sb.AppendLine("    console.error('提交失败:', error);");
        sb.AppendLine("    //MessagePlugin.error('提交失败，请重试');");
        sb.AppendLine("  } finally {");
        sb.AppendLine("    submitLoading.value = false; // 关闭加载状态");
        sb.AppendLine("  }");
        sb.AppendLine("");
        sb.AppendLine("};");
        sb.AppendLine("");
        sb.AppendLine("");
        sb.AppendLine("watch(");
        sb.AppendLine("  () => formVisible.value,");
        sb.AppendLine("  (val) => {");
        sb.AppendLine("    emit('update:visible', val);");
        sb.AppendLine("  },");
        sb.AppendLine(");");
        sb.AppendLine("");
        sb.AppendLine("watch(");
        sb.AppendLine("  () => visible,");
        sb.AppendLine("  (val) => {");
        sb.AppendLine("    formVisible.value = val;");
        sb.AppendLine("    if (val) {");
        sb.AppendLine("      // 打开对话框时重新加载数据");
        sb.AppendLine("      initInfo();");
        sb.AppendLine("    }");
        sb.AppendLine("  },");
        sb.AppendLine($"  {{ immediate: true }} // 立即执行，解决懒加载组件初始化问题");
        sb.AppendLine(");");
        sb.AppendLine("</script>       ");
        //sb.AppendLine("//-------------------组件逻辑-------------------");
        await _sw.WriteAsync(sb.ToString());
        _sw.Close();

    }
    private async Task CreatPagesComponentsImport(string file, MyChyEntity i, MyChyEntityNamespace ns)
    {
        var sb = new StringBuilder();

        string files = file + $"/DialogImport.vue";
        var _sw = new StreamWriter(new FileStream(files, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.Read), Encoding.UTF8);

        sb.AppendLine("<template>");
        sb.AppendLine("    <div>DialogImport</div>");
        sb.AppendLine("</template>");

        await _sw.WriteAsync(sb.ToString());
        _sw.Close();

    }



    private async Task CreatPagesComponentsExport(string file, MyChyEntity i, MyChyEntityNamespace ns)
    {
        var sb = new StringBuilder();

        string files = file + $"/DialogExport.vue";
        var _sw = new StreamWriter(new FileStream(files, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.Read), Encoding.UTF8);

        sb.AppendLine("<template>");
        sb.AppendLine("    <div>DialogExport</div>");
        sb.AppendLine("</template>");
        await _sw.WriteAsync(sb.ToString());

        _sw.Close();

    }

    private async Task CreatPagesComponentsSearchPanel(string file, MyChyEntity i, MyChyEntityNamespace ns)
    {
        var sb = new StringBuilder();

        string files = file + $"/SearchPanel.vue";
        var _sw = new StreamWriter(new FileStream(files, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.Read), Encoding.UTF8);

        sb.AppendLine("<template>");
        sb.AppendLine("   <t-form ");
        sb.AppendLine("      ref=\"formRef\" ");
        sb.AppendLine("      :data=\"searchForm\" ");
        sb.AppendLine("      layout=\"inline\" ");
        sb.AppendLine("      label-align=\"left\"");
        sb.AppendLine("      class=\"search-form\"");
        sb.AppendLine("      @submit=\"onSubmit\" ");
        sb.AppendLine("      @reset=\"onReset\"");
        sb.AppendLine("    >");
        sb.AppendLine("      <t-form-item label=\"关键词\" name=\"keyword\">");
        sb.AppendLine("        <t-input ");
        sb.AppendLine("          v-model=\"searchForm.keyword\" ");
        sb.AppendLine("          :placeholder=\"t('components.placeholder')\" ");
        sb.AppendLine("          clearable ");
        sb.AppendLine("          @enter=\"onSubmit\"");
        sb.AppendLine("          style=\"width: 200px;\"");
        sb.AppendLine("        />");
        sb.AppendLine("      </t-form-item>");
        sb.AppendLine("      ");
        sb.AppendLine("      <t-form-item label=\"状态\" name=\"state\">");
        sb.AppendLine("        <t-select ");
        sb.AppendLine("          v-model=\"searchForm.state\" ");
        sb.AppendLine("          clearable ");
        sb.AppendLine("          placeholder=\"请选择状态\"");
        sb.AppendLine("          style=\"width: 120px;\"");
        sb.AppendLine("        >");
        sb.AppendLine("          <t-option value=\"\" label=\"全部\" />");
        sb.AppendLine("          <t-option :value=\"true\" label=\"启用\" />");
        sb.AppendLine("          <t-option :value=\"false\" label=\"禁用\" />");
        sb.AppendLine("        </t-select>");
        sb.AppendLine("      </t-form-item>");
        sb.AppendLine("      ");
        sb.AppendLine("      ");
        sb.AppendLine("      ");
        sb.AppendLine("      ");
        sb.AppendLine("      <t-form-item label-width=\"0\" class=\"button-group\">");
        sb.AppendLine("        <t-space :size=\"8\">");
        sb.AppendLine("          <t-button theme=\"primary\" type=\"submit\">");
        sb.AppendLine("            <template #icon><t-icon name=\"search\" /></template>");
        sb.AppendLine("            {{ t('components.search') }}");
        sb.AppendLine("          </t-button>");
        sb.AppendLine("          ");
        sb.AppendLine("          <t-button theme=\"default\" type=\"reset\" variant=\"outline\">");
        sb.AppendLine("            <template #icon><t-icon name=\"refresh\" /></template>");
        sb.AppendLine("            {{ t('components.reset') }}");
        sb.AppendLine("          </t-button>");
        sb.AppendLine("        </t-space>");
        sb.AppendLine("      </t-form-item>");
        sb.AppendLine("    </t-form>");
        sb.AppendLine(" ");
        sb.AppendLine("</template>");
        sb.AppendLine("");
        sb.AppendLine("<script setup>");
        sb.AppendLine("import { ref } from 'vue';");
        sb.AppendLine("import { t } from '@/locales';");
        sb.AppendLine("");
        sb.AppendLine("const formRef = ref(null);");
        sb.AppendLine("const searchForm = ref({");
        sb.AppendLine("  keyword: '',");
        sb.AppendLine("  state: '',");
        sb.AppendLine("  // type: '', // 第三个搜索条件示例");
        sb.AppendLine("});");
        sb.AppendLine("");
        sb.AppendLine("const emit = defineEmits(['search', 'reset']);");
        sb.AppendLine("");
        sb.AppendLine("// 提交搜索");
        sb.AppendLine("const onSubmit = () => {");
        sb.AppendLine("  console.log('SearchPanel-onSubmit', searchForm.value);");
        sb.AppendLine("  emit('search', { ...searchForm.value });");
        sb.AppendLine("};");
        sb.AppendLine("");
        sb.AppendLine("// 重置搜索");
        sb.AppendLine("const onReset = () => {");
        sb.AppendLine("  console.log('SearchPanel-onReset');");
        sb.AppendLine("  // 注意：此处代码将重置逻辑留给了父组件的 handleSearchReset，但仍需要清空本地表单。");
        sb.AppendLine("  searchForm.value = {");
        sb.AppendLine("    keyword: '',");
        sb.AppendLine("    state: '',");
        sb.AppendLine("    // type: '',");
        sb.AppendLine("  };");
        sb.AppendLine("  // 通知父组件执行重置和重新加载数据");
        sb.AppendLine("  emit('reset');");
        sb.AppendLine("};");
        sb.AppendLine("</script>");
        sb.AppendLine("");
        sb.AppendLine("<style scoped lang=\"less\">");
        sb.AppendLine("@import '@/style/table-page.less';");
        sb.AppendLine("</style>");
        await _sw.WriteAsync(sb.ToString());

        _sw.Close();

    }


    public static string FirstCharToLowerCase(string str)
    {
        // 检查字符串是否为 null 或空，如果是，则直接返回
        if (string.IsNullOrEmpty(str))
        {
            return str;
        }

        // 检查字符串长度是否只有一位
        if (str.Length == 1)
        {
            return char.ToLower(str[0]).ToString();
        }

        // 将第一个字符转换为小写，并与字符串的其余部分拼接
        return char.ToLower(str[0]) + str.Substring(1);
    }
}
