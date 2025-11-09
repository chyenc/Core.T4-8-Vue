using MyChy.Frame.Core.Common.Extensions;
using MyChy.Frame.Core.EFCore.Entitys;
using MyChy.Frame.Core.EFCore.Entitys.Abstraction;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.Loader;
using System.Runtime.InteropServices;

namespace MyChy.Core.T4.Common
{
    public class LoadDll
    {

        private const string T4NotGenerate = "T4NotGenerateAttribute";
        private const string T4ViewGenerate = "T4ViewGenerateAttribute";

        private IList<string> OutAttributes = new List<string> { "Id","CreatedOn", "CreatedBy", "UpdatedOn", "UpdatedBy",
                                                                 "DeletedOn", "DeletedBy", "IsDeleted","SortIndex" };

        private IList<string> OutNamespace = new List<string> { "CommonUse", "Certifications", "Logs", "Competences", "Expands" };

        private const string FileNmae = "MyChy.Core.Domains.";

        public IList<MyChyEntityNamespace> AnalysisLoad()
        {
            bool IsAttributes;
            var list = LoadAttribute(out IsAttributes);

            if (!IsAttributes)
            {
                list = LoadFolder();
            }
            return AnalysisType(list);
            // return new List<MyChyEntityNamespace>();
        }

        public string GetDirectory()
        {
            var solutionsPath = Directory.GetCurrentDirectory();

            // Remove the build output suffix depending on OS.
            // On Windows paths use backslashes; on macOS/Linux they use forward slashes.
            var windowsSuffix = "\\MyChy.Core.T4.Console\\bin\\Debug\\net8.0";
            var unixSuffix = "/MyChy.Core.T4.Console/bin/Debug/net8.0";

            try
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) || RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    solutionsPath = solutionsPath.Replace(unixSuffix, "");
                }
                else
                {
                    solutionsPath = solutionsPath.Replace(windowsSuffix, "");
                }
            }
            catch
            {
                // Fallback: try removing both suffix forms in case detection fails or paths differ.
                solutionsPath = solutionsPath.Replace(windowsSuffix, "").Replace(unixSuffix, "");
            }

            return solutionsPath;
        }


        private IEnumerable<Type> LoadFolder()
        {
            List<Type> typeToRegisterCustomModelBuilders = new List<Type>();
            string solutionsPath = GetDirectory();

            // var path = Directory.GetDirectories(solutionsPath);
            string modelFile = Path.Combine(solutionsPath, "Lib", $"{FileNmae}dll");

            Assembly assembly;
            try
            {
                assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(modelFile);

            }
            catch (FileLoadException)
            {
                // Get loaded assembly
                assembly = Assembly.Load(new AssemblyName(Path.GetFileNameWithoutExtension(modelFile)));

                if (assembly == null)
                {
                    throw;
                }
            }

            //获取所有继承BaseEntity的实体
            var entityClassTypes = assembly.ExportedTypes.Where(x => typeof(IEntity).IsAssignableFrom(x) && !x.GetTypeInfo().IsAbstract);

            var outentityClassTypes = new List<Type>();
            var Namespace = string.Empty;
            foreach (var i in entityClassTypes)
            {
                Namespace = i.Namespace.Replace(FileNmae, "");
                if (!OutNamespace.Contains(Namespace))
                {
                    outentityClassTypes.Add(i);
                }
            }

            //( x.GetTypeInfo().IsSubclassOf(typeof(BaseEntity)) && !x.GetTypeInfo().IsAbstract)||
            typeToRegisterCustomModelBuilders.AddRange(outentityClassTypes);
            // && !x.GetTypeInfo().IsDefined(typeof(MapIgnoreAttribute), false)




            return typeToRegisterCustomModelBuilders;


        }


        private IEnumerable<Type> LoadAttribute(out bool IsAttributes)
        {
            List<Type> typeToRegisterCustomModelBuilders = new List<Type>();
            string solutionsPath = GetDirectory();
            IsAttributes = false;
            // var path = Directory.GetDirectories(solutionsPath);

            string modelFile = Path.Combine(solutionsPath, "Lib", $"{FileNmae}dll");

            Assembly assembly;
            try
            {
                assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(modelFile);

            }
            catch (FileLoadException)
            {
                // Get loaded assembly
                assembly = Assembly.Load(new AssemblyName(Path.GetFileNameWithoutExtension(modelFile)));

                if (assembly == null)
                {
                    throw;
                }
            }

            //获取所有继承BaseEntity的实体
            var entityClassTypes = assembly.ExportedTypes.Where(x => typeof(IEntity).IsAssignableFrom(x) && !x.GetTypeInfo().IsAbstract);

            var outentityClassTypes = new List<Type>();
            var Namespace = string.Empty;
            foreach (var i in entityClassTypes)
            {
                bool IsGenerate = true;
                foreach (var x in i.CustomAttributes)
                {
                    var fullname = x.AttributeType.FullName.ToString();
                    var leg = fullname.IndexOf("MyChy.Core.Domains.Attributes");
                    if (leg == 0)
                    {
                        var AttributeName = fullname.Substring(30);
                        if (AttributeName == T4NotGenerate)
                        {
                            IsGenerate = false;
                            IsAttributes = true;
                        }
                    }
                }
                if (IsGenerate)
                {
                    outentityClassTypes.Add(i);
                }
            }

            //( x.GetTypeInfo().IsSubclassOf(typeof(BaseEntity)) && !x.GetTypeInfo().IsAbstract)||
            typeToRegisterCustomModelBuilders.AddRange(outentityClassTypes);
            // && !x.GetTypeInfo().IsDefined(typeof(MapIgnoreAttribute), false)

            return typeToRegisterCustomModelBuilders;


        }

        private IList<MyChyEntityNamespace> AnalysisType(IEnumerable<Type> List)
        {
            var result = new List<MyChyEntityNamespace>();
            if (List != null && List.Count() > 0)
            {
                List = List.OrderBy(x => x.Namespace);
                string np = string.Empty;

                var Mep = new MyChyEntityNamespace();
                var entity = new MyChyEntity();
                var attributes = new MyChyEntityAttributes();
                foreach (var i in List)
                {
                    np = i.Namespace.Replace(FileNmae, "");
                    if (!string.IsNullOrEmpty(Mep.Namespace) && np != Mep.Namespace)
                    {
                        result.Add(Mep);
                        Mep = new MyChyEntityNamespace
                        {
                            Namespace = i.Namespace.Replace(FileNmae, "")
                        };
                    }
                    Mep.Namespace = np;
                    entity = new MyChyEntity
                    {
                        Name = i.Name,
                        Description = i.ToDescription(),
                        Alias = i.Name,
                    };
                    if (i.BaseType == typeof(BaseWithAllEntity))
                    {
                        entity.IsBaseWithAllEntity = true;
                    }

                    foreach (var x in i.CustomAttributes)
                    {
                        var fullname = x.AttributeType.FullName.ToString();
                        var key = "MyChy.Core.Domains.Attributes";
                        var leg = fullname.IndexOf(key);
                        if (leg == 0)
                        {
                            var AttributeName = fullname.Substring(key.Length + 1);
                            if (AttributeName == T4ViewGenerate)
                            {
                                entity.IsViewEntity = true;
                                break;
                            }
                            else if (AttributeName == "AuthorityAttribute")
                            {
                                entity.AuthorityCode = x.ConstructorArguments[0].Value.To<string>();
                            }
                            else if (AttributeName == "AliasAttribute")
                            {
                                entity.Alias = x.ConstructorArguments[0].Value.To<string>();
                            }
                            entity.CustomAttributeList.Add(AttributeName);
                        }
                        key = "MyChy.Frame.Core.EFCore.Entitys.Attributes";
                        leg = fullname.IndexOf(key);
                        if (leg == 0)
                        {
                            var AttributeName = fullname.Substring(key.Length + 1);
                            entity.CustomAttributeList.Add(AttributeName);
                        }

                    }

                    var m = Activator.CreateInstance(i);
                    var col = TypeDescriptor.GetProperties(m);
                    var list = new HashSet<string>();
                    foreach (PropertyDescriptor item in col)
                    {
                        attributes = new MyChyEntityAttributes
                        {
                            Description = item.Description,
                            Name = item.Name,
                            Types0f = ConvertTypes0f(item.PropertyType.ToString()),
                            List = new List<EntityAttributesInfo>(),
                            AttributesName = new List<string>(),

                        };
                        foreach (var x in item.Attributes)
                        {
                            var leg = x.ToString().IndexOf("MyChy.Core.Domains.Attributes");
                            if (leg == 0)
                            {
                                var info = new EntityAttributesInfo();

                                attributes.Types0f = "Attributes";
                                info.Name = x.ToString().Substring(30);
                                if (info.Name == "EnumListStringAttribute"
                                    || info.Name == "EnumListCheckAttribute")
                                {
                                    info.Code = GetModelValue("Code", x);
                                    if (!entity.ServiceList.Contains("Expands"))
                                    {
                                        entity.ServiceList.Add("Expands");
                                    }

                                }
                                else if (info.Name == "TableToAttribute")
                                {
                                    if (!entity.ScriptList.Contains("TableToAttribute"))
                                    {
                                        entity.ScriptList.Add("TableToAttribute");
                                    }

                                    info.One = GetModelValue("TableName", x);
                                    info.Two = GetModelValue("Area", x);
                                    info.Three = GetModelValue("Column", x);
                                    if (info.Two != "BaseArea" && !string.IsNullOrEmpty(info.Two))
                                    {
                                        if (!entity.ServiceList.Contains(info.Two))
                                        {
                                            entity.ServiceList.Add(info.Two);
                                        }
                                    }

                                }
                                else if (info.Name == "ThumbnailAttribute")
                                {
                                    info.One = GetModelValue("ThumWith", x);
                                    info.Two = GetModelValue("ThumHigth", x);
                                    entity.IsThumbnail = true;
                                }
                                else if (info.Name == "TableColumnAttribute")
                                {
                                   // attributes.Types0f = ConvertTypes0f(item.PropertyType.ToString());
                                }
                                if (!attributes.AttributesName.Contains(info.Name))
                                {
                                    attributes.AttributesName.Add(info.Name);
                                    attributes.List.Add(info);
                                }

                            }
                        }
                        if (attributes.Types0f == "Enum")
                        {
                            attributes.EnumName = item.PropertyType.ToString().Substring(19);
                        }
                        if (OutAttributes.Contains(item.Name))
                        {
                            entity.OutAttributes.Add(attributes);
                        }
                        else
                        {
                            entity.Attributes.Add(attributes);
                        }

                        entity.AttributeName.Add(attributes.Name);
                    }
                    Mep.FileName.Add(entity);

                }
                result.Add(Mep);
            }


            return result;

        }

        public string GetModelValue(string FieldName, object obj)
        {
            try
            {
                Type Ts = obj.GetType();
                object o = Ts.InvokeMember(FieldName, BindingFlags.GetField,
                             null, obj, new object[] { });

                //object o = Ts.GetProperty(FieldName).GetValue(obj, null);
                string Value = Convert.ToString(o);
                if (string.IsNullOrEmpty(Value)) return null;
                return Value;
            }
            catch (Exception e)
            {
                string error = e.Message;
                return null;
            }

        }

        private string ConvertTypes0f(string val)
        {
            var result = "string";
            switch (val)
            {
                case "System.Int32":
                    result = "int";
                    break;
                case "System.Int64":
                    result = "long";
                    break;

                case "System.String":
                    result = "string";
                    break;

                case "System.Boolean":
                    result = "bool";
                    break;

                case "System.DateTimeOffset":
                    result = "DateTimeOffset";
                    break;

                case "System.Nullable`1[System.DateTimeOffset]":
                    result = "DateTimeOffset";
                    break;

                case "System.Nullable`1[System.DateTime]":
                    result = "DateTime";
                    break;
                case "System.DateTime":
                    result = "DateTime";
                    break;
                case "System.Decimal":
                    result = "decimal";
                    break;
                case "System.Nullable`1[System.Decimal]":
                    result = "decimal?";
                    break;
                case "System.Nullable`1[System.Int32]":
                    result = "int?";
                    break;
                case "System.Nullable`1[System.Boolean]":
                    result = "bool?";
                    break;
                default:
                    var xx = val.IndexOf("MyChy.Core.Domains.");
                    if (val.IndexOf("MyChy.Core.Domains.") == 0)
                    {
                        result = "Enum";
                    }

                    break;
            }
            return result;
        }
    }
}
