
using UnityEngine;

public class Colors
{
	const float c_00 = 0.0f / 255.0f;
	const float c_55 = 85.0f / 255.0f;
	const float c_AA = 170.0f / 255.0f;
	const float c_FF = 255.0f / 255.0f;

	public static Color Black = new Color( c_00, c_00, c_00 );
	public static Color DarkBlue = new Color( c_00, c_00, c_AA );
	public static Color DarkGreen = new Color( c_00, c_AA, c_00 );
	public static Color DarkCyan = new Color( c_00, c_AA, c_AA );
	public static Color DarkRed = new Color( c_AA, c_00, c_00 );
	public static Color DarkMagenta = new Color( c_AA, c_00, c_AA );
	public static Color Brown = new Color( c_AA, c_55, c_00 );
	public static Color LightGray = new Color( c_AA, c_AA, c_AA );
	public static Color DarkGray = new Color( c_55, c_55, c_55 );
	public static Color LightBlue = new Color( c_55, c_55, c_FF );
	public static Color LightGreen = new Color( c_55, c_FF, c_55 );
	public static Color LightCyan = new Color( c_55, c_FF, c_FF );
	public static Color LightRed = new Color( c_FF, c_55, c_55 );
	public static Color LightMagenta = new Color( c_FF, c_55, c_FF );
	public static Color Yellow = new Color( c_FF, c_FF, c_55 );
	public static Color White = new Color( c_FF, c_FF, c_FF );
}
