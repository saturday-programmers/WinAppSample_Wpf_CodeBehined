using System;


namespace WinAppSample_Wpf_CodeBehined.Service
{
	/// <summary>
	/// 減算処理を行うクラス
	/// </summary>
	/// <typeparam name="T">被減数、減数、解の型</typeparam>
	public class SubtractionCalculator<T> : ICalculator<T> where T : struct
	{
		#region private fields
		private T minuend;
		private T subtrahend;
		#endregion

		#region constructors
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="minuend">被減数</param>
		/// <param name="subtrahend">減数</param>
		public SubtractionCalculator(T minuend, T subtrahend)
		{
			this.minuend = minuend;
			this.subtrahend = subtrahend;
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
			switch (this.minuend)
			{
				case float floatMinuend:
					return (T)(object)(floatMinuend - (float)(object)this.subtrahend);
				default:
					throw new NotImplementedException();
			}
		}
		#endregion
	}
}
