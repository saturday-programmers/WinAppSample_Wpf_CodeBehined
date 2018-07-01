using System;


namespace WinAppSample_Wpf_CodeBehined.Service
{
	/// <summary>
	/// 除算処理を行うクラス
	/// </summary>
	/// <typeparam name="T">被除数、除数、解の型</typeparam>
	public class DivisionCalculator<T> : ICalculator<T> where T : struct
	{
		#region private fields
		private T dividend;
		private T divisor;
		#endregion

		#region constructors
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="dividend">被除数</param>
		/// <param name="divisor">除数</param>
		public DivisionCalculator(T dividend, T divisor)
		{
			this.dividend = dividend;
			this.divisor = divisor;
		}
		#endregion

		#region public methods
		/// <summary>
		/// 計算対象の値の検証を行う
		/// </summary>
		/// <param name="errorMessage">エラーメッセージ</param>
		/// <returns>検証結果</returns>
		public bool Validate(out string errorMessage)
		{
			errorMessage = null;
			if (double.Parse(this.divisor.ToString()) == 0)
			{
				errorMessage = "0で割ることはできません。";
			}
			return (errorMessage == null);
		}

		/// <summary>
		/// 計算を行う
		/// </summary>
		/// <returns>計算結果の値</returns>
		public T Calculate()
		{
			switch (this.dividend)
			{
				case float floatDividend:
					return (T)(object)(floatDividend / (float)(object)this.divisor);
				default:
					throw new NotImplementedException();
			}
		}
		#endregion
	}
}
