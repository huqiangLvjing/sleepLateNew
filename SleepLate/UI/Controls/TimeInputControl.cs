using SleepLate.App;
using SleepLate.Utils;

namespace SleepLate.UI.Controls;

// Time input control with increment/decrement buttons
public class TimeInputControl : UserControl
{
    private readonly Logger _logger = Logger.Instance;

    private NumericUpDown _input = null!;
    private Button _decreaseBtn = null!;
    private Button _increaseBtn = null!;
    private Label _unitLabel = null!;

    public int Value
    {
        get => (int)_input.Value;
        set => _input.Value = value;
    }

    public int Minimum
    {
        get => (int)_input.Minimum;
        set => _input.Minimum = value;
    }

    public int Maximum
    {
        get => (int)_input.Maximum;
        set => _input.Maximum = value;
    }

    public int Step { get; set; } = 1;

    public string Unit
    {
        get => _unitLabel.Text;
        set => _unitLabel.Text = value;
    }

    public event EventHandler? ValueChanged;

    public TimeInputControl()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        Size = new System.Drawing.Size(120, 80);

        _decreaseBtn = new Button
        {
            Text = "-",
            Font = new System.Drawing.Font("微软雅黑", 14F),
            Location = new System.Drawing.Point(0, 0),
            Size = new System.Drawing.Size(35, 35),
            BackColor = System.Drawing.Color.FromArgb(22, 119, 255),
            ForeColor = System.Drawing.Color.White,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };

        _input = new NumericUpDown
        {
            Value = 30,
            Minimum = 0,
            Maximum = 999,
            Font = new System.Drawing.Font("微软雅黑", 16F),
            TextAlign = HorizontalAlignment.Center,
            Location = new System.Drawing.Point(40, 0),
            Size = new System.Drawing.Size(50, 35),
            BorderStyle = BorderStyle.FixedSingle
        };

        _increaseBtn = new Button
        {
            Text = "+",
            Font = new System.Drawing.Font("微软雅黑", 14F),
            Location = new System.Drawing.Point(95, 0),
            Size = new System.Drawing.Size(35, 35),
            BackColor = System.Drawing.Color.FromArgb(22, 119, 255),
            ForeColor = System.Drawing.Color.White,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };

        _unitLabel = new Label
        {
            Text = "分钟",
            Font = new System.Drawing.Font("微软雅黑", 9F),
            TextAlign = ContentAlignment.TopCenter,
            Location = new System.Drawing.Point(40, 40),
            Size = new System.Drawing.Size(50, 20)
        };

        _decreaseBtn.Click += (s, e) => ChangeValue(-Step);
        _increaseBtn.Click += (s, e) => ChangeValue(Step);
        _input.ValueChanged += (s, e) => ValueChanged?.Invoke(this, EventArgs.Empty);

        Controls.AddRange(new Control[] { _decreaseBtn, _input, _increaseBtn, _unitLabel });
    }

    private void ChangeValue(int delta)
    {
        var newValue = _input.Value + delta;
        if (newValue >= _input.Minimum && newValue <= _input.Maximum)
        {
            _input.Value = newValue;
            _logger.Debug("TimeInputControl", $"值改变: {newValue}");
        }
    }
}