using static WinAppSample_Wpf_CodeBehined.Presentation.MainWindow;


namespace WinAppSample_Wpf_CodeBehined.Presentation
{
	/// <summary>
	/// 列挙型の拡張メソッドを定義するクラス
	/// </summary>
	public static class EnumExtension
	{
		/// <summary>
		/// <see cref="OtherCalcPattern"/>の各列挙子と対応する日本語文字列を取得する
		/// </summary>
		/// <param name="type">列挙子</param>
		/// <returns>日本語名</returns>
		public static string Name(this OtherCalcPattern type)
		{
			switch (type)
			{
				case OtherCalcPattern.Power:
					return "べき乗";
				case OtherCalcPattern.Sine:
					return "sin";
				case OtherCalcPattern.Cosine:
					return "cos";
				default:
					return string.Empty;
			}
		}
	}
}
