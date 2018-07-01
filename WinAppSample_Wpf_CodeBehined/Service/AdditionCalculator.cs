using System;


namespace WinAppSample_Wpf_CodeBehined.Service
{
	/// <summary>
	/// 加算処理を行うクラス
	/// </summary>
	/// <typeparam name="T">被加数、加数、解の型</typeparam>
	public class AdditionCalculator<T> : ICalculator<T> where T : struct
	{
		#region private fields
		private T augend;
		private T addend;
		#endregion

		#region constructors
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="augend">被加数</param>
		/// <param name="addend">加数</param>
		public AdditionCalculator(T augend, T addend)
		{
			this.augend = augend;
			this.addend = addend;
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
			switch (this.augend)
			{
				case float floatAugend:
					return (T)(object)(floatAugend + (float)(object)this.addend);
				default:
					throw new NotImplementedException();
			}
		}
		#endregion
	}
}
