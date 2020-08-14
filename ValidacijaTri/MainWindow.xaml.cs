using FluentValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ValidacijaTri
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			DataContext = new Osoba();

			//Osoba o = new Osoba();
			//o.Ime = "P";
			//MessageBox.Show(o["Ime"]);
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			(DataContext as Osoba).Foo();
		}
	}

	public class Osoba  : IDataErrorInfo
	{
		public string Ime { get; set; }
		public string Prezime { get; set; }
		public int Starost { get; set; }

		private OsobaValidator Validator;// = new OsobaValidator();

		public void Foo()
		{ 
			var rez = Validator.Validate(this);

			if (rez.IsValid)
			{
				MessageBox.Show("Unos ok!");
			} else
			{
				foreach (var err in rez.Errors)
					MessageBox.Show($"{err.PropertyName} -- greska {err.ErrorMessage}");
			}
		} 

		//Ne koristi se u wpfu vecinom
		public string Error => throw new NotImplementedException();

		public string this[string columnName]
		{
			get
			{
				if (Validator == null)
				{
					Validator = new OsobaValidator();
					return null;
				}
				var rez = Validator.Validate(this);
				
				foreach (var err in rez.Errors)
				{
					if (err.PropertyName == columnName)
						return err.ErrorMessage;
				}
				return null;
			}
		}

		/*public string this[string columnName]
		{
			get
			{
				switch(columnName)
				{
					case "Ime":
						if (Ime != null && Ime.Length < 3)
							return "Premalo";
						break;
					case "Starost":
						if (Starost < 1)
							return "Ne ide :)";
						break;
				}
				return null;
			}
		}*/
	}

	public class OsobaValidator : AbstractValidator<Osoba>
	{
		public OsobaValidator()
		{
			RuleFor(o => o.Ime).NotNull().WithMessage("Null je :(")
								.NotEmpty().WithMessage("Prazno je")
								.MinimumLength(3).WithMessage("Premaleno");
			RuleFor(o => o.Prezime).Must(p => Provera(p)).WithMessage("Lose");
			RuleFor(o => o.Starost).InclusiveBetween(1, 150).WithMessage("Jok");
		}

		public bool Provera(string s) => !string.IsNullOrEmpty(s);
	}

	public class ImeValidator : ValidationRule
	{
		public override ValidationResult Validate(object value, CultureInfo cultureInfo)
		{
			if (value is string s && s != null && s.Length >= 3)
				return ValidationResult.ValidResult;
			else
				return new ValidationResult(false, "Nesto nije ok");
		}
	}
}
