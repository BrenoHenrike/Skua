namespace Skua.Core.Interfaces;
public interface IProcessStartService
{
    void OpenLink(string link);
    void OpenVSC();
    void OpenVSC(string path);
}
