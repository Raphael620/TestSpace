using System.Diagnostics;
using System.Runtime.InteropServices;

string env = "C:\\xxx\\python\\";
string script = @"import os
print(len(os.environ))
print(os.environ['TEST_KEY'])
";

//string previous = Environment.CurrentDirectory;
//Directory.SetCurrentDirectory(env);
IntPtr dll = NativeLibrary.Load(Path.Combine(env, "python3.dll"));
PyMain? pyMain = null;
if (dll != IntPtr.Zero) pyMain = Marshal.GetDelegateForFunctionPointer<PyMain>(
    NativeLibrary.GetExport(dll, "Py_Main"));
//Directory.SetCurrentDirectory(previous);

System.Diagnostics.Debug.Print(Environment.GetEnvironmentVariables().Count.ToString());
Environment.SetEnvironmentVariable("TEST_KEY", "test_var");
System.Diagnostics.Debug.Print(Environment.GetEnvironmentVariables().Count.ToString());

List<string> argvs = new List<string>();
argvs.Add(Process.GetCurrentProcess().MainModule.FileName);
argvs.Add("-I");
argvs.Add("-s");
argvs.Add("-S");
argvs.Add("-c");
argvs.Add(script);
IntPtr argPtr = Marshal.AllocHGlobal(IntPtr.Size * argvs.Count);
for (int i = 0; i < argvs.Count; i++)
{
    Marshal.WriteIntPtr(argPtr, i * IntPtr.Size, Marshal.StringToHGlobalUni(argvs[i]));
}
int res = pyMain(argvs.Count, argPtr);
System.Diagnostics.Debug.Print(res.ToString());

delegate int PyMain(int argc, IntPtr argv);
