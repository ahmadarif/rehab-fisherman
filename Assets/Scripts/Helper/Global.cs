public class Global
{
	public string message;

	private Global()
	{
		// Prevent outside instantiation
	}

	private static readonly Global _singleton = new Global();

	public static Global Instance()
	{
		return _singleton;
	}

}