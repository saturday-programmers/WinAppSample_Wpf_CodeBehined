using System;


namespace WinAppSample_Wpf_CodeBehined.Service
{
	/// <summary>
	/// Sin関数の計算を行うクラス
	/// </summary>
	/// <typeparam name="T">角度の型</typeparam>
	public class SineCalculator<T> : ICalculator<T> where T : struct
	{
		#region private fields
		private T degree;
		#endregion

		#region constructors
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="degree">角度</param>
		public SineCalculator(T degree)
		{
			this.degree = degree;
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
			switch (this.degree)
			{
				case float floatDegree:
					return (T)(object)Convert.ToSingle(Math.Sin(MathUtil.DegreeToRadian(floatDegree)));
				default:
					throw new NotImplementedException();
			}
		}
		#endregion

	}
}




