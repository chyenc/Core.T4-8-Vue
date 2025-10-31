using MyChy.Core.T4.Common;
using MyChy.Frame.Core.Common.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MyChy.Core.T4.Template
{
    public class CoreData
    {
        private const string IPath = "/MyChy.Core.Data";
        private const string Ipath = "/WorkArea";
        private const string Implementation = "/Implementation";

        /// <summary>
        /// 输出模板
        /// </summary>
        /// <param name="Path"></param>
        /// <param name="EntityNamespace"></param>
        public async Task Write(string Path, IList<MyChyEntityNamespace> list)
        {

            var file = Path + IPath;
           // FileHelper.DeleteFolder(file);

            FileHelper.CreatedFolder(file);
            await CreatModuleInitializer(file, list);

            file = file + Ipath;
            FileHelper.CreatedFolder(file);
            await CreatIWorkArea(file, list);

            file = file + Implementation;
            FileHelper.CreatedFolder(file);
            await CreatWorkArea(file, list);


        }

        private async Task CreatModuleInitializer(string Path, IList<MyChyEntityNamespace> list)
        {
            string files = Path + "/ModuleInitializer.txt";

            var _sw = new StreamWriter(new FileStream(files, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.Read), Encoding.UTF8);

            StringBuilder sb = new StringBuilder();
            foreach (var i in list)
            {
                sb.AppendLine($"using MyChy.Core.Data.{i.Namespace};" );
                sb.AppendLine($"using MyChy.Core.Data.{i.Namespace}.Implementation;");
               
            }

            sb.AppendLine("");


            foreach (var i in list)
            {
                sb.AppendFormat("services.AddTransient<I{0}WorkArea, {0}WorkArea>();", i.Namespace);
                sb.AppendLine("");
            }
            await _sw.WriteAsync(sb.ToString());
            _sw.Close();


        }

        private async Task CreatIWorkArea(string Path, IList<MyChyEntityNamespace> list)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var i in list)
            {
                sb = new StringBuilder();
                string files = Path + $"/I{i.Namespace}WorkArea.cs";
                var _sw = new StreamWriter(new FileStream(files, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.Read), Encoding.UTF8);

                sb.AppendLine($"using MyChy.Core.Domains.{i.Namespace};");
           
                sb.AppendLine("using MyChy.Frame.Core.EFCore.Repositorys.Interface;");
                sb.AppendLine($"namespace MyChy.Core.Data.{i.Namespace}");
                sb.AppendLine("{");
                sb.AppendLine($"public interface I{i.Namespace}WorkArea");
                sb.AppendLine("{");
                foreach (var x in i.FileName)
                {
                    sb.Append($"IBaseRepository<{x.Name}>  {x.Name}R ");
                    sb.AppendLine("{ get; }");

                    sb.AppendLine("");
                }
                sb.AppendLine("}");
                sb.AppendLine("}");

                await _sw.WriteAsync(sb.ToString());
                _sw.Close();

            }


        }


        private async Task  CreatWorkArea(string Path, IList<MyChyEntityNamespace> list)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var i in list)
            {
                sb = new StringBuilder();
                string files = Path + $"/{i.Namespace}WorkArea.cs";
                var _sw = new StreamWriter(new FileStream(files, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.Read), Encoding.UTF8);

                sb.AppendLine($"using MyChy.Core.Domains.{i.Namespace};");
                sb.AppendLine("using MyChy.Frame.Core.EFCore;");
                sb.AppendLine("using MyChy.Frame.Core.EFCore.Repositorys.Interface;");
                sb.AppendLine($"namespace MyChy.Core.Data.{i.Namespace}.Implementation");
                sb.AppendLine("{");
                sb.AppendLine($"public class {i.Namespace}WorkArea : EFCoreWorkArea<CoreDbContext>, I{i.Namespace}WorkArea");
                sb.AppendLine("{");
                sb.AppendLine($"public {i.Namespace}WorkArea(CoreDbContext context) : base(context)");
                sb.AppendLine("{");
                foreach (var x in i.FileName)
                {
                    sb.AppendLine($"{x.Name}R = new BaseRepository<{x.Name}>(context);");
                }
                sb.AppendLine("}");
                sb.AppendLine("");
                foreach (var x in i.FileName)
                {
                    //sb.Append("public IBaseRepository<Picture> PictureR { get; }");
                    sb.Append($"public IBaseRepository<{x.Name}>  {x.Name}R ");
                    sb.AppendLine("{ get; }");
                }
                sb.AppendLine("}");
                sb.AppendLine("}");
                await _sw.WriteAsync(sb.ToString());
                _sw.Close();

            }


        }
    }
}
