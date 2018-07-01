namespace WinAppSample_Wpf_CodeBehined.Service
{
	/// <summary>
	/// 計算処理のインターフェース
	/// </summary>
	/// <typeparam name="T">解の型</typeparam>
	public interface ICalculator<T> where T : struct
	{
		/// <summary>
		/// 計算対象の値の検証を行う
		/// </summary>
		/// <param name="errorMessage">エラーメッセージ</param>
		/// <returns>検証結果</returns>
		bool Validate(out string errorMessage);

		/// <summary>
		/// 計算を行う
		/// </summary>
		/// <returns>計算結果の値</returns>
		T Calculate();
	}
}
