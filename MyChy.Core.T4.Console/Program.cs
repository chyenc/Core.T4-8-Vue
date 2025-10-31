// See https://aka.ms/new-console-template for more information
using MyChy.Core.T4.Common;
using MyChy.Core.T4.Template;
using MyChy.Frame.Core.Common.Helper;

string IPath = "/T4";

Console.WriteLine("Hello, World!");
var dll = new LoadDll();

var list = dll.AnalysisLoad();

var path = dll.GetDirectory();

path = path + IPath;

FileHelper.DeleteFolder(path);

var ss = RunCoreData(path, list);
ss.Wait();

var ss1 = RunViewModels(path, list);
ss1.Wait();

var ss2 = RunWeb(path, list);
ss2.Wait();

var ss3 = RunService(path, list);
ss3.Wait();


var ss4 = RunWebCore(path, list);
ss4.Wait();

var ss5 = RunCoreDomains(path, list);
ss5.Wait();

var ss6 = RunCoreEFStartupTask(path, list);
ss6.Wait();


var sss = RunSuccess(path, list);
sss.Wait();

RunWebVue(path, list).Wait();




static async Task RunCoreData(string path, IList<MyChyEntityNamespace> list)
{
    var cd = new CoreData();
    await cd.Write(path, list);


}

static async Task RunViewModels(string path, IList<MyChyEntityNamespace> list)
{
    var cd = new ViewModels();
    await cd.Write(path, list);


}


static async Task RunWeb(string path, IList<MyChyEntityNamespace> list)
{
    var cd = new Web();
    await cd.Write(path, list);
}

static async Task RunService(string path, IList<MyChyEntityNamespace> list)
{
    var cd = new Service();
    await cd.Write(path, list);

    var cd1 = new ServiceWeb();
    await cd1.Write(path, list);

    var cd2 = new ServiceWebFront();
    await cd2.Write(path, list);

    var cd3 = new ServiceWebService();
    await cd3.Write(path, list);

    var cd4 = new ServiceWebInterface();
    await cd4.Write(path, list);
    

}

static async Task RunWebCore(string path, IList<MyChyEntityNamespace> list)
{
    var cd = new WebCore();
    await cd.Write(path, list);


}

static async Task RunCoreDomains(string path, IList<MyChyEntityNamespace> list)
{
    var cd = new CoreDomains();
    await cd.Write(path, list);


}

static async Task RunCoreEFStartupTask(string path, IList<MyChyEntityNamespace> list)
{
    var cd = new CoreEFStartupTask();
    await cd.Write(path, list);

}

static async Task RunWebVue(string path, IList<MyChyEntityNamespace> list)
{
    var cd = new WebVue();
    await cd.Write(path, list);

}


static async Task RunSuccess(string path, IList<MyChyEntityNamespace> list)
{
    var cd = new Success();
    await cd.Write(path, list);


}