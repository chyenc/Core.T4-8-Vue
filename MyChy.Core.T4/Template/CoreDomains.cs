using MyChy.Core.T4.Common;
using MyChy.Frame.Core.Common.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MyChy.Core.T4.Template
{
    public class CoreDomains
    {
        private const string IPath = "/MyChy.Core.Domains";
        private const string IMappings = "/ModelBuilders";
 



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

            file = file + IMappings;
            FileHelper.CreatedFolder(file);

            await CreatModuleInitializer(file, list);
       
        }

        private async Task CreatModuleInitializer(string Path, IList<MyChyEntityNamespace> list)
        {
            string files = Path + "/CustomModelBuilder.txt";

            var _sw = new StreamWriter(new FileStream(files, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.Read), Encoding.UTF8);

            StringBuilder sb = new StringBuilder();
            foreach (var i in list)
            {
                sb.AppendLine($"{i.Namespace}(modelBuilder);");
                sb.AppendLine("");
            }


            foreach (var i in list)
            {
     
                sb.AppendLine("");
                sb.AppendLine($"public void {i.Namespace}(ModelBuilder modelBuilder)");
                sb.AppendLine("{");
                foreach (var x in i.FileName)
                {
                    sb.AppendLine("");
                    sb.AppendLine($"modelBuilder.Entity<{x.Name}>(b =>");
                    sb.AppendLine("{");
                    if (x.IsBaseWithAllEntity)
                    {
                        sb.AppendLine("b.MapCreatedMeta().MapUpdatedMeta().MapDeletedMeta();");
                        sb.AppendLine("b.HasQueryFilter(x => x.IsDeleted == false);");
                    }

                    foreach (var y in x.Attributes)
                    {
                        if (y.Types0f == "decimal"||y.Types0f== "decimal?")
                        {
                            sb.AppendLine($"b.Property(x => x.{y.Name}).HasPrecision(18, 4);");
                        }
                    }

                    if (!x.IsViewEntity)
                    {
                        sb.AppendLine($"b.ToTable(\"{x.Name}\");");
                    }
                    else
                    {
                        sb.AppendLine($"b.ToView(\"{x.Name}\");");
                    }

                    sb.AppendLine($"//b.ToTable(\"{i.Namespace}{x.Name}\");");
                    sb.AppendLine("});");
                }
                sb.AppendLine("}");
            }
            await _sw.WriteAsync(sb.ToString());
            _sw.Close();


        }
    }
}
