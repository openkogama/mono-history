using System.IO;

public class LogFileAppender : IAppender
{
	private FileStream fs;

	private StreamWriter writer;

	public LogFileAppender(string filename)
	{
		fs = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Write);
		writer = new StreamWriter(fs);
	}

	public void Log(string loggerName, string message)
	{
		writer.WriteLine(loggerName + ": " + message);
		writer.Flush();
	}

	~LogFileAppender()
	{
		writer.Close();
		fs.Close();
	}
}
