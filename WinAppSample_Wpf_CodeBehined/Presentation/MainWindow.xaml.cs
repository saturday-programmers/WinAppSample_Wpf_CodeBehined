using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WinAppSample_Wpf_CodeBehined.Business;
using static WinAppSample_Wpf_CodeBehined.Business.BizMain;


namespace WinAppSample_Wpf_CodeBehined.Presentation
{
	/// <summary>
	/// MainWindow.xaml の相互作用ロジック
	/// </summary>
	public partial class MainWindow : Window
	{
		#region constants
		private const string AdditionSign = "＋";
		private const string SubtractionSign = "－";
		private const string MultiplicationSign = "×";
		private const string DivisionSign = "÷";
		private const string PowerSign = "^";
		private const string SineSign = "sin";
		private const string CosineSign = "cos";
		private const int ValueTxtMaxLength = 7;

		private const string NotNumericErrorMessage = "数値を入力して下さい";
		private const string CalculatingMessage = "計算中です...";
		private const string CofirmCancelingMessage = "実行中の計算処理を停止します。よろしいですか？";
		private const string CancelingCalcMessage = "計算処理を停止します";
		#endregion

		#region private fields
		private BizMain biz = new BizMain();
		private List<TextBox> valueTextBoxes;
		#endregion

		#region constructors
		/// <summary>
		/// デフォルトコントラクタ
		/// </summary>
		public MainWindow()
		{
			InitializeComponent();

			this.Loaded += this.OnLoaded;
			this.Closing += this.OnClosing;
		}
		#endregion

		#region enums
		/// <summary>
		/// その他の計算パターン
		/// </summary>
		public enum OtherCalcPattern
		{
			Power,
			Sine,
			Cosine
		}
		#endregion

		#region private properties
		private IEnumerable<TextBox> InputtableTextBoxes => this.valueTextBoxes.Where(x => x.IsEnabled);
		#endregion

		#region event handlers

		#region form
		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			// アセンブリ名のカンマまでをシステム名としてタイトルバーに記載
			this.Title = new string(Assembly.GetEntryAssembly().FullName.TakeWhile(x => x != ',').ToArray());

			this.txtValue1.MaxLength = ValueTxtMaxLength;
			this.txtValue2.MaxLength = ValueTxtMaxLength;
			this.valueTextBoxes = new List<TextBox>(new TextBox[] { this.txtValue1, this.txtValue2 });

			// OtherCalcPattern列挙型の全ての列挙子をリストにしたものをコンボボックスのデータソースに設定
			var otherCalcPatternList = Enum.GetValues(typeof(OtherCalcPattern)).OfType<OtherCalcPattern>().Select(x => x.Name()).ToList();
			this.cmbOtherCalcPattern.ItemsSource = otherCalcPatternList;

			this.btnCancel.IsEnabled = false;

			this.Initialize();
		}

		private void OnClosing(object sender, CancelEventArgs e)
		{
			// 計算処理中の場合はキャンセル処理を同期的に実行
			if (this.biz.IsCalculating)
			{
				if (!this.CancelCalculationAsync(true).Result)
				{
					e.Cancel = true;
				}
			}
		}
		#endregion

		#region buttons
		private async void btnCalc_Click(object sender, RoutedEventArgs e)
		{
			// 選択されている計算パターンとビジネスクラスの計算パターンを紐づける
			var calcType = this.DetermineCalculateType();
			if (!calcType.HasValue)
			{
				return;
			}

			// クリアボタン以外は入力不可にする
			this.ControlAllEnabledProperty(true);

			var values = this.InputtableTextBoxes.Select(x => !string.IsNullOrEmpty(x.Text) ? float.Parse(x.Text) : 0).ToArray();

			this.txtBlockStatusBar.Text = CalculatingMessage;
			// 非同期で計算実行
			try
			{
				var result = await biz.CalculateAsync(calcType.Value, values);
				this.txtBlockResult.Text = result.ToString("#,##0." + new string('#', ValueTxtMaxLength));
				this.txtBlockStatusBar.Text = string.Empty;
			}
			catch (ApplicationException ex)
			{
				// 業務エラー発生時は内容をステータスバーに表示
				this.txtBlockStatusBar.Text = ex.Message;
			}
			catch (AggregateException ex)
			{
				// タスクキャンセルによる例外発生時は処理続行
				if (!ex.InnerExceptions.Any(x => x is TaskCanceledException))
				{
					throw;
				}
			}

			// 入力可能に戻す
			this.ControlAllEnabledProperty(false);
		}

		private async void btnCancel_Click(object sender, RoutedEventArgs e)
		{
			await this.CancelCalculationAsync();
		}

		private void btnClear_Click(object sender, RoutedEventArgs e)
		{
			this.Initialize();
		}
		#endregion

		#region radio buttons
		private void rbtnCalcPattern_CheckedChanged(object sender, RoutedEventArgs e)
		{
			System.Windows.Controls.RadioButton rbtn = (RadioButton)sender;
			if (!rbtn.IsChecked.Value) return;

			// その他が選択された場合のみコンボボックス活性化
			this.cmbOtherCalcPattern.IsEnabled = this.rbtnOther.IsChecked.Value;

			if (this.rbtnOther.IsChecked.Value)
			{
				// 何も選択されていない場合は1番目を選択状態に変更
				if (this.cmbOtherCalcPattern.SelectedIndex == -1)
				{
					this.cmbOtherCalcPattern.SelectedIndex = 0;
				}
			}

			var setSignAction = this.rbtnOther.IsChecked.Value ? new Action(this.SetOtherCalcPatternSign) : new Action(this.SetRegularCalcPatternSign);
			var getInputValueCntFunc = this.rbtnOther.IsChecked.Value ? new Func<int>(GetInputValueCntOnOtherCalcPattern) : new Func<int>(this.GetInputValueCntOnRegularCalcPattern);

			this.OnCalcPatternChanged(setSignAction, getInputValueCntFunc);
		}
		#endregion

		#region combo boxes
		private void cmbOtherCalcPattern_SelectedIndexChanged(object sender, RoutedEventArgs e)
		{
			this.OnCalcPatternChanged(this.SetOtherCalcPatternSign, this.GetInputValueCntOnOtherCalcPattern);
		}
		#endregion

		#region text boxes
		private void txtValue_TextChanged(object sender, RoutedEventArgs e)
		{
			this.txtBlockResult.Text = string.Empty;
		}

		private void txtValue_Validating(object sender, CancelEventArgs e)
		{
			TextBox txt = (TextBox)sender;
			this.Validate(txt);
		}
		#endregion
		#endregion

		#region private methods
		private void Initialize()
		{
			this.cmbOtherCalcPattern.SelectedIndex = -1;
			this.txtValue1.Text = string.Empty;
			this.txtValue2.Text = string.Empty;
			this.rbtnAddition.IsChecked = true;
		}

		private void OnCalcPatternChanged(Action setSignAction, Func<int> getInputValueCntFunc)
		{
			// 算術記号設定
			setSignAction();

			// テキストボックス活性制御
			this.EnableTextBoxes(getInputValueCntFunc());
			this.Validate();

			this.txtBlockResult.Text = string.Empty;
		}

		private void SetRegularCalcPatternSign()
		{
			if (this.rbtnAddition.IsChecked.Value)
			{
				this.txtBlockCalcSign1.Text = string.Empty;
				this.txtBlockCalcSign2.Text = AdditionSign;
			}
			else if (this.rbtnSubtraction.IsChecked.Value)
			{
				this.txtBlockCalcSign1.Text = string.Empty;
				this.txtBlockCalcSign2.Text = SubtractionSign;
			}
			else if (this.rbtnMultiplication.IsChecked.Value)
			{
				this.txtBlockCalcSign1.Text = string.Empty;
				this.txtBlockCalcSign2.Text = MultiplicationSign;
			}
			else if (this.rbtnDivision.IsChecked.Value)
			{
				this.txtBlockCalcSign1.Text = string.Empty;
				this.txtBlockCalcSign2.Text = DivisionSign;
			}
		}

		private void SetOtherCalcPatternSign()
		{
			var otherCalcPattern = (OtherCalcPattern)(this.cmbOtherCalcPattern.SelectedIndex);
			switch (otherCalcPattern)
			{
				case OtherCalcPattern.Power:
					this.txtBlockCalcSign1.Text = string.Empty;
					this.txtBlockCalcSign2.Text = PowerSign;
					break;
				case OtherCalcPattern.Sine:
					this.txtBlockCalcSign1.Text = SineSign + "(";
					this.txtBlockCalcSign2.Text = "°)";
					break;
				case OtherCalcPattern.Cosine:
					this.txtBlockCalcSign1.Text = CosineSign + "(";
					this.txtBlockCalcSign2.Text = "°)";
					break;
			}
		}

		private int GetInputValueCntOnRegularCalcPattern()
		{
			return 2;
		}

		private int GetInputValueCntOnOtherCalcPattern()
		{
			var otherCalcPattern = (OtherCalcPattern)(this.cmbOtherCalcPattern.SelectedIndex);
			int ret = 0;
			switch (otherCalcPattern)
			{
				case OtherCalcPattern.Power:
					ret = 2;
					break;
				case OtherCalcPattern.Sine:
				case OtherCalcPattern.Cosine:
					ret = 1;
					break;
			}
			return ret;
		}

		private void EnableTextBoxes(int count)
		{
			this.valueTextBoxes.Take(count).ToList().ForEach(x => x.IsEnabled = true);
			this.valueTextBoxes.Skip(count).ToList().ForEach(x => { x.IsEnabled = false; x.Text = string.Empty; });
		}

		private void Validate(TextBox targetTxt = null)
		{
			var targets = (targetTxt == null) ? this.InputtableTextBoxes : new TextBox[] { targetTxt };

			foreach (var txt in targets)
			{
				if (string.IsNullOrEmpty(txt.Text) || float.TryParse(txt.Text, out var _))
				{
					// エラー情報クリア
					//this.errorProvider.SetError(txt, string.Empty);
				}
				else
				{
					// エラー情報設定
					//this.errorProvider.SetError(txt, NotNumericErrorMessage);
				}
			}

			// 入力可能なテキストボックス全てに計算可能な値が入力されている場合は計算ボタンを活性化
			//this.btnCalc.IsEnabled = this.InputtableTextBoxes.ToList().TrueForAll(x => x.Text.Length > 0 && this.errorProvider.GetError(x) == string.Empty);
		}

		private void ControlAllEnabledProperty(bool isCalculating)
		{
			this.rbtnAddition.IsEnabled = !isCalculating;
			this.rbtnSubtraction.IsEnabled = !isCalculating;
			this.rbtnMultiplication.IsEnabled = !isCalculating;
			this.rbtnDivision.IsEnabled = !isCalculating;
			this.rbtnOther.IsEnabled = !isCalculating;
			this.cmbOtherCalcPattern.IsEnabled = isCalculating ? false : this.rbtnOther.IsChecked.Value;

			this.valueTextBoxes.ForEach(x => x.IsReadOnly = isCalculating);
			this.btnCalc.IsEnabled = !isCalculating;
			this.btnClear.IsEnabled = !isCalculating;
			this.btnCancel.IsEnabled = isCalculating;
		}

		private CalculateType? DetermineCalculateType()
		{
			CalculateType? calcType = null;
			if (this.rbtnAddition.IsChecked.Value)
			{
				calcType = CalculateType.Addition;
			}
			else if (this.rbtnSubtraction.IsChecked.Value)
			{
				calcType = CalculateType.Subtraction;
			}
			else if (this.rbtnMultiplication.IsChecked.Value)
			{
				calcType = CalculateType.Multiplication;
			}
			else if (this.rbtnDivision.IsChecked.Value)
			{
				calcType = CalculateType.Division;
			}
			else if (this.rbtnOther.IsChecked.Value)
			{
				switch ((OtherCalcPattern)this.cmbOtherCalcPattern.SelectedIndex)
				{
					case OtherCalcPattern.Power:
						calcType = CalculateType.Power;
						break;
					case OtherCalcPattern.Sine:
						calcType = CalculateType.Sine;
						break;
					case OtherCalcPattern.Cosine:
						calcType = CalculateType.Cosine;
						break;
				}
			}

			return calcType;
		}

		private Task<bool> CancelCalculationAsync(bool isClosingWindow = false)
		{
			Task waitTask = Task.CompletedTask;
			if (this.biz.IsCalculating)
			{
				// 確認メッセージ表示
				if (MessageBox.Show(CofirmCancelingMessage, this.Title, MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
				{
					this.txtBlockStatusBar.Text = CancelingCalcMessage;
					// キャンセル処理が一瞬で終わるとステータスバーのメッセージを視認できないので待機用のタスク生成
					waitTask = Task.Delay(1000);
				}
				else
				{
					return Task.FromResult(false);
				}
			}
			else
			{
				return Task.FromResult(true);
			}

			var cancelTask = Task.Run(() =>
			{
				this.biz.CancelCalculation();
				if (!isClosingWindow)
				{
					// UIスレッドでコントロール活性制御
					this.Dispatcher.BeginInvoke(new Action(() => this.ControlAllEnabledProperty(false)));
				}
			});

			//　キャンセル処理と待機処理両方が完了後にステータスバーのメッセージをクリアするタスクを返却
			var task = Task.WhenAll(cancelTask, waitTask);
			if (!isClosingWindow)
			{
				task = task.ContinueWith(_ => this.Dispatcher.BeginInvoke(new Action(() => this.txtBlockStatusBar.Text = string.Empty)));
			}
			return task.ContinueWith(_ => true);

		}
		#endregion
	}
}
