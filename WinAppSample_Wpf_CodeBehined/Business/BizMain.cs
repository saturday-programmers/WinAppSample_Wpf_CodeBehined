using System;
using System.Threading;
using System.Threading.Tasks;
using WinAppSample_Wpf_CodeBehined.Service;


namespace WinAppSample_Wpf_CodeBehined.Business
{
	// <summary>
	/// メイン画面のビジネスロジッククラス
	/// </summary>
	public class BizMain
	{
		#region private fields
		private Task<float> calcultionTask;
		private CancellationTokenSource tokenSource;
		#endregion

		#region enums
		/// <summary>
		/// 計算種別
		/// </summary>
		public enum CalculateType
		{
			/// <summary>加算</summary>
			Addition,
			/// <summary>減算</summary>
			Subtraction,
			/// <summary>乗算</summary>
			Multiplication,
			/// <summary>除残</summary>
			Division,
			/// <summary>べき乗</summary>
			Power,
			/// <summary>Sin</summary>
			Sine,
			/// <summary>Cos</summary>
			Cosine,
		}
		#endregion

		#region public properties
		public bool IsCalculating => this.calcultionTask?.Status == TaskStatus.Running || this.calcultionTask?.Status == TaskStatus.WaitingToRun;
		#endregion

		#region public methods
		/// <summary>
		/// 計算処理を行う
		/// </summary>
		/// <param name="calculateType">計算種別</param>
		/// <param name="values">計算対象の値</param>
		/// <returns>計算結果</returns>
		/// <exception cref="ApplicationException">業務エラー発生時に例外を発生させる。</exception>
		public Task<float> CalculateAsync(CalculateType calculateType, params float[] values)
		{
			if (!this.HasCorrectParameterCount(calculateType, values))
			{
				throw new ArgumentException("引数の数が正しくありません。");
			}

			//　計算処理を行うインスタンス生成
			var calculator = this.CreateCalculator(calculateType, values);
			if (calculator == null)
			{
				throw new NotImplementedException(calculateType.ToString() + "の処理は実装されていません。");
			}

			string errorMessage;
			if (calculator.Validate(out errorMessage))
			{
				// 処理キャンセル時に必要なオブジェクトを保持しておく
				this.tokenSource = new CancellationTokenSource();
				this.tokenSource.Token.ThrowIfCancellationRequested();
				// 非同期で計算処理実行
				// 既定ではオーバーフロー時に例外は発生しないので(unchecked)考慮しない
				this.calcultionTask = Task.Run(() => calculator.Calculate(), this.tokenSource.Token);
			}
			else
			{
				this.calcultionTask = Task.FromException<float>(new ApplicationException(errorMessage));
			}

			return this.calcultionTask;
		}

		/// <summary>
		/// 実行中の計算処理を停止する。
		/// </summary>
		public void CancelCalculation()
		{
			if (!this.tokenSource?.IsCancellationRequested ?? true)
			{
				this.tokenSource?.Cancel(true);
				this.tokenSource?.Dispose();
			}
		}
		#endregion

		#region private methods
		private bool HasCorrectParameterCount(CalculateType calculateType, params float[] values)
		{
			int count = 0;
			switch (calculateType)
			{
				case CalculateType.Addition:
				case CalculateType.Subtraction:
				case CalculateType.Multiplication:
				case CalculateType.Division:
				case CalculateType.Power:
					count = 2;
					break;
				case CalculateType.Sine:
				case CalculateType.Cosine:
					count = 1;
					break;
			}

			return (values.Length == count);
		}

		private ICalculator<float> CreateCalculator(CalculateType calculateType, params float[] values)
		{
			ICalculator<float> ret = null;
			switch (calculateType)
			{
				case CalculateType.Addition:
					ret = new AdditionCalculator<float>(values[0], values[1]);
					break;
				case CalculateType.Subtraction:
					ret = new SubtractionCalculator<float>(values[0], values[1]);
					break;
				case CalculateType.Multiplication:
					ret = new MultiplicationCalculator<float>(values[0], values[1]);
					break;
				case CalculateType.Division:
					ret = new DivisionCalculator<float>(values[0], values[1]);
					break;
				case CalculateType.Power:
					ret = new PowerCalculator<float>(values[0], values[1]);
					break;
				case CalculateType.Sine:
					ret = new SineCalculator<float>(values[0]);
					break;
				case CalculateType.Cosine:
					ret = new CosineCalculator<float>(values[0]);
					break;
			}

			return ret;
		}
		#endregion
	}
}
