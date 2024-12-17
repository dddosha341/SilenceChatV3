using Silence.Infrastructure.ViewModels;

namespace Silence.App.Pages;

public partial class LoginPage : ContentPage
{
	public LoginPage()
	{
		InitializeComponent();

        BindingContext = MauiProgram.Services.GetService<LoginViewModel>();
    }
}