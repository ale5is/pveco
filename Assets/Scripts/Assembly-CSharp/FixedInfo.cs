public static class FixedInfo
{
	public static string FileExtension = ".smf";

	public static string SAAInfo = "SAinfo" + FileExtension;

	public static string PlayerInfoName = "Pinfo" + FileExtension;

	public static string SettingInfoName = "Setting" + FileExtension;

	public static int BasePlant = 11;

	public static int Shadow = 12;

	public static int ProtectBack = 13;

	public static int Vase = 14;

	public static int PlantFront = 20;

	public static int PlantBack = 60;

	public static int FloatPlant = 61;

	public static int ProtectPlant = 62;

	public static int BulletSort = 199;

	public static int Ladder = 59;

	public static int CageBack = 9;

	public static int CageFront = 60;

	public static int Splash = 194;

	public static float LadderPlantHp = 1000f;

	public static int GetBaseSort(int line)
	{
		return line * 200;
	}
}
