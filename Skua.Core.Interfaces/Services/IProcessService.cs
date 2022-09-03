namespace Skua.Core.Interfaces;
public interface IProcessService
{
    void OpenLink(string link);
    void OpenVSC();
    void OpenVSC(string path);
}
