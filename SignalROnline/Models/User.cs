namespace SignalROnline.Models
{
	public class User
	{
		public  int Id { get; set; }
		public string Username { get; set; }
		public string Nickname { get; set; }
		public Icon ?Icon { get; set; }
		public int Level { get; set; } = 0;
	}
	public enum Icon
	{
		Sonic,Tarzan,Naruto,Luffy

	}
}
