using System;


namespace WinAppSample_Wpf_CodeBehined.Service
{
	/// <summary>
	/// 乗算処理を行うクラス
	/// </summary>
	/// <typeparam name="T">被乗数、乗数、解の型</typeparam>
	public class MultiplicationCalculator<T> : ICalculator<T> where T : struct
	{
		#region private fields
		private T multiplicand;
		private T multiplier;
		#endregion

		#region constructors
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="multiplicand">被乗数</param>
		/// <param name="multiplier">乗数</param>
		public MultiplicationCalculator(T multiplicand, T multiplier)
		{
			this.multiplicand = multiplicand;
			this.multiplier = multiplier;
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
			return true;
		}

		/// <summary>
		/// 計算を行う
		/// </summary>
		/// <returns>計算結果の値</returns>
		public T Calculate()
		{
			switch (this.multiplicand)
			{
				case float floatMultiplicand:
					return (T)(object)(floatMultiplicand * (float)(object)this.multiplier);
				default:
					throw new NotImplementedException();
			}
		}
		#endregion
	}
}
