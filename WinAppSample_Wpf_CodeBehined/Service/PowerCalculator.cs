using System;
using System.Threading;


namespace WinAppSample_Wpf_CodeBehined.Service
{
	/// <summary>
	/// べき乗の計算処理を行うクラス
	/// </summary>
	/// <typeparam name="T">底、指数、解の型</typeparam>
	public class PowerCalculator<T> : ICalculator<T> where T : struct
	{
		#region private fields
		private T baseValue;
		private T exponent;
		#endregion

		#region constructors
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="baseValue">底</param>
		/// <param name="exponent">指数</param>
		public PowerCalculator(T baseValue, T exponent)
		{
			this.baseValue = baseValue;
			this.exponent = exponent;
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
			Thread.Sleep(10000);
			switch (this.baseValue)
			{
				case float floatBaseValue:
					return (T)(object)Convert.ToSingle(Math.Pow(floatBaseValue, (float)(object)this.exponent));
				default:
					throw new NotImplementedException();
			}
		}
		#endregion
	}
}
