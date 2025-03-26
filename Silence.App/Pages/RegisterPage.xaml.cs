//Реализация логики поведения на страничке
using Silence.Infrastructure.ViewModels;

namespace Silence.App.Pages;

public partial class RegisterPage : ContentPage
{
	public RegisterPage()
	{
		InitializeComponent();
        
		BindingContext = MauiProgram.Services.GetService<RegisterViewModel>();
    }
}