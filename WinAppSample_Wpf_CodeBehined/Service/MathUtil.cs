using System;


namespace WinAppSample_Wpf_CodeBehined.Service
{
	/// <summary>
	/// 計算処理のユーティリティクラス
	/// </summary>
	public static class MathUtil
	{
		#region public methods
		/// <summary>
		/// 角度をラジアンに変換する。
		/// </summary>
		/// <param name="degree">角度</param>
		/// <returns>ラジアン</returns>
		public static double DegreeToRadian(double degree)
		{
			return degree * (Math.PI / 180);
		}
		#endregion
	}
}
