using System;


[Serializable]
public class ProcessingLogItem
{
    public int Index = 0;
    public DateTime DateTime = DateTime.Now;
    public string Message = "";

    public string GetLogString()
    {
        return "[" + DateTime.ToString("HH:mm:ss") + "] " + Message;
    }
}
